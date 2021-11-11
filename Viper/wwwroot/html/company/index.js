/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var vm = null, _isMobile = false;
$(function () {
    Init();
    LoadData(1,20);
});

function Init() {
    vm = new Vue({
        el: '#company',
        data: {
            form: { name: "" },
            total: 0,
            companysData: [],
            currentPage: 1,
            pagesize: 20,
            pagesizes: [10, 20, 30, 40]
        }, methods: {
            handleSizeChange: function(val) {
                this.pagesize = val;
                LoadData(vm.currentPage, val);               
            },
            handleCurrentChange: function(val) {
                this.currentPage = val;
                LoadData(val, vm.pagesize);
            },
            handleClick: function (row) {
                window.location.href = "./detail?id=row.ID";
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
            $("#company").show();
        }
    });
}
function LoadData(page,pagesize) {
    var input = anno.getInput();
    input.where = '{ "rules": [{ "field": "name", "op": "like", "value": "' + vm.form.name + '", "type": "string" }], "op": "and" }';
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
    anno.process(input,"Anno.Plugs.Logic/Platform/GetAllbif_company", function (data) {
        vm.companysData = data.Rows;
        vm.total = parseInt(data.Total);
    });
}
