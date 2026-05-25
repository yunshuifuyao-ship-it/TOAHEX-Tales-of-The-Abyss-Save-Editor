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
        private ushort[] _arteIds = new ushort[4];
        private uint _currentTitleIndex;
        private Image[] _charPortraits;

        public MainForm()
        {
            InitializeComponent();
            SetControlsEnabled(false);
            LoadCharPortraits();
            LoadDatData();
            LanguageConfig.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RefreshAllUI();
        }

        private void RefreshAllUI()
        {
            this.Text = LangText("TOAHEX v1.0 - Tales of the Abyss Save Editor", "TOAHEX v1.0 - Tales of the Abyss Save Editor");
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
            statusLabel.Text = LangText("未加载存档", "セーブ未読み込み");

            tabGlobal.Text = LangText("全局数据", "全局データ");
            tabCharacter.Text = LangText("角色编辑", "キャラ編集");
            tabItems.Text = LangText("背包管理", "バッグ管理");
            tabCooking.Text = LangText("料理修改", "料理編集");
            tabSystem.Text = LangText("系统数据", "システムデータ");
            tabFSChamber.Text = LangText("谱石管理", "FSチャンバー");

            subTabStats.Text = LangText("基础属性", "基本ステータス");
            tabSubCombat.Text = LangText("战斗属性", "戦闘属性");
            subTabEquip.Text = LangText("装备", "装備");
            subTabArtes.Text = LangText("术技", "アーツ");
            subTabADSkill.Text = LangText("AD技能", "ADスキル");
            subTabTitle.Text = LangText("称号", "称号");

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
                MessageBox.Show(LangText("文件大小不匹配！\nTOA_XXX应为49120字节，TOASYS应为1860字节。", "ファイルサイズが一致しません！\nTOA_XXXは49120バイト、TOASYSは1860バイトである必要があります。"), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                tabControl.TabPages.Remove(tabGlobal);
                tabControl.TabPages.Remove(tabCharacter);
                tabControl.TabPages.Remove(tabItems);
                tabControl.TabPages.Remove(tabCooking);
                tabControl.TabPages.Remove(tabFSChamber);
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
                tabControl.TabPages.Remove(tabSystem);
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
                    dlg.Filter = LangText("TOA存档文件|TOA_*;TOASYS|所有文件|*.*", "TOAセーブファイル|TOA_*;TOASYS|すべてのファイル|*.*");
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
            bool ok = _saveData.VerifyChecksum();
            statusLabel.Text = $"{type} | {_saveData.FilePath} | {LangText("校验和", "チェックサム")}: {(ok ? LangText("通过", "OK") : LangText("失败", "NG"))}";
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
                try { lblDifficulty.Text = _saveData.Difficulty.ToString("F1"); } catch { lblDifficulty.Text = "-"; }
                try { lblPartyCount.Text = _saveData.PartyCount.ToString(); } catch { lblPartyCount.Text = "-"; }
                try { lblLocation.Text = _saveData.LocationName; } catch { lblLocation.Text = "-"; }

                try { SetNumericSafe(numEncount, _saveData.ReadU32(SaveOffsets.HEAD_ENCOUNTER)); } catch { }
                try { SetNumericSafe(numHit, _saveData.ReadU32(SaveOffsets.HEAD_HIT)); } catch { }

                try { decimal gradeVal = (decimal)_saveData.Grade; if (gradeVal < numGrade.Minimum) gradeVal = numGrade.Minimum; if (gradeVal > numGrade.Maximum) gradeVal = numGrade.Maximum; numGrade.Value = gradeVal; } catch { numGrade.Value = 0; }

                try
                {
                    uint diffVal = _saveData.ReadU32(SaveOffsets.HEAD_DIFFICULTY);
                    if (diffVal < 4) cmbDifficulty.SelectedIndex = (int)diffVal;
                    else cmbDifficulty.SelectedIndex = 3;
                }
                catch { cmbDifficulty.SelectedIndex = 0; }

                try
                {
                    byte featureFlags = _saveData.ReadU8(SaveOffsets.BODY_FEATURE_FLAGS);
                    chkCCore.Checked = (featureFlags & 0x01) != 0;
                    chkFSChamber.Checked = (featureFlags & 0x02) != 0;
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

                if (cmbLeader != null)
                {
                    try
                    {
                        int leaderIdx = _saveData.ReadU8(SaveOffsets.BODY_LEADER);
                        if (leaderIdx <= 0 || leaderIdx > 7) leaderIdx = 1;
                        cmbLeader.SelectedIndex = leaderIdx - 1;
                    }
                    catch { cmbLeader.SelectedIndex = 0; }
                }



            }
            finally
            {
                _loading = false;
            }
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

        private void cmbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int diffIdx = cmbDifficulty.SelectedIndex;
            if (diffIdx >= 0 && diffIdx < 4)
            {
                _saveData.WriteU32(SaveOffsets.HEAD_DIFFICULTY, (uint)diffIdx);
                _saveData.WriteU8(SaveOffsets.BODY_DIFFICULTY, (byte)diffIdx);
            }
        }

        private void numGrade_ValueChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            _saveData.Grade = (float)numGrade.Value;
        }

        private void chkCCore_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            byte flags = _saveData.ReadU8(SaveOffsets.BODY_FEATURE_FLAGS);
            if (chkCCore.Checked) flags |= 0x01; else flags &= unchecked((byte)~0x01);
            _saveData.WriteU8(SaveOffsets.BODY_FEATURE_FLAGS, flags);
        }

        private void chkFSChamber_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            byte flags = _saveData.ReadU8(SaveOffsets.BODY_FEATURE_FLAGS);
            if (chkFSChamber.Checked) flags |= 0x02; else flags &= unchecked((byte)~0x02);
            _saveData.WriteU8(SaveOffsets.BODY_FEATURE_FLAGS, flags);
        }

        private void btnJournalAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                byte[] fill = new byte[SaveOffsets.JOURNAL_FLAGS_SIZE];
                for (int i = 0; i < fill.Length; i++) fill[i] = 0xFF;
                _saveData.WriteBytes(SaveOffsets.JOURNAL_FLAGS_OFFSET, fill);
                MessageBox.Show(LangText("日志已全开。", "Journalを全開放しました。"), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LangText("日志全开失败：\n{0}", "Journal全開放失敗：\n{0}"), ex.Message), LangText("错误", "エラー"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemBookAll_Click(object sender, EventArgs e)
        {
            if (_saveData == null || _saveData.Type != SaveType.ToaXxx) return;

            try
            {
                byte[] fill = new byte[SaveOffsets.BOOK_FLAGS_SIZE];
                for (int i = 0; i < fill.Length; i++) fill[i] = 0xFF;
                _saveData.WriteBytes(SaveOffsets.BOOK_FLAGS_OFFSET, fill);

                _saveData.WriteU8(SaveOffsets.BOOK_DETAIL_FLAGS, 0xFF);
                _saveData.WriteU8(SaveOffsets.BOOK_DETAIL_FLAGS + 1, 0xFF);

                byte[] detailFill = new byte[SaveOffsets.BOOK_DETAIL_DATA_SIZE];
                for (int i = 0; i < detailFill.Length; i++) detailFill[i] = 0x01;
                _saveData.WriteBytes(SaveOffsets.BOOK_DETAIL_DATA, detailFill);

                int itemCount = 0;
                for (int i = 0; i < SaveOffsets.BODY_ITEM_COUNT; i++)
                {
                    byte val = _saveData.ReadU8(SaveOffsets.BODY_ITEM_ARRAY + i);
                    if (val == 0)
                    {
                        _saveData.WriteU8(SaveOffsets.BODY_ITEM_ARRAY + i, 1);
                        itemCount++;
                    }
                }

                RefreshItemsTab();
                MessageBox.Show(string.Format(LangText("道具图鉴已全开，获得 {0} 个新道具。", "アイテム図鑑を全開にし、{0}個の新アイテムを獲得しました。"), itemCount), LangText("提示", "情報"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var items = ItemDatabase.GetByCategory(category);
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

        private void cmbLeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int leaderVal = cmbLeader.SelectedIndex + 1;
            if (leaderVal < 1) leaderVal = 1;
            _saveData.WriteU8(SaveOffsets.BODY_LEADER, (byte)leaderVal);
            _saveData.WriteU8(SaveOffsets.BODY_SUB_LEADER, (byte)leaderVal);
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
            MessageBox.Show(LangText("所有角色AD技能已全开！", "全キャラクターのADスキルを全解放しました！"), LangText("完成", "完了"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #endregion

        #region 角色编辑页

        private static readonly string[] CharNames = { "", "Luke", "Tear", "Jade", "Anise", "Guy", "Natalia", "Asch" };

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
                try { SetNumericSafe(numGrowthPoints, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_GROWTH_POINTS)); } catch { numGrowthPoints.Value = numGrowthPoints.Minimum; }

                int charIdx = cmbCharSelect.SelectedIndex + 1;
                try { PopulateEquipCombo(cmbWeapon, charIdx, 0, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 0 * 2)); } catch { }
                try { PopulateEquipCombo(cmbArmor, charIdx, 1, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 1 * 2)); } catch { }
                try { PopulateEquipCombo(cmbAcc1, charIdx, 2, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 2 * 2)); } catch { }
                try { PopulateEquipCombo(cmbAcc2, charIdx, 3, _saveData.ReadU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 3 * 2)); } catch { }
                try { PopulateKyouritsufuCombo(cmbKyouritsufu, _saveData.ReadKyouritsufu(charIdx)); } catch { }

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

        private void PopulateEquipCombo(ComboBox cmb, int charIndex, int slotIndex, ushort currentValue)
        {
            try
            {
                cmb.BeginUpdate();
                cmb.Items.Clear();
                string noneText = LangText("(无)", "(なし)");
                cmb.Items.Add(new ComboItem { Id = 0, Name = noneText });

                foreach (var item in EquipIndexDatabase.GetEquipItemsForSlot(charIndex, slotIndex))
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    cmb.Items.Add(new ComboItem { Id = item.Id, Name = item.Name });
                }

                bool found = false;
                for (int i = 0; i < cmb.Items.Count; i++)
                {
                    if (((ComboItem)cmb.Items[i]).Id == (int)currentValue)
                    {
                        cmb.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (currentValue != 0)
                    {
                        string itemName = ItemDatabase.GetById(currentValue)?.Name;
                        string display = !string.IsNullOrEmpty(itemName) ? itemName : string.Format("(ID:{0})", currentValue);
                        cmb.Items.Add(new ComboItem { Id = (int)currentValue, Name = display });
                        cmb.SelectedIndex = cmb.Items.Count - 1;
                    }
                    else
                    {
                        cmb.SelectedIndex = 0;
                    }
                }
            }
            catch { }
            finally
            {
                try { cmb.EndUpdate(); } catch { }
                if (cmb.SelectedIndex < 0 && cmb.Items.Count > 0)
                    cmb.SelectedIndex = 0;
            }
        }

        private void PopulateKyouritsufuCombo(ComboBox cmb, ushort currentValue)
        {
            try
            {
                cmb.BeginUpdate();
                cmb.Items.Clear();
                foreach (var entry in KyouritsufuDatabase.GetAll())
                {
                    cmb.Items.Add(new ComboItem { Id = entry.Id, Name = entry.Name });
                }
                bool found = false;
                for (int i = 0; i < cmb.Items.Count; i++)
                {
                    if (((ComboItem)cmb.Items[i]).Id == (int)currentValue)
                    {
                        cmb.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (currentValue != 0)
                    {
                        string display = KyouritsufuDatabase.GetName(currentValue);
                        if (string.IsNullOrEmpty(display)) display = string.Format("(ID:{0})", currentValue);
                        cmb.Items.Add(new ComboItem { Id = (int)currentValue, Name = display });
                        cmb.SelectedIndex = cmb.Items.Count - 1;
                    }
                    else
                    {
                        cmb.SelectedIndex = 0;
                    }
                }
            }
            catch { }
            finally
            {
                try { cmb.EndUpdate(); } catch { }
                if (cmb.SelectedIndex < 0 && cmb.Items.Count > 0)
                    cmb.SelectedIndex = 0;
            }
        }

        private void cmbKyouritsufu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _saveData == null) return;
            int idx = cmbCharSelect.SelectedIndex + 1;
            if (idx < 1 || idx > 7) return;
            if (cmbKyouritsufu.SelectedItem != null)
                _saveData.WriteKyouritsufu(idx, (ushort)((ComboItem)cmbKyouritsufu.SelectedItem).Id);
        }

        private void numLevel_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); uint packed = _saveData.ReadU32(baseOff + SaveOffsets.CHAR_LEVEL); packed = (packed & 0xFFFFFF00) | ((uint)numLevel.Value & 0xFF); _saveData.WriteU32(baseOff + SaveOffsets.CHAR_LEVEL, packed); }
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
        private void numGrowthPoints_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); _saveData.WriteU16(baseOff + SaveOffsets.CHAR_GROWTH_POINTS, (ushort)numGrowthPoints.Value); }

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

        private void cmbWeapon_SelectedIndexChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); if (cmbWeapon.SelectedItem != null) _saveData.WriteU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 0 * 2, (ushort)((ComboItem)cmbWeapon.SelectedItem).Id); }
        private void cmbArmor_SelectedIndexChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); if (cmbArmor.SelectedItem != null) _saveData.WriteU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 1 * 2, (ushort)((ComboItem)cmbArmor.SelectedItem).Id); }
        private void cmbAcc1_SelectedIndexChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); if (cmbAcc1.SelectedItem != null) _saveData.WriteU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 2 * 2, (ushort)((ComboItem)cmbAcc1.SelectedItem).Id); }
        private void cmbAcc2_SelectedIndexChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = cmbCharSelect.SelectedIndex + 1; if (idx < 1 || idx > 7) return; int baseOff = _saveData.GetCharBaseOffset(idx); if (cmbAcc2.SelectedItem != null) _saveData.WriteU16(baseOff + SaveOffsets.CHAR_EQUIP_ARRAY + 3 * 2, (ushort)((ComboItem)cmbAcc2.SelectedItem).Id); }

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
                    if (clbTitles.GetItemChecked(i))
                        obtainedFlags |= (1u << (i + 1));
                    else
                        obtainedFlags &= ~(1u << (i + 1));
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
                string name = item != null && !string.IsNullOrEmpty(item.Name) ? item.Name : $"(ID {i})";
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

        private void ApplyItemFilter()
        {
            if (_itemTable == null) return;
            string category = cmbItemCategory.SelectedItem as string;
            if (category == null) return;

            if (category == "全部")
            {
                _itemTable.DefaultView.RowFilter = "";
                return;
            }

            var ids = ItemDatabase.GetByCategory(category).Select(i => i.Id).ToList();
            if (ids.Count == 0)
            {
                _itemTable.DefaultView.RowFilter = "ID = -1";
                return;
            }

            _itemTable.DefaultView.RowFilter = $"ID IN ({string.Join(",", ids)})";
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loading || _saveData == null || _itemTable == null) return;
            if (e.RowIndex < 0) return;
            string qtyCol = LangText("数量", "数量");
            try
            {
                DataRow row = _itemTable.Rows[e.RowIndex];
                int id = (int)row["ID"];
                int qty = (int)row[qtyCol];
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
                try { numToasysDifficulty.Value = (decimal)_saveData.ReadFloat(SaveOffsets.TOASYS_DIFFICULTY); } catch { }
                try { SetNumericSafe(numToasysGald, _saveData.ReadU32(SaveOffsets.TOASYS_GALD_COPY)); } catch { }
                try { SetNumericSafe(numToasysPlaytime, _saveData.ReadU32(SaveOffsets.TOASYS_PLAYTIME_COPY)); } catch { }
                try { SetNumericSafe(numToasysTotalTime, _saveData.ReadU32(SaveOffsets.TOASYS_TOTAL_TIME)); } catch { }
                try { SetNumericSafe(numToasysSaveCount, _saveData.ReadU32(SaveOffsets.TOASYS_SAVE_COUNT)); } catch { }
                try { SetNumericSafe(numToasysSysFlag1, _saveData.ReadU32(SaveOffsets.TOASYS_SYS_FLAG1)); } catch { }
                try { SetNumericSafe(numToasysSysFlag2, _saveData.ReadU32(SaveOffsets.TOASYS_SYS_FLAG2)); } catch { }
                try { SetNumericSafe(numToasysSysFlag3, _saveData.ReadU32(SaveOffsets.TOASYS_SYS_FLAG3)); } catch { }
                try { SetNumericSafe(numToasysEncounter, _saveData.ReadU32(SaveOffsets.TOASYS_ENCOUNTER)); } catch { }
                string[] charNames = { "Luke", "Tear", "Jade", "Anise", "Guy", "Natalia", "Asch" };
                for (int i = 0; i < 7; i++)
                {
                    try { SetNumericSafe(numToasysCharUsage[i], _saveData.ReadU32(SaveOffsets.TOASYS_CHAR_USAGE + i * 4)); } catch { }
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void numToasysDifficulty_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteFloat(SaveOffsets.TOASYS_DIFFICULTY, (float)numToasysDifficulty.Value); }
        private void numToasysGald_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_GALD_COPY, (uint)numToasysGald.Value); }
        private void numToasysPlaytime_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_PLAYTIME_COPY, (uint)numToasysPlaytime.Value); }
        private void numToasysTotalTime_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_TOTAL_TIME, (uint)numToasysTotalTime.Value); }
        private void numToasysSaveCount_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_SAVE_COUNT, (uint)numToasysSaveCount.Value); }
        private void numToasysSysFlag1_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_SYS_FLAG1, (uint)numToasysSysFlag1.Value); }
        private void numToasysSysFlag2_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_SYS_FLAG2, (uint)numToasysSysFlag2.Value); }
        private void numToasysSysFlag3_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_SYS_FLAG3, (uint)numToasysSysFlag3.Value); }
        private void numToasysEncounter_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; _saveData.WriteU32(SaveOffsets.TOASYS_ENCOUNTER, (uint)numToasysEncounter.Value); }
        private void numToasysCharUsage_ValueChanged(object sender, EventArgs e) { if (_loading || _saveData == null) return; int idx = (int)((NumericUpDown)sender).Tag; _saveData.WriteU32(SaveOffsets.TOASYS_CHAR_USAGE + idx * 4, (uint)((NumericUpDown)sender).Value); }

        #endregion

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() { return Name; }
        }
    }
}
