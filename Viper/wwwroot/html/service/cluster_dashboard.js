/// <reference path="../../js/jquery.js" />
/// <reference path="../../js/base.js" />
var vm = null, _isMobile = false;
$(function () {
    _isMobile = isMobile();
    Init();
    LoadData();
});

function Init() {
    vm = new Vue({
        el: '#cluster_dashboard',
        data: {
            isMobile: _isMobile,
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
                return types[this.getRandom(0,4,0)];
            }, getRandom: function (start, end, fixed) {
                let differ = end - start;
                let random = Math.random();
                return (start + differ * random).toFixed(fixed);
            }
        }, created: function () {
            $("#cluster_dashboard").show();
        }
    });
}
function LoadData() {
    var input = anno.getInput();
    anno.process(input, "ServiceInstance", function (data) {
        vm.services = data.outputData;
    });
}

// 判断浏览器函数
function isMobile() {
    if (window.navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i)) {
        return true;  // 移动端
    } else {
        return false;  // PC端
    }
}