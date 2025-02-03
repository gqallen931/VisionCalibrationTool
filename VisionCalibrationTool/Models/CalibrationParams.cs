using HalconDotNet;

namespace VisionCalibrationTool.Models
{
    /// <summary>
    /// 单目标定参数模型
    /// </summary>
    public class CalibrationParams
    {
        public HTuple CameraParameters { get; set; }  // 相机内参
        public HTuple CameraPoses { get; set; }       // 相机位姿
        public HTuple WorldPoses { get; set; }        // 世界坐标系位姿
        public HTuple Quality { get; set; }           // 标定质量
    }

    /// <summary>
    /// 双目标定参数模型
    /// </summary>
    public class StereoCalibrationParams
    {
        public HTuple LeftCameraParameters { get; set; }  // 左相机参数
        public HTuple RightCameraParameters { get; set; } // 右相机参数
        public HTuple RelativePose { get; set; }          // 相对位姿
        public HTuple Quality { get; set; }               // 标定质量
    }
}
