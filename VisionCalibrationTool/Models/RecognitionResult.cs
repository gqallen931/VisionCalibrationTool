namespace VisionCalibrationTool.Models
{
	/// <summary>
	/// 识别结果数据模型
	/// </summary>
	public class RecognitionResult
	{
		public string ObjectType { get; set; }    // 物体类型标识
		public double CenterX { get; set; }       // X坐标 (像素)
		public double CenterY { get; set; }       // Y坐标 (像素)
		public double Width { get; set; }         // 宽度 (像素)
		public double Height { get; set; }        // 高度 (像素)
		public double Angle { get; set; }         // 旋转角度 (度)
		public double Confidence { get; set; }    // 置信度 (0~1)
	}
}