using HalconDotNet;
using System;

namespace VisionCalibrationProject.ImageProcessing
{
    public class GeometricCorrection
    {
        /// <summary>
        /// 进行图像几何校正
        /// </summary>
        /// <param name="image">待校正的图像</param>
        /// <param name="cameraParams">相机内参</param>
        /// <param name="distortionParams">畸变系数</param>
        /// <returns>校正后的图像</returns>
        public HImage CorrectGeometry(HImage image, HTuple cameraParams, HTuple distortionParams)
        {
            if (image == null || cameraParams == null || distortionParams == null)
            {
                throw new ArgumentException("输入的图像、相机内参或畸变系数不能为空。");
            }

            try
            {
                // 创建畸变校正映射
                HTuple mapColumn, mapRow;
                HOperatorSet.GenDistortionMap(out mapColumn, out mapRow, cameraParams, distortionParams, "bilinear");

                // 应用畸变校正映射到图像上
                HImage correctedImage = new HImage();
                HOperatorSet.MapImage(image, correctedImage, mapColumn, mapRow, "constant", "false", "");

                return correctedImage;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"图像几何校正失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 获取相机畸变系数
        /// </summary>
        /// <param name="calibrationImages">标定图像列表</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <returns>畸变系数</returns>
        public HTuple GetDistortionCoefficients(List<HImage> calibrationImages, HTuple calibrationObjectModel)
        {
            if (calibrationImages == null || calibrationImages.Count == 0 || calibrationObjectModel == null)
            {
                throw new ArgumentException("输入的标定图像列表或标定板模型不能为空。");
            }

            try
            {
                HCalibData calibData = new HCalibData();
                calibData.CreateCalibData("calibration_object", 1, 1);

                foreach (HImage image in calibrationImages)
                {
                    HTuple pose;
                    HTuple numFound;
                    HTuple foundIndices;
                    HOperatorSet.FindCalibObject(image, calibrationObjectModel, out pose, out numFound, out foundIndices, 1, 1, 0, 1);

                    if (numFound.I > 0)
                    {
                        calibData.AddCalibData("image", 0, 0, pose, image);
                    }
                }

                HTuple cameraParams;
                HOperatorSet.CalibrateCamera("area_scan_division", calibrationObjectModel,
                    calibData.GetCalibData("image", 0, "pose"), calibData.GetCalibData("image", 0, "image"),
                    out cameraParams);

                // 从相机内参中提取畸变系数
                HTuple distortionParams = new HTuple(new double[]
                {
                    cameraParams[4].D, // k1
                    cameraParams[5].D, // k2
                    cameraParams[6].D, // p1
                    cameraParams[7].D, // p2
                    cameraParams[8].D  // k3
                });

                return distortionParams;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"获取相机畸变系数失败: {ex.Message}");
                return new HTuple();
            }
        }
    }
}