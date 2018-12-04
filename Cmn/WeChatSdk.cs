using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace NWJ.Cmn
{
    public class WeChatSdk
    {
        public static wxuser GetwxUser(string appid, string secret, string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + code + "&grant_type=authorization_code";
            return Get.GetJson<wxuser>(url);
        }
        public class wxuser
        {
            public string access_token;
            public int expires_in;
            public string refresh_token;
            public string openid;
            public string scope;
        }

        public static wxuserinfo GetwxUserInfo(string token, string openid)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + token + "&openid=" + openid + "&lang=zh_CN";
            return Get.GetJson<wxuserinfo>(url);
        }

        /// <summary>
        /// 用户微信资料
        /// </summary>
        public class wxuserinfo
        {
            public string openid;
            public string nickname;
            public string sex;
            public string province;
            public string city;
            public string country;
            public string headimgurl;
            public List<string> privilege;
            public string unionid;
        }
        public class WxJsonResult
        {
            public ReturnCode errcode { get; set; }
            public string errmsg { get; set; }

        }

        public enum ReturnCode
        {
            系统繁忙 = -1,
            请求成功 = 0,
            验证失败 = 40001,
            不合法的凭证类型 = 40002,
            不合法的OpenID = 40003,
            不合法的媒体文件类型 = 40004,
            不合法的文件类型 = 40005,
            不合法的文件大小 = 40006,
            不合法的媒体文件id = 40007,
            不合法的消息类型 = 40008,
            不合法的图片文件大小 = 40009,
            不合法的语音文件大小 = 40010,
            不合法的视频文件大小 = 40011,
            不合法的缩略图文件大小 = 40012,
            不合法的APPID = 40013,
            不合法的access_token = 40014,
            不合法的菜单类型 = 40015,
            不合法的按钮个数1 = 40016,
            不合法的按钮个数2 = 40017,
            不合法的按钮名字长度 = 40018,
            不合法的按钮KEY长度 = 40019,
            不合法的按钮URL长度 = 40020,
            不合法的菜单版本号 = 40021,
            不合法的子菜单级数 = 40022,
            不合法的子菜单按钮个数 = 40023,
            不合法的子菜单按钮类型 = 40024,
            不合法的子菜单按钮名字长度 = 40025,
            不合法的子菜单按钮KEY长度 = 40026,
            不合法的子菜单按钮URL长度 = 40027,
            不合法的自定义菜单使用用户 = 40028,
            不合法的oauth_code = 40029,
            不合法的refresh_token = 40030,
            缺少access_token参数 = 41001,
            缺少appid参数 = 41002,
            缺少refresh_token参数 = 41003,
            缺少secret参数 = 41004,
            缺少多媒体文件数据 = 41005,
            缺少media_id参数 = 41006,
            缺少子菜单数据 = 41007,
            access_token超时 = 42001,
            需要GET请求 = 43001,
            需要POST请求 = 43002,
            需要HTTPS请求 = 43003,
            多媒体文件为空 = 44001,
            POST的数据包为空 = 44002,
            图文消息内容为空 = 44003,
            多媒体文件大小超过限制 = 45001,
            消息内容超过限制 = 45002,
            标题字段超过限制 = 45003,
            描述字段超过限制 = 45004,
            链接字段超过限制 = 45005,
            图片链接字段超过限制 = 45006,
            语音播放时间超过限制 = 45007,
            图文消息超过限制 = 45008,
            接口调用超过限制 = 45009,
            创建菜单个数超过限制 = 45010,
            不存在媒体数据 = 46001,
            不存在的菜单版本 = 46002,
            不存在的菜单数据 = 46003,
            解析JSON_XML内容错误 = 47001,
            api功能未授权 = 48001,
            用户未授权该api = 50001,
            发送消息失败_48小时内用户未互动 = 10706,
            发送消息失败_该用户已被加入黑名单_无法向此发送消息 = 62751,
            发送消息失败_对方关闭了接收消息 = 10703,
            对方不是粉丝 = 10700
        }
    }
}



