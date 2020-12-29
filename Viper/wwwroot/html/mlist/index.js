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
            handleSizeChange(val) {
                this.pagesize = val;
                LoadData(vm.currentPage,val);
                //console.log(`每页 ${val} 条`);
            },
            handleCurrentChange(val) {
                //console.log(`当前页: ${val}`);
                this.currentPage = val;
                LoadData(val, vm.pagesize);
            },
            handleClick: function (row) {
                window.location.href = "./detail?id=row.ID";
            }, tableRowClassName({ row, rowIndex }) {
                if (row.state === "0") {
                    return 'warning-row';
                }
                return '';
            },indexMethod(index) {
                return index +1;
            },
            //启用 停用
            EditState: function (uid, state) {
                this.$alert('公司列表未实现', '提示', {
                    confirmButtonText: '确定',
                    callback: function (action) {
                        this.$message({
                            type: 'info',
                            message: 'action:' + action
                        });
                    }
                });
                return false;//公司列表未实现

            }
            , onQuery: function () {
                vm.currentPage = 1;
                LoadData(vm.currentPage, vm.pagesize);
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
