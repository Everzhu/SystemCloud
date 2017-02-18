using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 区域
    /// </summary>
    public class tbDictRegion : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 区域
        /// </summary>
        [Display(Name = "区域"), Required]
        public string RegionName { get; set; }

        /// <summary>
        /// 上级
        /// </summary>
        [Display(Name = "上级")]
        public virtual tbDictRegion tbDictRegionParent { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        [Display(Name = "简称")]
        public string ShortName { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        [Display(Name = "级别")]
        public int LevelType { get; set; }

        /// <summary>
        /// 城市区号
        /// </summary>
        [Display(Name = "城市区号")]
        public string CityCode { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [Display(Name = "邮政编码")]
        public string ZipCode { get; set; }

        /// <summary>
        /// 合并名称
        /// </summary>
        [Display(Name = "合并名称")]
        public string MergerName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Display(Name = "经度")]
        public string Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Display(Name = "纬度")]
        public string Lat { get; set; }

        /// <summary>
        /// 拼音
        /// </summary>
        [Display(Name = "拼音")]
        public string Pinyin { get; set; }

        /// <summary>
        /// 首字母
        /// </summary>
        [Display(Name = "首字母")]
        public string FirstChar { get; set; }

        /// <summary>
        /// 首字母缩写
        /// </summary>
        [Display(Name = "首字母缩写")]
        public string ShortHand { get; set; }
    }
}