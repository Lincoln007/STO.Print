﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System.Data;
using System.ServiceModel;

namespace DotNet.IService
{
    using DotNet.Utilities;
    
    /// <summary>
    /// IMessageService
    /// 
    /// 修改记录
    /// 
    ///		2013.11.12 版本：1.0 JiRiGaLa 添加权限。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.11.12</date>
    /// </author> 
    /// </summary>
    public partial interface IMessageService
    {
        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        [OperationContract]
        DataTable GetInnerOrganizeDT(BaseUserInfo userInfo);

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">上级主键</param>
        /// <returns>数据表</returns>
        [OperationContract]
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <returns>区域数组</returns>
        /// [OperationContract]
        /// string[] GetArea(BaseUserInfo result);
        
        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="area">区域</param>
        /// <returns>省份数组</returns>
        [OperationContract]
        string[] GetProvince(BaseUserInfo userInfo);
        
        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省份主键</param>
        /// <returns>城市数组</returns>
        [OperationContract]
        string[] GetCity(BaseUserInfo userInfo, string province);
        
        /// <summary>
        /// 获取县区
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="cityId">城市主键</param>
        /// <returns>县区数组</returns>
        [OperationContract]
        string[] GetDistrict(BaseUserInfo userInfo, string province, string city);

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <returns>数据表</returns>
        [OperationContract]
        string[] GetOrganizeByProvince(BaseUserInfo userInfo, string province);

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <returns>数据表</returns>
        [OperationContract]
        string[] GetOrganizeByCity(BaseUserInfo userInfo, string province, string city);

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <param name="district">县区</param>
        /// <returns>数据表</returns>
        [OperationContract]
        string[] GetOrganizeByDistrict(BaseUserInfo userInfo, string province, string city, string district);
    }
}