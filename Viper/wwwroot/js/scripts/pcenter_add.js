/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

$(function () {
    Load();
});
var ds = null;
var roles = null;
var statusData = [
        { Name: '禁用', state: '0' },
        { Name: '启用', state: '1' }
];
function Load() {
    var input = bif.getInput();
    InitForm();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "GetcurRoles";
    input.uid = -1;
    bif.process(input, function (data) {
        if (data.status) {
            roles = data;
            BuildGrid();
        } else {
            $.ligerDialog.error(data.msg);
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
                {
                    type: 'select', label: '是否启用', name: 'state', comboboxName: "state",
                    options: { textField: "Name", valueField: "state", data: statusData }, newline: false
                },
                { type: 'password', label: '密码', name: 'pwd', newline: false }
        ], buttons: [
       { text: '保存', width: 60, click: Save }
        ]
                   , readonly: false
    });
    form.setData({ state: 1 });
}

function BuildGrid() {
    var griddata = null;
    $('#grid').ligerGrid({
        columns: [
            { display: '角色编号', width: 225, name: 'ID', text: "text" },
            {
                display: '角色', width: 225, name: 'name', textField: "name", editor: {
                    type: 'popup', valueField: 'name', textField: 'name', grid:
                            {
                                data: { Rows: roles.outputData.lr }, columns: [
                                { name: 'name', width: 200, display: 'name' }, { name: 'ID', width: 200, display: '编号' }
                                ]
                            },
                    condition: {
                        prefixID: 'name',
                        fields: [
                            { name: 'name', type: 'text', label: '角色', width: 200 }
                        ]
                    }, onSelected: f_onSelected,
                    searchClick: function (e) {
                        e.grid.loadData($.ligerFilter.getFilterFunction(e.rules));
                    }

                }
            }
        ],
        data: griddata,
        usePager: false,
        rownumbers: true,
        enabledEdit: true,
        width: '99.7%',
        heightDiff: -4,
        toolbar: {
            items: [{ text: '添加', click: AddRoles, icon: 'add' },
                { text: '删除', click: DelRoles, icon: 'delete' }]
        }
    });

}
function Save() {
    var grid = $("#grid").ligerGetGridManager();
    var griddata = grid.getData();
    var formdata = form.getData();
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "AddUser";
    input.ubase = JSON.stringify(formdata);
    input.uroles = JSON.stringify(griddata);
    bif.process(input, function (data) {
        if (data.status) {
            $.ligerDialog.success('保存成功');
            window.location.href = "pcenter.html?_id=" + data.outputData.id + "&Tname=pcenter_m";
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
}



//添加角色
var newrow = -1001;
function AddRoles() {
    var manager = $("#grid").ligerGetGridManager();
    row = {
        __id: "x1001",
        __index: 0,
        __nextid: "x1002",
        __previd: -1,
        __status: "nochanged",
        ID: -1,
        name: "-1"
    };
    manager.addEditRow({
        ID: newrow--,
        name: ""
    }, row, false);
}
function DelRoles() {
    var manager = $("#grid").ligerGetGridManager();
    var row = manager.getSelectedRow();
    if (row == null) {
        $.ligerDialog.warn('请选择要删除的角色');
        return false;
    }
    $.ligerDialog.confirm('确定删除？' + row.name, function (confirm) {
        if (confirm) {
            manager.deleteSelectedRow();
        }
    })
}

function f_onSelected(e) {
    if (!e.data || !e.data.length) return;
    var grid = $("#grid").ligerGetGridManager();
    var selected = e.data[0];
    grid.updateRow(grid.lastEditRow, {
        ID: selected.ID,
        name: selected.name
    });
}