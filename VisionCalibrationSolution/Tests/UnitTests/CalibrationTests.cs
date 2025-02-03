using HalconDotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using VisionCalibrationProject.Calibration;

namespace VisionCalibrationProject.Tests
{
    [TestFixture]
    public class CalibrationTests
    {
        private SingleCameraCalibration singleCameraCalibration;
        private StereoCameraCalibration stereoCameraCalibration;
        private CalibrationMethods calibrationMethods;
        private List<HImage> singleCalibrationImages;
        private List<List<HImage>> stereoCalibrationImages;
        private List<List<HImage>> multiCalibrationImages;
        private HTuple calibrationObjectModel;
        private HTuple calibrationBoardSize;

        [OneTimeSetUp]
        public void Setup()
        {
            singleCameraCalibration = new SingleCameraCalibration();
            stereoCameraCalibration = new StereoCameraCalibration();
            calibrationMethods = new CalibrationMethods();

            // 模拟加载标定图像，实际应用中需要替换为真实的图像加载逻辑
            singleCalibrationImages = new List<HImage>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    singleCalibrationImages.Add(new HImage("single_calibration_image_" + i + ".jpg"));
                }
                catch (HOperatorException ex)
                {
                    Assert.Fail($"无法加载单目标定测试图像: {ex.Message}");
                }
            }

            stereoCalibrationImages = new List<List<HImage>>();
            List<HImage> leftImages = new List<HImage>();
            List<HImage> rightImages = new List<HImage>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    leftImages.Add(new HImage("left_calibration_image_" + i + ".jpg"));
                    rightImages.Add(new HImage("right_calibration_image_" + i + ".jpg"));
                }
                catch (HOperatorException ex)
                {
                    Assert.Fail($"无法加载双目标定测试图像: {ex.Message}");
                }
            }
            stereoCalibrationImages.Add(leftImages);
            stereoCalibrationImages.Add(rightImages);

            multiCalibrationImages = new List<List<HImage>>();
            for (int j = 0; j < 3; j++) // 模拟三目相机
            {
                List<HImage> cameraImages = new List<HImage>();
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        cameraImages.Add(new HImage("multi_calibration_image_" + j + "_" + i + ".jpg"));
                    }
                    catch (HOperatorException ex)
                    {
                        Assert.Fail($"无法加载多目标定测试图像: {ex.Message}");
                    }
                }
                multiCalibrationImages.Add(cameraImages);
            }

            // 初始化标定板模型和尺寸，实际应用中需要根据真实情况设置
            calibrationObjectModel = new HTuple(); 
            calibrationBoardSize = new HTuple(); 
        }

        [Test]
        public void TestSingleCameraCalibration()
        {
            try
            {
                HTuple cameraParams, poseParams, distortionParams;
                singleCameraCalibration.Calibrate(singleCalibrationImages, calibrationObjectModel, calibrationBoardSize,
                    out cameraParams, out poseParams, out distortionParams);

                Assert.IsNotNull(cameraParams, "单目标定相机内参为空");
                Assert.IsNotNull(poseParams, "单目标定位姿参数为空");
                Assert.IsNotNull(distortionParams, "单目标定畸变系数为空");
            }
            catch (Exception ex)
            {
                Assert.Fail($"单目标定测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestStereoCameraCalibration()
        {
            try
            {
                HTuple leftCameraParams, rightCameraParams, relativePoseParams, leftDistortionParams, rightDistortionParams;
                stereoCameraCalibration.Calibrate(stereoCalibrationImages[0], stereoCalibrationImages[1],
                    calibrationObjectModel, calibrationBoardSize,
                    out leftCameraParams, out rightCameraParams, out relativePoseParams,
                    out leftDistortionParams, out rightDistortionParams);

                Assert.IsNotNull(leftCameraParams, "双目标定左相机内参为空");
                Assert.IsNotNull(rightCameraParams, "双目标定右相机内参为空");
                Assert.IsNotNull(relativePoseParams, "双目标定相对位姿参数为空");
                Assert.IsNotNull(leftDistortionParams, "双目标定左相机畸变系数为空");
                Assert.IsNotNull(rightDistortionParams, "双目标定右相机畸变系数为空");
            }
            catch (Exception ex)
            {
                Assert.Fail($"双目标定测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestMultiCameraCalibration()
        {
            try
            {
                List<HTuple> cameraParamsList, poseParamsList;
                calibrationMethods.MultiCameraCalibration(multiCalibrationImages, calibrationObjectModel,
                    out cameraParamsList, out poseParamsList);

                Assert.IsNotNull(cameraParamsList, "多目标定相机内参列表为空");
                Assert.IsNotNull(poseParamsList, "多目标定位姿参数列表为空");
                Assert.IsTrue(cameraParamsList.Count > 0, "多目标定相机内参列表长度为 0");
                Assert.IsTrue(poseParamsList.Count > 0, "多目标定位姿参数列表长度为 0");
            }
            catch (Exception ex)
            {
                Assert.Fail($"多目标定测试失败: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            foreach (HImage image in singleCalibrationImages)
            {
                image.Dispose();
            }

            foreach (List<HImage> cameraImages in stereoCalibrationImages)
            {
                foreach (HImage image in cameraImages)
                {
                    image.Dispose();
                }
            }

            foreach (List<HImage> cameraImages in multiCalibrationImages)
            {
                foreach (HImage image in cameraImages)
                {
                    image.Dispose();
                }
            }
        }
    }
}