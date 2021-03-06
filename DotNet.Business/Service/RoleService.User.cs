﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// RoleService
    /// 角色管理服务
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.1 JiRiGaLa 程序排版优化。
    ///		2012.03.27 版本：1.0 JiRiGaLa 分离程序。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class RoleService : IRoleService
    {
        /// <summary>
        /// 添加角色(同时添加用户，一个数据库事务里进行处理)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        public string AddWithUser(BaseUserInfo userInfo, BaseRoleEntity entity, string[] userIds, out string statusCode, out string statusMessage)
        {
            string result = string.Empty;

            string returnCode = string.Empty;
            string returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                // 这里是判断已经登录的用户是否有调用当前函数的权限，加强服务层被远程调用的安全性的
                string tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.Add(entity, out returnCode);
                if (!string.IsNullOrEmpty(result) && userIds != null && userIds.Length > 0)
                {
                    tableName = userInfo.SystemCode + "UserRole";
                    var userManager = new BaseUserManager(dbHelper, userInfo, tableName);
                    // 小心异常，检查一下参数的有效性
                    if (userIds != null)
                    {
                        userManager.AddToRole(userInfo.SystemCode, userIds, new string[] { result });
                    }
                }
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;

            return result;
        }


        /// <summary>
        /// 获得角色中的用户主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>用户主键</returns>
        public string[] GetRoleUserIds(BaseUserInfo userInfo, string roleId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                result = manager.GetUserIdsInRoleId(userInfo.SystemCode, roleId);
            });

            return result;
        }

        /// <summary>
        /// 用户添加到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="addUserIds">用户主键</param>
        /// <returns>影响行数</returns>
        public int AddUserToRole(BaseUserInfo userInfo, string roleId, string[] addUserIds)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (addUserIds != null)
                {
                    result += manager.AddToRole(userInfo.SystemCode, addUserIds, new string[] { roleId });
                }
            });

            return result;
        }

        /// <summary>
        /// 将用户从角色中移除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="userIds">用户主键</param>
        /// <returns>影响行数</returns>
        public int RemoveUserFromRole(BaseUserInfo userInfo, string roleId, string[] userIds)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                if (userIds != null)
                {
                    result += manager.RemoveFormRole(userInfo.SystemCode, userIds, new string[] { roleId });
                }
            });

            return result;
        }

        /// <summary>
        /// 清除角色用户关联
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int ClearRoleUser(BaseUserInfo userInfo, string roleId)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                result = manager.ClearUser(userInfo.SystemCode, roleId);
            });

            return result;
        }

        /// <summary>
        /// 设置角色中的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色</param>
        /// <param name="userIds">用户主键数组</param>
        /// <returns>影响行数</returns>
        public int SetUsersToRole(BaseUserInfo userInfo, string roleId, string[] userIds)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                result = manager.ClearUser(userInfo.SystemCode, roleId);
                // 小心异常，检查一下参数的有效性
                if (userIds != null)
                {
                    result += manager.AddToRole(userInfo.SystemCode, userIds, new string[] { roleId });
                }
            });

            return result;
        }

        /// <summary>
        /// 获取用户角色数据列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="targetUserId">目标角色</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByUser(BaseUserInfo userInfo, string targetUserId)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                string tableName = userInfo.SystemCode + "UserRole";
                var manager = new BaseUserManager(dbHelper, userInfo, tableName);
                string[] roleIds = manager.GetRoleIds(userInfo.SystemCode, targetUserId);

                tableName = userInfo.SystemCode + "Role";
                var roleManager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = roleManager.GetDataTable(BaseRoleEntity.FieldId, roleIds, BaseRoleEntity.FieldSortCode);
                result.TableName = BaseRoleEntity.TableName;
            });

            return result;
        }



        /// <summary>
        /// 获取角色的用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="companyId">网点主键</param>
        /// <param name="userId">用户主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页多少</param>
        /// <param name="orderBy">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetRoleUserDataTable(BaseUserInfo userInfo, string systemCode, string roleId, string companyId, string userId, string searchValue, out int recordCount, int pageIndex, int pageSize, string orderBy)
        {
            DataTable result = new DataTable(BaseOrganizeEntity.TableName);

            int myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleManager = new BaseRoleManager(userInfo);
                result = roleManager.GetUserDataTable(systemCode, roleId, companyId, userId, searchValue, out myRecordCount, pageIndex, pageSize, orderBy);
                result.TableName = BaseUserEntity.TableName;
            });
            recordCount = myRecordCount;

            return result;
        }
    }
}