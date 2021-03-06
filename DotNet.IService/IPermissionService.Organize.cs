﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.ServiceModel;

namespace DotNet.IService
{
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
        /// 组织机构权限关联关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// 获取组织机构权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetOrganizePermissionIds(BaseUserInfo userInfo, string organizeId);

        /// <summary>
        /// 获取组织机构主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        string[] GetOrganizeIdsByPermission(BaseUserInfo userInfo, string permissionId);

        /// <summary>
        /// 授予组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="grantPermissionId">授予权限</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        string GrantOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string grantPermissionId);

        /// <summary>
        /// 授予组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int GrantOrganizePermissions(BaseUserInfo userInfo, string[] organizeIds, string[] grantPermissionIds);

        /// <summary>
        /// 撤消组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeOrganizePermissions(BaseUserInfo userInfo, string[] organizeIds, string[] revokePermissionIds);

        /// <summary>
        /// 撤消组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        int RevokeOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string revokePermissionId);

        /// <summary>
        /// 清除权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        [OperationContract]
        int ClearOrganizePermission(BaseUserInfo userInfo, string id);


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 用户组织机构范围权限（省市县区域）关联相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        PermissionOrganizeScope GetUserOrganizeScope(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        string SetUserOrganizeScope(BaseUserInfo userInfo, string userId, PermissionOrganizeScope permissionOrganizeScope, string permissionCode);

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 用户组织机构范围权限（省市县区域）关联相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        [OperationContract]
        PermissionOrganizeScope GetRoleOrganizeScope(BaseUserInfo userInfo, string roleId, string permissionCode);

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        [OperationContract]
        string SetRoleOrganizeScope(BaseUserInfo userInfo, string roleId, PermissionOrganizeScope permissionOrganizeScope, string permissionCode);
    }
}