﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// PermissionService
    /// 角色权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2015.12.17 版本：1.1 JiRiGaLa 增加 GetRolePermissionList 方法。
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.17</date>
    /// </author> 
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 角色权限关联关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        #region public List<BaseModuleEntity> GetRolePermissionList(BaseUserInfo userInfo, string roleId)
        /// <summary>
        /// 获取角色权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>权限列表</returns>
        public List<BaseModuleEntity> GetRolePermissionList(BaseUserInfo userInfo, string roleId)
        {
            List<BaseModuleEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseRolePermissionManager(dbHelper, userInfo);
                result = manager.GetPermissionList(userInfo.SystemCode, roleId);
            });

            return result;
        }
        #endregion

        #region public string[] GetRolePermissionIds(BaseUserInfo userInfo, string roleId)
        /// <summary>
        /// 获取角色权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRolePermissionIds(BaseUserInfo userInfo, string roleId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                var manager = new BaseRolePermissionManager(dbHelper, userInfo);
                result = manager.GetPermissionIds(userInfo.SystemCode, roleId);
			});

			return result;
        }
        #endregion

        #region public string[] GetRoleIdsByPermission(BaseUserInfo userInfo, string result)
        /// <summary>
        /// 获取角色主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result">操作权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIdsByPermission(BaseUserInfo userInfo, string permissionId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
			ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                var manager = new BaseRolePermissionManager(dbHelper, userInfo);
                result = manager.GetRoleIds(userInfo.SystemCode, permissionId);
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
			});

			return result;
        }
        #endregion

        #region public int GrantRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] grantPermissionIds)
        /// <summary>
        /// 授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public int GrantRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] grantPermissionIds)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "Permission";
				var manager = new BaseRolePermissionManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(roleIds != null && grantPermissionIds != null)
				{
					result += manager.Grant(userInfo.SystemCode, roleIds, grantPermissionIds);
				}
			});

			return result;
        }
        #endregion

        #region public string GrantRolePermissionById(BaseUserInfo userInfo, string roleId, string grantPermissionId)
        /// <summary>
        /// 授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>数据主键</returns>
        public string GrantRolePermissionById(BaseUserInfo userInfo, string roleId, string grantPermissionId)
        {
            string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                var manager = new BaseRolePermissionManager(dbHelper, userInfo);
				// 小心异常，检查一下参数的有效性
				if(grantPermissionId != null)
				{
					result = manager.Grant(userInfo.SystemCode, roleId, grantPermissionId);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] revokePermissionIds)
        /// <summary>
        /// 撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] revokePermissionIds)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "Permission";
				var manager = new BaseRolePermissionManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(roleIds != null && revokePermissionIds != null)
				{
					result += manager.Revoke(userInfo.SystemCode, roleIds, revokePermissionIds);
				}
			});

			return result;
        }
        #endregion

        #region public int ClearRolePermission(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 清除角色权限
        /// 
        /// 1.清除角色的用户归属。
        /// 2.清除角色的模块权限。
        /// 3.清除角色的操作权限。
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public int ClearRolePermission(BaseUserInfo userInfo, string id)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var userManager = new BaseUserManager(dbHelper, userInfo);
                result += userManager.ClearUser(userInfo.SystemCode, id);

                var rolePermissionManager = new BaseRolePermissionManager(dbHelper, userInfo);
				result += rolePermissionManager.RevokeAll(userInfo.SystemCode, id);

                var roleScopeManager = new BaseRoleScopeManager(dbHelper, userInfo);
                result += roleScopeManager.RevokeAll(userInfo.SystemCode, id);
			});

			return result;
        }
        #endregion

        #region public int RevokeRolePermissionById(BaseUserInfo userInfo, string roleId, string revokePermissionId)
        /// <summary>
        /// 撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeRolePermissionById(BaseUserInfo userInfo, string roleId, string revokePermissionId)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                var manager = new BaseRolePermissionManager(dbHelper, userInfo);
				// 小心异常，检查一下参数的有效性
				if(revokePermissionId != null)
				{
					result += manager.Revoke(userInfo.SystemCode, roleId, revokePermissionId);
				}
			});

			return result;
        }
        #endregion

        #region public string[] GetRoleScopeUserIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 28.获取角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleScopeUserIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetUserIds(userInfo.SystemCode, roleId, permissionCode);
			});

			return result;
        }
        #endregion

        #region public string[] GetRoleScopeRoleIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 28.获取角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleScopeRoleIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				result = manager.GetRoleIds(roleId, permissionCode);
			});

			return result;
        }
        #endregion

        #region public string[] GetRoleScopeOrganizeIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 获取角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleScopeOrganizeIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				result = manager.GetOrganizeIds(roleId, permissionCode);
			});

			return result;
        }
        #endregion

        #region public int GrantRoleUserScopes(BaseUserInfo userInfo, string roleId, string[] grantUserIds, string permissionCode)
        /// <summary>
        /// 29.授予角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantUserIds">授予用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantRoleUserScopes(BaseUserInfo userInfo, string roleId, string[] grantUserIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(grantUserIds != null)
				{
                    result += manager.GrantUsers(roleId, grantUserIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRoleUserScopes(BaseUserInfo userInfo, string roleId, string[] revokeUserIds, string permissionCode)
        /// <summary>
        /// 30.撤消角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeUserIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRoleUserScopes(BaseUserInfo userInfo, string roleId, string[] revokeUserIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(revokeUserIds != null)
				{
                    result += manager.RevokeUsers(roleId, revokeUserIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int GrantRoleRoleScopes(BaseUserInfo userInfo, string roleId, string[] grantRoleIds, string permissionCode)
        /// <summary>
        /// 31.授予角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantRoleIds">授予角色主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantRoleRoleScopes(BaseUserInfo userInfo, string roleId, string[] grantRoleIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(grantRoleIds != null)
				{
                    result += manager.GrantRoles(roleId, grantRoleIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRoleRoleScopes(BaseUserInfo userInfo, string roleId, string[] revokeRoleIds, string permissionCode)
        /// <summary>
        /// 32.撤消角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeRoleIds">撤消的角色主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRoleRoleScopes(BaseUserInfo userInfo, string roleId, string[] revokeRoleIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(revokeRoleIds != null)
				{
                    result += manager.RevokeRoles(roleId, revokeRoleIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int GrantRoleOrganizeScopes(BaseUserInfo userInfo, string roleId, string result, string[] grantOrganizeIds)
        /// <summary>
        /// 授予角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantOrganizeIds">授予组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantRoleOrganizeScopes(BaseUserInfo userInfo, string roleId, string[] grantOrganizeIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(grantOrganizeIds != null)
				{
					result += manager.GrantOrganizes(roleId, grantOrganizeIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRoleOrganizeScopes(BaseUserInfo userInfo, string roleId, string[] revokeOrganizeIds, string permissionCode)
        /// <summary>
        /// 撤消角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeOrganizeIds">撤消的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRoleOrganizeScopes(BaseUserInfo userInfo, string roleId, string[] revokeOrganizeIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
			ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(revokeOrganizeIds != null)
				{
                    result += manager.RevokeOrganizes(roleId, revokeOrganizeIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public string[] GetRoleScopePermissionIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 获取角色授权权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">操作权限项</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleScopePermissionIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
			ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				result = manager.GetPermissionIds(roleId, permissionCode);
			});

			return result;
        }
        #endregion

        #region public int GrantRolePermissionScopes(BaseUserInfo userInfo, string roleId, string[] grantPermissionIds)
        /// <summary>
        /// 授予角色的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantPermissionIds">授予的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantRolePermissionScopes(BaseUserInfo userInfo, string roleId, string[] grantPermissionIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseRoleScopeManager(dbHelper, userInfo);
				// 小心异常，检查一下参数的有效性
				if(grantPermissionIds != null)
				{
					result += manager.GrantPermissiones(roleId, grantPermissionIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRolePermissionScopes(BaseUserInfo userInfo, string roleId, string[] revokePermissionIds, string permissionCode)
        /// <summary>
        /// 授予角色的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionIds">撤消的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRolePermissionScopes(BaseUserInfo userInfo, string roleId, string[] revokePermissionIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				// 小心异常，检查一下参数的有效性
                if (revokePermissionIds != null)
                {
                    var manager = new BaseRoleScopeManager(dbHelper, userInfo);
                    result += manager.RevokePermissions(roleId, revokePermissionIds, permissionCode);
                }
			});

			return result;
        }
        #endregion

        #region public int ClearRolePermissionScope(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 清除角色权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">主键</param>
        /// <returns>数据表</returns>
        public int ClearRolePermissionScope(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				result = manager.ClearRolePermissionScope(roleId, permissionCode);
			});

			return result;
        }
        #endregion


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 角色模块关联关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取角色可以访问的模块主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleModuleIds(BaseUserInfo userInfo, string roleId)
        {
            return GetRoleScopeModuleIds(userInfo, roleId, "Resource.AccessPermission");
        }

        #region public string[] GetRoleScopeModuleIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 获取用户模块权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleScopeModuleIds(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				result = manager.GetModuleIds(roleId, permissionCode);
			});

			return result;
        }
        #endregion

        #region public int GrantRoleModuleScopes(BaseUserInfo userInfo, string roleId, string[] grantModuleIds, string permissionCode)
        /// <summary>
        /// 授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantModuleIds">授予模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantRoleModuleScopes(BaseUserInfo userInfo, string roleId, string[] grantModuleIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
			ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(grantModuleIds != null)
				{
                    result += manager.GrantModules(roleId, grantModuleIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public string GrantRoleModuleScope(BaseUserInfo userInfo, string roleId, string grantModuleId, string permissionCode)
        /// <summary>
        /// 授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantModuleId">授予模块主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string GrantRoleModuleScope(BaseUserInfo userInfo, string roleId, string grantModuleId, string permissionCode)
        {
            string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(grantModuleId != null)
				{
					result = manager.GrantModule(roleId, grantModuleId, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRoleModuleScopes(BaseUserInfo userInfo, string roleId, string[] revokeModuleIds, string permissionCode)
        /// <summary>
        /// 撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeModuleIds">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRoleModuleScopes(BaseUserInfo userInfo, string roleId, string[] revokeModuleIds, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(revokeModuleIds != null)
				{
                    result += manager.RevokeModules(roleId, revokeModuleIds, permissionCode);
				}
			});

			return result;
        }
        #endregion

        #region public int RevokeRoleModuleScope(BaseUserInfo userInfo, string roleId, string revokeModuleId, string permissionCode)
        /// <summary>
        /// 撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeModuleId">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeRoleModuleScope(BaseUserInfo userInfo, string roleId, string revokeModuleId, string permissionCode)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
                string tableName = userInfo.SystemCode + "PermissionScope";
				var manager = new BaseRoleScopeManager(dbHelper, userInfo, tableName);
				// 小心异常，检查一下参数的有效性
				if(revokeModuleId != null)
				{
					result += manager.RevokeModule(roleId, revokeModuleId, permissionCode);
				}
			});

			return result;
        }
        #endregion
    }
}