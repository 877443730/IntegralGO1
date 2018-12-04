using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NWJ.Cmn
{
    public class LogHelper
    {
        /// <summary>
        /// 系统异常记录
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public static void WriteLogHelper(string type, object obj)
        {
            System.DateTime currentTime = System.DateTime.Now;
            string strYMD = currentTime.ToString("yyyy-MM-dd");
            string FILE_NAME = "系统异常记录" + strYMD + ".txt";//每天按照日期建立一个不同的文件名  
            StreamWriter sr;
            string strPath = HttpContext.Current.Server.MapPath("~/Resource/Abnormaldirectory/");
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            if (File.Exists("" + strPath + "" + FILE_NAME))
            {
                sr = File.AppendText("" + strPath + "" + FILE_NAME);
            }
            else
            {
                sr = File.CreateText("" + strPath + "" + FILE_NAME);
            }
            sr.WriteLine(type + "   " + obj + "　  " + DateTime.Now + "\r\n");
            sr.Close();
        }
        /// <summary>
        /// 数据库异常记录
        /// </summary>
        /// <param name="obj"></param>
        public void writeInfos(object obj)
        {
            System.DateTime currentTime = System.DateTime.Now;
            string strYMD = currentTime.ToString("yyyy-MM-dd");
            string FILE_NAME = "数据库异常记录" + strYMD + ".txt";//每天按照日期建立一个不同的文件名  
            StreamWriter sr;
            string strPath = HttpContext.Current.Server.MapPath("~/Resource/Abnormaldirectory/");
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            
            if (File.Exists("" + strPath + "" + FILE_NAME))
                sr = File.AppendText("" + strPath + "" + FILE_NAME);
            else
                sr = File.CreateText("" + strPath + "" + FILE_NAME);
            sr.WriteLine(obj + "　  " + DateTime.Now + "\r\n");
            sr.Close();
        }
    }
}
