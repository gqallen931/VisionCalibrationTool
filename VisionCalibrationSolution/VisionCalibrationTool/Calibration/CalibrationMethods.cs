using HalconDotNet;
using System;
using System.Collections.Generic;

namespace VisionCalibrationProject.Calibration
{
    public class CalibrationMethods
    {
        /// <summary>
        /// 单目标定方法，采用张氏标定法
        /// </summary>
        /// <param name="calibrationImages">标定图像列表</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <param name="calibrationBoardSize">标定板尺寸</param>
        /// <param name="cameraParams">输出的相机内参</param>
        /// <param name="poseParams">输出的相机外参</param>
        /// <param name="distortionParams">输出的畸变系数</param>
        public void SingleCameraCalibration(List<HImage> calibrationImages, HTuple calibrationObjectModel,
            HTuple calibrationBoardSize, out HTuple cameraParams, out HTuple poseParams, out HTuple distortionParams)
        {
            HCalibData calibData = new HCalibData();
            calibData.CreateCalibData("calibration_object", 1, 1);

            foreach (HImage image in calibrationImages)
            {
                HTuple pose;
                HTuple numFound;
                HTuple foundIndices;
                HOperatorSet.FindCalibObject(image, calibrationObjectModel, out pose, out numFound, out foundIndices, 1, 1, 0, 1);
                calibData.AddCalibData("image", 0, 0, pose, image);
            }

            HOperatorSet.CalibrateCamera("area_scan_division", calibrationObjectModel,
                calibData.GetCalibData("image", 0, "pose"), calibData.GetCalibData("image", 0, "image"),
                out cameraParams);

            poseParams = calibData.GetCalibData("image", 0, 0, "pose");
            // 从相机内参中提取畸变系数，这里假设畸变系数存储在特定位置
            distortionParams = new HTuple(new double[] { cameraParams[4].D, cameraParams[5].D, cameraParams[6].D, cameraParams[7].D, cameraParams[8].D });
        }

        /// <summary>
        /// 双目标定方法，基于双目立体视觉原理
        /// </summary>
        /// <param name="leftCalibrationImages">左相机标定图像列表</param>
        /// <param name="rightCalibrationImages">右相机标定图像列表</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <param name="leftCameraParams">输出的左相机内参</param>
        /// <param name="rightCameraParams">输出的右相机内参</param>
        /// <param name="relativePoseParams">输出的左右相机相对位姿参数</param>
        public void StereoCameraCalibration(List<HImage> leftCalibrationImages, List<HImage> rightCalibrationImages,
            HTuple calibrationObjectModel, out HTuple leftCameraParams, out HTuple rightCameraParams, out HTuple relativePoseParams)
        {
            // 分别对左右相机进行单目标定
            HTuple leftPoseParams, leftDistortionParams;
            SingleCameraCalibration(leftCalibrationImages, calibrationObjectModel, new HTuple(), out leftCameraParams, out leftPoseParams, out leftDistortionParams);

            HTuple rightPoseParams, rightDistortionParams;
            SingleCameraCalibration(rightCalibrationImages, calibrationObjectModel, new HTuple(), out rightCameraParams, out rightPoseParams, out rightDistortionParams);

            HCalibData stereoCalibData = new HCalibData();
            stereoCalibData.CreateCalibData("stereo", 2, 1);

            stereoCalibData.AddCalibData("camera", 0, "", leftCameraParams);
            stereoCalibData.AddCalibData("camera", 1, "", rightCameraParams);

            for (int i = 0; i < leftCalibrationImages.Count; i++)
            {
                stereoCalibData.AddCalibData("image", 0, i, leftPoseParams, leftCalibrationImages[i]);
                stereoCalibData.AddCalibData("image", 1, i, rightPoseParams, rightCalibrationImages[i]);
            }

            HTuple error;
            HOperatorSet.CalibrateStereoSystem(stereoCalibData, out relativePoseParams, out error);
        }

        /// <summary>
        /// 多目标定方法（三目及以上）
        /// </summary>
        /// <param name="calibrationImagesList">每个相机的标定图像列表</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <param name="cameraParamsList">输出的每个相机的内参列表</param>
        /// <param name="poseParamsList">输出的每个相机的位姿参数列表</param>
        public void MultiCameraCalibration(List<List<HImage>> calibrationImagesList, HTuple calibrationObjectModel,
            out List<HTuple> cameraParamsList, out List<HTuple> poseParamsList)
        {
            int cameraCount = calibrationImagesList.Count;
            cameraParamsList = new List<HTuple>();
            poseParamsList = new List<HTuple>();

            HCalibData multiCalibData = new HCalibData();
            multiCalibData.CreateCalibData("calibration_object", cameraCount, 1);

            // 单目标定获取内参
            for (int i = 0; i < cameraCount; i++)
            {
                HTuple cameraParams, poseParams, distortionParams;
                SingleCameraCalibration(calibrationImagesList[i], calibrationObjectModel, new HTuple(), out cameraParams, out poseParams, out distortionParams);
                cameraParamsList.Add(cameraParams);
                multiCalibData.AddCalibData("camera", i, "", cameraParams);

                foreach (HImage image in calibrationImagesList[i])
                {
                    HTuple currentPose;
                    HTuple numFound;
                    HTuple foundIndices;
                    HOperatorSet.FindCalibObject(image, calibrationObjectModel, out currentPose, out numFound, out foundIndices, 1, 1, 0, 1);
                    multiCalibData.AddCalibData("image", i, 0, currentPose, image);
                }
            }

            // 进行多相机标定
            HTuple error;
            HOperatorSet.CalibrateMultiCameraSystem(multiCalibData, out error);

            // 获取每个相机的最终位姿
            for (int i = 0; i < cameraCount; i++)
            {
                HTuple poseParams = multiCalibData.GetCalibData("camera", i, "", "pose");
                poseParamsList.Add(poseParams);
            }
        }
    }
}