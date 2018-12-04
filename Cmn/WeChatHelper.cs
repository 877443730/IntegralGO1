using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

namespace NWJ.Cmn
{
    public class WeChatHelper
    {
        /// <summary>
        /// 使用Post方法获取字符串结果，常规提交
        /// </summary>
        /// <returns></returns>
       
        public class FileHelper
        {
            public static FileStream GetFileStream(string fileName)
            {
                FileStream fileStream = null;
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    fileStream = new FileStream(fileName, FileMode.Open);
                }
                return fileStream;
            }
        }
      

    }
}
