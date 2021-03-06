﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

namespace DotNet.Business
{
    using DotNet.IService;
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// FileService
    /// 文件服务
    /// 
    /// 修改记录
    /// 
	///		2013.06.05 张祈璟重构
	///		2008.04.30 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.04.30</date>
    /// </author> 
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class FileService : IFileService
    {
        #region public string Send(BaseUserInfo userInfo, string fileName, byte[] file, string toUserId)
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件内容</param>
        /// <param name="toUserId">发送给谁主键</param>
        /// <returns>文件主键</returns>
        public string Send(BaseUserInfo userInfo, string fileName, byte[] file, string toUserId)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				BaseFolderEntity folderEntity = new BaseFolderEntity();
				var folderManager = new BaseFolderManager(dbHelper, userInfo);
				// 检查相应的系统必备文件夹
				folderManager.FolderCheck();
				BaseUserEntity userEntity = new BaseUserManager(dbHelper, userInfo).GetObject(toUserId);
				if(!string.IsNullOrEmpty(userEntity.Id))
				{
					// 04:判断发送者的空间是否存在？
					// 05:判断已收文件夹是否存在？
					if(!folderManager.Exists(userEntity.Id))
					{
						folderEntity.FolderName = userEntity.RealName + AppMessage.FileService_File;
						folderEntity.ParentId = "UserSpace";
						folderEntity.Id = userEntity.Id;
						folderEntity.Enabled = 1;
						folderEntity.DeletionStateCode = 0;
						folderManager.AddObject(folderEntity);
					}
					// 06:判断来自谁的文件夹是否存在？
					// 07:判断发给谁的文件夹是否存在？
					if(!folderManager.Exists(toUserId + "_Receive"))
					{
						folderEntity.FolderName = AppMessage.FileService_ReceiveFile;
						folderEntity.ParentId = toUserId;
						folderEntity.Id = toUserId + "_Receive";
						folderEntity.Enabled = 1;
						folderEntity.DeletionStateCode = 0;
						folderManager.AddObject(folderEntity);
					}
					if(!folderManager.Exists(userInfo.Id + "_Send_" + toUserId))
					{
						folderEntity.FolderName = userEntity.RealName + "(" + userEntity.UserName + ")";
						folderEntity.ParentId = userInfo.Id + "_Send";
						folderEntity.Id = userInfo.Id + "_Send_" + toUserId;
						folderEntity.Enabled = 1;
						folderEntity.DeletionStateCode = 0;
						folderManager.AddObject(folderEntity);
					}
					if(!folderManager.Exists(toUserId + "_Receive_" + userInfo.Id))
					{
						folderEntity.FolderName = userInfo.RealName + "(" + userInfo.UserName + ")";
						folderEntity.ParentId = toUserId + "_Receive";
						folderEntity.Id = toUserId + "_Receive_" + userInfo.Id;
						folderEntity.Enabled = 1;
						folderEntity.DeletionStateCode = 0;
						folderManager.AddObject(folderEntity);
					}
					// 08:已发送文件夹多一个文件。
					// 09:已接收文件夹多一个文件。
					BaseFileEntity fileEntity = new BaseFileEntity();
					fileEntity.FileName = fileName;
					fileEntity.Contents = file;
					fileEntity.Enabled = 1;
					fileEntity.ReadCount = 0;
					fileEntity.FolderId = userInfo.Id + "_Send_" + toUserId;
					// 把修改人显示出来
					fileEntity.ModifiedBy = userInfo.RealName;
					fileEntity.ModifiedUserId = userInfo.Id;
					fileEntity.ModifiedOn = DateTime.Now;
					var fileManager = new BaseFileManager(dbHelper, userInfo);
					fileManager.AddObject(fileEntity);
					fileEntity.FolderId = toUserId + "_Receive_" + userInfo.Id;
					result = fileManager.AddObject(fileEntity);
                    // string webHostUrl = BaseSystemInfo.WebHost;
					// if (string.IsNullOrEmpty(webHostUrl))
					// {
					//    webHostUrl = "WebHostUrl";
					// }
					// 10:应该还发一个短信提醒一下才对。
					BaseMessageEntity messageEntity = new BaseMessageEntity();
                    //messageEntity.Id = Guid.NewGuid().ToString("N");
					messageEntity.CategoryCode = MessageCategory.Send.ToString();
					messageEntity.FunctionCode = MessageFunction.Message.ToString();
					messageEntity.ObjectId = result;
					messageEntity.ReceiverId = toUserId;
					// target=\"_blank\"
					messageEntity.Contents = AppMessage.FileService_SendFileFrom + " <a href={WebHostUrl}Download.aspx?Id=" + result + ">" + fileName + "</a>" + AppMessage.FileService_CheckReceiveFile;
					messageEntity.IsNew = (int)MessageStateCode.New;
					messageEntity.ReadCount = 0;
					messageEntity.DeletionStateCode = 0;
					var messageManager = new BaseMessageManager(dbHelper, userInfo);
					messageManager.Identity = true;
					result = messageManager.Add(messageEntity);
				}
			});
			return result;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, string description, bool enabled, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file"></param>
        /// <param name="description"></param>
        /// <param name="enabled"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public string Add(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, string description, bool enabled, out string statusCode, out string statusMessage)
        {
			string returnCode = string.Empty;
			string returnMessage = string.Empty;
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod()); 
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.Add(folderId, fileName, file, description, enabled, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public BaseFileEntity GetObject(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseFileEntity GetObject(BaseUserInfo userInfo, string id)
        {
			BaseFileEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				entity = manager.GetObject(id);
			});
			return entity;
        }
        #endregion

        #region public bool Exists(BaseUserInfo userInfo, string folderId, string fileName)
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <returns>存在</returns>
        public bool Exists(BaseUserInfo userInfo, string folderId, string fileName)
        {
			bool result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				//result = fileManager.Exists(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName));
				//加入删除状态为0的条件
				List<KeyValuePair<string, object>> parametersList = new List<KeyValuePair<string, object>>();
				parametersList.Add(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId));
				parametersList.Add(new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName));
				parametersList.Add(new KeyValuePair<string, object>(BaseFileEntity.FieldDeletionStateCode, 0));
				result = manager.Exists(parametersList);
			});

			return result;
        }
        #endregion

        #region public byte[] Download(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>文件</returns>
        public byte[] Download(BaseUserInfo userInfo, string id)
        {
			byte[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.Download(id);
			});
			return result;
        }
        #endregion

        #region public byte[] Downloads(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>文件</returns>
        public byte[] Downloads(BaseUserInfo userInfo, string id)
        {
			byte[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				if(BaseSystemInfo.UploadStorageMode == "Disk")
				{
					manager.UpdateReadCount(id);// 阅读次数要加一
					BaseFileEntity entity = manager.GetObject(id);

					System.IO.FileStream fs = null;
					//建立二进制读取
					System.IO.BinaryReader br = null;
					fs = new FileStream(entity.FilePath + entity.FileName, System.IO.FileMode.Open);
					br = new BinaryReader((Stream)fs);
					result = br.ReadBytes((int)fs.Length);
					br.Close();
					fs.Close();
				}
				else
				{
					result = manager.Download(id);
				}
			});
			return result;
        }
        #endregion

        #region public string Upload(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, bool enabled)
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="enabled">有效</param>
        /// <returns>主键</returns>
        public string Upload(BaseUserInfo userInfo, string folderId, string fileName, byte[] file,bool enabled)
        {
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.Upload(folderId, fileName, file, enabled);
			});
			return result;
        }
        #endregion

        #region public string UplaodeByBlock(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, int filesize, bool enabled)
        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="enabled">有效</param>
        /// <returns>主键</returns>
        public string UplaodeByBlock(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, int filesize, bool enabled)
        {	
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var fileManager = new BaseFileManager(dbHelper, userInfo);
				//上传到磁盘
				if(BaseSystemInfo.UploadStorageMode == "Disk")
				{
					result = fileManager.UplaodeByBlock(folderId, BaseSystemInfo.UploadPath, fileName, null, filesize, enabled);
					CreateBlankFile(fileName, filesize);
					UploadFileChunkBytes(file, 0, fileName);
				}
				else
				{
					result = fileManager.UplaodeByBlock(folderId, BaseSystemInfo.UploadPath, fileName, file, filesize, enabled);
				}
			});
			return result;
        }
        #endregion

        #region public string UplaodeByBlockUpdate(BaseUserInfo userInfo, string folderId, string fileName, byte[] file, bool enabled)
        /// <summary>
        /// 分段上传文件 ，2012-10-14 HJC Add 暂时支持SQL数据库 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="length"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string UplaodeByBlockUpdate(BaseUserInfo userInfo, string Id, string fileName, int length, byte[] file)
        {			
			string result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				if(BaseSystemInfo.UploadStorageMode == "Disk")
				{
					UploadFileChunkBytes(file, length, fileName);
				}
				else
				{
					result = manager.UplaodeByBlockUpdate(Id, length, file);
				}

				//分节上转不记录日志文件
				// BaseLogManager.Instance.Add(result, this.serviceName, AppMessage.FileService_Upload, MethodBase.GetCurrentMethod());
			});
			return result;
        }
        #endregion

        #region 写文件到服务器指定目录
        /// <summary>
        /// 创建空白文件
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="Length">长度</param>
        public void CreateBlankFile(string FileName, int Length) 
        {
            FileStream fileStream = new FileStream(BaseSystemInfo.UploadPath + "//" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.Write(new byte[Length], 0, Length);
            fileStream.Dispose();
            fileStream.Close();            
        }

        /// <summary>
        /// 整个件上传
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        public void UploadFileBytes(byte[] bytes, string fileName)
        {
            UploadFileChunkBytes(bytes, 0, fileName);
        }

        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <param name="fileName"></param>
        public void UploadFileChunkBytes(byte[] bytes, int position, string fileName)
        {
            try
            {
                //写文件时总是被点用，暂时通过写入异常来处理是被占用
                UploadFileByteWrite(bytes, position, fileName);
                //using (FileStream fs = new FileStream(BaseSystemInfo.UploadPath + "//" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                //{
                //    //该 Bytes 的字节要写到 服务器端 相应文件的从 Position 开始的字节 
                //    fs.Position = Position;
                //    fs.Write(Bytes, 0, Bytes.Length);
                //    //fs.Dispose();
                //    //fs.Close();
                //    //Thread.Sleep(120);
                //}
            }
            catch
            {
                throw;
            }
        }
        public void UploadFileByteWrite(byte[] bytes, int position, string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(BaseSystemInfo.UploadPath + "//" + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fs.Position = position;
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch
            {
                //if (ex.StatusMessage.IndexOf("正由另一进程使用，因此该进程无法访问此文件")>0)
                //{
                    Thread.Sleep(30);
                    UploadFileByteWrite(bytes, position, fileName);
                //}
            }
        }
        #endregion

        #region public DataTable GetDataTableByFolder(BaseUserInfo userInfo, string folderId)
        /// <summary>
        /// 按文件夹获取文件列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>列表</returns>
        public DataTable GetDataTableByFolder(BaseUserInfo userInfo, string folderId)
        {			
			var dt = new DataTable(BaseFileEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				dt = manager.GetDataTableByFolder(folderId);
				dt.TableName = BaseFolderEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
			var dt = new DataTable(BaseFileEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				dt = manager.GetDataTable(BaseFileEntity.FieldId, ids, BaseFileEntity.FieldSortCode);
				dt.TableName = BaseFileEntity.TableName;
			});
			return dt;
        }
        #endregion

        #region public int DeleteByFolder(BaseUserInfo userInfo, string folderId)
        /// <summary>
        /// 按文件夹删除文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响行数</returns>
        public int DeleteByFolder(BaseUserInfo userInfo, string folderId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.DeleteByFolder(folderId);
			});
			return result;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, string id, string folderId, string fileName, string description, bool enabled, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id"></param>
        /// <param name="folderId"></param>
        /// <param name="fileName"></param>
        /// <param name="description"></param>
        /// <param name="enabled"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public int Update(BaseUserInfo userInfo, string id, string folderId, string fileName, string description, bool enabled, out string statusCode, out string statusMessage)
        {
			string returnCode = string.Empty;
			string returnMessage = string.Empty;
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.Update(id, folderId, fileName, description, enabled, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;

			return result;
        }
        #endregion

        #region public int UpdateFile(BaseUserInfo userInfo, string id, string fileName, byte[] file, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">文件主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件内容</param>
        /// <param name="statusCode">返回状态</param>
        /// <param name="statusMessage">反悔信息</param>
        /// <returns></returns>
        public int UpdateFile(BaseUserInfo userInfo, string id, string fileName, byte[] file, out string statusCode, out string statusMessage)
        {
			string returnCode = string.Empty;
			string returnMessage = string.Empty;
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.UpdateFile(id, fileName, file, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public int Rename(BaseUserInfo userInfo, string id, string newName, bool enabled, out string statusCode, out string statusMessage)
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="newName">新名称</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Rename(BaseUserInfo userInfo, string id, string newName, bool enabled, out string statusCode, out string statusMessage)
        {
			string returnCode = string.Empty;
			string returnMessage = string.Empty;
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				BaseFileEntity fileEntity = new BaseFileEntity();
				var manager = new BaseFileManager(dbHelper, userInfo);
				var dt = manager.GetDataTableById(id);
				fileEntity.GetSingle(dt);
				fileEntity.FileName = newName;
				fileEntity.Enabled = enabled ? 1 : 0;
				result = manager.Update(fileEntity, out returnCode);
				returnMessage = manager.GetStateMessage(returnCode);
			});
			statusCode = returnCode;
			statusMessage = returnMessage;
			return result;
        }
        #endregion

        #region public DataTable Search(BaseUserInfo userInfo, string searchValue)
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="search">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string searchValue)
        {
			var dt = new DataTable(BaseFileEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				dt = manager.Search(searchValue);
				dt.TableName = BaseFileEntity.TableName;
			});

			return dt;
        }
        #endregion

        #region public int MoveTo(BaseUserInfo userInfo, string id, string folderId)
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="folderId">移动到目录</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string id, string folderId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.MoveTo(id, folderId);
			});

			return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string folderId)
        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">文件主键</param>
        /// <param name="folderId">移动到目录主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string folderId)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				for(int i = 0; i < ids.Length; i++)
				{
					result += manager.MoveTo(ids[i], folderId);
				}
			});
			return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">文件主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.Delete(id);
			});
			return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">文件主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				for(int i = 0; i < ids.Length; i++)
				{
					result += manager.Delete(ids[i]);
				}
			});
			return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result)
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="result"></param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
			int result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
			{
				var manager = new BaseFileManager(dbHelper, userInfo);
				result = manager.BatchSave(dt);
			});
			return result;
        }
        #endregion
    }
}