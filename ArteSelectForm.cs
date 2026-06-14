using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TOAHEX
{
    public class ArteSelectForm : Form
    {
        private TextBox txtSearch;
        private DataGridView dgvArtes;
        private Button btnOK;
        private Button btnCancel;
        public int SelectedArteId { get; private set; }
        private int _charIndex = -1;

        public ArteSelectForm()
        {
            Text = LangText("选择术技", "アーツ選択");
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(12, 12);
            txtSearch.Size = new Size(460, 22);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            Controls.Add(txtSearch);

            dgvArtes = new DataGridView();
            dgvArtes.Location = new Point(12, 44);
            dgvArtes.Size = new Size(460, 270);
            dgvArtes.AllowUserToAddRows = false;
            dgvArtes.AllowUserToDeleteRows = false;
            dgvArtes.ReadOnly = true;
            dgvArtes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvArtes.MultiSelect = false;
            dgvArtes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvArtes.CellDoubleClick += DgvArtes_CellDoubleClick;
            Controls.Add(dgvArtes);

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

            BuildArteTable("");
            SelectedArteId = -1;
        }

        public ArteSelectForm(int charIndex)
        {
            _charIndex = charIndex;
            Text = LangText("选择术技", "アーツ選択");
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(12, 12);
            txtSearch.Size = new Size(460, 22);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            Controls.Add(txtSearch);

            dgvArtes = new DataGridView();
            dgvArtes.Location = new Point(12, 44);
            dgvArtes.Size = new Size(460, 270);
            dgvArtes.AllowUserToAddRows = false;
            dgvArtes.AllowUserToDeleteRows = false;
            dgvArtes.ReadOnly = true;
            dgvArtes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvArtes.MultiSelect = false;
            dgvArtes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvArtes.CellDoubleClick += DgvArtes_CellDoubleClick;
            Controls.Add(dgvArtes);

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

            BuildArteTable("");
            SelectedArteId = -1;
        }

        private string LangText(string cn, string jp)
        {
            return LanguageConfig.Current == Language.JP ? jp : cn;
        }

        private void BuildArteTable(string keyword)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add(LangText("日文名", "日本語名"), typeof(string));
                table.Columns.Add(LangText("中文名", "中国語名"), typeof(string));

                if (_charIndex > 0)
                {
                    var arteIds = ArteDatabase.GetArteIdsForChar(_charIndex);
                    foreach (var arteId in arteIds)
                    {
                        var entry = ArteDatabase.GetById(arteId);
                        if (string.IsNullOrEmpty(keyword) ||
                            entry.jp.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) ||
                            entry.cn.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                        {
                            table.Rows.Add(arteId, entry.jp, entry.cn);
                        }
                    }
                }
                else
                {
                    var artes = string.IsNullOrEmpty(keyword) ? ArteDatabase.GetAll() : ArteDatabase.Search(keyword);
                    if (artes != null)
                    {
                        foreach (var a in artes)
                            table.Rows.Add(a.id, a.jp, a.cn);
                    }
                }

                dgvArtes.DataSource = table;
                if (dgvArtes.Columns.Count > 0)
                {
                    dgvArtes.Columns["ID"].Width = 50;
                    string colJp = LangText("日文名", "日本語名");
                    string colCn = LangText("中文名", "中国語名");
                    if (dgvArtes.Columns.Contains(colJp))
                        dgvArtes.Columns[colJp].Width = 200;
                    if (dgvArtes.Columns.Contains(colCn))
                        dgvArtes.Columns[colCn].Width = 200;
                }
            }
            catch { }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            BuildArteTable(txtSearch.Text);
        }

        private void DgvArtes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) SelectRow(e.RowIndex);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dgvArtes.SelectedRows.Count > 0) SelectRow(dgvArtes.SelectedRows[0].Index);
        }

        private void SelectRow(int rowIndex)
        {
            try
            {
                var row = dgvArtes.Rows[rowIndex];
                if (row.Cells["ID"].Value != null)
                {
                    SelectedArteId = (int)row.Cells["ID"].Value;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch { }
        }
    }
}
