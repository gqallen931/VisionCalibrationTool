using HalconDotNet;
using System;
using VisionCalibrationTool.Models;

namespace VisionCalibrationTool.Services
{
    public class CalibrationService
    {
        private HDevelopExport _halconExport = new HDevelopExport();
        
        // 单目标定
        public CalibrationParams SingleCameraCalibration(string[] imageFiles, CalibrationBoardType boardType)
        {
            try
            {
                // 初始化Halcon标定参数
                HTuple cameraParams = new HTuple();
                HTuple cameraPoses = new HTuple();
                HTuple worldPoses = new HTuple();
                HTuple quality = new HTuple();

                // 调用Halcon标定算法
                _halconExport.CalibrateSingleCamera(
                    imageFiles, 
                    boardType.ToHalconBoardType(),
                    out cameraParams,
                    out cameraPoses,
                    out worldPoses,
                    out quality);

                return new CalibrationParams
                {
                    CameraParameters = cameraParams,
                    CameraPoses = cameraPoses,
                    WorldPoses = worldPoses,
                    Quality = quality
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("单目标定失败", ex);
            }
        }

        // 双目标定
        public StereoCalibrationParams StereoCameraCalibration(
            string[] leftImages, 
            string[] rightImages,
            CalibrationBoardType boardType)
        {
            try
            {
                HTuple leftParams = new HTuple();
                HTuple rightParams = new HTuple();
                HTuple relPose = new HTuple();
                HTuple quality = new HTuple();

                // 调用Halcon双目标定算法
                _halconExport.CalibrateStereoCameras(
                    leftImages,
                    rightImages,
                    boardType.ToHalconBoardType(),
                    out leftParams,
                    out rightParams,
                    out relPose,
                    out quality);

                return new StereoCalibrationParams
                {
                    LeftCameraParameters = leftParams,
                    RightCameraParameters = rightParams,
                    RelativePose = relPose,
                    Quality = quality
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("双目标定失败", ex);
            }
        }

        // 生成标定板
        public void GenerateCalibrationBoard(
            CalibrationBoardType boardType,
            int width,
            int height,
            double spacing,
            string savePath)
        {
            try
            {
                _halconExport.GenerateCalibrationBoard(
                    boardType.ToHalconBoardType(),
                    width,
                    height,
                    spacing,
                    savePath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("标定板生成失败", ex);
            }
        }
    }

    public enum CalibrationBoardType
    {
        Chessboard,
        CircleGrid
    }
}
