using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VisionCalibrationProject.Forms
{
    public partial class ResultDisplayForm : Form
    {
        private DataGridView resultDataGridView;
        private Chart errorChart;
        private DataTable resultDataTable;

        public ResultDisplayForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ResultDisplayForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "ResultDisplayForm";
            this.Text = "结果展示界面";
            this.ResumeLayout(false);
        }

        private void InitializeUI()
        {
            // 参数表格
            resultDataGridView = new DataGridView();
            resultDataGridView.Dock = DockStyle.Top;
            resultDataGridView.Height = this.Height / 2;
            resultDataTable = new DataTable();
            resultDataGridView.DataSource = resultDataTable;
            this.Controls.Add(resultDataGridView);

            // 误差图表
            errorChart = new Chart();
            errorChart.Dock = DockStyle.Fill;
            ChartArea chartArea = new ChartArea();
            errorChart.ChartAreas.Add(chartArea);
            Series errorSeries = new Series();
            errorSeries.ChartType = SeriesChartType.Bar;
            errorChart.Series.Add(errorSeries);
            this.Controls.Add(errorChart);

            // 保存图表按钮
            Button saveChartButton = new Button();
            saveChartButton.Text = "保存图表";
            saveChartButton.Location = new Point(20, this.Height - 50);
            saveChartButton.Click += SaveChartButton_Click;
            this.Controls.Add(saveChartButton);

            // 打印按钮
            Button printButton = new Button();
            printButton.Text = "打印结果";
            printButton.Location = new Point(150, this.Height - 50);
            printButton.Click += PrintButton_Click;
            this.Controls.Add(printButton);

            // 复制按钮
            Button copyButton = new Button();
            copyButton.Text = "复制数据";
            copyButton.Location = new Point(280, this.Height - 50);
            copyButton.Click += CopyButton_Click;
            this.Controls.Add(copyButton);
        }

        public void SetResultData(DataTable dataTable)
        {
            resultDataTable = dataTable;
            resultDataGridView.DataSource = resultDataTable;
        }

        public void SetErrorData(double[] errorData)
        {
            Series errorSeries = errorChart.Series[0];
            errorSeries.Points.Clear();
            for (int i = 0; i < errorData.Length; i++)
            {
                errorSeries.Points.AddY(errorData[i]);
            }
        }

        private void SaveChartButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG图像 (*.png)|*.png|JPEG图像 (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                ChartImageFormat imageFormat = ChartImageFormat.Png;
                if (fileExtension == ".jpg")
                {
                    imageFormat = ChartImageFormat.Jpeg;
                }
                errorChart.SaveImage(saveFileDialog.FileName, imageFormat);
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // 打印数据表格
            Bitmap dataGridViewBitmap = new Bitmap(resultDataGridView.Width, resultDataGridView.Height);
            resultDataGridView.DrawToBitmap(dataGridViewBitmap, new Rectangle(0, 0, resultDataGridView.Width, resultDataGridView.Height));
            e.Graphics.DrawImage(dataGridViewBitmap, 10, 10);

            // 打印误差图表
            Bitmap chartBitmap = new Bitmap(errorChart.Width, errorChart.Height);
            errorChart.DrawToBitmap(chartBitmap, new Rectangle(0, 0, errorChart.Width, errorChart.Height));
            e.Graphics.DrawImage(chartBitmap, 10, resultDataGridView.Height + 20);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (resultDataGridView.Rows.Count > 0)
            {
                string data = "";
                // 添加表头
                for (int i = 0; i < resultDataGridView.Columns.Count; i++)
                {
                    data += resultDataGridView.Columns[i].HeaderText + "\t";
                }
                data += Environment.NewLine;

                // 添加数据行
                for (int i = 0; i < resultDataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < resultDataGridView.Columns.Count; j++)
                    {
                        data += resultDataGridView.Rows[i].Cells[j].Value + "\t";
                    }
                    data += Environment.NewLine;
                }

                Clipboard.SetText(data);
                MessageBox.Show("数据已复制到剪贴板。");
            }
        }
    }
}