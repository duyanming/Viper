/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var rlt = {};

function LoadData(appName) {
    var input = anno.getInput();
    if (appName !== undefined && appName !== null) {
        input.appName = appName;
    } else {
        input.appName = vm.appName;
    }

    anno.process(input,"Anno.Plugs.Trace/Router/GetRoutingInfo", function (data) {
        rlt.Rows = data.outputData;
        rlt.Total = data.outputData.length;
        BuildGrid(rlt);
    });
}
var grid = null;
function BuildGrid(data) {
    $.each(data.Rows,
        function(i,item) {
            item.ParameterCount = item.Value.Parameters.length;
        });
    grid = window.$('#grid').ligerGrid({
        columns: [
            {
                display: '查看', name: '详细', width: 60, render: function (rowdata, rowindex, value) {
                    return '<a  style="color: #409EFF;" href="javascript:openDetail(' + rowindex + ')">详细</a>';
                }
            },
            { display: '服务名称', width: 120, name: 'App', type: "text" },
            { display: '管道', width: 200, name: 'Channel', type: "text" },
            { display: '请路由', width: 200, name: 'Router', type: "text" },
            { display: '方法', width: 150, name: 'Method', type: "text" },
            { display: '参数个数', width: 94, name: 'ParameterCount', type: "number" },
            { display: '描述', name: 'Desc', width: 250 }
        ],
        headerRowHeight: 36,
        rowHeight: 45,
        isScroll: false,
        frozen: false,
        showTitle: false,
        data: data,
        detail: { onShowDetail: showParameters,height:'auto'},
        alternatingRow: true,
        usePager: false,
        rownumbers: false,
        enabledEdit: false,
        height: '100%',
        width: '99.7%',
        heightDiff: -4,
        autoFilter: true
    });
}

function getParametersData(row) {
    var data = { Rows:[]};
    data.Rows = row.Value.Parameters;
    return data;
}

//显示参数信息
function showParameters(row, detailPanel, callback) {
    var subGrid = $("<div></div>");
    window.$(detailPanel).append(subGrid);
    window.$(subGrid).css('margin', 10).ligerGrid({
        columns:
            [
                { display: '参数名称', name: 'Name', type: 'text', width: 200 },
                { display: '位置顺序', name: 'Position', type: 'text', width: 50 },
                { display: '参数类型', name: 'ParameterType', width: 200 },
                { display: '描述', name: 'Desc', width: 300 }
            ],
        headerRowHeight: 36,
        rowHeight: 45,
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
    $.ligerDialog.open({
        height: 500,
        width: 800,
        title: '路由详情：'+data.Desc,
        url: 'routerdetail.html',
        showMax: false,
        showToggle: true,
        showMin: false,
        isResize: true,
        slide: false,
        data:data
        //自定义参数
        //,myDataName: $("#txtValue").val()
    });
} 