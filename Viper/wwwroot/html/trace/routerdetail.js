/// <reference path="../../js/base.js" />
/// <reference path="../../js/jquery.js" />

$(function () {
    Init();
});
var grid = null;
var data = null;
function Init() {
    var dialog = frameElement.dialog; 
    data = dialog.get('data');//获取data参数
    InitForm();
    BuildGrid();
}
var groupicon = "../../js/lib/ligerUI/skins/icons/communication.gif";
var form;
function InitForm() {
    form = $("#form").ligerForm({
        fields: [
            { type: 'text', label: '服务名称', name: 'App', group: "基本信息", groupicon: groupicon },
            { type: 'text', label: '管道', name: 'Channel', newline: false },
            { type: 'text', label: '请路由', name: 'Router', newline: true },
            { type: 'text', label: '方法', name: 'Method', newline: false },
            { type: 'number', label: '参数个数', name: 'ParameterCount', newline: true },
            { type: 'textarea', width: 500, label: '描述', name: 'Desc', newline: true },
            { type: 'textarea', width: 500, label: 'DebugResult', name: 'Rlt', newline: true }
        ]
        , readonly: true
    });
    form.setData(data);
}
function BuildGrid() {
    $.each(data.Value.Parameters, function(i, item) {
        item.Pvalue = "";
    });
    var griddata = { Rows: data.Value.Parameters };
    $('#grid').ligerGrid({
        columns: [
            { display: '名称', width: 100, name: 'Name', text: "text", editor: { type: 'text' } },
            { display: '类型', width: 150, name: 'ParameterType', text: "text" },
            { display: '值', width: 233, name: 'Pvalue', text: "text", editor: { type: 'text' } },
            { display: '描述', width: 250, name: 'Desc', text: "name" }
        ],
        toolbar: {
            items: [{ text: 'Debug', click: Debug, icon: 'modify' }
                , { text: '附加参数', click: AddParameter, icon: 'add' }
                , { text: '删除参数', click: DelParameter, icon: 'delete' }]
        },
        data: griddata,
        usePager: false,
        rownumbers: true,
        enabledEdit: true,
        width: '99.7%',
        heightDiff: -4
    });

}

function DelParameter() {
    var manager = $("#grid").ligerGetGridManager();
    var row = manager.getSelectedRow();
    if (row === null) {
        $.ligerDialog.warn('请选择要删除的参数');
        return false;
    }
    $.ligerDialog.confirm('确定删除？' + row.Name,
        function(confirm) {
            if (confirm) {
                manager.deleteSelectedRow();
            }
        });
    return true;
}
//添加角色
var newRow = 1;
function AddParameter() {
    var manager = $("#grid").ligerGetGridManager();
    manager.addRow({
        Name: "Name"+newRow++,
        ParameterType: "Default",
        Pvalue: ""
    });
}
function Debug() {
    var input = bif.getInput();
    input.channel = data.Channel.substring(0,data.Channel.length-7);
    input.router = data.Router.substring(0, data.Router.length - 6);
    input.method = data.Method;
    var manager = $("#grid").ligerGetGridManager();
    var gdata = manager.getData();

    $.each(gdata, function(i, item) {
        input[item.Name] = item.Pvalue;
    });
    bif.process(input, function (ds) {
        data.Rlt = JSON.stringify(ds);
        form.setData(data);
    });
}