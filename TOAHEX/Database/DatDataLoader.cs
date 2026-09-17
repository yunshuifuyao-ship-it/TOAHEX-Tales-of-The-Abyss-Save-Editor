using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TOAHEX
{
    public static class DatDataLoader
    {
        public static Dictionary<int, string> LoadAcsData(string filePath)
        {
            var parsed = ParseExportFile(filePath);
            var result = new Dictionary<int, string>();
            foreach (var kv in parsed)
            {
                if (kv.Value.TryGetValue("ptr2", out var name) && !string.IsNullOrEmpty(name))
                {
                    result[kv.Key] = name;
                }
            }
            return result;
        }

        public static Dictionary<int, string> LoadSpData(string filePath)
        {
            var parsed = ParseExportFile(filePath);
            var result = new Dictionary<int, string>();
            foreach (var kv in parsed)
            {
                if (kv.Value.TryGetValue("name", out var name) && !string.IsNullOrEmpty(name))
                {
                    result[kv.Key] = name;
                }
            }
            return result;
        }

        public static Dictionary<int, string> LoadItemData(string filePath)
        {
            var parsed = ParseExportFile(filePath);
            var result = new Dictionary<int, string>();
            foreach (var kv in parsed)
            {
                if (kv.Value.TryGetValue("name", out var name))
                {
                    result[kv.Key] = name;
                }
            }
            return result;
        }

        public static Dictionary<int, Dictionary<string, string>> LoadItemFullData(string filePath)
        {
            return ParseExportFile(filePath);
        }

        public static Dictionary<int, string> LoadCkdData(string filePath)
        {
            var parsed = ParseExportFile(filePath);
            var result = new Dictionary<int, string>();
            foreach (var kv in parsed)
            {
                if (kv.Value.TryGetValue("table", out var table) && table == "table1"
                    && kv.Value.TryGetValue("name", out var name) && !string.IsNullOrEmpty(name))
                {
                    result[kv.Key] = name;
                }
            }
            return result;
        }

        private static Dictionary<int, Dictionary<string, string>> ParseExportFile(string filePath)
        {
            var result = new Dictionary<int, Dictionary<string, string>>();

            try
            {
                if (!File.Exists(filePath))
                    return result;

                string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                int currentIndex = -1;
                Dictionary<string, string> currentEntry = null;

                foreach (var rawLine in lines)
                {
                    string line = rawLine.Trim();

                    if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                        continue;

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        string indexStr = line.Substring(1, line.Length - 2);
                        if (int.TryParse(indexStr, out currentIndex))
                        {
                            currentEntry = new Dictionary<string, string>();
                            result[currentIndex] = currentEntry;
                        }
                        continue;
                    }

                    if (currentEntry != null)
                    {
                        int eqIdx = line.IndexOf('=');
                        if (eqIdx > 0)
                        {
                            string key = line.Substring(0, eqIdx);
                            string value = eqIdx + 1 < line.Length ? line.Substring(eqIdx + 1) : "";
                            currentEntry[key] = value;
                        }
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        public static List<string> ValidateData(Dictionary<int, string> data, string typeName)
        {
            var errors = new List<string>();

            if (data == null || data.Count == 0)
            {
                errors.Add(string.Format("{0}: 无数据", typeName));
                return errors;
            }

            foreach (var kv in data)
            {
                if (string.IsNullOrEmpty(kv.Value))
                {
                    errors.Add(string.Format("{0}: ID {1} 名称为空", typeName, kv.Key));
                }
            }

            var ids = data.Keys.OrderBy(k => k).ToList();
            for (int i = 0; i < ids.Count - 1; i++)
            {
                if (ids[i + 1] > ids[i] + 1)
                {
                    errors.Add(string.Format("{0}: ID {1} 到 {2} 之间存在间隙", typeName, ids[i], ids[i + 1]));
                }
            }

            return errors;
        }
    }
}
