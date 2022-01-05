using IMOOC.EduCourses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Public
{
    static class Log
    {
        static public bool isDebugInfoOn = true; 

        static private string inspectLogFile()
        {
            string savePath = @"D:\IMOOC\error_log.txt";
            string saveFolder = @"D:\IMOOC";
            if (!Directory.Exists(saveFolder))
            {
                try
                {
                    Directory.CreateDirectory(saveFolder);
                }
                catch (Exception)
                {
                   savePath = @"C:\IMOOC\error_log.txt";
                   saveFolder = @"C:\IMOOC";
                    Directory.CreateDirectory(saveFolder);
                }
               
            }
            if (!File.Exists(savePath))
            {
                File.Create(savePath);
            }
            return savePath;

        }

        static public void WriterLog(string errorContent, string courseName = "Default Name", string coursePath = "Default Path")
        {
            if (isDebugInfoOn == false)
            {
                return;
            }
            string savePath = inspectLogFile();
            string MethodName = new StackTrace(true).GetFrame(1).GetMethod().Name;

            try
            {
                FileStream fs = new FileStream(savePath, FileMode.Append, FileAccess.Write);
                var sw = new StreamWriter(fs);

                sw.WriteLine("          ");
                sw.WriteLine("-----------------------CourseName-----------------------" + courseName + "--------------------------");
                sw.WriteLine("  CourseSavePath      " + coursePath);
                sw.WriteLine("  Time                " + DateTime.Now);
                sw.WriteLine("  Type                Error");
                sw.WriteLine("  MethodName          " + MethodName);
                sw.WriteLine("          ");
                sw.WriteLine("  Content");
                sw.Write("  " + errorContent);
                sw.WriteLine("          ");
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch
            {
            }
           
        }


        static public void FileOrFolder(LogType type, string target, string courseName = "Default Name", string coursePath = "Default Path")
        {
            if (isDebugInfoOn == false)
            {
                return;
            }
            string savePath = inspectLogFile();
            string MethodName = new StackTrace(true).GetFrame(1).GetMethod().Name;
            string OperationType = type.ToString();

            try
            {
                FileStream fs = new FileStream(savePath, FileMode.Append, FileAccess.Write);
                var sw = new StreamWriter(fs);

                sw.WriteLine("          ");
                sw.WriteLine("-----------------------CourseName-----------------------" + courseName + "--------------------------");
                sw.WriteLine("  CourseSavePath      " + coursePath);
                sw.WriteLine("  Time                " + DateTime.Now);
                sw.WriteLine("  Type                " + OperationType);
                sw.WriteLine("  MethodName          " + MethodName);
                sw.WriteLine("          ");
                sw.WriteLine("  Content");
                sw.Write("Target Folder is  " + target);
                sw.WriteLine("          ");
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch 
            {
                
            }        
        }
    }

     public enum LogType
    {
        CreatFolder=1,
        DeleteFolder = 2,
        CreatFile=3,
        DeleteFile=4, 
        CopeFile=5      
    }

}
