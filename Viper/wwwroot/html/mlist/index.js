/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var vm = null, _isMobile = false;
$(function () {
    Init();
    LoadData(1,20);
});

function Init() {
    vm = new Vue({
        el: '#member',
        data: {
            form: { account: "" },
            total: 0,
            mData: [],
            currentPage: 1,
            pagesize: 20,
            pagesizes: [10, 20, 30, 40]
        }, methods: {
            handleSizeChange: function(val) {
                this.pagesize = val;
                LoadData(vm.currentPage,val);
                //console.log(`每页 ${val} 条`);
            },
            handleCurrentChange: function(val) {
                //console.log(`当前页: ${val}`);
                this.currentPage = val;
                LoadData(val, vm.pagesize);
            },
            handleClick: function (row) {
                window.location.href = '../component.html?anno_component_name=anno-user-manager&_id=' + row.ID;
            }, indexMethod: function(index) {
                return index +1;
            },
            //启用 停用
            EditState: function (row, state) {
                var that = this;
                var input = anno.getInput();
                input.ID = row.ID;
                input.state = state;
                anno.process(input,"Anno.Plugs.Logic/Platform/EditState", function (data) {
                    if (data.status) {
                        row.state = state;
                        if (state===0) {
                            that.$message({
                                type: 'success',
                                duration: 1500,
                                message: '已停用!'
                            });
                        } else {
                            that.$message({
                                type: 'success',
                                duration: 1500,
                                message: '已启用!'
                            });
                        }                       
                    } else {
                        that.$message({
                            type: 'error',
                            duration: 1500,
                            message: data.msg
                        });
                    }
                });
            }
            , reSet: function (row) {
                var that = this;
                that.$confirm('确定密码，重置后不能恢复？', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning',
                    callback: function (action,instance) {
                        if (action === "confirm") {
                            var input = anno.getInput();
                            input.ID = row.ID;
                            delete input.state;
                            anno.process(input, "Anno.Plugs.Logic/Platform/ReSetpwd", function (data) {
                                if (data.status) {
                                    that.$message({
                                        type: 'success',
                                        duration: 1500,
                                        message: '重置成功!'
                                    });
                                } else {
                                    that.$message({
                                        type: 'error',
                                        duration: 1500,
                                        message: data.msg
                                    });
                                }
                            });
                        }
                    }
                });
            }
            , onQuery: function () {
                vm.currentPage = 1;
                LoadData(vm.currentPage, vm.pagesize);
            }, AddUser: function () {
                window.location.href = '../component.html?anno_component_name=anno-user-add';
            }
        }, created: function () {
            $("#member").show();
        }
    });
}
function LoadData(page,pagesize) {
    var input = anno.getInput();
    input.where = '{ "rules": [{ "field": "account", "op": "like", "value": "' + vm.form.account + '", "type": "string" }], "op": "and" }';
    if (page !== null && page !== undefined) {
        input.page = page;
    } else {
        input.page = 1;
    }
    if (pagesize !== null && pagesize !== undefined) {
        input.pagesize = pagesize;
    } else {

        input.pagesize = 20;
    }
    anno.process(input, "Anno.Plugs.Logic/Platform/GetAllsys_member", function (data) {
        vm.mData = data.Rows;
        vm.total = parseInt(data.Total);
    });
}
