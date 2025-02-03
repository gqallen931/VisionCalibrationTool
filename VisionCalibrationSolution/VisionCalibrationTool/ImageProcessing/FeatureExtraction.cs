using HalconDotNet;
using System;

namespace VisionCalibrationProject.ImageProcessing
{
    public class FeatureExtraction
    {
        /// <summary>
        /// 角点提取方法，用于标定板图像的角点提取
        /// </summary>
        /// <param name="image">输入的标定板图像</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <param name="corners">输出的角点坐标</param>
        /// <param name="numFound">找到的角点数量</param>
        /// <param name="foundIndices">找到的角点索引</param>
        public void ExtractCorners(HImage image, HTuple calibrationObjectModel, out HTuple corners, out HTuple numFound, out HTuple foundIndices)
        {
            if (image == null || calibrationObjectModel == null)
            {
                throw new ArgumentException("输入的图像或标定板模型不能为空。");
            }

            try
            {
                HTuple pose;
                // 使用 Halcon 的 FindCalibObject 算子提取棋盘格角点
                HOperatorSet.FindCalibObject(image, calibrationObjectModel, out pose, out numFound, out foundIndices, 1, 1, 0, 1);

                // 这里可以根据实际需求处理角点信息，目前简单将 pose 作为角点坐标返回
                corners = pose;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"角点提取失败: {ex.Message}");
                corners = new HTuple();
                numFound = new HTuple();
                foundIndices = new HTuple();
            }
        }

        /// <summary>
        /// 边缘提取方法，使用 Canny 边缘检测算法
        /// </summary>
        /// <param name="image">输入的图像</param>
        /// <param name="edges">输出的边缘图像</param>
        /// <param name="lowThreshold">Canny 边缘检测的低阈值</param>
        /// <param name="highThreshold">Canny 边缘检测的高阈值</param>
        public void ExtractEdges(HImage image, out HImage edges, HTuple lowThreshold, HTuple highThreshold)
        {
            if (image == null)
            {
                throw new ArgumentException("输入的图像不能为空。");
            }

            try
            {
                edges = new HImage();
                // 使用 Halcon 的 EdgesSubPix 算子进行边缘提取
                HOperatorSet.EdgesSubPix(image, edges, "canny", 1, lowThreshold, highThreshold);
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"边缘提取失败: {ex.Message}");
                edges = null;
            }
        }
    }
}