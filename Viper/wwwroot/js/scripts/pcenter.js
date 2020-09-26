/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

$(function () {
    Load();
});
var ds = null;
function Load() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "PCenter";
    bif.process(input, function (data) {
        if (data.status) {
            ds = data;
            InitForm();
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "GetcurRoles";
    input.uid = JSON.parse(localStorage.profile).ID;
    bif.process(input, function (data) {
        if (data.status) {
            roles = data;
            BuildGrid();
        } else {
            $.ligerDialog.error("获取角色：" + data.msg);
        }
    });
}
var groupicon = "../js/lib/ligerUI/skins/icons/communication.gif";
var form, formpwd;
function InitForm() {
    form = $("#form").ligerForm({
        fields: [
                { type: 'text', label: '用户', name: 'name', group: "基础信息", groupicon: groupicon },
                { type: 'text', label: '登录名', name: 'account', newline: false },
                { type: 'text', label: '职位', name: 'position', newline: false },
                { type: 'text', label: '状态', name: 'state_enum', newline: true },
                { type: 'text', label: '最近登录', name: 'timespan', newline: false }
        ]
                   , readonly: true
    });
    formpwd = $("#formpwd").ligerForm({
        fields: [
                { type: 'password', label: '旧密码', name: 'opwd', group: "修改密码", groupicon: groupicon },
                { type: 'password', label: '新密码', name: 'pwd', newline: false }
        ], buttons: [
                { text: '修改', width: 60, click: EditPwd }
        ]
               , readonly: false
    });
    form.setData(ds.outputData);
    if (ds.outputData.state == true) {
        $("input[name=state_enum]").val("启用");
    } else {
        $("input[name=state_enum]").val("禁用");
    }
}

function BuildGrid() {
    var griddata = { Rows: roles.outputData.cur };
    $('#grid').ligerGrid({
        columns: [
            { display: '角色编号', width: 225, name: 'ID', text: "text" },
            {
                display: '角色', width: 225, name: 'name', textField: "name"
            }
        ],
        data: griddata,
        usePager: false,
        rownumbers: true,
        enabledEdit: false,
        width: '99.7%',
        heightDiff: -4
    });

}
function EditPwd() {
    var input = bif.getInput();
    var _formdata = formpwd.getData();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "ChangePwd";
    input.dto = JSON.stringify({
        pwd: _formdata.pwd,
        oldPwd: _formdata.opwd,
        ID: JSON.parse(localStorage.profile).ID
    });
    input.opwd = _formdata.opwd;
    input.pwd = _formdata.pwd;
    bif.process(input, function (data) {
        if (data.status) {
            $.ligerDialog.success('修改成功');
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
}