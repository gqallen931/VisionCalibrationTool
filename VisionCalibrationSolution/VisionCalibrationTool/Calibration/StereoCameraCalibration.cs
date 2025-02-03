using HalconDotNet;
using System;
using System.Collections.Generic;

namespace VisionCalibrationProject.Calibration
{
    public class StereoCameraCalibration
    {
        private SingleCameraCalibration singleCalibration;

        public StereoCameraCalibration()
        {
            singleCalibration = new SingleCameraCalibration();
        }

        /// <summary>
        /// 双目标定方法
        /// </summary>
        /// <param name="leftCalibrationImages">左相机的标定图像列表</param>
        /// <param name="rightCalibrationImages">右相机的标定图像列表</param>
        /// <param name="calibrationBoardModel">标定板模型</param>
        /// <param name="calibrationBoardSize">标定板尺寸（例如棋盘格的行数和列数）</param>
        /// <param name="leftCameraParams">输出的左相机内参</param>
        /// <param name="rightCameraParams">输出的右相机内参</param>
        /// <param name="relativePoseParams">输出的左右相机相对位姿参数</param>
        /// <param name="leftDistortionParams">输出的左相机畸变系数</param>
        /// <param name="rightDistortionParams">输出的右相机畸变系数</param>
        public void Calibrate(List<HImage> leftCalibrationImages, List<HImage> rightCalibrationImages,
            HTuple calibrationBoardModel, HTuple calibrationBoardSize,
            out HTuple leftCameraParams, out HTuple rightCameraParams, out HTuple relativePoseParams,
            out HTuple leftDistortionParams, out HTuple rightDistortionParams)
        {
            if (leftCalibrationImages == null || leftCalibrationImages.Count == 0 ||
                rightCalibrationImages == null || rightCalibrationImages.Count == 0)
            {
                throw new ArgumentException("左右相机的标定图像列表不能为空。");
            }

            if (leftCalibrationImages.Count != rightCalibrationImages.Count)
            {
                throw new ArgumentException("左右相机的标定图像数量必须一致。");
            }

            List<HTuple> leftPoseParams;
            List<HTuple> rightPoseParams;

            // 分别对左右相机进行单目标定
            singleCalibration.Calibrate(leftCalibrationImages, calibrationBoardModel, calibrationBoardSize,
                out leftCameraParams, out leftPoseParams, out leftDistortionParams);

            singleCalibration.Calibrate(rightCalibrationImages, calibrationBoardModel, calibrationBoardSize,
                out rightCameraParams, out rightPoseParams, out rightDistortionParams);

            HCalibData stereoCalibData = new HCalibData();
            stereoCalibData.CreateCalibData("stereo", 2, 1);

            // 添加左右相机的内参
            stereoCalibData.AddCalibData("camera", 0, "", leftCameraParams);
            stereoCalibData.AddCalibData("camera", 1, "", rightCameraParams);

            // 添加左右相机的标定图像和位姿信息
            for (int i = 0; i < leftCalibrationImages.Count; i++)
            {
                stereoCalibData.AddCalibData("image", 0, i, leftPoseParams[i], leftCalibrationImages[i]);
                stereoCalibData.AddCalibData("image", 1, i, rightPoseParams[i], rightCalibrationImages[i]);
            }

            HTuple error;
            // 进行双目标定，计算相对位姿参数
            HOperatorSet.CalibrateStereoSystem(stereoCalibData, out relativePoseParams, out error);
        }
    }
}