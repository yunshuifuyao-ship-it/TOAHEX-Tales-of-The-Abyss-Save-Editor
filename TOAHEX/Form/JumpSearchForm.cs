using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TOAHEX
{
    /// <summary>
    /// 自定义跳转辅助窗口：按名称/编号搜索地图或事件，双击条目（或选中后点确定）回填 ID。
    /// 模式 Map：枚举 StoryJumpDatabase 全部地图；模式 Event：主线全部跳转条目 + 支线条目。
    /// </summary>
    public class JumpSearchForm : Form
    {
        public enum SearchMode { Map, Event }

        private readonly SearchMode _mode;
        private TextBox txtSearch;
        private DataGridView dgvList;
        private Button btnOK;
        private Button btnCancel;
        private Label lblCount;

        // 行数据: [0]=id(uint) [1]=名称 [2]=来源
        private readonly List<object[]> _rows = new List<object[]>();

        /// <summary>双击/确定后选中的 ID（地图ID 或 事件ID）。</summary>
        public uint SelectedId { get; private set; }
        /// <summary>选中条目的名称（用于状态栏提示）。</summary>
        public string SelectedName { get; private set; }

        public JumpSearchForm(SearchMode mode)
        {
            _mode = mode;
            Text = mode == SearchMode.Map
                ? LangText("搜索地图（双击条目填入）", "マップ検索（ダブルクリックで反映）")
                : LangText("搜索事件（双击条目填入）", "イベント検索（ダブルクリックで反映）");
            Size = new Size(520, 460);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            try { Icon = MainForm.GetAppIcon(); } catch { }

            txtSearch = new TextBox();
            txtSearch.Location = new Point(12, 12);
            txtSearch.Size = new Size(480, 22);
            txtSearch.TextChanged += (s, e) => RebuildList();
            Controls.Add(txtSearch);

            dgvList = new DataGridView();
            dgvList.Location = new Point(12, 44);
            dgvList.Size = new Size(480, 330);
            dgvList.AllowUserToAddRows = false;
            dgvList.AllowUserToDeleteRows = false;
            dgvList.AllowUserToResizeRows = false;
            dgvList.ReadOnly = true;
            dgvList.RowHeadersVisible = false;
            dgvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvList.MultiSelect = false;
            dgvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvList.CellDoubleClick += DgvList_CellDoubleClick;
            Controls.Add(dgvList);

            lblCount = new Label();
            lblCount.Location = new Point(12, 382);
            lblCount.Size = new Size(300, 18);
            lblCount.ForeColor = Color.DimGray;
            Controls.Add(lblCount);

            btnOK = new Button();
            btnOK.Text = LangText("填入所选", "反映");
            btnOK.Location = new Point(312, 378);
            btnOK.Size = new Size(88, 28);
            btnOK.Click += (s, e) => PickSelected();
            Controls.Add(btnOK);

            btnCancel = new Button();
            btnCancel.Text = LangText("取消", "キャンセル");
            btnCancel.Location = new Point(408, 378);
            btnCancel.Size = new Size(84, 28);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            BuildRows();
            RebuildList();
        }

        private void BuildRows()
        {
            if (_mode == SearchMode.Map)
            {
                // 全部地图，按 ID 升序；重复名保留（同名多层地图）
                foreach (var kv in StoryJumpDatabase.GetAllMaps().OrderBy(x => x.Key))
                    _rows.Add(new object[] { kv.Key, kv.Value, LangText("地图表", "maptable") });
            }
            else
            {
                // 特殊值：0 = 无事件（仅跳地图）
                _rows.Add(new object[] { 0u, LangText("（无事件 · 仅跳地图）", "（イベントなし・マップのみ）"), "-" });

                // 主线 + 支线按「章节 → 事件ID」升序混排：
                // 主线用其章号；支线无章号字段，用事件ID反推所属主线章节
                // （StoryJumpDatabase.EventIdToChapter：先与主线条目精确匹配，再按 ID 公式回推）。
                var all = new List<Tuple<int, uint, object[]>>();
                foreach (var kv in StoryJumpDatabase.GetAllJumpEntries())
                    all.Add(Tuple.Create(kv.Key, kv.Value.event_id, new object[] { kv.Value.event_id, kv.Value.menu_name,
                        string.Format(LangText("第{0}章", "{0}章"), kv.Key) }));
                foreach (int pg in SideQuestJumpDatabase.GetPages())
                    foreach (var ent in SideQuestJumpDatabase.GetEntries(pg))
                        if (ent.event_id > 0)
                            all.Add(Tuple.Create((int)StoryJumpDatabase.EventIdToChapter(ent.event_id), ent.event_id,
                                new object[] { ent.event_id, ent.name,
                                    string.Format(LangText("支线{0}页", "サブ{0}P"), pg) }));

                foreach (var t in all.OrderBy(x => x.Item1).ThenBy(x => x.Item2))
                    _rows.Add(t.Item3);
            }
        }

        private void RebuildList()
        {
            string kw = (txtSearch.Text ?? "").Trim();
            dgvList.Columns.Clear();
            dgvList.Rows.Clear();

            dgvList.Columns.Add("cId", "ID");
            dgvList.Columns.Add("cName", _mode == SearchMode.Map ? LangText("地图名", "マップ名") : LangText("事件/剧情名", "イベント名"));
            dgvList.Columns.Add("cSrc", LangText("来源", "来源"));
            dgvList.Columns[0].FillWeight = 18;
            dgvList.Columns[1].FillWeight = 58;
            dgvList.Columns[2].FillWeight = 24;

            int shown = 0;
            foreach (var r in _rows)
            {
                string idStr = ((uint)r[0]).ToString();
                string name = (string)r[1];
                if (kw.Length > 0 && !idStr.Contains(kw) && name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                dgvList.Rows.Add(idStr, name, r[2]);
                shown++;
            }
            lblCount.Text = string.Format(LangText("共 {0} 条（双击填入）", "全{0}件（ダブルクリックで反映）"), shown);
        }

        private void DgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) PickSelected();
        }

        private void PickSelected()
        {
            if (dgvList.CurrentRow == null) return;
            if (!uint.TryParse(dgvList.CurrentRow.Cells[0].Value as string, out uint id)) return;
            SelectedId = id;
            SelectedName = dgvList.CurrentRow.Cells[1].Value as string;
            DialogResult = DialogResult.OK;
            Close();
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }
    }
}
