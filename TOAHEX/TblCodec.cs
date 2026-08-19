using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TOAHEX
{
    /// <summary>
    /// 汉化补丁码表（new_patched.tbl）编解码器。
    /// 码表格式：每行 "HEX码=字符"，如 30=0（单字节）、8145=・（双字节）、81498148=⁉（四字节组合码）。
    /// 解码按最长匹配优先；未收录时回退 Shift-JIS（CP932）；仍失败用 U+FFFD 替换。
    /// </summary>
    public static class TblCodec
    {
        private static readonly object _lock = new object();
        private static bool _initialized;
        private static Dictionary<string, string> _decodeTable;  // 正查表：大写HEX串 -> 字符（可能为多字符组合）
        private static Dictionary<string, byte[]> _encodeTable; // 反查表：字符 -> 最短编码
        private static int _maxCodeBytes;   // 码表中最长编码的字节数（决定解码尝试上限）
        private static int _maxValueChars;  // 码表中最长值的字符数（决定编码贪心尝试上限）
        private static int _entryCount;
        private static string _loadError;

        private static readonly Encoding ShiftJis = Encoding.GetEncoding(932);

        /// <summary>码表是否成功加载（至少含一条有效条目）</summary>
        public static bool IsLoaded
        {
            get { EnsureLoaded(); return _decodeTable != null && _decodeTable.Count > 0; }
        }

        /// <summary>已加载的码表条目数（未加载时为 0）</summary>
        public static int EntryCount
        {
            get { EnsureLoaded(); return _entryCount; }
        }

        /// <summary>加载状态描述（双语），用于状态栏等提示</summary>
        public static string StatusText
        {
            get
            {
                EnsureLoaded();
                bool jp = LanguageConfig.Current == Language.JP;
                if (_decodeTable != null && _decodeTable.Count > 0)
                    return jp ? string.Format("コード表読込済み（{0}件）", _entryCount)
                              : string.Format("码表已加载（{0} 条）", _entryCount);
                if (_loadError != null)
                    return jp ? "コード表未読み込み" : "码表未加载";
                return jp ? "コード表未読み込み" : "码表未加载";
            }
        }

        private static void EnsureLoaded()
        {
            if (_initialized) return;
            lock (_lock)
            {
                if (_initialized) return;
                Load();
                _initialized = true;
            }
        }

        private static void Load()
        {
            try
            {
                // 码表已内嵌到 exe 资源（TOAHEX.new_patched.tbl），不再依赖外置文件
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                string[] names = asm.GetManifestResourceNames();
                string resName = Array.Find(names, n => n.EndsWith("new_patched.tbl", StringComparison.OrdinalIgnoreCase));
                if (resName == null) { _loadError = "embedded resource not found"; return; }

                var lines = new List<string>();
                using (Stream stream = asm.GetManifestResourceStream(resName))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null) break;
                        lines.Add(line);
                    }
                }

                _decodeTable = new Dictionary<string, string>();
                _encodeTable = new Dictionary<string, byte[]>();
                _maxCodeBytes = 0;
                _maxValueChars = 0;
                _entryCount = 0;

                foreach (string raw in lines)
                {
                    // ReadAllLines 已去除行尾换行；值可能本身是空格（如 20= ），整行不可再 Trim
                    string line = raw;
                    if (line.Length == 0) continue;
                    string probe = line.Trim();
                    if (probe.Length == 0) continue;
                    // 容错：跳过常见注释行
                    if (probe[0] == ';' || probe[0] == '#' || probe.StartsWith("//", StringComparison.Ordinal)) continue;

                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue; // 格式错行
                    string hex = line.Substring(0, eq).Trim();
                    string value = line.Substring(eq + 1); // 保留尾部空格等原样值
                    byte[] code;
                    if (!TryHexToBytes(hex, out code)) continue;
                    if (value.Length == 0) continue;

                    // 正查表：重复码保留首条
                    string key = hex.ToUpperInvariant();
                    if (!_decodeTable.ContainsKey(key))
                        _decodeTable[key] = value;

                    // 反查表：同一值多码时取最短码
                    byte[] existing;
                    if (!_encodeTable.TryGetValue(value, out existing) || code.Length < existing.Length)
                        _encodeTable[value] = code;

                    if (code.Length > _maxCodeBytes) _maxCodeBytes = code.Length;
                    if (value.Length > _maxValueChars) _maxValueChars = value.Length;
                    _entryCount++;
                }
            }
            catch (Exception ex)
            {
                _loadError = ex.Message;
            }
        }

        /// <summary>
        /// 按码表解码：从首个 0x00 截断后，逐位置最长匹配优先；
        /// 未收录时回退 Shift-JIS 双字节/单字节（可逆校验），仍失败用 U+FFFD 替换。
        /// </summary>
        public static string Decode(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;
            int len = Array.IndexOf(data, (byte)0);
            if (len < 0) len = data.Length;
            if (len == 0) return string.Empty;

            EnsureLoaded();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; )
            {
                // 1) 码表最长优先（如 81498148=⁉ 优先于 8149=！）
                bool matched = false;
                if (_decodeTable != null && _decodeTable.Count > 0)
                {
                    int maxTry = Math.Min(_maxCodeBytes, len - i);
                    for (int take = maxTry; take >= 1; take--)
                    {
                        string value;
                        if (_decodeTable.TryGetValue(BytesToHex(data, i, take), out value))
                        {
                            sb.Append(value);
                            i += take;
                            matched = true;
                            break;
                        }
                    }
                }
                if (matched) continue;

                // 2) 回退 Shift-JIS：双字节优先（首字节 >= 0x81 视为可能的双字节序列）
                if (i + 1 < len && data[i] >= 0x81)
                {
                    string two;
                    if (TrySjisDecode(data, i, 2, out two))
                    {
                        sb.Append(two);
                        i += 2;
                        continue;
                    }
                }
                string one;
                if (TrySjisDecode(data, i, 1, out one))
                {
                    sb.Append(one);
                }
                else
                {
                    sb.Append('\uFFFD');
                }
                i += 1;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 按码表编码：对输入文本贪心最长匹配；无法编码的字符收集进 invalidChars（去重），
        /// 返回已成功部分的编码字节（调用方应先检查 invalidChars 是否为空）。
        /// </summary>
        public static byte[] Encode(string text, out List<string> invalidChars)
        {
            invalidChars = new List<string>();
            if (string.IsNullOrEmpty(text)) return new byte[0];

            EnsureLoaded();
            List<byte> output = new List<byte>();
            for (int i = 0; i < text.Length; )
            {
                bool matched = false;
                if (_encodeTable != null && _encodeTable.Count > 0)
                {
                    int maxTry = Math.Min(_maxValueChars, text.Length - i);
                    for (int take = maxTry; take >= 1; take--)
                    {
                        byte[] code;
                        if (_encodeTable.TryGetValue(text.Substring(i, take), out code))
                        {
                            output.AddRange(code);
                            i += take;
                            matched = true;
                            break;
                        }
                    }
                }
                if (!matched)
                {
                    // 记录全部非法字符（去重）并跳过继续，便于一次性报告
                    string bad = text.Substring(i, 1);
                    if (!invalidChars.Contains(bad)) invalidChars.Add(bad);
                    i++;
                }
            }
            return output.ToArray();
        }

        // Shift-JIS 解码并做可逆校验（往返一致才算有效，避免乱码被静默接受）
        private static bool TrySjisDecode(byte[] data, int offset, int count, out string decoded)
        {
            decoded = null;
            try
            {
                string s = ShiftJis.GetString(data, offset, count);
                if (string.IsNullOrEmpty(s) || s.IndexOf('\uFFFD') >= 0) return false;
                byte[] re = ShiftJis.GetBytes(s);
                if (re.Length != count) return false;
                for (int i = 0; i < count; i++)
                    if (re[i] != data[offset + i]) return false;
                decoded = s;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string BytesToHex(byte[] data, int offset, int count)
        {
            char[] chars = new char[count * 2];
            for (int i = 0; i < count; i++)
            {
                byte b = data[offset + i];
                chars[i * 2] = HexDigit(b >> 4);
                chars[i * 2 + 1] = HexDigit(b & 0xF);
            }
            return new string(chars);
        }

        private static char HexDigit(int v)
        {
            return v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }

        private static bool TryHexToBytes(string hex, out byte[] bytes)
        {
            bytes = null;
            if (hex.Length == 0 || hex.Length % 2 != 0) return false;
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                int hi = HexValue(hex[i * 2]);
                int lo = HexValue(hex[i * 2 + 1]);
                if (hi < 0 || lo < 0) return false;
                result[i] = (byte)((hi << 4) | lo);
            }
            bytes = result;
            return true;
        }

        private static int HexValue(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            return -1;
        }
    }
}
