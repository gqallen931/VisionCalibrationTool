using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using HalconDotNet;

namespace VisionCalibrationProject.Forms
{
    public partial class MainForm : Form
    {
        private HImage currentImage;
        private HWindowControl hWindowControl;
        private ImageAcquisition.ImageAcquisition imageAcquisition;
        private Thread imageDisplayThread;
        private bool isAcquiring = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
            imageAcquisition = new ImageAcquisition.ImageAcquisition();
        }

        private void InitializeUI()
        {
            // 菜单栏
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("文件");
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("打开");
            openMenuItem.Click += OpenMenuItem_Click;
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("保存");
            saveMenuItem.Click += SaveMenuItem_Click;
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += ExitMenuItem_Click;
            fileMenu.DropDownItems.Add(openMenuItem);
            fileMenu.DropDownItems.Add(saveMenuItem);
            fileMenu.DropDownItems.Add(exitMenuItem);

            ToolStripMenuItem editMenu = new ToolStripMenuItem("编辑");
            ToolStripMenuItem modifyParamsMenuItem = new ToolStripMenuItem("参数修改");
            modifyParamsMenuItem.Click += ModifyParamsMenuItem_Click;
            ToolStripMenuItem resetCalibrationMenuItem = new ToolStripMenuItem("重置标定流程");
            resetCalibrationMenuItem.Click += ResetCalibrationMenuItem_Click;
            editMenu.DropDownItems.Add(modifyParamsMenuItem);
            editMenu.DropDownItems.Add(resetCalibrationMenuItem);

            ToolStripMenuItem calibrationMenu = new ToolStripMenuItem("标定");
            ToolStripMenuItem selectCalibrationModeMenuItem = new ToolStripMenuItem("选择标定目数");
            selectCalibrationModeMenuItem.Click += SelectCalibrationModeMenuItem_Click;
            ToolStripMenuItem startCalibrationMenuItem = new ToolStripMenuItem("开始标定");
            startCalibrationMenuItem.Click += StartCalibrationMenuItem_Click;
            calibrationMenu.DropDownItems.Add(selectCalibrationModeMenuItem);
            calibrationMenu.DropDownItems.Add(startCalibrationMenuItem);

            ToolStripMenuItem viewMenu = new ToolStripMenuItem("视图");
            ToolStripMenuItem showImageAreaMenuItem = new ToolStripMenuItem("显示图像区");
            showImageAreaMenuItem.Click += ShowImageAreaMenuItem_Click;
            ToolStripMenuItem showParamsAreaMenuItem = new ToolStripMenuItem("显示参数区");
            showParamsAreaMenuItem.Click += ShowParamsAreaMenuItem_Click;
            ToolStripMenuItem showResultAreaMenuItem = new ToolStripMenuItem("显示结果区");
            showResultAreaMenuItem.Click += ShowResultAreaMenuItem_Click;
            viewMenu.DropDownItems.Add(showImageAreaMenuItem);
            viewMenu.DropDownItems.Add(showParamsAreaMenuItem);
            viewMenu.DropDownItems.Add(showResultAreaMenuItem);

            ToolStripMenuItem helpMenu = new ToolStripMenuItem("帮助");
            ToolStripMenuItem usageInstructionsMenuItem = new ToolStripMenuItem("使用说明");
            usageInstructionsMenuItem.Click += UsageInstructionsMenuItem_Click;
            ToolStripMenuItem versionInfoMenuItem = new ToolStripMenuItem("版本信息");
            versionInfoMenuItem.Click += VersionInfoMenuItem_Click;
            helpMenu.DropDownItems.Add(usageInstructionsMenuItem);
            helpMenu.DropDownItems.Add(versionInfoMenuItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            menuStrip.Items.Add(calibrationMenu);
            menuStrip.Items.Add(viewMenu);
            menuStrip.Items.Add(helpMenu);
            this.MainMenuStrip = menuStrip;

            // 工具栏
            ToolStrip toolStrip = new ToolStrip();
            ToolStripButton realTimeAcquisitionButton = new ToolStripButton("实时采集");
            realTimeAcquisitionButton.Click += RealTimeAcquisitionButton_Click;
            ToolStripButton batchAcquisitionButton = new ToolStripButton("批量采集");
            batchAcquisitionButton.Click += BatchAcquisitionButton_Click;
            ToolStripButton startCalibrationToolButton = new ToolStripButton("开始标定");
            startCalibrationToolButton.Click += StartCalibrationToolButton_Click;
            ToolStripButton stopCalibrationToolButton = new ToolStripButton("停止标定");
            stopCalibrationToolButton.Click += StopCalibrationToolButton_Click;
            toolStrip.Items.Add(realTimeAcquisitionButton);
            toolStrip.Items.Add(batchAcquisitionButton);
            toolStrip.Items.Add(startCalibrationToolButton);
            toolStrip.Items.Add(stopCalibrationToolButton);
            this.Controls.Add(toolStrip);

            // 图像显示区
            hWindowControl = new HWindowControl();
            hWindowControl.Dock = DockStyle.Fill;
            Panel imagePanel = new Panel();
            imagePanel.Dock = DockStyle.Left;
            imagePanel.Width = this.Width / 2;
            imagePanel.Controls.Add(hWindowControl);
            this.Controls.Add(imagePanel);

            // 参数设置区
            GroupBox paramGroupBox = new GroupBox("参数设置");
            paramGroupBox.Dock = DockStyle.Top;
            paramGroupBox.Height = this.Height / 2;
            // 相机参数设置
            Label exposureLabel = new Label();
            exposureLabel.Text = "曝光时间";
            NumericUpDown exposureNumericUpDown = new NumericUpDown();
            exposureNumericUpDown.Minimum = 0;
            exposureNumericUpDown.Maximum = 1000;
            exposureNumericUpDown.Value = 50;
            Label gainLabel = new Label();
            gainLabel.Text = "增益";
            NumericUpDown gainNumericUpDown = new NumericUpDown();
            gainNumericUpDown.Minimum = 0;
            gainNumericUpDown.Maximum = 100;
            gainNumericUpDown.Value = 20;
            // 图像处理参数设置
            Label filterLabel = new Label();
            filterLabel.Text = "滤波方式";
            ComboBox filterComboBox = new ComboBox();
            filterComboBox.Items.Add("均值滤波");
            filterComboBox.Items.Add("高斯滤波");
            filterComboBox.Items.Add("中值滤波");
            filterComboBox.SelectedIndex = 0;
            Label thresholdLabel = new Label();
            thresholdLabel.Text = "阈值";
            NumericUpDown thresholdNumericUpDown = new NumericUpDown();
            thresholdNumericUpDown.Minimum = 0;
            thresholdNumericUpDown.Maximum = 255;
            thresholdNumericUpDown.Value = 128;
            // 标定相关参数设置
            Label calibrationBoardSizeLabel = new Label();
            calibrationBoardSizeLabel.Text = "标定板尺寸";
            TextBox calibrationBoardSizeTextBox = new TextBox();
            calibrationBoardSizeTextBox.Text = "100x100";
            Label chessboardCountLabel = new Label();
            chessboardCountLabel.Text = "棋盘格数量";
            NumericUpDown chessboardCountNumericUpDown = new NumericUpDown();
            chessboardCountNumericUpDown.Minimum = 1;
            chessboardCountNumericUpDown.Maximum = 100;
            chessboardCountNumericUpDown.Value = 8;

            paramGroupBox.Controls.Add(exposureLabel);
            paramGroupBox.Controls.Add(exposureNumericUpDown);
            paramGroupBox.Controls.Add(gainLabel);
            paramGroupBox.Controls.Add(gainNumericUpDown);
            paramGroupBox.Controls.Add(filterLabel);
            paramGroupBox.Controls.Add(filterComboBox);
            paramGroupBox.Controls.Add(thresholdLabel);
            paramGroupBox.Controls.Add(thresholdNumericUpDown);
            paramGroupBox.Controls.Add(calibrationBoardSizeLabel);
            paramGroupBox.Controls.Add(calibrationBoardSizeTextBox);
            paramGroupBox.Controls.Add(chessboardCountLabel);
            paramGroupBox.Controls.Add(chessboardCountNumericUpDown);
            Panel paramPanel = new Panel();
            paramPanel.Dock = DockStyle.Right;
            paramPanel.Width = this.Width / 2;
            paramPanel.Controls.Add(paramGroupBox);
            this.Controls.Add(paramPanel);

            // 结果展示区
            GroupBox resultGroupBox = new GroupBox("结果展示");
            resultGroupBox.Dock = DockStyle.Bottom;
            resultGroupBox.Height = this.Height / 2;
            Panel resultPanel = new Panel();
            resultPanel.Dock = DockStyle.Fill;
            resultGroupBox.Controls.Add(resultPanel);
            this.Controls.Add(resultGroupBox);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件 (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentImage = new HImage();
                currentImage.ReadImage(openFileDialog.FileName);
                DisplayImage(currentImage);
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG图像 (*.jpg)|*.jpg";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentImage.WriteImage("jpeg", 0, saveFileDialog.FileName);
                }
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ModifyParamsMenuItem_Click(object sender, EventArgs e)
        {
            // 弹出新的参数设置窗口，这里简单提示
            MessageBox.Show("参数修改功能待实现");
        }

        private void ResetCalibrationMenuItem_Click(object sender, EventArgs e)
        {
            // 将所有参数恢复为默认值并清空已采集图像和中间结果
            MessageBox.Show("重置标定流程功能待实现");
        }

        private void SelectCalibrationModeMenuItem_Click(object sender, EventArgs e)
        {
            CalibrationModeSelectionForm calibrationModeSelectionForm = new CalibrationModeSelectionForm();
            calibrationModeSelectionForm.ShowDialog();
        }

        private void StartCalibrationMenuItem_Click(object sender, EventArgs e)
        {
            // 触发标定功能模块的计算
            MessageBox.Show("开始标定功能待实现");
        }

        private void ShowImageAreaMenuItem_Click(object sender, EventArgs e)
        {
            hWindowControl.Visible = !hWindowControl.Visible;
        }

        private void ShowParamsAreaMenuItem_Click(object sender, EventArgs e)
        {
            // 假设参数设置区有一个容器控件 paramPanel
            // 这里需要根据实际的控件名修改
            // paramPanel.Visible = !paramPanel.Visible;
            MessageBox.Show("显示参数区功能待实现，需根据实际控件调整");
        }

        private void ShowResultAreaMenuItem_Click(object sender, EventArgs e)
        {
            // 假设结果展示区有一个容器控件 resultPanel
            // 这里需要根据实际的控件名修改
            // resultPanel.Visible = !resultPanel.Visible;
            MessageBox.Show("显示结果区功能待实现，需根据实际控件调整");
        }

        private void UsageInstructionsMenuItem_Click(object sender, EventArgs e)
        {
            // 显示预先编写好的 RichTextBox 控件内容
            MessageBox.Show("使用说明功能待实现");
        }

        private void VersionInfoMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("版本信息：1.0.0");
        }

        private void RealTimeAcquisitionButton_Click(object sender, EventArgs e)
        {
            if (!isAcquiring)
            {
                isAcquiring = true;
                imageDisplayThread = new Thread(RealTimeAcquisitionLoop);
                imageDisplayThread.Start();
            }
        }

        private void RealTimeAcquisitionLoop()
        {
            while (isAcquiring)
            {
                currentImage = imageAcquisition.GrabImage();
                if (currentImage != null)
                {
                    hWindowControl.Invoke(new Action(() =>
                    {
                        DisplayImage(currentImage);
                    }));
                }
                Thread.Sleep(100);
            }
        }

        private void BatchAcquisitionButton_Click(object sender, EventArgs e)
        {
            // 用户设定采集数量和存储路径
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG图像 (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string savePath = Path.GetDirectoryName(saveFileDialog.FileName);
                int acquisitionCount = 10; // 示例采集数量
                for (int i = 0; i < acquisitionCount; i++)
                {
                    currentImage = imageAcquisition.GrabImage();
                    if (currentImage != null)
                    {
                        string fileName = $"{DateTime.Now.ToString("yyyyMMdd")}_{i:D3}.jpg";
                        string fullPath = Path.Combine(savePath, fileName);
                        currentImage.WriteImage("jpeg", 0, fullPath);
                    }
                }
            }
        }

        private void StartCalibrationToolButton_Click(object sender, EventArgs e)
        {
            // 触发标定功能模块的计算
            MessageBox.Show("开始标定功能待实现");
        }

        private void StopCalibrationToolButton_Click(object sender, EventArgs e)
        {
            isAcquiring = false;
            if (imageDisplayThread != null && imageDisplayThread.IsAlive)
            {
                imageDisplayThread.Join();
            }
        }

        private void DisplayImage(HImage image)
        {
            if (hWindowControl.HalconWindow != null)
            {
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(0, 0, hWindowControl.Width, hWindowControl.Height, hWindowControl.HalconWindow, "visible", "", out _);
                HOperatorSet.DispObj(image, hWindowControl.HalconWindow);
            }
        }
    }
}