﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseUserScopeManager
    /// 用户模块权限域
    /// 
    /// 修改记录
    ///
    ///     2011.03.13 版本：2.0 JiRiGaLa 重新整理代码。
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.03.13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetModuleIds(string userId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="userId">员工主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetModuleIds(string systemCode, string userId, string permissionCode)
        {
            string[] result = null;
            
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.TableName));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode)));

            var dt = this.GetDataTable(parameters);
            result = BaseBusinessLogic.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return result;
        }
        #endregion

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantModule(BasePermissionScopeManager manager, string id, string userId, string grantModuleId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantModule(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string grantModuleId, string permissionCode)
        {
            string result = string.Empty;
            BasePermissionScopeEntity resourcePermissionScopeEntity = new BasePermissionScopeEntity();
            string permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (string.IsNullOrEmpty(permissionId))
            {
                return string.Empty;
            }
            resourcePermissionScopeEntity.PermissionId = permissionId;
            resourcePermissionScopeEntity.ResourceCategory = BaseUserEntity.TableName;
            resourcePermissionScopeEntity.ResourceId = userId;
            resourcePermissionScopeEntity.TargetCategory = BaseModuleEntity.TableName;
            resourcePermissionScopeEntity.TargetId = grantModuleId;
            resourcePermissionScopeEntity.Enabled = 1;
            resourcePermissionScopeEntity.DeletionStateCode = 0;
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantModule(string userId, string grantModuleId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="result">权限主键</param>
        /// <param name="organizeId">权组织机构限主键</param>
        /// <returns>主键</returns>
        public string GrantModule(string systemCode, string userId, string grantModuleId, string permissionCode)
        {
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            return this.GrantModule(permissionScopeManager, systemCode, userId, grantModuleId, permissionCode);
        }
        #endregion

        public int GrantModules(string systemCode, string userId, string[] grantModuleIds, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < grantModuleIds.Length; i++)
            {
                this.GrantModule(permissionScopeManager, systemCode, userId, grantModuleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        public int GrantModules(string systemCode, string[] userIds, string grantModuleId, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < userIds.Length; i++)
            {
                this.GrantModule(permissionScopeManager, systemCode, userIds[i], grantModuleId, permissionCode);
                result++;
            }
            return result;
        }

        public int GrantModules(string systemCode, string[] userIds, string[] grantModuleIds, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < userIds.Length; i++)
            {
                for (int j = 0; j < grantModuleIds.Length; j++)
                {
                    this.GrantModule(permissionScopeManager, systemCode, userIds[i], grantModuleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeModule(BasePermissionScopeManager manager, string userId, string revokeModuleId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeModule(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string revokeModuleId, string permissionCode)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.TableName));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeModuleId));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode)));
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeModule(string userId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeModule(string systemCode, string userId, string revokeModuleId, string permissionCode)
        {
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            return this.RevokeModule(permissionScopeManager, systemCode, userId, revokeModuleId, permissionCode);
        }
        #endregion

        public int RevokeModules(string systemCode, string userId, string[] revokeModuleIds, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < revokeModuleIds.Length; i++)
            {
                this.RevokeModule(permissionScopeManager, systemCode, userId, revokeModuleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        public int RevokeModules(string systemCode, string[] userIds, string revokeModuleId, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < userIds.Length; i++)
            {
                this.RevokeModule(permissionScopeManager, systemCode, userIds[i], revokeModuleId, permissionCode);
                result++;
            }
            return result;
        }

        public int RevokeModules(string systemCode, string[] userIds, string[] revokeModuleIds, string permissionCode)
        {
            int result = 0;
            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, this.CurrentTableName);
            for (int i = 0; i < userIds.Length; i++)
            {
                for (int j = 0; j < revokeModuleIds.Length; j++)
                {
                    this.RevokeModule(permissionScopeManager, systemCode, userIds[i], revokeModuleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}