using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using Microsoft.VisualBasic;
using System.Text;

namespace DoAnTinHoc
{
    public partial class Form1 : Form
    {
        
        private AVLTree tree;
        private AVLNode root;
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
            string outputPath = Path.Combine(Application.StartupPath, "out.json");

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = comboBox1.SelectedItem.ToString(); // Lấy lựa chọn

            if (selected == "age")
            {
                string csvFilePath = Path.Combine(Application.StartupPath, "Data.csv");
                var rows = ReadCsvFile(csvFilePath);
                if (rows.Count <= 1) return;

                var header = rows[0];
                var dataRows = rows.Skip(1).ToList();

                int examScoreIndex = Array.IndexOf(header, "age");
                if (examScoreIndex == -1)
                {
                    MessageBox.Show("Không tìm thấy cột age!");
                    return;
                }

                tree = new AVLTree();
                root = null;
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
                sortedNodes.Reverse();
                DataTable table = new DataTable();
                foreach (var col in header)
                    table.Columns.Add(col);

                foreach (var node in sortedNodes)
                    table.Rows.Add(node.RowData);

                dataGridView1.DataSource = table;
                dataGridView1.ColumnHeadersVisible = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else if (selected == "attendance_percentage")
            {
                string csvFilePath = Path.Combine(Application.StartupPath, "Data.csv");
                var rows = ReadCsvFile(csvFilePath);
                if (rows.Count <= 1) return;

                var header = rows[0];
                var dataRows = rows.Skip(1).ToList();

                int examScoreIndex = Array.IndexOf(header, "attendance_percentage");
                if (examScoreIndex == -1)
                {
                    MessageBox.Show("Không tìm thấy cột sleep_hours!");
                    return;
                }
                tree = new AVLTree();
                root = null;

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
                sortedNodes.Reverse();
                DataTable table = new DataTable();
                foreach (var col in header)
                    table.Columns.Add(col);

                foreach (var node in sortedNodes)
                    table.Rows.Add(node.RowData);

                dataGridView1.DataSource = table;
                dataGridView1.ColumnHeadersVisible = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else if (selected == "study_hours_per_day")
            {
                string csvFilePath = Path.Combine(Application.StartupPath, "Data.csv");
                var rows = ReadCsvFile(csvFilePath);
                if (rows.Count <= 1) return;

                var header = rows[0];
                var dataRows = rows.Skip(1).ToList();

                int examScoreIndex = Array.IndexOf(header, "study_hours_per_day");
                if (examScoreIndex == -1)
                {
                    MessageBox.Show("Không tìm thấy cột study_hours_per_day!");
                    return;
                }
                tree = new AVLTree();
                root = null;

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
                sortedNodes.Reverse();
                DataTable table = new DataTable();
                foreach (var col in header)
                    table.Columns.Add(col);

                foreach (var node in sortedNodes)
                    table.Rows.Add(node.RowData);

                dataGridView1.DataSource = table;
                dataGridView1.ColumnHeadersVisible = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
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
                tree = new AVLTree();
                root = null;

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
                sortedNodes.Reverse();
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


        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (tree == null || root == null)
            {
                MessageBox.Show("Vui lòng chọn dữ liệu ở ComboBox1 trước!");
                return;
            }

            string selected = comboBox2.SelectedItem.ToString();
            if (selected == "chiều cao cây")
            {
                int height = tree.GetTreeHeight(root);
                MessageBox.Show($"Chiều cao của cây AVL là: {height}");
            }
            else if (selected == "số node lá")
            {
                int count = tree.CountLeafNodes(root);
                MessageBox.Show($"Số node lá trong cây là: {count}");
            }
            else if (selected == "GTNN")
            {
                double min = tree.FindMin(root);
                MessageBox.Show($"Giá trị nhỏ nhất trong cây: {min}");
            }
            else if (selected == "GTLN")
            {
                double max = tree.FindMax(root);
                MessageBox.Show($"Giá trị lớn nhất trong cây: {max}");
            }
            else if (selected == "Tìm dữ liệu ")
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Nhập giá trị cần tìm:", "Tìm kiếm");

                if (double.TryParse(input, out double x))
                {
                    var foundNodes = tree.SearchAll(root, x);
                    if (foundNodes.Count > 0)
                    {
                        DataTable table = (DataTable)dataGridView1.DataSource;
                        DataTable resultTable = table.Clone();

                        foreach (var node in foundNodes)
                        {
                            resultTable.Rows.Add(node.RowData);
                        }

                        // Hiển thị kết quả tìm được
                        dataGridView1.DataSource = resultTable;

                        MessageBox.Show($"Đã hiển thị {foundNodes.Count} kết quả tìm thấy cho {x}!");
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy giá trị {x} trong cây.");
                    }
                }
                else
                {
                    MessageBox.Show("Giá trị nhập không hợp lệ!");
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Vui lòng chọn dữ liệu ở ComboBox1 trước!");
                return;
            }

            DataTable table = (DataTable)dataGridView1.DataSource;
            if (table.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu!");
                return;
            }


            string input = Microsoft.VisualBasic.Interaction.InputBox("Nhập số dòng cần lấy:", "Lấy dòng đầu tiên");

            if (!int.TryParse(input, out int k) || k <= 0)
            {
                MessageBox.Show("Giá trị nhập không hợp lệ!");
                return;
            }

            int takeCount = Math.Min(k, table.Rows.Count);
            var topRows = table.AsEnumerable().Take(takeCount).ToList();

            DataTable newTable = table.Clone();
            foreach (var row in topRows)
                newTable.ImportRow(row);

            dataGridView1.DataSource = newTable;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersVisible = true;

            MessageBox.Show($"Đã lấy {takeCount} dòng đầu tiên từ dữ liệu!",
                            "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Vui lòng nhấn Button3 để lấy dữ liệu trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable table = (DataTable)dataGridView1.DataSource;
            if (table.Rows.Count == 0)
            {
                MessageBox.Show("DataGridView không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show("DataGridView không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!table.Columns.Contains("thayTruong"))
            {
                MessageBox.Show("Không tìm thấy cột 'thầy Trường' trong bảng dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int colIndex = table.Columns["thayTruong"].Ordinal;


            AVLTree tree = new AVLTree();
            AVLNode root = null;

            foreach (DataRow row in table.Rows)
            {
                if (double.TryParse(row[colIndex].ToString(), out double key))
                {
                    string[] rowData = row.ItemArray.Select(x => x.ToString()).ToArray();
                    root = tree.Insert(root, key, rowData);
                }
            }

            // Nhập tầng cần in
            string input = Microsoft.VisualBasic.Interaction.InputBox("Nhập các tầng muốn in (cách nhau bởi dấu phẩy, root = 0):","In tầng của cây");

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Bạn chưa nhập tầng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tách danh sách các tầng hợp lệ
            var levelStrings = input.Split(',')
                                    .Select(s => s.Trim())
                                    .Where(s => int.TryParse(s, out _))
                                    .ToList();

            if (levelStrings.Count == 0)
            {
                MessageBox.Show("Không có giá trị tầng hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<int> levels = levelStrings.Select(int.Parse).Distinct().ToList();

            // Duyệt cây để lấy tất cả node ở các tầng
            List<string[]> nodesAtLevel = new List<string[]>();

            void TraverseByLevel(AVLNode node, int currentLevel)
            {
                if (node == null) return;
                if (levels.Contains(currentLevel))
                    nodesAtLevel.Add(node.RowData);

                TraverseByLevel(node.Left, currentLevel + 1);
                TraverseByLevel(node.Right, currentLevel + 1);
            }

            TraverseByLevel(root, 0);

            // Hiển thị kết quả ra DataGridView
            DataTable newTable = table.Clone(); // giữ cấu trúc cột
            foreach (var rowData in nodesAtLevel)
                newTable.Rows.Add(rowData);

            dataGridView1.DataSource = newTable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersVisible = true;

            MessageBox.Show(
                $"Đã hiển thị {nodesAtLevel.Count} node ở các tầng: {string.Join(", ", levels)}.",
                "Kết quả",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}