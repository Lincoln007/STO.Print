﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Reflection;
using System.ServiceModel;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// SequenceService
    /// 序列服务
    /// 
    /// 修改记录
    /// 
    ///		2013.02.17 版本：3.0 JiRiGaLa 序列表虽然进行了读写分离，但是一定要都从主表获取才可以，因为实时性要求很高。
    ///		2007.08.15 版本：2.2 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.1 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///		2007.05.14 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.08.15</date>
    /// </author> 
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class SequenceService : ISequenceService
    {
        /// <summary>
        /// 多用户并发处理用
        /// </summary>
        private static Object LOCK = new Object();


        #region public string Add(BaseUserInfo userInfo, BaseSequenceEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加序列
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">序列实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseSequenceEntity entity, out string statusCode, out string statusMessage)
        {
			string result = string.Empty;
			
            string returnCode = string.Empty;
			string returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper, userInfo);
				// 调用方法，并且返回运行结果
				result = sequenceManager.Add(entity, out returnCode);
				// result = businessCardManager.Add(businessCardEntity, out statusCode);
				returnMessage = sequenceManager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;

			return result;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, DataTable result, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result">数据表</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public string Add(BaseUserInfo userInfo, DataTable dt, out string statusCode, out string statusMessage)
        {
            BaseSequenceEntity sequenceEntity = BaseEntity.Create<BaseSequenceEntity>(dt);
            return this.Add(userInfo, sequenceEntity, out statusCode, out statusMessage);
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取序列号列表
        /// <summary>
        /// 获取序列号列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
			var dt = new DataTable(BaseSequenceEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper);
				dt = sequenceManager.GetDataTable();
				dt.TableName = BaseSequenceEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public BaseSequenceEntity GetObject(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseSequenceEntity GetObject(BaseUserInfo userInfo, string id)
        {
			BaseSequenceEntity sequenceEntity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseSequenceManager(dbHelper, userInfo);
				sequenceEntity = manager.GetObject(id);
			});

			return sequenceEntity;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, BaseSequenceEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新序列
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">序列实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseSequenceEntity entity, out string statusCode, out string statusMessage)
        {
			int result = 0;
			
            string returnCode = string.Empty;
			string returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseSequenceManager(dbHelper, userInfo);
				// 编辑数据
				result = manager.Update(entity, out returnCode);
				// result = businessCardManager.Update(businessCardEntity, out statusCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, DataTable result, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result">数据表</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public int Update(BaseUserInfo userInfo, DataTable dt, out string statusCode, out string statusMessage)
        {
            BaseSequenceEntity sequenceEntity = BaseEntity.Create<BaseSequenceEntity>(dt);
            return this.Update(userInfo, sequenceEntity, out statusCode, out statusMessage);
        }
        #endregion

        #region public string Increment(BaseUserInfo userInfo, string fullName) 获取序列号
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        public string Increment(BaseUserInfo userInfo, string fullName)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithLock(userInfo, parameter, LOCK, (dbHelper) =>
			{
                result = this.Increment(dbHelper, userInfo, fullName);
			});
			return result;
        }
        #endregion

        #region public string Increment(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName) 获取序列号
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        public string Increment(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName)
        {
            var sequenceManager = new BaseSequenceManager(dbHelper);
            return sequenceManager.Increment(fullName);
        }
        #endregion

        #region public string GetOldSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix) 获取原序列号
        /// <summary>
        /// 获取原序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充补零</param>
        /// <returns>序列号</returns>
        public string GetOldSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithLock(userInfo, parameter, LOCK, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper);
                result = sequenceManager.StoreCounter(fullName, defaultSequence, sequenceLength, fillZeroPrefix);
			});
			return result;
        }
        #endregion

        #region public string GetNewSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix) 获取原序列号
        /// <summary>
        /// 获取新序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="defaultSequence">默认序列</param>
        /// <param name="sequenceLength">序列长度</param>
        /// <param name="fillZeroPrefix">是否填充补零</param>
        /// <returns>序列号</returns>
        public string GetNewSequence(BaseUserInfo userInfo, string fullName, int defaultSequence, int sequenceLength, bool fillZeroPrefix)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithLock(userInfo, parameter, LOCK, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper);
                result = sequenceManager.Increment(fullName, defaultSequence, sequenceLength, fillZeroPrefix);
			});
			return result;
        }
        #endregion


        #region public string[] GetBatchSequence(BaseUserInfo userInfo, string fullName, int count) 获取序列号
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="count">个数</param>
        /// <returns>序列号</returns>
        public string[] GetBatchSequence(BaseUserInfo userInfo, string fullName, int count)
        {
			string[] result = new string[0];

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				result = this.GetBatchSequence(dbHelper, userInfo, fullName, count);
			});
			return result;
        }
        #endregion

        #region public string[] GetBatchSequence(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName, count) 获取序列号
        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <param name="count">个数</param>
        /// <returns>序列号</returns>
        public string[] GetBatchSequence(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName, int count)
        {
            var sequenceManager = new BaseSequenceManager(dbHelper);
            return sequenceManager.GetBatchSequence(fullName, count);
        }
        #endregion

        #region public string GetReduction(BaseUserInfo userInfo, string fullName) 获取序列号
        /// <summary>
        /// 获取倒序序列号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        public string GetReduction(BaseUserInfo userInfo, string fullName)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithLock(userInfo, parameter, LOCK, (dbHelper) =>
			{
				result = this.GetReduction(dbHelper, userInfo, fullName);
			});
			return result;
        }
        #endregion

        #region public string GetReduction(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName)
        /// <summary>
        /// 获取倒序序列号
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">序列名称</param>
        /// <returns>序列号</returns>
        public string GetReduction(IDbHelper dbHelper, BaseUserInfo userInfo, string fullName)
        {
            var sequenceManager = new BaseSequenceManager(dbHelper);
            return sequenceManager.GetReduction(fullName);
        }
        #endregion

        #region public int Reset(BaseUserInfo userInfo, string[] ids) 批量重置
        /// <summary>
        /// 批量重置序列
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int Reset(BaseUserInfo userInfo, string[] ids)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper);
				result = sequenceManager.Reset(ids);
			});
			return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id) 删除序列
        /// <summary>
        /// 删除序列
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
				var sequenceManager = new BaseSequenceManager(dbHelper);
				result = sequenceManager.Delete(id);
			});
			return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除序列
        /// <summary>
        /// 批量删除序列
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
			int result = 0;
            
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var sequenceManager = new BaseSequenceManager(dbHelper);
				result = sequenceManager.Delete(ids);
			});
			return result;
        }
        #endregion
    }
}