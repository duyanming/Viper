/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

var groupicon = "../js/lib/ligerUI/skins/icons/communication.gif";
var form;
var rltData = null;
var statusData = [
    { Name: '禁用', show: '0' },
    { Name: '启用', show: '1' }
];
function _initform() {
    form = $("#form").ligerForm({
        inputWidth: 200, labelWidth: 90, space: 40,
        fields: [
            { type: 'text', label: '功能名称', name: 'fname', newline: true, group: "功能属性", groupicon: groupicon },
            { type: 'text', label: '功能编码', name: 'fcode', newline: true },
            { type: 'number', label: '排序编号', name: 'forder', newline: false },
            { type: 'textarea', label: '功能地址', name: 'furl', newline: true },
            {
                type: 'select', label: '是否启用', name: 'show', comboboxName: "show",
                options: { textField: "Name", valueField: "show", data: statusData }, newline: false
            },
            { type: 'textarea', label: '图标地址', name: 'icon', newline: true }
        ], buttons: [
            { text: '删除', width: 60, click: f_del },
            { text: '添加兄弟', width: 60, click: f_addX },
            { text: '添加子点', width: 60, click: f_addZ },
            { text: '更新', width: 60, click: f_save }
        ]
        //, readonly: true
    });

}
function getAllfunc() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "Get_all_power";
    bif.process(input, function (data) {
        rltData = data;
        BuildTree(data.outputData);
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
        nodeWidth: 200,
        idFieldName: 'ID',
        parentIDFieldName: 'pid',
        textFieldName: 'fname'
    });
    //Tree.collapseAll(); //折叠节点
}
var curnode = null;
function onClick(node) {
    curnode = node.data;
    form.setData(curnode);
}

function f_save() {
    if (curnode == null) {
        $.ligerDialog.question('请选择节点');
        return false;
    }
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "EditFunc";
    input.type =4;
    var _formdata = form.getData();
    curnode.fname = _formdata.fname;
    curnode.fcode = _formdata.fcode;
    curnode.forder = _formdata.forder;

    curnode.furl = _formdata.furl;
    curnode.icon = _formdata.icon;
    curnode.show = _formdata.show;
    input.inputData = JSON.stringify(curnode);
    bif.process(input, function (data) {
        if (data.status) {
            window.location = window.location;
        } else {
            $.ligerDialog.error(data.msg);
        }
    });

}
function f_add(type) {
    if (curnode == null) {
        $.ligerDialog.question('请选择节点');
        return false;
    }
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "EditFunc";
    input.type = type;
    var _formdata = form.getData();
    curnode.fname = _formdata.fname;
    curnode.fcode = _formdata.fcode;
    curnode.forder = _formdata.forder;

    curnode.furl = _formdata.furl;
    curnode.icon = _formdata.icon;
    input.inputData = JSON.stringify(curnode);
    bif.process(input, function (data) {
        if (data.status) {
            window.location = window.location;
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
}
function f_addX() {
    f_add(1);
}
function f_addZ() {
    f_add(2);
}
function f_del() {
    if (curnode == null) {
        $.ligerDialog.question('请选择节点');
        return false;
    }
    var input = bif.getInput();
    $.ligerDialog.confirm('确定删除？', function (confirm) {
        if (confirm) {
            input.channel = "Anno.Plugs.Logic";
            input.router = "Platform";
            input.method = "EditFunc";
            input.type = 3;

            input.inputData = JSON.stringify(curnode);
            bif.process(input, function (data) {
                if (data.status) {
                    window.location = window.location;
                } else {
                    $.ligerDialog.error(data.msg);
                }
            });
        }
    });
}
