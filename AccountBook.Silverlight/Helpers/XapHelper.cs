using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml;

namespace AccountBook.Silverlight.Helpers
{
    public class XapHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private struct AssemblyPartInfo
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name
            {
                get;
                set;
            }
            /// <summary>
            /// dll相对路径
            /// </summary>
            public string Source
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 从XAP包中返回程序集信息
        /// </summary>
        /// <param name="packageStream">Xap Stream</param>
        /// <returns>入口程序集</returns>
        public static Assembly LoadAssemblyFromXap(Stream packageStream)
        {
            // 加载AppManifest.xaml
            Stream stream = Application.GetResourceStream(new StreamResourceInfo(packageStream, null), new Uri("AppManifest.xaml", UriKind.Relative)).Stream;
            XmlReader xmlReader = XmlReader.Create(stream);

            // 读取程序集信息
            Assembly entryAssembly = null;
            string entryAssemblyName = string.Empty;
            var assemblyPartInfos = new List<AssemblyPartInfo>();
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "Deployment")
                        {
                            // 入口程序集名称
                            entryAssemblyName = xmlReader.GetAttribute("EntryPointAssembly");
                        }
                        else if (xmlReader.Name == "AssemblyPart")
                        {
                            var name = xmlReader.GetAttribute("x:Name");
                            var source = xmlReader.GetAttribute("Source");

                            assemblyPartInfos.Add(new AssemblyPartInfo { Name = name, Source = source });
                        }
                        break;
                    default:
                        break;
                }
            }

            var assemblyPart = new AssemblyPart();
            // 加载程序集
            foreach (var assemblyPartInfo in assemblyPartInfos)
            {
                StreamResourceInfo streamInfo = Application.GetResourceStream(new StreamResourceInfo(packageStream, "application/binary"), new Uri(assemblyPartInfo.Source, UriKind.Relative));
                // 入口程序集
                if (assemblyPartInfo.Name == entryAssemblyName)
                {
                    entryAssembly = assemblyPart.Load(streamInfo.Stream);
                }
                // 其他程序集
                else
                {
                    assemblyPart.Load(streamInfo.Stream);
                }
            }

            return entryAssembly;
        }
    }
}