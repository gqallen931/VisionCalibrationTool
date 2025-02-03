using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisionCalibrationProject.Calibration
{
    public class MultiCameraCalibration
    {
        private int cameraCount;
        private List<HCalibData> calibDataList;
        private List<HTuple> cameraParamsList;
        private List<HTuple> poseParamsList;

        public MultiCameraCalibration(int cameraCount)
        {
            this.cameraCount = cameraCount;
            calibDataList = new List<HCalibData>();
            cameraParamsList = new List<HTuple>();
            poseParamsList = new List<HTuple>();

            // 初始化每个相机的标定数据
            for (int i = 0; i < cameraCount; i++)
            {
                HCalibData calibData = new HCalibData();
                calibData.CreateCalibData("calibration_object", 1, 1);
                calibDataList.Add(calibData);
            }
        }

        // 添加标定图像
        public void AddCalibrationImage(int cameraIndex, HImage image, HTuple calibrationObjectModel)
        {
            if (cameraIndex < 0 || cameraIndex >= cameraCount)
            {
                throw new ArgumentOutOfRangeException(nameof(cameraIndex), "相机索引超出范围");
            }

            HCalibData calibData = calibDataList[cameraIndex];
            HTuple pose;
            HTuple numFound;
            HTuple foundIndices;

            // 查找标定板角点
            HOperatorSet.FindCalibObject(image, calibrationObjectModel, out pose, out numFound, out foundIndices, 1, 1, 0, 1);

            // 添加标定数据
            calibData.AddCalibData("image", cameraIndex, 0, pose, image);
        }

        // 单目标定获取内参
        public void SingleCameraCalibration()
        {
            for (int i = 0; i < cameraCount; i++)
            {
                HCalibData calibData = calibDataList[i];
                HTuple cameraParams;

                // 进行单目标定
                HOperatorSet.CalibrateCamera("area_scan_division", calibData.GetCalibData("calib_object", 0, "model"),
                    calibData.GetCalibData("image", i, "pose"), calibData.GetCalibData("image", i, "image"),
                    out cameraParams);

                cameraParamsList.Add(cameraParams);
            }
        }

        // 多目标定
        public void MultiCameraCalibrationProcess()
        {
            // 单目标定获取内参
            SingleCameraCalibration();

            // 创建多相机标定数据
            HCalibData multiCalibData = new HCalibData();
            multiCalibData.CreateCalibData("calibration_object", cameraCount, 1);

            // 添加每个相机的内参
            for (int i = 0; i < cameraCount; i++)
            {
                multiCalibData.AddCalibData("camera", i, "", cameraParamsList[i]);
            }

            // 添加每个相机的标定图像和位姿
            for (int i = 0; i < cameraCount; i++)
            {
                HCalibData singleCalibData = calibDataList[i];
                int imageCount = singleCalibData.GetCalibDataInt("image", i, "count");
                for (int j = 0; j < imageCount; j++)
                {
                    HTuple pose = singleCalibData.GetCalibData("image", i, j, "pose");
                    HImage image = singleCalibData.GetCalibDataImage("image", i, j);
                    multiCalibData.AddCalibData("image", i, j, pose, image);
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

        // 获取相机内参
        public List<HTuple> GetCameraParams()
        {
            return cameraParamsList;
        }

        // 获取相机位姿参数
        public List<HTuple> GetPoseParams()
        {
            return poseParamsList;
        }
    }
}