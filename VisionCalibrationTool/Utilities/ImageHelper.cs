using HalconDotNet;
using System.Windows.Media.Imaging;

namespace VisionCalibrationTool.Utilities
{
    /// <summary>
    /// 图像处理工具类
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// 将 Halcon 图像转换为 WPF BitmapSource
        /// </summary>
        public static BitmapSource ConvertHImageToBitmap(HImage image)
        {
            image.GetImagePointer1(out string type, out int width, out int height);
            IntPtr ptr = image.GetImagePointer1(out _, out _, out _);
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, ptr, width, width, 1);
        }

        /// <summary>
        /// 在指定窗口显示 Halcon 图像
        /// </summary>
        public static void DisplayImage(HImage image, string windowTitle)
        {
            HWindow window = new HWindow();
            window.OpenWindow(0, 0, 800, 600, 0, "visible", "");
            window.SetPart(0, 0, image.Height, image.Width);
            window.DispObj(image);
            window.SetWindowAttr("title", windowTitle);
        }
    }
}