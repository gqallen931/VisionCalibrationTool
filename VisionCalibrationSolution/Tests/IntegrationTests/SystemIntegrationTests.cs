using HalconDotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using VisionCalibrationProject.Calibration;
using VisionCalibrationProject.ImageAcquisition;
using VisionCalibrationProject.ImageProcessing;
using VisionCalibrationProject.ResultOutput;
using VisionCalibrationProject.ResultPresentation;

namespace VisionCalibrationProject.Tests
{
    [TestFixture]
    public class SystemIntegrationTests
    {
        private CameraConnection cameraConnection;
        private ImageAcquisition imageAcquisition;
        private FeatureExtraction featureExtraction;
        private SingleCameraCalibration singleCalibration;
        private StereoCameraCalibration stereoCalibration;
        private CalibrationMethods calibrationMethods;
        private ResultPresenter resultPresenter;
        private ParameterFileGenerator parameterFileGenerator;

        [OneTimeSetUp]
        public void Setup()
        {
            cameraConnection = new CameraConnection();
            imageAcquisition = new ImageAcquisition();
            featureExtraction = new FeatureExtraction();
            singleCalibration = new SingleCameraCalibration();
            stereoCalibration = new StereoCameraCalibration();
            calibrationMethods = new CalibrationMethods();
            resultPresenter = new ResultPresenter();
            parameterFileGenerator = new ParameterFileGenerator();
        }

        [Test]
        public void TestFullSystemIntegration()
        {
            try
            {
                // 相机连接
                bool isConnected = cameraConnection.ConnectCamera("GigEVision2", "YourCameraDeviceName");
                Assert.IsTrue(isConnected, "相机连接失败");

                // 图像采集
                int acquisitionCount = 10;
                string savePath = "CalibrationImages";
                List<HImage> calibrationImages = imageAcquisition.BatchAcquisition(acquisitionCount, savePath);
                Assert.IsNotNull(calibrationImages, "图像采集失败");
                Assert.IsTrue(calibrationImages.Count == acquisitionCount, "采集的图像数量不正确");

                // 特征提取
                HTuple calibrationObjectModel = new HTuple(); // 实际中需要正确初始化
                HTuple corners, numFound, foundIndices;
                bool atLeastOneImageHasCorners = false;
                foreach (HImage image in calibrationImages)
                {
                    featureExtraction.ExtractCorners(image, calibrationObjectModel, out corners, out numFound, out foundIndices);
                    if (numFound.I > 0)
                    {
                        atLeastOneImageHasCorners = true;
                    }
                }
                Assert.IsTrue(atLeastOneImageHasCorners, "所有图像均未找到角点");

                // 单目标定
                HTuple singleCameraParams, singlePoseParams, singleDistortionParams;
                singleCalibration.Calibrate(calibrationImages, calibrationObjectModel, new HTuple(),
                    out singleCameraParams, out singlePoseParams, out singleDistortionParams);
                Assert.IsNotNull(singleCameraParams, "单目标定相机内参为空");
                Assert.IsNotNull(singlePoseParams, "单目标定位姿参数为空");
                Assert.IsNotNull(singleDistortionParams, "单目标定畸变系数为空");

                // 双目标定（假设已有左右相机图像）
                List<HImage> leftCalibrationImages = new List<HImage>();
                List<HImage> rightCalibrationImages = new List<HImage>();
                // 填充左右相机图像数据
                HTuple leftCameraParams, rightCameraParams, relativePoseParams, leftDistortionParams, rightDistortionParams;
                stereoCalibration.Calibrate(leftCalibrationImages, rightCalibrationImages, calibrationObjectModel, new HTuple(),
                    out leftCameraParams, out rightCameraParams, out relativePoseParams, out leftDistortionParams, out rightDistortionParams);
                Assert.IsNotNull(leftCameraParams, "双目标定左相机内参为空");
                Assert.IsNotNull(rightCameraParams, "双目标定右相机内参为空");
                Assert.IsNotNull(relativePoseParams, "双目标定相对位姿参数为空");
                Assert.IsNotNull(leftDistortionParams, "双目标定左相机畸变系数为空");
                Assert.IsNotNull(rightDistortionParams, "双目标定右相机畸变系数为空");

                // 多目标定（假设已有多个相机图像）
                List<List<HImage>> multiCalibrationImages = new List<List<HImage>>();
                // 填充多个相机图像数据
                List<HTuple> multiCameraParamsList, multiPoseParamsList;
                calibrationMethods.MultiCameraCalibration(multiCalibrationImages, calibrationObjectModel,
                    out multiCameraParamsList, out multiPoseParamsList);
                Assert.IsNotNull(multiCameraParamsList, "多目标定相机内参列表为空");
                Assert.IsNotNull(multiPoseParamsList, "多目标定位姿参数列表为空");

                // 结果展示（这里无法直接验证展示效果，可考虑模拟展示逻辑或检查展示相关的数据是否正确）
                resultPresenter.PresentSingleCalibrationResult(singleCameraParams, singlePoseParams, singleDistortionParams);
                resultPresenter.PresentStereoCalibrationResult(leftCameraParams, rightCameraParams, relativePoseParams,
                    leftDistortionParams, rightDistortionParams);
                resultPresenter.PresentMultiCalibrationResult(multiCameraParamsList, multiPoseParamsList);

                // 结果保存
                string singleCalibrationFilePath = "SingleCalibrationResult.xml";
                string stereoCalibrationFilePath = "StereoCalibrationResult.xml";
                string multiCalibrationFilePath = "MultiCalibrationResult.xml";
                parameterFileGenerator.SaveSingleCalibrationResult(singleCameraParams, singlePoseParams, singleDistortionParams, singleCalibrationFilePath);
                parameterFileGenerator.SaveStereoCalibrationResult(leftCameraParams, rightCameraParams, relativePoseParams,
                    leftDistortionParams, rightDistortionParams, stereoCalibrationFilePath);
                parameterFileGenerator.SaveMultiCalibrationResult(multiCameraParamsList, multiPoseParamsList, multiCalibrationFilePath);
                Assert.IsTrue(System.IO.File.Exists(singleCalibrationFilePath), "单目标定结果保存失败");
                Assert.IsTrue(System.IO.File.Exists(stereoCalibrationFilePath), "双目标定结果保存失败");
                Assert.IsTrue(System.IO.File.Exists(multiCalibrationFilePath), "多目标定结果保存失败");

                // 断开相机连接
                cameraConnection.DisconnectCamera();
            }
            catch (Exception ex)
            {
                Assert.Fail($"系统集成测试失败: {ex.Message}");
            }
        }
    }
}