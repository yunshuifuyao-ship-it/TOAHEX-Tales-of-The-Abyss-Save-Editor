using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TOAHEX
{
    public class ConvertPs2Form : Form
    {
        private TextBox txtMainPath;
        private TextBox txtSysPath;
        private TextBox txtOutput;
        private Button btnBrowseMain;
        private Button btnBrowseSys;
        private Button btnBrowseDir;
        private CheckBox chkOutputMain;
        private CheckBox chkOutputSys;
        private Label lblSlot;
        private Button btnConvert;
        private Label lblResult;

        public ConvertPs2Form(string prefillMain = null, string prefillSys = null, Icon icon = null)
        {
            Text = LangText("PS2 → 3DS 存档转换", "PS2 → 3DS セーブ変換");
            AutoScaleMode = AutoScaleMode.None;
            // 用 ClientSize 而非 Size，保证按钮等控件相对客户区定位、绝不越出窗口
            // 2026-09-03：整体缩小布局，并支持文件拖入输入框
            ClientSize = new Size(524, 332);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            if (icon != null) this.Icon = icon;

            // === 输入区 GroupBox（整合三项输入、两个复选框、自动槽位识别）===
            var grpInput = new GroupBox();
            grpInput.Text = LangText("存档文件与输出设置（可直接拖入文件）", "セーブデータと出力設定（ドラッグ＆ドロップ対応）");
            grpInput.Location = new Point(12, 12);
            grpInput.Size = new Size(500, 164);
            Controls.Add(grpInput);

            int labelW = 104;
            int pathX = 116;
            int pathW = 286;
            int btnX = 408;
            int btnW = 80;
            int rowH = 28;
            int startY = 24;

            // 主存档
            var lblMain = new Label();
            lblMain.Text = LangText("PS2 主存档：", "PS2 メインセーブ：");
            lblMain.Location = new Point(10, startY + 3);
            lblMain.Size = new Size(labelW, 20);
            lblMain.AutoSize = false;
            lblMain.TextAlign = ContentAlignment.MiddleRight;
            grpInput.Controls.Add(lblMain);

            txtMainPath = new TextBox();
            txtMainPath.Location = new Point(pathX, startY);
            txtMainPath.Size = new Size(pathW, 22);
            txtMainPath.Text = prefillMain ?? string.Empty;
            txtMainPath.TextChanged += (s, e) => UpdateSlotLabel();
            grpInput.Controls.Add(txtMainPath);

            btnBrowseMain = new Button();
            btnBrowseMain.Text = LangText("浏览…", "参照…");
            btnBrowseMain.Location = new Point(btnX, startY - 1);
            btnBrowseMain.Size = new Size(btnW, 24);
            btnBrowseMain.Click += BtnBrowseMain_Click;
            grpInput.Controls.Add(btnBrowseMain);

            // 系统存档
            int y2 = startY + rowH;
            var lblSys = new Label();
            lblSys.Text = LangText("PS2 系统存档：", "PS2 システムセーブ：");
            lblSys.Location = new Point(10, y2 + 3);
            lblSys.Size = new Size(labelW, 20);
            lblSys.AutoSize = false;
            lblSys.TextAlign = ContentAlignment.MiddleRight;
            grpInput.Controls.Add(lblSys);

            txtSysPath = new TextBox();
            txtSysPath.Location = new Point(pathX, y2);
            txtSysPath.Size = new Size(pathW, 22);
            txtSysPath.Text = prefillSys ?? string.Empty;
            grpInput.Controls.Add(txtSysPath);

            btnBrowseSys = new Button();
            btnBrowseSys.Text = LangText("浏览…", "参照…");
            btnBrowseSys.Location = new Point(btnX, y2 - 1);
            btnBrowseSys.Size = new Size(btnW, 24);
            btnBrowseSys.Click += BtnBrowseSys_Click;
            grpInput.Controls.Add(btnBrowseSys);

            // 输出目录
            int y3 = startY + rowH * 2;
            var lblOutput = new Label();
            lblOutput.Text = LangText("输出目录：", "出力フォルダ：");
            lblOutput.Location = new Point(10, y3 + 3);
            lblOutput.Size = new Size(labelW, 20);
            lblOutput.AutoSize = false;
            lblOutput.TextAlign = ContentAlignment.MiddleRight;
            grpInput.Controls.Add(lblOutput);

            txtOutput = new TextBox();
            txtOutput.Location = new Point(pathX, y3);
            txtOutput.Size = new Size(pathW, 22);
            grpInput.Controls.Add(txtOutput);

            btnBrowseDir = new Button();
            btnBrowseDir.Text = LangText("浏览…", "参照…");
            btnBrowseDir.Location = new Point(btnX, y3 - 1);
            btnBrowseDir.Size = new Size(btnW, 24);
            btnBrowseDir.Click += BtnBrowseDir_Click;
            grpInput.Controls.Add(btnBrowseDir);

            // 输出选项复选框（横向并排）
            int y4 = startY + rowH * 3 + 2;
            chkOutputMain = new CheckBox();
            chkOutputMain.Text = LangText("输出游戏存档", "メインセーブを出力");
            chkOutputMain.Location = new Point(pathX, y4);
            chkOutputMain.Size = new Size(140, 22);
            chkOutputMain.AutoSize = false;
            chkOutputMain.Checked = true;
            grpInput.Controls.Add(chkOutputMain);

            chkOutputSys = new CheckBox();
            chkOutputSys.Text = LangText("输出系统存档", "システムセーブを出力");
            chkOutputSys.Location = new Point(pathX + 148, y4);
            chkOutputSys.Size = new Size(160, 22);
            chkOutputSys.AutoSize = false;
            chkOutputSys.Checked = true;
            grpInput.Controls.Add(chkOutputSys);

            // 槽位识别行
            int y5 = startY + rowH * 4 + 2;
            lblSlot = new Label();
            lblSlot.Location = new Point(pathX, y5);
            lblSlot.Size = new Size(pathW + btnW + 6, 18);
            lblSlot.AutoSize = false;
            lblSlot.ForeColor = Color.DimGray;
            grpInput.Controls.Add(lblSlot);
            UpdateSlotLabel();

            // === 结果区 ===
            lblResult = new Label();
            lblResult.Location = new Point(12, 182);
            lblResult.Size = new Size(500, 100);
            lblResult.AutoSize = false;
            Controls.Add(lblResult);

            // === 按钮区（底部居右，仅保留“开始转换”；关闭用标题栏 X）===
            btnConvert = new Button();
            btnConvert.Text = LangText("开始转换", "変換開始");
            btnConvert.Size = new Size(120, 30);
            btnConvert.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnConvert.Location = new Point(ClientSize.Width - 132, ClientSize.Height - 42);
            btnConvert.Click += BtnConvert_Click;
            Controls.Add(btnConvert);
            AcceptButton = btnConvert;

            // === 拖放支持：三个输入框 + 整个窗口 ===
            EnableDropTarget(txtMainPath);
            EnableDropTarget(txtSysPath);
            EnableDropTarget(txtOutput);
            AllowDrop = true;
            DragEnter += FormDragEnter;
            DragDrop += FormDragDrop;
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }

        // ===== 拖放支持 =====

        private void EnableDropTarget(TextBox box)
        {
            box.AllowDrop = true;
            box.DragEnter += FormDragEnter;
            box.DragDrop += (s, e) => AssignDroppedToBox(box, e);
        }

        private void FormDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void FormDragDrop(object sender, DragEventArgs e)
        {
            DistributeDroppedItems(GetDropPaths(e));
        }

        /// <summary>拖到具体输入框上：按目标框直接赋值（目录→输出框）。</summary>
        private void AssignDroppedToBox(TextBox box, DragEventArgs e)
        {
            string[] paths = GetDropPaths(e);
            if (paths.Length == 0) return;
            if (box == txtOutput)
            {
                AssignOutput(paths);
                return;
            }
            // 主/系统框：若只拖入一项则按目标框赋值；拖多项时按类型自动分配
            if (paths.Length > 1)
            {
                DistributeDroppedItems(paths);
                return;
            }
            string p = paths[0];
            if (box == txtMainPath && IsDirectory(p))
            {
                // 主存档框不接受目录，转作输出目录
                AssignOutput(paths);
                return;
            }
            box.Text = p;
            if (string.IsNullOrWhiteSpace(txtOutput.Text) && !IsDirectory(p))
                txtOutput.Text = Path.GetDirectoryName(p);
        }

        /// <summary>拖到窗口空白处：按文件大小自动分配（49096=主存档，1832=系统存档，目录=输出）。</summary>
        private void DistributeDroppedItems(string[] paths)
        {
            foreach (string p in paths)
            {
                if (IsDirectory(p))
                {
                    txtOutput.Text = p;
                    continue;
                }
                try
                {
                    long len = new FileInfo(p).Length;
                    if (len == Ps2To3dsConverter.Ps2MainSize)
                        txtMainPath.Text = p;
                    else if (len == Ps2To3dsConverter.Ps2SysSize)
                        txtSysPath.Text = p;
                    else if (Path.GetFileNameWithoutExtension(p).IndexOf("TOA", StringComparison.OrdinalIgnoreCase) >= 0)
                        txtMainPath.Text = p;
                    else if (Path.GetFileNameWithoutExtension(p).IndexOf("SYS", StringComparison.OrdinalIgnoreCase) >= 0)
                        txtSysPath.Text = p;
                    else
                        txtMainPath.Text = p;
                }
                catch
                {
                    txtMainPath.Text = p;
                }
            }
            // 输出目录为空时默认取主存档所在目录
            if (string.IsNullOrWhiteSpace(txtOutput.Text) && !string.IsNullOrWhiteSpace(txtMainPath.Text))
                txtOutput.Text = Path.GetDirectoryName(txtMainPath.Text);
        }

        private void AssignOutput(string[] paths)
        {
            foreach (string p in paths)
            {
                if (IsDirectory(p)) { txtOutput.Text = p; return; }
            }
            if (paths.Length > 0)
                txtOutput.Text = Path.GetDirectoryName(paths[0]);
        }

        private static bool IsDirectory(string p)
        {
            return Directory.Exists(p);
        }

        private static string[] GetDropPaths(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                return (string[])e.Data.GetData(DataFormats.FileDrop);
            return new string[0];
        }

        /// <summary>
        /// 从 PS2 主存档文件名识别 3DS 槽位名：
        /// BISLPS-25586_TOA_001（或任意 *_TOA_数字、直接 TOA_数字）→ TOA_001；
        /// 无法识别时默认 TOA_000。
        /// </summary>
        private static string DetectSlotName(string path)
        {
            string name = Path.GetFileNameWithoutExtension((path ?? string.Empty).Trim());
            if (name.Length > 4 && name.StartsWith("TOA_", StringComparison.OrdinalIgnoreCase))
            {
                int slot;
                if (int.TryParse(name.Substring(4), out slot) && slot >= 0 && slot <= 999)
                    return string.Format("TOA_{0:D3}", slot);
            }
            int idx = name.LastIndexOf("_TOA_", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                int slot;
                if (int.TryParse(name.Substring(idx + 5), out slot) && slot >= 0 && slot <= 999)
                    return string.Format("TOA_{0:D3}", slot);
            }
            return "TOA_000";
        }

        private void UpdateSlotLabel()
        {
            if (string.IsNullOrWhiteSpace(txtMainPath.Text))
            {
                lblSlot.Text = LangText("识别槽位：—（未选择主存档）", "スロット：—（メインセーブ未選択）");
                return;
            }
            lblSlot.Text = string.Format(LangText("识别槽位：{0}（输出 {0}\\{0}）", "スロット：{0}（{0}\\{0} を出力）"),
                DetectSlotName(txtMainPath.Text));
        }

        private void BtnBrowseMain_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                // PS2 存档无扩展名，不能用 *.sav 过滤
                dlg.Title = LangText("选择 PS2 主存档", "PS2 メインセーブを選択");
                dlg.Filter = LangText("存档文件|*.*", "セーブファイル|*.*");
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtMainPath.Text = dlg.FileName;
            }
        }

        private void BtnBrowseSys_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = LangText("选择 PS2 系统存档", "PS2 システムセーブを選択");
                dlg.Filter = LangText("存档文件|*.*", "セーブファイル|*.*");
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtSysPath.Text = dlg.FileName;
            }
        }

        private void BtnBrowseDir_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = LangText("选择输出目录", "出力フォルダを選択");
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtOutput.Text = dlg.SelectedPath;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                bool wantMain = chkOutputMain.Checked;
                bool wantSys = chkOutputSys.Checked;

                if (!wantMain && !wantSys)
                {
                    MessageBox.Show(LangText("请至少选择一种输出（游戏存档或系统存档）。",
                        "メインセーブまたはシステムセーブのいずれかを選択してください。"),
                        LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOutput.Text))
                {
                    MessageBox.Show(LangText("请先选择输出目录。", "出力フォルダを先に選択してください。"),
                        LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                byte[] main = null;
                string slotName = null;
                if (wantMain)
                {
                    if (string.IsNullOrWhiteSpace(txtMainPath.Text))
                    {
                        MessageBox.Show(LangText("请先选择 PS2 主存档。", "PS2 メインセーブを先に選択してください。"),
                            LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    byte[] ps2Main = File.ReadAllBytes(txtMainPath.Text);
                    if (ps2Main.Length != Ps2To3dsConverter.Ps2MainSize)
                    {
                        MessageBox.Show(string.Format(
                            LangText("PS2 主存档大小应为 {0} 字节，实际 {1} 字节。", "PS2 メインセーブのサイズは {0} バイトのはずですが、実際は {1} バイトです。"),
                            Ps2To3dsConverter.Ps2MainSize, ps2Main.Length),
                            LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    slotName = DetectSlotName(txtMainPath.Text);
                    main = Ps2To3dsConverter.ConvertMain(ps2Main, Ps2To3dsConverter.LoadEmbeddedTemplate());
                    Ps2To3dsConverter.VerifyMain(main);
                }

                byte[] sys = null;
                if (wantSys)
                {
                    if (string.IsNullOrWhiteSpace(txtSysPath.Text))
                    {
                        MessageBox.Show(LangText("请先选择 PS2 系统存档。", "PS2 システムセーブを先に選択してください。"),
                            LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    byte[] ps2Sys = File.ReadAllBytes(txtSysPath.Text);
                    if (ps2Sys.Length != Ps2To3dsConverter.Ps2SysSize)
                    {
                        MessageBox.Show(string.Format(
                            LangText("PS2 系统存档大小应为 {0} 字节，实际 {1} 字节。", "PS2 システムセーブのサイズは {0} バイトのはずですが、実際は {1} バイトです。"),
                            Ps2To3dsConverter.Ps2SysSize, ps2Sys.Length),
                            LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    sys = Ps2To3dsConverter.ConvertSystem(ps2Sys);
                    Ps2To3dsConverter.VerifySystem(sys);
                }

                string outputDir = txtOutput.Text.Trim();
                var sb = new StringBuilder(LangText("转换成功！\n", "変換成功！\n"));

                if (main != null)
                {
                    string slotDir = Path.Combine(outputDir, slotName);
                    Directory.CreateDirectory(slotDir);
                    File.WriteAllBytes(Path.Combine(slotDir, slotName), main);

                    uint mapId = BitConverter.ToUInt32(main, 0x528);
                    float posX = BitConverter.ToSingle(main, 0x530);
                    float posY = BitConverter.ToSingle(main, 0x534);
                    sb.AppendLine(string.Format(
                        LangText("{0}\\{0}：{1} 字节，校验和通过（地图ID：{2}，X：{3:F2}，Y：{4:F2}）",
                                 "{0}\\{0}：{1} バイト、チェックサムOK（マップID：{2}、X：{3:F2}、Y：{4:F2}）"),
                        slotName, main.Length, mapId, posX, posY));
                }

                if (sys != null)
                {
                    string sysDir = Path.Combine(outputDir, "TOASYS");
                    Directory.CreateDirectory(sysDir);
                    File.WriteAllBytes(Path.Combine(sysDir, "TOASYS"), sys);
                    sb.AppendLine(string.Format(
                        LangText("TOASYS\\TOASYS：{0} 字节，校验和通过", "TOASYS\\TOASYS：{0} バイト、チェックサムOK"),
                        sys.Length));
                }

                lblResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("转换失败：\n{0}", "変換失敗：\n{0}"), ex.Message),
                    LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
