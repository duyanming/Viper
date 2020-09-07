using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.QueryServices.Platform
{
    public class PageParameter
    {
        /// <summary>
        /// 每一页的数据条数
        /// </summary>
        public int Pagesize { get; set; } = 20;

        /// <summary>
        /// 第几页
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 排序的数据列 列名称
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// 排序 方式 ASC升序， DESC降序
        /// </summary>
        public string SortOrder { get; set; } = "desc";

        /// <summary>
        /// 过滤条件
        /// </summary>
        public string Where { get; set; }

    }
}
