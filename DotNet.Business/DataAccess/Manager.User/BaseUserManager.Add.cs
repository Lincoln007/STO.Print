﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public void BeforeAdd(BaseUserEntity entity) 用户添加之前执行的方法
        /// <summary>
        /// 用户添加之前执行的方法
        /// </summary>
        /// <param name="entity">用户实体</param>
        public void BeforeAdd(BaseUserEntity entity)
        {
            // 检测成功，可以添加用户
            this.StatusCode = Status.OKAdd.ToString();
            if (!string.IsNullOrEmpty(entity.UserName)
                    && this.Exists(new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, entity.UserName)
                , new KeyValuePair<string, object>(BaseUserEntity.FieldDeletionStateCode, 0)))
            {
                // 用户名已重复
                this.StatusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                // 检查编号是否重复
                if (!string.IsNullOrEmpty(entity.Code)
                    && this.Exists(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, entity.Code)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldDeletionStateCode, 0)))
                {
                    // 编号已重复
                    this.StatusCode = Status.ErrorCodeExist.ToString();
                }

                if (entity.IsStaff == 1)
                {
                    List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
                    parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, entity.UserName));
                    parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldDeletionStateCode, 0));
                    if (DbLogic.Exists(DbHelper, BaseStaffEntity.TableName, parameters))
                    {
                        // 编号已重复
                        this.StatusCode = Status.ErrorNameExist.ToString();
                    }
                    if (!string.IsNullOrEmpty(entity.Code))
                    {
                        parameters = new List<KeyValuePair<string, object>>();
                        parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldCode, entity.Code));
                        parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldDeletionStateCode, 0));
                        if (DbLogic.Exists(DbHelper, BaseStaffEntity.TableName, parameters))
                        {
                            // 编号已重复
                            this.StatusCode = Status.ErrorCodeExist.ToString();
                        }
                    }
                }
            }
        }
        #endregion

        #region public string Add(BaseUserEntity entity) 添加用户
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserEntity entity)
        {
            string result = string.Empty;

            this.BeforeAdd(entity);

            if (this.StatusCode == Status.OKAdd.ToString())
            {
                result = this.AddObject(entity);

                // 用户访问表里，插入一条记录
                BaseUserLogOnEntity userLogOnEntity = new BaseUserLogOnEntity();
                userLogOnEntity.Id = entity.Id;
                userLogOnEntity.CompanyId = entity.CompanyId;
                // 把一些默认值读取到，系统的默认值，这样增加用户时可以把系统的默认值带入
                userLogOnEntity.MultiUserLogin = BaseSystemInfo.CheckOnLine ? 0 : 1;
                userLogOnEntity.CheckIPAddress = BaseSystemInfo.CheckIPAddress ? 1 : 0;
                //此处设置密码强度级别
                userLogOnEntity.PasswordStrength = SecretUtil.GetUserPassWordRate(userLogOnEntity.UserPassword);
                // 若是系统需要用加密的密码，这里需要加密密码。
                if (BaseSystemInfo.ServerEncryptPassword)
                {
                    userLogOnEntity.UserPassword = this.EncryptUserPassword(entity.UserPassword);
                    // 安全通讯密码、交易密码也生成好
                    // userLogOnEntity.UserPassword = this.EncryptUserPassword(entity.CommunicationPassword);
                }

                new BaseUserLogOnManager(this.DbHelper, this.UserInfo).Add(userLogOnEntity);

                this.AfterAdd(entity);
            }

            return result;
        }
        #endregion
    }
}