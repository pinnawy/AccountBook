using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace ElpSim.Library.Common
{
    /// <summary>
    /// 压缩解压 Zip 类，使用 SharpZipLib
    ///     jhzhang 2008/03/07 Creation
    /// </summary>
    public class Zipper
    {
        public static bool MakeZipFile(string[] filenames, string zipFile)
        {
            // Zip up the files - From SharpZipLib Demo Code
            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFile)))
            {
                s.SetLevel(9); // 0-9, 9 being the highest level of compression

                byte[] buffer = new byte[4096];

                foreach (string file in filenames)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);

                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);

                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();

                return true;
            }

        }
        public static void PackFiles(string folder,string zipFile)
        {
            using (ZipOutputStream zos = new ZipOutputStream(File.Create(zipFile)))
            {
                string dir = Path.GetDirectoryName(folder);
                addZipEntry(dir,folder, zos);
                zos.Finish();
                zos.Close();
            }
        }
        public static void PackFiles(string[] folders, string zipFile)
        {
            using (ZipOutputStream zos = new ZipOutputStream(File.Create(zipFile)))
            {
                foreach (string folder in folders)
                {
                    string dir = Path.GetDirectoryName(folder);
                    addZipEntry(dir, folder, zos);
                }
                zos.Finish();
                zos.Close();
            }
        }
        private static  void addZipEntry(string dir,string PathStr, ZipOutputStream zos)
        {
            
            DirectoryInfo di = new DirectoryInfo(PathStr);
            foreach (DirectoryInfo item in di.GetDirectories())
            {
                addZipEntry(dir,item.FullName, zos);
            }
            foreach (FileInfo item in di.GetFiles())
            {
                FileStream fs = File.OpenRead(item.FullName);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string strEntryName = ShortFileName(item.FullName,dir);
                ZipEntry entry = new ZipEntry(strEntryName);
                entry.Size = fs.Length;
                zos.PutNextEntry(entry);
                zos.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }  
        private static string ShortFileName(string fullname,string dir)
        {
            string fileName=fullname.Replace(dir,"");
            if (fileName.StartsWith("\\"))
                fileName = fileName.TrimStart(new char[] { '\\'});
            return fileName;
        }

        public static void UnZip(string zipFile, string dir)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile));
            
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = dir;
                string fileName = Path.GetFileName(theEntry.Name);

                //生成解压目录
                Directory.CreateDirectory(directoryName);

                if (fileName != String.Empty)
                {
                    //解压文件到指定的目录
                    string desDir = Path.Combine(dir, fileName);
                    FileStream streamWriter = File.Create(desDir);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();
                }
            }
            s.Close();
        }//end UnZip

        /**/
        /// <summary>
        /// Create a zip archive.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="directory">The directory to zip.</param> 
        public static void PackFiles(string filename, string directory, string filter)
        {
            try
            {
                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, true, filter);
                fz = null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /**/
        /// <summary>
        /// Unpacks the files.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>if succeed return true,otherwise false.</returns>
        public static bool UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(file)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {

                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);

                        if (directoryName != String.Empty)
                            Directory.CreateDirectory(dir + directoryName);

                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(dir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                streamWriter.Close();
                            }
                        }
                    }

                    s.Close();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Unpacks the files.
        /// update by yuwang 2011-3-17
        /// </summary>
        /// <param name="byteArry">The Zip file byte[].</param>
        /// <param name="dir">Unpack folder.</param>
        /// <param name="withFolder">Unpack with folder</param>
        public static void UnpackFiles(byte[] byteArry, string dir, bool withFolder = true)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var ms = new MemoryStream(byteArry))
            using (var s = new ZipInputStream(ms))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    //folder
                    if (theEntry.IsDirectory )
                    {
                        if(!withFolder) continue;

                        Directory.CreateDirectory(Path.Combine(dir, theEntry.Name));
                    }
                    //file
                    else
                    {
                        string fileName = withFolder ? theEntry.Name : Path.GetFileName(theEntry.Name);
                        if (string.IsNullOrEmpty(fileName))
                        {
                            throw new Exception("filename shouldn't be null or empty");
                        }
                        using (FileStream streamWriter = File.Create(Path.Combine(dir, fileName)))
                        {
                            var data = new byte[2048];
                            while (true)
                            {
                                int size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                } //end while
            }
        }

        /**/
        /// <summary>
        /// Unpacks the files.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>if succeed return true,otherwise false.</returns>
        public static bool UnpackFiles(string file, string dir, string password, string strGuid)
        {
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(file)))
                {
                    if (password != null && password.Length > 0)
                    {
                        s.Password = password;
                    }
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {

                        string directoryName = strGuid;//Path.GetDirectoryName(strGuid);//theEntry.Name
                        string fileName = Path.GetFileName(theEntry.Name);

                        if (directoryName != String.Empty)
                            Directory.CreateDirectory(dir + directoryName);

                        if (fileName != String.Empty)
                        {
                            string[] str = null;
                            string strFile = string.Empty;


                            //str = theEntry.Name.Split('/'); //old
                            //if (str[1].Contains(".xml"))
                            //{
                            //    str[1] = strGuid + ".xml";
                            //    strFile = str[1].ToString();
                            //}
                            //update by mswu  2010.5.17----
                            if (theEntry.Name.IndexOf('/') != -1)
                            {
                                str = theEntry.Name.Split('/');
                                if (str[1].Contains(".xml"))
                                {
                                    str[1] = strGuid + ".xml";
                                    strFile = str[1].ToString();
                                }
                            }
                            else {
                                strFile = theEntry.Name;
                                if (strFile.Contains(".xml")) {
                                    strFile = strGuid + ".xml";
                                }
                            }
                            //-----------
                            using (FileStream streamWriter = File.Create(dir + strGuid + "/" + strFile))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                streamWriter.Close();
                            }
                        }
                    }

                    s.Close();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
