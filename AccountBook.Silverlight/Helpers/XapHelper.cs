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
        /// 从XAP包中返回程序集信息
        /// </summary>
        /// <param name="packageStream">Xap Stream</param>
        /// <returns>Main Assembly</returns>
        public static Assembly LoadAssemblyFromXap(Stream packageStream)
        {
            Stream stream = Application.GetResourceStream(new StreamResourceInfo(packageStream, null), new Uri("AppManifest.xaml", UriKind.Relative)).Stream;
            Assembly mainAssembly = null;
            XmlReader xmlReader = XmlReader.Create(stream);

            var assemblyNames = new List<string>();
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "AssemblyPart")
                        {
                            var assemblyName = xmlReader.GetAttribute("Source");
                            assemblyNames.Add(assemblyName);
                        }
                        break;
                    default:
                        break;
                }
            }

            var assemblyPart = new AssemblyPart();
            foreach (var assemblyName in assemblyNames)
            {
                StreamResourceInfo streamInfo = Application.GetResourceStream(new StreamResourceInfo(packageStream, "application/binary"), new Uri(assemblyName, UriKind.Relative));
                if (mainAssembly == null)
                {
                    mainAssembly = assemblyPart.Load(streamInfo.Stream);
                }
                else
                {
                    assemblyPart.Load(streamInfo.Stream);
                }
            }

            return mainAssembly;
        }
    }
}
