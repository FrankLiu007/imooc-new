using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace Public
{
    static class Register
    {
        public static bool IsRegister { get; set; } = false;

        private static string AllID { get; set; } = null;

        public static bool CheckRegister(string serialNumber=null)
        {
            if (serialNumber == null)
            {
                serialNumber = GetSerialNumber();
                if (serialNumber == "")
                {
                    IsRegister = false;
                    return false;
                }
            }             
            
            if (AllID==null)
            {
                AllID = GetAllID();
            }
            
            string Decrypt = HelperMethods.DESDecrypt(serialNumber,
                "IsMaggie", "IsKimmie");

            if (Decrypt==AllID)
            {
                IsRegister = true;
                return true;
            }

            IsRegister = false;
            return false;
        }

        static public bool RegisterIMOOC(string serialNumber)
        {
            if (CheckRegister(serialNumber))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教\\AppConfig\\";
                string fileName = @"register.txt";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Public.Log.FileOrFolder(LogType.CreatFolder, path);
                }

                FileStream fs = new FileStream(path + fileName, FileMode.Create);
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(serialNumber);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                return true;
            }


            return false;
        }

        static string GetSerialNumber()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\善而教\\AppConfig\\";
            string fileName = @"register.txt";
            if (!File.Exists(path + fileName))
            {
                return "";
            }
            StreamReader sr = new StreamReader(path + fileName, Encoding.Default);
            string SerialNumber = sr.ReadLine();
            sr.Close();
            return SerialNumber;

        }

        /// <summary>
        /// 返回CPU 硬盘 网卡ID，并去除字母与数字外的其他字符
        /// </summary>
        /// <returns></returns>
        public static string GetAllID()
        {
            string id = (GetCpuID()+GetDiskID()).ToUpper();
            ArrayList ID = new ArrayList();
            foreach (var item in id)
            {
                if (Char.IsNumber(item) || Char.IsLetter(item))
                {
                    ID.Add(item.ToString());
                }
            }

            string allID= string.Join("", (string[])ID.ToArray(typeof(string)));
            if (allID.Length>=24)
            {
                return allID.Substring(0, 24);
            }
            else if (allID.Length>=16)
            {
                return allID.Substring(0, 16);
            }
            else if (allID.Length >= 8)
            {
                return allID.Substring(0, 8);
            }
            else
            {
                return allID = "";
            }           
        }

        static string GetCpuID()
        {
            try
            {
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
        }

        static string GetDiskID()
        {
            try
            {
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
        }

        static string GetMacAddress()
        {
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
        }




    }
}
