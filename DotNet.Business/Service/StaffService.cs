﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.ServiceModel;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// StaffService
    /// 员工服务
    /// 
    /// 修改记录
    ///		2007.05.22 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.05.22</date>
    /// </author> 
    /// </summary>
   [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class StaffService : IStaffService
    {
        #region public DataTable GetAddressDataTable(BaseUserInfo userInfo, string organizeId, string searchValue)
        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="search">查询内容</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTable(BaseUserInfo userInfo, string organizeId, string searchValue)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.GetAddressDataTable(organizeId, searchValue);
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public DataTable GetAddressDataTableByPage(BaseUserInfo userInfo, string organizeId, string searchValue, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null)
        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="search">查询内容</param>
        /// <param name="pageSize">分页的条数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTableByPage(BaseUserInfo userInfo, string organizeId, string searchValue, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);
			int myrecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				// 这里是不获取所有的列，只获取自己需要的列的标准方法
				var manager = new BaseStaffManager(dbHelper, userInfo);
                manager.SelectFields = "Id, Code, RealName, DepartmentName, DutyName, OfficePhone, Extension, Mobile, ShortNumber, Email, QQ, Description";
				dt = manager.GetAddressDataTableByPage(organizeId, searchValue, out myrecordCount, pageSize, pageIndex, sort);
				dt.TableName = BaseStaffEntity.TableName;
			});
			recordCount = myrecordCount;
			return dt;
        }
        #endregion

        #region public int UpdateAddress(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage) 更新通讯地址
        /// <summary>
        /// 更新通讯地址
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        public int UpdateAddress(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage)
        {
			int result = 0;
			
            string returnCode = string.Empty;
			string returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				// for (int i = 0; i < result.Rows.Count; i++)
				// {
				result += manager.UpdateAddress(entity, out returnCode);
				// }
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        /// <summary>
        /// 批量更新通讯地址
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchUpdateAddress(BaseUserInfo userInfo, DataTable dt)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				// 这里是只更新部分字段的例子
				foreach(DataRow dr in dt.Rows)
				{
					var sqlBuilder = new SQLBuilder(dbHelper);
					sqlBuilder.BeginUpdate(BaseStaffEntity.TableName);
					// 这里是界面上显示的字段，需要更新的字段
					sqlBuilder.SetValue(BaseStaffEntity.FieldOfficePhone, dr[BaseStaffEntity.FieldOfficePhone].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldExtension, dr[BaseStaffEntity.FieldExtension].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldMobile, dr[BaseStaffEntity.FieldMobile].ToString());
					sqlBuilder.SetValue(BaseStaffEntity.FieldShortNumber, dr[BaseStaffEntity.FieldShortNumber].ToString());
					sqlBuilder.SetValue(BaseStaffEntity.FieldEmail, dr[BaseStaffEntity.FieldEmail].ToString());
					sqlBuilder.SetValue(BaseStaffEntity.FieldQQ, dr[BaseStaffEntity.FieldQQ].ToString());
					sqlBuilder.SetValue(BaseStaffEntity.FieldDescription, dr[BaseStaffEntity.FieldDescription].ToString());
					sqlBuilder.SetWhere(BaseStaffEntity.FieldId, dr[BaseStaffEntity.FieldId].ToString());
					result += sqlBuilder.EndUpdate();
				}
			});
			return result;
        }

        #region public string AddStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage) 添加员工
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        public string AddStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage)
        {
			string result = string.Empty;
			
            string returnCode = string.Empty;
			string returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				// 1.若添加用户成功，添加员工。
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.Add(entity, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
				// 2.自己不用给自己发提示信息，这个提示信息是为了提高工作效率的
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public int UpdateStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage) 更新员工
        /// <summary>
        /// 更新员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        public int UpdateStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage)
        {
			int result = 0;
			
            string returnCode = string.Empty;
			string returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				// 3.员工的修改
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.Update(entity, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取员工列表
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.GetDataTable(new KeyValuePair<string, object>(BaseStaffEntity.FieldDeletionStateCode, 0), BaseStaffEntity.FieldSortCode);
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseStaffEntity GetObject(BaseUserInfo userInfo, string id)
        {
			BaseStaffEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				entity = manager.GetObject(id);
			});
			return entity;
        }

        #region public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids) 获取员工列表
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.GetDataTable(BaseStaffEntity.FieldId, ids, BaseStaffEntity.FieldSortCode);
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string searchValue, string auditStates, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null) 查询用户
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="whereClause">查询</param>
        /// <param name="auditStates">有效</param>
        /// <param name="roleIds">用户角色</param>
        /// <returns>数据表</returns>
        public DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string companyId, string whereClause, string auditStates, bool? enabled, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null)
        {
            recordCount = 0;
            int myRecordCount = 0;
            DataTable result = new DataTable();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var staffManager = new BaseStaffManager(dbHelper, userInfo);
                // result = staffManager.GetDataTable(100, BaseStaffEntity.FieldSortCode);
                result = staffManager.SearchByPage(permissionCode, whereClause, enabled, auditStates, companyId, null, out myRecordCount, pageIndex, pageSize, sort);
                result.TableName = BaseStaffEntity.TableName;
            });
            recordCount = myRecordCount;
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByCompany(BaseUserInfo userInfo, string companyId, bool containChildren) 按公司获取部门员工
        /// <summary>
        /// 按公司获取部门员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCompany(BaseUserInfo userInfo, string companyId, bool containChildren)
        {
            var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                if (containChildren)
                {
                    dt = manager.GetChildrenStaffs(companyId);
                }
                else
                {
                    dt = manager.GetDataTableByCompany(companyId);
                }
                dt.TableName = BaseStaffEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren) 按部门获取员工列表
        /// <summary>
        /// 按部门获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				if(containChildren)
				{
					dt = manager.GetChildrenStaffs(departmentId);
				}
				else
				{
					dt = manager.GetDataTableByDepartment(departmentId);
				}
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public DataTable GetDataTableByOrganize(BaseUserInfo BaseUserInfo, string organizeId, bool containChildren) 按公司获取员工列表
        /// <summary>
        /// 按公司获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganize(BaseUserInfo userInfo, string organizeId, bool containChildren)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				// 用户已经不存在的需要整理干净，防止数据不完整。
				string sqlQuery = "UPDATE BaseStaff SET UserId = NULL WHERE (UserId NOT IN (SELECT Id FROM BaseUser))";
				dbHelper.ExecuteNonQuery(sqlQuery);
				dbHelper.Close();
				// 这里是读取的服务器连接
				dbHelper.Open(BaseSystemInfo.UserCenterReadDbConnection);
				var manager = new BaseStaffManager(dbHelper, userInfo);
				if(containChildren)
				{
					dt = manager.GetChildrenStaffs(organizeId);
				}
				else
				{
					dt = manager.GetDataTableByOrganize(organizeId);
				}
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        public DataTable GetChildrenStaffs(BaseUserInfo userInfo, string organizeId)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				// 获得组织机构列表
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.GetChildrenStaffs(organizeId);
				dt.DefaultView.Sort = BaseStaffEntity.FieldSortCode;
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }

        public DataTable GetParentChildrenStaffs(BaseUserInfo userInfo, string organizeId)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				// 获得组织机构列表
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.GetParentChildrenStaffs(organizeId);
				dt.DefaultView.Sort = BaseStaffEntity.FieldSortCode;
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }

        #region public DataTable Search(BaseUserInfo userInfo, string organizeId, string searchValue) 获得员工列表
        /// <summary>
        /// 获得员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织主键</param>
        /// <param name="search">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string organizeId, string searchValue)
        {
			var dt = new DataTable(BaseStaffEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				dt = manager.Search(organizeId, searchValue, false);
				dt.TableName = BaseStaffEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public int SetStaffUser(BaseUserInfo userInfo, string staffId, string userId)
        /// <summary>
        /// 员工关联用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>影响行数</returns>
        public int SetStaffUser(BaseUserInfo userInfo, string staffId, string userId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				if(string.IsNullOrEmpty(userId))
				{
					result = manager.SetProperty(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId));
				}
				else
				{
					// 一个用户只能帮定到一个帐户上，检查是否已经绑定过这个用户了。
					string[] staffIds = manager.GetIds(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeletionStateCode, 0));
					if(staffIds == null || staffIds.Length == 0)
					{
						result = manager.SetProperty(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId));
						BaseUserManager userManager = new BaseUserManager(dbHelper, userInfo);
						BaseUserEntity userEntity = userManager.GetObject(userId);
						result = manager.SetProperty(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, userEntity.UserName));
					}
				}
			});
			return result;
        }
        #endregion

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(BaseUserInfo userInfo, bool all = false)
        {
            int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var staffManager = new BaseStaffManager(dbHelper, userInfo);
                // result = staffManager.Synchronous(all);
            });

            return result;
        }

        #region public int DeleteUser(BaseUserInfo userInfo, string staffId)
        /// <summary>
        /// 删除员工关联的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <returns>影响行数</returns>
        public int DeleteUser(BaseUserInfo userInfo, string staffId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.DeleteUser(staffId);
			});
			return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id) 单个删除
        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.Delete(id);
			});
			return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.BatchDelete(ids);
			});

			return result;
        }
        #endregion

        #region public int SetDeleted(BaseUserInfo userInfo, string[] ids) 批量打删除标志
        /// <summary>
        /// 批量打删除标志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string[] ids)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var userManager = new BaseUserManager(dbHelper, userInfo);
				var staffManager = new BaseStaffManager(dbHelper, userInfo);
				BaseStaffEntity staffEntity = null;
				for(int i = 0; i < ids.Length; i++)
				{
					// 删除相应的用户
					staffEntity = staffManager.GetObject(ids[i]);
					if(staffEntity.UserId != null)
					{
						userManager.SetDeleted(staffEntity.UserId);
					}
					// 删除职员
					result = staffManager.SetDeleted(ids[i], true);
				}
			});
			return result;
        }
        #endregion

        #region public int MoveTo(BaseUserInfo userInfo, string id, string organizeId) 移动数据
        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键数组</param>
        /// <param name="organizeId">父结点主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string id, string organizeId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.SetProperty(id, new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, organizeId));
			});

			return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string organizeId) 移动数据
        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键数组</param>
        /// <param name="organizeId">父结点主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string organizeId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				for(int i = 0; i < ids.Length; i++)
				{
					result += manager.SetProperty(ids[i], new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, organizeId));
				}
			});
			return result;
        }
        #endregion

        #region public int ChangeDepartment(BaseUserInfo userInfo, string id, string CompanyId, string DepartmentId) 部门变动
        /// <summary>
        /// 部门变动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">员工主键</param>
        /// <param name="companyId">单位主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>影响行数</returns>
        public int ChangeDepartment(BaseUserInfo userInfo, string id, string companyId, string departmentId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
				parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldCompanyId, companyId));
				parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, departmentId));
				result = manager.SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id), parameters);
			});

			return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result) 批量保存员工
        /// <summary>
        /// 批量保存员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper, userInfo);
				result = manager.BatchSave(dt);
				// ReturnDataTable = Staff.GetDataTableByOrganize(organizeId);
			});
			return result;
        }
        #endregion

        #region public int ResetSortCode(BaseUserInfo userInfo) 重新排序数据
        /// <summary>
        /// 重新排序数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>影响行数</returns>
        public int ResetSortCode(BaseUserInfo userInfo)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper);
				result = manager.ResetSortCode();
			});
			return result;
        }
        #endregion

        #region  public string GetId(BaseUserInfo userInfo,string name, object value) 获取主键
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="name">查询的参数</param>
        /// <param name="value">参数值</param>
        /// <returns>影响行数</returns>
        public string GetId(BaseUserInfo userInfo, string name, object value)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseStaffManager(dbHelper);
				result = manager.GetId(new KeyValuePair<string, object>(name, value));
			});
			return result;
        }
        #endregion
    }
}