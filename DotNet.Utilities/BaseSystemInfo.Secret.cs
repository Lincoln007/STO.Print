﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

namespace DotNet.Utilities
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 密码错误锁定次数
        /// </summary>
        public static int PasswordErrorLockLimit = 10;

        /// <summary>
        /// 连续输入N次密码后，密码错误锁定周期(分钟)
        /// 0 表示 需要系统管理员进行审核，帐户直接被设置为无效
        /// </summary>
        public static int PasswordErrorLockCycle = 5;

        /// <summary>
        /// 是否区分大小写
        /// </summary>
        public static bool MatchCase = true;

        /// <summary>
        /// 最多获取数据的行数限制
        /// </summary>
        public static int TopLimit = 200;

        /// <summary>
        /// 强类型密码管理
        /// </summary>
        public static bool CheckPasswordStrength = true;

        /// <summary>
        /// 服务器端加密存储密码
        /// </summary>
        public static bool ServerEncryptPassword = true;

        /// <summary>
        /// 密码最小长度
        /// </summary>
        public static int PasswordMiniLength = 6;

        /// <summary>
        /// 必须字母+数字组合
        /// </summary>
        public static bool NumericCharacters = true;

        /// <summary>
        /// 密码修改周期(月)
        /// </summary>
        public static int PasswordChangeCycle = 3;

        /// <summary>
        /// 用户名最小长度
        /// </summary>
        public static int AccountMinimumLength = 2;

        /// <summary>
        /// 系统加密Key
        /// </summary>
        public static string SecurityKey = "ZhJingXi.XuXia.Zto.com";

        /// <summary>
        /// 遠端調用Service使用者名稱（提高系統安全性）
        /// </summary>
        public static string ServiceUserName = "JiRiGaLa";

        /// <summary>
        /// 遠端調用Service密碼（提高系統安全性）
        /// </summary>
        public static string ServicePassword = "JiRiGaLa";

        /// <summary>
        /// 白名单
        /// 121.40.33.143 电商的ip服务器位置
        /// </summary>
        public static string WhiteList = "116.228.70.118,116.228.70.122,180.166.232.222,58.246.115.34,183.195.130.78,121.40.33.143";

        /// <summary>
        /// 黑名单
        /// </summary>
        public static string BlackList = "";
    }
}