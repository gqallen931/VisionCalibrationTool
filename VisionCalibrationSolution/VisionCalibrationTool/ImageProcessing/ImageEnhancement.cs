using HalconDotNet;
using System;

namespace VisionCalibrationProject.ImageProcessing
{
    public class ImageEnhancement
    {
        /// <summary>
        /// 灰度拉伸
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="lowPercent">低百分比</param>
        /// <param name="highPercent">高百分比</param>
        /// <returns>增强后的图像</returns>
        public HImage GrayStretching(HImage inputImage, double lowPercent, double highPercent)
        {
            if (inputImage == null)
            {
                throw new ArgumentException("输入图像不能为空。");
            }

            try
            {
                HImage enhancedImage = new HImage();
                HTuple minGray, maxGray;
                // 计算图像的灰度最小值和最大值
                HOperatorSet.MinMaxGray(inputImage, new HTuple(), 0, out minGray, out maxGray, new HTuple());

                // 计算拉伸后的灰度范围
                HTuple lowGray = minGray + (maxGray - minGray) * lowPercent / 100;
                HTuple highGray = minGray + (maxGray - minGray) * highPercent / 100;

                // 进行灰度拉伸
                HOperatorSet.ChangeDomain(inputImage, enhancedImage, "full");
                HOperatorSet.HistogramEqualization(enhancedImage, enhancedImage, lowGray, highGray);

                return enhancedImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"灰度拉伸失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 直方图均衡化
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <returns>增强后的图像</returns>
        public HImage HistogramEqualization(HImage inputImage)
        {
            if (inputImage == null)
            {
                throw new ArgumentException("输入图像不能为空。");
            }

            try
            {
                HImage enhancedImage = new HImage();
                // 调用 Halcon 的直方图均衡化算子
                HOperatorSet.HistogramEqualization(inputImage, enhancedImage);

                return enhancedImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"直方图均衡化失败: {ex.Message}");
                return null;
            }
        }
    }
}