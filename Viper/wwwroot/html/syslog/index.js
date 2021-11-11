/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var vm = null, _isMobile = false;
$(function () {
    Init();
    try {
        var args;
        args = anno.GetUrlParms();
        if (args.TraceId != undefined && args.TraceId != null) {
            vm.form.title = args.TraceId;
        }
    } catch(ex) {console.log(ex); }
    LoadData(1, 20);
});

function Init() {
    vm = new Vue({
        el: '#sys_log',
        data: {
            form: { title: "", logType:"-1" },
            total: 0,
            mData: [],
            currentPage: 1,
            pagesize: 20,
            pagesizes: [10, 20, 30, 40]
        }, methods: {
            handleSizeChange: function (val) {
                this.pagesize = val;
                LoadData(vm.currentPage, val);
            },
            handleCurrentChange: function (val) {
                this.currentPage = val;
                LoadData(val, vm.pagesize);
            },
            linkToTrack: function (row) {
                window.location.href = '../trace/detail.html?TraceId=' + row.TraceId;
            }
            , indexMethod: function (index) {
                return index + 1;
            }
            , onQuery: function () {
                vm.currentPage = 1;
                LoadData(vm.currentPage, vm.pagesize);
            },
            keyupAnno: function () {
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {
                        vm.onQuery();
                    }
                }
            }
        },
        created: function () {
            $("#sys_log").show();
            this.keyupAnno();
        }
    });
}
function LoadData(page, pagesize) {
    var input = anno.getInput();
    input.title = vm.form.title;
    input.logType = vm.form.logType;
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
    anno.process(input, "Anno.Plugs.Trace/Trace/SysLog", function (data) {
        vm.mData = data.Rows;
        vm.total = parseInt(data.Total);
    });
}
