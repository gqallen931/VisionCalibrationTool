using HalconDotNet;
using System;

namespace VisionCalibrationProject.ImageAcquisition
{
    public class CameraConnection
    {
        private HFramegrabber framegrabber;
        private bool isConnected;

        public CameraConnection()
        {
            isConnected = false;
        }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="deviceType">相机设备类型</param>
        /// <param name="deviceName">相机设备名称</param>
        /// <returns>连接是否成功</returns>
       >
        public bool ConnectCamera(string deviceType, string deviceName)
        {
            try
            {
                framegrabber = new HFramegrabber(deviceType, 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1, "false", deviceName, 0, -1);
                framegrabber.OpenFramegrabber();
                isConnected = true;
                return true;
            }
            catch (HOperatorException ex)
            {
                Console.WriteLine($"相机连接失败: {ex.Message}");
                isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// 断开相机连接
        /// </summary>
        public void DisconnectCamera()
        {
            if (isConnected)
            {
                try
                {
                    framegrabber.CloseFramegrabber();
                    framegrabber.Dispose();
                    isConnected = false;
                }
                catch (HOperatorException ex)
                {
                    Console.WriteLine($"相机断开连接失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 抓取单张图像
        /// </summary>
        /// <returns>抓取到的图像</returns>
        public HImage GrabImage()
        {
            if (isConnected)
            {
                try
                {
                    HImage image = new HImage();
                    image.GrabImage(framegrabber);
                    return image;
                }
                catch (HOperatorException ex)
                {
                    Console.WriteLine($"图像抓取失败: {ex.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("相机未连接，无法抓取图像。");
                return null;
            }
        }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns>设置是否成功</returns>
        public bool SetCameraParameter(string paramName, HTuple paramValue)
        {
            if (isConnected)
            {
                try
                {
                    framegrabber.SetFramegrabberParam(paramName, paramValue);
                    return true;
                }
                catch (HOperatorException ex)
                {
                    Console.WriteLine($"设置相机参数 {paramName} 失败: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("相机未连接，无法设置参数。");
                return false;
            }
        }

        /// <summary>
        /// 获取相机参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <returns>参数值</returns>
        public HTuple GetCameraParameter(string paramName)
        {
            if (isConnected)
            {
                try
                {
                    return framegrabber.GetFramegrabberParam(paramName);
                }
                catch (HOperatorException ex)
                {
                    Console.WriteLine($"获取相机参数 {paramName} 失败: {ex.Message}");
                    return new HTuple();
                }
            }
            else
            {
                Console.WriteLine("相机未连接，无法获取参数。");
                return new HTuple();
            }
        }
    }
}