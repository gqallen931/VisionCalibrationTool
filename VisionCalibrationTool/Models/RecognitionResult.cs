namespace VisionCalibrationTool.Models
{
	/// <summary>
	/// ʶ��������ģ��
	/// </summary>
	public class RecognitionResult
	{
		public string ObjectType { get; set; }    // �������ͱ�ʶ
		public double CenterX { get; set; }       // X���� (����)
		public double CenterY { get; set; }       // Y���� (����)
		public double Width { get; set; }         // ��� (����)
		public double Height { get; set; }        // �߶� (����)
		public double Angle { get; set; }         // ��ת�Ƕ� (��)
		public double Confidence { get; set; }    // ���Ŷ� (0~1)
	}
}