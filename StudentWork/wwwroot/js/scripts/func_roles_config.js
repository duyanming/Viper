/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />
$(function () {
    load();
});
var _curSelectedRole,
    _curTree;
var _curfunc = [];
var ds = null;
function load(pars) {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "Get_rfs";
    bif.process(input, function (data) {
        if (data.status) {
            ds = data;
            BuildGrid();
            BuildTree();
        }
    });
}

/*
绑定角色列表
*/
function BuildGrid() {
    var griddata = { Rows: ds.outputData.lr };
    $('#roleGrid').ligerGrid({
        columns: [
                { display: '', name: 'ID', hide: 1, width: 1 },
                { display: '角色名称', width: 225, name: 'name', type: "text", editor: { type: 'text' } }
        ],
        data: griddata,
        usePager: false,
        rownumbers: false,
        enabledEdit: true,
        height: '100%',
        width: '228',
        heightDiff: 25,
        toolbar: { items: [{ text: '添加', click: AddRoles, icon: 'add' }, { text: '删除', click: DelRoles, icon: 'delete' }, { text: '配置功能', click: Save, icon: 'save' }] },
        onSelectRow: function (rowdata, rowindex, rowDomEle) {
            ClickRoles(rowdata, rowindex, rowDomEle)
        }, onBeforeEdit: function (e) {
            if (e.record.__status == "nochanged") { return false; }
        }, onAfterEdit: function (e) {
            if (e.value != "") {
                var input = bif.getInput();
                input.method = "AddRole";
                input.Name = e.record.name;
                bif.process(input, function (data) {
                    if (data.status) {
                        e.record.__status = "nochanged";
                        e.record.ID = data.outputData.ID;
                        $.ligerDialog.success('角色添加成功');
                    } else {
                        $.ligerDialog.error(data.msg);
                    }
                });
            }
            return true;
        }
    });

}

//添加角色
var newrow = 1001;
function AddRoles() {
    var manager = $("#roleGrid").ligerGetGridManager();
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
        ID: newrow++,
        name: "System"
    }, row, false);
}
/*
删除角色
*/
function DelRoles() {
    var manager = $("#roleGrid").ligerGetGridManager();
    var row = manager.getSelectedRow();
    if (row == null) {
        $.ligerDialog.warn('请选择要删除的角色');
        return false;
    }
    $.ligerDialog.confirm('确定删除？', function (confirm) {
        if (confirm) {
            var input = bif.getInput();
            input.method = "DelRole";
            input.ID = row.ID;
            bif.process(input, function (data) {
                if (data.status) {
                    manager.deleteSelectedRow();
                } else {
                    $.ligerDialog.error(data.msg);
                }
            });
        }
    });
}

function Save() {
    var checkdata = _curTree.getChecked();
    var rfl = [];
    for (var i = 0; i < checkdata.length; i++) {
        rfl.push({ ID: i, rid: _curSelectedRole.ID, fid: checkdata[i].data.ID });
    }
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "ModifyRoleLink";
    input.inputData = JSON.stringify(rfl);
    input.rid = _curSelectedRole.ID;
    bif.process(input, function (data) {
        if (data.status) {
            ds.outputData.lrl = data.outputData;
            $.ligerDialog.success('配置成功');
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
}
/*
绑定功能树
*/
function BuildTree() {
    $("#funTree").html("");
    _curTree = null;
    _curTree = $("#funTree").ligerTree({
        data: ds.outputData.lf,
        checkbox: true,
        nodeWidth: 300,
        idFieldName: 'ID',
        btnClickToToggleOnly: false,
        enabledCompleteCheckbox: false,
        parentIDFieldName: 'pid',
        textFieldName: 'fname'
    });
    // _curTree.collapseAll();//折叠节点
}
/*
根据功能ID串，选择功能节点
*/
function ClickRoles(rowdata, rowindex, rowDomEle) {
    _curSelectedRole = rowdata;
    _curfunc = [];
    GetRoles_func(rowdata.ID);
    _curTree.selectNode(function (data) {
        for (var i = 0; i < _curfunc.length; i++) {
            if (_curfunc[i].fid === data.ID) {
                return true;
            }
        }
        _curTree.cancelSelect(data);
        return false;
    });
}

function GetRoles_func(rid) {
    for (var i = 0; i < ds.outputData.lrl.length; i++) {
        var _cf = ds.outputData.lrl[i];
        if (_cf.rid == rid) {
            _curfunc.push(_cf);
        }
    }
}