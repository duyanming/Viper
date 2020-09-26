/// <reference path="../jquery.min.js" />
/// <reference path="base.js" />

function exitUser() {
    if (confirm("确定退出？")) {
        //input.channel = "bif.Platform";
        //input.method = "LogOut";

        //process(input, function () {});//退出清空Session
        localStorage.clear();
        bif.input.profile = undefined;
        bif.input.uname = undefined;
        window.location.href = "/";
    }
}

function getfunc() {
    var input = bif.getInput();
    input.channel = "Anno.Plugs.Logic";
    router = "Platform";
    input.method = "GetFunc";
    bif.ajaxpara.async = false;
    bif.process(input, function (data) {
        flist = data.outputData;
    });
}


function f_heightChanged(options) {
    if (tab)
        tab.addHeight(options.diff);
    if (accordion && options.middleHeight - 24 > 0)
        accordion.setHeight(options.middleHeight - 24);
}
function f_addTab(tabid, text, url) {
    tab.addTabItem({
        tabid: tabid,
        text: text,
        url: url,
        callback: function () {
           
        }
    });
}


function BindTree(func) {
    _f = [];
    gff(func.id);
    $("#" + func.fcode).ligerTree({
        data: _f,
        idFieldName :'id',
        parentIDFieldName :'pid',
        checkbox: false,
        slide: true,
        btnClickToToggleOnly: false,
        nodeWidth: 160,
        isExpand: false,
        attribute: ['nodename', 'url'],
        onSelect: function (node) {
            if (node.data === null || (!node.data.url)) return;
            var tabid = $(node.target).attr("tabid");
            if (!tabid) {
                tabid = new Date().getTime();
                $(node.target).attr("tabid", tabid);
            }
            f_addTab(tabid, node.data.text, node.data.url);

        }
    });
}
function gff(id) {
    for (var i = 0; i < flist.length; i++) {
        if (id === flist[i].pid) {
            _f.push({ id: flist[i].id, pid: flist[i].pid, text: flist[i].fname, url: flist[i].furl });
            if (flist[i].furl === null || flist[i].furl === "") {
                gff(flist[i].id);
            }
        }
    }
}

function InitHome(flist) {
    window.$("a[uname]").html(profile.name);
    var leftmenum = window.$("#accordion1");
    var mb = window.$("div[mb]").html();
    funlist.map(function (item, index) {
        var mbhtml = window.$(mb);
        mbhtml.attr("title", item.fname);
        mbhtml.find("ul").attr("id", item.fcode);
        leftmenum.append(mbhtml);
    });
}