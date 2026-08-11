using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TOAHEX
{
    public class TitleSelectForm : Form
    {
        private DataGridView dgvTitles;
        private Button btnOK;
        private Button btnCancel;
        private int _charSlotIndex;
        public int SelectedTitleIndex { get; private set; }

        public TitleSelectForm(int charSlotIndex)
        {
            _charSlotIndex = charSlotIndex;
            Text = LangText("选择称号", "称号選択");
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            dgvTitles = new DataGridView();
            dgvTitles.Location = new Point(12, 12);
            dgvTitles.Size = new Size(460, 300);
            dgvTitles.AllowUserToAddRows = false;
            dgvTitles.AllowUserToDeleteRows = false;
            dgvTitles.ReadOnly = true;
            dgvTitles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTitles.MultiSelect = false;
            dgvTitles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTitles.CellDoubleClick += DgvTitles_CellDoubleClick;
            Controls.Add(dgvTitles);

            btnOK = new Button();
            btnOK.Text = LangText("确定", "OK");
            btnOK.Location = new Point(300, 324);
            btnOK.Size = new Size(80, 28);
            btnOK.Click += BtnOK_Click;
            Controls.Add(btnOK);

            btnCancel = new Button();
            btnCancel.Text = LangText("取消", "キャンセル");
            btnCancel.Location = new Point(392, 324);
            btnCancel.Size = new Size(80, 28);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            BuildTitleTable();
            SelectedTitleIndex = -1;
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }

        private void BuildTitleTable()
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add(LangText("索引", "索引"), typeof(int));
                table.Columns.Add(LangText("日文名", "日本語名"), typeof(string));
                table.Columns.Add(LangText("中文名", "中国語名"), typeof(string));

                var titles = TitleDatabase.GetAllTitles(_charSlotIndex);
                foreach (var t in titles)
                    table.Rows.Add(t.Index, t.Jp, t.Cn);

                dgvTitles.DataSource = table;
                if (dgvTitles.Columns.Count > 0)
                {
                    string colIdx = LangText("索引", "索引");
                    string colJp = LangText("日文名", "日本語名");
                    string colCn = LangText("中文名", "中国語名");
                    if (dgvTitles.Columns.Contains(colIdx))
                        dgvTitles.Columns[colIdx].Width = 50;
                    if (dgvTitles.Columns.Contains(colJp))
                        dgvTitles.Columns[colJp].Width = 200;
                    if (dgvTitles.Columns.Contains(colCn))
                        dgvTitles.Columns[colCn].Width = 200;
                }
            }
            catch { }
        }

        private void DgvTitles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) SelectRow(e.RowIndex);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dgvTitles.SelectedRows.Count > 0) SelectRow(dgvTitles.SelectedRows[0].Index);
        }

        private void SelectRow(int rowIndex)
        {
            try
            {
                var row = dgvTitles.Rows[rowIndex];
                string colIdx = LangText("索引", "索引");
                if (dgvTitles.Columns.Contains(colIdx) && row.Cells[colIdx].Value != null)
                {
                    SelectedTitleIndex = (int)row.Cells[colIdx].Value;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch { }
        }
    }
}
