using Public;
using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace IMOOC.EduCourses.Utils
{
    public static class UserAppConfigStatic
    {
        public static string CourseName { get; set; } = "MyCourse1";

        public static bool PenMode { get; set; } = false;

        public static bool BoardMode { get; set; } = true;

        public static string SavePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\" + MyHttpWebRequest.CurrUser.UserName + "\\";

        public static bool IsDebugInfoOn { get; set; } = true;

        public static int Port { get; set; } = 8911;

        /// <summary>
        /// true:只支持笔写
        /// </summary>
        public static bool IsStylusOnly { get; set; } = false;
    }

    public class UserAppConfig
    {
        public string CourseName { get; set; }

        public bool PenMode { get; set; }

        public bool BoardMode { get; set; }

        public string SavePath { get; set; }

        public bool IsDebugInfoOn { get; set; }

        public int Port { get; set; }

        public bool IsStylusOnly { get; set; }

        public UserAppConfig()
        {
            CourseName = "MyCourse1";
            PenMode = false;
            BoardMode = true;
            SavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\" + MyHttpWebRequest.CurrUser.UserName + "\\";
            IsDebugInfoOn = true;
            IsStylusOnly = false;
            Port = 8911;
        }

        /// <summary>
        /// 将UserAppConfig对象的值赋给UserAppConfigStatic静态对象
        /// </summary>
        public void UserAppConfigToUserAppConfigStatic()
        {
            UserAppConfigStatic.CourseName = CourseName;
            UserAppConfigStatic.PenMode = PenMode;
            UserAppConfigStatic.BoardMode = BoardMode;
            UserAppConfigStatic.SavePath = SavePath;
            UserAppConfigStatic.IsDebugInfoOn = IsDebugInfoOn;
            UserAppConfigStatic.IsStylusOnly = IsStylusOnly;
            UserAppConfigStatic.Port = Port;
        }

        /// <summary>
        /// 将UserAppConfigStatic静态对象的值赋值给该UserAppConfig对象
        /// </summary>
        public void UserAppConfigStaticToUserAppConfig()
        {
            CourseName = UserAppConfigStatic.CourseName;
            PenMode = UserAppConfigStatic.PenMode;
            BoardMode = UserAppConfigStatic.BoardMode;
            SavePath = UserAppConfigStatic.SavePath;
            IsDebugInfoOn = UserAppConfigStatic.IsDebugInfoOn;
            IsStylusOnly = UserAppConfigStatic.IsStylusOnly;
            Port = UserAppConfigStatic.Port;
        }

    }

}
