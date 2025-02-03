using HalconDotNet;
using System;

namespace VisionCalibrationProject.ImageProcessing
{
    public class ImageFiltering
    {
        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="maskSize">滤波核大小</param>
        /// <returns>滤波后的图像</returns>
        public HImage MeanFiltering(HImage inputImage, int maskSize)
        {
            if (inputImage == null)
            {
                throw new ArgumentException("输入图像不能为空。");
            }

            try
            {
                HImage filteredImage = new HImage();
                // 调用 Halcon 的均值滤波算子
                HOperatorSet.MeanFilter(inputImage, filteredImage, maskSize, maskSize);
                return filteredImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"均值滤波失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 高斯滤波
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="sigma">高斯核的标准差</param>
        /// <returns>滤波后的图像</returns>
        public HImage GaussianFiltering(HImage inputImage, double sigma)
        {
            if (inputImage == null)
            {
                throw new ArgumentException("输入图像不能为空。");
            }

            try
            {
                HImage filteredImage = new HImage();
                // 调用 Halcon 的高斯滤波算子
                HOperatorSet.GaussFilter(inputImage, filteredImage, sigma);
                return filteredImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"高斯滤波失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="maskSize">滤波核大小</param>
        /// <returns>滤波后的图像</returns>
        public HImage MedianFiltering(HImage inputImage, int maskSize)
        {
            if (inputImage == null)
            {
                throw new ArgumentException("输入图像不能为空。");
            }

            try
            {
                HImage filteredImage = new HImage();
                // 调用 Halcon 的中值滤波算子
                HOperatorSet.MedianFilter(inputImage, filteredImage, "circle", maskSize, "mirrored");
                return filteredImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"中值滤波失败: {ex.Message}");
                return null;
            }
        }
    }
}