using IMOOC.EduCourses.Utils;
using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Public
{
     static class HelperMethods
    {
        public static T GetAncestor<T>(this DependencyObject element)
        {
            while (!(element == null || element is T))
            {
                element = VisualTreeHelper.GetParent(element);
            }


            if ((element != null) && (element is T))
            {
                return (T)(object)element;
            }

            return default(T);
        }


        //查找父控件
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

       static public void ZipFolder(string sourcePath, string targetFile)
        {
            //var name = Path.GetFileName(sourcePath);
            using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
            {
                zip.AddDirectory(sourcePath, Path.GetFileName(sourcePath));
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.Save(targetFile);
            }
            //string str = targetFile.Replace(".Zip", ".zh");
            //File.Move(targetFile, str);
        }

        static public void UnZipFolder(string sourceFile, string targetPath)
        {

            try
            {
                using (ZipFile zip1 = ZipFile.Read(sourceFile))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "解压.ZH文件时发生错误！");
                message.ShowDialog();
                Public.Log.WriterLog("解压.ZH文件时发生错误！" + ex.Message);
            }
            
            
        }

        //复制文件夹
        static public void CopyFolder(string sourceFolder, string destFolder,bool overwrite=false)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                    Public.Log.FileOrFolder(Public.LogType.CreatFolder, destFolder);
                }
                if (!Directory.Exists(sourceFolder))
                {
                    return;
                }              
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    try
                    {
                        File.Copy(file, dest, overwrite);
                        //Public.Log.FileOrFolder(Public.LogType.CopeFile, file + "  to  " + dest);
                    }
                    catch (Exception ex)
                    {
                        Public.Log.WriterLog("复制文件时发生错误。存在同名文件   " + ex.Message
                                            + "    source   " + file +"   dest   "+dest);
                    }                                       

                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    //if (folder== Path.Combine(sourceFolder, "temporary"))
                    //{
                    //    break;
                    //}
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "移动PPT文件时发生错误!");
                message.ShowDialog();
                Public.Log.WriterLog(ex.Message + "移动PPT文件时发生错误！"+ "sourceFolder = "+ sourceFolder+ ";destFolder = "+ destFolder);            
            }
           
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool DeleteFolder(string path)
        {
            if (Directory.Exists(path) == false)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "将要删除文件夹的路径不存在");
                message.ShowDialog();
                Log.FileOrFolder(LogType.DeleteFolder, path+ "    将要删除文件夹的路径不存在");
                return false;
            }
            if (path == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            {
                MessageWin message = new MessageWin(MessageWinType.Error, "删除路径为整个我的文档");
                message.ShowDialog();
                Log.FileOrFolder(LogType.DeleteFolder, path + "    删除路径为整个我的文档");
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.ToString().Contains("Boot"))
                        {
                            DeleteFolder(dir.ToString() + "\\" + item.ToString());
                            //Log.FileOrFolder(LogType.DeleteFolder, dir.ToString() + "\\" + item.ToString());
                        }
                        else
                        {
                            MessageWin message = new MessageWin(MessageWinType.Error, "删除路径包含敏感字符");
                            message.ShowDialog();
                            Log.WriterLog("删除路径包含敏感字符");
                        }
                    }
                }
                Directory.Delete(path,true);
                Log.FileOrFolder(LogType.DeleteFolder, path);

                return true;
            }
            catch (Exception)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "删除文件时发生错误");
                message.ShowDialog();
                Log.FileOrFolder(LogType.DeleteFolder, path + "    删除文件时发生错误");
                return false;
            }

        }

        static public bool KillProcess(string processName)
        {
            Process[] myProgress;
            myProgress = Process.GetProcesses();          //获取当前启动的所有进程
            foreach (Process p in myProgress)            //关闭当前启动的Excel进程
            {
                if (p.ProcessName.ToUpper() == processName.ToUpper())          //通过进程名来寻找
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception)
                    {
                        Public.Log.WriterLog("关闭node_shanyun进程失败");
                    }
                    Public.Log.WriterLog("关闭node_shanyun进程成功");
                    return true;
                }
            }
            return false;
        }

        static public bool CheckProcess(string processName)
        {
            Process[] myProgress; 
            myProgress = Process.GetProcesses();          //获取当前启动的所有进程
            foreach (Process p in myProgress)            //关闭当前启动的Excel进程
            {
                if (p.ProcessName.ToUpper() == processName.ToUpper())          //通过进程名来寻找
                {
                    return true;
                }
            }
            return false;
        }

        static public bool CheckIMOOCIsOpen()
        {
            int Number = 0;
            Process[] myProgress;
            myProgress = Process.GetProcesses();          //获取当前启动的所有进程
            foreach (Process p in myProgress)            //关闭当前启动的Excel进程
            {
                if (p.ProcessName.ToUpper() == "IMOOC".ToUpper())          //通过进程名来寻找
                {
                    Number++;
                }
            }

            if (Number==2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 压缩图片分辨率
        /// </summary>
        /// <param name="sFile">原图片的完整路径</param>
        /// <param name="dPath">目标位置的完整路径</param>
        /// <param name="flag">图片的压缩比例</param>
        /// <returns></returns>
        public static bool CompressPic(string sFile, string dPath, int flag)
        { 
            if (!File.Exists(sFile))
            {
                return false;
            }
            string path1 = Path.GetDirectoryName(dPath);
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
            }

            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            System.Drawing.Imaging.ImageFormat tFormat = iSource.RawFormat;
            //以下代码为保存图片时，设置压缩质量 
            System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100 
            System.Drawing.Imaging.EncoderParameter eParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();//获取系统内置的编码器
                System.Drawing.Imaging.ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];//查找出JPEG编码器
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    iSource.Save(dPath, jpegICIinfo, ep);//dFile是压缩后的新路径                     
                }
                else
                {
                    iSource.Save(dPath, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
            }
        }

        //DES加密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="originalValue">明文内容</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string DESEncrypt(string originalValue, string key, string iv)
        {
            if (originalValue == "")
            {
                return "";
            }
            using (DESCryptoServiceProvider sa
                = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform ct = sa.CreateEncryptor())
                {
                    byte[] by = Encoding.UTF8.GetBytes(originalValue);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct,
                                                         CryptoStreamMode.Write))
                        {
                            cs.Write(by, 0, by.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }


        //DES解密
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="encryptedValue">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string DESDecrypt(string encryptedValue, string key, string iv)
        {
            if (encryptedValue=="")
            {
                return "";
            }
            try
            {
                using (DESCryptoServiceProvider sa =
                new DESCryptoServiceProvider
                { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
                {
                    using (ICryptoTransform ct = sa.CreateDecryptor())
                    {
                        byte[] byt = Convert.FromBase64String(encryptedValue);

                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                            {
                                cs.Write(byt, 0, byt.Length);
                                cs.FlushFinalBlock();
                            }
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog(ex.Message);
                return "";
            }
            
        }

        public static bool IsOffice2003Above()
        {
            RegistryKey rk = Registry.LocalMachine;
            //office2016
            RegistryKey office2016 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\16.0\\PowerPoint\\InstallRoot\\");
            //office2013
            RegistryKey office2013 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\15.0\\PowerPoint\\InstallRoot\\");
            //office2010
            RegistryKey office2010 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\14.0\\PowerPoint\\InstallRoot\\");
            //office2007
            RegistryKey office2007 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\12.0\\PowerPoint\\InstallRoot\\");
            //office 2003
            RegistryKey office2003 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\11.0\\PowerPoint\\InstallRoot\\");
            //office xp
            RegistryKey officexp = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\10.0\\PowerPoint\\InstallRoot\\");
            //office 2000
            RegistryKey office2000 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\9.0\\PowerPoint\\InstallRoot\\");
            //office 98
            RegistryKey office98 = rk.OpenSubKey(@"SOFTWARE\\Microsoft\\Office\\8.0\\PowerPoint\\InstallRoot\\");

            //检查本机是否安装Office2003及以下版本
            if (office98!=null)
            {
                string file = office98.GetValue("Path").ToString();
                if (File.Exists(file + "Excel.exe"))
                {
                    return false;
                }
            }
            else if (office2000!=null)
            {
                string file = office2000.GetValue("Path").ToString();
                if (File.Exists(file + "Excel.exe"))
                {
                    return false;
                }
            }
            else if (officexp != null)
            {
                string file = officexp.GetValue("Path").ToString();
                if (File.Exists(file + "Excel.exe"))
                {
                    return false;
                }
            }
            else if (office2003 != null)
            {
                string file = office2003.GetValue("Path").ToString();
                if (File.Exists(file + "Excel.exe"))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
