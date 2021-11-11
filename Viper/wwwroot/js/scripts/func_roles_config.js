/// <reference path="../jquery.min.js" />
/// <reference path="../base.js" />
var vm = null;
var   _curTree;
var _curfunc = [];
var ds = null;
$(function () {
    vm = new Vue({
        el: '#app',
        data: {
            roleData: [],
            currentRow:null
        },
        methods: {
            indexMethod: function (index) {
                return index + 1;
            },
            handleCurrentChange: function(val) {
              this.currentRow = val;
              ClickRoles();
            },
            saveRoleFunc:function(){
                var that=this;
                if(that.currentRow===null){
                    that.$message.error("请选择角色！");
                    return;
                }
                var checkdata = _curTree.getChecked();
                var rfl = [];
                for (var i = 0; i < checkdata.length; i++) {
                    rfl.push({ ID: i, rid: that.currentRow.ID, fid: checkdata[i].data.ID });
                }
                var input = anno.getInput();
                input.inputData = JSON.stringify(rfl);
                input.rid = that.currentRow.ID;
                anno.process(input, "Anno.Plugs.Logic/Platform/ModifyRoleLink", function (data) {
                    if (data.status) {
                        ds.outputData.lrl = data.outputData;
                        that.$message({
                            showClose: true,
                            message: "保存成功",
                            type: "success",
                          });
                    } else {
                        that.$message.error(data.msg);
                    }
                });
            }
        },
        created: function () {
            var that = this;
            var input = anno.getInput();
            anno.process(input, "Anno.Plugs.Logic/Platform/Get_rfs", function (data) {
                if (data.status) {
                    ds=data;
                    that.roleData = data.outputData.lr;
                    BuildTree();
                }
            });
        }
    });
});
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
function ClickRoles() {
    var that=vm;
    _curfunc = [];
    GetRoles_func(that.currentRow.ID);
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