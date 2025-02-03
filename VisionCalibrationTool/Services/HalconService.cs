using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using VisionCalibrationTool.Models;

namespace VisionCalibrationTool.Services
{
    /// <summary>
    /// Halcon 功能服务封装
    /// </summary>
    public class HalconService : IDisposable
    {
        private HWindow _halconWindow = new HWindow();
        private HTuple _calibrationDataID = new HTuple();

        // 生成棋盘格标定板
        public HImage GenerateCheckerboard(int rows, int cols, double squareSize)
        {
            try
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string descrPath = Path.Combine(docPath, "caltab.descr");
                string psPath = Path.Combine(docPath, "caltab.ps");

                // 创建标定数据
                HOperatorSet.CreateCalibData("calibration_object", 1, 1, out _calibrationDataID);
                HOperatorSet.SetCalibDataCalibObject(_calibrationDataID, 0, "checkerboard",
                    new HTuple(rows, cols, squareSize));

                // 生成标定板图像
                HImage caltabImage = new HImage();
                HOperatorSet.CreateCheckerboardImage(out caltabImage, "checkerboard",
                    rows, cols, squareSize, 0.5, 0, 255);

                // 保存标定板描述文件
                HOperatorSet.WriteCalibData(_calibrationDataID, descrPath);
                HOperatorSet.WriteImage(caltabImage, "ps", 0, psPath);

                return caltabImage;
            }
            catch (HOperatorException ex)
            {
                throw new Exception($"Halcon Error - GenerateCheckerboard: {ex.Message}");
            }
        }

        // 单目标定
        public CalibrationParams CalibrateSingleCamera(List<HImage> calibrationImages)
        {
            try
            {
                HOperatorSet.CreateCalibData("calibration_object", 1, 1, out _calibrationDataID);
                HOperatorSet.SetCalibDataCalibObject(_calibrationDataID, 0, "checkerboard",
                    new HTuple(7, 7, 0.03)); // 示例参数

                // 添加所有标定图像
                for (int i = 0; i < calibrationImages.Count; i++)
                {
                    HOperatorSet.FindCalibObject(calibrationImages[i], _calibrationDataID, 0, 0, i,
                        new HTuple(), new HTuple());
                }

                // 执行标定
                HTuple cameraParams, error;
                HOperatorSet.CalibrateCameras(_calibrationDataID, out cameraParams, out error);

                return new CalibrationParams
                {
                    FocalLength = cameraParams[0].D,
                    DistortionCoeffs = new double[] {
                        cameraParams[4].D, cameraParams[5].D,
                        cameraParams[6].D, cameraParams[7].D,
                        cameraParams[8].D
                    },
                    ReprojectionError = error.D
                };
            }
            catch (HOperatorException ex)
            {
                throw new Exception($"Halcon Error - CalibrateSingleCamera: {ex.Message}");
            }
        }

        // 目标识别
        public RecognitionResult RecognizeObject(HImage image, string modelPath)
        {
            try
            {
                HShapeModel shapeModel = new HShapeModel();
                shapeModel.ReadShapeModel(modelPath);

                HTuple row, column, angle, score;
                HOperatorSet.FindShapeModel(image, shapeModel,
                    (new HTuple(0)).TupleRad(), (new HTuple(0.5)).TupleRad(),
                    0.5, 1, 0.5, "least_squares", 0, 0.9,
                    out row, out column, out angle, out score);

                return new RecognitionResult
                {
                    CenterX = column.D,
                    CenterY = row.D,
                    Angle = angle.TupleDeg().D,
                    Confidence = score.D
                };
            }
            catch (HOperatorException ex)
            {
                throw new Exception($"Halcon Error - RecognizeObject: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _halconWindow.Dispose();
            if (_calibrationDataID != null)
                HOperatorSet.ClearCalibData(_calibrationDataID);
        }
    }
}