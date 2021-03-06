﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseModuleManager
    /// 模块(菜单)类（程序OK）
    /// 
    /// 修改记录
    /// 
    ///     2015.12.10 版本：3.1 JiRiGaLa   增加缓存功能。
    ///     2008.05.19 版本：3.0 JiRiGaLa   将模块访问权限进行完善，按两种权限分配机制。
    ///     2007.12.05 版本：2.1 JiRiGaLa   整理主键、完善排序顺序功能。
    ///     2007.06.06 版本：2.0 JiRiGaLa   整理主键顺序，注释,规范主键。
    ///     2007.05.30 版本：1.3 JiRiGaLa   进行改进整理。
    ///     2006.06.01 版本：1.2 JiRiGaLa   添加了一个GetList()方法
    ///		2006.02.06 版本：1.2 JiRiGaLa   重新调整主键的规范化。
    ///		2004.05.18 版本：1.0 JiRiGaLa   改进表结构,添加表结构定义部分,优化菜单生成的方法
    ///		2003.12.29 版本：1.1 JiRiGaLa   改进成以后可以扩展到多种数据库的结构形式
    ///
    /// </summary>
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.10</date>
    /// </author>
    public partial class BaseModuleManager : BaseManager
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="code">编号</param>
        public BaseModuleEntity GetObjectByCode(string code)
        {
            BaseModuleEntity result = null;

            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, code));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));
            result = BaseEntity.Create<BaseModuleEntity>(this.ExecuteReader(parameters));

            return result;
        }

        /// <summary>
        /// 获取一个操作权限的主键
        /// 若不存在就自动增加一个
        /// </summary>
        /// <param name="permissionCode">操作权限编号</param>
        /// <param name="permissionItemName">操作权限名称</param>
        /// <returns>主键</returns>
        public string GetIdByAdd(string permissionCode, string permissionName = null)
        {
            // 判断当前判断的权限是否存在，否则很容易出现前台设置了权限，后台没此项权限
            // 需要自动的能把前台判断过的权限，都记录到后台来
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, permissionCode));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));

            BaseModuleEntity entity = BaseEntity.Create<BaseModuleEntity>(this.GetDataTable(parameters, BaseModuleEntity.FieldId));

            string result = string.Empty;
            if (entity != null)
            {
                result = entity.Id.ToString();
            }
            else
            {
                entity = new BaseModuleEntity();
            }

            // 若是在调试阶段、有没在的权限被判断了，那就自动加入这个权限，不用人工加入权限了，工作效率会提高很多，
            // 哪些权限是否有被调用什么的，还可以进行一些诊断
            // #if (DEBUG)
            if (String.IsNullOrEmpty(result))
            {
                /*
                // 这里需要进行一次加锁，方式并发冲突发生
                lock (PermissionLock)
                {
                    entity.Code = permissionCode;
                    if (string.IsNullOrEmpty(permissionName))
                    {
                        entity.FullName = permissionCode;
                    }
                    else
                    {
                        entity.FullName = permissionName;
                    }
                    entity.ParentId = null;
                    entity.IsScope = 0;
                    entity.IsPublic = 0;
                    // permissionEntity.CategoryCode = "Application";
                    entity.IsMenu = 0;
                    entity.IsVisible = 1;
                    entity.DeletionStateCode = 0;
                    entity.Enabled = 1;
                    entity.AllowDelete = 1;
                    entity.AllowEdit = 1;
                    // 这里是防止主键重复？
                    // permissionEntity.Id = BaseBusinessLogic.NewGuid();
                    result = this.AddObject(entity);
                }
                */
            }
            else
            {
                // 更新最后一次访问日期，设置为当前服务器日期
                SQLBuilder sqlBuilder = new SQLBuilder(DbHelper);
                sqlBuilder.BeginUpdate(this.CurrentTableName);
                sqlBuilder.SetDBNow(BaseModuleEntity.FieldLastCall);
                sqlBuilder.SetWhere(BaseModuleEntity.FieldId, result);
                sqlBuilder.EndUpdate();
            }
            //    #endif

            return result;
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="fullName">名称</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string AddByDetail(string code, string fullName, out string statusCode)
        {
            BaseModuleEntity permissionEntity = new BaseModuleEntity();
            permissionEntity.Code = code;
            permissionEntity.FullName = fullName;
            return this.Add(permissionEntity, out statusCode);
        }


        /// <summary>
        /// 获得用户授权范围
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByUser(string systemCode, string userId, string permissionCode)
        {
            DataTable result = new DataTable(this.CurrentTableName);
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            // 这里需要判断,是系统权限？
            bool isRole = false;
            BaseUserManager userManager = new BaseUserManager(this.DbHelper, this.UserInfo);
            // 用户管理员
            isRole = userManager.IsInRoleByCode(userId, "UserAdmin");
            if (isRole)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCategoryCode, "System"));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));
                result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                result.TableName = this.CurrentTableName;
                return result;
            }

            // 这里需要判断,是业务权限？
            isRole = userManager.IsInRoleByCode(userId, "Admin");
            if (isRole)
            {
                parameters = new List<KeyValuePair<string, object>>();
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCategoryCode, "Application"));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));
                result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                result.TableName = this.CurrentTableName;
                return result;
            }

            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            string[] permissionIds = permissionScopeManager.GetTreeResourceScopeIds(systemCode, userId, BaseModuleEntity.TableName, permissionCode, true);
            // 有效的，未被删除的
            parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldId, permissionIds));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));

            result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
            result.TableName = this.CurrentTableName;
            return result;
        }

        #region public DataTable GetRootList()
        /// <summary>
        /// GetRoot 的主键
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetRootList()
        {
            return this.GetDataTableByParent(string.Empty);
        }
        #endregion

        /// <summary>
        /// 获取一个
        /// </summary>
        /// <param name="code">编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCode(string code)
        {
            return this.GetDataTable(new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, code));
        }

        /// <summary>
        /// 获取一个
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByFormName(string formName)
        {
            return this.GetDataTable(new KeyValuePair<string, object>(BaseModuleEntity.FieldFormName, formName));
        }

        /// <summary>
        /// 获得用户授权范围
        /// </summary>
        /// <param name="staffId">员工主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPermission(string systemCode, string userId, string permissionCode)
        {
            DataTable result = new DataTable(this.CurrentTableName);
            //string[] names = null;
            //object[] values = null;

            // 这里需要判断,是系统权限？
            bool isRole = false;
            BaseUserManager userManager = new BaseUserManager(this.DbHelper, this.UserInfo);
            // 用户管理员
            isRole = userManager.IsInRoleByCode(userId, "UserAdmin");
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            if (isRole)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCategoryCode, "System"));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));

                result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                result.TableName = this.CurrentTableName;
                return result;
            }

            // 这里需要判断,是业务权限？
            isRole = userManager.IsInRoleByCode(userId, "Admin");
            if (isRole)
            {
                parameters = new List<KeyValuePair<string, object>>();
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCategoryCode, "Application"));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));
                result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                result.TableName = this.CurrentTableName;
                return result;
            }

            BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            string[] moduleIds = permissionScopeManager.GetTreeResourceScopeIds(systemCode, userId, BaseModuleEntity.TableName, permissionCode, true);
            // 有效的，未被删除的
            parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldId, moduleIds));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));

            result = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
            result.TableName = this.CurrentTableName;
            return result;
        }

        #region public string Add(string fullName)
        /// <summary>
        /// Add 添加的主键
        /// </summary>
        /// <param name="paramobject">对象</param>
        /// <returns>主键</returns>
        public string Add(string fullName)
        {
            string statusCode = string.Empty;
            BaseModuleEntity entity = new BaseModuleEntity();
            entity.FullName = fullName;
            return this.Add(entity, out statusCode);
        }
        #endregion

        #region public string Add(BaseModuleEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>返回</returns>
        public string Add(BaseModuleEntity entity, out string statusCode)
        {
            string result = string.Empty;
            // 检查名称是否重复
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, entity.ParentId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, entity.Code));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldFullName, entity.FullName));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));

            if (this.Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                result = this.AddObject(entity);
                // 运行成功
                statusCode = Status.OKAdd.ToString();
            }
            return result;
        }
        #endregion

        #region public override int BatchSave(DataTable dt) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            int result = 0;
            BaseModuleEntity entity = new BaseModuleEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    string id = dr[BaseModuleEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        if (dr[BaseModuleEntity.FieldAllowDelete, DataRowVersion.Original].ToString().Equals("1"))
                        {
                            result += this.DeleteObject(id);
                        }
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    string id = dr[BaseModuleEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        // 判断是否允许编辑
                        if (this.UserInfo.IsAdministrator || entity.AllowEdit == 1)
                        {
                            result += this.UpdateObject(entity);
                        }
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
                    result += this.AddObject(entity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            this.StatusCode = Status.OK.ToString();
            return result;
        }
        #endregion

        #region public int ResetSortCode(string parentId) 重置排序码
        /// <summary>
        /// 重置排序码
        /// </summary>
        /// <param name="parentId">父节点主键</param>
        public int ResetSortCode(string parentId)
        {
            int result = 0;
            var dt = this.GetDataTableByParent(parentId);
            string id = string.Empty;
            BaseSequenceManager sequenceManager = new BaseSequenceManager(DbHelper);
            string[] sortCode = sequenceManager.GetBatchSequence(BaseModuleEntity.TableName, dt.Rows.Count);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                id = dr[BaseModuleEntity.FieldId].ToString();
                result += this.SetProperty(id, new KeyValuePair<string, object>(BaseModuleEntity.FieldSortCode, sortCode[i]));
                i++;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取有某个权限的用户列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">权限主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>用户列表</returns>
        public DataTable GetModuleUserDataTable(string systemCode, string moduleId, string companyId, string userId)
        {
            DataTable result = new DataTable(BaseUserEntity.TableName);

            string tableName = BasePermissionEntity.TableName;

            if (string.IsNullOrEmpty(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(this.UserInfo.SystemCode))
                {
                    systemCode = this.UserInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            tableName = systemCode + "Permission";
            string commandText = @"SELECT BaseUser.Id
                                    , BaseUser.UserName
                                    , BaseUser.CompanyName
                                    , BaseUser.DepartmentName
                                    , BaseUser.RealName 
                                    , BaseUser.Description 
                                    , Permission.Enabled
                                    , Permission.CreateOn
                                    , Permission.CreateBy
                                    , Permission.ModifiedOn
                                    , Permission.ModifiedBy
                        FROM BaseUser RIGHT OUTER JOIN
                          (SELECT ResourceId AS UserId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
                             FROM " + tableName + @"
                            WHERE DeletionStateCode = 0 "
                              + " AND ResourceCategory = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                              + " AND PermissionId = " + DbHelper.GetParameter(BaseModuleEntity.FieldId) + @") Permission 
                            ON BaseUser.Id = Permission.UserId AND BaseUser.DeletionStateCode = 0 AND BaseUser.Enabled = 1 
                         WHERE BaseUser.Id IS NOT NULL ";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName));
            dbParameters.Add(DbHelper.MakeParameter(BaseModuleEntity.FieldId, moduleId));

            if (!string.IsNullOrEmpty(companyId))
            {
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }

            if (!string.IsNullOrEmpty(userId))
            {
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " = " + userId;
            }
            commandText += " ORDER BY Permission.CreateOn DESC ";

            result = this.Fill(commandText, dbParameters.ToArray());

            return result;
        }

        public DataTable GetModuleRoleDataTable(string systemCode, string moduleId)
        {
            DataTable result = new DataTable(BaseRoleEntity.TableName);

            string tableName = BasePermissionEntity.TableName;

            if (string.IsNullOrEmpty(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(this.UserInfo.SystemCode))
                {
                    systemCode = this.UserInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            tableName = systemCode + "Permission";

            string roleTableName = BaseRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                roleTableName = systemCode + "Role";
            }
            string commandText = @"SELECT Role.Id
                                    , Role.Code
                                    , Role.RealName 
                                    , Role.Description 
                                    , Permission.Enabled
                                    , Permission.CreateOn
                                    , Permission.CreateBy
                                    , Permission.ModifiedOn
                                    , Permission.ModifiedBy
                        FROM (SELECT Id, Code, RealName, Description FROM BaseRole ";

            if (!systemCode.Equals("Base"))
            {
                commandText += " UNION SELECT Id, Code, RealName, Description FROM roleTableName ";
            }

            commandText += @") Role RIGHT OUTER JOIN
                          (SELECT ResourceId AS RoleId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
                             FROM BasePermission Permission
                            WHERE DeletionStateCode = 0 "
                              + " AND ResourceCategory = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                              + " AND PermissionId = " + DbHelper.GetParameter(BaseModuleEntity.FieldId) + @") Permission 
                            ON Role.Id = Permission.RoleId
                         WHERE Role.Id IS NOT NULL
                      ORDER BY Permission.CreateOn DESC";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, roleTableName));
            dbParameters.Add(DbHelper.MakeParameter(BaseModuleEntity.FieldId, moduleId));

            commandText = commandText.Replace("BasePermission", tableName);
            commandText = commandText.Replace("roleTableName", roleTableName);
            // commandText = commandText.Replace("BaseRole", roleTableName);
            result = this.Fill(commandText, dbParameters.ToArray());

            return result;
        }

        public DataTable GetModuleOrganizeDataTable(string systemCode, string moduleId)
        {
            DataTable result = new DataTable(BaseOrganizeEntity.TableName);

            if (string.IsNullOrEmpty(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(this.UserInfo.SystemCode))
                {
                    systemCode = this.UserInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            string tableName = systemCode + "Permission";

            string commandText = @"SELECT BaseOrganize.Id
                                    , BaseOrganize.Code
                                    , BaseOrganize.FullName 
                                    , BaseOrganize.Description 
                                    , Permission.Enabled
                                    , Permission.CreateOn
                                    , Permission.CreateBy
                                    , Permission.ModifiedOn
                                    , Permission.ModifiedBy
                        FROM BaseOrganize RIGHT OUTER JOIN
                          (SELECT ResourceId AS OrganizeId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
                             FROM BasePermission
                            WHERE DeletionStateCode = 0 "
                              + " AND ResourceCategory = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                              + " AND PermissionId = " + DbHelper.GetParameter(BaseModuleEntity.FieldId) + @") Permission 
                            ON BaseOrganize.Id = Permission.OrganizeId
                         WHERE BaseOrganize.Id IS NOT NULL
                      ORDER BY Permission.CreateOn DESC";

            commandText = commandText.Replace("BasePermission", tableName);

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, "BaseOrganize"));
            dbParameters.Add(DbHelper.MakeParameter(BaseModuleEntity.FieldId, moduleId));

            result = this.Fill(commandText, dbParameters.ToArray());

            return result;
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public static BaseModuleEntity GetObjectByCache(string id)
        {
            return GetObjectByCache("Base", id);
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public static BaseModuleEntity GetObjectByCache(BaseUserInfo userInfo, string id)
        {
            return GetObjectByCache(userInfo.SystemCode, id);
        }

        public static BaseModuleEntity GetObjectByCache(string systemCode, string id, bool refreshCache = false)
        {
            BaseModuleEntity result = null;

            string cacheKey = systemCode + ".Module";
            if (!string.IsNullOrEmpty(id))
            {
                cacheKey = systemCode + ".Module." + id;
            }

            if (!refreshCache)
            {
                result = GetCacheByKey(cacheKey);
            }

            if (result == null)
            {
                // 动态读取表中的数据
                string tableName = systemCode + "Module";
                BaseModuleManager manager = new BaseModuleManager(tableName);
                result = manager.GetObject(id);
                // 若是空的不用缓存，继续读取实体
                if (result != null)
                {
                    SetCache(systemCode, result);
                }
            }

            return result;
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>权限实体</returns>
        public static BaseModuleEntity GetObjectByCacheByCode(string systemCode, string code)
        {
            BaseModuleEntity result = null;

            string cacheKey = systemCode + ".Module";
            if (!string.IsNullOrEmpty(code))
            {
                cacheKey = systemCode + ".Module." + code;
            }
            result = GetCacheByKey(cacheKey);

            if (result == null)
            {
                // 动态读取表中的数据
                string tableName = systemCode + "Module";
                BaseModuleManager manager = new BaseModuleManager(tableName);
                result = manager.GetObjectByCode(code);
                // 若是空的不用缓存，继续读取实体
                if (result != null)
                {
                    SetCache(systemCode, result);
                }
            }

            return result;
        }

        #region public static string GetIdByCodeByCache(string systemCode, string code) 通过编号获取主键
        /// <summary>
        /// 通过编号获取主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>主键</returns>
        public static string GetIdByCodeByCache(string systemCode, string code)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(code))
            {
                BaseModuleEntity moduleEntity = GetObjectByCacheByCode(systemCode, code);
                if (moduleEntity != null)
                {
                    result = moduleEntity.Id.ToString();
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 通过编号获取选项的显示内容
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetNameByCache(string systemCode, string id)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                BaseModuleEntity moduleEntity = GetObjectByCache(systemCode, id);
                if (moduleEntity != null)
                {
                    result = moduleEntity.FullName;
                }
            }

            return result;
        }

        #region public static List<BaseModuleEntity> GetEntitiesByCache(string systemCode = "Base") 获取模块菜单表，从缓存读取
        /// <summary>
        /// 获取模块菜单表，从缓存读取
        /// 没有缓存也可以用的。
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>菜单</returns>
        public static List<BaseModuleEntity> GetEntitiesByCache(string systemCode = "Base", bool refreshCache = false)
        {
            List<BaseModuleEntity> result = null;

            string tableName = systemCode + "Module";

# if Redis
            if (!refreshCache)
            {
                result = GetListCache(tableName);
            }
#endif

            if (result == null)
            {
                BaseModuleManager moduleManager = new BaseModuleManager(tableName);
                // 读取目标表中的数据
                List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
                // 有效的菜单
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                // 没被删除的菜单
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeletionStateCode, 0));
                // parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldIsVisible, 1));
                // 2015-11-30 吉日嘎拉，这里没必要进行排序，浪费时间，列表可以认为是无序的，在界面上进行排序就可以了
                result = moduleManager.GetList<BaseModuleEntity>(parameters);
                // 若是空的不用缓存，继续读取实体
                if (result != null)
                {
# if Redis
                    SetListCache(tableName, result);
#endif
                }
            }

            return result;
        }
        #endregion
    }
}