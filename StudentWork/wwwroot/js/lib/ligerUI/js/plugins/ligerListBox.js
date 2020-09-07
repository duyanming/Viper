/**
* jQuery ligerUI 1.3.2
* 
* http://ligerui.com
*  
* Author daomi 2015 [ gd_star@163.com ] 
* 
*/
(function ($)
{

    $.fn.ligerListBox = function (options)
    {
        return $.ligerui.run.call(this, "ligerListBox", arguments);
    }; 

    $.ligerDefaults.ListBox = {
        isMultiSelect: false,   //是否多选
        isShowCheckBox: false,  //是否选择复选框
        columns: null,          //表格状态
        width: 150,            //宽度
        height: 100,           //高度
        onSelect: false,        //选择前事件
        onSelected: null,       //选择值事件  
        valueField: 'id',       //值成员
        textField: 'text',      //显示成员
        valueFieldID: null,     //值 隐藏域 表单名 
        split: ";",             //分隔符
        data: null,             //数据  
        parms: null,            //ajax提交表单 
        url: null,              //数据源URL(需返回JSON)
        urlParms: null,                     //url带参数
        ajaxContentType: null,
        ajaxType : 'post',
        onSuccess: null,
        onError: null,
        render: null,            //显示html自定义函数 
        css: null,               //附加css  
        value: null,            //值 
        valueFieldCssClass: null
    };

    //扩展方法
    $.ligerMethos.ListBox = $.ligerMethos.ListBox || {};


    $.ligerui.controls.ListBox = function (element, options)
    {
        $.ligerui.controls.ListBox.base.constructor.call(this, element, options);
    };
    $.ligerui.controls.ListBox.ligerExtend($.ligerui.controls.Input, {
        __getType: function ()
        {
            return 'ListBox';
        },
        _extendMethods: function ()
        {
            return $.ligerMethos.ListBox;
        },
        _init: function ()
        {
            $.ligerui.controls.ListBox.base._init.call(this); 
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
                g.selectBox = $('<div></div>').insertBefore(this.element);
            } else
            {
                g.selectBox = $(this.element);
            }
            g.selectBox.html('<div class="l-listbox-inner"><table cellpadding="0" cellspacing="0" border="0" class="l-listbox-table"></table></div>').addClass("l-listbox").append(g.valueField);
            g.selectBox.table = $("table:first", g.selectBox); 
              
            g.set(p); 

            g._addClickEven();
        },
        destroy: function ()
        { 
            if (this.selectBox) this.selectBox.remove();
            this.options = null;
            $.ligerui.remove(this);
        },
        clear : function()
        {
            this._changeValue("");
            this.trigger('clear');
        },
        _setIsShowCheckBox : function(value)
        {
            if (value)
            {
                $("table", this.selectBox).addClass("l-table-checkbox");
            } else
            { 
                $("table", this.selectBox).addClass("l-table-nocheckbox");
            }
        },
        _setCss: function (css)
        {
            if (css)
            {
                this.selectBox.addClass(css);
            } 
        }, 
        _setDisabled: function (value)
        {
            //禁用样式
            if (value)
            {
                this.selectBox.addClass('l-text-disabled');
            } else
            {
                this.selectBox.removeClass('l-text-disabled');
            }
        }, 
        _setWidth: function (value)
        {
            this.selectBox.width(value);
        },
        _setHeight: function (value)
        {
            this.selectBox.height(value);
        },
        getText : function()
        {
            var value = this.getValue();
            return this.findTextByValue(value);
        },
        //查找Text,适用多选和单选
        findTextByValue: function (value)
        {
            var g = this, p = this.options;
            if (value == null || value == "") return "";
            var texts = [];
            var contain = function (checkvalue)
            {
                var targetdata = value.toString().split(p.split);
                for (var i = 0; i < targetdata.length; i++)
                {
                    if (targetdata[i] == checkvalue) return true;
                }
                return false;
            };
            $(g.data).each(function (i, item)
            {
                var val = item[p.valueField];
                var txt = item[p.textField];
                if (contain(val))
                {
                    texts.push(txt);
                }
            }); 
            return texts.join(p.split);
        },
        getDataByValue: function (value)
        {
            var g = this, p = this.options;
            for (var i = 0, l = g.data.length; i < l; i++)
            {
                if (g.data[i][p.valueField] == value) return g.data[i];
            }
            return null;
        },
        indexOf: function (item)
        {
            var g = this, p = this.options;
            if (!g.data) return -1;
            var isObj = typeof (item) == "object";
            for (var i = 0, l = g.data.length; i < l; i++)
            {
                if (isObj)
                {
                    if (g.data[i] == item) return i;
                } else
                {
                    if (g.data[i][p.valueField] && g.data[i][p.valueField].toString() == item.toString()) return i;
                }
            }
            return -1;
        },
        removeItems : function(items)
        {
            var g = this;
            if (!g.data) return;
            $(items).each(function (i,item)
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
        insertItem: function (item,index)
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
        getSelectedItems: function()
        {
            var g = this, p = this.options; 
            if (!g.data) return null;
            var value = g.getValue();
            if (!value) return null;
            var items = [];
            $(value.split(p.split)).each(function ()
            {
                var index = g.indexOf(this.toString());
                if (index != -1) items.push(g.data[index]);
            });
            return items;
        },
        _setValue: function (value)
        {
            var g = this, p = this.options; 
            p.value = value;
            this._dataInit();
        },
        setValue: function (value)
        { 
            this._setValue(value);
        },
        _setUrl: function (url)
        {
            if (!url) return;
            var g = this, p = this.options;
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
            var ajaxOp = {
                type: p.ajaxType || 'post',
                url: url,
                data: parms,
                cache: false,
                dataType: 'json',
                success: function (data)
                {
                    g.setData(data);
                    g.trigger('success', [data]);
                },
                error: function (XMLHttpRequest, textStatus)
                {
                    g.trigger('error', [XMLHttpRequest, textStatus]);
                }
            };

            if (p.ajaxContentType)
            {
                ajaxOp.contentType = p.ajaxContentType;
            }
            $.ajax(ajaxOp);
        },
        reload : function()
        {
            this.set('url', this.options.url);
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
            $("table", g.selectBox).html(""); 
        },
        _setColumns : function(columns)
        {
            var g = this, p = this.options;
            p.columns = columns;
            g.refresh();
        },
        _setData : function(data)
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
        refresh:function()
        {
            var g = this, p = this.options, data = this.data; 
            this.clearContent();
            if (!data) return;
            if (p.columns)
            {
                g.selectBox.table.headrow = $("<tr class='l-table-headerow'><td width='18px' class='l-checkboxrow'></td></tr>");
                g.selectBox.table.append(g.selectBox.table.headrow);
                g.selectBox.table.addClass("l-listbox-grid");
                for (var j = 0; j < p.columns.length; j++)
                {
                    var headrow = $("<td columnindex='" + j + "' columnname='" + p.columns[j].name + "'>" + p.columns[j].header + "</td>");
                    if (p.columns[j].width)
                    {
                        headrow.width(p.columns[j].width);
                    }
                    g.selectBox.table.headrow.append(headrow);

                }
            }
            var out = [];
            for (var i = 0; i < data.length; i++)
            {
                var val = data[i][p.valueField];
                var txt = data[i][p.textField];
                var valueIndexStr = " value='" + val + "' index='" + i + "'";
                if (!p.columns)
                {
                    out.push("<tr " + valueIndexStr + ">");
                    out.push("<td style='width:18px;' class='l-checkboxrow'><input type='checkbox'" + valueIndexStr + "/></td>");
                    var itemHtml = txt;
                    if (p.render)
                    {
                        itemHtml = p.render({
                            data: data[i],
                            value: val,
                            text: txt 
                        });
                    } 
                    out.push("<td align='left'>" + itemHtml + "</td></tr>");
                } else
                {
                    out.push("<tr " + valueIndexStr + "><td style='width:18px;' class='l-checkboxrow'><input type='checkbox' " + valueIndexStr + "/></td>");
                    for (var j = 0; j < p.columns.length; j++)
                    {
                        var columnname = p.columns[j].name;
                        out.push("<td>" + data[i][columnname] + "</td>");
                    }
                    out.push('</tr>');  
                }
            } 
            g.selectBox.table.append(out.join(''));
        },
        _getValue: function ()
        {
            return $(this.valueField).val();
        },
        getValue: function ()
        {
            //获取值
            return this._getValue();
        },  
        updateStyle: function ()
        { 
            this._dataInit();
        },
        selectAll: function ()
        {
            var g = this, p = this.options; 
            var values = [];
            $("tr", g.selectBox).each(function ()
            {
                var jrow = $(this);
                values.push(jrow.attr("value"));
            }); 
            $("tr", g.selectBox).addClass("l-selected").find(":checkbox").each(function () { this.checked = true; });
            g.valueField.val(values.join(p.split));
        },
        _dataInit: function ()
        {
            var g = this, p = this.options;
            var value = p.value;
            //根据值来初始化
            if (value != null)
            {
                g._changeValue(value);
            } 
            else if (g.valueField.val() != "")
            {
                p.value = g.valueField.val();  
            } 
            var valueArr = (value || "").toString().split(p.split);

            $("tr.l-selected", g.selectBox)
                   .removeClass("l-selected")
                   .find(":checkbox").each(function () { this.checked = false; });
            $(valueArr).each(function (i, item)
            {
                $("tr[value='" + item + "']", g.selectBox)
                    .addClass("l-selected")
                    .find(":checkbox").each(function () { this.checked = true; });
            }); 
        },
        //设置值到 隐藏域
        _changeValue: function (newValue)
        {
            var g = this, p = this.options; 
            g.valueField.val(newValue); 
            g.selectedValue = newValue;
        },
        //更新值到隐藏域
        _updateValue: function ()
        {
            var g = this, p = this.options;
            var values = [];
            $("tr", g.selectBox).each(function ()
            {
                var jrow = $(this);
                if (jrow.hasClass("l-selected"))
                {
                    values.push(jrow.attr("value"));
                }
            }); 
            g._changeValue(values.join(p.split));
        },
        _addClickEven: function ()
        {
            var g = this, p = this.options;
            //选项点击
            g.selectBox.click(function (e)
            {  
                var obj = (e.target || e.srcElement); 
                var jrow = $(obj).parents("tr:first");
                if (!jrow.length) return;
                var value = jrow.attr("value");
                var text = g.findTextByValue(value), data = g.getDataByValue(value);
                if (g.hasBind('select'))
                {
                    if (g.trigger('select', [value, text, data]) == false)
                    {
                        return false;
                    }
                }
                if (!p.isMultiSelect)
                {
                    $("tr.l-selected", g.selectBox).not(jrow)
                        .removeClass("l-selected")
                        .find(":checkbox").each(function () { this.checked = false });
                }
                if (jrow.hasClass("l-selected"))
                {
                    jrow.removeClass("l-selected");
                } else
                {
                    jrow.addClass("l-selected");
                }
                jrow.find(":checkbox").each(function () { this.checked = jrow.hasClass("l-selected"); });
                g._updateValue();
                g.trigger('selected', [value, text, data]);
            });
        } 
    });
      

})(jQuery);