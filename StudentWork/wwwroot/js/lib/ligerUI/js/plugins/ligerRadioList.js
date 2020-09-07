/**
* jQuery ligerUI 1.3.3
* 
* http://ligerui.com
*  
* Author daomi 2015 [ gd_star@163.com ] 
* 
*/
(function ($)
{

    $.fn.ligerRadioList = function (options)
    {
        return $.ligerui.run.call(this, "ligerRadioList", arguments);
    };

    $.ligerDefaults.RadioList = {
        rowSize: 3,            //每行显示元素数   
        valueField: 'id',       //值成员
        textField: 'text',      //显示成员 
        valueFieldID: null,      //隐藏域
        name: null,            //表单名 
        data: null,             //数据  
        parms: null,            //ajax提交表单 
        url: null,              //数据源URL(需返回JSON)
        urlParms: null,                     //url带参数
        ajaxContentType: null,
        ajaxType: 'post', 
        onSuccess: null,
        onError: null,
        onSelect: null,
        css: null,               //附加css  
        value: null,            //值 
        valueFieldCssClass: null
    };

    //扩展方法
    $.ligerMethos.RadioList = $.ligerMethos.RadioList || {};


    $.ligerui.controls.RadioList = function (element, options)
    {
        $.ligerui.controls.RadioList.base.constructor.call(this, element, options);
    };
    $.ligerui.controls.RadioList.ligerExtend($.ligerui.controls.Input, {
        __getType: function ()
        {
            return 'RadioList';
        },
        _extendMethods: function ()
        {
            return $.ligerMethos.RadioList;
        },
        _init: function ()
        {
            $.ligerui.controls.RadioList.base._init.call(this);
        },
        _render: function ()
        {
            var g = this, p = this.options;
            g.data = p.data;
            g.valueField = null; //隐藏域(保存值) 
             
            if ($(this.element).is(":hidden") || $(this.element).is(":text"))
            {
                g.valueField = $(this.element);
                if ($(this.element).is(":text"))
                {
                    g.valueField.hide();
                }
            }
            else if (p.valueFieldID)
            {
                g.valueField = $("#" + p.valueFieldID + ":input,[name=" + p.valueFieldID + "]:input");
                if (g.valueField.length == 0) g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = p.valueFieldID;
            }
            else
            {
                g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = g.id + "_val";
            }
            if (g.valueField[0].name == null) g.valueField[0].name = g.valueField[0].id;
            if (p.valueFieldCssClass)
            {
                g.valueField.addClass(p.valueFieldCssClass);
            }
            g.valueField.attr("data-ligerid", g.id);


            if ($(this.element).is(":hidden") || $(this.element).is(":text"))
            {
                g.radioList = $('<div></div>').insertBefore(this.element);
            } else
            {
                g.radioList = $(this.element);
            }
            g.radioList.html('<div class="l-radiolist-inner"><table cellpadding="0" cellspacing="0" border="0" class="l-radiolist-table"></table></div>').addClass("l-radiolist").append(g.valueField);
            g.radioList.table = $("table:first", g.radioList);


            p.value = g.valueField.val() || p.value;

            g.set(p);

            g._addClickEven();
        },
        destroy: function ()
        {
            if (this.radioList) this.radioList.remove();
            this.options = null;
            $.ligerui.remove(this);
        },
        clear: function ()
        {
            this._changeValue("");
            this.trigger('clear');
        },
        _setCss: function (css)
        {
            if (css)
            {
                this.radioList.addClass(css);
            }
        },
        _setDisabled: function (value)
        {
            //禁用样式
            if (value)
            {
                this.radioList.addClass('l-radiolist-disabled');
                $("input:radio", this.radioList).attr("disabled", true);
            } else
            {
                this.radioList.removeClass('l-radiolist-disabled');
                $("input:radio", this.radioList).removeAttr("disabled");
            }
        },
        _setWidth: function (value)
        {
            this.radioList.width(value);
        },
        _setHeight: function (value)
        {
            this.radioList.height(value);
        },
        indexOf: function (item)
        {
            var g = this, p = this.options;
            if (!g.data) return -1;
            for (var i = 0, l = g.data.length; i < l; i++)
            {
                if (typeof (item) == "object")
                {
                    if (g.data[i] == item) return i;
                } else
                {
                    if (g.data[i][p.valueField].toString() == item.toString()) return i;
                }
            }
            return -1;
        },
        removeItems: function (items)
        {
            var g = this;
            if (!g.data) return;
            $(items).each(function (i, item)
            {
                var index = g.indexOf(item);
                if (index == -1) return;
                g.data.splice(index, 1);
            });
            g.refresh();
        },
        removeItem: function (item)
        {
            if (!this.data) return;
            var index = this.indexOf(item);
            if (index == -1) return;
            this.data.splice(index, 1);
            this.refresh();
        },
        insertItem: function (item, index)
        {
            var g = this;
            if (!g.data) g.data = [];
            g.data.splice(index, 0, item);
            g.refresh();
        },
        addItems: function (items)
        {
            var g = this;
            if (!g.data) g.data = [];
            $(items).each(function (i, item)
            {
                g.data.push(item);
            });
            g.refresh();
        },
        addItem: function (item)
        {
            var g = this;
            if (!g.data) g.data = [];
            g.data.push(item);
            g.refresh();
        },
        _setValue: function (value)
        {
            var g = this, p = this.options;
            g.valueField.val(value);
            p.value = value;
            this._dataInit();
        },
        setValue: function (value)
        {
            this._setValue(value);
        },
        _setUrl: function (url)
        {
            var g = this, p = this.options;
            if (!url) return;
            var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
            if (urlParms)
            {
                for (name in urlParms)
                {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    url += name + "=" + urlParms[name];
                }
            }
            var parms = $.isFunction(p.parms) ? p.parms() : p.parms;
            if (p.ajaxContentType == "application/json" && typeof (parms) != "string")
            {
                parms = liger.toJSON(parms);
            }
            $.ajax({
                type: 'post',
                url: url,
                data: parms,
                cache: false,
                dataType: 'json',
                contentType: p.ajaxContentType,
                success: function (data)
                {
                    g.setData(data);
                    g.trigger('success', [data]);
                },
                error: function (XMLHttpRequest, textStatus)
                {
                    g.trigger('error', [XMLHttpRequest, textStatus]);
                }
            }); 
        },
        setUrl: function (url)
        {
            return this._setUrl(url);
        },
        setParm: function (name, value)
        {
            if (!name) return;
            var g = this;
            var parms = g.get('parms');
            if (!parms) parms = {};
            parms[name] = value;
            g.set('parms', parms);
        },
        clearContent: function ()
        {
            var g = this, p = this.options;
            $("table", g.radioList).html("");
        },
        _setData: function (data)
        {
            this.setData(data);
        },
        setData: function (data)
        {
            var g = this, p = this.options;
            if (!data || !data.length) return;
            g.data = data;
            g.refresh();
            g.updateStyle();
        },
        refresh: function ()
        {
            var g = this, p = this.options, data = this.data;
            this.clearContent();
            if (!data) return;
            var out = [], rowSize = p.rowSize, appendRowStart = false, name = p.name || g.id;
            for (var i = 0; i < data.length; i++)
            {
                var val = data[i][p.valueField], txt = data[i][p.textField], id = g.id + "-" + i;
                var newRow = i % rowSize == 0;
                //0,5,10
                if (newRow)
                {
                    if (appendRowStart) out.push('</tr>');
                    out.push("<tr>");
                    appendRowStart = true;
                }
                out.push("<td><input type='radio' name='" + name + "' value='" + val + "' id='" + id + "'/><label for='" + id + "'>" + txt + "</label></td>");
            }
            if (appendRowStart) out.push('</tr>');
            g.radioList.table.append(out.join(''));
        },
        _getValue: function ()
        {
            var g = this, p = this.options, name = p.name || g.id;
            return $('input:radio[name="' + name + '"]:checked').val();
        },
        getValue: function ()
        {
            //获取值
            return this._getValue();
        },
        updateStyle: function ()
        {
            var g = this, p = this.options;
            g._dataInit();
            $(":radio", g.element).change(function ()
            {
                var value = g.getValue();
                g.trigger('select', [{
                    value: value
                }]);
            });
        },
        _dataInit: function ()
        {
            var g = this, p = this.options;
            var value = g.valueField.val() || g._getValue() || p.value;
            g._changeValue(value);
        },
        //设置值到 隐藏域
        _changeValue: function (newValue)
        {
            var g = this, p = this.options, name = p.name || g.id;
            $("input:radio[name='" + name + "']", g.radioList).each(function ()
            {
                this.checked = this.value == newValue;
            });
            g.valueField.val(newValue);
            g.selectedValue = newValue;
        },
        _addClickEven: function ()
        {
            var g = this, p = this.options;
            //选项点击
            g.radioList.click(function (e)
            {
                var value = g.getValue();
                if (value) g.valueField.val(value);
            });
        }
    });


})(jQuery);