
namespace VisionCalibrationTool.Models
{
    /// <summary>
    /// 相机设备数据模型
    /// </summary>
    public class CameraModel
    {
        public string Name { get; set; }          // 相机名称
        public string IP { get; set; }            // 相机IP地址
        public bool IsConnected { get; set; }     // 连接状态
    }
}