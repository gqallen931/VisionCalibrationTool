using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using HalconDotNet;

namespace VisionCalibrationProject.ResultOutput
{
    public class ParameterFileGenerator
    {
        /// <summary>
        /// 保存单目标定结果到 XML 文件
        /// </summary>
        /// <param name="cameraParams">相机内参</param>
        /// <param name="poseParams">相机位姿参数</param>
        /// <param name="distortionParams">畸变系数</param>
        /// <param name="filePath">保存的文件路径</param>
        public void SaveSingleCalibrationResult(HTuple cameraParams, HTuple poseParams, HTuple distortionParams, string filePath)
        {
            // 创建 XML 文档对象
            XmlDocument xmlDoc = new XmlDocument();
            // 创建 XML 声明
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            // 创建根节点
            XmlElement root = xmlDoc.CreateElement("SingleCalibrationResult");
            xmlDoc.AppendChild(root);

            // 添加相机内参、位姿参数和畸变系数到 XML 文档
            AddParameterElement(xmlDoc, root, "CameraParams", cameraParams);
            AddParameterElement(xmlDoc, root, "PoseParams", poseParams);
            AddParameterElement(xmlDoc, root, "DistortionParams", distortionParams);

            try
            {
                // 保存 XML 文档到指定文件路径
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存单目标定结果失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存双目标定结果到 XML 文件
        /// </summary>
        /// <param name="leftCameraParams">左相机内参</param>
        /// <param name="rightCameraParams">右相机内参</param>
        /// <param name="relativePoseParams">相对位姿参数</param>
        /// <param name="leftDistortionParams">左相机畸变系数</param>
        /// <param name="rightDistortionParams">右相机畸变系数</param>
        /// <param name="filePath">保存的文件路径</param>
        public void SaveStereoCalibrationResult(HTuple leftCameraParams, HTuple rightCameraParams, HTuple relativePoseParams,
            HTuple leftDistortionParams, HTuple rightDistortionParams, string filePath)
        {
            // 创建 XML 文档对象
            XmlDocument xmlDoc = new XmlDocument();
            // 创建 XML 声明
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            // 创建根节点
            XmlElement root = xmlDoc.CreateElement("StereoCalibrationResult");
            xmlDoc.AppendChild(root);

            // 添加左相机内参、右相机内参、相对位姿参数、左相机畸变系数和右相机畸变系数到 XML 文档
            AddParameterElement(xmlDoc, root, "LeftCameraParams", leftCameraParams);
            AddParameterElement(xmlDoc, root, "RightCameraParams", rightCameraParams);
            AddParameterElement(xmlDoc, root, "RelativePoseParams", relativePoseParams);
            AddParameterElement(xmlDoc, root, "LeftDistortionParams", leftDistortionParams);
            AddParameterElement(xmlDoc, root, "RightDistortionParams", rightDistortionParams);

            try
            {
                // 保存 XML 文档到指定文件路径
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存双目标定结果失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存多目标定结果到 XML 文件
        /// </summary>
        /// <param name="cameraParamsList">每个相机的内参列表</param>
        /// <param name="poseParamsList">每个相机的位姿参数列表</param>
        /// <param name="filePath">保存的文件路径</param>
        public void SaveMultiCalibrationResult(List<HTuple> cameraParamsList,List<HTuple> poseParamsList, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF - 8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            XmlElement root = xmlDoc.CreateElement("MultiCalibrationResult");
            xmlDoc.AppendChild(root);

            for (int i = 0; i < cameraParamsList.Count; i++)
            {
                XmlElement cameraElement = xmlDoc.CreateElement($"Camera{i + 1}");
                root.AppendChild(cameraElement);

                AddParameterElement(xmlDoc, cameraElement, "CameraParams", cameraParamsList[i]);
                AddParameterElement(xmlDoc, cameraElement, "PoseParams", poseParamsList[i]);
            }

            try
            {
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存多目标定结果失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 辅助方法：向 XML 文档中添加参数元素
        /// </summary>
        /// <param name="xmlDoc">XML 文档对象</param>
        /// <param name="parentElement">父元素</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值（Halcon 的 HTuple 类型）</param>
        private void AddParameterElement(XmlDocument xmlDoc, XmlElement parentElement, string paramName, HTuple paramValue)
        {
            XmlElement paramElement = xmlDoc.CreateElement(paramName);
            parentElement.AppendChild(paramElement);

            for (int i = 0; i < paramValue.Length; i++)
            {
                XmlElement valueElement = xmlDoc.CreateElement($"Value{i + 1}");
                valueElement.InnerText = paramValue[i].D.ToString();
                paramElement.AppendChild(valueElement);
            }
        }
    }
}