using HalconDotNet;
using System;
using System.Collections.Generic;
using VisionCalibrationProject.Calibration;
using VisionCalibrationProject.ImageAcquisition;
using VisionCalibrationProject.ImageProcessing;
using VisionCalibrationProject.ResultOutput;
using VisionCalibrationProject.ResultPresentation;

namespace VisionCalibrationProject
{
    class Program
    {
        static void Main()
        {
            try
            {
                // 相机连接
                CameraConnection cameraConnection = new CameraConnection();
                bool isConnected = cameraConnection.ConnectCamera("GigEVision2", "YourCameraDeviceName");
                if (!isConnected)
                {
                    Console.WriteLine("相机连接失败，程序退出。");
                    return;
                }

                // 图像采集
                ImageAcquisition imageAcquisition = new ImageAcquisition();
                imageAcquisition.ConnectCamera("GigEVision2", "YourCameraDeviceName");

                // 假设采集 10 张图像用于标定
                int acquisitionCount = 10;
                string savePath = "CalibrationImages";
                List<HImage> calibrationImages = imageAcquisition.BatchAcquisition(acquisitionCount, savePath);

                // 特征提取
                FeatureExtraction featureExtraction = new FeatureExtraction();
                HTuple calibrationObjectModel = new HTuple(); // 实际中需要正确初始化
                HTuple corners, numFound, foundIndices;
                foreach (HImage image in calibrationImages)
                {
                    featureExtraction.ExtractCorners(image, calibrationObjectModel, out corners, out numFound, out foundIndices);
                    if (numFound.I > 0)
                    {
                        Console.WriteLine($"在图像中找到 {numFound.I} 个角点。");
                    }
                    else
                    {
                        Console.WriteLine("未在图像中找到角点。");
                    }
                }

                // 单目标定
                SingleCameraCalibration singleCalibration = new SingleCameraCalibration();
                HTuple singleCameraParams, singlePoseParams, singleDistortionParams;
                singleCalibration.Calibrate(calibrationImages, calibrationObjectModel, new HTuple(),
                    out singleCameraParams, out singlePoseParams, out singleDistortionParams);

                // 双目标定（假设已有左右相机图像）
                List<HImage> leftCalibrationImages = new List<HImage>();
                List<HImage> rightCalibrationImages = new List<HImage>();
                // 填充左右相机图像数据
                StereoCameraCalibration stereoCalibration = new StereoCameraCalibration();
                HTuple leftCameraParams, rightCameraParams, relativePoseParams, leftDistortionParams, rightDistortionParams;
                stereoCalibration.Calibrate(leftCalibrationImages, rightCalibrationImages, calibrationObjectModel, new HTuple(),
                    out leftCameraParams, out rightCameraParams, out relativePoseParams, out leftDistortionParams, out rightDistortionParams);

                // 多目标定（假设已有多个相机图像）
                List<List<HImage>> multiCalibrationImages = new List<List<HImage>>();
                // 填充多个相机图像数据
                CalibrationMethods calibrationMethods = new CalibrationMethods();
                List<HTuple> multiCameraParamsList, multiPoseParamsList;
                calibrationMethods.MultiCameraCalibration(multiCalibrationImages, calibrationObjectModel,
                    out multiCameraParamsList, out multiPoseParamsList);

                // 结果展示
                ResultPresenter resultPresenter = new ResultPresenter();
                resultPresenter.PresentSingleCalibrationResult(singleCameraParams, singlePoseParams, singleDistortionParams);
                resultPresenter.PresentStereoCalibrationResult(leftCameraParams, rightCameraParams, relativePoseParams,
                    leftDistortionParams, rightDistortionParams);
                resultPresenter.PresentMultiCalibrationResult(multiCameraParamsList, multiPoseParamsList);

                // 结果保存
                ParameterFileGenerator parameterFileGenerator = new ParameterFileGenerator();
                parameterFileGenerator.SaveSingleCalibrationResult(singleCameraParams, singlePoseParams, singleDistortionParams, "SingleCalibrationResult.xml");
                parameterFileGenerator.SaveStereoCalibrationResult(leftCameraParams, rightCameraParams, relativePoseParams,
                    leftDistortionParams, rightDistortionParams, "StereoCalibrationResult.xml");
                parameterFileGenerator.SaveMultiCalibrationResult(multiCameraParamsList, multiPoseParamsList, "MultiCalibrationResult.xml");

                // 断开相机连接
                cameraConnection.DisconnectCamera();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"程序出现错误: {ex.Message}");
            }

            Console.WriteLine("程序执行完毕，按任意键退出。");
            Console.ReadKey();
        }
    }
}
