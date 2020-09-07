/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

$(function () {
    BuildGrid();
});
var grid = null;
function BuildGrid() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "GetAllsys_member";
    grid = $('#grid').ligerGrid({
        columns: [
            {
                display: '查看', name: '详细', width: 80, render: function (rowdata, rowindex, value) {
                    var url = "pcenter.html?_id=";
                    return '<a href="' + url + rowdata.ID + '&Tname=pcenter_m">详细</a>';
                }, frozen: true
            },
                {
                    display: '重置密码', name: '重置', width: 80, render: function (rowdata, rowindex, value) {
                        return '<a href="javascript:reset(' + rowdata.ID + ')">重置</a>';
                    }, frozen: true
                },
                {
                    display: '状态', width: 80, name: 'state', type: "text", render: function (item) {
                        if (item.state == 1) {
                            return '<a style="display:block;background-color:#81DE8B;width:+3; height:100%;" href="javascript:EditState(' + item.ID + ',0)">启用</a>';
                        } else {
                            return '<a style="display:block;background-color:gray; height:100%;" href="javascript:EditState(' + item.ID + ',1)">禁用</a>';
                        }
                    }, frozen: true
                },
                { display: '公司编码', width: 80, name: 'co', type: "text", frozen: true },
                { display: '公司名称', width: 125, name: 'co_n', type: "text", frozen: true },
                { display: '用户名', width: 125, name: 'name', type: "text", frozen: true },
                { display: '登录账户', width: 125, name: 'account', type: "text" },
                { display: '职位', width: 125, name: 'position', type: "text" },
                { display: '最近登录', width: 200, name: 'timespan', type: "date" },
                { display: '注册时间', width: 200, name: 'rdt', type: "date" }
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
        toolbar: { items: [{ text: '添加', click: AddUser, icon: 'add' }] },
        onBeforeEdit: function (e) {
            if (e.record.__status == "nochanged") { return false; }
        }, onAfterEdit: function (e) {
            //if (e.value != "") {

            //}
            return true;
        },
        autoFilter: true
    });

}
//重置密码
function reset(uid) {
    $.ligerDialog.confirm('确定密码，重置后不能恢复？', function (confirm) {
        if (confirm) {
            var input = bif.getInput();
            input.channel = "Anno.Plugs.Logic";
            input.router = "Platform";
            input.method = "ReSetpwd";
            input.ID = uid;
            delete input.state;
            bif.process(input, function (data) {
                if (data.status) {
                    $.ligerDialog.success('重置成功');
                } else {
                    $.ligerDialog.error(data.msg);
                }
            });
        }
    });
}
//启用 停用
function EditState(uid, state) {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "EditState";
    input.ID = uid;
    input.state = state;
    var selected = grid.getSelected();
    bif.process(input, function (data) {
        if (data.status) {
            if (!state) {
                $.ligerDialog.success('已停用');
            } else {
                $.ligerDialog.success('已启用');
            }
            grid.updateRow(selected, {
                state: input.state
            });
        } else {
            $.ligerDialog.error(data.msg);
        }
    });
}
//添加用户
function AddUser() {
    window.location.href = "pcenter.html?Tname=pcenter_add";
}