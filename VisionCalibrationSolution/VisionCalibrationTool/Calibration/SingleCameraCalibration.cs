using HalconDotNet;
using System;
using System.Collections.Generic;

namespace VisionCalibrationProject.Calibration
{
    public class SingleCameraCalibration
    {
        /// <summary>
        /// 单目标定方法
        /// </summary>
        /// <param name="calibrationImages">标定图像列表</param>
        /// <param name="calibrationBoardModel">标定板模型</param>
        /// <param name="calibrationBoardSize">标定板尺寸（例如棋盘格的行数和列数）</param>
        /// <param name="cameraParams">输出的相机内参</param>
        /// <param name="poseParams">输出的相机外参（位姿参数）</param>
        /// <param name="distortionParams">输出的畸变系数</param>
        public void Calibrate(List<HImage> calibrationImages, HTuple calibrationBoardModel, HTuple calibrationBoardSize,
            out HTuple cameraParams, out List<HTuple> poseParams, out HTuple distortionParams)
        {
            if (calibrationImages == null || calibrationImages.Count == 0)
            {
                throw new ArgumentException("标定图像列表不能为空。");
            }

            HCalibData calibData = new HCalibData();
            calibData.CreateCalibData("calibration_object", 1, 1);

            poseParams = new List<HTuple>();

            // 遍历每一张标定图像，提取角点信息
            foreach (HImage image in calibrationImages)
            {
                HTuple pose;
                HTuple numFound;
                HTuple foundIndices;

                // 查找标定板角点
                HOperatorSet.FindCalibObject(image, calibrationBoardModel, out pose, out numFound, out foundIndices, 1, 1, 0, 1);

                if (numFound.I > 0)
                {
                    // 将找到角点的图像和位姿信息添加到标定数据中
                    calibData.AddCalibData("image", 0, 0, pose, image);
                    poseParams.Add(pose);
                }
            }

            if (poseParams.Count == 0)
            {
                throw new Exception("在所有标定图像中均未找到有效的标定板角点。");
            }

            // 进行相机标定，计算相机内参
            HOperatorSet.CalibrateCamera("area_scan_division", calibrationBoardModel,
                calibData.GetCalibData("image", 0, "pose"), calibData.GetCalibData("image", 0, "image"),
                out cameraParams);

            // 从相机内参中提取畸变系数
            distortionParams = new HTuple(new double[]
            {
                cameraParams[4].D, // k1
                cameraParams[5].D, // k2
                cameraParams[6].D, // p1
                cameraParams[7].D, // p2
                cameraParams[8].D  // k3
            });
        }
    }
}