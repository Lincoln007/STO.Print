﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel;

namespace DotNet.IService
{
    using DotNet.Model;
    using DotNet.Utilities;
    
    /// <summary>
    /// IPermissionService
    /// 与权限判断等相关的接口定义
    /// 
    /// 修改记录
    /// 
    ///		2012.03.22 版本：1.0 JiRiGaLa 添加权限。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial interface IPermissionService
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 用户关联模块关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        [OperationContract]
        string[] GetPermissionIds(BaseUserInfo userInfo, string systemCode, string userId, bool fromCache);

        /// <summary>
        /// 59.获得用户有权限的模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fromCache">从缓存读取</param>
        /// <returns>数据表</returns>
        [OperationContract]
        List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo, bool fromCache);

        /// <summary>
        /// 60.获得用户有权限的模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="fromCache">从缓存读取</param>
        /// <returns>模块列表</returns>
        [OperationContract]
        List<BaseModuleEntity> GetPermissionListByUser(BaseUserInfo userInfo, string systemCode, string userId, string companyId, bool fromCache);

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 用户权限关联关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// 40.获取用户权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserPermissionIds(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 40.获取用户主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserIdsByPermission(BaseUserInfo userInfo, string permissionId);

        /// <summary>
        /// 41.授予用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] grantPermissionIds);

        /// <summary>
        /// 42.授予用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        string GrantUserPermissionById(BaseUserInfo userInfo, string userId, string grantPermissionId);

        /// <summary>
        /// 43.撤消用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] revokePermissionIds);

        /// <summary>
        /// 44.撤消用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionId">撤消权限</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserPermissionById(BaseUserInfo userInfo, string userId, string revokePermissionId);

        /// <summary>
        /// 45.获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserScopeOrganizeIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 46.设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizeIds">授予的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserOrganizeScopes(BaseUserInfo userInfo, string userId, string[] grantOrganizeIds, string permissionCode);

        /// <summary>
        /// 47.设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizeIds">撤消的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserOrganizeScopes(BaseUserInfo userInfo, string userId, string[] revokeOrganizeIds, string permissionCode);

        /// <summary>
        /// 48.获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserScopeUserIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode);

        /// <summary>
        /// 49.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserIds">授予的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserUserScopes(BaseUserInfo userInfo, string userId, string[] grantUserIds, string permissionCode);

        /// <summary>
        /// 50.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserUserScopes(BaseUserInfo userInfo, string userId, string[] revokeUserIds, string permissionCode);

        /// <summary>
        /// 51.获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserScopeRoleIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode);

        /// <summary>
        /// 52.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantRoleIds">授予的角色主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserRoleScopes(BaseUserInfo userInfo, string systemCode, string userId, string[] grantRoleIds, string permissionCode);

        /// <summary>
        /// 53.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserRoleScopes(BaseUserInfo userInfo, string userId, string[] revokeRoleds, string permissionCode);

        /// <summary>
        /// 54.获取用户授权权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserScopePermissionIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 55.授予用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionIds">授予的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] grantPermissionIds, string permissionCode);

        /// <summary>
        /// 56.撤消用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionIds">撤消的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] revokePermissionIds, string permissionCode);

        /// <summary>
        /// 57.清除用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int ClearUserPermission(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 58.清除用户权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int ClearUserPermissionScope(BaseUserInfo userInfo, string id, string permissionCode);

        /// <summary>
        /// 61.获取用户模块权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetUserScopeModuleIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 62.授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleId">授予模块主键</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        string GrantUserModuleScope(BaseUserInfo userInfo, string userId, string grantModuleId, string permissionCode);

        /// <summary>
        /// 63.授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleIds">授予模块主键数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantUserModuleScopes(BaseUserInfo userInfo, string userId, string[] grantModuleIds, string permissionCode);

        /// <summary>
        /// 64.撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleId">撤消模块主键数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserModuleScope(BaseUserInfo userInfo, string userId, string revokeModuleId, string permissionCode);

        /// <summary>
        /// 65.撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleIds">撤消模块主键数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeUserModuleScopes(BaseUserInfo userInfo, string userId, string[] revokeModuleIds, string permissionCode);

        /// <summary>
        /// 获取用户权限树
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>用户主键</returns>
        [OperationContract]
        string[] GetPermissionTreeUserIds(BaseUserInfo userInfo, string userId, string permissionCode);
    }
}