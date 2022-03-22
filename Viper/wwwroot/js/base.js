var anno = {
    input: {
        profile: localStorage.token,
        uname: localStorage.account
    },
    ajaxpara: {
        async: true,
        dataType: "json",
        type: 'post',
        src: window.location.origin === undefined
            ? window.location.protocol + "//" + window.location.host + "/AnnoApi/"
            : window.location.origin + "/AnnoApi/" //兼容老版本IE origin
    },
    process: function (input, url, callback, errorCallBack) {
        window.$.ajax({
            url: anno.ajaxpara.src + url + "?t=" + new Date().getMilliseconds(),
            type: anno.ajaxpara.type,
            async: anno.ajaxpara.async,
            dataType: anno.ajaxpara.dataType,
            data: input,
            success: function (data, status) {
                if (status === "success" && data.status && (data.msg === null || data.msg === "")) {
                    callback(data, status);
                } else if (errorCallBack !== null && errorCallBack !== undefined) {
                    errorCallBack(data, status);
                } else {
                    callback(data, status);
                }
            }
        }
        );
    },
    watch: function (obj, attr, callback) {
        if (typeof obj.defaultValues === 'undefined') {
            obj.defaultValues = {};
            for (var p in obj) {
                if (typeof obj[p] !== 'object')
                    obj.defaultValues[p] = obj[p];
            }
        }
        if (typeof obj.setAttr === 'undefined') {
            obj.setAttr = function (attr, value) {
                if (this[attr] !== value) {
                    this.defaultValues[attr] = this[attr];
                    this[attr] = value;
                    return callback(this);
                }
                return this;
            };
        }
    },
    observe: function (dom, config, callback) {
        var MutationObserver =
            window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver; //浏览器兼容
        var _config = { attributes: true, childList: true }; //配置对象
        if (config !== null) {
            _config = config;
        }
        var observer = new MutationObserver(function (mutations) { //构造函数回调
            mutations.forEach(function (record) {
                callback(record);
            });
        });
        dom.each(function () {
            observer.observe(this, _config);
        });
    },
    inherit: function (p) { //对象继承
        if (p === null) throw TypeError();
        if (Object.create)
            return Object.create(p);
        var t = typeof p;
        if (t !== "object" && t !== "function") throw TypeError();

        function F() { }

        F.prototype = p;
        return new F();
    },
    cloneObj: function (obj) { //对象克隆
        var str, newobj = obj.constructor === Array ? [] : {};
        if (typeof obj !== 'object') {
            return newobj;
        } else if (window.JSON) {
            str = JSON.stringify(obj), //序列化对象
                newobj = JSON.parse(str); //还原
        } else {
            for (var i in obj) {
                newobj[i] = typeof obj[i] === 'object' ? cloneObj(obj[i]) : obj[i];
            }
        }
        return newobj;
    },
    getInput: function () {
        return this.cloneObj(this.input);
    },
    GetUrlParms: function () {
        var args = new Object();
        var query = location.search.substring(1); //获取查询串
        var pairs = query.split("&"); //在逗号处断开
        for (var i = 0; i < pairs.length; i++) {
            var pos = pairs[i].indexOf('='); //查找name=value
            if (pos === -1) continue; //如果没有找到就跳过
            var argname = pairs[i].substring(0, pos); //提取name
            var value = pairs[i].substring(pos + 1); //提取value
            args[argname] = unescape(value); //存为属性
        }
        return args;
    }
};

function LoadScriptToHead(src,callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.onload = script.onreadystatechange = function () {
        if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
          if(callback!=undefined&&callback!=null&&(typeof callback)==="function"){
              callback();
          }
        script.onload = script.onreadystatechange = null;
        }
    };
    script.src = src;
    head.appendChild(script);
}

function LoadStyleToHead(src,callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('link');
    script.rel = 'stylesheet';
    script.onload = script.onreadystatechange = function () {
        if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
          if(callback!=undefined&&callback!=null&&(typeof callback)==="function"){
              callback();
          }
        script.onload = script.onreadystatechange = null;
        }
    };
    script.href = src;
    head.appendChild(script);
}
