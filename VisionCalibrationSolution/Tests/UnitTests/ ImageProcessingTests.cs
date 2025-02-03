using HalconDotNet;
using NUnit.Framework;
using System;
using VisionCalibrationProject.ImageProcessing;

namespace VisionCalibrationProject.Tests
{
    [TestFixture]
    public class ImageProcessingTests
    {
        private ImageFiltering imageFiltering;
        private ImageEnhancement imageEnhancement;
        private FeatureExtraction featureExtraction;
        private GeometricCorrection geometricCorrection;
        private HImage testImage;

        [OneTimeSetUp]
        public void Setup()
        {
            imageFiltering = new ImageFiltering();
            imageEnhancement = new ImageEnhancement();
            featureExtraction = new FeatureExtraction();
            geometricCorrection = new GeometricCorrection();

            // 加载测试图像，这里假设存在一个测试图像文件
            try
            {
                testImage = new HImage("test_image.jpg");
            }
            catch (HOperatorException ex)
            {
                Assert.Fail($"无法加载测试图像: {ex.Message}");
            }
        }

        [Test]
        public void TestMeanFiltering()
        {
            try
            {
                int maskSize = 3;
                HImage filteredImage = imageFiltering.MeanFiltering(testImage, maskSize);
                Assert.IsNotNull(filteredImage, "均值滤波后图像为空");
                filteredImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"均值滤波测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestGaussianFiltering()
        {
            try
            {
                double sigma = 1.0;
                HImage filteredImage = imageFiltering.GaussianFiltering(testImage, sigma);
                Assert.IsNotNull(filteredImage, "高斯滤波后图像为空");
                filteredImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"高斯滤波测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestMedianFiltering()
        {
            try
            {
                int maskSize = 3;
                HImage filteredImage = imageFiltering.MedianFiltering(testImage, maskSize);
                Assert.IsNotNull(filteredImage, "中值滤波后图像为空");
                filteredImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"中值滤波测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestGrayStretching()
        {
            try
            {
                double lowPercent = 10;
                double highPercent = 90;
                HImage enhancedImage = imageEnhancement.GrayStretching(testImage, lowPercent, highPercent);
                Assert.IsNotNull(enhancedImage, "灰度拉伸后图像为空");
                enhancedImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"灰度拉伸测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestHistogramEqualization()
        {
            try
            {
                HImage enhancedImage = imageEnhancement.HistogramEqualization(testImage);
                Assert.IsNotNull(enhancedImage, "直方图均衡化后图像为空");
                enhancedImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"直方图均衡化测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestCornerExtraction()
        {
            try
            {
                HTuple calibrationObjectModel = new HTuple(); // 实际中需要正确初始化
                HTuple corners, numFound, foundIndices;
                featureExtraction.ExtractCorners(testImage, calibrationObjectModel, out corners, out numFound, out foundIndices);
                Assert.IsNotNull(corners, "角点提取结果为空");
            }
            catch (Exception ex)
            {
                Assert.Fail($"角点提取测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestEdgeExtraction()
        {
            try
            {
                HTuple lowThreshold = 50;
                HTuple highThreshold = 150;
                HImage edges;
                featureExtraction.ExtractEdges(testImage, out edges, lowThreshold, highThreshold);
                Assert.IsNotNull(edges, "边缘提取结果为空");
                edges.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"边缘提取测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestGeometricCorrection()
        {
            try
            {
                HTuple cameraParams = new HTuple(); // 实际中需要正确初始化
                HTuple distortionParams = new HTuple(); // 实际中需要正确初始化
                HImage correctedImage = geometricCorrection.CorrectGeometry(testImage, cameraParams, distortionParams);
                Assert.IsNotNull(correctedImage, "几何校正后图像为空");
                correctedImage.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail($"几何校正测试失败: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (testImage != null)
            {
                testImage.Dispose();
            }
        }
    }
}