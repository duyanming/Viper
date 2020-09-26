/// <reference path="../../js/jquery.min.js" />
/// <reference path="../../js/base.js" />



var groupicon = "../js/lib/ligerUI/skins/icons/communication.gif";
var form;
var rltData = null;
var Tree;
function _initform() {
    var args = new Object();
    args = bif.GetUrlParms();

    var input = bif.getInput();
    input.channel = "Anno.Plugs.Trace";
    input.router = "Trace";
    input.method = "GetTraceByGlobalTraceId";
    input.GId = args["TraceId"];
    bif.process(input, function (data) {
        rltData = data;
        //BuildTree(data.outputData);
        BuildTreeTable(data);
    });
}

/*
   绑定功能树
   */
function BuildTree(dataTree) {
    Tree = $("#funTree").ligerTree({
        data: dataTree,
        checkbox: false,
        btnClickToToggleOnly: true,
        onClick: onClick,
        nodeWidth: 600,
        idFieldName: 'TraceId',
        parentIDFieldName: 'PreTraceId',
        textFieldName: 'AppName'
    });
    //Tree.collapseAll(); //折叠节点
}

function BuildTreeTable(dataTree) {
    Tree = $("#funTree").ligerGrid({
            columns: [
                { display: '调用方名称', name: 'AppName', id: 'TraceId', width: 250, align: 'left', frozen: true },
            { display: '目标服务名称', name: 'AppNameTarget', width: 100, align: 'left', type: "text", frozen: true },
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
                        return '<span style="display:block;background-color:#ff461f; height:100%;" title="' + Msg+'">' + value + '</span>';
                    }
                }, frozen: true
            },
            { display: '调用人编号', width: 100, name: 'Uname', type: "text", frozen: true },
            { display: '目标服务', name: 'Target', id: 'TraceId2', width: 250, align: 'left' },
                { display: '请求管道', width: 100, name: 'Askchannel', type: "text" },
                { display: '请求路由', width: 100, name: 'Askrouter', type: "text" },
                { display: '请求方法', width: 100, name: 'Askmethod', type: "text" },
                { display: '追踪标识', width: 200, name: 'TraceId', type: "text" },
                { display: '访问者IP', width: 200, name: 'Ip', type: "text" },
                { display: '调用时间', width: 200, name: 'Timespan', type: "date" }
        ], width: '100%',
            height: '99%',
            usePager:false,
            data: dataTree, alternatingRow: false, tree: {
                columnId: 'TraceId',
                idField: 'TraceId',
                parentIDField: 'PreTraceId'
            }
        }
    );
}

function onClick(node) {
    var str = JSON.stringify(node.data);
    window.$.ligerDialog.show({
        content: str,
        width: 500,
        height: 300,
        isResize:true
    });
}