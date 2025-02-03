using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VisionCalibrationTool.Models;
using VisionCalibrationTool.Services;

namespace VisionCalibrationTool.ViewModels
{
    /// <summary>
    /// 主界面视图模型
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly HalconService _halconService = new HalconService();
        private CalibrationParams _calibrationResult;
        private ObservableCollection<RecognitionResult> _recognitionResults = new ObservableCollection<RecognitionResult>();
        private bool _isRecognitionEnabled;

        // 标定模式枚举
        public enum CalibrationMode { SingleCamera, Stereo, MultiCamera }
        public CalibrationMode SelectedMode { get; set; } = CalibrationMode.SingleCamera;

        // 标定结果
        public CalibrationParams CalibrationResult
        {
            get => _calibrationResult;
            set => Set(ref _calibrationResult, value);
        }

        // 识别结果列表
        public ObservableCollection<RecognitionResult> RecognitionResults
        {
            get => _recognitionResults;
            set => Set(ref _recognitionResults, value);
        }

        // 识别开关
        public bool IsRecognitionEnabled
        {
            get => _isRecognitionEnabled;
            set
            {
                Set(ref _isRecognitionEnabled, value);
                if (value) StartRecognition();
            }
        }

        // 生成标定板命令
        public ICommand GenerateCalibrationPlateCommand => new RelayCommand(() =>
        {
            try
            {
                var image = _halconService.GenerateCheckerboard(7, 7, 0.03);
                ImageHelper.DisplayImage(image, "CalibrationPlate");
            }
            catch (Exception ex)
            {
                // 处理异常
            }
        });

        // 执行标定命令
        public ICommand RunCalibrationCommand => new RelayCommand(async () =>
        {
            var images = await LoadCalibrationImagesAsync();
            CalibrationResult = await Task.Run(() => _halconService.CalibrateSingleCamera(images));
        });

        // 开始实时识别
        private void StartRecognition()
        {
            Task.Run(() =>
            {
                while (IsRecognitionEnabled)
                {
                    var image = CaptureCameraImage();
                    var result = _halconService.RecognizeObject(image, "shape_model.shm");
                    App.Current.Dispatcher.Invoke(() => RecognitionResults.Add(result));
                    Task.Delay(100).Wait();
                }
            });
        }

        // 模拟加载标定图像
        private async Task<List<HImage>> LoadCalibrationImagesAsync()
        {
            return await Task.Run(() =>
            {
                // 实际项目中应从相机或文件加载图像
                return new List<HImage>();
            });
        }

        // 模拟相机采集
        private HImage CaptureCameraImage()
        {
            // 实际项目中应调用相机SDK
            return new HImage();
        }
    }
}