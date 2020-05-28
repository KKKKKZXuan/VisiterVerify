using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SqlClient
{
    public class SystemLog : System.Web.UI.Page
    {
        //日志文件夹
        public static readonly string logFilePath = ConfigurationManager.AppSettings["logfilepath"];

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志内容</param>
        public static void WriteLog(string log)
        {
            StreamWriter sw;
            string filename = System.DateTime.Now.Date.ToString("yyyyMMdd") + ".txt";
            string path = HttpContext.Current.Server.MapPath("~/" + logFilePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                sw = new StreamWriter(path + "/" + filename, true);
                sw.WriteLine();
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.UrlReferrer != null)
                    {
                        sw.WriteLine(HttpContext.Current.Request.UrlReferrer.AbsolutePath + "(" + HttpContext.Current.Request.UserHostAddress + ")--->" + HttpContext.Current.Request.Url);
                    }
                    else
                    {
                        sw.WriteLine("自行输入☞(" + HttpContext.Current.Request.UserHostAddress + ")--->" + HttpContext.Current.Request.Url);
                    }
                }
                sw.WriteLine(System.DateTime.Now.ToString());
                sw.WriteLine(log);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception e)
            {
                sw = new StreamWriter(path + "/(1)" + filename, true);
                sw.WriteLine(log);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }
    }
}
