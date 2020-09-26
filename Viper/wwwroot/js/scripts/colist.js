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
    input.method = "GetAllbif_company";
    grid = window.$('#grid').ligerGrid({
        columns: [
            {
                display: '查看', name: '详细', width: 80, render: function (rowdata, rowindex, value) {
                    var url = "company.html?_id=";
                    return '<a href="' + url + rowdata.ID + '&Tname=pcenter_m">详细</a>';
                }, frozen: true
            },
                {
                    display: '状态', width: 80, name: 'state', type: "text", render: function (item) {
                        if (item.state === 1) {
                            return '<a style="display:block;background-color:#81DE8B;width:+3; height:100%;" href="javascript:EditState(' + item.id + ',true)">启用</a>';
                        } else {
                            return '<a style="display:block;background-color:gray; height:100%;" href="javascript:EditState(' + item.id + ',false)">禁用</a>';
                        }
                    }, frozen: true
                },
                { display: '编码', width: 80, name: 'code', type: "text", frozen: true },
                { display: '名称', width: 125, name: 'name', type: "text", frozen: true },
                { display: '地址', width: 125, name: 'address', type: "text", frozen: true },
                { display: '电话', width: 125, name: 'tel', type: "text" },
                { display: '负责人', width: 125, name: 'person', type: "text" },
                { display: '注册时间', width: 200, name: 'rdt', type: "date" }
        ],
        url: bif.ajaxpara.src,
        dataAction: 'server', //服务器排序
        pageSize: 20,
        parms: input,
        alternatingRow: false,
        usePager: true,
        rownumbers: true,
        enabledEdit: false,
        height: '100%',
        width: '99.7%',
        heightDiff: -4,
        onBeforeEdit: function (e) {
            if (e.record.__status === "nochanged") { return false; }
        }, onAfterEdit: function (e) {
            //if (e.value != "") {

            //}
            return true;
        },
        autoFilter: true
    });

}

//启用 停用
function EditState(uid, state) {
    return false;//公司列表未实现
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "ReSetpwd";
    input.uid = uid;
    input.state = (state == false);
    var selected = grid.getSelected();
    bif.process(input, function (data) {
        if (data.status) {
            if (state) {
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