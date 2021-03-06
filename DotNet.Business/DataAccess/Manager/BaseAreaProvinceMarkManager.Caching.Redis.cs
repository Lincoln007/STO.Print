﻿//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using DotNet.Model;
    using DotNet.Utilities;

    /// <summary>
    /// BaseAreaProvinceMarkManager
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015-06-24 版本：1.0 JiRiGaLa 减少数据库的压力、空的也缓存起来。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-06-24</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaProvinceMarkManager : BaseManager, IBaseManager
    {
        private static void SetListCache(string key, List<BaseAreaProvinceMarkEntity> list)
        {
            if (!string.IsNullOrWhiteSpace(key) && list != null && list.Count > 0)
            {
                using (var redisClient = PooledRedisHelper.GetClient())
                {
                    redisClient.Set<List<BaseAreaProvinceMarkEntity>>(key, list, DateTime.Now.AddMinutes(30));
                }
            }
        }

        private static List<BaseAreaProvinceMarkEntity> GetListCache(string key)
        {
            List<BaseAreaProvinceMarkEntity> result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                using (var redisClient = PooledRedisHelper.GetClient())
                {
                    result = redisClient.Get<List<BaseAreaProvinceMarkEntity>>(key);
                }
            }

            return result;
        }

        public static List<BaseAreaProvinceMarkEntity> GetAreaRouteMarkByCache(string parentId)
        {
            List<BaseAreaProvinceMarkEntity> result = null;

            int areaId = 0;
            if (!ValidateUtil.IsInt(parentId))
            {
                return result;
            }
            else
            {
                areaId = int.Parse(parentId);
            }

            string key = "ProvinceMark";
            if (!string.IsNullOrEmpty(parentId))
            {
                key = key + areaId;
            }

            List<BaseAreaProvinceMarkEntity> provinceMarkList = GetListCache(key);
            if (provinceMarkList == null)
            {
                BaseAreaProvinceMarkManager manager = new DotNet.Business.BaseAreaProvinceMarkManager();
                provinceMarkList = manager.GetListByArea(areaId);
                // provinceMarkList = manager.GetList();
                // 若是空的不用缓存，继续读取实体
                if (provinceMarkList == null)
                {
                    // 这里提高缓存的保存效率，空也保存起来、减少数据库的压力
                    provinceMarkList = new List<BaseAreaProvinceMarkEntity>();
                }
                SetListCache(key, provinceMarkList);
            }
            if (provinceMarkList != null)
            {
                result = provinceMarkList.Where((t => t.AreaId == areaId && t.Enabled == 1)).ToList<BaseAreaProvinceMarkEntity>();
            }

            return result;
        }
    }
}
