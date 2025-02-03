using HalconDotNet;
using System;
using System.Collections.Generic;
using VisionCalibrationProject.Forms;

namespace VisionCalibrationProject.Calibration
{
    public class CalibrationModeSelector
    {
        private CalibrationMethods calibrationMethods;

        public CalibrationModeSelector()
        {
            calibrationMethods = new CalibrationMethods();
        }

        /// <summary>
        /// 根据选择的标定模式进行标定
        /// </summary>
        /// <param name="calibrationMode">标定模式，如 "单目", "双目", "三目" 等</param>
        /// <param name="calibrationImages">标定图像列表，对于多相机情况，是每个相机的图像列表的列表</param>
        /// <param name="calibrationObjectModel">标定板模型</param>
        /// <param name="cameraParams">输出的相机内参</param>
        /// <param name="poseParams">输出的相机位姿参数</param>
        /// <param name="distortionParams">输出的畸变系数（仅单目有效）</param>
        /// <param name="relativePoseParams">输出的相对位姿参数（仅双目有效）</param>
        public void PerformCalibration(string calibrationMode, object calibrationImages, HTuple calibrationObjectModel,
            out List<HTuple> cameraParams, out List<HTuple> poseParams, out List<HTuple> distortionParams, out HTuple relativePoseParams)
        {
            cameraParams = new List<HTuple>();
            poseParams = new List<HTuple>();
            distortionParams = new List<HTuple>();
            relativePoseParams = new HTuple();

            switch (calibrationMode)
            {
                case "单目":
                    List<HImage> singleCalibrationImages = calibrationImages as List<HImage>;
                    if (singleCalibrationImages == null)
                    {
                        throw new ArgumentException("输入的标定图像格式不正确，单目标定需要 List<HImage> 类型。");
                    }
                    HTuple singleCameraParams, singlePoseParams, singleDistortionParams;
                    calibrationMethods.SingleCameraCalibration(singleCalibrationImages, calibrationObjectModel, new HTuple(),
                        out singleCameraParams, out singlePoseParams, out singleDistortionParams);
                    cameraParams.Add(singleCameraParams);
                    poseParams.Add(singlePoseParams);
                    distortionParams.Add(singleDistortionParams);
                    break;
                case "双目":
                    List<List<HImage>> stereoCalibrationImages = calibrationImages as List<List<HImage>>;
                    if (stereoCalibrationImages == null || stereoCalibrationImages.Count != 2)
                    {
                        throw new ArgumentException("输入的标定图像格式不正确，双目标定需要 List<List<HImage>> 类型且包含两个相机的图像列表。");
                    }
                    HTuple leftCameraParams, rightCameraParams, stereoRelativePoseParams;
                    calibrationMethods.StereoCameraCalibration(stereoCalibrationImages[0], stereoCalibrationImages[1],
                        calibrationObjectModel, out leftCameraParams, out rightCameraParams, out stereoRelativePoseParams);
                    cameraParams.Add(leftCameraParams);
                    cameraParams.Add(rightCameraParams);
                    // 这里简单假设位姿参数分别为左右相机相对于标定板的位姿
                    poseParams.Add(new HTuple()); 
                    poseParams.Add(new HTuple()); 
                    relativePoseParams = stereoRelativePoseParams;
                    break;
                case "三目":
                    ValidateMultiCalibrationImages(calibrationImages, 3);
                    List<List<HImage>> threeCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> threeCameraParamsList, threePoseParamsList;
                    calibrationMethods.MultiCameraCalibration(threeCameraCalibrationImages, calibrationObjectModel,
                        out threeCameraParamsList, out threePoseParamsList);
                    cameraParams = threeCameraParamsList;
                    poseParams = threePoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                case "四目":
                    ValidateMultiCalibrationImages(calibrationImages, 4);
                    List<List<HImage>> fourCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> fourCameraParamsList, fourPoseParamsList;
                    calibrationMethods.MultiCameraCalibration(fourCameraCalibrationImages, calibrationObjectModel,
                        out fourCameraParamsList, out fourPoseParamsList);
                    cameraParams = fourCameraParamsList;
                    poseParams = fourPoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                case "五目":
                    ValidateMultiCalibrationImages(calibrationImages, 5);
                    List<List<HImage>> fiveCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> fiveCameraParamsList, fivePoseParamsList;
                    calibrationMethods.MultiCameraCalibration(fiveCameraCalibrationImages, calibrationObjectModel,
                        out fiveCameraParamsList, out fivePoseParamsList);
                    cameraParams = fiveCameraParamsList;
                    poseParams = fivePoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                case "六目":
                    ValidateMultiCalibrationImages(calibrationImages, 6);
                    List<List<HImage>> sixCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> sixCameraParamsList, sixPoseParamsList;
                    calibrationMethods.MultiCameraCalibration(sixCameraCalibrationImages, calibrationObjectModel,
                        out sixCameraParamsList, out sixPoseParamsList);
                    cameraParams = sixCameraParamsList;
                    poseParams = sixPoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                case "七目":
                    ValidateMultiCalibrationImages(calibrationImages, 7);
                    List<List<HImage>> sevenCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> sevenCameraParamsList, sevenPoseParamsList;
                    calibrationMethods.MultiCameraCalibration(sevenCameraCalibrationImages, calibrationObjectModel,
                        out sevenCameraParamsList, out sevenPoseParamsList);
                    cameraParams = sevenCameraParamsList;
                    poseParams = sevenPoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                case "八目":
                    ValidateMultiCalibrationImages(calibrationImages, 8);
                    List<List<HImage>> eightCameraCalibrationImages = (List<List<HImage>>)calibrationImages;
                    List<HTuple> eightCameraParamsList, eightPoseParamsList;
                    calibrationMethods.MultiCameraCalibration(eightCameraCalibrationImages, calibrationObjectModel,
                        out eightCameraParamsList, out eightPoseParamsList);
                    cameraParams = eightCameraParamsList;
                    poseParams = eightPoseParamsList;
                    distortionParams = new List<HTuple>(); 
                    relativePoseParams = new HTuple(); 
                    break;
                default:
                    throw new ArgumentException($"不支持的标定模式: {calibrationMode}");
            }
        }

        private void ValidateMultiCalibrationImages(object calibrationImages, int expectedCameraCount)
        {
            List<List<HImage>> multiCalibrationImages = calibrationImages as List<List<HImage>>;
            if (multiCalibrationImages == null || multiCalibrationImages.Count != expectedCameraCount)
            {
                throw new ArgumentException($"输入的标定图像格式不正确，{expectedCameraCount}目标定需要 List<List<HImage>> 类型且包含 {expectedCameraCount} 个相机的图像列表。");
            }
        }
    }
}