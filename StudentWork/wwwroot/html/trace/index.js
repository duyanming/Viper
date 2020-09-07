/// <reference path="../../js/jquery.min.js" />
/// <reference path="../../js/base.js" />
$(function () {
    BuildGrid();
});
var grid = null;
function BuildGrid() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Trace";
    input.router = "Trace";
    input.method = "GetTrace";
    grid = $('#grid').ligerGrid({
        columns: [
            {
                display: '查看', name: '详细', width: 60, render: function (rowdata, rowindex, value) {
                    var url = "detail.html?TraceId=";
                    return '<a href="' + url + rowdata.GlobalTraceId + '">详细</a>';
                }, frozen: true
            },
            { display: '调用方名称', width: 100, name: 'AppName', type: "text", frozen: true },
            { display: '目标服务名称', width: 100, name: 'AppNameTarget', type: "text", frozen: true },
            {
                display: '耗时(毫秒)', width: 100, name: 'UseTimeMs', type: "float", render: function (rowData, rowindex, value) {
                    if (rowData.Rlt) {
                        var colorStr = "#40de5a";
                        if (value <= 50) {
                            colorStr = "#40de5a";
                        }
                        else if (value <= 100) {
                            colorStr = "#00e09e";
                        } else if (value <= 300) {
                            colorStr = "#2edfa3";
                        }
                        else if (value <= 500) {
                            colorStr = "#a4e2c6";
                        }
                        else if (value <= 800) {
                            colorStr = "#faff72";
                        } else {
                            colorStr = "#ffa631";
                        }
                        return '<span style="display:block;background-color:' + colorStr + ';width:+3; height:100%;">' + value + '</span>';
                    } else {
                        var Msg = $.parseJSON(rowData.Response).Msg;
                        return '<span style="display:block;background-color:#ff461f; height:100%;" title="' + Msg + '">' + value + '</span>';
                    }
                }, frozen: true
            },
            { display: '调用人编号', width: 100, name: 'Uname', type: "text", frozen: true },
            { display: '目标服务', width: 200, name: 'Target', type: "text" },
            { display: '请求管道', width: 100, name: 'Askchannel', type: "text" },
            { display: '请求路由', width: 100, name: 'Askrouter', type: "text" },
            { display: '请求方法', width: 100, name: 'Askmethod', type: "text" },
            { display: '追踪标识', width: 200, name: 'TraceId', type: "text" },
            { display: '访问者IP', width: 200, name: 'Ip', type: "text" },
            { display: '调用时间', width: 200, name: 'Timespan', type: "date" }
        ],
        url: bif.ajaxpara.src,
        dataAction: 'server', //服务器排序
        pageSize: 20,
        parms: input,
        alternatingRow: true,
        usePager: true,
        rownumbers: true,
        enabledEdit: false,
        height: '100%',
        width: '99.7%',
        heightDiff: -4,
        autoFilter: true
    });
}