using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Linq;

namespace DoAnTinHoc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // ==== HÀM ĐỌC FILE CSV ====
        private List<string[]> ReadCsvFile(string filePath)
        {
            List<string[]> rows = new List<string[]>();

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file " + filePath);
                    return rows;
                }

                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] values = line.Split(',');
                    rows.Add(values);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc: {ex.Message}");
            }

            return rows;
        }

        // ==== HÀM GHI FILE CSV ====
        private void WriteCsvFile(string filePath, List<string[]> data)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var row in data)
                    {
                        string line = string.Join(",", row);
                        writer.WriteLine(line);
                    }
                }

                MessageBox.Show($"Ghi file CSV thành công: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi: {ex.Message}");
            }
        }

        // ==== HIỂN THỊ DỮ LIỆU LÊN DATAGRIDVIEW ====
        private void ShowDataOnGrid(List<string[]> data)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để hiển thị!");
                return;
            }

            DataTable table = new DataTable();

            // Dòng đầu tiên làm header
            var header = data[0];
            foreach (var colName in header)
                table.Columns.Add(colName);  // đặt tên cột chính xác

            // Thêm dữ liệu còn lại
            for (int i = 1; i < data.Count; i++)
                table.Rows.Add(data[i]);

            dataGridView1.DataSource = table;

            dataGridView1.ColumnHeadersVisible = true;  // hiện header từ CSV
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }



        // ==== NÚT ĐỌC FILE ====
        private void button1_Click(object sender, EventArgs e)
        {
            string csvFilePath = Path.Combine(Application.StartupPath, "Data.csv");
            var csvData = ReadCsvFile(csvFilePath);
            ShowDataOnGrid(csvData);
        }

        // ==== NÚT GHI FILE ====
        private void button2_Click(object sender, EventArgs e)
        {
            string outputPath = Path.Combine(Application.StartupPath, "Out.csv");

            // Nếu DataGridView trống, báo lỗi
            if (dataGridView1.DataSource == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để ghi!");
                return;
            }

            // Đọc dữ liệu từ DataGridView hiện tại
            List<string[]> gridData = new List<string[]>();

            // Lấy header
            var headers = dataGridView1.Columns
                .Cast<DataGridViewColumn>()
                .Select(c => c.HeaderText)
                .ToArray();

            gridData.Add(headers);

            // Lấy từng dòng dữ liệu (bỏ dòng trống cuối)
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string[] rowData = new string[dataGridView1.Columns.Count];
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    rowData[i] = row.Cells[i].Value?.ToString() ?? "";
                }
                gridData.Add(rowData);
            }

            // Ghi ra file CSV
            WriteCsvFile(outputPath, gridData);
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAVL_Click(object sender, EventArgs e)
        {
            string csvFilePath = Path.Combine(Application.StartupPath, "Data.csv");
            var rows = ReadCsvFile(csvFilePath);
            if (rows.Count <= 1) return;

            var header = rows[0];
            var dataRows = rows.Skip(1).ToList();

            int examScoreIndex = Array.IndexOf(header, "exam_score");
            if (examScoreIndex == -1)
            {
                MessageBox.Show("Không tìm thấy cột exam_score!");
                return;
            }

            AVLTree tree = new AVLTree();
            AVLNode root = null;

            foreach (var row in dataRows)
            {
                try
                {
                    double score = double.Parse(row[examScoreIndex]);
                    root = tree.Insert(root, score, row);
                }
                catch
                {
                    
                }
            }

            var sortedNodes = tree.InOrder(root);
            sortedNodes.Reverse();//tăng
            DataTable table = new DataTable();
            foreach (var col in header)
                table.Columns.Add(col);

            foreach (var node in sortedNodes)
                table.Rows.Add(node.RowData);

            dataGridView1.DataSource = table;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

    }
}
