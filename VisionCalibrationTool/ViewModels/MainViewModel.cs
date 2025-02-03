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
    /// ��������ͼģ��
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly HalconService _halconService = new HalconService();
        private CalibrationParams _calibrationResult;
        private ObservableCollection<RecognitionResult> _recognitionResults = new ObservableCollection<RecognitionResult>();
        private bool _isRecognitionEnabled;

        // �궨ģʽö��
        public enum CalibrationMode { SingleCamera, Stereo, MultiCamera }
        public CalibrationMode SelectedMode { get; set; } = CalibrationMode.SingleCamera;

        // �궨���
        public CalibrationParams CalibrationResult
        {
            get => _calibrationResult;
            set => Set(ref _calibrationResult, value);
        }

        // ʶ�����б�
        public ObservableCollection<RecognitionResult> RecognitionResults
        {
            get => _recognitionResults;
            set => Set(ref _recognitionResults, value);
        }

        // ʶ�𿪹�
        public bool IsRecognitionEnabled
        {
            get => _isRecognitionEnabled;
            set
            {
                Set(ref _isRecognitionEnabled, value);
                if (value) StartRecognition();
            }
        }

        // ���ɱ궨������
        public ICommand GenerateCalibrationPlateCommand => new RelayCommand(() =>
        {
            try
            {
                var image = _halconService.GenerateCheckerboard(7, 7, 0.03);
                ImageHelper.DisplayImage(image, "CalibrationPlate");
            }
            catch (Exception ex)
            {
                // �����쳣
            }
        });

        // ִ�б궨����
        public ICommand RunCalibrationCommand => new RelayCommand(async () =>
        {
            var images = await LoadCalibrationImagesAsync();
            CalibrationResult = await Task.Run(() => _halconService.CalibrateSingleCamera(images));
        });

        // ��ʼʵʱʶ��
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

        // ģ����ر궨ͼ��
        private async Task<List<HImage>> LoadCalibrationImagesAsync()
        {
            return await Task.Run(() =>
            {
                // ʵ����Ŀ��Ӧ��������ļ�����ͼ��
                return new List<HImage>();
            });
        }

        // ģ������ɼ�
        private HImage CaptureCameraImage()
        {
            // ʵ����Ŀ��Ӧ�������SDK
            return new HImage();
        }
    }
}