using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWJ.Cmn
{
    public class NEnums
    {
        /// <summary>
        /// 统一管理操作枚举
        /// </summary>
        public enum ActionEnum
        {
            /// <summary>
            /// 所有
            /// </summary>
            All,
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 查看
            /// </summary>
            View,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Edit,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 回复
            /// </summary>
            Reply,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel,
            /// <summary>
            /// 作废
            /// </summary>
            Invalid,
            /// <summary>
            /// 生成
            /// </summary>
            Build,
            /// <summary>
            /// 安装
            /// </summary>
            Instal,
            /// <summary>
            /// 卸载
            /// </summary>
            UnLoad,
            /// <summary>
            /// 登录
            /// </summary>
            Login,
            /// <summary>
            /// 备份
            /// </summary>
            Back,
            /// <summary>
            /// 还原
            /// </summary>
            Restore,
            /// <summary>
            /// 替换
            /// </summary>
            Replace,
            /// <summary>
            /// 复制
            /// </summary>
            Copy
        }

        public enum MemorialType
        {
            /// <summary>
            ///     公共馆
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            publictype = 1,
            /// <summary>
            ///     私有馆
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            privatetype = 2
        }
        /// <summary>
        ///     登录类型
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public enum login_type
        {
            /// <summary>
            ///     网页
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            web = 1,
            /// <summary>
            ///     安卓
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            android = 2,
            /// <summary>
            ///     ios
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            ios = 3,
            /// <summary>
            ///     手机WEB
            /// </summary>
            /// <remarks>
            ///     
            /// </remarks>
            mobileweb = 4
        }
        /// <summary>
        ///     隐私
        /// </summary>
        /// <remarks>
        ///     
        /// </remarks>
        public enum privacy
        {
            publics = 1,//公开
            firend = 2,//盆友圈
            privates = 3//个人
        }
        public enum stype {
            simple=5,
            doubles=6,
        }
    }
}
