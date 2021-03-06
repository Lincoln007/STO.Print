﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// MessageService
    /// 消息服务
    /// 
    /// 修改记录
    /// 
    ///		2013.11.26 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.11.26</date>
    /// </author> 
    /// </summary>
    public partial class MessageService : IMessageService
    {
        #region public string[] GetUserByOrganize(BaseUserInfo userInfo, string companyId, string departmentId = null) 按组织机构获取用户列表
        /// <summary>
        /// 按组织机构获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">组织机构主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public string[] GetUserByOrganize(BaseUserInfo userInfo, string companyId, string departmentId = null)
        {
            string[] result = null;

            //var parameter = ServiceParameter.CreateWithMessage(userInfo
            //    , MethodBase.GetCurrentMethod()
            //    , this.serviceName
            //    , AppMessage.MessageService_GetUserDTByDepartment);

            //ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            //{
            result = BaseMessageManager.GetUserByOrganizeByCache(companyId, departmentId);
            //});

            return result;
        }
        #endregion

        #region public string[] GetUserDTByRole(BaseUserInfo userInfo, string roleId) 按角色获取用户列表
        /// <summary>
        /// 按角色获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>数据表</returns>
        public string[] GetUserByRole(BaseUserInfo userInfo, string roleId)
        {
            string[] result = null;

            var dt = new DataTable(BaseUserEntity.TableName);
            
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                string sqlQuery = "SELECT " + BaseUserEntity.FieldId + "," + BaseUserEntity.FieldRealName
                                + " FROM " + BaseUserEntity.TableName;
                sqlQuery += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeletionStateCode + " = 0 "
                            + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1  "
                            + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = 1 ) ";

                if (!String.IsNullOrEmpty(roleId))
                {
                    // 从用户读取用户
                    sqlQuery += " AND " + BaseUserEntity.FieldId + " IN ("
                            + "SELECT " + BaseUserRoleEntity.FieldUserId
                            + "   FROM " + BaseUserRoleEntity.TableName
                            + "  WHERE " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldDeletionStateCode + " = 0  "
                            + "       AND " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldEnabled + " = 1  "
                            + "       AND " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldRoleId + " = '" + roleId + "') ";
                }
                sqlQuery += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;

                dt = userManager.Fill(sqlQuery);
                dt.TableName = BaseUserEntity.TableName;

                List<string> list = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr[BaseUserEntity.FieldId].ToString() + "=" + dr[BaseUserEntity.FieldRealName].ToString());
                }
                result = list.ToArray();
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 获取部门用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        public DataTable GetDepartmentUser(BaseUserInfo userInfo)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);

                string sqlQuery = "SELECT " + BaseUserEntity.FieldId
                                          + "," + BaseUserEntity.FieldRealName
                                          + "," + BaseUserEntity.FieldCompanyName
                                          + "," + BaseUserEntity.FieldDepartmentName
                                 + "  FROM " + BaseUserEntity.TableName
                                 + " WHERE " + BaseUserEntity.FieldCompanyName + " = '" + userInfo.CompanyName + "'"
                                        + " AND " + BaseUserEntity.FieldDepartmentName + " = '" + userInfo.DepartmentName + "'"
                                        + " AND " + BaseUserEntity.FieldDeletionStateCode + " = 0 "
                                        + " AND " + BaseUserEntity.FieldEnabled + " = 1  "
                                        + " AND " + BaseUserEntity.FieldIsVisible + " = 1 "
                                  + " ORDER BY " + BaseUserEntity.FieldSortCode;
                result = userManager.Fill(sqlQuery);
                result.TableName = BaseUserEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获取公司用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        public DataTable GetCompanyUser(BaseUserInfo userInfo)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);

                string sqlQuery = "SELECT " + BaseUserEntity.FieldId
                                          + "," + BaseUserEntity.FieldRealName
                                          + "," + BaseUserEntity.FieldCompanyName
                                          + "," + BaseUserEntity.FieldDepartmentName
                                 + "  FROM " + BaseUserEntity.TableName
                                 + " WHERE " + BaseUserEntity.FieldCompanyName + " = '" + userInfo.CompanyName + "'"
                                        + " AND " + BaseUserEntity.FieldDeletionStateCode + " = 0 "
                                        + " AND " + BaseUserEntity.FieldEnabled + " = 1  "
                                        + " AND " + BaseUserEntity.FieldIsVisible + " = 1 "
                                 + " ORDER BY " + BaseUserEntity.FieldSortCode;
                result = userManager.Fill(sqlQuery);
                result.TableName = BaseUserEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获取最近联系人
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        public DataTable GetRecentContacts(BaseUserInfo userInfo)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                /*
                var manager = new BaseMessageRecentManager(dbHelper, userInfo);
                string sqlQuery = string.Empty;
                if (dbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlQuery = "SELECT " + BaseMessageRecentEntity.FieldTargetId + " AS " + BaseUserEntity.FieldId
                                   + "," + BaseMessageRecentEntity.FieldRealName
                                   + "," + BaseMessageRecentEntity.FieldCompanyName
                                   + "," + BaseMessageRecentEntity.FieldDepartmentName
                            + "  FROM " + BaseMessageRecentEntity.TableName
                            + " WHERE " + BaseMessageRecentEntity.FieldUserId + "= '" + userInfo.Id + "'"
                            + " AND ROWNUM < = 50 "
                            + " ORDER BY " + BaseMessageRecentEntity.FieldModifiedOn + " DESC ";
                }
                else
                {
                    sqlQuery = "SELECT TOP 50 " + BaseMessageRecentEntity.FieldTargetId + " AS " + BaseUserEntity.FieldId
                                   + "," + BaseMessageRecentEntity.FieldRealName
                                   + "," + BaseMessageRecentEntity.FieldCompanyName
                                   + "," + BaseMessageRecentEntity.FieldDepartmentName
                            + "  FROM " + BaseMessageRecentEntity.TableName
                            + " WHERE " + BaseMessageRecentEntity.FieldUserId + "= '" + userInfo.Id + "'"
                            + " ORDER BY " + BaseMessageRecentEntity.FieldModifiedOn + " DESC ";
                }
                */

                // 2015-09-27 吉日嘎拉 最新联系人方式，从缓存服务器里读取
                result = new DataTable(BaseUserEntity.TableName);
                result.Columns.Add(BaseUserEntity.FieldId.ToUpper());
                result.Columns.Add(BaseUserEntity.FieldRealName.ToUpper());
                result.Columns.Add(BaseUserEntity.FieldCompanyName.ToUpper());
                result.Columns.Add(BaseUserEntity.FieldDepartmentName.ToUpper());
                result.Columns.Add(BaseUserEntity.FieldSortCode.ToUpper());

# if Redis
                // 2015-11-20 吉日嘎拉 为了让程序能编译通过
                using (var redisClient = PooledRedisHelper.GetMessageClient())
                {
                    List<string> list = redisClient.GetAllItemsFromSortedSetDesc(("r" + userInfo.Id));
                    for (int i = 0; i < list.Count && i < 100; i++)
                    {
                        BaseUserEntity userEntity = BaseUserManager.GetObjectByCache(list[i]);
                        if (userEntity != null)
                        {
                            DataRow dr = result.NewRow();
                            dr[BaseUserEntity.FieldId] = userEntity.Id;
                            dr[BaseUserEntity.FieldRealName] = userEntity.RealName;
                            dr[BaseUserEntity.FieldCompanyName] = userEntity.CompanyName;
                            dr[BaseUserEntity.FieldDepartmentName] = userEntity.DepartmentName;
                            dr[BaseUserEntity.FieldSortCode] = i;
                            result.Rows.Add(dr);
                        }
                    }
                }
#endif
                result.AcceptChanges();
            });

            return result;

            /*
            SELECT ReceiverId AS UserId, MAX(CreateOn) AS CreateOn
                FROM BaseMessage
                WHERE FunctionCode = 'UserMessage'
                 AND (CreateUserId = 121)
                GROUP BY ReceiverId
                ORDER BY MAX(CreateOn)


            SELECT CreateUserId AS UserId, MAX(CreateOn) AS CreateOn
            FROM BaseMessage
            WHERE FunctionCode = 'UserMessage'
             AND (ReceiverId = 121)
            GROUP BY CreateUserId
            ORDER BY MAX(CreateOn)
            */
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="search">查询</param>
        /// <returns>用户列表</returns>
        public DataTable Search(BaseUserInfo userInfo, string search)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                userManager.ShowUserLogOnInfo = false;
                userManager.SelectFields = BaseUserEntity.FieldId
                                          + "," + BaseUserEntity.FieldRealName
                                          + "," + BaseUserEntity.FieldCompanyName
                                          + "," + BaseUserEntity.FieldDepartmentName;
                result = userManager.Search(userInfo.SystemCode, string.Empty, search, null, true, null, string.Empty);
                result.TableName = BaseUserEntity.TableName;
            });
            return result;
        }

        /*
        public DataTable GetExternalUser(BaseUserInfo userInfo, string id)
        {
            BaseMessageManager manager = new BaseMessageManager(userInfo);
            return manager.GetExternalUser(id);
        }
        */
    }
}