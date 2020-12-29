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
                window.location.href = '../pcenter.html?_id=' + row.ID+'&Tname=pcenter_m';
            }, indexMethod: function(index) {
                return index +1;
            },
            //启用 停用
            EditState: function (row, state) {
                var that = this;
                var input = bif.getInput();
                input.channel = "Anno.Plugs.Logic";
                input.router = "Platform";
                input.method = "EditState";
                input.ID = row.ID;
                input.state = state;
                bif.process(input, function (data) {
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
                            var input = bif.getInput();
                            input.channel = "Anno.Plugs.Logic";
                            input.router = "Platform";
                            input.method = "ReSetpwd";
                            input.ID = row.ID;
                            delete input.state;
                            bif.process(input, function (data) {
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
                window.location.href = '../pcenter.html?Tname=pcenter_add';
            }
        }, created: function () {
            $("#member").show();
        }
    });
}
function LoadData(page,pagesize) {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    input.router = "Platform";
    input.method = "GetAllsys_member";
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
    bif.process(input, function (data) {
        vm.mData = data.Rows;
        vm.total = parseInt(data.Total);
    });
}
