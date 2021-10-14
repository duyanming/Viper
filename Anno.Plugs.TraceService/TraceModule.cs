using Anno.EngineData;
using System;
using System.Linq;
using Anno.Const.Attribute;
using SqlSugar;

namespace Anno.Plugs.TraceService
{
    using Anno.Model;
    using System.Collections.Generic;
    /// <summary>
    /// 调用链或者日志用户都可以根据自己的需求重写
    /// 比如写入 ElasticSearch
    /// </summary>
    public class TraceModule : BaseModule
    {
        private readonly SqlSugarClient _db;
        public TraceModule()
        {
            _db = Infrastructure.DbInstance.Db;
        }
        /// <summary>
        /// 批量接收追踪信息
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "持久化链路信息")]
        public ActionResult TraceBatch()
        {
            List<sys_trace> traces = Request<List<sys_trace>>("traces");
            traces.ForEach(t => { t.ID = IdWorker.NextId(); });
            _db.Insertable<sys_trace>(traces).ExecuteCommand();
            return new ActionResult(true, null, null, null);
        }
        /// <summary>
        /// 获取调用链列表信息
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取调用链列表信息")]
        public ActionResult GetTrace()
        {
            PageParameter pageParameter = new PageParameter
            {
                Page = RequestInt32("page") ?? 1,
                Pagesize = RequestInt32("pagesize") ?? 20,
                SortName = RequestString("sortname") ?? "Timespan",
                SortOrder = RequestString("sortorder") ?? "desc",
                Where = Filter()
            };
            pageParameter.SortName?.Replace(Table, ".");
            pageParameter.SortOrder?.Replace(Table, ".");
            var totalNumber = 0;
            if (string.IsNullOrWhiteSpace(pageParameter.SortName))
            {
                pageParameter.SortName = "Timespan";
            }
            if (string.IsNullOrWhiteSpace(pageParameter.SortOrder))
            {
                pageParameter.SortOrder = "desc";
            }
            var dt = _db.Queryable<sys_trace>().Where(t => t.PreTraceId == null || t.PreTraceId == "")
                .Where(pageParameter.Where)
                .OrderBy($"{pageParameter.SortName} {pageParameter.SortOrder}")
                .ToPageList(pageParameter.Page, pageParameter.Pagesize, ref totalNumber);
            var output = new Dictionary<string, object> { { "#Total", totalNumber }, { "#Rows", dt } };
            return new ActionResult(true, dt, output);
        }
        /// <summary>
        /// 根据GlobalTraceId 查看单个调用链明细
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "根据GlobalTraceId 查看单个调用链明细")]
        public ActionResult GetTraceByGlobalTraceId()
        {
            string tid = RequestString("TraceId");
            List<sys_trace> ts;
            if (string.IsNullOrWhiteSpace(tid))
            {
                string gId = RequestString("GId");
                ts = _db.Ado.SqlQuery<sys_trace>($"select * from sys_trace where GlobalTraceId=@gId;", new { gId }).ToList();
            }
            else
            {
                string sql = @"SELECT  * FROM  sys_trace 
WHERE  GlobalTraceId in(SELECT  c.GlobalTraceId FROM sys_trace as c WHERE c.TraceId=@tid)";
                ts = _db.Ado.SqlQuery<sys_trace>(sql, new { tid }).ToList();
            }
            var output = new Dictionary<string, object> { { "#Total", ts.Count }, { "#Rows", ts } };
            return new ActionResult(true, null, output);
        }

        #region 用户日志
        /// <summary>
        /// 批量写入日志
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public ActionResult LogBatch(List<sys_log> logs)
        {
            logs.ForEach(t => { t.ID = IdWorker.NextId(); });
            _db.Insertable(logs).ExecuteCommand();
            return new ActionResult();
        }

        /// <summary>
        /// 获取用户日志
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取用户日志")]
        public ActionResult SysLog()
        {
            PageParameter pageParameter = new PageParameter
            {
                Page = RequestInt32("page") ?? 1,
                Pagesize = RequestInt32("pagesize") ?? 20,
                SortName = RequestString("sortname") ?? "Timespan",
                SortOrder = RequestString("sortorder") ?? "desc",
                Where = Filter()
            };
            pageParameter.SortName?.Replace(Table, ".");
            pageParameter.SortOrder?.Replace(Table, ".");
            var totalNumber = 0;
            if (string.IsNullOrWhiteSpace(pageParameter.SortName))
            {
                pageParameter.SortName = "Timespan";
            }
            if (string.IsNullOrWhiteSpace(pageParameter.SortOrder))
            {
                pageParameter.SortOrder = "desc";
            }
            var title = this.RequestString("title");
            var logType = this.RequestInt32("logType");
            var dt = _db.Queryable<sys_log>()
                .Where(pageParameter.Where)
                .WhereIF(!string.IsNullOrWhiteSpace(title), it => it.Title.Contains(title) || it.Content.Contains(title) || it.TraceId.Contains(title))
                .WhereIF(logType != null && logType > -1, it => it.LogType.Equals(logType))
                .OrderBy($"{pageParameter.SortName} {pageParameter.SortOrder}")
                .ToPageList(pageParameter.Page, pageParameter.Pagesize, ref totalNumber);
            var output = new Dictionary<string, object> { { "#Total", totalNumber }, { "#Rows", dt } };
            return new ActionResult(true, dt, output);
        }
        #endregion
        #region  Module 初始化
        public override bool Init(Dictionary<string, string> input)
        {
            base.Init(input);
            return true;
        }
        #endregion
    }
}
