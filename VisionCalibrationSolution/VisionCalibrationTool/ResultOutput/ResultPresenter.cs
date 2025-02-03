using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HalconDotNet;

namespace VisionCalibrationProject.ResultPresentation
{
    public class ResultPresenter
    {
        private DataTable resultTable;
        private Chart errorChart;
        private Form resultForm;

        public ResultPresenter()
        {
            resultTable = new DataTable();
            errorChart = new Chart();
            resultForm = new Form();
            resultForm.Text = "标定结果展示";
            resultForm.Size = new Size(800, 600);

            // 初始化表格
            InitializeResultTable();
            // 初始化图表
            InitializeErrorChart();

            // 添加表格和图表到窗体
            resultForm.Controls.Add(CreateTablePanel());
            resultForm.Controls.Add(CreateChartPanel());

            // 添加打印和复制按钮
            AddPrintAndCopyButtons();
        }

        private void InitializeResultTable()
        {
            resultTable.Columns.Add("参数名称", typeof(string));
            resultTable.Columns.Add("参数值", typeof(string));
        }

        private void InitializeErrorChart()
        {
            errorChart.Dock = DockStyle.Fill;
            ChartArea chartArea = new ChartArea();
            errorChart.ChartAreas.Add(chartArea);
            Series errorSeries = new Series();
            errorSeries.ChartType = SeriesChartType.Bar;
            errorChart.Series.Add(errorSeries);
        }

        private Panel CreateTablePanel()
        {
            Panel tablePanel = new Panel();
            tablePanel.Dock = DockStyle.Top;
            tablePanel.Height = 200;

            DataGridView dataGridView = new DataGridView();
            dataGridView.DataSource = resultTable;
            dataGridView.Dock = DockStyle.Fill;
            tablePanel.Controls.Add(dataGridView);

            return tablePanel;
        }

        private Panel CreateChartPanel()
        {
            Panel chartPanel = new Panel();
            chartPanel.Dock = DockStyle.Fill;
            chartPanel.Controls.Add(errorChart);

            return chartPanel;
        }

        private void AddPrintAndCopyButtons()
        {
            Button printButton = new Button();
            printButton.Text = "打印结果";
            printButton.Location = new Point(20, resultForm.Height - 50);
            printButton.Click += PrintButton_Click;
            resultForm.Controls.Add(printButton);

            Button copyButton = new Button();
            copyButton.Text = "复制数据";
            copyButton.Location = new Point(150, resultForm.Height - 50);
            copyButton.Click += CopyButton_Click;
            resultForm.Controls.Add(copyButton);
        }

        public void PresentSingleCalibrationResult(HTuple cameraParams, HTuple poseParams, HTuple distortionParams)
        {
            AddParameterToTable("相机内参", cameraParams);
            AddParameterToTable("相机位姿参数", poseParams);
            AddParameterToTable("畸变系数", distortionParams);

            // 这里假设简单的误差数据，实际应根据具体计算
            double[] errorData = { 0.1, 0.2, 0.3 };
            AddErrorDataToChart(errorData);

            resultForm.ShowDialog();
        }

        public void PresentStereoCalibrationResult(HTuple leftCameraParams, HTuple rightCameraParams, HTuple relativePoseParams,
            HTuple leftDistortionParams, HTuple rightDistortionParams)
        {
            AddParameterToTable("左相机内参", leftCameraParams);
            AddParameterToTable("右相机内参", rightCameraParams);
            AddParameterToTable("相对位姿参数", relativePoseParams);
            AddParameterToTable("左相机畸变系数", leftDistortionParams);
            AddParameterToTable("右相机畸变系数", rightDistortionParams);

            // 这里假设简单的误差数据，实际应根据具体计算
            double[] errorData = { 0.15, 0.25, 0.35 };
            AddErrorDataToChart(errorData);

            resultForm.ShowDialog();
        }

        public void PresentMultiCalibrationResult(List<HTuple> cameraParamsList, List<HTuple> poseParamsList)
        {
            for (int i = 0; i < cameraParamsList.Count; i++)
            {
                AddParameterToTable($"相机 {i + 1} 内参", cameraParamsList[i]);
                AddParameterToTable($"相机 {i + 1} 位姿参数", poseParamsList[i]);
            }

            // 这里假设简单的误差数据，实际应根据具体计算
            double[] errorData = new double[cameraParamsList.Count];
            for (int i = 0; i < cameraParamsList.Count; i++)
            {
                errorData[i] = new Random().NextDouble(); // 随机生成误差数据示例
            }
            AddErrorDataToChart(errorData);

            resultForm.ShowDialog();
        }

        private void AddParameterToTable(string paramName, HTuple paramValue)
        {
            string valueString = "";
            for (int i = 0; i < paramValue.Length; i++)
            {
                valueString += paramValue[i].D.ToString() + " ";
            }
            resultTable.Rows.Add(paramName, valueString.Trim());
        }

        private void AddErrorDataToChart(double[] errorData)
        {
            Series errorSeries = errorChart.Series[0];
            errorSeries.Points.Clear();
            for (int i = 0; i < errorData.Length; i++)
            {
                errorSeries.Points.AddY(errorData[i]);
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = printDoc;
            previewDialog.ShowDialog();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // 打印表格
            Rectangle tableRect = new Rectangle(20, 20, resultForm.Width - 40, 200);
            Bitmap tableBitmap = new Bitmap(resultForm.Width - 40, 200);
            resultForm.Controls[0].DrawToBitmap(tableBitmap, new Rectangle(0, 0, resultForm.Width - 40, 200));
            e.Graphics.DrawImage(tableBitmap, tableRect);

            // 打印图表
            Rectangle chartRect = new Rectangle(20, 240, resultForm.Width - 40, resultForm.Height - 260);
            Bitmap chartBitmap = new Bitmap(resultForm.Width - 40, resultForm.Height - 260);
            resultForm.Controls[1].DrawToBitmap(chartBitmap, new Rectangle(0, 0, resultForm.Width - 40, resultForm.Height - 260));
            e.Graphics.DrawImage(chartBitmap, chartRect);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            string dataToCopy = "";
            foreach (DataRow row in resultTable.Rows)
            {
                dataToCopy += row["参数名称"] + "\t" + row["参数值"] + Environment.NewLine;
            }
            Clipboard.SetText(dataToCopy);
            MessageBox.Show("数据已复制到剪贴板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}