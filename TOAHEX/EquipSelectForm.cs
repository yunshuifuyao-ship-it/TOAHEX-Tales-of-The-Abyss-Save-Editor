using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TOAHEX
{
    /// <summary>
    /// 装备选择器对话框（术技选择器同款模式）：
    /// 槽位 0-3 = 武器/防具/饰品1/饰品2（EquipIndexDatabase 按角色+槽位过滤），
    /// 槽位 4 = 响律符（KyouritsufuDatabase）。顶部搜索框实时过滤，
    /// "(无)"与当前已装备项恒保留可见。确定/双击返回 SelectedEquipId。
    /// </summary>
    public class EquipSelectForm : Form
    {
        private TextBox txtSearch;
        private DataGridView dgvItems;
        private Button btnOK;
        private Button btnCancel;
        public int SelectedEquipId { get; private set; }

        private readonly int _charIndex;
        private readonly int _slotIndex;
        private readonly int _currentId;

        public EquipSelectForm(int charIndex, int slotIndex, int currentId)
        {
            _charIndex = charIndex;
            _slotIndex = slotIndex;
            _currentId = currentId;
            SelectedEquipId = -1;

            Text = LangText("选择装备", "装備選択") + " - " + SlotName(slotIndex);
            Size = new Size(500, 420);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(12, 12);
            txtSearch.Size = new Size(460, 22);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            Controls.Add(txtSearch);

            dgvItems = new DataGridView();
            dgvItems.Location = new Point(12, 44);
            dgvItems.Size = new Size(460, 290);
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.ReadOnly = true;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.CellDoubleClick += DgvItems_CellDoubleClick;
            Controls.Add(dgvItems);

            btnOK = new Button();
            btnOK.Text = LangText("确定", "OK");
            btnOK.Location = new Point(300, 344);
            btnOK.Size = new Size(80, 28);
            btnOK.Click += BtnOK_Click;
            Controls.Add(btnOK);

            btnCancel = new Button();
            btnCancel.Text = LangText("取消", "キャンセル");
            btnCancel.Location = new Point(392, 344);
            btnCancel.Size = new Size(80, 28);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            BuildTable("");
        }

        public static string SlotName(int slotIndex)
        {
            switch (slotIndex)
            {
                case 0: return LanguageConfig.Current == Language.JP ? "武器" : "武器";
                case 1: return LanguageConfig.Current == Language.JP ? "防具" : "防具";
                case 2: return LanguageConfig.Current == Language.JP ? "アクセ1" : "饰品1";
                case 3: return LanguageConfig.Current == Language.JP ? "アクセ2" : "饰品2";
                case 4: return LanguageConfig.Current == Language.JP ? "響律符" : "响律符";
                default: return "?";
            }
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }

        // 构建列表：首项"(无)"(Id=0)；其余按槽位数据源；搜索时"(无)"与当前装备项恒保留
        private void BuildTable(string keyword)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add(LangText("名称", "名称"), typeof(string));

                string noneText = LangText("(无)", "(なし)");
                bool keywordEmpty = string.IsNullOrEmpty(keyword);
                table.Rows.Add(0, noneText);

                if (_slotIndex >= 0 && _slotIndex <= 3)
                {
                    foreach (var item in EquipIndexDatabase.GetEquipItemsForSlot(_charIndex, _slotIndex))
                    {
                        if (string.IsNullOrEmpty(item.Name)) continue;
                        if (item.Id == _currentId) continue; // 当前装备项由末尾统一补回，避免重复
                        if (!keywordEmpty && item.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        table.Rows.Add(item.Id, item.Name);
                    }
                }
                else if (_slotIndex == 4)
                {
                    foreach (var entry in KyouritsufuDatabase.GetAll())
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;
                        if (entry.Id == _currentId) continue;
                        if (!keywordEmpty && entry.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        table.Rows.Add(entry.Id, entry.Name);
                    }
                }

                // 当前已装备项恒在列表末尾可见（即使不匹配搜索词）
                if (_currentId != 0)
                {
                    string currentName = ResolveName(_currentId);
                    table.Rows.Add(_currentId, currentName);
                }

                dgvItems.DataSource = table;
                if (dgvItems.Columns.Count > 0)
                {
                    dgvItems.Columns["ID"].Width = 50;
                }

                // 预选当前装备项（不可见时回退首行）
                SelectCurrent();
            }
            catch { }
        }

        private string ResolveName(int id)
        {
            if (id == 0) return LangText("(无)", "(なし)");
            if (_slotIndex == 4)
            {
                string n = KyouritsufuDatabase.GetName(id);
                return string.IsNullOrEmpty(n) ? string.Format("(ID:{0})", id) : n;
            }
            string name = ItemDatabase.GetById(id)?.Name;
            return string.IsNullOrEmpty(name) ? string.Format("(ID:{0})", id) : name;
        }

        private void SelectCurrent()
        {
            try
            {
                for (int i = 0; i < dgvItems.Rows.Count; i++)
                {
                    if ((int)dgvItems.Rows[i].Cells["ID"].Value == _currentId)
                    {
                        dgvItems.ClearSelection();
                        dgvItems.Rows[i].Selected = true;
                        dgvItems.CurrentCell = dgvItems.Rows[i].Cells[0];
                        return;
                    }
                }
                if (dgvItems.Rows.Count > 0)
                {
                    dgvItems.ClearSelection();
                    dgvItems.Rows[0].Selected = true;
                    dgvItems.CurrentCell = dgvItems.Rows[0].Cells[0];
                }
            }
            catch { }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            BuildTable(txtSearch.Text);
        }

        private void DgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) SelectRow(e.RowIndex);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0) SelectRow(dgvItems.SelectedRows[0].Index);
        }

        private void SelectRow(int rowIndex)
        {
            try
            {
                var row = dgvItems.Rows[rowIndex];
                if (row.Cells["ID"].Value != null)
                {
                    SelectedEquipId = (int)row.Cells["ID"].Value;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch { }
        }
    }
}
