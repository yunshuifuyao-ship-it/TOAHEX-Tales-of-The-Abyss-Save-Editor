using System;
using System.Drawing;
using System.Windows.Forms;

namespace TOAHEX
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip = new MenuStrip();
            this.menuFile = new ToolStripMenuItem();
            this.menuFileOpen = new ToolStripMenuItem();
            this.menuFileSave = new ToolStripMenuItem();
            this.menuFileSaveAs = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.menuFileExit = new ToolStripMenuItem();
            this.menuLanguage = new ToolStripMenuItem();
            this.menuLangCN = new ToolStripMenuItem();
            this.menuLangJP = new ToolStripMenuItem();
            this.menuHelp = new ToolStripMenuItem();
            this.menuHelpAbout = new ToolStripMenuItem();
            this.menuTools = new ToolStripMenuItem();
            this.menuToolsCharName = new ToolStripMenuItem();
            this.menuToolsConvertPs2 = new ToolStripMenuItem();
            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();
            this.tabControl = new TabControl();
            this.tabGlobal = new TabPage();
            this.tabCharacter = new TabPage();
            this.tabItems = new TabPage();
            this.tabCooking = new TabPage();
            this.tabFSChamber = new TabPage();
            this.tabSystem = new TabPage();
            this.tabSystemKills = new TabPage();
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileExit.Name = "menuFileExit";

            this.menuStrip.Items.AddRange(new ToolStripItem[] { this.menuFile, this.menuLanguage, this.menuHelp, this.menuTools });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(780, 25);
            this.menuStrip.TabIndex = 0;

            this.menuFile.DropDownItems.AddRange(new ToolStripItem[] {
                this.menuFileOpen, this.menuFileSave, this.menuFileSaveAs, this.toolStripSeparator1, this.menuFileExit });
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new Size(37, 21);
            this.menuFile.Text = LangText("文件", "ファイル");

            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.Size = new Size(124, 22);
            this.menuFileOpen.Text = LangText("打开", "開く");
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);

            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.Size = new Size(124, 22);
            this.menuFileSave.Text = LangText("保存", "保存");
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);

            this.menuFileSaveAs.Name = "menuFileSaveAs";
            this.menuFileSaveAs.Size = new Size(124, 22);
            this.menuFileSaveAs.Text = LangText("另存为", "名前を付けて保存");
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);

            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(121, 6);

            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new Size(124, 22);
            this.menuFileExit.Text = LangText("退出", "終了");
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);

            this.menuLanguage.Name = "menuLanguage";
            this.menuLanguage.Size = new Size(37, 21);
            this.menuLanguage.Text = LangText("语言", "言語");

            this.menuLangCN.Name = "menuLangCN";
            this.menuLangCN.Size = new Size(124, 22);
            this.menuLangCN.Text = LangText("中文", "中文");
            this.menuLangCN.Click += new System.EventHandler(this.menuLangCN_Click);

            this.menuLangJP.Name = "menuLangJP";
            this.menuLangJP.Size = new Size(124, 22);
            this.menuLangJP.Text = LangText("日文", "日本語");
            this.menuLangJP.Click += new System.EventHandler(this.menuLangJP_Click);

            this.menuLanguage.DropDownItems.AddRange(new ToolStripItem[] { this.menuLangCN, this.menuLangJP });

            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new Size(37, 21);
            this.menuHelp.Text = LangText("帮助", "ヘルプ");

            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new Size(124, 22);
            this.menuHelpAbout.Text = LangText("关于", "バージョン情報");
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            this.menuHelp.DropDownItems.AddRange(new ToolStripItem[] { this.menuHelpAbout });

            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new Size(37, 21);
            this.menuTools.Text = LangText("工具", "ツール");

            this.menuToolsCharName.Name = "menuToolsCharName";
            this.menuToolsCharName.Size = new Size(220, 22);
            this.menuToolsCharName.Text = LangText("更改角色名…", "キャラ名変更…");
            this.menuToolsCharName.Click += new System.EventHandler(this.menuEditCharName_Click);

            this.menuToolsConvertPs2.Name = "menuToolsConvertPs2";
            this.menuToolsConvertPs2.Size = new Size(220, 22);
            this.menuToolsConvertPs2.Text = LangText("PS2 → 3DS 存档转换…", "PS2 → 3DS セーブ変換…");
            this.menuToolsConvertPs2.Click += new System.EventHandler(this.menuToolsConvertPs2_Click);

            this.menuTools.DropDownItems.AddRange(new ToolStripItem[] { this.menuToolsCharName, this.menuToolsConvertPs2 });

            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.statusLabel });
            this.statusStrip.Location = new Point(0, 545);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(780, 22);
            this.statusStrip.TabIndex = 1;

            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(969, 17);
            this.statusLabel.Text = LangText("未加载存档", "セーブ未読み込み");
            this.statusLabel.Spring = true;

            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(780, 520);
            this.tabControl.TabIndex = 2;
            this.tabControl.Controls.Add(this.tabGlobal);
            this.tabControl.Controls.Add(this.tabCharacter);
            this.tabControl.Controls.Add(this.tabItems);
            this.tabControl.Controls.Add(this.tabCooking);
            this.tabControl.Controls.Add(this.tabFSChamber);
            this.tabStoryJump = new TabPage();
            this.tabControl.Controls.Add(this.tabStoryJump);

            InitGlobalTab();
            InitCharacterTab();
            InitItemsTab();
            InitCookingTab();
            InitFSChamberTab();
            InitStoryJumpTab();
            InitSystemTab();
            InitSystemKillsTab();

            this.AutoScaleMode = AutoScaleMode.None;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ClientSize = new Size(780, 620);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = LangText("TOAHEX v1.1 - Tales of the Abyss Save Editor", "TOAHEX v1.1 - Tales of the Abyss Save Editor");
            this.Icon = LoadAppIcon();
            this.AllowDrop = true;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.MainForm_KeyDown);
            this.DragEnter += new DragEventHandler(this.MainForm_DragEnter);
            this.DragDrop += new DragEventHandler(this.MainForm_DragDrop);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Icon LoadAppIcon()
        {
            return GetAppIcon();
        }

        private static Icon _appIcon;
        /// <summary>获取应用图标（缓存，跨对话框复用）。</summary>
        internal static Icon GetAppIcon()
        {
            if (_appIcon != null) return _appIcon;
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "TOAHEX.Icon.File.ico";
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        _appIcon = new Icon(stream);
                        return _appIcon;
                    }
                }
            }
            catch { }

            try
            {
                string icoPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location), "Icon", "File.ico");
                if (System.IO.File.Exists(icoPath))
                {
                    _appIcon = new Icon(icoPath);
                    return _appIcon;
                }
            }
            catch { }

            return null;
        }

        private void InitGlobalTab()
        {
            this.tabGlobal.Location = new Point(4, 22);
            this.tabGlobal.Name = "tabGlobal";
            this.tabGlobal.Padding = new Padding(3);
            this.tabGlobal.Size = new Size(760, 495);
            this.tabGlobal.TabIndex = 0;
            this.tabGlobal.Text = LangText("全局数据", "全局データ");
            this.tabGlobal.AutoScroll = true;
            this.tabGlobal.UseVisualStyleBackColor = true;

            var grpEdit = new GroupBox();
            grpEdit.Text = LangText("可编辑数据", "編集可能データ");
            grpEdit.Location = new Point(12, 12);
            grpEdit.Size = new Size(340, 270);
            grpEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            int ey = 22;

            var lblGald = new Label();
            lblGald.Text = LangText("金币(Gald):", "ガルド:");
            lblGald.Location = new Point(10, ey + 2);
            lblGald.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblGald);

            this.numGald = new NumericUpDown();
            this.numGald.Location = new Point(95, ey);
            this.numGald.Size = new Size(200, 20);
            this.numGald.Maximum = 999999999;
            this.numGald.ValueChanged += new System.EventHandler(this.numGald_ValueChanged);
            grpEdit.Controls.Add(this.numGald);

            ey += 28;

            var lblPlayTime = new Label();
            lblPlayTime.Text = LangText("游戏时间(帧):", "ゲーム時間(フレーム):");
            lblPlayTime.Location = new Point(10, ey + 2);
            lblPlayTime.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblPlayTime);

            this.numPlayTime = new NumericUpDown();
            this.numPlayTime.Location = new Point(95, ey);
            this.numPlayTime.Size = new Size(200, 20);
            this.numPlayTime.Maximum = 4294967295;
            this.numPlayTime.ValueChanged += new System.EventHandler(this.numPlayTime_ValueChanged);
            grpEdit.Controls.Add(this.numPlayTime);

            ey += 28;

            var lblEncount = new Label();
            lblEncount.Text = LangText("遇敌次数:", "エンカウント数:");
            lblEncount.Location = new Point(10, ey + 2);
            lblEncount.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblEncount);

            this.numEncount = new NumericUpDown();
            this.numEncount.Location = new Point(95, ey);
            this.numEncount.Size = new Size(200, 20);
            this.numEncount.Maximum = 999999;
            this.numEncount.ValueChanged += new System.EventHandler(this.numEncount_ValueChanged);
            grpEdit.Controls.Add(this.numEncount);

            ey += 28;

            var lblHit = new Label();
            lblHit.Text = LangText("命中次数:", "ヒット数:");
            lblHit.Location = new Point(10, ey + 2);
            lblHit.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblHit);

            this.numHit = new NumericUpDown();
            this.numHit.Location = new Point(95, ey);
            this.numHit.Size = new Size(200, 20);
            this.numHit.Maximum = 999999;
            this.numHit.ValueChanged += new System.EventHandler(this.numHit_ValueChanged);
            grpEdit.Controls.Add(this.numHit);

            ey += 28;

            var lblGrade = new Label();
            lblGrade.Text = LangText("Grade:", "Grade:");
            lblGrade.Location = new Point(10, ey + 2);
            lblGrade.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblGrade);

            this.numGrade = new NumericUpDown();
            this.numGrade.Location = new Point(95, ey);
            this.numGrade.Size = new Size(200, 20);
            this.numGrade.Minimum = 0;
            this.numGrade.Maximum = 9999999;
            this.numGrade.ValueChanged += new System.EventHandler(this.numGrade_ValueChanged);
            grpEdit.Controls.Add(this.numGrade);

            ey += 28;

            // 赌场筹码（脚本变量 #271，赌场菜单显示的持有数）
            var lblCasinoChips = new Label();
            lblCasinoChips.Text = LangText("赌场筹码:", "カジノチップ:");
            lblCasinoChips.Location = new Point(10, ey + 2);
            lblCasinoChips.Size = new Size(84, 18);
            grpEdit.Controls.Add(lblCasinoChips);

            this.numCasinoChips = new NumericUpDown();
            this.numCasinoChips.Location = new Point(95, ey);
            this.numCasinoChips.Size = new Size(200, 20);
            this.numCasinoChips.Minimum = 0;
            this.numCasinoChips.Maximum = 99999999;
            this.numCasinoChips.ValueChanged += new System.EventHandler(this.numCasinoChips_ValueChanged);
            grpEdit.Controls.Add(this.numCasinoChips);

            ey += 28;

            // 难度（原队伍编排组移入此处，双写 0x7D0+0xABF3）。
            // 必须走流式布局：此前固定 y=190 与"累计Grade"行重叠且被其遮挡（后加控件沉底）
            var lblDiffEdit = new Label();
            lblDiffEdit.Text = LangText("难度:", "難易度:");
            lblDiffEdit.Location = new Point(10, ey + 2);
            lblDiffEdit.Size = new Size(80, 18);
            grpEdit.Controls.Add(lblDiffEdit);

            this.cmbDifficulty = new ComboBox();
            this.cmbDifficulty.Location = new Point(95, ey);
            this.cmbDifficulty.Size = new Size(200, 22);
            this.cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDifficulty.Items.AddRange(new object[] { LangText("普通", "ノーマル"), LangText("困难", "ハード"), LangText("狂热", "マニア"), LangText("未知", "アンノウン") });
            this.cmbDifficulty.SelectedIndexChanged += new System.EventHandler(this.cmbDifficulty_SelectedIndexChanged);
            grpEdit.Controls.Add(this.cmbDifficulty);

            this.tabGlobal.Controls.Add(grpEdit);

            var grpInfo = new GroupBox();
            grpInfo.Text = LangText("存档信息（只读）", "セーブ情報（読み取り専用）");
            grpInfo.Location = new Point(12, 290);
            grpInfo.Size = new Size(340, 140);
            grpInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            var lblVerTitle = new Label();
            lblVerTitle.Text = LangText("游戏版本:", "ゲームバージョン:");
            lblVerTitle.Location = new Point(10, 24);
            lblVerTitle.Size = new Size(60, 18);
            grpInfo.Controls.Add(lblVerTitle);

            this.lblVersion = new Label();
            this.lblVersion.Text = "-";
            this.lblVersion.Location = new Point(75, 24);
            this.lblVersion.Size = new Size(220, 18);
            grpInfo.Controls.Add(this.lblVersion);

            var lblDiffTitle = new Label();
            lblDiffTitle.Text = LangText("难度:", "難易度:");
            lblDiffTitle.Location = new Point(10, 48);
            lblDiffTitle.Size = new Size(60, 18);
            grpInfo.Controls.Add(lblDiffTitle);

            this.lblDifficulty = new Label();
            this.lblDifficulty.Text = "-";
            this.lblDifficulty.Location = new Point(75, 48);
            this.lblDifficulty.Size = new Size(220, 18);
            grpInfo.Controls.Add(this.lblDifficulty);

            var lblPartyTitle = new Label();
            lblPartyTitle.Text = LangText("队伍人数:", "パーティ人数:");
            lblPartyTitle.Location = new Point(10, 72);
            lblPartyTitle.Size = new Size(60, 18);
            grpInfo.Controls.Add(lblPartyTitle);

            this.lblPartyCount = new Label();
            this.lblPartyCount.Text = "-";
            this.lblPartyCount.Location = new Point(75, 72);
            this.lblPartyCount.Size = new Size(220, 18);
            grpInfo.Controls.Add(this.lblPartyCount);

            var lblLocTitle = new Label();
            lblLocTitle.Text = LangText("当前位置:", "現在地:");
            lblLocTitle.Location = new Point(10, 96);
            lblLocTitle.Size = new Size(60, 18);
            grpInfo.Controls.Add(lblLocTitle);

            this.lblLocation = new Label();
            this.lblLocation.Text = "-";
            this.lblLocation.Location = new Point(75, 96);
            this.lblLocation.Size = new Size(240, 18);
            grpInfo.Controls.Add(this.lblLocation);

            this.tabGlobal.Controls.Add(grpInfo);

            var grpParty = new GroupBox();
            grpParty.Text = LangText("队伍编排", "パーティ編成");
            grpParty.Location = new Point(365, 12);
            grpParty.Size = new Size(380, 160);
            grpParty.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.cmbPartySlot = new ComboBox[8];
            string[] partyCharNames = { LangText("(空)", "(空)"), "卢克", "缇娅", "杰德", "阿妮丝", "凯", "娜塔莉亚", "阿修" };
            for (int i = 0; i < 8; i++)
            {
                var lblSlot = new Label();
                lblSlot.Text = string.Format(LangText("位置{0}:", "枠{0}:"), i + 1);
                lblSlot.Location = new Point(10, 18 + i * 17);
                lblSlot.Size = new Size(40, 16);
                grpParty.Controls.Add(lblSlot);

                this.cmbPartySlot[i] = new ComboBox();
                this.cmbPartySlot[i].Location = new Point(52, 16 + i * 17);
                this.cmbPartySlot[i].Size = new Size(120, 18);
                this.cmbPartySlot[i].DropDownStyle = ComboBoxStyle.DropDownList;
                this.cmbPartySlot[i].Items.AddRange(partyCharNames);
                this.cmbPartySlot[i].SelectedIndexChanged += new System.EventHandler(this.cmbPartySlot_SelectedIndexChanged);
                grpParty.Controls.Add(this.cmbPartySlot[i]);
            }

            // 领队（0x7C3，单写即生效）：索引0=空、1-6=角色ID，不含阿修（阿修不可当领队）
            // 位于原难度下拉位置；难度已移至"可编辑数据"组
            var lblLeader = new Label();
            lblLeader.Text = LangText("领队:", "リーダー:");
            lblLeader.Location = new Point(200, 46);
            lblLeader.Size = new Size(36, 18);
            grpParty.Controls.Add(lblLeader);

            this.cmbLeader = new ComboBox();
            this.cmbLeader.Location = new Point(240, 44);
            this.cmbLeader.Size = new Size(126, 22);
            this.cmbLeader.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbLeader.Items.AddRange(new object[] { LangText("(空)", "(空)"), "卢克", "缇娅", "杰德", "阿妮丝", "凯", "娜塔莉亚", "阿修" });
            this.cmbLeader.SelectedIndexChanged += new System.EventHandler(this.cmbLeader_SelectedIndexChanged);
            grpParty.Controls.Add(this.cmbLeader);

            this.tabGlobal.Controls.Add(grpParty);

            var grpFeatureFlags = new GroupBox();
            grpFeatureFlags.Text = LangText("功能解锁", "機能解放");
            grpFeatureFlags.Location = new Point(365, 180);
            grpFeatureFlags.Size = new Size(380, 50);
            grpFeatureFlags.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.chkCCore = new CheckBox();
            this.chkCCore.Text = LangText("C·コア(响律符)", "C·コア");
            this.chkCCore.Location = new Point(12, 20);
            this.chkCCore.Size = new Size(150, 20);
            this.chkCCore.CheckedChanged += new System.EventHandler(this.chkCCore_CheckedChanged);
            grpFeatureFlags.Controls.Add(this.chkCCore);

            this.chkFSChamber = new CheckBox();
            this.chkFSChamber.Text = LangText("音素质点嵌石", "FSチャンバー");
            this.chkFSChamber.Location = new Point(170, 20);
            this.chkFSChamber.Size = new Size(180, 20);
            this.chkFSChamber.CheckedChanged += new System.EventHandler(this.chkFSChamber_CheckedChanged);
            grpFeatureFlags.Controls.Add(this.chkFSChamber);

            this.tabGlobal.Controls.Add(grpFeatureFlags);

            var grpTools = new GroupBox();
            grpTools.Text = LangText("快捷修改", "クイック編集");
            grpTools.Location = new Point(365, 238);
            grpTools.Size = new Size(380, 162);
            grpTools.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.btnJournalAll = new Button();
            this.btnJournalAll.Text = LangText("日志全开", "Journal全開放");
            this.btnJournalAll.Location = new Point(12, 20);
            this.btnJournalAll.Size = new Size(170, 24);
            this.btnJournalAll.Click += new System.EventHandler(this.btnJournalAll_Click);
            grpTools.Controls.Add(this.btnJournalAll);

            this.btnItemBookAll = new Button();
            this.btnItemBookAll.Text = LangText("道具图鉴全开", "アイテム図鑑全開");
            this.btnItemBookAll.Location = new Point(192, 20);
            this.btnItemBookAll.Size = new Size(170, 24);
            this.btnItemBookAll.Click += new System.EventHandler(this.btnItemBookAll_Click);
            grpTools.Controls.Add(this.btnItemBookAll);

            this.btnMaxAllLevel = new Button();
            this.btnMaxAllLevel.Text = LangText("全角色满级", "全キャラLvMAX");
            this.btnMaxAllLevel.Location = new Point(12, 50);
            this.btnMaxAllLevel.Size = new Size(110, 24);
            this.btnMaxAllLevel.Click += new EventHandler(this.btnMaxAllLevel_Click);
            grpTools.Controls.Add(this.btnMaxAllLevel);

            this.btnAllTitles = new Button();
            this.btnAllTitles.Text = LangText("全称号", "全称号");
            this.btnAllTitles.Location = new Point(130, 50);
            this.btnAllTitles.Size = new Size(110, 24);
            this.btnAllTitles.Click += new EventHandler(this.btnAllTitles_Click);
            grpTools.Controls.Add(this.btnAllTitles);

            this.btnAllADSkills = new Button();
            this.btnAllADSkills.Text = LangText("全附加技能", "全追加スキル");
            this.btnAllADSkills.Location = new Point(248, 50);
            this.btnAllADSkills.Size = new Size(110, 24);
            this.btnAllADSkills.Click += new EventHandler(this.btnAllADSkills_Click);
            grpTools.Controls.Add(this.btnAllADSkills);

            this.btnAllFSMax = new Button();
            this.btnAllFSMax.Text = LangText("全谱石满级", "全FSチャンバーMAX");
            this.btnAllFSMax.Location = new Point(12, 80);
            this.btnAllFSMax.Size = new Size(170, 24);
            this.btnAllFSMax.Click += new EventHandler(this.btnAllFSMax_Click);
            grpTools.Controls.Add(this.btnAllFSMax);

            this.btnAllCookingMax = new Button();
            this.btnAllCookingMax.Text = LangText("全料理满级", "全料理マスター");
            this.btnAllCookingMax.Location = new Point(192, 80);
            this.btnAllCookingMax.Size = new Size(170, 24);
            this.btnAllCookingMax.Click += new EventHandler(this.btnAllCookingMax_Click);
            grpTools.Controls.Add(this.btnAllCookingMax);

            this.btnAllItemsMax = new Button();
            this.btnAllItemsMax.Text = LangText("所有道具全满", "全アイテム最大");
            this.btnAllItemsMax.Location = new Point(12, 110);
            this.btnAllItemsMax.Size = new Size(170, 24);
            this.btnAllItemsMax.Click += new EventHandler(this.btnAllItemsMax_Click);
            grpTools.Controls.Add(this.btnAllItemsMax);

            // "更改角色名"已移至顶部「工具」菜单（menuToolsCharName）
            this.btnMapAll = new Button();
            this.btnMapAll.Text = LangText("地图全开", "マップ全開放");
            this.btnMapAll.Location = new Point(192, 110);
            this.btnMapAll.Size = new Size(170, 24);
            this.btnMapAll.Click += new EventHandler(this.btnMapAll_Click);
            grpTools.Controls.Add(this.btnMapAll);

            this.tabGlobal.Controls.Add(grpTools);

        }

        private void InitCharacterTab()
        {
            this.tabCharacter.Location = new Point(4, 22);
            this.tabCharacter.Name = "tabCharacter";
            this.tabCharacter.Padding = new Padding(3);
            this.tabCharacter.Size = new Size(760, 495);
            this.tabCharacter.TabIndex = 1;
            this.tabCharacter.Text = LangText("角色编辑", "キャラ編集");
            this.tabCharacter.UseVisualStyleBackColor = true;

            var lblSelect = new Label();
            lblSelect.Text = LangText("选择角色:", "キャラ選択:");
            lblSelect.Location = new Point(12, 10);
            lblSelect.Size = new Size(70, 18);
            this.tabCharacter.Controls.Add(lblSelect);

            this.cmbCharSelect = new ComboBox();
            this.cmbCharSelect.Location = new Point(85, 8);
            this.cmbCharSelect.Size = new Size(130, 20);
            this.cmbCharSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCharSelect.SelectedIndexChanged += new System.EventHandler(this.cmbCharSelect_SelectedIndexChanged);
            this.tabCharacter.Controls.Add(this.cmbCharSelect);

            this.charInnerTab = new TabControl();
            this.charInnerTab.Location = new Point(4, 32);
            this.charInnerTab.Size = new Size(760, 425);
            this.charInnerTab.TabIndex = 0;

            this.subTabStats = new TabPage(LangText("角色属性", "キャラステータス"));
            this.subTabEquip = new TabPage(LangText("装备", "装備"));
            this.subTabArtes = new TabPage(LangText("术技", "アーツ"));
            this.subTabADSkill = new TabPage(LangText("附加技能", "追加スキル"));
            this.subTabTitle = new TabPage(LangText("称号", "称号"));

            this.charInnerTab.Controls.Add(this.subTabStats);
            this.charInnerTab.Controls.Add(this.subTabEquip);
            this.charInnerTab.Controls.Add(this.subTabArtes);
            this.charInnerTab.Controls.Add(this.subTabADSkill);
            this.charInnerTab.Controls.Add(this.subTabTitle);

            this.tabCharacter.Controls.Add(this.charInnerTab);

            InitSubTabStats();
            InitSubTabEquip();
            InitSubTabArtes();
            InitSubTabADSkill();
            InitSubTabTitle();
        }

        private void InitSubTabStats()
        {
            var grpBasic = new GroupBox();
            grpBasic.Text = LangText("基础属性", "基本ステータス");
            grpBasic.Location = new Point(8, 8);
            grpBasic.Size = new Size(370, 400);
            this._grpStats = grpBasic;
            this.subTabStats.Controls.Add(grpBasic);

            // 基础/战斗两个面板叠放，切换按钮互斥显示（右侧立绘与成长组不变）
            this.pnlStatBasic = new Panel();
            pnlStatBasic.Location = new Point(2, 20);
            pnlStatBasic.Size = new Size(366, 342);
            grpBasic.Controls.Add(pnlStatBasic);

            this.pnlStatCombat = new Panel();
            pnlStatCombat.Location = new Point(2, 20);
            pnlStatCombat.Size = new Size(366, 342);
            pnlStatCombat.Visible = false;
            grpBasic.Controls.Add(pnlStatCombat);

            this.btnStatToggle = new Button();
            btnStatToggle.Text = LangText("显示战斗属性 ▸", "戦闘ステータスへ ▸");
            btnStatToggle.AutoSize = true;
            btnStatToggle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStatToggle.Padding = new Padding(8, 2, 8, 2);
            btnStatToggle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStatToggle.Location = new Point(12, 368);
            btnStatToggle.Click += new System.EventHandler(this.btnStatToggle_Click);
            grpBasic.Controls.Add(this.btnStatToggle);

            int y = 6;
            this.numLevel = AddNumericRow(pnlStatBasic, LangText("等级:", "レベル:"), 12, ref y, 1, 200);
            this.numLevel.ValueChanged += new System.EventHandler(this.numLevel_ValueChanged);
            this.numExp = AddNumericRow(pnlStatBasic, LangText("经验值:", "経験値:"), 12, ref y, 0, 4294967295);
            this.numExp.ValueChanged += new System.EventHandler(this.numExp_ValueChanged);
            this.numHP = AddNumericRow(pnlStatBasic, LangText("当前HP:", "現在HP:"), 12, ref y, 0, 99999);
            this.numHP.ValueChanged += new System.EventHandler(this.numHP_ValueChanged);
            this.numTP = AddNumericRow(pnlStatBasic, LangText("当前TP:", "現在TP:"), 12, ref y, 0, 9999);
            this.numTP.ValueChanged += new System.EventHandler(this.numTP_ValueChanged);
            this.numMaxHP = AddNumericRow(pnlStatBasic, LangText("最大HP:", "最大HP:"), 12, ref y, 1, 99999);
            this.numMaxHP.ValueChanged += new System.EventHandler(this.numMaxHP_ValueChanged);
            this.numMaxTP = AddNumericRow(pnlStatBasic, LangText("最大TP:", "最大TP:"), 12, ref y, 1, 9999);
            this.numMaxTP.ValueChanged += new System.EventHandler(this.numMaxTP_ValueChanged);
            this.numGrowthPoints = AddNumericRow(pnlStatBasic, LangText("成长点数:", "成長ポイント:"), 12, ref y, 0, 65535);
            this.numGrowthPoints.ValueChanged += new System.EventHandler(this.numGrowthPoints_ValueChanged);

            var lblTitleLabel = new Label();
            lblTitleLabel.Text = LangText("当前称号:", "現在称号:");
            lblTitleLabel.Location = new Point(12, y + 2);
            lblTitleLabel.Size = new Size(80, 20);
            pnlStatBasic.Controls.Add(lblTitleLabel);

            this.lblTitle = new Label();
            this.lblTitle.Text = LangText("(无)", "(なし)");
            this.lblTitle.Location = new Point(96, y + 2);
            this.lblTitle.Size = new Size(140, 20);
            pnlStatBasic.Controls.Add(this.lblTitle);

            this.btnTitleChange = new Button();
            this.btnTitleChange.Text = LangText("更改", "変更");
            this.btnTitleChange.Location = new Point(244, y);
            this.btnTitleChange.Size = new Size(60, 24);
            this.btnTitleChange.Click += new System.EventHandler(this.btnTitleChange_Click);
            pnlStatBasic.Controls.Add(this.btnTitleChange);

            // 战斗属性面板（原独立子页签并入此处，随按钮切换）
            int cy = 6;
            this.numBasePATK = AddNumericRow(pnlStatCombat, LangText("物攻(P.ATK):", "物攻(P.ATK):"), 12, ref cy, 0, 99999);
            this.numBasePATK.ValueChanged += new System.EventHandler(this.numBasePATK_ValueChanged);
            this.numBasePDEF = AddNumericRow(pnlStatCombat, LangText("物防(P.DEF):", "物防(P.DEF):"), 12, ref cy, 0, 99999);
            this.numBasePDEF.ValueChanged += new System.EventHandler(this.numBasePDEF_ValueChanged);
            this.numBaseFATK = AddNumericRow(pnlStatCombat, LangText("譜攻(F.ATK):", "譜攻(F.ATK):"), 12, ref cy, 0, 99999);
            this.numBaseFATK.ValueChanged += new System.EventHandler(this.numBaseFATK_ValueChanged);
            this.numBaseFDEF = AddNumericRow(pnlStatCombat, LangText("譜防(F.DEF):", "譜防(F.DEF):"), 12, ref cy, 0, 99999);
            this.numBaseFDEF.ValueChanged += new System.EventHandler(this.numBaseFDEF_ValueChanged);
            this.numBaseAGI = AddNumericRow(pnlStatCombat, LangText("敏捷(AGI):", "敏捷(AGI):"), 12, ref cy, 0, 99999);
            this.numBaseAGI.ValueChanged += new System.EventHandler(this.numBaseAGI_ValueChanged);
            this.numBaseLUCK = AddNumericRow(pnlStatCombat, LangText("幸运(LUCK):", "運(LUCK):"), 12, ref cy, 0, 9999);
            this.numBaseLUCK.ValueChanged += new System.EventHandler(this.numBaseLUCK_ValueChanged);
            this.numOvlGauge = AddNumericRow(pnlStatCombat, LangText("OVL", "OVLゲージ:"), 12, ref cy, 0, 1000);
            this.numOvlGauge.ValueChanged += new System.EventHandler(this.numOvlGauge_ValueChanged);
            this.numKillCount = AddNumericRow(pnlStatCombat, LangText("杀敌数(魔武器):", "撃破数(魔武器):"), 12, ref cy, 0, 999999);
            this.numKillCount.ValueChanged += new System.EventHandler(this.numKillCount_ValueChanged);

            // C-Core 加成已按用户要求隐藏：控件保留在无父容器中（不显示），
            // 仅维持 RefreshCharFields 装载/幸运联动逻辑不抛空引用
            var grpCCoreHidden = new GroupBox();
            int ccy = 20;
            this.numCCorePATK = AddNumericRow(grpCCoreHidden, LangText("C-Core物攻:", "C-Core物攻:"), 12, ref ccy, 0, 9999);
            this.numCCorePATK.ValueChanged += new System.EventHandler(this.numCCorePATK_ValueChanged);
            this.numCCorePDEF = AddNumericRow(grpCCoreHidden, LangText("C-Core物防:", "C-Core物防:"), 12, ref ccy, 0, 9999);
            this.numCCorePDEF.ValueChanged += new System.EventHandler(this.numCCorePDEF_ValueChanged);
            this.numCCoreFATK = AddNumericRow(grpCCoreHidden, LangText("C-Core谱攻:", "C-Core譜攻:"), 12, ref ccy, 0, 9999);
            this.numCCoreFATK.ValueChanged += new System.EventHandler(this.numCCoreFATK_ValueChanged);
            this.numCCoreFDEF = AddNumericRow(grpCCoreHidden, LangText("C-Core谱防:", "C-Core譜防:"), 12, ref ccy, 0, 9999);
            this.numCCoreFDEF.ValueChanged += new System.EventHandler(this.numCCoreFDEF_ValueChanged);
            this.numCCoreAGI = AddNumericRow(grpCCoreHidden, LangText("C-Core敏捷:", "C-Core敏捷:"), 12, ref ccy, 0, 9999);
            this.numCCoreAGI.ValueChanged += new System.EventHandler(this.numCCoreAGI_ValueChanged);
            this.numCCoreLUK = AddNumericRow(grpCCoreHidden, LangText("C-Core幸运:", "C-Core運:"), 12, ref ccy, 0, 9999);
            this.numCCoreLUK.ValueChanged += new System.EventHandler(this.numCCoreLUK_ValueChanged);

            this.picCharPortrait = new PictureBox();
            this.picCharPortrait.Location = new Point(540, 8);
            this.picCharPortrait.Size = new Size(200, 280);
            this.picCharPortrait.SizeMode = PictureBoxSizeMode.Zoom;
            this.picCharPortrait.BorderStyle = BorderStyle.FixedSingle;
            this.subTabStats.Controls.Add(this.picCharPortrait);

            // 等级联动成长：调整等级时按每级增量自动增减基础属性（近似值；游戏读档时会重算衍生属性）
            var grpGrowth = new GroupBox();
            grpGrowth.Text = LangText("等级联动成长", "レベル連動成長");
            grpGrowth.Location = new Point(386, 8);
            grpGrowth.Size = new Size(148, 404);
            this.subTabStats.Controls.Add(grpGrowth);

            this.chkLevelGrowth = new CheckBox();
            this.chkLevelGrowth.Text = LangText("启用(每级增量)", "有効(毎レベル)");
            this.chkLevelGrowth.Location = new Point(10, 20);
            this.chkLevelGrowth.Size = new Size(130, 20);
            grpGrowth.Controls.Add(this.chkLevelGrowth);

            int gy = 44;
            this.numGrowHP = AddNumericRow(grpGrowth, LangText("HP+", "HP+"), 10, ref gy, 0, 9999, 46);
            this.numGrowTP = AddNumericRow(grpGrowth, LangText("TP+", "TP+"), 10, ref gy, 0, 999, 46);
            this.numGrowPATK = AddNumericRow(grpGrowth, LangText("物攻+", "物攻+"), 10, ref gy, 0, 999, 46);
            this.numGrowPDEF = AddNumericRow(grpGrowth, LangText("物防+", "物防+"), 10, ref gy, 0, 999, 46);
            this.numGrowFATK = AddNumericRow(grpGrowth, LangText("谱攻+", "譜攻+"), 10, ref gy, 0, 999, 46);
            this.numGrowFDEF = AddNumericRow(grpGrowth, LangText("谱防+", "譜防+"), 10, ref gy, 0, 999, 46);
            this.numGrowAGI = AddNumericRow(grpGrowth, LangText("敏捷+", "敏捷+"), 10, ref gy, 0, 999, 46);
            this.numGrowLUK = AddNumericRow(grpGrowth, LangText("幸运+", "運+"), 10, ref gy, 0, 99, 46);
            this.numGrowHP.Value = 45; this.numGrowTP.Value = 6;
            this.numGrowPATK.Value = 4; this.numGrowPDEF.Value = 3;
            this.numGrowFATK.Value = 4; this.numGrowFDEF.Value = 3;
            this.numGrowAGI.Value = 1; this.numGrowLUK.Value = 0;

            var lblGrowthHint = new Label();
            lblGrowthHint.Text = LangText("调整等级时按增量自动增减基础属性。增量为近似值；游戏读档时会按等级重算衍生属性(sub_3E1038)。", "レベル変更時に基礎ステータスを自動増減。近似値。ゲーム読込時に衍生値は再計算されます。");
            lblGrowthHint.Location = new Point(10, gy + 2);
            lblGrowthHint.Size = new Size(130, 140);
            grpGrowth.Controls.Add(lblGrowthHint);
        }

        private void InitSubTabEquip()
        {
            // 装备全开按钮（右上）
            this.btnGetAllEquip = new Button();
            this.btnGetAllEquip.Text = LangText("装备全开", "全装備獲得");
            this.btnGetAllEquip.Location = new Point(640, 10);
            this.btnGetAllEquip.Size = new Size(110, 24);
            this.btnGetAllEquip.Click += new System.EventHandler(this.btnGetAllEquip_Click);
            this.subTabEquip.Controls.Add(this.btnGetAllEquip);

            // 术技页同款紧凑布局：5 行"槽位: 当前装备名" + 更改按钮（行距 32）；
            // 五行从 y=46 开始，顶部整行留给"装备全开"按钮（避免与首行"更改"重叠）
            this.lblEquip = new Label[5];
            this.btnEquipChange = new Button[5];
            string[] slotNames =
            {
                LangText("武器", "武器"),
                LangText("防具", "防具"),
                LangText("饰品1", "アクセ1"),
                LangText("饰品2", "アクセ2"),
                LangText("响律符", "響律符"),
            };
            for (int i = 0; i < 5; i++)
            {
                int y = 46 + i * 32;

                this.lblEquip[i] = new Label();
                this.lblEquip[i].Text = slotNames[i] + ": -";
                this.lblEquip[i].Location = new Point(12, y + 2);
                this.lblEquip[i].Size = new Size(580, 20);
                this.subTabEquip.Controls.Add(this.lblEquip[i]);

                this.btnEquipChange[i] = new Button();
                this.btnEquipChange[i].Text = LangText("更改", "変更");
                this.btnEquipChange[i].Location = new Point(620, y);
                this.btnEquipChange[i].Size = new Size(60, 22);
                this.btnEquipChange[i].Tag = i;
                this.btnEquipChange[i].Click += new System.EventHandler(this.btnEquipChange_Click);
                this.subTabEquip.Controls.Add(this.btnEquipChange[i]);
            }
        }

        private void InitSubTabArtes()
        {
            this.lblArte = new Label[4];
            this.btnArteChange = new Button[4];

            for (int i = 0; i < 4; i++)
            {
                int x = 6;
                int ay = i * 26 + 5;

                this.lblArte[i] = new Label();
                this.lblArte[i].Text = string.Format(LangText("快捷{0}: (空)", "ショートカット{0}: (空)"), i + 1);
                this.lblArte[i].Location = new Point(x, ay + 2);
                this.lblArte[i].Size = new Size(280, 18);
                this.subTabArtes.Controls.Add(this.lblArte[i]);

                this.btnArteChange[i] = new Button();
                this.btnArteChange[i].Text = LangText("更改", "変更");
                this.btnArteChange[i].Location = new Point(x + 290, ay);
                this.btnArteChange[i].Size = new Size(50, 20);
                this.btnArteChange[i].Tag = i;
                this.btnArteChange[i].Click += new System.EventHandler(this.btnArteChange_Click);
                this.subTabArtes.Controls.Add(this.btnArteChange[i]);
            }

            var lblArteLearned = new Label();
            lblArteLearned.Text = LangText("已学习术技:", "習得術技:");
            lblArteLearned.Location = new Point(6, 110);
            lblArteLearned.Size = new Size(80, 18);
            this.subTabArtes.Controls.Add(lblArteLearned);

            this.clbArteLearned = new CheckedListBox();
            this.clbArteLearned.Location = new Point(6, 130);
            this.clbArteLearned.Size = new Size(340, 180);
            this.clbArteLearned.ItemCheck += new ItemCheckEventHandler(this.clbArteLearned_ItemCheck);
            this.subTabArtes.Controls.Add(this.clbArteLearned);

            var btnArteLearnedAll = new Button();
            btnArteLearnedAll.Text = LangText("全选", "全選");
            btnArteLearnedAll.Location = new Point(6, 314);
            btnArteLearnedAll.Size = new Size(60, 22);
            btnArteLearnedAll.Click += new System.EventHandler(this.btnArteLearnedSelectAll_Click);
            this.subTabArtes.Controls.Add(btnArteLearnedAll);

            var btnArteLearnedNone = new Button();
            btnArteLearnedNone.Text = LangText("全不选", "全解除");
            btnArteLearnedNone.Location = new Point(72, 314);
            btnArteLearnedNone.Size = new Size(60, 22);
            btnArteLearnedNone.Click += new System.EventHandler(this.btnArteLearnedDeselectAll_Click);
            this.subTabArtes.Controls.Add(btnArteLearnedNone);

            var lblArteUsageTitle = new Label();
            lblArteUsageTitle.Text = LangText("使用次数:", "使用回数:");
            lblArteUsageTitle.Location = new Point(355, 112);
            lblArteUsageTitle.Size = new Size(80, 18);
            this.subTabArtes.Controls.Add(lblArteUsageTitle);

            this.pnlArteUsage = new Panel();
            this.pnlArteUsage.Location = new Point(355, 130);
            this.pnlArteUsage.Size = new Size(390, 180);
            this.pnlArteUsage.AutoScroll = true;
            this.numArteUsage = new NumericUpDown[25];
            this.lblArteUsage = new Label[25];
            for (int i = 0; i < 25; i++)
            {
                this.lblArteUsage[i] = new Label();
                this.lblArteUsage[i].Text = "";
                this.lblArteUsage[i].Size = new Size(120, 20);
                this.lblArteUsage[i].Location = new Point(5, 5 + i * 26);
                this.pnlArteUsage.Controls.Add(this.lblArteUsage[i]);
                this.numArteUsage[i] = new NumericUpDown();
                this.numArteUsage[i].Minimum = 0;
                this.numArteUsage[i].Maximum = 65535;
                this.numArteUsage[i].Size = new Size(80, 20);
                this.numArteUsage[i].Location = new Point(130, 3 + i * 26);
                this.numArteUsage[i].Tag = i;
                this.numArteUsage[i].ValueChanged += new System.EventHandler(this.numArteUsage_ValueChanged);
                this.pnlArteUsage.Controls.Add(this.numArteUsage[i]);
            }
            this.subTabArtes.Controls.Add(this.pnlArteUsage);
        }

        private void InitSubTabADSkill()
        {
            this.clbADSkills = new CheckedListBox();
            this.clbADSkills.Location = new Point(6, 6);
            this.clbADSkills.Size = new Size(740, 280);
            this.clbADSkills.ItemCheck += new ItemCheckEventHandler(this.clbADSkills_ItemCheck);
            this.subTabADSkill.Controls.Add(this.clbADSkills);

            for (int i = 0; i < 88; i++)
            {
                this.clbADSkills.Items.Add(string.Format("{0:D2}: {1}", i, ADSkillDatabase.GetName(i)));
            }

            this.btnADSelectAll = new Button();
            this.btnADSelectAll.Text = LangText("全选", "全選択");
            this.btnADSelectAll.Location = new Point(6, 292);
            this.btnADSelectAll.Size = new Size(70, 22);
            this.btnADSelectAll.Click += new System.EventHandler(this.btnADSelectAll_Click);
            this.subTabADSkill.Controls.Add(this.btnADSelectAll);

            this.btnADDeselectAll = new Button();
            this.btnADDeselectAll.Text = LangText("全不选", "全解除");
            this.btnADDeselectAll.Location = new Point(82, 292);
            this.btnADDeselectAll.Size = new Size(70, 22);
            this.btnADDeselectAll.Click += new System.EventHandler(this.btnADDeselectAll_Click);
            this.subTabADSkill.Controls.Add(this.btnADDeselectAll);

            this.btnADLearnAll = new Button();
            this.btnADLearnAll.Text = LangText("全掌握", "全習得");
            this.btnADLearnAll.Location = new Point(158, 292);
            this.btnADLearnAll.Size = new Size(70, 22);
            this.btnADLearnAll.Click += new System.EventHandler(this.btnADLearnAll_Click);
            this.subTabADSkill.Controls.Add(this.btnADLearnAll);
        }

        private void InitSubTabTitle()
        {
            this.btnTitleOpenAll = new Button();
            this.btnTitleOpenAll.Text = LangText("称号全开", "称号全開放");
            this.btnTitleOpenAll.Location = new Point(6, 6);
            this.btnTitleOpenAll.Size = new Size(100, 24);
            this.btnTitleOpenAll.Click += new System.EventHandler(this.btnTitleOpenAll_Click);
            this.subTabTitle.Controls.Add(this.btnTitleOpenAll);

            this.clbTitles = new CheckedListBox();
            this.clbTitles.Location = new Point(6, 36);
            this.clbTitles.Size = new Size(740, 330);
            this.clbTitles.CheckOnClick = true; // 单击行即勾选/取消，无需先选中再点击
            this.clbTitles.ItemCheck += new ItemCheckEventHandler(this.clbTitles_ItemCheck);
            this.subTabTitle.Controls.Add(this.clbTitles);
        }

        private void InitFSChamberTab()
        {
            this.tabFSChamber.Location = new Point(4, 22);
            this.tabFSChamber.Name = "tabFSChamber";
            this.tabFSChamber.Padding = new Padding(3);
            this.tabFSChamber.Size = new Size(760, 495);
            this.tabFSChamber.TabIndex = 5;
            this.tabFSChamber.Text = LangText("谱石管理", "FSチャンバー");
            this.tabFSChamber.UseVisualStyleBackColor = true;

            var lblFSChar = new Label();
            lblFSChar.Text = LangText("选择角色:", "キャラ選択:");
            lblFSChar.Location = new Point(12, 10);
            lblFSChar.Size = new Size(70, 18);
            this.tabFSChamber.Controls.Add(lblFSChar);

            this.cmbFSCharSelect = new ComboBox();
            this.cmbFSCharSelect.Location = new Point(85, 8);
            this.cmbFSCharSelect.Size = new Size(130, 20);
            this.cmbFSCharSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFSCharSelect.Items.AddRange(new object[] { "卢克", "缇娅", "杰德", "阿妮丝", "凯", "娜塔莉亚", "阿修" });
            this.cmbFSCharSelect.SelectedIndexChanged += new System.EventHandler(this.cmbFSCharSelect_SelectedIndexChanged);
            this.tabFSChamber.Controls.Add(this.cmbFSCharSelect);

            this.dgvFSChamber = new DataGridView();
            this.dgvFSChamber.Location = new Point(8, 36);
            this.dgvFSChamber.Size = new Size(745, 380);
            this.dgvFSChamber.AllowUserToAddRows = false;
            this.dgvFSChamber.AllowUserToDeleteRows = false;
            this.dgvFSChamber.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFSChamber.CellValueChanged += new DataGridViewCellEventHandler(this.dgvFSChamber_CellValueChanged);
            this.dgvFSChamber.DataError += new DataGridViewDataErrorEventHandler(this.dgvFSChamber_DataError);

            var colArteName = new DataGridViewTextBoxColumn();
            colArteName.HeaderText = LangText("术技名", "アーツ名");
            colArteName.ReadOnly = true;
            colArteName.FillWeight = 30;
            this.dgvFSChamber.Columns.Add(colArteName);

            var colEquipped = new DataGridViewComboBoxColumn();
            colEquipped.HeaderText = LangText("装备谱石", "装備FS");
            colEquipped.Items.AddRange(new object[] { LangText("无", "無"), LangText("赤", "赤"), LangText("青", "青"), LangText("緑", "緑"), LangText("黄", "黄") });
            colEquipped.FillWeight = 14;
            this.dgvFSChamber.Columns.Add(colEquipped);

            string[] colorNames = { LangText("赤 Lv.", "赤 Lv."), LangText("青 Lv.", "青 Lv."), LangText("緑 Lv.", "緑 Lv."), LangText("黄 Lv.", "黄 Lv.") };
            for (int i = 0; i < 4; i++)
            {
                var colLevel = new DataGridViewComboBoxColumn();
                colLevel.HeaderText = colorNames[i];
                colLevel.Items.AddRange(new object[] { "Lv.1", "Lv.2", "Lv.3", "Lv.4", "Lv.5", "Lv.6" });
                colLevel.FillWeight = 14;
                this.dgvFSChamber.Columns.Add(colLevel);
            }

            this.tabFSChamber.Controls.Add(this.dgvFSChamber);

            var grpMax = new GroupBox();
            grpMax.Text = LangText("谱石最大值", "FSチャンバー最大値");
            grpMax.Location = new Point(8, 422);
            grpMax.Size = new Size(540, 55);
            this.numFSChamberMax = new NumericUpDown[4];
            string[] maxLabels = { LangText("赤", "赤"), LangText("青", "青"), LangText("緑", "緑"), LangText("黄", "黄") };
            for (int i = 0; i < 4; i++)
            {
                var lbl = new Label();
                lbl.Text = maxLabels[i];
                lbl.Location = new Point(12 + i * 130, 14);
                lbl.Size = new Size(25, 18);
                grpMax.Controls.Add(lbl);

                this.numFSChamberMax[i] = new NumericUpDown();
                this.numFSChamberMax[i].Minimum = 0;
                this.numFSChamberMax[i].Maximum = 255;
                this.numFSChamberMax[i].Size = new Size(70, 20);
                this.numFSChamberMax[i].Location = new Point(38 + i * 130, 12);
                this.numFSChamberMax[i].Tag = i;
                this.numFSChamberMax[i].ValueChanged += new System.EventHandler(this.numFSChamberMax_ValueChanged);
                grpMax.Controls.Add(this.numFSChamberMax[i]);
            }
            this.tabFSChamber.Controls.Add(grpMax);

            this.btnFSAllMax = new Button();
            this.btnFSAllMax.Text = LangText("全部满级", "全LvMAX");
            this.btnFSAllMax.Location = new Point(560, 428);
            this.btnFSAllMax.Size = new Size(90, 24);
            this.btnFSAllMax.Click += new System.EventHandler(this.btnFSAllMax_Click);
            this.tabFSChamber.Controls.Add(this.btnFSAllMax);

            this.btnFSAllReset = new Button();
            this.btnFSAllReset.Text = LangText("全部重置", "全リセット");
            this.btnFSAllReset.Location = new Point(660, 428);
            this.btnFSAllReset.Size = new Size(90, 24);
            this.btnFSAllReset.Click += new System.EventHandler(this.btnFSAllReset_Click);
            this.tabFSChamber.Controls.Add(this.btnFSAllReset);
        }

        private void InitCookingTab()
        {
            this.tabCooking.Location = new Point(4, 22);
            this.tabCooking.Name = "tabCooking";
            this.tabCooking.Padding = new Padding(3);
            this.tabCooking.Size = new Size(760, 495);
            this.tabCooking.TabIndex = 3;
            this.tabCooking.Text = LangText("料理修改", "料理編集");
            this.tabCooking.UseVisualStyleBackColor = true;

            var lblCookingTitle = new Label();
            lblCookingTitle.Text = LangText("料理列表（勾选表示已习得）:", "料理一覧（チェックで習得済み）:");
            lblCookingTitle.Location = new Point(12, 10);
            lblCookingTitle.Size = new Size(200, 16);
            this.tabCooking.Controls.Add(lblCookingTitle);

            this.clbCooking = new CheckedListBox();
            this.clbCooking.Location = new Point(12, 30);
            this.clbCooking.Size = new Size(320, 280);
            this.clbCooking.CheckOnClick = true;
            this.clbCooking.ItemCheck += new ItemCheckEventHandler(this.clbCooking_ItemCheck);
            var allCooking = CookingDatabase.GetAll();
            foreach (var item in allCooking)
            {
                this.clbCooking.Items.Add(string.Format("{0:D2}: {1} ({2})", item.id, item.cn, item.jp));
            }
            this.tabCooking.Controls.Add(this.clbCooking);

            this.btnCookingSelectAll = new Button();
            this.btnCookingSelectAll.Text = LangText("全选", "全選択");
            this.btnCookingSelectAll.Location = new Point(12, 314);
            this.btnCookingSelectAll.Size = new Size(70, 22);
            this.btnCookingSelectAll.Click += new System.EventHandler(this.btnCookingSelectAll_Click);
            this.tabCooking.Controls.Add(this.btnCookingSelectAll);

            this.btnCookingDeselectAll = new Button();
            this.btnCookingDeselectAll.Text = LangText("全不选", "全解除");
            this.btnCookingDeselectAll.Location = new Point(87, 314);
            this.btnCookingDeselectAll.Size = new Size(70, 22);
            this.btnCookingDeselectAll.Click += new System.EventHandler(this.btnCookingDeselectAll_Click);
            this.tabCooking.Controls.Add(this.btnCookingDeselectAll);

            var lblCookingMastery = new Label();
            lblCookingMastery.Text = LangText("熟练度", "熟練度");
            lblCookingMastery.Location = new Point(342, 10);
            lblCookingMastery.Size = new Size(80, 18);
            this.tabCooking.Controls.Add(lblCookingMastery);

            var lblCookingChar = new Label();
            lblCookingChar.Text = LangText("角色:", "キャラ:");
            lblCookingChar.Location = new Point(342, 35);
            lblCookingChar.Size = new Size(40, 18);
            this.tabCooking.Controls.Add(lblCookingChar);

            this.cmbCookingChar = new ComboBox();
            this.cmbCookingChar.Location = new Point(385, 33);
            this.cmbCookingChar.Size = new Size(100, 20);
            this.cmbCookingChar.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCookingChar.Items.AddRange(new object[] { "卢克", "缇娅", "杰德", "阿妮丝", "凯", "娜塔莉亚", "阿修" });
            this.cmbCookingChar.SelectedIndex = 0;
            this.cmbCookingChar.SelectedIndexChanged += new System.EventHandler(this.cmbCookingChar_SelectedIndexChanged);
            this.tabCooking.Controls.Add(this.cmbCookingChar);

            this.lblCookingMasteryName = new Label[20];
            this.numCookingMastery = new NumericUpDown[20];
            this.lblCookingMasteryStar = new Label[20];
            for (int i = 0; i < 20; i++)
            {
                int col = i / 10;
                int row = i % 10;

                this.lblCookingMasteryName[i] = new Label();
                this.lblCookingMasteryName[i].Text = CookingDatabase.GetName(i);
                this.lblCookingMasteryName[i].Size = new Size(80, 20);
                this.lblCookingMasteryName[i].Location = new Point(342 + col * 210, 65 + row * 24);
                this.tabCooking.Controls.Add(this.lblCookingMasteryName[i]);

                this.numCookingMastery[i] = new NumericUpDown();
                this.numCookingMastery[i].Minimum = 0;
                this.numCookingMastery[i].Maximum = 255;
                this.numCookingMastery[i].Size = new Size(80, 20);
                this.numCookingMastery[i].Location = new Point(425 + col * 210, 63 + row * 24);
                this.numCookingMastery[i].Tag = i;
                this.numCookingMastery[i].ValueChanged += new System.EventHandler(this.numCookingMastery_ValueChanged);
                this.tabCooking.Controls.Add(this.numCookingMastery[i]);

                this.lblCookingMasteryStar[i] = new Label();
                this.lblCookingMasteryStar[i].Text = "⭐";
                this.lblCookingMasteryStar[i].Size = new Size(40, 20);
                this.lblCookingMasteryStar[i].Location = new Point(508 + col * 210, 65 + row * 24);
                this.tabCooking.Controls.Add(this.lblCookingMasteryStar[i]);
            }
        }

        private void InitSystemTab()
        {
            this.tabSystem.Location = new Point(4, 22);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Padding = new Padding(3);
            this.tabSystem.Size = new Size(760, 495);
            this.tabSystem.TabIndex = 4;
            this.tabSystem.Text = LangText("系统数据", "システムデータ");
            this.tabSystem.UseVisualStyleBackColor = true;

            // 基础记录组（左上）
            var grpBase = new GroupBox();
            grpBase.Text = LangText("基础记录", "基本記録");
            grpBase.Location = new Point(12, 12);
            grpBase.Size = new Size(366, 204);
            this.tabSystem.Controls.Add(grpBase);

            int sy = 22;
            var lblVer = new Label();
            lblVer.Text = LangText("存档版本(只读):", "セーブバージョン(読取専用):");
            lblVer.Location = new Point(12, sy + 2);
            lblVer.Size = new Size(140, 20);
            grpBase.Controls.Add(lblVer);

            this.numToasysDifficulty = new NumericUpDown();
            this.numToasysDifficulty.Location = new Point(160, sy);
            this.numToasysDifficulty.Size = new Size(194, 22);
            this.numToasysDifficulty.DecimalPlaces = 6;
            this.numToasysDifficulty.Minimum = 0;
            this.numToasysDifficulty.Maximum = 1;
            this.numToasysDifficulty.Increment = 0.1m;
            this.numToasysDifficulty.ReadOnly = true;   // 0x04 为版本号(float 0.2)，只读展示
            this.numToasysDifficulty.ValueChanged += new System.EventHandler(this.numToasysDifficulty_ValueChanged);
            grpBase.Controls.Add(this.numToasysDifficulty);

            sy += 28;
            this.numToasysGald = AddNumericRow(grpBase, LangText("最大持有Gald:", "最大所持ガルド:"), 12, ref sy, 0, 99999999);
            this.numToasysGald.ValueChanged += new System.EventHandler(this.numToasysGald_ValueChanged);
            this.numToasysPlaytime = AddNumericRow(grpBase, LangText("最长游戏时间(帧):", "最長ゲーム時間(フレーム):"), 12, ref sy, 0, 4294967295);
            this.numToasysPlaytime.ValueChanged += new System.EventHandler(this.numToasysPlaytime_ValueChanged);
            this.numToasysGaldSpent = AddNumericRow(grpBase, LangText("累计使用Gald:", "累計使用ガルド:"), 12, ref sy, 0, 4294967295);
            this.numToasysGaldSpent.ValueChanged += new System.EventHandler(this.numToasysGaldSpent_ValueChanged);
            this.numToasysSaveCount = AddNumericRow(grpBase, LangText("存档次数:", "セーブ回数:"), 12, ref sy, 0, 999999);
            this.numToasysSaveCount.ValueChanged += new System.EventHandler(this.numToasysSaveCount_ValueChanged);
            this.numToasysEncounter = AddNumericRow(grpBase, LangText("遭遇数:", "エンカウント数:"), 12, ref sy, 0, 4294967295);
            this.numToasysEncounter.ValueChanged += new System.EventHandler(this.numToasysEncounter_ValueChanged);

            // 战斗记录组（右上）
            var grpBattle = new GroupBox();
            grpBattle.Text = LangText("战斗记录", "戦闘記録");
            grpBattle.Location = new Point(390, 12);
            grpBattle.Size = new Size(358, 232);
            this.tabSystem.Controls.Add(grpBattle);

            int by = 22;
            this.numToasysClearCount = AddNumericRow(grpBattle, LangText("通关次数:", "クリア回数:"), 12, ref by, 0, 9999);
            this.numToasysClearCount.ValueChanged += new System.EventHandler(this.numToasysClearCount_ValueChanged);
            this.numToasysEscape = AddNumericRow(grpBattle, LangText("逃跑次数:", "逃走回数:"), 12, ref by, 0, 4294967295);
            this.numToasysEscape.ValueChanged += new System.EventHandler(this.numToasysEscape_ValueChanged);
            this.numToasysMaxDamage = AddNumericRow(grpBattle, LangText("最大伤害:", "最大ダメージ:"), 12, ref by, 0, 999999);
            this.numToasysMaxDamage.ValueChanged += new System.EventHandler(this.numToasysMaxDamage_ValueChanged);
            this.numToasysMaxCombo = AddNumericRow(grpBattle, LangText("最大连击:", "最大ヒット数:"), 12, ref by, 0, 4294967295);
            this.numToasysMaxCombo.ValueChanged += new System.EventHandler(this.numToasysMaxCombo_ValueChanged);
            this.numToasysDamageDealt = AddNumericRow(grpBattle, LangText("造成总伤害:", "総与ダメージ:"), 12, ref by, 0, 4294967295);
            this.numToasysDamageDealt.ValueChanged += new System.EventHandler(this.numToasysDamageDealt_ValueChanged);
            this.numToasysDamageTaken = AddNumericRow(grpBattle, LangText("承受总伤害:", "総被ダメージ:"), 12, ref by, 0, 4294967295);
            this.numToasysDamageTaken.ValueChanged += new System.EventHandler(this.numToasysDamageTaken_ValueChanged);
            this.numToasysBattleTime = AddNumericRow(grpBattle, LangText("战斗总时间(帧,只读):", "総戦闘時間(フレーム):"), 12, ref by, 0, 4294967295);
            this.numToasysBattleTime.ReadOnly = true;

            // 通关内容组（左下）：音效测试等通关后菜单 = 通关次数≠0（sub_333800 菜单构建）
            var grpClear = new GroupBox();
            grpClear.Text = LangText("通关内容", "クリア特典");
            grpClear.Location = new Point(12, 224);
            grpClear.Size = new Size(366, 130);
            this.tabSystem.Controls.Add(grpClear);

            this.chkSoundTest = new CheckBox();
            this.chkSoundTest.Text = LangText("解锁音效测试等通关后菜单", "サウンドテスト等クリアメニュー解放");
            this.chkSoundTest.Location = new Point(12, 22);
            this.chkSoundTest.Size = new Size(340, 20);
            this.chkSoundTest.CheckedChanged += new System.EventHandler(this.chkSoundTest_CheckedChanged);
            grpClear.Controls.Add(this.chkSoundTest);

            this.btnToasysUnlockAll = new Button();
            this.btnToasysUnlockAll.Text = LangText("收集累计全开", "コレクション全開");
            this.btnToasysUnlockAll.Location = new Point(12, 48);
            this.btnToasysUnlockAll.Size = new Size(150, 24);
            this.btnToasysUnlockAll.Click += new System.EventHandler(this.btnToasysUnlockAll_Click);
            grpClear.Controls.Add(this.btnToasysUnlockAll);

            var lblClearHint = new Label();
            lblClearHint.Text = LangText("通关后菜单由通关次数驱动（≥1 出现）；收集累计含音效曲目等（128B 位图全开）。\nGrade商店购入项由脚本flag承载，不在此编辑。", "クリアメニューはクリア回数で解放。コレクションは128Bビットマップ。\nグレードショップはスクリプトフラグのため編集不可。");
            lblClearHint.Location = new Point(12, 78);
            lblClearHint.Size = new Size(344, 44);
            lblClearHint.ForeColor = SystemColors.GrayText;
            grpClear.Controls.Add(lblClearHint);

            // 角色使用计数组（右下）：0x6C 起 6×u32，÷遭遇数=使用率
            var grpUsage = new GroupBox();
            grpUsage.Text = LangText("角色使用计数（÷遭遇数=使用率）", "キャラ使用回数（÷エンカウント数）");
            grpUsage.Location = new Point(390, 252);
            grpUsage.Size = new Size(358, 150);
            this.tabSystem.Controls.Add(grpUsage);

            this.numToasysCharUsage = new NumericUpDown[6];
            this.lblToasysUsagePct = new Label[6];
            string[] usageNames = { LangText("卢克", "ルーク"), LangText("缇娅", "ティア"), LangText("杰德", "ジェイド"), LangText("阿妮丝", "アニス"), LangText("凯", "ガイ"), LangText("娜塔莉亚", "ナタリア") };
            for (int i = 0; i < 6; i++)
            {
                var lblChar = new Label();
                lblChar.Text = usageNames[i] + ":";
                lblChar.Location = new Point(12, 22 + i * 20);
                lblChar.Size = new Size(64, 16);
                grpUsage.Controls.Add(lblChar);

                this.numToasysCharUsage[i] = new NumericUpDown();
                this.numToasysCharUsage[i].Location = new Point(80, 20 + i * 20);
                this.numToasysCharUsage[i].Size = new Size(130, 20);
                this.numToasysCharUsage[i].Minimum = 0;
                this.numToasysCharUsage[i].Maximum = 4294967295;
                this.numToasysCharUsage[i].Tag = i;
                this.numToasysCharUsage[i].ValueChanged += new System.EventHandler(this.numToasysCharUsage_ValueChanged);
                grpUsage.Controls.Add(this.numToasysCharUsage[i]);

                this.lblToasysUsagePct[i] = new Label();
                this.lblToasysUsagePct[i].Text = "-";
                this.lblToasysUsagePct[i].Location = new Point(218, 22 + i * 20);
                this.lblToasysUsagePct[i].Size = new Size(120, 16);
                grpUsage.Controls.Add(this.lblToasysUsagePct[i]);
            }
        }

        private void InitSystemKillsTab()
        {
            this.tabSystemKills.Location = new Point(4, 22);
            this.tabSystemKills.Name = "tabSystemKills";
            this.tabSystemKills.Padding = new Padding(3);
            this.tabSystemKills.Size = new Size(760, 495);
            this.tabSystemKills.TabIndex = 5;
            this.tabSystemKills.Text = LangText("角色杀敌数(魔武器)", "キャラ撃破数(魔武器)");
            this.tabSystemKills.UseVisualStyleBackColor = true;

            var grpKills = new GroupBox();
            grpKills.Text = LangText("角色杀敌数（魔武器攻击力加成，上限999999）", "キャラ撃破数（魔武器攻撃力加算、上限999999）");
            grpKills.Location = new Point(12, 12);
            grpKills.Size = new Size(366, 200);
            this.tabSystemKills.Controls.Add(grpKills);

            this.numToasysKillCount = new NumericUpDown[7];
            string[] killNames = { LangText("卢克", "ルーク"), LangText("缇娅", "ティア"), LangText("杰德", "ジェイド"), LangText("阿妮丝", "アニス"), LangText("凯", "ガイ"), LangText("娜塔莉亚", "ナタリア"), LangText("阿修", "アッシュ") };
            for (int i = 0; i < 7; i++)
            {
                var lblChar = new Label();
                lblChar.Text = killNames[i] + ":";
                lblChar.Location = new Point(12, 22 + i * 20);
                lblChar.Size = new Size(64, 16);
                grpKills.Controls.Add(lblChar);

                this.numToasysKillCount[i] = new NumericUpDown();
                this.numToasysKillCount[i].Location = new Point(80, 20 + i * 20);
                this.numToasysKillCount[i].Size = new Size(130, 20);
                this.numToasysKillCount[i].Minimum = 0;
                this.numToasysKillCount[i].Maximum = 999999;
                this.numToasysKillCount[i].Tag = i;
                this.numToasysKillCount[i].ValueChanged += new System.EventHandler(this.numToasysKillCount_ValueChanged);
                grpKills.Controls.Add(this.numToasysKillCount[i]);
            }

            var lblKillsHint = new Label();
            lblKillsHint.Text = LangText("杀敌数驱动魔武器攻击力（每杀1敌+1攻）。TOASYS 为跨周目累计（NG+继承），单个 TOA_XXX 存档为当前周目。", "撃破数は魔武器攻撃力に反映（1撃破=+1攻）。TOASYSは周回累計（NG+引継）、TOA_XXXは現周回分。");
            lblKillsHint.Location = new Point(12, 170);
            lblKillsHint.Size = new Size(344, 28);
            lblKillsHint.ForeColor = SystemColors.GrayText;
            grpKills.Controls.Add(lblKillsHint);
        }

        private void InitItemsTab()
        {
            this.tabItems.Location = new Point(4, 22);
            this.tabItems.Name = "tabItems";
            this.tabItems.Padding = new Padding(3);
            this.tabItems.Size = new Size(760, 495);
            this.tabItems.TabIndex = 2;
            this.tabItems.Text = LangText("背包管理", "バッグ管理");
            this.tabItems.UseVisualStyleBackColor = true;

            var lblCat = new Label();
            lblCat.Text = LangText("类别筛选:", "カテゴリ絞込:");
            lblCat.Location = new Point(12, 10);
            lblCat.Size = new Size(70, 16);
            this.tabItems.Controls.Add(lblCat);

            this.cmbItemCategory = new ComboBox();
            this.cmbItemCategory.Location = new Point(85, 8);
            this.cmbItemCategory.Size = new Size(130, 20);
            this.cmbItemCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbItemCategory.SelectedIndexChanged += new System.EventHandler(this.cmbItemCategory_SelectedIndexChanged);
            this.tabItems.Controls.Add(this.cmbItemCategory);

            this.btnGetAllItems = new Button();
            this.btnGetAllItems.Text = LangText("全道具获得", "全アイテム獲得");
            this.btnGetAllItems.Location = new Point(360, 6);
            this.btnGetAllItems.Size = new Size(110, 22);
            this.btnGetAllItems.Click += new System.EventHandler(this.btnGetAllItems_Click);
            this.tabItems.Controls.Add(this.btnGetAllItems);

            this.btnGetCategoryItems = new Button();
            this.btnGetCategoryItems.Text = LangText("当前类别全获得", "カテゴリ全獲得");
            this.btnGetCategoryItems.Location = new Point(478, 6);
            this.btnGetCategoryItems.Size = new Size(120, 22);
            this.btnGetCategoryItems.Click += new System.EventHandler(this.btnGetCategoryItems_Click);
            this.tabItems.Controls.Add(this.btnGetCategoryItems);

            this.btnSaveBagState = new Button();
            this.btnSaveBagState.Text = LangText("保存当前背包", "現在のバッグを保存");
            this.btnSaveBagState.Location = new Point(606, 6);
            this.btnSaveBagState.Size = new Size(120, 22);
            this.btnSaveBagState.Click += new System.EventHandler(this.btnSaveBagState_Click);
            this.tabItems.Controls.Add(this.btnSaveBagState);

            this.lblItemWheelHint = new Label();
            this.lblItemWheelHint.Text = LangText("悬停数量列滚动滚轮调整数量（Ctrl×10）", "数量列にカーソルを合わせホイールで調整（Ctrl×10）");
            this.lblItemWheelHint.Location = new Point(12, 31);
            this.lblItemWheelHint.Size = new Size(430, 16);
            this.lblItemWheelHint.ForeColor = SystemColors.GrayText;
            this.tabItems.Controls.Add(this.lblItemWheelHint);

            // 道具搜索框：按名称（或纯数字按ID）实时过滤，与类别筛选叠加
            var lblItemSearch = new Label();
            lblItemSearch.Text = LangText("搜索:", "検索:");
            lblItemSearch.Location = new Point(452, 31);
            lblItemSearch.Size = new Size(40, 16);
            this.tabItems.Controls.Add(lblItemSearch);

            this.txtItemSearch = new TextBox();
            this.txtItemSearch.Location = new Point(496, 28);
            this.txtItemSearch.Size = new Size(256, 20);
            this.txtItemSearch.TextChanged += new System.EventHandler(this.txtItemSearch_TextChanged);
            this.tabItems.Controls.Add(this.txtItemSearch);

            this.dgvItems = new DataGridView();
            this.dgvItems.Location = new Point(12, 50);
            this.dgvItems.Size = new Size(740, 435);
            this.dgvItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.CellValidating += new DataGridViewCellValidatingEventHandler(this.dgvItems_CellValidating);
            this.dgvItems.CellValueChanged += new DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            this.tabItems.Controls.Add(this.dgvItems);
        }

        private NumericUpDown AddNumericRow(Control parent, string label, int x, ref int y, decimal min, decimal max, int fieldWidth = 200)
        {
            var lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(x, y + 2);
            lbl.Size = new Size(140, 20);
            parent.Controls.Add(lbl);

            var num = new NumericUpDown();
            num.Location = new Point(x + 148, y);
            num.Size = new Size(fieldWidth, 22);
            num.Minimum = min;
            num.Maximum = max;
            parent.Controls.Add(num);

            y += 28;
            return num;
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripMenuItem menuFileSaveAs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuLanguage;
        private ToolStripMenuItem menuLangCN;
        private ToolStripMenuItem menuLangJP;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuHelpAbout;
        private ToolStripMenuItem menuTools;
        private ToolStripMenuItem menuToolsCharName;
        private ToolStripMenuItem menuToolsConvertPs2;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private TabControl tabControl;
        private TabPage tabGlobal;
        private TabPage tabCharacter;
        private TabPage tabItems;
        private TabPage tabCooking;
        private TabPage tabSystem;
        private TabPage tabSystemKills;

        private NumericUpDown numGald;
        private NumericUpDown numPlayTime;
        private NumericUpDown numEncount;
        private NumericUpDown numHit;
        private NumericUpDown numGrade;
        private NumericUpDown numCasinoChips;
        private Label lblVersion;
        private Label lblDifficulty;
        private Label lblPartyCount;
        private Label lblLocation;

        private CheckBox chkCCore;
        private CheckBox chkFSChamber;

        private Button btnJournalAll;
        private Button btnItemBookAll;
        private Button btnMapAll;

        private Button btnGetAllEquip;

        private Button btnGetAllItems;
        private Button btnGetCategoryItems;
        private Button btnSaveBagState;

        private ComboBox cmbCharSelect;
        private TabControl charInnerTab;
        private TabPage subTabStats;
        private GroupBox _grpStats;
        private Panel pnlStatBasic;
        private Panel pnlStatCombat;
        private Button btnStatToggle;
        private TabPage subTabEquip;
        private TabPage subTabArtes;
        private TabPage subTabADSkill;
        private TabPage subTabTitle;
        private NumericUpDown numLevel;
        private NumericUpDown numExp;
        private NumericUpDown numHP;
        private NumericUpDown numTP;
        private NumericUpDown numMaxHP;
        private NumericUpDown numMaxTP;
        private NumericUpDown numBasePATK;
        private NumericUpDown numBasePDEF;
        private NumericUpDown numBaseFATK;
        private NumericUpDown numBaseFDEF;
        private NumericUpDown numBaseAGI;
        private NumericUpDown numBaseLUCK;
        private NumericUpDown numOvlGauge;
        private NumericUpDown numKillCount;
        private Label lblTitle;
        private Button btnTitleChange;
        private Label[] lblEquip;
        private Button[] btnEquipChange;
        private ComboBox cmbDifficulty;
        private ComboBox cmbLeader;
        private Label[] lblArte;
        private Button[] btnArteChange;
        private CheckedListBox clbArteLearned;

        private NumericUpDown numGrowthPoints;
        private CheckBox chkLevelGrowth;
        private NumericUpDown numGrowHP;
        private NumericUpDown numGrowTP;
        private NumericUpDown numGrowPATK;
        private NumericUpDown numGrowPDEF;
        private NumericUpDown numGrowFATK;
        private NumericUpDown numGrowFDEF;
        private NumericUpDown numGrowAGI;
        private NumericUpDown numGrowLUK;
        private CheckedListBox clbADSkills;
        private Button btnADSelectAll;
        private Button btnADDeselectAll;
        private Button btnADLearnAll;
        private Button btnTitleOpenAll;
        private CheckedListBox clbTitles;

        private ComboBox cmbItemCategory;
        private DataGridView dgvItems;
        private Label lblItemWheelHint;
        private TextBox txtItemSearch;

        private CheckedListBox clbCooking;
        private Button btnCookingSelectAll;
        private Button btnCookingDeselectAll;
        private PictureBox picCharPortrait;
        private ComboBox[] cmbPartySlot;
        private NumericUpDown numCCorePATK;
        private NumericUpDown numCCorePDEF;
        private NumericUpDown numCCoreFATK;
        private NumericUpDown numCCoreFDEF;
        private NumericUpDown numCCoreAGI;
        private NumericUpDown numCCoreLUK;

        private TabPage tabFSChamber;
        private TabPage tabStoryJump;
        private TabControl subStoryTab;

        // 剧情跳跃标签页控件
        private Label lblStoryCurrentEvent;
        private Label lblStoryCurrentEventVal;
        private Label lblStoryCurrentMap;
        private Label lblStoryCurrentMapVal;
        private Label lblStoryCurrentChapter;
        private Label lblStoryCurrentChapterVal;
        private Label lblStoryChapterSelect;
        private ComboBox cmbStoryChapter;
        private TextBox txtStorySearch;
        private Label lblStoryBranchSelect;
        private ListBox lstStoryBranches;
        private Label lblStoryTargetInfo;
        private Button btnStoryJump;
        private Button btnStoryJumpNoEvent;

        // 支线修改 + 能力习得 控件
        private ComboBox cmbSidePage;
        private TextBox txtSideSearch;
        private ListBox lstSideQuests;
        private Label lblSideTargetInfo;
        private Button btnSideJump;
        private Button btnSideToggleComplete;
        private Button btnSideAllDone;
        private Button btnSideAllReset;
        private ListBox lstQuestSummary;
        private Label[] lblAbilityItems;
        private Label lblSideDoneStatus;
        private ComboBox cmbFSCharSelect;
        private DataGridView dgvFSChamber;
        private NumericUpDown[] numFSChamberMax;
        private Button btnFSAllMax;
        private Button btnFSAllReset;

        private Panel pnlArteUsage;
        private NumericUpDown[] numArteUsage;
        private Label[] lblArteUsage;

        private NumericUpDown[] numCookingMastery;
        private Label[] lblCookingMasteryName;
        private Label[] lblCookingMasteryStar;
        private ComboBox cmbCookingChar;

        private NumericUpDown numToasysDifficulty;
        private NumericUpDown numToasysGald;
        private NumericUpDown numToasysPlaytime;
        private NumericUpDown numToasysGaldSpent;
        private NumericUpDown numToasysSaveCount;
        private NumericUpDown numToasysEncounter;
        private NumericUpDown numToasysClearCount;
        private NumericUpDown numToasysEscape;
        private NumericUpDown numToasysMaxDamage;
        private NumericUpDown numToasysMaxCombo;
        private NumericUpDown numToasysDamageDealt;
        private NumericUpDown numToasysDamageTaken;
        private NumericUpDown numToasysBattleTime;
        private CheckBox chkSoundTest;
        private Button btnToasysUnlockAll;
        private NumericUpDown[] numToasysCharUsage;
        private Label[] lblToasysUsagePct;
        private NumericUpDown[] numToasysKillCount;

        private Button btnMaxAllLevel;
        private Button btnAllTitles;
        private Button btnAllADSkills;
        private Button btnAllFSMax;
        private Button btnAllCookingMax;
        private Button btnAllItemsMax;

        private void InitStoryJumpTab()
        {
            // tabStoryJump 已在 InitializeComponent 中创建并添加到 tabControl，此处直接使用
            this.tabStoryJump.Text = LangText("剧情跳跃", "ストーリージャンプ");
            this.tabStoryJump.Padding = new Padding(3);
            this.tabStoryJump.UseVisualStyleBackColor = true;

            // === 当前剧情进度显示区（左侧，缩小） ===
            var grpCurrent = new GroupBox();
            grpCurrent.Text = LangText("当前存档剧情进度", "現在のストーリー進行");
            grpCurrent.Location = new Point(8, 8);
            grpCurrent.Size = new Size(500, 90);
            this.tabStoryJump.Controls.Add(grpCurrent);

            this.lblStoryCurrentEvent = new Label();
            this.lblStoryCurrentEvent.Text = LangText("当前事件:", "現在イベント:");
            this.lblStoryCurrentEvent.Location = new Point(12, 20);
            this.lblStoryCurrentEvent.Size = new Size(70, 18);
            grpCurrent.Controls.Add(this.lblStoryCurrentEvent);

            this.lblStoryCurrentEventVal = new Label();
            this.lblStoryCurrentEventVal.Location = new Point(82, 20);
            this.lblStoryCurrentEventVal.Size = new Size(408, 18);
            this.lblStoryCurrentEventVal.ForeColor = System.Drawing.Color.Blue;
            grpCurrent.Controls.Add(this.lblStoryCurrentEventVal);

            this.lblStoryCurrentMap = new Label();
            this.lblStoryCurrentMap.Text = LangText("当前地图:", "現在マップ:");
            this.lblStoryCurrentMap.Location = new Point(12, 40);
            this.lblStoryCurrentMap.Size = new Size(70, 18);
            grpCurrent.Controls.Add(this.lblStoryCurrentMap);

            this.lblStoryCurrentMapVal = new Label();
            this.lblStoryCurrentMapVal.Location = new Point(82, 40);
            this.lblStoryCurrentMapVal.Size = new Size(408, 18);
            this.lblStoryCurrentMapVal.ForeColor = System.Drawing.Color.Blue;
            grpCurrent.Controls.Add(this.lblStoryCurrentMapVal);

            this.lblStoryCurrentChapter = new Label();
            this.lblStoryCurrentChapter.Text = LangText("推断章节:", "推定章:");
            this.lblStoryCurrentChapter.Location = new Point(12, 60);
            this.lblStoryCurrentChapter.Size = new Size(70, 18);
            grpCurrent.Controls.Add(this.lblStoryCurrentChapter);

            this.lblStoryCurrentChapterVal = new Label();
            this.lblStoryCurrentChapterVal.Location = new Point(82, 60);
            this.lblStoryCurrentChapterVal.Size = new Size(408, 18);
            this.lblStoryCurrentChapterVal.ForeColor = System.Drawing.Color.Blue;
            grpCurrent.Controls.Add(this.lblStoryCurrentChapterVal);

            // === 缪能力（右侧，纵向排列，双击切换；超振动已隐藏） ===
            var grpAbility = new GroupBox();
            grpAbility.Text = LangText("缪能力（双击切换档位）", "ミュウ能力（ダブルクリックで段階切替）");
            grpAbility.Location = new Point(516, 8);
            grpAbility.Size = new Size(236, 82);
            this.tabStoryJump.Controls.Add(grpAbility);

            int[] abilityY = { 18, 38, 58 };
            this.lblAbilityItems = new Label[3];
            for (int i = 0; i < 3; i++)
            {
                var lbl = new Label();
                lbl.Location = new Point(12, abilityY[i]);
                lbl.Size = new Size(214, 16);
                lbl.Tag = i;
                lbl.Cursor = System.Windows.Forms.Cursors.Hand;
                lbl.ForeColor = System.Drawing.Color.DarkGreen;
                lbl.DoubleClick += new System.EventHandler(this.lblAbility_DoubleClick);
                grpAbility.Controls.Add(lbl);
                this.lblAbilityItems[i] = lbl;
            }

            // === 子标签页：主线修改 + 支线修改 ===
            this.subStoryTab = new TabControl();
            this.subStoryTab.Location = new Point(8, 104);
            this.subStoryTab.Size = new Size(744, 392);
            this.tabStoryJump.Controls.Add(this.subStoryTab);

            // ---- 主线修改 ----
            var tabMainJump = new TabPage();
            tabMainJump.Text = LangText("主线修改", "メインストーリー編集");
            tabMainJump.Padding = new Padding(3);
            tabMainJump.UseVisualStyleBackColor = true;
            this.subStoryTab.Controls.Add(tabMainJump);

            this.lblStoryChapterSelect = new Label();
            this.lblStoryChapterSelect.Text = LangText("章节:", "章:");
            this.lblStoryChapterSelect.Location = new Point(12, 14);
            this.lblStoryChapterSelect.Size = new Size(50, 18);
            tabMainJump.Controls.Add(this.lblStoryChapterSelect);

            this.cmbStoryChapter = new ComboBox();
            this.cmbStoryChapter.Location = new Point(66, 12);
            this.cmbStoryChapter.Size = new Size(180, 20);
            this.cmbStoryChapter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStoryChapter.SelectedIndexChanged += new System.EventHandler(this.cmbStoryChapter_SelectedIndexChanged);
            tabMainJump.Controls.Add(this.cmbStoryChapter);

            var lblStorySearch = new Label();
            lblStorySearch.Text = LangText("搜索:", "検索:");
            lblStorySearch.Location = new Point(252, 14);
            lblStorySearch.Size = new Size(45, 18);
            tabMainJump.Controls.Add(lblStorySearch);

            this.txtStorySearch = new TextBox();
            this.txtStorySearch.Location = new Point(300, 12);
            this.txtStorySearch.Size = new Size(170, 20);
            this.txtStorySearch.TextChanged += new System.EventHandler(this.txtStorySearch_TextChanged);
            tabMainJump.Controls.Add(this.txtStorySearch);

            this.lblStoryBranchSelect = new Label();
            this.lblStoryBranchSelect.Text = LangText("分支:", "シーン:");
            this.lblStoryBranchSelect.Location = new Point(12, 40);
            this.lblStoryBranchSelect.Size = new Size(50, 18);
            tabMainJump.Controls.Add(this.lblStoryBranchSelect);

            this.lstStoryBranches = new ListBox();
            this.lstStoryBranches.Location = new Point(66, 40);
            this.lstStoryBranches.Size = new Size(450, 255);
            this.lstStoryBranches.SelectedIndexChanged += new System.EventHandler(this.lstStoryBranches_SelectedIndexChanged);
            tabMainJump.Controls.Add(this.lstStoryBranches);

            this.lblStoryTargetInfo = new Label();
            this.lblStoryTargetInfo.Location = new Point(526, 40);
            this.lblStoryTargetInfo.Size = new Size(208, 220);
            this.lblStoryTargetInfo.Text = "";
            tabMainJump.Controls.Add(this.lblStoryTargetInfo);

            this.btnStoryJump = new Button();
            this.btnStoryJump.Text = LangText("跳转（地图+事件）", "ジャンプ(マップ+イベント)");
            this.btnStoryJump.Location = new Point(66, 300);
            this.btnStoryJump.Size = new Size(200, 30);
            this.btnStoryJump.Enabled = false;
            this.btnStoryJump.Click += new System.EventHandler(this.btnStoryJump_Click);
            tabMainJump.Controls.Add(this.btnStoryJump);

            this.btnStoryJumpNoEvent = new Button();
            this.btnStoryJumpNoEvent.Text = LangText("仅跳地图", "マップのみ");
            this.btnStoryJumpNoEvent.Location = new Point(276, 300);
            this.btnStoryJumpNoEvent.Size = new Size(140, 30);
            this.btnStoryJumpNoEvent.Enabled = false;
            this.btnStoryJumpNoEvent.Click += new System.EventHandler(this.btnStoryJumpNoEvent_Click);
            tabMainJump.Controls.Add(this.btnStoryJumpNoEvent);

            // ---- 支线修改 ----
            InitSideEditPage();

            // 填充章节下拉框
            if (StoryJumpDatabase.IsLoaded)
            {
                foreach (int chNum in StoryJumpDatabase.GetChapters())
                {
                    var ch = StoryJumpDatabase.GetChapter(chNum);
                    if (ch != null)
                        this.cmbStoryChapter.Items.Add(string.Format("{0} ({1}分支)", ch.chapter_name, ch.menu_branch_count));
                }
                if (this.cmbStoryChapter.Items.Count > 0)
                    this.cmbStoryChapter.SelectedIndex = 0;
            }

            // 初始化能力/支线状态显示（未加载存档时显示"未加载"）
            RefreshAbilityStatus();
            RefreshSideDoneStatus();
        }

        private void InitSideEditPage()
        {
            var tabSideEdit = new TabPage();
            tabSideEdit.Text = LangText("支线修改", "サブクエスト編集");
            tabSideEdit.Padding = new Padding(3);
            tabSideEdit.UseVisualStyleBackColor = true;
            this.subStoryTab.Controls.Add(tabSideEdit);

            this.lblSideDoneStatus = new Label();
            this.lblSideDoneStatus.Location = new Point(12, 6);
            this.lblSideDoneStatus.Size = new Size(720, 18);
            this.lblSideDoneStatus.ForeColor = System.Drawing.Color.DarkGreen;
            tabSideEdit.Controls.Add(this.lblSideDoneStatus);

            this.cmbSidePage = new ComboBox();
            this.cmbSidePage.Location = new Point(12, 28);
            this.cmbSidePage.Size = new Size(80, 20);
            this.cmbSidePage.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSidePage.SelectedIndexChanged += new System.EventHandler(this.cmbSidePage_SelectedIndexChanged);
            tabSideEdit.Controls.Add(this.cmbSidePage);

            this.txtSideSearch = new TextBox();
            this.txtSideSearch.Location = new Point(96, 28);
            this.txtSideSearch.Size = new Size(110, 20);
            this.txtSideSearch.TextChanged += new System.EventHandler(this.txtSideSearch_TextChanged);
            tabSideEdit.Controls.Add(this.txtSideSearch);

            this.btnSideAllDone = new Button();
            this.btnSideAllDone.Text = LangText("全部完成", "全部完了");
            this.btnSideAllDone.Location = new Point(212, 26);
            this.btnSideAllDone.Size = new Size(80, 24);
            this.btnSideAllDone.Click += new System.EventHandler(this.btnSideAllDone_Click);
            tabSideEdit.Controls.Add(this.btnSideAllDone);

            this.btnSideAllReset = new Button();
            this.btnSideAllReset.Text = LangText("全部重置", "全部リセット");
            this.btnSideAllReset.Location = new Point(296, 26);
            this.btnSideAllReset.Size = new Size(80, 24);
            this.btnSideAllReset.Click += new System.EventHandler(this.btnSideAllReset_Click);
            tabSideEdit.Controls.Add(this.btnSideAllReset);

            this.btnSideToggleComplete = new Button();
            this.btnSideToggleComplete.Text = LangText("切换完成状态", "完了状態を切替");
            this.btnSideToggleComplete.Location = new Point(380, 26);
            this.btnSideToggleComplete.Size = new Size(120, 24);
            this.btnSideToggleComplete.Enabled = false;
            this.btnSideToggleComplete.Click += new System.EventHandler(this.btnSideToggleComplete_Click);
            tabSideEdit.Controls.Add(this.btnSideToggleComplete);

            this.btnSideJump = new Button();
            this.btnSideJump.Text = LangText("跳转（地图+事件+flag）", "ジャンプ(マップ+イベント+flag)");
            this.btnSideJump.Location = new Point(504, 26);
            this.btnSideJump.Size = new Size(228, 24);
            this.btnSideJump.Enabled = false;
            this.btnSideJump.Click += new System.EventHandler(this.btnSideJump_Click);
            tabSideEdit.Controls.Add(this.btnSideJump);

            this.lstSideQuests = new ListBox();
            this.lstSideQuests.Location = new Point(12, 58);
            this.lstSideQuests.Size = new Size(420, 290);
            this.lstSideQuests.SelectedIndexChanged += new System.EventHandler(this.lstSideQuests_SelectedIndexChanged);
            this.lstSideQuests.DoubleClick += new System.EventHandler(this.lstSideQuests_DoubleClick);
            tabSideEdit.Controls.Add(this.lstSideQuests);

            this.lblSideTargetInfo = new Label();
            this.lblSideTargetInfo.Location = new Point(440, 58);
            this.lblSideTargetInfo.Size = new Size(292, 115);
            this.lblSideTargetInfo.Text = "";
            tabSideEdit.Controls.Add(this.lblSideTargetInfo);

            var lblQuestSummaryTitle = new Label();
            lblQuestSummaryTitle.Text = LangText("聚合完成度（多步任务）：", "クエスト集計完了度：");
            lblQuestSummaryTitle.Location = new Point(440, 177);
            lblQuestSummaryTitle.Size = new Size(292, 16);
            lblQuestSummaryTitle.ForeColor = System.Drawing.Color.DarkBlue;
            tabSideEdit.Controls.Add(lblQuestSummaryTitle);

            this.lstQuestSummary = new ListBox();
            this.lstQuestSummary.Location = new Point(440, 195);
            this.lstQuestSummary.Size = new Size(292, 153);
            tabSideEdit.Controls.Add(this.lstQuestSummary);

            foreach (int pg in SideQuestJumpDatabase.GetPages())
            {
                this.cmbSidePage.Items.Add(string.Format(LangText("第 {0} 页", "第{0}ページ"), pg));
            }
            if (this.cmbSidePage.Items.Count > 0)
                this.cmbSidePage.SelectedIndex = 0;
        }

    }
}
