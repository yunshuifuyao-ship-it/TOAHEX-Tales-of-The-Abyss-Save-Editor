using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TOAHEX
{
    public partial class MainForm : Form
    {
        private SaveData _saveData;
        private bool _loading;
        private bool _showCombatStats; // 基础/战斗属性面板切换状态（false=显示基础）
        private readonly int[] _lastCharLevel = new int[9]; // 各角色 UI 上次显示的等级（等级联动成长用）
        private ushort[] _arteIds = new ushort[4];
        private uint _currentTitleIndex;
        private Image[] _charPortraits;
        private ItemWheelFilter _itemWheelFilter;

        public MainForm()
        {
            InitializeComponent();
            SetControlsEnabled(false);
            LoadCharPortraits();
            LoadDatData();
            LanguageConfig.LanguageChanged += OnLanguageChanged;
            // 全局捕获滚轮消息：悬停背包"数量"列滚动滚轮直接增减该道具数量
            _itemWheelFilter = new ItemWheelFilter(this);
            Application.AddMessageFilter(_itemWheelFilter);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_itemWheelFilter != null)
                Application.RemoveMessageFilter(_itemWheelFilter);
            base.OnFormClosed(e);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RefreshAllUI();
        }

        private void RefreshAllUI()
        {
            this.Text = LangText("TOAHEX v1.1 - Tales of the Abyss Save Editor", "TOAHEX v1.1 - Tales of the Abyss Save Editor");
            menuFile.Text = LangText("文件", "ファイル");
            menuFileOpen.Text = LangText("打开", "開く");
            menuFileSave.Text = LangText("保存", "保存");
            menuFileExit.Text = LangText("退出", "終了");
            menuLanguage.Text = LangText("语言", "言語");
            menuLangCN.Text = LangText("中文", "中文");
            menuLangJP.Text = LangText("日文", "日本語");
            menuLangCN.Checked = LanguageConfig.Current == Language.CN;
            menuLangJP.Checked = LanguageConfig.Current == Language.JP;
            menuHelp.Text = LangText("帮助", "ヘルプ");
            menuHelpAbout.Text = LangText("关于", "バージョン情報");
            menuTools.Text = LangText("工具", "ツール");
            menuToolsCharName.Text = LangText("更改角色名…", "キャラ名変更…");
            menuToolsConvertPs2.Text = LangText("PS2 → 3DS 存档转换…", "PS2 → 3DS セーブ変換…");
            statusLabel.Text = LangText("未加载存档", "セーブ未読み込み");

            tabGlobal.Text = LangText("全局数据", "全局データ");
            tabCharacter.Text = LangText("角色编辑", "キャラ編集");
            tabItems.Text = LangText("背包管理", "バッグ管理");
            tabCooking.Text = LangText("料理修改", "料理編集");
            tabSystem.Text = LangText("系统数据", "システムデータ");
            tabFSChamber.Text = LangText("谱石管理", "FSチャンバー");

            subTabStats.Text = LangText("角色属性", "キャラステータス");
            subTabEquip.Text = LangText("装备", "装備");
            subTabArtes.Text = LangText("术技", "アーツ");
            subTabADSkill.Text = LangText("附加技能", "追加スキル");
            subTabTitle.Text = LangText("称号", "称号");

            lblItemWheelHint.Text = LangText("悬停数量列滚动滚轮调整数量（Ctrl×10）", "数量列にカーソルを合わせホイールで調整（Ctrl×10）");
            btnAllItemsMax.Text = LangText("所有道具全满", "全アイテム最大");

            if (_saveData != null)
            {
                try { LoadGlobalData(); } catch { }
                try { RefreshCharFields(); } catch { }
                try { BuildItemTable(); ApplyItemFilter(); } catch { }
                try { LoadCookingData(); } catch { }
                try { if (tabFSChamber != null && tabControl.TabPages.Contains(tabFSChamber)) LoadFSChamberData(); } catch { }
                try { LoadToasysData(); } catch { }
            }
        }

        private void menuLangCN_Click(object sender, EventArgs e)
        {
            LanguageConfig.Current = Language.CN;
        }

        private void menuLangJP_Click(object sender, EventArgs e)
        {
            LanguageConfig.Current = Language.JP;
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new Form())
            {
                aboutForm.Text = LangText("关于", "バージョン情報");
                aboutForm.StartPosition = FormStartPosition.CenterParent;
                aboutForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                aboutForm.MaximizeBox = false;
                aboutForm.MinimizeBox = false;
                aboutForm.Size = new Size(380, 300);
                try { aboutForm.Icon = this.Icon; } catch { }

                var picIcon = new PictureBox();
                bool imageLoaded = false;
                try
                {
                    string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icon", "pen.png");
                    if (System.IO.File.Exists(iconPath))
                    {
                        picIcon.Image = Image.FromFile(iconPath);
                        imageLoaded = true;
                    }
                }
                catch { }
                
                if (!imageLoaded)
                {
                    try
                    {
                        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TOAHEX.Icon.pen.png");
                        if (stream != null)
                        {
                            picIcon.Image = Image.FromStream(stream);
                            imageLoaded = true;
                        }
                    }
                    catch { }
                }
                
                if (!imageLoaded)
                {
                    try { picIcon.Image = this.Icon?.ToBitmap(); } catch { }
                }
                picIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                picIcon.Size = new Size(48, 48);
                picIcon.Location = new Point(20, 20);
                aboutForm.Controls.Add(picIcon);

                var lblInfo = new Label();
                lblInfo.Text = LangText(
                    "TOAHEX\nTales of the Abyss Save Editor\n\n作者: 云水扶摇|passerby",
                    "TOAHEX\nTales of the Abyss Save Editor\n\n作者: 云水扶摇|passerby");
                lblInfo.Location = new Point(80, 20);
                lblInfo.Size = new Size(270, 80);
                aboutForm.Controls.Add(lblInfo);

                var lblGithub = new LinkLabel();
                lblGithub.Text = LangText("项目仓库", "プロジェクトリポジトリ");
                lblGithub.Location = new Point(80, 105);
                lblGithub.Size = new Size(270, 20);
                lblGithub.LinkClicked += (s, args) => { try { System.Diagnostics.Process.Start("https://github.com/yunshuifuyao-ship-it/TOAHEX-Tales-of-The-Abyss-Save-Editor"); } catch { } };
                aboutForm.Controls.Add(lblGithub);

                var lblDonate = new LinkLabel();
                lblDonate.Text = LangText("捐赠（爱发电）", "寄付（愛発電）");
                lblDonate.Location = new Point(80, 130);
                lblDonate.Size = new Size(270, 20);
                lblDonate.LinkClicked += (s, args) => { try { System.Diagnostics.Process.Start("https://ifdian.net/a/YunShuifuyao"); } catch { } };
                aboutForm.Controls.Add(lblDonate);

                var btnOk = new Button();
                btnOk.Text = LangText("确定", "OK");
                btnOk.Size = new Size(80, 26);
                btnOk.Location = new Point(140, 220);
                btnOk.DialogResult = DialogResult.OK;
                aboutForm.Controls.Add(btnOk);
                aboutForm.AcceptButton = btnOk;

                aboutForm.ShowDialog(this);
            }
        }

        private void LoadCharPortraits()
        {
            _charPortraits = new Image[7];
            string[] names = { "luke", "tear", "jade", "anise", "guy", "natalia", "asch" };
            for (int i = 0; i < 7; i++)
            {
                try
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "TOAHEX.Icon." + names[i] + ".png");
                    if (stream != null)
                    {
                        _charPortraits[i] = Image.FromStream(stream);
                        continue;
                    }
                }
                catch { }
                try
                {
                    string path = Path.Combine(Application.StartupPath, "Icon", names[i] + ".png");
                    if (File.Exists(path))
                        _charPortraits[i] = Image.FromFile(path);
                }
                catch { }
            }
        }

        private void LoadDatData()
        {
            try
            {
                string datDir = null;
                string tryDir = Path.Combine(Application.StartupPath, "..", "..", "..", "DAT");
                if (Directory.Exists(tryDir)) datDir = Path.GetFullPath(tryDir);
                if (datDir == null)
                {
                    tryDir = Path.Combine(Application.StartupPath, "DAT");
                    if (Directory.Exists(tryDir)) datDir = tryDir;
                }
                if (datDir == null) return;

                string acsFile = Path.Combine(datDir, "_acs_export.txt");
                string spFile = Path.Combine(datDir, "_sp_export.txt");
                string iFile = Path.Combine(datDir, "_i_export.txt");
                string ckdFile = Path.Combine(datDir, "_ckd_export.txt");

                if (File.Exists(acsFile)) ADSkillDatabase.LoadFromDat(acsFile);
                if (File.Exists(spFile)) ArteDatabase.LoadFromDat(spFile);
                if (File.Exists(iFile))
                {
                    ItemDatabase.LoadFromDatFull(iFile);
                }
                if (File.Exists(ckdFile)) CookingDatabase.LoadFromDat(ckdFile);
            }
            catch { }
        }

        private void LoadSaveFile(string filePath)
        {
            var data = new SaveData();
            if (!data.Load(filePath))
            {
                MessageBox.Show(LangText("文件大小不匹配！\n3DS：TOA_XXX应为49120字节，TOASYS应为1860字节。\nPS2：主存档应为49096字节，系统存档应为1832字节。", "ファイルサイズが一致しません！\n3DS：TOA_XXXは49120バイト、TOASYSは1860バイト。\nPS2：メインは49096バイト、システムは1832バイト。"), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saveData = data;
            bool checksumOk = _saveData.VerifyChecksum();
            if (!checksumOk)
            {
                MessageBox.Show(LangText("校验和验证失败！存档可能已损坏或被其他工具修改。\n仍可编辑，但请谨慎操作。", "チェックサム検証失敗！セーブデータが破損しているか、他のツールで変更された可能性があります。\n編集は可能ですが、慎重に操作してください。"), LangText("警告", "警告"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetControlsEnabled(true);
            UpdateStatusBar();
            if (_saveData.Type == SaveType.Toasys)
            {
                if (!tabControl.TabPages.Contains(tabSystem))
                    tabControl.TabPages.Add(tabSystem);
                if (!tabControl.TabPages.Contains(tabSystemKills))
                    tabControl.TabPages.Add(tabSystemKills);
                tabControl.TabPages.Remove(tabGlobal);
                tabControl.TabPages.Remove(tabCharacter);
                tabControl.TabPages.Remove(tabItems);
                tabControl.TabPages.Remove(tabCooking);
                tabControl.TabPages.Remove(tabFSChamber);
                tabControl.TabPages.Remove(tabStoryJump);
                tabControl.SelectedTab = tabSystem;
                try { LoadToasysData(); } catch { }
            }
            else
            {
                if (!tabControl.TabPages.Contains(tabGlobal))
                    tabControl.TabPages.Add(tabGlobal);
                if (!tabControl.TabPages.Contains(tabCharacter))
                    tabControl.TabPages.Add(tabCharacter);
                if (!tabControl.TabPages.Contains(tabItems))
                    tabControl.TabPages.Add(tabItems);
                if (!tabControl.TabPages.Contains(tabCooking))
                    tabControl.TabPages.Add(tabCooking);
                if (!tabControl.TabPages.Contains(tabFSChamber))
                    tabControl.TabPages.Add(tabFSChamber);
                if (!tabControl.TabPages.Contains(tabStoryJump))
                    tabControl.TabPages.Add(tabStoryJump);
                tabControl.TabPages.Remove(tabSystem);
                tabControl.TabPages.Remove(tabSystemKills);
                numGald.Enabled = true;
                numPlayTime.Enabled = true;
                try { LoadGlobalData(); } catch { }
                try { LoadCharacterData(); } catch { }
                try { LoadItemData(); } catch { }
                try { LoadCookingData(); } catch { }
                try { LoadFSChamberData(); } catch { }
            }
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new OpenFileDialog())
                {
                    // TOASB 为游戏备份存档（sub_36C4C0 使用 "TOASB%03d" 命名，格式与 TOA_*/TOASYS 相同，按大小识别）
                    dlg.Filter = LangText("TOA存档文件|TOA_*;TOASYS;TOASB*|所有文件|*.*", "TOAセーブファイル|TOA_*;TOASYS;TOASB*|すべてのファイル|*.*");
                    dlg.Title = LangText("打开存档文件", "セーブファイルを開く");
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    LoadSaveFile(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("打开文件时出错：\n{0}", "ファイルを開く際にエラー：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_saveData == null) return;
                _saveData.Save();
                MessageBox.Show(LangText("保存成功！", "保存成功！"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("保存文件时出错：\n{0}", "保存ファイル時エラー：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                if (_saveData == null) return;
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = LangText("TOA存档文件|*", "TOAセーブファイル|*");
                    dlg.Title = LangText("另存为", "名前を付けて保存");
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _saveData.Save(dlg.FileName);
                        MessageBox.Show(LangText("保存成功！", "保存成功！"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("保存文件时出错：\n{0}", "保存ファイル時エラー：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuToolsConvertPs2_Click(object sender, EventArgs e)
        {
            string prefillMain = null, prefillSys = null;
            if (_saveData != null && _saveData.IsLoaded && _saveData.Platform == Platform.Ps2)
            {
                if (_saveData.Type == SaveType.ToaXxx) prefillMain = _saveData.FilePath;
                else if (_saveData.Type == SaveType.Toasys) prefillSys = _saveData.FilePath;
            }
            using (var form = new ConvertPs2Form(prefillMain, prefillSys, this.Icon))
                form.ShowDialog(this);
        }

        private void menuEditCharName_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx)
            {
                MessageBox.Show(LangText("请先打开 TOA_XXX 存档。", "先にTOA_XXXセーブデータを開いてください。"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 动态构建改名对话框（避免新增窗体文件）
            using (var dlg = new Form())
            {
                dlg.Text = LangText("更改角色名", "キャラ名変更");
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ClientSize = new Size(380, 168);
                try { dlg.Icon = this.Icon; } catch { }

                var lblChar = new Label();
                lblChar.Text = LangText("角色", "キャラ");
                lblChar.Location = new Point(14, 16);
                lblChar.AutoSize = true;
                dlg.Controls.Add(lblChar);

                var cmbChar = new ComboBox();
                cmbChar.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbChar.Location = new Point(100, 12);
                cmbChar.Size = new Size(150, 21);
                for (int i = 1; i <= 7; i++)
                    cmbChar.Items.Add(CharNames[i]);
                // 默认选中角色页当前选中的角色（若有）
                int current = (cmbCharSelect != null && cmbCharSelect.SelectedIndex >= 0 && cmbCharSelect.SelectedIndex < 7)
                    ? cmbCharSelect.SelectedIndex : 0;
                cmbChar.SelectedIndex = current;
                dlg.Controls.Add(cmbChar);

                var lblCurrent = new Label();
                lblCurrent.Text = LangText("当前名称", "現在の名前");
                lblCurrent.Location = new Point(14, 51);
                lblCurrent.AutoSize = true;
                dlg.Controls.Add(lblCurrent);

                var txtCurrent = new TextBox();
                txtCurrent.ReadOnly = true;
                txtCurrent.Location = new Point(100, 47);
                txtCurrent.Size = new Size(250, 21);
                dlg.Controls.Add(txtCurrent);

                var lblNew = new Label();
                lblNew.Text = LangText("新名称", "新しい名前");
                lblNew.Location = new Point(14, 86);
                lblNew.AutoSize = true;
                dlg.Controls.Add(lblNew);

                var txtNew = new TextBox();
                txtNew.Location = new Point(100, 82);
                txtNew.Size = new Size(250, 21);
                txtNew.MaxLength = 15;
                dlg.Controls.Add(txtNew);

                // 随下拉切换刷新当前名称显示
                cmbChar.SelectedIndexChanged += (s2, e2) =>
                {
                    int idx = cmbChar.SelectedIndex + 1;
                    txtCurrent.Text = idx >= 1 && idx <= 7 ? _saveData.ReadCharName(idx) : string.Empty;
                };
                txtCurrent.Text = _saveData.ReadCharName(cmbChar.SelectedIndex + 1);

                var btnOk = new Button();
                btnOk.Text = LangText("确定", "OK");
                btnOk.Location = new Point(180, 124);
                btnOk.Size = new Size(85, 28);
                dlg.Controls.Add(btnOk);
                dlg.AcceptButton = btnOk;

                var btnCancel = new Button();
                btnCancel.Text = LangText("取消", "キャンセル");
                btnCancel.Location = new Point(272, 124);
                btnCancel.Size = new Size(85, 28);
                btnCancel.DialogResult = DialogResult.Cancel;
                dlg.Controls.Add(btnCancel);
                dlg.CancelButton = btnCancel;

                btnOk.Click += (s2, e2) =>
                {
                    int idx = cmbChar.SelectedIndex + 1;
                    string error;
                    if (_saveData.WriteCharName(idx, txtNew.Text.Trim(), out error))
                    {
                        // 名字仅在对话框与 HEAD 摘要中使用，保存时 RebuildHeadSummary 会自动同步，无需刷新其他控件
                        MessageBox.Show(LangText("角色名已更改。", "キャラ名を変更しました。"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.Close();
                    }
                    else
                    {
                        MessageBox.Show(error, LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                dlg.ShowDialog(this);
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    LoadSaveFile(files[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("打开文件时出错：\n{0}", "ファイルを開く際にエラー：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            tabControl.Enabled = enabled;
            menuFileSave.Enabled = enabled;
        }

        private void SetNumericSafe(NumericUpDown num, uint value)
        {
            decimal d = (decimal)value;
            if (d < num.Minimum) d = num.Minimum;
            if (d > num.Maximum) d = num.Maximum;
            num.Value = d;
        }

        private void UpdateStatusBar()
        {
            if (_saveData == null)
            {
                statusLabel.Text = LangText("未加载存档", "セーブ未読み込み");
                return;
            }

            string type = _saveData.Type == SaveType.ToaXxx ? "TOA_XXX" : "TOASYS";
            string platform = _saveData.Platform == Platform.Ps2
                ? LangText("平台：PS2", "プラットフォーム：PS2")
                : LangText("平台：3DS", "プラットフォーム：3DS");
            bool ok = _saveData.VerifyChecksum();
            statusLabel.Text = $"{type} | {platform} | {_saveData.FilePath} | {LangText("校验和", "チェックサム")}: {(ok ? LangText("通过", "OK") : LangText("失败", "NG"))}";
            // 码表未加载时提示文本回退解码方式（已加载则保持克制不提示）
            if (!TblCodec.IsLoaded)
                statusLabel.Text += LangText("（码表未加载，文本按Shift-JIS解码）", "（コード表未読込、テキストはShift-JISでデコード）");
        }

        #region 全局数据页

        private void LoadGlobalData()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            _loading = true;
            try
            {
                SetNumericSafe(numGald, _saveData.Gald);
                SetNumericSafe(numPlayTime, _saveData.PlayTime);
                try { lblVersion.Text = _saveData.Version.ToString("F1"); } catch { lblVersion.Text = "-"; }
                // 0x7D0 为真实难度（0=普通 1=困难 2=狂热 3=未知），非法值显示 "-"
                try
                {
                    int diffVal = _saveData.ReadU8(SaveOffsets.BODY_DIFFICULTY);
                    lblDifficulty.Text = (diffVal >= 0 && diffVal <= 3) ? GetDifficultyName(diffVal) : "-";
                }
                catch { lblDifficulty.Text = "-"; }
                try { lblPartyCount.Text = _saveData.PartyCount.ToString(); } catch { lblPartyCount.Text = "-"; }
                try { lblLocation.Text = _saveData.LocationName; } catch { lblLocation.Text = "-"; }

                try { SetNumericSafe(numEncount, _saveData.ReadU32(SaveOffsets.HEAD_ENCOUNTER)); } catch { }
                try { SetNumericSafe(numHit, _saveData.ReadU32(SaveOffsets.HEAD_HIT)); } catch { }

                // Grade：赌场余额（0xABA4 定点数，游戏显示 floor(/100)），此值是唯一源头。
                try
                {
                    decimal gradeVal = (decimal)_saveData.CasinoGradeDisplay;
                    if (gradeVal < numGrade.Minimum) gradeVal = numGrade.Minimum;
                    if (gradeVal > numGrade.Maximum) gradeVal = numGrade.Maximum;
                    numGrade.Value = gradeVal;
                }
                catch { numGrade.Value = 0; }
                try { SetNumericSafe(numCasinoChips, _saveData.CasinoChips); } catch { }

                try
                {
                    // 功能解锁状态 = 全局 flag 位图教程完成 flag（2026-09-03 修正：旧 0x52C 是地图切换计数，写入无效）
                    chkCCore.Checked = _saveData.ReadGlobalFlag(SaveOffsets.GLOBAL_FLAG_C_CORE);
                    chkFSChamber.Checked = _saveData.ReadGlobalFlag(SaveOffsets.GLOBAL_FLAG_FS_CHAMBER);
                }
                catch { }

                if (cmbPartySlot != null)
                {
                    byte[] partyOrder = _saveData.ReadPartyOrder();
                    for (int i = 0; i < SaveOffsets.BODY_PARTY_ORDER_COUNT && i < cmbPartySlot.Length; i++)
                    {
                        try
                        {
                            int val = partyOrder[i];
                            if (val >= 0 && val < cmbPartySlot[i].Items.Count)
                                cmbPartySlot[i].SelectedIndex = val;
                            else
                                cmbPartySlot[i].SelectedIndex = 0;
                        }
                        catch { cmbPartySlot[i].SelectedIndex = 0; }
                    }
                }

                if (cmbDifficulty != null)
                {
                    try
                    {
                        int diff = _saveData.ReadU8(SaveOffsets.BODY_DIFFICULTY);
                        if (diff < 0 || diff > 3) diff = 0; // 非法值按普通处理
                        cmbDifficulty.SelectedIndex = diff;
                    }
                    catch { cmbDifficulty.SelectedIndex = 0; }
                }

                // 领队：0x7C3 单写即生效（摘要块只覆盖 runtime[0..115]，+1656 超出范围）
                if (cmbLeader != null)
                {
                    try
                    {
                        int leader = _saveData.ReadU8(SaveOffsets.BODY_LEADER);
                        if (leader < 0 || leader > 7) leader = 1; // 非法值按卢克（1-7 均合法，7=阿修，地图模型表含 ash00.npc）
                        cmbLeader.SelectedIndex = leader;
                    }
                    catch { cmbLeader.SelectedIndex = 1; }
                }

            }
            finally
            {
                _loading = false;
            }

            // 刷新剧情跳跃标签页
            try { RefreshStoryJumpTab(); } catch { }
        }

        private void numGald_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            _saveData.Gald = (uint)numGald.Value;
        }

        private void numPlayTime_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            _saveData.PlayTime = (uint)numPlayTime.Value;
        }

        private void numEncount_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            uint encount = (uint)numEncount.Value;
            _saveData.WriteU32(SaveOffsets.HEAD_ENCOUNTER, encount);
            _saveData.WriteU32(SaveOffsets.BODY_ENCOUNTER, encount);
        }

        private void numHit_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            uint hit = (uint)numHit.Value;
            _saveData.WriteU32(SaveOffsets.HEAD_HIT, hit);
            _saveData.WriteU32(SaveOffsets.BODY_HIT, hit);
        }

        private void numGrade_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            // 写 0xABA4（赌场余额定点数，唯一源头，保留小数 0.xx）并同步 var#773 整数缓存。
            // 不再写 0xB080/0xB088（那是战斗 Grade，与赌场余额无关）。
            _saveData.WriteCasinoGrade((uint)numGrade.Value);
        }

        private void numCasinoChips_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            _saveData.CasinoChips = (uint)numCasinoChips.Value;
        }

        private void chkCCore_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            // C·コア(响律符)解锁 = 全局 flag 2007（"新手教程 CC符"完成标志）
            _saveData.WriteGlobalFlag(SaveOffsets.GLOBAL_FLAG_C_CORE, chkCCore.Checked);
        }

        private void chkFSChamber_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            // FSチャンバー(音素质点嵌石)解锁 = 全局 flag 2009（"新手教程 FS嵌石"完成标志）
            _saveData.WriteGlobalFlag(SaveOffsets.GLOBAL_FLAG_FS_CHAMBER, chkFSChamber.Checked);
        }

        private void btnJournalAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                // 1) 日记条目解锁表：BOOK_SUB(file 0xBAD0) 前 114 字节全 0xFF（2026-08-28 修正：
                //    日记菜单 sub_2DE8CC/sub_2D6488 按 BOOK_SUB[i] 枚举第 i 条日记可见页数，
                //    0xFF=全部显示。与 Example/TOA_000"只日记全开"参考档逐字节一致。
                //    注意：聊天位图(0x418)不属于日记（参考档未动它），且置位后已看聊天不再触发，勿写。
                _saveData.UnlockAllDiaryEntries();

                // 2) 联动脚本变量 type 标记（var 154-269/338-340/563/621 高16位→0x3F80，
                //    同样与参考档逐字节一致）。
                int marked = _saveData.MarkDiaryScriptVars();

                MessageBox.Show(
                    LangText(
                        string.Format("日记已全开（114 条 + 变量标记 {0} 项），不影响剧情进度。", marked),
                        string.Format("日記を全開放しました（114条＋変数マーク{0}件）。ストーリー進行には影響しません。", marked)),
                    LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("日志全开失败：\n{0}", "Journal全開放失敗：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMapAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                // 地图全开 = 全局 flag 位图(0x218) 置位 flag 600-618 + 650-686（共 56 个）。
                // 与 Example/TOA_002"只地图全开"参考档完全一致；TotA15 四份存档交叉验证同一集合。
                // flag 619-649 非地图数据（所有参考档均未置位），不写入。
                int set = _saveData.UnlockAllMaps();

                MessageBox.Show(
                    LangText(
                        string.Format("地图已全开（新增 {0}/56 个地名 flag，当前 {1}/56）。", set, _saveData.CountMapUnlockFlags()),
                        string.Format("マップを全開放しました（新規{0}/56、現在{1}/56）。", set, _saveData.CountMapUnlockFlags())),
                    LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("地图全开失败：\n{0}", "マップ全開放失敗：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemBookAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                // 图鉴登记位：BOOK_EXTRA(file 0xBD10) 每道具 1 字节 bit0=已获得登记
                // （SetItemQtyWithClamp sub_2F366C 数量≠0 时 |=1，Load 遍历 640 道具自动登记，跨周目继承）。
                // 2026-08-28 黑屏排查新增：跳过"无实体/占位"道具 ID（BOOK_NO_ENTITY_IDS，如 43-51 谱石占位、
                // 561+ 敌人图鉴/音盘占位等），图鉴渲染假定 bit0=已获得 的道具必有实体配置，置位会读空配置崩黑屏。
                // ⚠ 二次修订：216 金属利刃、619 应援俱乐部会报 是真实道具，不再跳过（图鉴不再显示问号）。
                int registered = 0;
                for (int i = 0; i < SaveOffsets.BOOK_ITEM_REGISTER_COUNT; i++)
                {
                    if (SaveOffsets.BOOK_NO_ENTITY_IDS.Contains(i)) continue;
                    int off = SaveOffsets.BOOK_EXTRA_DATA_OFFSET + i;
                    byte cur = _saveData.ReadU8(off);
                    if (cur == 0 || (cur & 0x01) == 0)
                    {
                        _saveData.WriteU8(off, (byte)(cur | 0x01));
                        registered++;
                    }
                }

                // 发道具：跳过无实体 ID、id0、剧情贵重品（ID 531 罗蕾莱的宝珠——提前持有会触发
                // 剧情判定异常）与 ID≥561 的任务/系统关键道具，其余未持有的补 1 个用于图鉴登记。
                int itemCount = 0;
                for (int i = 1; i < SaveOffsets.BODY_ITEM_COUNT && i < 561; i++)
                {
                    if (SaveOffsets.BOOK_NO_ENTITY_IDS.Contains(i)) continue;
                    if (i == SaveOffsets.BOOK_STORY_KEEPSAKE_ID) continue;
                    byte val = _saveData.ReadU8(SaveOffsets.BODY_ITEM_ARRAY + i);
                    if (val == 0)
                    {
                        _saveData.WriteU8(SaveOffsets.BODY_ITEM_ARRAY + i, 1);
                        itemCount++;
                    }
                }

                // 道具收集图鉴（ID 565，贵重品·菜单开启钥匙）：持有时才开启主菜单"道具图鉴"页
                // （用户实测：无此道具图鉴菜单不显示），全开时强制设为 1。
                if (_saveData.ReadU8(SaveOffsets.BODY_ITEM_ARRAY + SaveOffsets.BOOK_COLLECT_BOOK_ID) == 0)
                {
                    _saveData.WriteU8(SaveOffsets.BODY_ITEM_ARRAY + SaveOffsets.BOOK_COLLECT_BOOK_ID, 1);
                    itemCount++;
                }

                RefreshItemsTab();
                MessageBox.Show(string.Format(LangText("道具图鉴已全开（登记 {0} 项，获得 {1} 个新道具，含图鉴菜单钥匙 565），跳过无实体/剧情贵重品。", "アイテム図鑑を全開放しました（{0}項目登録、新アイテム{1}個、図鑑メニュー鍵565含む）。実体なし/ストーリー貴重品は除外。"), registered, itemCount), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("道具图鉴全开失败：\n{0}", "アイテム図鑑全開放失敗：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetAllEquip_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                var equipItems = ItemDatabase.GetByCategory("武器")
                    .Concat(ItemDatabase.GetByCategory("防具"))
                    .Concat(ItemDatabase.GetByCategory("装饰品"));
                int count = 0;
                foreach (var item in equipItems)
                {
                    int offset = SaveOffsets.BODY_ITEM_ARRAY + item.Id;
                    if (offset < SaveOffsets.BODY_ITEM_ARRAY + SaveOffsets.BODY_ITEM_COUNT)
                    {
                        byte val = _saveData.ReadU8(offset);
                        if (val == 0)
                        {
                            _saveData.WriteU8(offset, 1);
                            count++;
                        }
                    }
                }
                RefreshItemsTab();
                MessageBox.Show(string.Format(LangText("已获得 {0} 件装备。", "{0}件の装備を獲得しました。"), count), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("错误：{0}", "エラー：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetAllItems_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                int count = 0;
                for (int i = 0; i < SaveOffsets.BODY_ITEM_COUNT; i++)
                {
                    byte val = _saveData.ReadU8(SaveOffsets.BODY_ITEM_ARRAY + i);
                    if (val == 0)
                    {
                        _saveData.WriteU8(SaveOffsets.BODY_ITEM_ARRAY + i, 1);
                        count++;
                    }
                }
                RefreshItemsTab();
                MessageBox.Show(string.Format(LangText("已获得 {0} 个道具。", "{0}個のアイテムを獲得しました。"), count), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("错误：{0}", "エラー：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetCategoryItems_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (cmbItemCategory.SelectedItem == null) return;

            try
            {
                string category = cmbItemCategory.SelectedItem as string;
                var items = ItemDatabase.GetByFilterCategory(category);
                int count = 0;
                foreach (var item in items)
                {
                    int offset = SaveOffsets.BODY_ITEM_ARRAY + item.Id;
                    if (offset < SaveOffsets.BODY_ITEM_ARRAY + SaveOffsets.BODY_ITEM_COUNT)
                    {
                        byte val = _saveData.ReadU8(offset);
                        if (val == 0)
                        {
                            _saveData.WriteU8(offset, 1);
                            count++;
                        }
                    }
                }
                RefreshItemsTab();
                MessageBox.Show(string.Format(LangText("已获得 {0} 个道具。", "{0}個のアイテムを獲得しました。"), count), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("错误：{0}", "エラー：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnArteLearnedSelectAll_Click(object sender, EventArgs e)
        {
            if (clbArteLearned == null) return;
            for (int i = 0; i < clbArteLearned.Items.Count; i++)
                clbArteLearned.SetItemChecked(i, true);
        }

        private void btnArteLearnedDeselectAll_Click(object sender, EventArgs e)
        {
            if (clbArteLearned == null) return;
            for (int i = 0; i < clbArteLearned.Items.Count; i++)
                clbArteLearned.SetItemChecked(i, false);
        }

        private void cmbPartySlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            for (int i = 0; i < SaveOffsets.BODY_PARTY_ORDER_COUNT && i < cmbPartySlot.Length; i++)
            {
                _saveData.WriteU8(SaveOffsets.BODY_PARTY_ORDER + i, (byte)cmbPartySlot[i].SelectedIndex);
            }
        }

        private void cmbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int diff = cmbDifficulty.SelectedIndex;
            if (diff < 0 || diff > 3) diff = 0;
            _saveData.WriteU8(SaveOffsets.BODY_DIFFICULTY, (byte)diff);
            _saveData.WriteU8(SaveOffsets.BODY_DIFFICULTY_SUMMARY, (byte)diff); // 读档时摘要块覆盖专用字节，此处在游戏内实际生效
        }

        private void cmbLeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int leader = cmbLeader.SelectedIndex;
            if (leader < 0 || leader > 7) leader = 1;
            _saveData.WriteU8(SaveOffsets.BODY_LEADER, (byte)leader);
        }

        /// <summary>难度值(0-3)转显示名，非法值返回 "-"</summary>
        private string GetDifficultyName(int diff)
        {
            switch (diff)
            {
                case 0: return LangText("普通", "ノーマル");
                case 1: return LangText("困难", "ハード");
                case 2: return LangText("狂热", "マニア");
                case 3: return LangText("未知", "アンノウン");
                default: return "-";
            }
        }



        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        menuFileSave_Click(sender, e);
                        e.Handled = true;
                        break;
                    case Keys.O:
                        menuFileOpen_Click(sender, e);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void btnMaxAllLevel_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            for (int i = 1; i <= 7; i++)
            {
                int baseOff = _saveData.GetCharBaseOffset(i);
                uint packed = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LEVEL_FLAGS);
                packed = (packed & 0xFFFFFF00) | 200;
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LEVEL_FLAGS, packed);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_EXP, 0);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_HP, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TP, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXHP, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXTP, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXHP_COPY, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXTP_COPY, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PATK, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PDEF, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FATK, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FDEF, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_AGI, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LUCK, 120);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_PATK, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_FATK, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_PDEF, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_FDEF, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_AGI, 9999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LUCK_COPY, 120);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_PATK, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_PDEF, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_FATK, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_FDEF, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_AGI, 999);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_LUK, 120);
            }
            if (cmbCharSelect.SelectedIndex >= 0) RefreshCharFields();
            MessageBox.Show(LangText("所有角色等级已设为200，属性已全满！", "全キャラクターのレベルを200に設定し、ステータスを最大にしました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllTitles_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            for (int i = 1; i <= 7; i++)
            {
                int baseOff = _saveData.GetCharBaseOffset(i);
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS, 0xFFFFFFFF);
            }
            if (cmbCharSelect.SelectedIndex >= 0) RefreshCharFields();
            MessageBox.Show(LangText("所有角色称号已全开！", "全キャラクターの称号を全解放しました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllADSkills_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            for (int i = 1; i <= 7; i++)
            {
                int baseOff = _saveData.GetCharBaseOffset(i);
                for (int j = 0; j < SaveOffsets.CHAR_AD_SKILL_SIZE; j++)
                {
                    _saveData.WriteByte(baseOff + SaveOffsets.CHAR_AD_SKILL + j, 0xFF);
                    _saveData.WriteByte(baseOff + SaveOffsets.CHAR_AD_SKILL_COPY + j, 0xFF);
                }
            }
            if (cmbCharSelect.SelectedIndex >= 0) RefreshCharFields();
            MessageBox.Show(LangText("所有角色附加技能已全开！", "全キャラクターの追加スキルを全解放しました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllFSMax_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            for (int c = 1; c <= 7; c++)
            {
                int arteCount = ArteDatabase.GetArteCount(c);
                for (int a = 0; a < arteCount; a++)
                {
                    for (int ci = 0; ci < 4; ci++)
                    {
                        _saveData.SetFSChamberLevel(c, a, ci, 6);
                    }
                }
            }
            MessageBox.Show(LangText("所有角色谱石已满级！", "全キャラクターのFSチャンバーをMAXにしました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllCookingMax_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            for (int c = 1; c <= 7; c++)
            {
                for (int r = 0; r < 20; r++)
                {
                    _saveData.WriteCookingMastery(c, r, 60);
                }
            }
            MessageBox.Show(LangText("所有角色料理已满级！", "全キャラクターの料理をマスターしました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAllItemsMax_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx)
            {
                MessageBox.Show(LangText("请先打开 TOA_XXX 存档。", "先にTOA_XXXセーブデータを開いてください。"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 全满规则（与游戏 SetItemQtyWithClamp 上限一致）：
                //   TypeCode 0x05~0x0C 武器/防具/装饰品（装备类）→ 16
                //   TypeCode 0x00~0x04 素材/软糖/瓶/强化/特殊（可堆叠类）→ 99
                //   跳过 ID 0（空名）与 ID ≥ 561（任务关键道具/系统道具，提前获得会破坏任务触发）
                int equipCount = 0;
                int stackCount = 0;
                foreach (var item in ItemDatabase.Items.Values)
                {
                    if (item.Id < 1 || item.Id > 560) continue;
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    int qty = (item.TypeCode >= 0x05 && item.TypeCode <= 0x0C) ? 16 : 99;
                    _saveData.SetItemQuantity(item.Id, (byte)qty);
                    if (qty == 16) equipCount++; else stackCount++;
                }

                RefreshItemsTab();
                MessageBox.Show(string.Format(
                    LangText("已将 {0} 个道具拉满（装备×16、消耗品×99），关键道具未改动。", "{0}個のアイテムを最大にしました（装備×16、消費アイテム×99）。キーアイテムは変更していません。"),
                    equipCount + stackCount), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("错误：{0}", "エラー：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 角色编辑页

        private static readonly string[] CharNames = { "", "卢克", "缇娅", "杰德", "阿妮丝", "凯", "娜塔莉亚", "阿修" };

        private void LoadCharacterData()
        {
            if (cmbCharSelect.Items.Count == 0)
            {
                for (int i = 1; i <= 7; i++)
                    cmbCharSelect.Items.Add(CharNames[i]);
                cmbCharSelect.SelectedIndex = 0;
            }
            else
            {
                RefreshCharFields();
            }
        }

        private void cmbCharSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCharFields();
        }

        private void RefreshCharFields()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;

            int portraitIdx = cmbCharSelect.SelectedIndex;
            picCharPortrait.Image = (portraitIdx >= 0 && portraitIdx < 7 && _charPortraits[portraitIdx] != null)
                ? _charPortraits[portraitIdx] : null;

            int baseOff = _saveData.GetCharBaseOffset(idx);
            _loading = true;
            try
            {
                try { SetNumericSafe(numLevel, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LEVEL) & 0xFF); } catch { numLevel.Value = numLevel.Minimum; }
                _lastCharLevel[idx] = (int)numLevel.Value;
                try { SetNumericSafe(numExp, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_EXP)); } catch { numExp.Value = numExp.Minimum; }
                try { SetNumericSafe(numHP, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_HP)); } catch { numHP.Value = numHP.Minimum; }
                try { SetNumericSafe(numTP, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_TP)); } catch { numTP.Value = numTP.Minimum; }
                try { SetNumericSafe(numMaxHP, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_MAXHP)); } catch { numMaxHP.Value = numMaxHP.Minimum; }
                try { SetNumericSafe(numMaxTP, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_MAXTP)); } catch { numMaxTP.Value = numMaxTP.Minimum; }

                try { SetNumericSafe(numBasePATK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PATK)); } catch { numBasePATK.Value = numBasePATK.Minimum; }
                try { SetNumericSafe(numBasePDEF, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PDEF)); } catch { numBasePDEF.Value = numBasePDEF.Minimum; }
                try { SetNumericSafe(numBaseFATK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FATK)); } catch { numBaseFATK.Value = numBaseFATK.Minimum; }
                try { SetNumericSafe(numBaseFDEF, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FDEF)); } catch { numBaseFDEF.Value = numBaseFDEF.Minimum; }
                try { SetNumericSafe(numBaseAGI, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_AGI)); } catch { numBaseAGI.Value = numBaseAGI.Minimum; }
                try { SetNumericSafe(numBaseLUCK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LUCK)); } catch { numBaseLUCK.Value = numBaseLUCK.Minimum; }
                try { SetNumericSafe(numOvlGauge, _saveData.ReadOvlGauge(idx)); } catch { numOvlGauge.Value = numOvlGauge.Minimum; }
                try { SetNumericSafe(numKillCount, _saveData.ReadU32(SaveOffsets.BODY_CHAR_KILLS + (idx - 1) * 4)); } catch { numKillCount.Value = numKillCount.Minimum; }
                try { SetNumericSafe(numGrowthPoints, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_GROWTH_POINTS)); } catch { numGrowthPoints.Value = numGrowthPoints.Minimum; }

                int charIdx = cmbCharSelect.SelectedIndex + 1;
                try { UpdateEquipRows(charIdx, baseOff); } catch { }

                if (lblArte != null)
                {
                    for (int i = 0; i < SaveOffsets.CHAR_ARTE_COUNT; i++)
                    {
                        try
                        {
                            _arteIds[i] = _saveData.ReadU16(baseOff + SaveOffsets.CHAR_ARTE_ARRAY + i * 2);
                            string arteName = ArteDatabase.GetName(_arteIds[i]);
                            string prefix = LangText("快捷", "ショートカット");
                            string empty = LangText("(空)", "(空)");
                            lblArte[i].Text = string.Format("{0}{1}: {2}", prefix, i + 1, _arteIds[i] == 0 ? empty : arteName);
                        }
                        catch { _arteIds[i] = 0; lblArte[i].Text = string.Format("{0}{1}: {2}", LangText("快捷", "ショートカット"), i + 1, LangText("(空)", "(空)")); }
                    }
                }

                if (clbArteLearned != null)
                {
                    clbArteLearned.Items.Clear();
                    var arteNames = ArteDatabase.GetArteNames(idx);
                    uint arteBitmap = _saveData.ReadArteLearnedBitmap(idx);
                    for (int i = 0; i < arteNames.Count; i++)
                    {
                        bool learned = (arteBitmap & (1u << i)) != 0;
                        clbArteLearned.Items.Add(arteNames[i], learned);
                    }
                }

                if (clbADSkills != null && clbADSkills.Items.Count > 0)
                {
                    byte[] adBytes = _saveData.ReadBytes(baseOff + SaveOffsets.CHAR_AD_SKILL, SaveOffsets.CHAR_AD_SKILL_SIZE);
                    for (int i = 0; i < clbADSkills.Items.Count && i < 88; i++)
                    {
                        try
                        {
                            int byteIdx = i / 8;
                            int bitMask = 1 << (i % 8);
                            bool learned = (adBytes[byteIdx] & bitMask) != 0;
                            clbADSkills.SetItemChecked(i, learned);
                        }
                        catch { clbADSkills.SetItemChecked(i, false); }
                    }
                }

                try { SetNumericSafe(numCCorePATK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PATK)); } catch { numCCorePATK.Value = numCCorePATK.Minimum; }
                try { SetNumericSafe(numCCorePDEF, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PDEF)); } catch { numCCorePDEF.Value = numCCorePDEF.Minimum; }
                try { SetNumericSafe(numCCoreFATK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FATK)); } catch { numCCoreFATK.Value = numCCoreFATK.Minimum; }
                try { SetNumericSafe(numCCoreFDEF, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FDEF)); } catch { numCCoreFDEF.Value = numCCoreFDEF.Minimum; }
                try { SetNumericSafe(numCCoreAGI, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_AGI)); } catch { numCCoreAGI.Value = numCCoreAGI.Minimum; }
                try { SetNumericSafe(numCCoreLUK, _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_LUK)); } catch { numCCoreLUK.Value = numCCoreLUK.Minimum; }

                int arteCount = ArteDatabase.GetArteCount(idx);
                for (int i = 0; i < 25; i++)
                {
                    if (i < arteCount)
                    {
                        lblArteUsage[i].Text = ArteDatabase.GetArteName(idx, i);
                        lblArteUsage[i].Visible = true;
                        numArteUsage[i].Visible = true;
                        try { numArteUsage[i].Value = _saveData.ReadU16(baseOff + SaveOffsets.CHAR_ARTE_USAGE + i * 2); } catch { }
                    }
                    else
                    {
                        lblArteUsage[i].Visible = false;
                        numArteUsage[i].Visible = false;
                    }
                }

                try
                {
                    _currentTitleIndex = _saveData.ReadU8(baseOff + SaveOffsets.CHAR_TITLE_INDEX);
                    lblTitle.Text = TitleDatabase.GetTitleNameCn(idx, (int)_currentTitleIndex);
                }
                catch { lblTitle.Text = LangText("(无)", "(なし)"); }

                if (clbTitles != null)
                {
                    clbTitles.Items.Clear();
                    int titleCount = TitleDatabase.GetTitleCount(idx);
                    uint obtainedFlags = 0;
                    try { obtainedFlags = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS); } catch { }

                    for (int i = 0; i < titleCount; i++)
                    {
                        string name = TitleDatabase.GetTitleNameCn(idx, i + 1);
                        bool obtained = (obtainedFlags & (1u << (i + 1))) != 0;
                        string status = obtained ? "" : LangText(" [未获得]", " [未取得]");
                        clbTitles.Items.Add(string.Format("{0}: {1}{2}", i + 1, name, status), obtained);
                    }
                }
            }
            finally
            {
                _loading = false;
            }
        }

        // 装备页五行显示：槽位 0-3=装备（角色块 0x88 起 4×u16），4=响律符（0x94）
        private void UpdateEquipRows(int charIdx, int baseOff)
        {
            if (lblEquip == null) return;
            for (int i = 0; i < 4; i++)
            {
                int id = _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + i * 2);
                lblEquip[i].Text = EquipSelectForm.SlotName(i) + ": " + ResolveEquipName(i, id);
            }
            int kyId = _saveData.ReadKyouritsufu(charIdx);
            lblEquip[4].Text = EquipSelectForm.SlotName(4) + ": " + ResolveEquipName(4, kyId);
        }

        private string ResolveEquipName(int slotIndex, int id)
        {
            if (id == 0) return LangText("(无)", "(なし)");
            if (slotIndex == 4)
            {
                string n = KyouritsufuDatabase.GetName(id);
                return string.IsNullOrEmpty(n) ? string.Format("(ID:{0})", id) : n;
            }
            string name = ItemDatabase.GetById(id)?.Name;
            return string.IsNullOrEmpty(name) ? string.Format("(ID:{0})", id) : name;
        }

        // 更改按钮：弹出装备选择器（内嵌搜索），确定后写回存档并刷新行显示
        private void btnEquipChange_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            int baseOff = _saveData.GetCharBaseOffset(idx);
            if (baseOff == 0) return;

            int slot = (int)((Button)sender).Tag;
            int currentId = slot == 4
                ? _saveData.ReadKyouritsufu(idx)
                : _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + slot * 2);

            using (var dlg = new EquipSelectForm(idx, slot, currentId))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedEquipId >= 0)
                {
                    int newId = dlg.SelectedEquipId;
                    if (slot == 4)
                        _saveData.WriteKyouritsufu(idx, (ushort)newId);
                    else
                        _saveData.WriteU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + slot * 2, (ushort)newId);
                    lblEquip[slot].Text = EquipSelectForm.SlotName(slot) + ": " + ResolveEquipName(slot, newId);
                }
            }
        }

        private void numLevel_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            int baseOff = _saveData.GetCharBaseOffset(idx);
            uint packed = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LEVEL);
            packed = (packed & 0xFFFFFF00) | ((uint)numLevel.Value & 0xFF);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LEVEL, packed);

            // 等级联动成长：按每级增量自动增减基础属性（近似值；读档时游戏按 sub_3E1038 重算衍生属性）
            if (chkLevelGrowth != null && chkLevelGrowth.Checked)
            {
                int delta = (int)numLevel.Value - _lastCharLevel[idx];
                if (delta != 0)
                {
                    ApplyLevelGrowth(baseOff, delta);
                    RefreshCharFields(); // 重读以刷新 UI（同时更新 _lastCharLevel）
                    return;
                }
            }
            _lastCharLevel[idx] = (int)numLevel.Value;
        }

        private void ApplyLevelGrowth(int baseOff, int delta)
        {
            long hp = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_HP) + delta * (int)numGrowHP.Value;
            long mhp = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_MAXHP) + delta * (int)numGrowHP.Value;
            long tp = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_TP) + delta * (int)numGrowTP.Value;
            long mtp = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_MAXTP) + delta * (int)numGrowTP.Value;
            long patk = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PATK) + delta * (int)numGrowPATK.Value;
            long pdef = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PDEF) + delta * (int)numGrowPDEF.Value;
            long fatk = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FATK) + delta * (int)numGrowFATK.Value;
            long fdef = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FDEF) + delta * (int)numGrowFDEF.Value;
            long agi = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_AGI) + delta * (int)numGrowAGI.Value;
            long luk = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LUCK) + delta * (int)numGrowLUK.Value;

            if (hp < 0) hp = 0; if (mhp < 1) mhp = 1; if (tp < 0) tp = 0; if (mtp < 1) mtp = 1;
            if (patk < 0) patk = 0; if (pdef < 0) pdef = 0; if (fatk < 0) fatk = 0; if (fdef < 0) fdef = 0;
            if (agi < 0) agi = 0; if (luk < 0) luk = 0;
            if (luk > 120) luk = 120;

            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_HP, (uint)hp);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXHP, (uint)mhp);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TP, (uint)tp);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXTP, (uint)mtp);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PATK, (uint)patk);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PDEF, (uint)pdef);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FATK, (uint)fatk);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FDEF, (uint)fdef);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_AGI, (uint)agi);
            // 幸运：同步总和字段（0x84，游戏上限 120）
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LUCK, (uint)luk);
            uint lukEquip = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_EQUIP_LUK);
            uint lukTotal = (uint)Math.Min(120, luk + lukEquip);
            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LUCK_TOTAL, lukTotal);
        }
        private void numExp_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_EXP, (uint)numExp.Value); }
        private void numHP_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_HP, (uint)numHP.Value); }
        private void numTP_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TP, (uint)numTP.Value); }
        private void numMaxHP_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint v = (uint)numMaxHP.Value; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXHP, v); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXHP_COPY, v); }
        private void numMaxTP_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint v = (uint)numMaxTP.Value; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXTP, v); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_MAXTP_COPY, v); }
        private void numBasePATK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newBase = (uint)numBasePATK.Value; uint ccore = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PATK); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PATK) - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PATK) - ccore; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_PATK, newBase); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PATK, newBase + ccore + equipBonus); }
        private void numBasePDEF_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newBase = (uint)numBasePDEF.Value; uint ccore = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PDEF); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PDEF) - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PDEF) - ccore; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_PDEF, newBase); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PDEF, newBase + ccore + equipBonus); }
        private void numBaseFATK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newBase = (uint)numBaseFATK.Value; uint ccore = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FATK); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FATK) - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FATK) - ccore; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_FATK, newBase); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FATK, newBase + ccore + equipBonus); }
        private void numBaseFDEF_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newBase = (uint)numBaseFDEF.Value; uint ccore = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FDEF); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FDEF) - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FDEF) - ccore; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_FDEF, newBase); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FDEF, newBase + ccore + equipBonus); }
        private void numBaseAGI_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newBase = (uint)numBaseAGI.Value; uint ccore = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_AGI); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_AGI) - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_AGI) - ccore; _saveData.WriteU32(baseOff + SaveOffsets.CHAR_BASE_AGI, newBase); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_AGI, newBase + ccore + equipBonus); }
        private void numBaseLUCK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; _saveData.WriteLuckBase(idx, (uint)numBaseLUCK.Value); }
        private void numOvlGauge_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; _saveData.WriteOvlGauge(idx, (ushort)numOvlGauge.Value); }
        private void numKillCount_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; _saveData.WriteU32(SaveOffsets.BODY_CHAR_KILLS + (idx - 1) * 4, (uint)numKillCount.Value); }
        private void numGrowthPoints_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); _saveData.WriteU16(baseOff + SaveOffsets.CHAR_GROWTH_POINTS, (ushort)numGrowthPoints.Value); }

        // 基础/战斗属性面板互斥切换：组标题随面板变化，按钮仅切换文字（C-Core 编辑已隐藏）
        private void btnStatToggle_Click(object sender, EventArgs e)
        {
            _showCombatStats = !_showCombatStats;
            pnlStatBasic.Visible = !_showCombatStats;
            pnlStatCombat.Visible = _showCombatStats;
            if (_grpStats != null)
                _grpStats.Text = _showCombatStats
                    ? LangText("战斗属性", "戦闘ステータス")
                    : LangText("基础属性", "基本ステータス");
            btnStatToggle.Text = _showCombatStats
                ? LangText("◂ 返回基础属性", "◂ 基本ステータスへ")
                : LangText("显示战斗属性 ▸", "戦闘ステータスへ ▸");
        }

        private void numCCorePATK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newCCore = (uint)numCCorePATK.Value; uint oldBase = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PATK); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PATK) - oldBase - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PATK); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_PATK, newCCore); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PATK, oldBase + newCCore + equipBonus); }
        private void numCCorePDEF_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newCCore = (uint)numCCorePDEF.Value; uint oldBase = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_PDEF); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_PDEF) - oldBase - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_PDEF); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_PDEF, newCCore); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_PDEF, oldBase + newCCore + equipBonus); }
        private void numCCoreFATK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newCCore = (uint)numCCoreFATK.Value; uint oldBase = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FATK); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FATK) - oldBase - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FATK); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_FATK, newCCore); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FATK, oldBase + newCCore + equipBonus); }
        private void numCCoreFDEF_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newCCore = (uint)numCCoreFDEF.Value; uint oldBase = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_FDEF); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_FDEF) - oldBase - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_FDEF); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_FDEF, newCCore); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_FDEF, oldBase + newCCore + equipBonus); }
        private void numCCoreAGI_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint newCCore = (uint)numCCoreAGI.Value; uint oldBase = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_BASE_AGI); uint equipBonus = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_AGI) - oldBase - _saveData.ReadU32(baseOff + SaveOffsets.CHAR_CCORE_AGI); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_CCORE_AGI, newCCore); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_AGI, oldBase + newCCore + equipBonus); }
        private void numCCoreLUK_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; _saveData.WriteLuckEquipBonus(idx, (uint)numCCoreLUK.Value); }

        private void numArteUsage_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            int baseOff = _saveData.GetCharBaseOffset(idx);
            int slot = (int)((NumericUpDown)sender).Tag;
            _saveData.WriteU16(baseOff + SaveOffsets.CHAR_ARTE_USAGE + slot * 2, (ushort)((NumericUpDown)sender).Value);
        }


        private void clbArteLearned_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            BeginInvoke(new Action(() => {
                uint arteBitmap = 0;
                for (int i = 0; i < clbArteLearned.Items.Count && i < 32; i++)
                {
                    if (clbArteLearned.GetItemChecked(i))
                        arteBitmap |= (1u << i);
                }
                _saveData.WriteArteLearnedBitmap(idx, arteBitmap);
            }));
        }

        private void clbADSkills_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            int baseOff = _saveData.GetCharBaseOffset(idx);
            BeginInvoke(new Action(() => {
                byte[] adBytes = new byte[SaveOffsets.CHAR_AD_SKILL_SIZE];
                for (int b = 0; b < SaveOffsets.CHAR_AD_SKILL_SIZE; b++)
                    adBytes[b] = _saveData.ReadU8(baseOff + SaveOffsets.CHAR_AD_SKILL + b);
                for (int i = 0; i < 88 && i < clbADSkills.Items.Count; i++)
                {
                    int byteIdx = i / 8;
                    int bitMask = 1 << (i % 8);
                    if (clbADSkills.GetItemChecked(i))
                        adBytes[byteIdx] |= (byte)bitMask;
                    else
                        adBytes[byteIdx] &= (byte)~bitMask;
                }
                for (int i = 0; i < SaveOffsets.CHAR_AD_SKILL_SIZE; i++)
                {
                    _saveData.WriteU8(baseOff + SaveOffsets.CHAR_AD_SKILL + i, adBytes[i]);
                    _saveData.WriteU8(baseOff + SaveOffsets.CHAR_AD_SKILL_COPY + i, adBytes[i]);
                }
            }));
        }

        private void clbTitles_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            int baseOff = _saveData.GetCharBaseOffset(idx);
            BeginInvoke(new Action(() => {
                uint obtainedFlags = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS);
                int titleCount = TitleDatabase.GetTitleCount(idx);
                for (int i = 0; i < titleCount && i < clbTitles.Items.Count; i++)
                {
                    bool isChecked = clbTitles.GetItemChecked(i);
                    if (isChecked)
                        obtainedFlags |= (1u << (i + 1));
                    else
                        obtainedFlags &= ~(1u << (i + 1));
                    // 实时刷新获得/未获得状态后缀（勾选=已获得）
                    string name = TitleDatabase.GetTitleNameCn(idx, i + 1);
                    string status = isChecked ? "" : LangText(" [未获得]", " [未取得]");
                    clbTitles.Items[i] = string.Format("{0}: {1}{2}", i + 1, name, status);
                }
                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS, obtainedFlags);
            }));
        }

        private void btnArteChange_Click(object sender, EventArgs e)
        {
            try
            {
                int slot = (int)((Button)sender).Tag;
                int charIdx = cmbCharSelect.SelectedIndex + 1;
                using (var dlg = new ArteSelectForm(charIdx))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedArteId >= 0)
                    {
                        _arteIds[slot] = (ushort)dlg.SelectedArteId;
                        string arteName = ArteDatabase.GetName(_arteIds[slot]);
                        string prefix = LangText("快捷", "ショートカット");
                        string empty = LangText("(空)", "(空)");
                        lblArte[slot].Text = string.Format("{0}{1}: {2}", prefix, slot + 1, _arteIds[slot] == 0 ? empty : arteName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("术技更改失败：{0}", "アーツ変更失敗：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnADSelectAll_Click(object sender, EventArgs e)
        {
            if (clbADSkills == null) return;
            for (int i = 0; i < clbADSkills.Items.Count; i++)
                clbADSkills.SetItemChecked(i, true);
        }

        private void btnADDeselectAll_Click(object sender, EventArgs e)
        {
            if (clbADSkills == null) return;
            for (int i = 0; i < clbADSkills.Items.Count; i++)
                clbADSkills.SetItemChecked(i, false);
        }

        private void btnADLearnAll_Click(object sender, EventArgs e)
        {
            if (clbADSkills == null) return;
            for (int i = 0; i < clbADSkills.Items.Count; i++)
                clbADSkills.SetItemChecked(i, true);
        }

        private void btnTitleChange_Click(object sender, EventArgs e)
        {
            try
            {
                int idx = cmbCharSelect.SelectedIndex + 1;
                if (idx < 1 || idx > 7) return;

                using (var dlg = new TitleSelectForm(idx))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedTitleIndex >= 0)
                    {
                        _currentTitleIndex = (uint)(dlg.SelectedTitleIndex + 1);
                        lblTitle.Text = TitleDatabase.GetTitleNameCn(idx, (int)_currentTitleIndex);

                        if (_saveData != null && _saveData.Type == SaveType.ToaXxx)
                        {
                            int baseOff = _saveData.GetCharBaseOffset(idx);
                            _saveData.WriteU8(baseOff + SaveOffsets.CHAR_TITLE_INDEX, (byte)_currentTitleIndex);

                            uint obtainedFlags = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS);
                            obtainedFlags |= (1u << (int)_currentTitleIndex);
                            _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS, obtainedFlags);

                            if (clbTitles != null && dlg.SelectedTitleIndex < clbTitles.Items.Count)
                            {
                                clbTitles.SetItemChecked(dlg.SelectedTitleIndex, true);
                                string name = TitleDatabase.GetTitleNameCn(idx, (int)_currentTitleIndex);
                                clbTitles.Items[dlg.SelectedTitleIndex] = string.Format("{0}: {1}", dlg.SelectedTitleIndex + 1, name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("称号更改失败：{0}", "称号変更失敗：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTitleOpenAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;

            try
            {
                int baseOff = _saveData.GetCharBaseOffset(idx);
                int titleCount = TitleDatabase.GetTitleCount(idx);
                if (titleCount <= 0) return;

                uint flags = 0;
                for (int i = 0; i < titleCount; i++)
                    flags |= (1u << (i + 1));

                _saveData.WriteU32(baseOff + SaveOffsets.CHAR_TITLE_FLAGS, flags);

                if (clbTitles != null)
                {
                    for (int i = 0; i < titleCount && i < clbTitles.Items.Count; i++)
                    {
                        clbTitles.SetItemChecked(i, true);
                        string name = TitleDatabase.GetTitleNameCn(idx, i + 1);
                        clbTitles.Items[i] = string.Format("{0}: {1}", i + 1, name);
                    }
                }

                MessageBox.Show(string.Format(LangText("已解锁{0}的所有{1}个称号。", "{0}の全{1}個の称号を解放しました。"), CharNames[idx], titleCount), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("称号全开失败：{0}", "称号全開放失敗：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbFSCharSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            LoadFSChamberData();
        }

        private void LoadFSChamberData()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (cmbFSCharSelect == null) return;
            int charIdx = cmbFSCharSelect.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;

            _loading = true;
            try
            {
                dgvFSChamber.Rows.Clear();
                int arteCount = ArteDatabase.GetArteCount(charIdx);
                string[] equippedNames = { LangText("无", "無"), LangText("赤", "赤"), LangText("青", "青"), LangText("緑", "緑"), LangText("黄", "黄") };
                for (int ai = 0; ai < arteCount; ai++)
                {
                    string arteName = ArteDatabase.GetArteName(charIdx, ai);
                    int equipped = _saveData.ReadFSChamberEquippedType(charIdx, ai);
                    var row = new DataGridViewRow();
                    row.CreateCells(dgvFSChamber);
                    row.Cells[0].Value = arteName;
                    row.Cells[1].Value = (equipped >= 0 && equipped <= 4) ? equippedNames[equipped] : equippedNames[0];
                    for (int ci = 0; ci < 4; ci++)
                    {
                        int level = _saveData.GetFSChamberLevel(charIdx, ai, ci);
                        row.Cells[ci + 2].Value = "Lv." + level;
                    }
                    dgvFSChamber.Rows.Add(row);
                }

                for (int i = 0; i < 4; i++)
                {
                    try { numFSChamberMax[i].Value = _saveData.ReadFSChamberMax(1, i); } catch { }
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void dgvFSChamber_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loading || _saveData == null) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return;
            int charIdx = cmbFSCharSelect.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;
            int arteSlot = e.RowIndex;
            try
            {
                var cell = dgvFSChamber.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string cellStr = cell.Value?.ToString() ?? "";
                if (e.ColumnIndex == 1)
                {
                    string[] equippedNames = { LangText("无", "無"), LangText("赤", "赤"), LangText("青", "青"), LangText("緑", "緑"), LangText("黄", "黄") };
                    int val = Array.IndexOf(equippedNames, cellStr);
                    if (val < 0) val = 0;
                    _saveData.WriteFSChamberEquippedType(charIdx, arteSlot, (byte)val);
                }
                else if (e.ColumnIndex >= 2 && e.ColumnIndex <= 5)
                {
                    int colorIdx = e.ColumnIndex - 2;
                    int level = 1;
                    if (cellStr.StartsWith("Lv.") && int.TryParse(cellStr.Substring(3), out int parsed))
                        level = parsed;
                    if (level < 1) level = 1;
                    if (level > 6) level = 6;
                    _saveData.SetFSChamberLevel(charIdx, arteSlot, colorIdx, level);
                }
            }
            catch { }
        }

        private void dgvFSChamber_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void numFSChamberMax_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int charIdx = cmbFSCharSelect.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;
            int colorIdx = (int)((NumericUpDown)sender).Tag;
            _saveData.WriteFSChamberMax(charIdx, colorIdx, (byte)((NumericUpDown)sender).Value);
        }

        private void btnFSAllMax_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            int charIdx = cmbFSCharSelect.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;
            int arteCount = ArteDatabase.GetArteCount(charIdx);
            for (int ai = 0; ai < arteCount; ai++)
            {
                for (int ci = 0; ci < 4; ci++)
                {
                    _saveData.SetFSChamberLevel(charIdx, ai, ci, 6);
                }
            }
            LoadFSChamberData();
        }

        private void btnFSAllReset_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            int charIdx = cmbFSCharSelect.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;
            int arteCount = ArteDatabase.GetArteCount(charIdx);
            for (int ai = 0; ai < arteCount; ai++)
            {
                _saveData.WriteFSChamberEquippedType(charIdx, ai, 0);
                for (int ci = 0; ci < 4; ci++)
                {
                    _saveData.SetFSChamberLevel(charIdx, ai, ci, 1);
                }
            }
            LoadFSChamberData();
        }

        #endregion

        #region 道具管理页

        private DataTable _itemTable;

        private void LoadItemData()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            if (cmbItemCategory.Items.Count == 0)
            {
                foreach (var cat in ItemDatabase.GetCategoryNames())
                    cmbItemCategory.Items.Add(cat);
                cmbItemCategory.SelectedIndex = 0;
            }

            BuildItemTable();
            ApplyItemFilter();
        }

        private void BuildItemTable()
        {
            _itemTable = new DataTable();
            _itemTable.Columns.Add("ID", typeof(int));
            _itemTable.Columns.Add("Hex", typeof(string));
            _itemTable.Columns.Add(LangText("名称", "名称"), typeof(string));
            _itemTable.Columns.Add(LangText("数量", "数量"), typeof(int));

            byte[] quantities = _saveData.GetItemQuantities();

            for (int i = 0; i < SaveOffsets.BODY_ITEM_COUNT; i++)
            {
                var item = ItemDatabase.GetById(i);
                // 隐藏只有ID没有具体名称的道具（ID 0/631 空名及 632-639 数据库未定义）
                if (item == null || string.IsNullOrEmpty(item.Name)) continue;
                string name = item.Name;
                int qty = quantities[i];
                _itemTable.Rows.Add(i, $"0x{i:X3}", name, qty);
            }

            dgvItems.DataSource = _itemTable;
            dgvItems.Columns["ID"].ReadOnly = true;
            dgvItems.Columns["ID"].Width = 50;
            dgvItems.Columns["Hex"].ReadOnly = true;
            dgvItems.Columns["Hex"].Width = 60;
            string nameCol = LangText("名称", "名称");
            if (dgvItems.Columns.Contains(nameCol))
            {
                dgvItems.Columns[nameCol].ReadOnly = true;
                dgvItems.Columns[nameCol].Width = 200;
            }
            string qtyCol = LangText("数量", "数量");
            if (dgvItems.Columns.Contains(qtyCol))
            {
                dgvItems.Columns[qtyCol].Width = 60;
            }
        }

        private void cmbItemCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyItemFilter();
        }

        private void txtItemSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyItemFilter();
        }

        private void ApplyItemFilter()
        {
            if (_itemTable == null) return;
            string category = cmbItemCategory.SelectedItem as string;
            if (category == null) return;

            // 类别筛选 + 名称/ID 搜索叠加（AND 组合）
            var conds = new List<string>();

            if (category != "全部")
            {
                var ids = ItemDatabase.GetByFilterCategory(category).Select(i => i.Id).ToList();
                conds.Add(ids.Count == 0 ? "ID = -1" : $"ID IN ({string.Join(",", ids)})");
            }

            string search = txtItemSearch != null ? txtItemSearch.Text.Trim() : "";
            if (search.Length > 0)
            {
                string nameCol = LangText("名称", "名称");
                string like = search.Replace("'", "''");
                string cond = $"{nameCol} LIKE '%{like}%'";
                int id;
                if (int.TryParse(search, out id))
                    cond += $" OR ID = {id}";
                conds.Add($"({cond})");
            }

            _itemTable.DefaultView.RowFilter = string.Join(" AND ", conds);
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loading || _saveData == null || _itemTable == null) return;
            if (e.RowIndex < 0) return;
            string qtyCol = LangText("数量", "数量");
            try
            {
                // 必须通过 DataBoundItem 取实际数据行：
                // e.RowIndex 是筛选/排序后视图的行号，直接索引 _itemTable.Rows 会写错道具
                DataRowView drv = dgvItems.Rows[e.RowIndex].DataBoundItem as DataRowView;
                if (drv == null) return;
                DataRow row = drv.Row;
                int id = (int)row["ID"];
                int qty = Convert.ToInt32(row[qtyCol]);
                if (qty < 0) qty = 0;
                if (qty > 99) qty = 99;
                _saveData.SetItemQuantity(id, (byte)qty);
            }
            catch { }
        }

        private void RefreshItemsTab()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            try { BuildItemTable(); ApplyItemFilter(); } catch { }
        }

        private void dgvItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string qtyCol = LangText("数量", "数量");
            if (dgvItems.Columns.Contains(qtyCol))
            {
                if (e.ColumnIndex != dgvItems.Columns[qtyCol].Index) return;
            }
            else if (e.ColumnIndex != 3)
            {
                return;
            }
            if (!int.TryParse(e.FormattedValue.ToString(), out int val) || val < 0 || val > 99)
            {
                e.Cancel = true;
                MessageBox.Show(LangText("数量必须在0~99之间。", "数量は0～99の間で入力してください。"), LangText("输入错误", "入力エラー"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSaveBagState_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx || _itemTable == null) return;

            try
            {
                string qtyCol = LangText("数量", "数量");
                int applied = 0;
                foreach (DataRow row in _itemTable.Rows)
                {
                    int id = (int)row["ID"];
                    int qty = Convert.ToInt32(row[qtyCol]);
                    if (qty < 0) qty = 0;
                    if (qty > 99) qty = 99;
                    _saveData.SetItemQuantity(id, (byte)qty);
                    applied++;
                }
                MessageBox.Show(string.Format(LangText("已保存当前背包状态（{0} 项道具）。", "現在のバッグ状態を保存しました（{0}アイテム）。"), applied), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("错误：{0}", "エラー：{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 滚轮悬停编辑：鼠标在背包表格"数量"列上滚动时直接增减数量。
        /// 返回 true 表示已吞掉消息（阻止表格滚动），false 放行保持默认行为。
        /// </summary>
        private bool HandleItemWheel(ref Message m)
        {
            if (_loading || _saveData == null || _saveData.Type != SaveType.ToaXxx) return false;
            if (dgvItems == null || !dgvItems.Visible || _itemTable == null) return false;

            // 消息目标控件须属于主窗体（模态弹窗中的滚轮操作不拦截）
            Control target = Control.FromHandle(m.HWnd);
            if (target == null || target.FindForm() != this) return false;

            string qtyCol = LangText("数量", "数量");
            if (!dgvItems.Columns.Contains(qtyCol)) return false;

            // WM_MOUSEWHEEL：lParam 为鼠标屏幕坐标（需转客户区再 HitTest），wParam 高16位为滚轮增量
            long lp = m.LParam.ToInt64();
            var screenPt = new Point((short)(lp & 0xFFFF), (short)((lp >> 16) & 0xFFFF));
            if (!dgvItems.RectangleToScreen(dgvItems.ClientRectangle).Contains(screenPt)) return false;

            Point clientPt = dgvItems.PointToClient(screenPt);
            DataGridView.HitTestInfo hit = dgvItems.HitTest(clientPt.X, clientPt.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return false;
            if (hit.ColumnIndex != dgvItems.Columns[qtyCol].Index) return false;

            long wp = m.WParam.ToInt64();
            int delta = (short)((wp >> 16) & 0xFFFF);
            if (delta == 0) return false;
            // MK_CONTROL(0x0008)：按住 Ctrl 时步长 ×10
            int step = ((wp & 0x0008) != 0 || (Control.ModifierKeys & Keys.Control) == Keys.Control) ? 10 : 1;

            try
            {
                if (dgvItems.IsCurrentCellInEditMode)
                {
                    try { dgvItems.EndEdit(); } catch { }
                }

                // 必须通过 DataBoundItem 取实际数据行：
                // hit.RowIndex 是筛选/排序后视图的行号，直接索引 _itemTable.Rows 会写错道具
                DataRowView drv = dgvItems.Rows[hit.RowIndex].DataBoundItem as DataRowView;
                if (drv == null) return false;
                DataRow row = drv.Row;
                int qty = Convert.ToInt32(row[qtyCol]);
                qty += delta > 0 ? step : -step;
                if (qty < 0) qty = 0;
                if (qty > 99) qty = 99;
                // 更新 DataTable 触发 dgvItems_CellValueChanged 写回链把数量写进存档字节
                row[qtyCol] = qty;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>全局滚轮消息过滤器：仅转发 WM_MOUSEWHEEL 给主窗体处理。</summary>
        private sealed class ItemWheelFilter : IMessageFilter
        {
            private const int WM_MOUSEWHEEL = 0x020A;
            private readonly MainForm _owner;

            public ItemWheelFilter(MainForm owner)
            {
                _owner = owner;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != WM_MOUSEWHEEL) return false;
                return _owner.HandleItemWheel(ref m);
            }
        }

        #endregion

        #region 料理修改页

        private void LoadCookingData()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (clbCooking == null || clbCooking.Items.Count == 0) return;

            _loading = true;
            try
            {
                uint cookingFlags = _saveData.ReadCookingFlags();
                int count = Math.Min(clbCooking.Items.Count, 20);
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        bool learned = (cookingFlags & (1u << (i + 1))) != 0;
                        clbCooking.SetItemChecked(i, learned);
                    }
                    catch
                    {
                        clbCooking.SetItemChecked(i, false);
                    }
                }

                if (cmbCookingChar != null && cmbCookingChar.SelectedIndex >= 0)
                {
                    LoadCookingMasteryForChar(cmbCookingChar.SelectedIndex + 1);
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void LoadCookingMasteryForChar(int charIndex)
        {
            if (_saveData == null || numCookingMastery == null) return;
            _loading = true;
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    try { numCookingMastery[i].Value = _saveData.ReadCookingMastery(charIndex, i); } catch { }
                    UpdateCookingMasteryStar(i);
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void btnCookingSelectAll_Click(object sender, EventArgs e)
        {
            if (clbCooking == null) return;
            for (int i = 0; i < clbCooking.Items.Count; i++)
                clbCooking.SetItemChecked(i, true);
        }

        private void btnCookingDeselectAll_Click(object sender, EventArgs e)
        {
            if (clbCooking == null) return;
            for (int i = 0; i < clbCooking.Items.Count; i++)
                clbCooking.SetItemChecked(i, false);
        }

        private void clbCooking_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading || _saveData == null) return;
            BeginInvoke(new Action(() => {
                uint cookingFlags = _saveData.ReadCookingFlags();
                cookingFlags &= ~0x1FFFFFEu;
                for (int i = 0; i < clbCooking.Items.Count && i < 20; i++)
                {
                    if (clbCooking.GetItemChecked(i))
                        cookingFlags |= (1u << (i + 1));
                }
                _saveData.WriteCookingFlags(cookingFlags);
            }));
        }

        private void numCookingMastery_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            if (cmbCookingChar == null) return;
            int charIdx = cmbCookingChar.SelectedIndex + 1;
            if (charIdx < 1 || charIdx > 7) return;
            int idx = (int)((NumericUpDown)sender).Tag;
            _saveData.WriteCookingMastery(charIdx, idx, (byte)((NumericUpDown)sender).Value);
            UpdateCookingMasteryStar(idx);
        }

        private void UpdateCookingMasteryStar(int index)
        {
            if (lblCookingMasteryStar == null || lblCookingMasteryStar.Length <= index) return;
            if (numCookingMastery == null || numCookingMastery.Length <= index) return;
            int star = _saveData.GetCookingMasteryStar((byte)numCookingMastery[index].Value);
            lblCookingMasteryStar[index].Text = new string('⭐', star);
        }

        private void cmbCookingChar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            if (cmbCookingChar.SelectedIndex < 0) return;
            LoadCookingMasteryForChar(cmbCookingChar.SelectedIndex + 1);
        }

        #endregion

        #region 系统数据页

        private void LoadToasysData()
        {
            if (_saveData == null || _saveData.Type != SaveType.Toasys) return;
            _loading = true;
            try
            {
                try { numToasysDifficulty.Value = (decimal)_saveData.ReadFloat(SaveOffsets.TOASYS_VERSION); } catch { }
                // 偏移语义见 SaveOffsets.TOASYS_* 注释（2026-08-19 双存档+IDA 定案）
                try { SetNumericSafe(numToasysGald, _saveData.ReadU32(SaveOffsets.TOASYS_GALD_MAX)); } catch { }
                try { SetNumericSafe(numToasysPlaytime, _saveData.ReadU32(SaveOffsets.TOASYS_PLAYTIME_MAX)); } catch { }
                try { SetNumericSafe(numToasysGaldSpent, _saveData.ReadU32(SaveOffsets.TOASYS_GALD_SPENT)); } catch { }
                try { SetNumericSafe(numToasysSaveCount, _saveData.ReadU32(SaveOffsets.TOASYS_SAVE_COUNT)); } catch { }
                try { SetNumericSafe(numToasysEncounter, _saveData.ReadU32(SaveOffsets.TOASYS_ENCOUNTER)); } catch { }
                try { SetNumericSafe(numToasysClearCount, _saveData.ReadU32(SaveOffsets.TOASYS_CLEAR_COUNT)); } catch { }
                chkSoundTest.Checked = _saveData.ReadU32(SaveOffsets.TOASYS_CLEAR_COUNT) != 0;
                try { SetNumericSafe(numToasysEscape, _saveData.ReadU32(SaveOffsets.TOASYS_ESCAPE)); } catch { }
                try { SetNumericSafe(numToasysMaxDamage, _saveData.ReadU32(SaveOffsets.TOASYS_MAX_DAMAGE)); } catch { }
                try { SetNumericSafe(numToasysMaxCombo, _saveData.ReadU32(SaveOffsets.TOASYS_MAX_COMBO)); } catch { }
                try { SetNumericSafe(numToasysDamageDealt, _saveData.ReadU32(SaveOffsets.TOASYS_DAMAGE_DEALT)); } catch { }
                try { SetNumericSafe(numToasysDamageTaken, _saveData.ReadU32(SaveOffsets.TOASYS_DAMAGE_TAKEN)); } catch { }
                try { SetNumericSafe(numToasysBattleTime, _saveData.ReadU32(SaveOffsets.TOASYS_BATTLE_TIME)); } catch { }
                for (int i = 0; i < SaveOffsets.TOASYS_CHAR_USAGE_COUNT; i++)
                {
                    try { SetNumericSafe(numToasysCharUsage[i], _saveData.ReadU32(SaveOffsets.TOASYS_CHAR_USAGE + i * 4)); } catch { }
                    UpdateUsagePct(i);
                }
                for (int i = 0; i < SaveOffsets.CHAR_KILL_COUNT; i++)
                {
                    try { SetNumericSafe(numToasysKillCount[i], _saveData.ReadU32(SaveOffsets.TOASYS_CHAR_KILLS + i * 4)); } catch { }
                }
            }
            finally
            {
                _loading = false;
            }
        }

        // 0x04 是版本号 float 0.2（sub_37D584 固定写入），控件已设为只读，不再写回
        private void numToasysDifficulty_ValueChanged(object sender, EventArgs e) { }
        private void numToasysGald_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_GALD_MAX, (uint)numToasysGald.Value); }
        private void numToasysPlaytime_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_PLAYTIME_MAX, (uint)numToasysPlaytime.Value); }
        private void numToasysGaldSpent_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_GALD_SPENT, (uint)numToasysGaldSpent.Value); }
        private void numToasysSaveCount_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_SAVE_COUNT, (uint)numToasysSaveCount.Value); }
        private void numToasysEncounter_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_ENCOUNTER, (uint)numToasysEncounter.Value); for (int i = 0; i < SaveOffsets.TOASYS_CHAR_USAGE_COUNT; i++) UpdateUsagePct(i); }
        private void numToasysClearCount_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            _saveData.WriteU32(SaveOffsets.TOASYS_CLEAR_COUNT, (uint)numToasysClearCount.Value);
            chkSoundTest.Checked = numToasysClearCount.Value > 0;
        }
        private void numToasysEscape_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_ESCAPE, (uint)numToasysEscape.Value); }
        private void numToasysMaxDamage_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_MAX_DAMAGE, (uint)numToasysMaxDamage.Value); }
        private void numToasysMaxCombo_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_MAX_COMBO, (uint)numToasysMaxCombo.Value); }
        private void numToasysDamageDealt_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_DAMAGE_DEALT, (uint)numToasysDamageDealt.Value); }
        private void numToasysDamageTaken_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_DAMAGE_TAKEN, (uint)numToasysDamageTaken.Value); }
        // 战斗总时间为游戏内统计汇总（0x5D0），只读展示不写回
        private void numToasysCharUsage_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = (int)((NumericUpDown)sender).Tag;
            _saveData.WriteU32(SaveOffsets.TOASYS_CHAR_USAGE + idx * 4, (uint)((NumericUpDown)sender).Value);
            UpdateUsagePct(idx);
        }

        private void numToasysKillCount_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = (int)((NumericUpDown)sender).Tag;
            _saveData.WriteU32(SaveOffsets.TOASYS_CHAR_KILLS + idx * 4, (uint)((NumericUpDown)sender).Value);
        }

        // 使用率% = 计数 ÷ 遭遇数（0x1C）
        private void UpdateUsagePct(int idx)
        {
            if (lblToasysUsagePct == null || lblToasysUsagePct.Length <= idx) return;
            if (numToasysCharUsage == null || numToasysCharUsage.Length <= idx || numToasysEncounter == null) return;
            uint enc = (uint)numToasysEncounter.Value;
            if (enc == 0) { lblToasysUsagePct[idx].Text = "-"; return; }
            double pct = (double)(uint)numToasysCharUsage[idx].Value * 100.0 / enc;
            lblToasysUsagePct[idx].Text = pct.ToString("F1") + "%";
        }

        // 音效测试等通关后菜单 = 通关次数≠0（sub_333800/sub_333174 菜单构建）：
        // 勾选时若原值为 0 则写 1（保留已有更高值）；取消勾选清 0
        private void chkSoundTest_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            uint cur = _saveData.ReadU32(SaveOffsets.TOASYS_CLEAR_COUNT);
            if (chkSoundTest.Checked)
            {
                if (cur == 0)
                {
                    _saveData.WriteU32(SaveOffsets.TOASYS_CLEAR_COUNT, 1);
                    SetNumericSafe(numToasysClearCount, 1);
                }
            }
            else if (cur != 0)
            {
                _saveData.WriteU32(SaveOffsets.TOASYS_CLEAR_COUNT, 0);
                SetNumericSafe(numToasysClearCount, 0);
            }
        }

        // 收集累计全开：0x6C4 起 128B 位图全 FF（byte_53CFE0，含音效曲目等累计解锁）
        private void btnToasysUnlockAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.Toasys) return;
            byte[] fill = new byte[SaveOffsets.TOASYS_UNLOCK_BITMAP_SIZE];
            for (int i = 0; i < fill.Length; i++) fill[i] = 0xFF;
            _saveData.WriteBytes(SaveOffsets.TOASYS_UNLOCK_BITMAP, fill);
            MessageBox.Show(LangText("收集累计已全开（保存后生效）。", "コレクション累計を全開放しました（保存後に有効）。"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() { return Name; }
        }

        #region 剧情跳跃标签页

        private StoryJumpDatabase.JumpEntry _selectedJumpEntry;

        /// <summary>
        /// 刷新剧情跳跃标签页的当前进度显示
        /// </summary>
        private void RefreshStoryJumpTab()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx)
            {
                lblStoryCurrentEventVal.Text = LangText("(未加载)", "(未読み込み)");
                lblStoryCurrentMapVal.Text = LangText("(未加载)", "(未読み込み)");
                lblStoryCurrentChapterVal.Text = LangText("(未加载)", "(未読み込み)");
                lblSideDoneStatus.Text = LangText("(未加载)", "(未読み込み)");
                RefreshAbilityStatus();
                return;
            }

            try
            {
                uint eventId = _saveData.ReadU32(SaveOffsets.BODY_EVENT_ID);
                uint mapId = _saveData.ReadU32(SaveOffsets.BODY_MAP_ID);
                string mapName = StoryJumpDatabase.GetMapName(mapId);
                string eventDesc = StoryJumpDatabase.GetEventDescription(eventId);
                int chapter = StoryJumpDatabase.EventIdToChapter(eventId);

                lblStoryCurrentEventVal.Text = string.Format("{0} ({1})", eventDesc, eventId);
                lblStoryCurrentMapVal.Text = string.Format("{0} ({1})", mapName, mapId);
                lblStoryCurrentChapterVal.Text = chapter > 0
                    ? string.Format(LangText("第 {0} 章", "第{0}章"), chapter)
                    : LangText("未知/特殊", "不明/特殊");
            }
            catch
            {
                lblStoryCurrentEventVal.Text = LangText("(读取失败)", "(読込失敗)");
            }

            RefreshAbilityStatus();
            RefreshSideDoneStatus();
        }

        /// <summary>缪能力真实存储（MYU 000~004 存档差分 + SB7 ICB415 脚本定案）：
        /// 脚本变量 var149=缪火焰等级(0/1/2)、var150=缪撞击(0/1)、var151=缪飞天(0/1)，值在变量 8 字节结构 +0(value:u32)。
        /// 游戏判定是层级依赖：撞击=任意 var>0；火焰=var150>0 或 var151>0；飞行=var151>0。
        /// 故 var149 单独设只会表现为"撞击"（火焰需撞击/飞行解锁）。
        /// 双击 5 档循环（用户实测定案顺序）：未习得→缪撞击→火焰一级→缪飞天→火焰二级。</summary>
        private static readonly int[][] ABILITY_LEVELS = {
            new[] { 0, 0, 0 },  // 0 未习得
            new[] { 1, 0, 0 },  // 1 缪撞击（var149=1，火焰未解锁→仅撞击）
            new[] { 1, 1, 0 },  // 2 火焰一级（+var150=1 撞击解锁火焰）
            new[] { 1, 1, 1 },  // 3 缪飞天（+var151=1）
            new[] { 2, 1, 1 },  // 4 火焰二级（var149=2）
        };
        private static readonly string[] ABILITY_LEVEL_NAMES_CN = {
            "未习得", "缪撞击", "火焰一级", "缪飞天", "火焰二级"
        };
        private static readonly string[] ABILITY_LEVEL_NAMES_JP = {
            "未習得", "バンプ", "ファイア", "フライ", "ファイア2"
        };
        private static readonly string[] ABILITY_NAMES_CN = { "缪撞击", "缪火焰", "缪飞天" };
        private static readonly string[] ABILITY_NAMES_JP = { "ミュウバンプ", "ミュウファイア", "ミュウフライ" };

        private string AbilityName(int i)
        {
            return LangText(ABILITY_NAMES_CN[i], ABILITY_NAMES_JP[i]);
        }

        private int AbilityVarOffset(int i)
        {
            return SaveOffsets.SCRIPT_VARS + (149 + i) * 8;
        }

        private int GetAbilityValue(int i)
        {
            if (_saveData == null) return 0;
            int max = (i == 0) ? 2 : 1;
            int v = (int)_saveData.ReadU32(AbilityVarOffset(i));
            if (v < 0) v = 0;
            if (v > max) v = max;
            return v;
        }

        private void SetAbilityValue(int i, int v)
        {
            _saveData.WriteU32(AbilityVarOffset(i), (uint)v);
        }

        private int GetAbilityLevel()
        {
            int f = GetAbilityValue(0), b = GetAbilityValue(1), y = GetAbilityValue(2);
            for (int lv = ABILITY_LEVELS.Length - 1; lv >= 0; lv--)
            {
                if (f >= ABILITY_LEVELS[lv][0] && b >= ABILITY_LEVELS[lv][1] && y >= ABILITY_LEVELS[lv][2])
                    return lv;
            }
            return 0;
        }

        private void SetAbilityLevel(int lv)
        {
            SetAbilityValue(0, ABILITY_LEVELS[lv][0]);
            SetAbilityValue(1, ABILITY_LEVELS[lv][1]);
            SetAbilityValue(2, ABILITY_LEVELS[lv][2]);
        }

        /// <summary>刷新缪能力显示（按游戏判定：撞击=任意var>0；火焰=撞击/飞行解锁后看var149等级；飞天=var151>0）。</summary>
        private void RefreshAbilityStatus()
        {
            if (lblAbilityItems == null) return;
            bool hasSave = _saveData != null && _saveData.Type == SaveType.ToaXxx;
            int fire = GetAbilityValue(0);  // var149 火焰等级
            int bump = GetAbilityValue(1);  // var150 撞击
            int fly = GetAbilityValue(2);   // var151 飞天

            for (int i = 0; i < 3; i++)
            {
                string st;
                if (!hasSave)
                    st = LangText("未加载", "未読込");
                else if (i == 0)  // 缪撞击 = 任意 var>0
                {
                    bool ok = fire > 0 || bump > 0 || fly > 0;
                    st = LangText(ok ? "已习得" : "未习得", ok ? "習得済" : "未習得");
                }
                else if (i == 1)  // 缪火焰 = 撞击/飞行解锁后，等级看 var149
                {
                    bool unlocked = bump > 0 || fly > 0;
                    st = !unlocked
                        ? LangText("未习得", "未習得")
                        : (fire >= 2 ? LangText("火焰二级", "ファイア2") : LangText("火焰一级", "ファイア"));
                }
                else  // 缪飞天 = var151>0
                {
                    bool ok = fly > 0;
                    st = LangText(ok ? "已习得" : "未习得", ok ? "習得済" : "未習得");
                }
                lblAbilityItems[i].Text = string.Format("{0}：{1}", AbilityName(i), st);
            }
        }

        /// <summary>双击缪能力标签：按 5 档循环切换（未习得→缪撞击→火焰一级→缪飞天→火焰二级→未习得）。</summary>
        private void lblAbility_DoubleClick(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            var lbl = sender as Label;
            if (lbl == null) return;

            int lv = GetAbilityLevel();
            int nlv = (lv + 1) % ABILITY_LEVELS.Length;
            SetAbilityLevel(nlv);

            _saveData.FixChecksum();
            RefreshAbilityStatus();
            RefreshSideDoneStatus();

            string state = LangText(ABILITY_LEVEL_NAMES_CN[nlv], ABILITY_LEVEL_NAMES_JP[nlv]);
            statusLabel.Text = string.Format(LangText("缪能力：{0}", "ミュウ能力：{0}"), state);
        }

        /// <summary>支线完成状态：按任务级完成标志统计（complete_flag 非 0 的任务中，已完成数）。</summary>
        private void RefreshSideDoneStatus()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx)
            {
                if (lblSideDoneStatus != null)
                    lblSideDoneStatus.Text = LangText("支线任务完成情况：(未加载)", "サブクエスト完了：(未読み込み)");
                if (lstQuestSummary != null) lstQuestSummary.Items.Clear();
                return;
            }
            int done = 0, tracked = 0;
            foreach (int pg in SideQuestJumpDatabase.GetPages())
            {
                var entries = SideQuestJumpDatabase.GetEntries(pg);
                if (entries == null) continue;
                foreach (var ent in entries)
                {
                    if (ent.complete_flag <= 0) continue;
                    tracked++;
                    if (IsFlagSet(ent.complete_flag)) done++;
                }
            }
            lblSideDoneStatus.Text = string.Format(
                LangText("支线任务完成情况：{0} / {1} 个已达成（★=有完成标志的任务；其余任务无独立完成标志）",
                    "サブクエスト完了：{0} / {1}（★=完了フラグありのタスク）"),
                done, tracked);
            PopulateSideQuestList();
            PopulateQuestSummary();
        }

        /// <summary>聚合完成度：按 quest 名聚合多步任务（n/m），显示已完成里程碑数/总数。</summary>
        private void PopulateQuestSummary()
        {
            if (lstQuestSummary == null) return;
            lstQuestSummary.Items.Clear();
            bool hasSave = _saveData != null && _saveData.Type == SaveType.ToaXxx;

            // quest_name -> 去重 complete_flag 列表
            var questFlags = new Dictionary<string, List<int>>();
            foreach (int pg in SideQuestJumpDatabase.GetPages())
            {
                var entries = SideQuestJumpDatabase.GetEntries(pg);
                if (entries == null) continue;
                foreach (var ent in entries)
                {
                    if (ent.quest_total <= 1 || ent.complete_flag <= 0) continue;
                    if (!questFlags.ContainsKey(ent.quest_name))
                        questFlags[ent.quest_name] = new List<int>();
                    if (!questFlags[ent.quest_name].Contains(ent.complete_flag))
                        questFlags[ent.quest_name].Add(ent.complete_flag);
                }
            }

            foreach (var kv in questFlags.OrderBy(k => k.Key))
            {
                int total = kv.Value.Count;
                if (total == 0) continue;  // 无独立完成标志的 quest 不显示
                int done = 0;
                if (hasSave)
                    foreach (int f in kv.Value)
                        if (IsFlagSet(f)) done++;
                string bar = done >= total ? "✔" : (done > 0 ? "◐" : "✘");
                lstQuestSummary.Items.Add(string.Format("{0} {1}（{2}/{3}）", bar, kv.Key, done, total));
            }
        }

        /// <summary>批量设置/清除所有支线任务的完成标志。</summary>
        private void SetAllSideComplete(bool set)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            foreach (int pg in SideQuestJumpDatabase.GetPages())
            {
                var entries = SideQuestJumpDatabase.GetEntries(pg);
                if (entries == null) continue;
                foreach (var ent in entries)
                    if (ent.complete_flag > 0)
                        SetStoryFlag(ent.complete_flag, set);
            }
            _saveData.FixChecksum();
            RefreshSideDoneStatus();
            statusLabel.Text = set
                ? LangText("已全部完成所有支线任务", "すべてのサブクエストを完了にしました")
                : LangText("已重置所有支线任务", "すべてのサブクエストをリセットしました");
        }

        private void btnSideAllDone_Click(object sender, EventArgs e)
        {
            SetAllSideComplete(true);
        }

        private void btnSideAllReset_Click(object sender, EventArgs e)
        {
            SetAllSideComplete(false);
        }

        private List<StoryJumpDatabase.JumpEntry> _currentChapterEntries = new List<StoryJumpDatabase.JumpEntry>();
        private List<StoryJumpDatabase.JumpEntry> _currentStoryEntries = new List<StoryJumpDatabase.JumpEntry>();

        private void cmbStoryChapter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentChapterEntries.Clear();
            int idx = cmbStoryChapter.SelectedIndex;
            if (idx >= 0)
            {
                var chapters = new List<int>(StoryJumpDatabase.GetChapters());
                if (idx < chapters.Count)
                {
                    var ch = StoryJumpDatabase.GetChapter(chapters[idx]);
                    if (ch != null)
                        foreach (var item in ch.items)
                            if (!string.IsNullOrEmpty(item.menu_name))
                                _currentChapterEntries.Add(item);
                }
            }
            PopulateStoryBranches();
        }

        private void PopulateStoryBranches()
        {
            lstStoryBranches.Items.Clear();
            _currentStoryEntries.Clear();
            _selectedJumpEntry = null;
            btnStoryJump.Enabled = false;
            btnStoryJumpNoEvent.Enabled = false;
            lblStoryTargetInfo.Text = "";

            string kw = txtStorySearch != null ? txtStorySearch.Text.Trim() : "";
            if (kw.Length == 0)
            {
                // 无搜索词：显示当前章节分支
                foreach (var item in _currentChapterEntries)
                {
                    _currentStoryEntries.Add(item);
                    lstStoryBranches.Items.Add(item.menu_name);
                }
            }
            else
            {
                // 全局搜索：跨所有章节
                foreach (int chNum in StoryJumpDatabase.GetChapters())
                {
                    var ch = StoryJumpDatabase.GetChapter(chNum);
                    if (ch == null) continue;
                    foreach (var item in ch.items)
                    {
                        if (string.IsNullOrEmpty(item.menu_name)) continue;
                        if (item.menu_name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) < 0) continue;
                        _currentStoryEntries.Add(item);
                        lstStoryBranches.Items.Add(string.Format("【{0}】{1}", ch.chapter_name, item.menu_name));
                    }
                }
            }
        }

        private void txtStorySearch_TextChanged(object sender, EventArgs e)
        {
            PopulateStoryBranches();
        }

        private void lstStoryBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstStoryBranches.SelectedIndex;
            if (idx < 0 || idx >= _currentStoryEntries.Count)
            {
                _selectedJumpEntry = null;
                btnStoryJump.Enabled = false;
                btnStoryJumpNoEvent.Enabled = false;
                lblStoryTargetInfo.Text = "";
                return;
            }
            _selectedJumpEntry = _currentStoryEntries[idx];

            bool hasJumpData = (_selectedJumpEntry.map_id != 0 || _selectedJumpEntry.event_id != 0)
                && _selectedJumpEntry.map_id < 650;  // >=8000 为场景事件ID（运行时触发，无法用存档地图ID跳转）
            bool isSceneEvent = _selectedJumpEntry.map_id >= 8000;
            btnStoryJump.Enabled = hasJumpData;
            btnStoryJumpNoEvent.Enabled = hasJumpData;

            if (!hasJumpData)
            {
                if (isSceneEvent)
                {
                    lblStoryTargetInfo.Text = _selectedJumpEntry.menu_name + "\n\n" +
                        LangText("（场景事件，需运行时触发，无法用存档跳转）", "（シーンイベント、ランタイム専用）");
                }
                else
                {
                    lblStoryTargetInfo.Text = _selectedJumpEntry.menu_name + "\n\n" +
                        LangText("（此分支无跳转数据）", "（このシーンにジャンプデータなし）");
                }
                return;
            }

            string mapName = _selectedJumpEntry.map_name;
            if (string.IsNullOrEmpty(mapName))
                mapName = StoryJumpDatabase.GetMapName(_selectedJumpEntry.map_id);
            string eventDesc = _selectedJumpEntry.event_id == 0
                ? LangText("无事件（自定义画面）", "イベントなし")
                : StoryJumpDatabase.GetEventDescription(_selectedJumpEntry.event_id);

            bool hasCoord = _selectedJumpEntry.x != 0 || _selectedJumpEntry.y != 0
                || _selectedJumpEntry.z != 0 || _selectedJumpEntry.angle != 0;
            string coordText = hasCoord
                ? string.Format(LangText("坐标 X={0} Y={1} Z={2} 朝向={3}°", "座標 X={0} Y={1} Z={2} 向き={3}°"),
                    _selectedJumpEntry.x, _selectedJumpEntry.y, _selectedJumpEntry.z, _selectedJumpEntry.angle)
                : LangText("坐标：地图默认出生点", "座標：マップ初期位置");

            lblStoryTargetInfo.Text = string.Format(
                "{0}\n\n{1}: {2} ({3})\n{4}: {5} ({6})\n{7}",
                _selectedJumpEntry.menu_name,
                LangText("目标地图", "目標マップ"), mapName, _selectedJumpEntry.map_id,
                LangText("目标事件", "目標イベント"), eventDesc, _selectedJumpEntry.event_id,
                coordText);
        }

        private void btnStoryJump_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (_selectedJumpEntry == null) return;

            ExecuteStoryJump(_selectedJumpEntry.map_id, _selectedJumpEntry.event_id, true,
                _selectedJumpEntry.x, _selectedJumpEntry.y, _selectedJumpEntry.z, _selectedJumpEntry.angle,
                _selectedJumpEntry.set_flags, _selectedJumpEntry.clear_flags);
        }

        private void btnStoryJumpNoEvent_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (_selectedJumpEntry == null) return;

            ExecuteStoryJump(_selectedJumpEntry.map_id, 0, false,
                _selectedJumpEntry.x, _selectedJumpEntry.y, _selectedJumpEntry.z, _selectedJumpEntry.angle,
                _selectedJumpEntry.set_flags, _selectedJumpEntry.clear_flags);
        }

        private void ExecuteStoryJump(uint mapId, uint eventId, bool writeEvent, float x, float y, float z, float angle,
            System.Collections.Generic.List<int> setFlags, System.Collections.Generic.List<int> clearFlags)
        {
            try
            {
                _loading = true;

                // 写地图ID
                _saveData.WriteU32(SaveOffsets.BODY_MAP_ID, mapId);

                // 同步头部地图信息（BuildToaSaveBuffer sub_37C948 每次存档都会重写这两处；
                // 游戏读档/存档界面显示的地图ID(0x34)和地名(0x4C)必须一起改，否则跳转后
                // 界面仍显示旧地图）。0x4C = maptable 显示名（TBL 编码，sub_2D8E14 原样 strcpy）。
                _saveData.HeadMapId = mapId;
                if (mapId >= 1 && mapId < 650)
                {
                    string mapDispName = StoryJumpDatabase.GetMapName(mapId);
                    if (!string.IsNullOrEmpty(mapDispName) && !mapDispName.StartsWith("<"))
                    {
                        // PS2 存档：DB 显示名为 3DS 中文译文，用 cp932 写入会产生大量 '?' 替换
                        // → 记忆卡列表显示会乱码；游戏下次内存档会按 JP 码表重写正确地名。
                        // PS2 跳过 0x4C 写入；3DS 仍按 TBL 编码写入（编码失败也只是跳过，不影响跳转本身）。
                        if (_saveData.Platform != Platform.Ps2)
                            _saveData.WriteLocationName(mapDispName, out string nameErr);
                    }
                }

                // 写坐标（入口点编号转 float，与 MapWarp 命令一致）
                // fallback 条件 = X==0 且 Z==0 时才用默认 X=0.01/Z=0.02（反汇编 sub_1A149C:
                //   VCMP X,0; VLDREQ Z; VCMPEQ Z,0 → 仅当 X 和 Z 都为 0 才 fallback，缺一不可）
                float fx = x;
                float fz = z;
                if (fx == 0.0f && fz == 0.0f)
                {
                    fx = 0.01f;
                    fz = 0.02f;
                }
                _saveData.WriteFloat(SaveOffsets.BODY_PLAYER_X, fx);
                _saveData.WriteFloat(SaveOffsets.BODY_PLAYER_Y, y);
                _saveData.WriteFloat(SaveOffsets.BODY_PLAYER_Z, fz);
                _saveData.WriteFloat(SaveOffsets.BODY_PLAYER_ANGLE, angle);

                // 写事件ID（关键：触发对应剧情脚本）
                if (writeEvent)
                {
                    _saveData.WriteU32(SaveOffsets.BODY_EVENT_ID, eventId);
                }

                // 写剧情 flag（SET_FLAG/CLEAR_FLAG，关键：部分分支依赖 flag 区分支线/状态）
                // flag N 存储在 file FLAG_BITMAP(0x218) + N/8 的 bit(N%8)
                if (setFlags != null)
                    foreach (int f in setFlags) SetStoryFlag(f, true);
                if (clearFlags != null)
                    foreach (int f in clearFlags) SetStoryFlag(f, false);

                // 重算双校验和（PS2/3DS 按平台分发）
                _saveData.FixChecksum();

                _loading = false;
                RefreshStoryJumpTab();

                string mode = writeEvent
                    ? LangText("地图+事件", "マップ+イベント")
                    : LangText("仅地图", "マップのみ");
                string mapName = StoryJumpDatabase.GetMapName(mapId);
                statusLabel.Text = string.Format(LangText("已跳转: {0} -> {1} ({2})", "ジャンプ完了: {0} → {1} ({2})"),
                    mode, mapName, mapId);
            }
            catch (Exception ex)
            {
                _loading = false;
                System.Windows.Forms.MessageBox.Show(
                    string.Format(LangText("跳转失败: {0}", "ジャンプ失敗: {0}"), ex.Message),
                    LangText("错误", "エラー"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>设置/清除剧情 flag。flag N 位于 file FLAG_BITMAP(0x218) + N/8 字节的 bit(N%8)。越界 flag 号直接忽略。</summary>
        private void SetStoryFlag(int flagNum, bool set)
        {
            if (flagNum < 0 || flagNum >= SaveOffsets.FLAG_BITMAP_SIZE * 8) return;
            int off = SaveOffsets.FLAG_BITMAP + flagNum / 8;
            int bit = flagNum % 8;
            byte b = _saveData.Buffer[off];
            if (set)
                b = (byte)(b | (1 << bit));
            else
                b = (byte)(b & ~(1 << bit));
            _saveData.Buffer[off] = b;
        }

        #region 支线跳跃标签页

        private SideQuestJumpDatabase.SideEntry _selectedSideEntry;
        private List<SideQuestJumpDatabase.SideEntry> _sideEntriesForCurrentPage = new List<SideQuestJumpDatabase.SideEntry>();

        /// <summary>读取剧情 flag。flag N 位于 file FLAG_BITMAP(0x218) + N/8 字节的 bit(N%8)。越界返回 false。</summary>
        private bool IsFlagSet(int flagNum)
        {
            if (_saveData == null || flagNum <= 0 || flagNum >= SaveOffsets.FLAG_BITMAP_SIZE * 8) return false;
            return (_saveData.Buffer[SaveOffsets.FLAG_BITMAP + flagNum / 8] & (1 << (flagNum % 8))) != 0;
        }

        /// <summary>当前任务的完成状态文字。</summary>
        private string SideEntryStatus(SideQuestJumpDatabase.SideEntry ent)
        {
            if (ent.complete_flag <= 0)
                return LangText("—", "—");
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx)
                return LangText("未加载", "未読込");
            return IsFlagSet(ent.complete_flag)
                ? LangText("已完成", "完了")
                : LangText("未完成", "未完了");
        }

        private void PopulateSideQuestList()
        {
            lstSideQuests.Items.Clear();
            _sideEntriesForCurrentPage.Clear();
            _selectedSideEntry = null;
            btnSideJump.Enabled = false;
            btnSideToggleComplete.Enabled = false;
            lblSideTargetInfo.Text = "";

            string kw = txtSideSearch != null ? txtSideSearch.Text.Trim() : "";
            bool hasSave = _saveData != null && _saveData.Type == SaveType.ToaXxx;

            // 构建 (页前缀, entry) 显示列表
            var show = new List<KeyValuePair<string, SideQuestJumpDatabase.SideEntry>>();
            if (kw.Length == 0)
            {
                // 无搜索词：显示当前页
                int idx = cmbSidePage.SelectedIndex;
                if (idx < 0) return;
                var pages = new List<int>(SideQuestJumpDatabase.GetPages());
                if (idx >= pages.Count) return;
                var entries = SideQuestJumpDatabase.GetEntries(pages[idx]);
                if (entries == null) return;
                foreach (var ent in entries)
                    show.Add(new KeyValuePair<string, SideQuestJumpDatabase.SideEntry>("", ent));
            }
            else
            {
                // 全局搜索：跨所有页
                foreach (int pg in SideQuestJumpDatabase.GetPages())
                {
                    var entries = SideQuestJumpDatabase.GetEntries(pg);
                    if (entries == null) continue;
                    foreach (var ent in entries)
                        if (ent.name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                            show.Add(new KeyValuePair<string, SideQuestJumpDatabase.SideEntry>(
                                string.Format(LangText("第{0}页 ", "第{0}ページ "), pg), ent));
                }
            }

            foreach (var kv in show)
            {
                var ent = kv.Value;
                _sideEntriesForCurrentPage.Add(ent);
                string marker, status;
                if (ent.complete_flag <= 0) { marker = "· "; status = LangText("—", "—"); }
                else if (!hasSave) { marker = "· "; status = LangText("未加载", "未読込"); }
                else if (IsFlagSet(ent.complete_flag)) { marker = "✔ "; status = LangText("已完成", "完了"); }
                else { marker = "✘ "; status = LangText("未完成", "未完了"); }
                lstSideQuests.Items.Add(string.Format("{0}{1}{2}　[{3}]", marker, kv.Key, ent.name, status));
            }
        }

        private void cmbSidePage_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSideQuestList();
        }

        private void txtSideSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateSideQuestList();
        }

        private void lstSideQuests_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstSideQuests.SelectedIndex;
            if (idx < 0 || idx >= _sideEntriesForCurrentPage.Count)
            {
                _selectedSideEntry = null;
                btnSideJump.Enabled = false;
                btnSideToggleComplete.Enabled = false;
                lblSideTargetInfo.Text = "";
                return;
            }
            _selectedSideEntry = _sideEntriesForCurrentPage[idx];
            btnSideJump.Enabled = true;
            btnSideToggleComplete.Enabled = _selectedSideEntry.complete_flag > 0;

            string mapName = StoryJumpDatabase.GetMapName(_selectedSideEntry.map_id);
            string evtDesc = StoryJumpDatabase.GetEventDescription(_selectedSideEntry.event_id);
            string status = SideEntryStatus(_selectedSideEntry);
            lblSideTargetInfo.Text = string.Format(
                LangText("任务：{0}\n状态：{1}\n事件：{2} ({3})\n地图：{4} ({5})\n坐标：X={6} Y={7} Z={8} 朝向={9}\nSET flag：{10}\nCLR flag：{11}",
                    "タスク：{0}\n状態：{1}\nイベント：{2} ({3})\nマップ：{4} ({5})\n座標：X={6} Y={7} Z={8} 向き={9}\nSET flag：{10}\nCLR flag：{11}"),
                _selectedSideEntry.name, status, evtDesc, _selectedSideEntry.event_id, mapName, _selectedSideEntry.map_id,
                _selectedSideEntry.x, _selectedSideEntry.y, _selectedSideEntry.z, _selectedSideEntry.angle,
                string.Join(",", _selectedSideEntry.set_flags), string.Join(",", _selectedSideEntry.clear_flags));
        }

        private void btnSideJump_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (_selectedSideEntry == null) return;

            ExecuteStoryJump(_selectedSideEntry.map_id, _selectedSideEntry.event_id, true,
                _selectedSideEntry.x, _selectedSideEntry.y, _selectedSideEntry.z, _selectedSideEntry.angle,
                _selectedSideEntry.set_flags, _selectedSideEntry.clear_flags);
        }

        /// <summary>切换所选支线任务的完成状态（set/clear complete_flag）。</summary>
        private void ToggleSideComplete()
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;
            if (_selectedSideEntry == null) return;
            int flag = _selectedSideEntry.complete_flag;
            if (flag <= 0)
            {
                statusLabel.Text = LangText("该任务无独立完成标志，无法切换", "このタスクは個別完了フラグなし");
                return;
            }
            bool cur = IsFlagSet(flag);
            SetStoryFlag(flag, !cur);
            _saveData.FixChecksum();
            int sel = lstSideQuests.SelectedIndex;
            RefreshSideDoneStatus();
            if (sel >= 0 && sel < lstSideQuests.Items.Count)
            {
                lstSideQuests.SelectedIndex = sel;
            }
            statusLabel.Text = string.Format(LangText("「{0}」→ {1}", "「{0}」→ {1}"),
                _selectedSideEntry.name,
                !cur ? LangText("已完成", "完了") : LangText("未完成", "未完了"));
        }

        private void btnSideToggleComplete_Click(object sender, EventArgs e)
        {
            ToggleSideComplete();
        }

        private void lstSideQuests_DoubleClick(object sender, EventArgs e)
        {
            ToggleSideComplete();
        }

        #endregion

        #endregion
    }
}
