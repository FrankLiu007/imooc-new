using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Collections.Specialized;
using IMOOC.EduCourses.Utils;

namespace Public
{
    static class MyHttpWebRequest
    {
        public static bool IsOnline { get; set; } = false;

        public static UserInfoItem CurrUser { get; set; } = new UserInfoItem();

        public static LoginResponse LoginResponse { get; set; }

        public static List<DataItem> AllSchoolName { get; set; } = new List<DataItem>(); 

        public static List<CourseAndChapter> WebCourses { get; set; } = new List<CourseAndChapter>();  

        private static CookieContainer _cookies;

        /// <summary>
        /// 登陆。
        /// param参数包括：
        /// username：用户名，
        /// password：密码，
        /// school_id：学校Id
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Login(string url, Dictionary<string, string> param)
        {
            try
            {
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.CookieContainer = new CookieContainer();
                req.ContentType = "application/x-www-form-urlencoded";
                #region 添加Post 参数  
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in param)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                _cookies = req.CookieContainer; //保存cookies            

                Stream stream = resp.GetResponseStream();
                //获取响应内容  
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                LoginResponse loginResponse = js.Deserialize<LoginResponse>(result);
                LoginResponse = loginResponse;
                return loginResponse.msg;

            }
            catch (Exception)
            {
                return "Error";
            }
        }

        /// <summary>
        /// 获取所有学校名称
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool GetSchoolNames()
        {
            try
            {
                string url = "http://shanyun.whalew.com/api_internal/api/schools";
                string Response = Response = Post(url);

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                SchoolNames allNames = js.Deserialize<SchoolNames>(Response);
                AllSchoolName.Clear();
                foreach (var item in allNames.data)
                {
                    AllSchoolName.Add(new DataItem()
                    { 
                        Id=item.id,
                        Content=item.name
                    });
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("获取所有学校名称时发生错误" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取课程对应的所有章节
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool GetChapter(Dictionary<string ,string> param,string course_id)
        {
            if (!IsOnline||_cookies==null)
            {
                return false;
            }
            try
            {
                string url = "http://shanyun.whalew.com/api_internal/api/lessons";
                string Response = Response = Post(url,param,_cookies);

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                Chapter chapter = js.Deserialize<Chapter>(Response);
                
                foreach (var item in chapter.data)
                {
                    for (int i = 0; i < WebCourses.Count; i++)
                    {
                        if (WebCourses[i].course.Id.ToString() == course_id)
                        {
                            WebCourses[i].chapter.Add(new DataItem()
                            {
                                Id=item.id,
                                Content=item.title
                            });
                            break;
                        }
                       
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("获取课程对应的所有章节时发生错误" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取该老师的所有课程
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool GetCourses(Dictionary<string, string> param)
        {
            if (!IsOnline || _cookies == null)
            {
                return false;
            }
            try
            {
                string url = "http://shanyun.whalew.com/api_internal/api/courses";
                string Response = Post(url, param, _cookies);

                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 52428800;
                WebCourses course = js.Deserialize<WebCourses>(Response);
                WebCourses.Clear();
                foreach (var item in course.data)
                {
                    WebCourses.Add(new CourseAndChapter()
                    {
                        course = new DataItem()
                        {
                            Id = item.id,
                            Content = item.title
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Public.Log.WriterLog("获取该老师的所有课程时发生错误" + ex.Message);
                return false;
            }
            return true;

        }

        //未完成
        public static void UploadCourse()
        {
            //示例
            var filePath = @"E:\1120.zip";
            var url = "http://shanyun.whalew.com/api_internal/api/upload-lesson";
            NameValueCollection aa = new NameValueCollection();
            aa.Add("lesson_id", "66");
            string[] files = { filePath };

            UploadFilesToRemoteUrl(url, files, aa);
        }

        /// <summary>
        /// 上传文件方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files">上传的文件</param>
        /// <param name="formFields">附带的参数</param>
        /// <returns></returns>
        public static string UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" +
                                    boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Timeout = 600000;
            request.CookieContainer = _cookies;

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "--");


            string formdataTemplate = "\r\n--" + boundary +
                                        "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "FileData", files[i]);// FileData 文件域
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (var response = request.GetResponse())
            {
                Stream stream2 = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                return reader2.ReadToEnd();
            }
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="param">附带的参数</param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> param = null, CookieContainer cookies = null)
        {
            try
            {
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                if (cookies != null)
                {
                    req.CookieContainer = cookies;
                }
                #region 添加Post 参数  
                if (param != null)
                {
                    StringBuilder builder = new StringBuilder();
                    int i = 0;
                    foreach (var item in param)
                    {
                        if (i > 0)
                            builder.Append("&");
                        builder.AppendFormat("{0}={1}", item.Key, item.Value);
                        i++;
                    }
                    byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                    req.ContentLength = data.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }
                }
                #endregion
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                Stream stream = resp.GetResponseStream();
                //获取响应内容  
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }


    // 数据结构类
    #region
    [Serializable]
    public class DataItem
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }       
            DataItem dataItem = obj as DataItem;
            return (Id == dataItem.Id) && (Content==dataItem.Content);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public class StatusBase
    {
        public int status { get; set; }
        public string msg { get; set; }
    }


    public sealed class SchoolNames: StatusBase
    {    
        public List<schoolNmaeItem> data { get; set; }
    }

    public sealed class schoolNmaeItem
    {
        public int id { get; set; }
        public string name { get; set; }
    }


    public sealed class LoginResponse: StatusBase
    {
        public LoginResponseItem data { get; set; }
    }

    public sealed class LoginResponseItem
    {
        public int uid { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public bool is_teacher { get; set; }
        public int notice_num { get; set; }
    }


    public sealed class Chapter : StatusBase
    {
        public List<ChapterItem> data { get; set; }
    }

    public sealed class ChapterItem
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public sealed class WebCourses : StatusBase
    {
        public List<CourseItem> data { get; set; }
    }

    public sealed class CourseItem
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public sealed class CourseAndChapter
    {
        public CourseAndChapter()
        {
            course = new DataItem();
            chapter = new List<DataItem>();
        }
        public DataItem course { get; set; }
        public List<DataItem> chapter { get; set; }
    }


    #endregion
}
