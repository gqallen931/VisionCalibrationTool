using System;
using System.Windows;
using HalconDotNet;

namespace VisionCalibrationTool
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ���� Halcon ����
            string halconRoot = @"D:\Halcon-24.05.0.0\Program Files\LOCALAPPDATA\Programs\MVTec\HALCON-24.05-Progress";
            Environment.SetEnvironmentVariable("HALCONROOT", halconRoot);
            Environment.SetEnvironmentVariable("HALCONARCH", "x64-win64");

            // ��ʼ�� Halcon ����ʱ
            HOperatorSet.SetSystem("use_window_thread", "true");
            HOperatorSet.SetSystem("clip_region", "false");
        }
    }
}