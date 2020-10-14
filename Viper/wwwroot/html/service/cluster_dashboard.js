/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
varvm = null;
$(function () {
    Init();
    LoadData();
});

function Init() {
    vm = new Vue({
        el: '#cluster_dashboard',
        data: {
            services: []
        }, methods: {
            openDetail: function (nickname) {
                window.location.href = "../trace/router.html?appName=" + nickname;
            },
            resourceDetail: function (nickname) {
                window.location.href = "../welcome.html?appName=" + nickname;
            },
            getRoundType: function () {
                var types = ["", "success", "info", "danger", "warning"];
                return types[this.getRandom(0,4)];
            }, getRandom: function (start, end, fixed = 0) {
                let differ = end - start
                let random = Math.random()
                return (start + differ * random).toFixed(fixed)
            }
        }, created: function () {
            $(".box-card").show();
        }
    });
}
function LoadData() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Trace";
    input.router = "Router";
    input.method = "GetServiceInstances";

    bif.process(input, function (data) {
        vm.services = data.outputData;
    });
}