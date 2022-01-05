using Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMOOC.EduCourses.Utils
{
    [Serializable]
    class LoginInfo
    {
        /// <summary>
        /// 最后一次登陆的用户信息
        /// </summary>
        public UserInfoItem LastUserItem { get; set; }

        public bool IsRemember { get; set; }

        public bool IsAutomatic { get; set; }     

        /// <summary>
        /// 本地所有记住的账号密码。
        /// </summary>
        public List<UserInfoItem> UsersInfo { get; set; }       

        public LoginInfo()
        {
            LastUserItem = new UserInfoItem();
            IsRemember = false;
            IsAutomatic = false;
            UsersInfo = new List<UserInfoItem>();
        }

        public bool RemoveUser(UserInfoItem item) => UsersInfo.Remove(item);

        public void RemoveUser(int index) => UsersInfo.RemoveAt(index);

        //public bool SigninUser(string userName)
        //{
        //    LastName = userName;
        //    return true;
        //}
    }

    [Serializable]
    public class UserInfoItem
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public DataItem SchoolName { get; set; }

        public bool IsRemember { get; set; } = false;
    } 

}
