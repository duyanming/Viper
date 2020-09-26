/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

$(function () {
    Load();
});
var ds = null;
var args = new Object();
args = bif.GetUrlParms();
var roles = null;
function Load() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "PCenter";
    input.type = "m";
    input.id = args["_id"];
    bif.process(input, function (data) {
        if (data.status) {
            ds = data;
            InitForm();
        } else {
            $.ligerDialog.error(data.msg);
        }
    });

    input.method = "GetcurRoles";
    input.uid = args["_id"];
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
var form;
function InitForm() {
    form = $("#form").ligerForm({
        fields: [
                { type: 'text', label: '用户', name: 'name', group: "基础信息", groupicon: groupicon },
                { type: 'text', label: '登录名', name: 'account', newline: false },
                { type: 'text', label: '职位', name: 'position', newline: false },
                { type: 'text', label: '状态', name: 'state_enum', newline: true },
                { type: 'text', label: '最近登录', name: 'timespan', newline: true, readonly: false }
        ], buttons: [
               {
                   text: '返回', width: 60, click: function () {
                       window.history.go(-1);
                   }
               }
        ]
                   , readonly: true
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
                display: '角色', width: 225, name: 'name', textField: "name", editor: {
                    type: 'popup', valueField: 'name', textField: 'name', grid:
                    {
                        data: { Rows: roles.outputData.lr },
                        columns: [
                         { name: 'name', width: 350, display: '角色' }, { name: 'ID', width: 305, display: '编号' }
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
                { text: '删除', click: DelRoles, icon: 'delete' }, { text: '保存', click: Save, icon: 'save' }]
        },
        onBeforeEdit: function (e) {
            // if (e.record.__status == "nochanged") { return false; }
        }, onAfterEdit: function (e) {
            if (e.value != "") {

            }
            return true;
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
function Save() {
    var manager = $("#grid").ligerGetGridManager();
    var gdata = manager.getData();
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "SaveCurRoles";
    input.uid = args["_id"];
    input.inputData = JSON.stringify(gdata);
    bif.process(input, function (data) {
        if (data.status) {
            $.ligerDialog.success('配置成功');
            window.location = window.location;
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
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