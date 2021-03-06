﻿//--------------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd. 
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseMessageManager
    /// 消息表
    /// 
    /// 修改记录
    /// 
    /// 2012-07-03 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-07-03</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseMessageManager()
        {
            if (base.dbHelper == null)
            {
                base.dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection);
            }
            base.CurrentTableName = BaseMessageEntity.TableName;
            base.PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseMessageManager(string tableName)
        {
            base.CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseMessageManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseMessageManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            base.CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主键</param>
        /// <returns>主键</returns>
        public string Add(BaseMessageEntity entity, bool identity = true, bool returnId = false)
        {
            this.Identity = identity;
            this.ReturnId = returnId;
            return this.AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseMessageEntity entity)
        {
            return this.UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseMessageEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseMessageEntity>(this.ExecuteReader(new KeyValuePair<string, object>(BaseMessageEntity.FieldId, id)));
            // return BaseEntity.Create<BaseMessageEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseMessageEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// 20151008 吉日嘎拉 把排序码去掉，排序码没有意义
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseMessageEntity entity)
        {
            string result = string.Empty;

            // 20151008 吉日嘎拉 消息的主键没必要自增，为了安全性高一些，直接用GUID就可以了，代码页简洁了，思路也简单了
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString("N");
            }

            result = entity.Id;

            var sqlBuilder = new SQLBuilder(DbHelper, this.Identity, this.ReturnId);
            sqlBuilder.BeginInsert(this.CurrentTableName, BaseMessageEntity.FieldId);
            sqlBuilder.SetValue(BaseMessageEntity.FieldId, entity.Id);

            this.SetObject(sqlBuilder, entity);

            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateDepartmentId, UserInfo.DepartmentId);
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateDepartmentName, UserInfo.DepartmentName);
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateCompanyId, entity.CreateCompanyId);
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateCompanyName, entity.CreateCompanyName);
            }
            else if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseMessageEntity.FieldCreateBy, entity.CreateBy);
            }

            sqlBuilder.SetDBNow(BaseMessageEntity.FieldCreateOn);

            sqlBuilder.EndInsert();

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateObject(BaseMessageEntity entity)
        {
            int result = 0;

            var sqlBuilder = new SQLBuilder(DbHelper);
            sqlBuilder.BeginUpdate(this.CurrentTableName);
            this.SetObject(sqlBuilder, entity);

            sqlBuilder.SetWhere(BaseMessageEntity.FieldId, entity.Id);
            result = sqlBuilder.EndUpdate();

            return result;
        }

        // 这个是声明扩展方法
        partial void SetObjectExpand(SQLBuilder sqlBuilder, BaseMessageEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="entity">实体</param>
        private void SetObject(SQLBuilder sqlBuilder, BaseMessageEntity entity)
        {
            SetObjectExpand(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseMessageEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReceiverDepartmentId, entity.ReceiverDepartmentId);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReceiverDepartmentName, entity.ReceiverDepartmentName);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReceiverId, entity.ReceiverId);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReceiverRealName, entity.ReceiverRealName);
            sqlBuilder.SetValue(BaseMessageEntity.FieldFunctionCode, entity.FunctionCode);
            sqlBuilder.SetValue(BaseMessageEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseMessageEntity.FieldObjectId, entity.ObjectId);
            sqlBuilder.SetValue(BaseMessageEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseMessageEntity.FieldContents, entity.Contents);
            sqlBuilder.SetValue(BaseMessageEntity.FieldEmail, entity.Email);
            sqlBuilder.SetValue(BaseMessageEntity.FieldQQ, entity.QQ);
            sqlBuilder.SetValue(BaseMessageEntity.FieldTelephone, entity.Telephone);
            sqlBuilder.SetValue(BaseMessageEntity.FieldIsNew, entity.IsNew);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReadCount, entity.ReadCount);
            sqlBuilder.SetValue(BaseMessageEntity.FieldReadDate, entity.ReadDate);
            sqlBuilder.SetValue(BaseMessageEntity.FieldTargetURL, entity.TargetURL);
            sqlBuilder.SetValue(BaseMessageEntity.FieldIPAddress, entity.IPAddress);
            sqlBuilder.SetValue(BaseMessageEntity.FieldDeletionStateCode, entity.DeletionStateCode);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            return this.Delete(new KeyValuePair<string, object>(BaseMessageEntity.FieldId, id));
        }
    }
}
