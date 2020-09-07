/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/9/7 13:32:18 
Functional description： ReportModule
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.LogicService
{
    using Anno.Const.Attribute;
    using Anno.EngineData;
    using Anno.Plugs.LogicService.Dto;
    using Infrastructure;
    using System.Linq;

    public class ReportModule : BaseModule
    {
        public ReportModule()
        {
        }
        [AnnoInfo(Desc = "获取指定时间段内的{startDate} {endDate}服务访问次数")]
        public ActionResult GetServiceReport()
        {
            StringBuilder queryStr = new StringBuilder();
            DateTime startDate = (RequestDateTime("startDate") ?? DateTime.Today.AddDays(-6)).Date;
            DateTime endDate = (RequestDateTime("endDate") ?? DateTime.Now.AddDays(1));
            if (startDate > endDate)
            {
                return new ActionResult(false, null, null, "统计开始日期不能小于结束日期");
            }
            queryStr.AppendFormat(@"SELECT AppName ,count(1) as Count FROM(
                select AppName ,id FROM sys_trace WHERE Timespan>=@startDate and Timespan<=@endDate
                UNION ALL
                select AppNameTarget as AppName,id FROM sys_trace  WHERE Timespan>=@startDate and Timespan<=@endDate
                ) a GROUP BY AppName;");
            var reportData = DbInstance.Db.Ado.SqlQuery<TraceDto>(queryStr.ToString(), new { startDate, endDate }).ToList();
            return new ActionResult(true, new { xAxis = reportData.Select(t => t.AppName).ToList(), values = reportData.Select(t => t.count).ToList() });
        }
    }
}
