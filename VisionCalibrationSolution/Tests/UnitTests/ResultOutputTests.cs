using HalconDotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using VisionCalibrationProject.ResultOutput;

namespace VisionCalibrationProject.Tests
{
    [TestFixture]
    public class ResultOutputTests
    {
        private ParameterFileGenerator parameterFileGenerator;
        private HTuple singleCameraParams;
        private HTuple singlePoseParams;
        private HTuple singleDistortionParams;
        private HTuple leftCameraParams;
        private HTuple rightCameraParams;
        private HTuple relativePoseParams;
        private HTuple leftDistortionParams;
        private HTuple rightDistortionParams;
        private List<HTuple> multiCameraParamsList;
        private List<HTuple> multiPoseParamsList;

        [OneTimeSetUp]
        public void Setup()
        {
            parameterFileGenerator = new ParameterFileGenerator();

            // 初始化单目标定结果模拟数据
            singleCameraParams = new HTuple(new double[] { 1.0, 2.0, 3.0 });
            singlePoseParams = new HTuple(new double[] { 4.0, 5.0, 6.0 });
            singleDistortionParams = new HTuple(new double[] { 0.1, 0.2, 0.3 });

            // 初始化双目标定结果模拟数据
            leftCameraParams = new HTuple(new double[] { 7.0, 8.0, 9.0 });
            rightCameraParams = new HTuple(new double[] { 10.0, 11.0, 12.0 });
            relativePoseParams = new HTuple(new double[] { 13.0, 14.0, 15.0 });
            leftDistortionParams = new HTuple(new double[] { 0.4, 0.5, 0.6 });
            rightDistortionParams = new HTuple(new double[] { 0.7, 0.8, 0.9 });

            // 初始化多目标定结果模拟数据
            multiCameraParamsList = new List<HTuple>
            {
                new HTuple(new double[] { 16.0, 17.0, 18.0 }),
                new HTuple(new double[] { 19.0, 20.0, 21.0 })
            };
            multiPoseParamsList = new List<HTuple>
            {
                new HTuple(new double[] { 22.0, 23.0, 24.0 }),
                new HTuple(new double[] { 25.0, 26.0, 27.0 })
            };
        }

        [Test]
        public void TestSaveSingleCalibrationResult()
        {
            try
            {
                string filePath = "SingleCalibrationResultTest.xml";
                parameterFileGenerator.SaveSingleCalibrationResult(singleCameraParams, singlePoseParams, singleDistortionParams, filePath);

                // 验证文件是否存在
                Assert.IsTrue(File.Exists(filePath), "单目标定结果保存文件未生成");

                // 可以进一步验证文件内容的正确性，这里简单打印提示
                Console.WriteLine("单目标定结果保存文件生成成功，可进一步验证文件内容。");

                // 删除测试文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"单目标定结果保存测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestSaveStereoCalibrationResult()
        {
            try
            {
                string filePath = "StereoCalibrationResultTest.xml";
                parameterFileGenerator.SaveStereoCalibrationResult(leftCameraParams, rightCameraParams, relativePoseParams,
                    leftDistortionParams, rightDistortionParams, filePath);

                // 验证文件是否存在
                Assert.IsTrue(File.Exists(filePath), "双目标定结果保存文件未生成");

                // 可以进一步验证文件内容的正确性，这里简单打印提示
                Console.WriteLine("双目标定结果保存文件生成成功，可进一步验证文件内容。");

                // 删除测试文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"双目标定结果保存测试失败: {ex.Message}");
            }
        }

        [Test]
        public void TestSaveMultiCalibrationResult()
        {
            try
            {
                string filePath = "MultiCalibrationResultTest.xml";
                parameterFileGenerator.SaveMultiCalibrationResult(multiCameraParamsList, multiPoseParamsList, filePath);

                // 验证文件是否存在
                Assert.IsTrue(File.Exists(filePath), "多目标定结果保存文件未生成");

                // 可以进一步验证文件内容的正确性，这里简单打印提示
                Console.WriteLine("多目标定结果保存文件生成成功，可进一步验证文件内容。");

                // 删除测试文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"多目标定结果保存测试失败: {ex.Message}");
            }
        }
    }
}