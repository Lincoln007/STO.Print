﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseUserContactManager
    /// 系统用户联系方式表
    ///
    /// 修改记录
    ///
    ///		2014-01-13 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-01-13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserContactManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserContactManager()
        {
            if (base.dbHelper == null)
            {
                base.dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(base.CurrentTableName))
            {
                base.CurrentTableName = BaseUserContactEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserContactManager(string tableName): this()
        {
            base.CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserContactManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            base.CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserContactManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            base.CurrentTableName = tableName;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetObject(int? id)
        {
            return BaseEntity.Create<BaseUserContactEntity>(this.ExecuteReader(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserContactEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseUserContactEntity>(this.ExecuteReader(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserContactEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseUserContactEntity entity)
        {
            SQLBuilder sqlBuilder = new SQLBuilder(DbHelper, this.Identity, this.ReturnId);
            sqlBuilder.BeginInsert(this.CurrentTableName, BaseUserContactEntity.FieldId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldId, entity.Id);
            this.SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDBNow(BaseUserEntity.FieldCreateOn);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldModifiedUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldModifiedBy, UserInfo.RealName);
            }
            sqlBuilder.SetDBNow(BaseUserEntity.FieldModifiedOn);
            sqlBuilder.EndInsert();
            return entity.Id;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateObject(BaseUserContactEntity entity)
        {
            SQLBuilder sqlBuilder = new SQLBuilder(DbHelper);
            sqlBuilder.BeginUpdate(this.CurrentTableName);
            this.SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldModifiedUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldModifiedBy, UserInfo.RealName);
            }
            sqlBuilder.SetDBNow(BaseUserEntity.FieldModifiedOn);
            sqlBuilder.SetWhere(BaseUserEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="entity">实体</param>
        private void SetObject(SQLBuilder sqlBuilder, BaseUserContactEntity entity)
        {
            // 2016-03-02 吉日嘎拉 增加按公司可以区别数据的功能。
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldExtension, entity.Extension);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobile, entity.Mobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShowMobile, entity.ShowMobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileValiated, entity.MobileValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileVerificationDate, entity.MobileVerificationDate);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldTelephone, entity.Telephone);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShortNumber, entity.ShortNumber);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWW, entity.WW);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldQQ, entity.QQ);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChat, entity.WeChat);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatOpenId, entity.WeChatOpenId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatValiated, entity.WeChatValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYY, entity.YY);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmergencyContact, entity.EmergencyContact);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYiXin, entity.YiXin);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYiXinValiated, entity.YiXinValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyMail, entity.CompanyMail);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmail, entity.Email);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmailValiated, entity.EmailValiated);
        }
    }
}
