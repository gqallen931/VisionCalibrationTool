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

            // 配置 Halcon 环境
            string halconRoot = @"D:\Halcon-24.05.0.0\Program Files\LOCALAPPDATA\Programs\MVTec\HALCON-24.05-Progress";
            Environment.SetEnvironmentVariable("HALCONROOT", halconRoot);
            Environment.SetEnvironmentVariable("HALCONARCH", "x64-win64");

            // 初始化 Halcon 运行时
            HOperatorSet.SetSystem("use_window_thread", "true");
            HOperatorSet.SetSystem("clip_region", "false");
        }
    }
}