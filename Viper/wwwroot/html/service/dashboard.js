/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var rlt = {};
$(function () {
    LoadData();
});

function LoadData() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Trace";
    input.router = "Router";
    input.method = "GetServiceInstances";

    bif.process(input, function (data) {
        rlt.Rows = data.outputData;
        rlt.Total = data.outputData.length;
        BuildGrid(rlt);
    });
}
var grid = null;
function BuildGrid(data) {
    $.each(data.Rows,
        function(i,item) {
            item.ParameterCount = item.Tags.length;
        });
    grid = window.$('#grid').ligerGrid({
        columns: [   
            {
                display: '服务API', name: 'API详细', width: 60, render: function (rowdata, rowindex, value) {
                    return '<a href="javascript:openDetail(' + rowindex + ')">API详细</a>';
                }
            },
            {
                display: '服务资源', name: '资源详细', width: 60, render: function (rowdata, rowindex, value) {
                    return '<a href="javascript:resourceDetail(' + rowindex + ')">资源详细</a>';
                }
            },
            { display: '服务名称', width: 300, name: 'Nickname', type: "text" },
            { display: '服务地址', width: 200, name: 'Host', type: "text" },
            { display: '服务端口', width: 150, name: 'Port', type: "number" },
            { display: '超时时间(毫秒)', width: 160, name: 'Timeout', type: "number" },
            { display: '服务权重', width: 160, name: 'Weight', type: "number" }
        ],
        isScroll: false,
        frozen: false,
        showTitle: false,
        data: data,
        detail: { onShowDetail: showParameters,height:'auto'},
        alternatingRow: true,
        usePager: false,
        rownumbers: true,
        enabledEdit: false,
        height: '100%',
        width: '99.7%',
        heightDiff: -4,
        autoFilter: true
    });
}

function getParametersData(row) {
    var data = { Rows: [] };
    for (var i = 0; i < row.Tags.length; i++) {
        data.Rows.push({ Name: row.Tags[i]});
    }
    return data;
}

//显示参数信息
function showParameters(row, detailPanel, callback) {
    var subGrid = $("<div></div>");
    window.$(detailPanel).append(subGrid);
    window.$(subGrid).css('margin', 10).ligerGrid({
        columns:
            [
                { display: '路由标记', name: 'Name', type: 'text', width: 770 }
            ],
        isScroll: false,
        showToggleColBtn: false,
        height: '400px',
        data: getParametersData(row),
        showTitle: false,
        onAfterShowData: callback,
        frozen: false,
        usePager: false
       
    });
}

function openDetail(index) {
    var data = rlt.Rows[index];
    window.location.href = "../trace/router.html?appName=" + data.Nickname;   
} 
function resourceDetail(index) {
    var data = rlt.Rows[index];
    window.location.href = "../welcome.html?appName=" + data.Nickname;
}