using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisionCalibrationProject.Forms
{
    public partial class CalibrationModeSelectionForm : Form
    {
        private ComboBox calibrationModeComboBox;
        private GroupBox singleCameraGroupBox;
        private GroupBox stereoCameraGroupBox;
        private TableLayoutPanel threeCameraLayoutPanel;
        private TableLayoutPanel fourCameraLayoutPanel;
        private TableLayoutPanel fiveCameraLayoutPanel;
        private TableLayoutPanel sixCameraLayoutPanel;
        private TableLayoutPanel sevenCameraLayoutPanel;
        private TableLayoutPanel eightCameraLayoutPanel;

        public CalibrationModeSelectionForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CalibrationModeSelectionForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "CalibrationModeSelectionForm";
            this.Text = "标定模式选择";
            this.ResumeLayout(false);

        }

        private void InitializeUI()
        {
            // 下拉菜单
            calibrationModeComboBox = new ComboBox();
            calibrationModeComboBox.Items.AddRange(new string[] { "单目", "双目", "三目", "四目", "五目", "六目", "七目", "八目" });
            calibrationModeComboBox.SelectedIndex = 0;
            calibrationModeComboBox.Location = new Point(20, 20);
            calibrationModeComboBox.Size = new Size(200, 23);
            calibrationModeComboBox.SelectedIndexChanged += CalibrationModeComboBox_SelectedIndexChanged;
            this.Controls.Add(calibrationModeComboBox);

            // 单目标定参数设置区域
            singleCameraGroupBox = new GroupBox("单目标定参数");
            singleCameraGroupBox.Location = new Point(20, 60);
            singleCameraGroupBox.Size = new Size(760, 100);

            Label singleCameraIntrinsicLabel = new Label();
            singleCameraIntrinsicLabel.Text = "相机内参初始值";
            singleCameraIntrinsicLabel.Location = new Point(20, 30);
            singleCameraGroupBox.Controls.Add(singleCameraIntrinsicLabel);

            TextBox singleCameraIntrinsicTextBox = new TextBox();
            singleCameraIntrinsicTextBox.Location = new Point(150, 30);
            singleCameraIntrinsicTextBox.Size = new Size(200, 23);
            singleCameraGroupBox.Controls.Add(singleCameraIntrinsicTextBox);

            this.Controls.Add(singleCameraGroupBox);

            // 双目标定参数设置区域
            stereoCameraGroupBox = new GroupBox("双目标定参数");
            stereoCameraGroupBox.Location = new Point(20, 60);
            stereoCameraGroupBox.Size = new Size(760, 100);
            stereoCameraGroupBox.Visible = false;

            GroupBox leftCameraGroup = new GroupBox("左相机参数");
            leftCameraGroup.Location = new Point(20, 20);
            leftCameraGroup.Size = new Size(350, 70);

            Label leftCameraIntrinsicLabel = new Label();
            leftCameraIntrinsicLabel.Text = "相机内参初始值";
            leftCameraIntrinsicLabel.Location = new Point(20, 30);
            leftCameraGroup.Controls.Add(leftCameraIntrinsicLabel);

            TextBox leftCameraIntrinsicTextBox = new TextBox();
            leftCameraIntrinsicTextBox.Location = new Point(150, 30);
            leftCameraIntrinsicTextBox.Size = new Size(200, 23);
            leftCameraGroup.Controls.Add(leftCameraIntrinsicTextBox);

            stereoCameraGroupBox.Controls.Add(leftCameraGroup);

            GroupBox rightCameraGroup = new GroupBox("右相机参数");
            rightCameraGroup.Location = new Point(400, 20);
            rightCameraGroup.Size = new Size(350, 70);

            Label rightCameraIntrinsicLabel = new Label();
            rightCameraIntrinsicLabel.Text = "相机内参初始值";
            rightCameraIntrinsicLabel.Location = new Point(20, 30);
            rightCameraGroup.Controls.Add(rightCameraIntrinsicLabel);

            TextBox rightCameraIntrinsicTextBox = new TextBox();
            rightCameraIntrinsicTextBox.Location = new Point(150, 30);
            rightCameraIntrinsicTextBox.Size = new Size(200, 23);
            rightCameraGroup.Controls.Add(rightCameraIntrinsicTextBox);

            stereoCameraGroupBox.Controls.Add(rightCameraGroup);

            this.Controls.Add(stereoCameraGroupBox);

            // 三目标定参数设置区域
            threeCameraLayoutPanel = new TableLayoutPanel();
            threeCameraLayoutPanel.Location = new Point(20, 60);
            threeCameraLayoutPanel.Size = new Size(760, 100);
            threeCameraLayoutPanel.Visible = false;
            threeCameraLayoutPanel.ColumnCount = 3;
            threeCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 3; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                threeCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                threeCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(threeCameraLayoutPanel);

            // 四目标定参数设置区域
            fourCameraLayoutPanel = new TableLayoutPanel();
            fourCameraLayoutPanel.Location = new Point(20, 60);
            fourCameraLayoutPanel.Size = new Size(760, 100);
            fourCameraLayoutPanel.Visible = false;
            fourCameraLayoutPanel.ColumnCount = 4;
            fourCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 4; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                fourCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                fourCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(fourCameraLayoutPanel);

            // 五目标定参数设置区域
            fiveCameraLayoutPanel = new TableLayoutPanel();
            fiveCameraLayoutPanel.Location = new Point(20, 60);
            fiveCameraLayoutPanel.Size = new Size(760, 100);
            fiveCameraLayoutPanel.Visible = false;
            fiveCameraLayoutPanel.ColumnCount = 5;
            fiveCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 5; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                fiveCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                fiveCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(fiveCameraLayoutPanel);

            // 六目标定参数设置区域
            sixCameraLayoutPanel = new TableLayoutPanel();
            sixCameraLayoutPanel.Location = new Point(20, 60);
            sixCameraLayoutPanel.Size = new Size(760, 100);
            sixCameraLayoutPanel.Visible = false;
            sixCameraLayoutPanel.ColumnCount = 6;
            sixCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 6; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                sixCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                sixCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(sixCameraLayoutPanel);

            // 七目标定参数设置区域
            sevenCameraLayoutPanel = new TableLayoutPanel();
            sevenCameraLayoutPanel.Location = new Point(20, 60);
            sevenCameraLayoutPanel.Size = new Size(760, 100);
            sevenCameraLayoutPanel.Visible = false;
            sevenCameraLayoutPanel.ColumnCount = 7;
            sevenCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 7; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                sevenCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                sevenCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(sevenCameraLayoutPanel);

            // 八目标定参数设置区域
            eightCameraLayoutPanel = new TableLayoutPanel();
            eightCameraLayoutPanel.Location = new Point(20, 60);
            eightCameraLayoutPanel.Size = new Size(760, 100);
            eightCameraLayoutPanel.Visible = false;
            eightCameraLayoutPanel.ColumnCount = 8;
            eightCameraLayoutPanel.RowCount = 2;
            for (int i = 0; i < 8; i++)
            {
                Label cameraIntrinsicLabel = new Label();
                cameraIntrinsicLabel.Text = $"相机{i + 1}内参";
                eightCameraLayoutPanel.Controls.Add(cameraIntrinsicLabel, i, 0);

                TextBox cameraIntrinsicTextBox = new TextBox();
                eightCameraLayoutPanel.Controls.Add(cameraIntrinsicTextBox, i, 1);
            }
            this.Controls.Add(eightCameraLayoutPanel);
        }

        private void CalibrationModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            singleCameraGroupBox.Visible = false;
            stereoCameraGroupBox.Visible = false;
            threeCameraLayoutPanel.Visible = false;
            fourCameraLayoutPanel.Visible = false;
            fiveCameraLayoutPanel.Visible = false;
            sixCameraLayoutPanel.Visible = false;
            sevenCameraLayoutPanel.Visible = false;
            eightCameraLayoutPanel.Visible = false;

            switch (calibrationModeComboBox.SelectedIndex)
            {
                case 0:
                    singleCameraGroupBox.Visible = true;
                    break;
                case 1:
                    stereoCameraGroupBox.Visible = true;
                    break;
                case 2:
                    threeCameraLayoutPanel.Visible = true;
                    break;
                case 3:
                    fourCameraLayoutPanel.Visible = true;
                    break;
                case 4:
                    fiveCameraLayoutPanel.Visible = true;
                    break;
                case 5:
                    sixCameraLayoutPanel.Visible = true;
                    break;
                case 6:
                    sevenCameraLayoutPanel.Visible = true;
                    break;
                case 7:
                    eightCameraLayoutPanel.Visible = true;
                    break;
            }
        }
    }
}