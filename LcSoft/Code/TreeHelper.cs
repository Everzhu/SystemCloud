using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Code
{
    public class TreeHelper
    {
        /// <summary>
        /// 主Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public int pId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isChecked { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool open { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool chkDisabled { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeHelper> children { get; set; }
    }
}
