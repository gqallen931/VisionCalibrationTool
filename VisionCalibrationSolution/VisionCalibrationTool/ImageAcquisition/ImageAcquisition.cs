using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace VisionCalibrationProject.ImageAcquisition
{
    public class ImageAcquisition
    {
        private CameraConnection cameraConnection;
        private bool isAcquiring;
        private Thread acquisitionThread;

        public ImageAcquisition()
        {
            cameraConnection = new CameraConnection();
            isAcquiring = false;
        }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="deviceType">相机设备类型</param>
        /// <param name="deviceName">相机设备名称</param>
        /// <returns>连接是否成功</returns>
        public bool ConnectCamera(string deviceType, string deviceName)
        {
            return cameraConnection.ConnectCamera(deviceType, deviceName);
        }

        /// <summary>
        /// 断开相机连接
        /// </summary>
        public void DisconnectCamera()
        {
            cameraConnection.DisconnectCamera();
        }

        /// <summary>
        /// 开始实时采集图像
        /// </summary>
        /// <param name="imageCallback">采集到图像时的回调函数</param>
        public void StartRealTimeAcquisition(Action<HImage> imageCallback)
        {
            if (!isAcquiring)
            {
                isAcquiring = true;
                acquisitionThread = new Thread(() => RealTimeAcquisitionLoop(imageCallback));
                acquisitionThread.Start();
            }
        }

        /// <summary>
        /// 停止实时 停止实时采集图像
        /// </summary>
        public void StopRealTimeAcquisition()
        {
            if (isAcquiring)
            {
                isAcquiring = false;
                if (acquisitionThread != null && acquisitionThread.IsAlive)
                {
                    acquisitionThread.Join();
                }
            }
        }

        private void RealTimeAcquisitionLoop(Action<HImage> imageCallback)
        {
            while (isAcquiring)
            {
                HImage image = cameraConnection.GrabImage();
                if (image != null)
                {
                    imageCallback?.Invoke(image);
                    image.Dispose();
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 批量采集图像
        /// </summary>
        /// <param name="acquisitionCount">采集图像的数量</param>
        /// <param name="savePath">图像保存路径</param>
        /// <returns>采集到的图像列表</returns>
        public List<HImage> BatchAcquisition(int acquisitionCount, string savePath)
        {
            List<HImage> images = new List<HImage>();
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            for (int i = 0; i < acquisitionCount; i++)
            {
                HImage image = cameraConnection.GrabImage();
                if (image != null)
                {
                    images.Add(image);
                    string fileName = $"image_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{i}.jpg";
                    string fullPath = Path.Combine(savePath, fileName);
                    image.WriteImage("jpeg", 0, fullPath);
                }
            }

            return images;
        }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns>设置是否成功</returns>
        public bool SetCameraParameter(string paramName, HTuple paramValue)
        {
            return cameraConnection.SetCameraParameter(paramName, paramValue);
        }

        /// <summary>
        /// 获取相机参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns>参数值</returns>
        public HTuple GetCameraParameter(string paramName)
        {
            return cameraConnection.GetCameraParameter(paramName);
        }
    }
}