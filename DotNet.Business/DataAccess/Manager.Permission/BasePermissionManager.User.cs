﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BasePermissionManager
    /// 资源权限管理，操作权限管理（这里实现了用户操作权限，角色的操作权限）
    /// 
    /// 修改记录
    ///
    ///     2010.09.21 版本：2.0 JiRiGaLa 智能权限判断、后台自动增加权限，增加并发锁PermissionLock。
    ///     2009.09.22 版本：1.1 JiRiGaLa 前台判断的权限，后台都需要记录起来，防止后台缺失前台的判断权限。
    ///     2008.03.28 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.03.28</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 获得有某个权限的所有用户主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string systemCode, string permissionCode)
        {
            // 若不存在就需要自动能增加一个操作权限项
            string permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            return GetUserIdsByPermissionId(systemCode, permissionId);
        }

        public string[] GetUserIdsByPermissionId(string systemCode, string permissionId)
        {
            DataTable dt = null;
            string[] result = null;
            if (!string.IsNullOrEmpty(permissionId))
            {
                string tableName = systemCode + "Permission";
                string sqlQuery = string.Empty;
                
                // 1.本人直接就有某个操作权限的。
                sqlQuery = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = 'BaseUser') AND (PermissionId = " + permissionId + ") AND (DeletionStateCode = 0) AND (Enabled = 1) ";
                dt = this.Fill(sqlQuery);
                string[] userIds = BaseBusinessLogic.FieldToArray(dt, BasePermissionEntity.FieldResourceId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 2.角色本身就有某个操作权限的。
                sqlQuery = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = 'BaseRole') AND (PermissionId = " + permissionId + ") AND (DeletionStateCode = 0) AND (Enabled = 1) ";
                dt = this.Fill(sqlQuery);
                string[] roleIds = StringUtil.Concat(result, BaseBusinessLogic.FieldToArray(dt, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 3.组织机构有某个操作权限。。
                sqlQuery = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = 'BaseOrganize') AND (PermissionId = " + permissionId + ") AND (DeletionStateCode = 0) AND (Enabled = 1) ";
                dt = this.Fill(sqlQuery);
                string[] organizeIds = StringUtil.Concat(result, BaseBusinessLogic.FieldToArray(dt, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 4.获取所有有这个操作权限的用户Id，而且这些用户是有效的。
                BaseUserManager userManager = new BaseUserManager(this.DbHelper, this.UserInfo);
                result = userManager.GetUserIds(userIds, organizeIds, roleIds);
            }
            return result;
        }
    }
}