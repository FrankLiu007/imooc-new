using IMOOC.EduCourses.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Public;

namespace IMOOC
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string State;
        LoginInfo loginInfo;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitSchoolNamesSp();
            if (InitLoginInfo())
            {
                InitRememberUsers();
            }
         
        }

        /// <summary>
        /// 读取LoginInfo.kimmie文件中的用户信息，读取成功返回true。
        /// 如果文件不存在则创建默认的LoginInfo，返回false。
        /// </summary>
        /// <returns></returns>
        private bool InitLoginInfo()
        {
            IFormatter serializer = new BinaryFormatter();
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\AllUsers\\LoginInfo.kimmie";
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(path);
                    Public.Log.FileOrFolder(LogType.CreatFolder, path);
                    loginInfo = new LoginInfo();
                    return false;
                }
                FileStream loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                loadFile.Position = 0;
                loginInfo = serializer.Deserialize(loadFile) as LoginInfo;
                loadFile.Flush();
                loadFile.Close();
            }
            catch(Exception ex)
            {
                Public.Log.WriterLog(" "+ ex.Message);
                loginInfo = new LoginInfo();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 有网络的情况下异步请求学校列表，并初始化学校选择列表。
        /// </summary>
        private void InitSchoolNamesSp()   
        {
            try
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    Task task = new Task(() =>
                    {
                        Public.MyHttpWebRequest.GetSchoolNames();
                    });
                    task.Start();

                    task.ContinueWith((t) =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {

                            SchoolNamesSp.Children.Clear();
                            foreach (var item in Public.MyHttpWebRequest.AllSchoolName)
                            {
                                TextBox school = new TextBox();
                                school.Text = item.Content;
                                school.ToolTip = item.Id;
                                school.PreviewMouseDown += School_PreviewMouseDown;
                                SchoolNamesSp.Children.Add(school);
                            }
                        }));
                    });
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("获取学校列表失败"+ ex.Message);
            }       
        }

        /// <summary>
        /// 根据LoginInfo.kimmie文件，初始化本地记住的用户信息。
        /// </summary>
        private void InitRememberUsers()
        {
            RememberCheckBox.IsChecked = loginInfo.IsRemember;
            CheckBoxResores(RememberCheckBox);
            AutomaticCheckBox.IsChecked = loginInfo.IsAutomatic;
            CheckBoxResores(AutomaticCheckBox);

            //初始化记住的账号和密码以及学校
            #region
            Account.Text = loginInfo.LastUserItem.UserName;
            if (loginInfo.LastUserItem.IsRemember)
            {
                passWordBox.Password = Public.HelperMethods.DESDecrypt(loginInfo.LastUserItem.PassWord,
                "IsMaggie", (Environment.UserName + "IsKimmie").Substring(0, 8));
            }
            else
            {
                passWordBox.Password = "";
            }           

            TextBox textBox = new TextBox() { Text = loginInfo.LastUserItem.UserName};
            textBox.PreviewMouseDown += UserName_PreviewMouseDown;
            UserNameSp.Children.Add(textBox);

            foreach (var item in loginInfo.UsersInfo)
            {
                int i = 0;
                if (item.UserName != loginInfo.LastUserItem.UserName && i < 4)
                {
                    TextBox userName = new TextBox();
                    userName.Text = item.UserName;
                    userName.PreviewMouseDown += UserName_PreviewMouseDown;
                    UserNameSp.Children.Add(userName);
                    i++;
                }
            }

            schoolNamesTextBox.Text = loginInfo.LastUserItem.SchoolName.Content;
            schoolNamesTextBox.ToolTip = loginInfo.LastUserItem.SchoolName.Id;
            #endregion
        }

        private void UserName_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string userName = (sender as TextBox).Text;
            Account.Text = userName;
            foreach (var item in loginInfo.UsersInfo)
            {
                if (item.UserName == userName)
                {
                    if (item.IsRemember)
                    {
                        passWordBox.Password = Public.HelperMethods.DESDecrypt(item.PassWord,
                        "IsMaggie", (Environment.UserName + "IsKimmie").Substring(0, 8));
                    } 
                    else
                    {
                        passWordBox.Password = "";
                    }                  

                    schoolNamesTextBox.Text = item.SchoolName.Content;
                    schoolNamesTextBox.ToolTip = item.SchoolName.Id;

                    AccountPp.IsOpen = false;
                    return;
                }
            }
            passWordBox.Password = "";
        }

        private void School_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            schoolNamesTextBox.Text = textBox.Text;
            schoolNamesTextBox.ToolTip = textBox.ToolTip;
            SchoolNamesPp.IsOpen = false; 
        }

        private void OfflineBtn_Click(object sender, RoutedEventArgs e)
        {
            UserInfoItem userInfoItem = new UserInfoItem()
            {
                UserName = "DefaultUser",
                PassWord = "",
                SchoolName = new DataItem()
                {
                    Content = "",
                    Id = -1
                }
            };
            MyHttpWebRequest.IsOnline = false;
            MyHttpWebRequest.CurrUser = userInfoItem;
            State = "offline";
            Close();
        }

        private void OnlineBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Account.Text == "学号 / 用户名" || Account.Text.ToString().Replace(" ", "") == "" || passWordBox.Password.Length == 0
                || schoolNamesTextBox.Text == "学校名称" || schoolNamesTextBox.Text.ToString().Replace(" ", "") == "")
            {
                LoginStateLable.Content = "请填写有效的登陆信息";
                return;
            }

            //连接网络登陆时进行网络验证，无网络登陆时进行本地验证。
            if ( System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                if (!Login_NetWork())
                {
                    return;
                }            
            }
            else
            {
                if (!Login_NoNetWork())
                {
                    return;
                }   
            }

            State = "online";
            Close();
        }

        /// <summary>
        /// 有网络时进行登陆验证
        /// </summary>
        private bool Login_NetWork()
        {
            //准备登陆参数
            #region
            string url = "http://shanyun.whalew.com/api_internal/api/login";
            string userName = Account.Text.ToString().Trim();
            string passWord = Public.HelperMethods.DESEncrypt(passWordBox.Password, "IsMaggie", (Environment.UserName + "IsKimmie").Substring(0, 8));
            string schoolId = schoolNamesTextBox.ToolTip.ToString();

            Dictionary<string, string> paramCourses = new Dictionary<string, string>
                {
                     { "username", userName },
                     { "password", passWordBox.Password },
                     { "school_id", schoolId }
                };
            #endregion

            //开始尝试登陆
            #region
            SignIn.Visibility = Visibility.Hidden;
            landing.Visibility = Visibility.Visible;

            string stata = Public.MyHttpWebRequest.Login(url, paramCourses);
            if (stata == "Error")
            {
                SignIn.Visibility = Visibility.Visible;
                landing.Visibility = Visibility.Hidden;
                LoginStateLable.Content = "登陆失败";
                return false;
            }
            else if (stata == "用户名不存在")
            {
                SignIn.Visibility = Visibility.Visible;
                landing.Visibility = Visibility.Hidden;
                LoginStateLable.Content = "用户名不存在";
                return false;
            }
            else if (stata == "用户名或密码错误")
            {
                SignIn.Visibility = Visibility.Visible;
                landing.Visibility = Visibility.Hidden;
                LoginStateLable.Content = "用户名或密码错误";
                return false;
            }
            else
            {
                UserInfoItem userInfoItem = new UserInfoItem()
                {
                    UserName = userName,
                    PassWord = passWord,
                    SchoolName = new DataItem()
                    {
                        Content = schoolNamesTextBox.Text.ToString().Trim(),
                        Id = Convert.ToInt32(schoolId)
                    },
                    IsRemember = loginInfo.IsRemember
                };

                MyHttpWebRequest.IsOnline = true;
                MyHttpWebRequest.CurrUser = userInfoItem;

                loginInfo.LastUserItem = userInfoItem;

                if (!CheckUserInfoItem(userInfoItem,true))
                {
                    loginInfo.UsersInfo.Add(userInfoItem);
                }
            }
            #endregion
            SavcLogInfo();
            return true;
        }

        /// <summary>
        /// 无网络时进行本地登陆验证
        /// </summary>
        private bool Login_NoNetWork()
        {
            //准备参数
            #region
            string userName = Account.Text.ToString();
            string passWord = Public.HelperMethods.DESEncrypt(passWordBox.Password,
                "IsMaggie", (Environment.UserName + "IsKimmie").Substring(0, 8));
            string schoolId = schoolNamesTextBox.ToolTip.ToString();

            UserInfoItem userInfoItem = new UserInfoItem()
            {
                UserName = userName,
                PassWord = passWord,
                SchoolName = new DataItem()
                {
                    Content = schoolNamesTextBox.Text.ToString().Trim(),
                    Id = Convert.ToInt32(schoolId)
                },
                IsRemember = loginInfo.IsRemember
            };
            #endregion

            foreach (var item in loginInfo.UsersInfo)
            {
                if (item.UserName==userInfoItem.UserName)
                {
                    if (item.PassWord!=userInfoItem.PassWord)
                    {
                        LoginStateLable.Content = "密码验证错误";
                        return false;
                    }
                    if (!item.SchoolName.Equals(userInfoItem.SchoolName))                       
                    {
                        LoginStateLable.Content = "学校验证错误";
                        return false; 
                    }
                    MyHttpWebRequest.IsOnline = false;
                    MyHttpWebRequest.CurrUser = userInfoItem;
                    return true;
                }
            }
            LoginStateLable.Content = "本地未保存当前用户名，请先在线登陆";
            return false;         
        }

        /// <summary>
        /// 检查是否已经存储过该账户
        /// </summary>
        /// <param name="userInfoItem"></param>
        /// <param name="isUpdate">是否使用userInfoIte更新存储的信息</param>
        /// <returns></returns>
        private bool CheckUserInfoItem(UserInfoItem userInfoItem,bool isUpdate=false)
        {
            for (int i = 0; i < loginInfo.UsersInfo.Count; i++)
            {
                if (loginInfo.UsersInfo[i].UserName==userInfoItem.UserName)
                {
                    if (isUpdate)
                    {
                        loginInfo.UsersInfo[i] = userInfoItem;
                    }                    
                    return true;
                }
            }
            return false;
        }

        private void SavcLogInfo()
        {
            try
            {
                IFormatter serializer = new BinaryFormatter();
                //开始序列化
                string path= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\IMOOC\\AllUsers\\LoginInfo.kimmie";
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(path);
                    Public.Log.FileOrFolder(LogType.CreatFolder, path);
                }
                FileStream saveFile = new FileStream(path, FileMode.Create, FileAccess.Write);
                serializer.Serialize(saveFile, loginInfo);
                saveFile.Flush();
                saveFile.Close();
            }
            catch (Exception ex)
            {                
                Public.Log.WriterLog("保存登录信息时发生错误"+ex.Message);              
            }
            
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SetIP.Visibility = Visibility.Collapsed;
            SignIn.Visibility = Visibility.Visible;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RememberPassword_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxResores(RememberCheckBox);
            loginInfo.IsRemember = (sender as CheckBox).IsChecked ?? false;           
        }

        private void AutomaticSign_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxResores(AutomaticCheckBox);
            loginInfo.IsAutomatic = (sender as CheckBox).IsChecked ?? false;
        }

        private void CheckBoxResores(CheckBox checkBox)
        {
            if ((bool)checkBox.IsChecked == true)
            {
                checkBox.Content = FindResource("TrueCheck");
            }
            else
            {
                checkBox.Content = FindResource("FalseCheck");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SignIn.Visibility = Visibility.Collapsed;
            SetIP.Visibility = Visibility.Visible;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            textbox.Foreground = Brushes.Black;
            if (textbox.Text == "学号/用户名" || textbox.Text=="学校名称")
            {
                textbox.Text = "";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AccountPp.IsOpen = true;
        }

        private void Label_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var lable = sender as Label;
            lable.Foreground = Brushes.LightBlue;
        }

        private void register_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var lable = sender as Label;
            lable.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0391Db"));
        }


        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SchoolNamesPp.IsOpen = true;
        }

        private void cancelLoginBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
