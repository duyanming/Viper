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

    $.fn.ligerPopupEdit = function (options)
    {
        return $.ligerui.run.call(this, "ligerPopupEdit", arguments);
    };

    $.fn.ligerGetPopupEditManager = function ()
    {
        return $.ligerui.run.call(this, "ligerGetPopupEditManager", arguments);
    };

    $.ligerDefaults.PopupEdit = {
        valueFieldID: null,     //生成的value input:hidden 字段名
        css: null,             //附加css
        onButtonClick: null,    //利用这个参数来调用其他函数，比如打开一个新窗口来选择值 
        nullText: null,         //不能为空时的提示
        disabled: false,        //是否无效
        method: 'post',                         //获取数据http方式
        async: true,
        cancelable: true,
        width: 200,
        heigth: null,
        render: null,        //显示函数   
        split: ';',
        data: [],
        grid: null,       //在可查询、可分页列表的弹出框 中选择值 
        condition: null,  // 条件字段,比如 {fields:[{ name : 'Title' ,op : 'like', vt : 'string',type:'text' }]}
        valueField: 'id', //值字段
        textField: 'text',   //显示字段
        parms: null,
        onSelect: null,    //选择事件,可阻止
        onSelected: null,  //选择后事件
        valueFieldCssClass: null,
        searchClick: null      //弹窗查询搜索按钮自定义函数
    };


    //扩展方法
    $.ligerMethos.PopupEdit = $.ligerMethos.PopupEdit || {};

    $.ligerui.controls.PopupEdit = function (element, options)
    {
        $.ligerui.controls.PopupEdit.base.constructor.call(this, element, options);
    };
    $.ligerui.controls.PopupEdit.ligerExtend($.ligerui.controls.Input, {
        __getType: function ()
        {
            return 'PopupEdit';
        },
        _extendMethods: function ()
        {
            return $.ligerMethos.PopupEdit;
        },
        _init: function ()
        {
            $.ligerui.controls.PopupEdit.base._init.call(this);
        },
        _render: function ()
        {
            var g = this, p = this.options;
            g.inputText = null;
            //文本框初始化
            if (this.element.tagName.toLowerCase() == "input")
            {
                this.element.readOnly = true;
                g.inputText = $(this.element);
                g.textFieldID = this.element.id;
            }
            if (g.inputText[0].name == undefined) g.inputText[0].name = g.textFieldID;
            //隐藏域初始化
            g.valueField = null;
            if (p.valueFieldID)
            {
                g.valueField = $("#" + p.valueFieldID + ":input");
                if (g.valueField.length == 0) g.valueField = $('<input type="hidden"/>');
                if (g.valueField[0].name == undefined) g.valueField[0].id = g.valueField[0].name = p.valueFieldID;
            }
            else
            {
                g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = g.textFieldID + "_val";
            }
            if (g.valueField[0].name == undefined) g.valueField[0].name = g.valueField[0].id;
            if (p.valueFieldCssClass)
            {
                g.valueField.addClass(p.valueFieldCssClass);
            }
            //开关
            g.link = $('<div class="l-trigger"><div class="l-trigger-icon"></div></div>');
            //外层
            g.wrapper = g.inputText.wrap('<div class="l-text l-text-popup"></div>').parent();
            g.wrapper.append('<div class="l-text-l"></div><div class="l-text-r"></div>');
            g.wrapper.append(g.link);
            g.wrapper.append(g.valueField);
            //修复popup控件没有data-ligerid的问题
            g.valueField.attr("data-ligerid", g.id);
            g.inputText.addClass("l-text-field");
            //开关 事件
            g.link.hover(function ()
            {
                if (p.disabled) return;
                this.className = "l-trigger-hover";
            }, function ()
            {
                if (p.disabled) return;
                this.className = "l-trigger";
            }).mousedown(function ()
            {
                if (p.disabled) return;
                this.className = "l-trigger-pressed";
            }).mouseup(function ()
            {
                if (p.disabled) return;
                this.className = "l-trigger-hover";
            }).click(function ()
            {
                if (p.disabled) return;
                if (g.trigger('buttonClick') == false) return false;
            });
            g.inputText.click(function ()
            {
                if (p.disabled) return;
            }).blur(function ()
            {
                if (p.disabled) return;
                g.wrapper.removeClass("l-text-focus");
            }).focus(function ()
            {
                if (p.disabled) return;
                g.wrapper.addClass("l-text-focus");
            });
            g.wrapper.hover(function ()
            {
                if (p.disabled) return;
                g.wrapper.addClass("l-text-over");
            }, function ()
            {
                if (p.disabled) return;
                g.wrapper.removeClass("l-text-over");
            });

            g.set(p);
            g.setTextByVal(g.getValue());
            //alert(g.getValue());
        },
        destroy: function ()
        {
            if (this.wrapper) this.wrapper.remove();
            this.options = null;
            $.ligerui.remove(this);
        },
        clear: function ()
        {
            var g = this, p = this.options;
            g.inputText.val("");
            g.valueField.val("");
        },
        _setCss: function (css)
        {
            if (css)
            {
                this.wrapper.addClass(css);
            }
        },
        //取消选择 
        _setCancelable: function (value)
        {
            var g = this, p = this.options;
            if (!value && g.unselect)
            {
                g.unselect.remove();
                g.unselect = null;
            }
            if (!value && !g.unselect) return;
            g.unselect = $('<div class="l-trigger l-trigger-cancel"><div class="l-trigger-icon"></div></div>').hide();
            g.wrapper.hover(function ()
            {
                g.unselect.show();
            }, function ()
            {
                g.unselect.hide();
            })
            if (!p.disabled && p.cancelable)
            {
                g.wrapper.append(g.unselect);
            }
            g.unselect.hover(function ()
            {
                this.className = "l-trigger-hover l-trigger-cancel";
            }, function ()
            {
                this.className = "l-trigger l-trigger-cancel";
            }).click(function ()
            {
                g.clear();
            });
        },
        _setDisabled: function (value)
        {
            if (value)
            {
                this.wrapper.addClass('l-text-disabled');
            } else
            {
                this.wrapper.removeClass('l-text-disabled');
            }
        },
        _setWidth: function (value)
        {
            var g = this;
            if (value > 20)
            {
                g.wrapper.css({ width: value });
                g.inputText.css({ width: value - 20 });
            }
        },
        _setHeight: function (value)
        {
            var g = this;
            if (value > 10)
            {
                g.wrapper.height(value);
                g.inputText.height(value - 2);
            }
        },
        getData: function ()
        {
            var g = this, p = this.options;
            var data = [];
            var v = $(g.valueField).val(), t = $(g.inputText).val();
            var values = v ? v.split(p.split) : null, texts = t ? t.split(p.split) : null;
            $(values).each(function (i)
            {
                var o = {};
                o[p.textField] = texts[i];
                o[p.valueField] = values[i];
                data.push(o);
            });
            return data;
        },
        _getText: function ()
        {
            return $(this.inputText).val();
        },
        _getValue: function ()
        { 
            return $(this.valueField).val();
        },
        getValue: function ()
        { 
            return this._getValue();
        },
        getText: function ()
        {
            return this._getText();
        },
        //设置值到  隐藏域
        setValue: function (value, text)
        {
            if (value == '') return;
            var g = this, p = this.options;
            if (arguments.length >= 2)
            {
                g.setValue(value);
                g.setText(text);
                return;
            }
            g.valueField.val(value);
            //g.setTextByVal(value);
        },
        //根据值设置文本  value：数值或文本
        setTextByVal: function (value)
        {
            value = (typeof (value) != "string") ? value.toString : value;
            if (value == '') return;

            var g = this, text = [], p = this.options, gridData = [];
            if (g.valueField.val() != value) g.valueField.val(value);

            var gridparms = p.grid;
            if ($.isFunction(gridparms)) gridparms = gridparms();
            var gridOptions = $.extend({
                parms: p.parms
            }, gridparms);

            if (p.data.length > 0)
            {
                gridData = p.data.Rows;
            } else if (gridOptions.url)
            {
                g.loadServerData(gridOptions.url, value);
                return;
            } else
            {
                gridData = gridOptions.data.Rows;
            }


            var values = value.split(p.split);

            $(gridData).each(function (i, rowdata)
            {
                if ($.inArray(rowdata[p.valueField], values) != -1)
                {
                    text.push(rowdata[p.textField]);
                }
            });
            text = text.join(p.split);
            g.setText(text);
        },
        loadServerData: function (param, value)
        {
            var g = this, p = this.options, gdata = [];
            if ($.isFunction(param)) param = param();
            var ajaxOptions = {
                type: p.method,
                url: param,
                async: p.async,
                //data: [],
                dataType: 'json',
                success: function (data)
                {
                    //g.trigger('success', [data, g]);
                    gridData = $.extend(true, {}, data);
                    p.data = gridData;
                    gridData = data.Rows;
                    var values = value.split(p.split);
                    var text = [];

                    $(gridData).each(function (i, rowdata)
                    {
                        if ($.inArray(rowdata[p.valueField], values) != -1)
                        {
                            text.push(rowdata[p.textField]);
                        }
                    });
                    text = text.join(p.split);
                    g.setText(text);
                    return;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    return;
                }

            }
            $.ajax(ajaxOptions);

        },
        //设置值到 文本框 
        setText: function (text)
        {
            var g = this, p = this.options;
            if (p.render)
            {
                g.inputText.val(p.render(text));
            }
            else
            {
                g.inputText.val(text);
            }
        },
        addValue: function (value, text)
        {
            var g = this, p = this.options;
            if (!value) return;
            var v = g.getValue(), t = g.getText();
            if (!v)
            {
                g.setValue(value);
                g.setText(text);
            } else
            {
                var arrV = [], arrT = [], old = v.split(p.split), value = value.split(p.split), text = text.split(p.split);
                for (var i = 0, l = value.length; i < l; i++)
                {
                    if ($.inArray(value[i], old) == -1)
                    {
                        arrV.push(value[i]);
                        arrT.push(text[i]);
                    }
                }
                if (arrV.length)
                {
                    g.setValue(v + p.split + arrV.join(p.split));
                    g.setText(t + p.split + arrT.join(p.split));
                }
            }
        },
        removeValue: function (value, text)
        {
            var g = this, p = this.options;
            if (!value) return;
            var v = g.getValue(), t = g.getText();
            if (!v) return;
            var oldV = v.split(p.split), oldT = t.split(p.split), value = value.split(p.split);
            for (var i = 0, index = -1, l = value.length; i < l; i++)
            {
                if ((index = $.inArray(value[i], oldV)) != -1)
                {
                    oldV.splice(index, 1);
                    oldT.splice(index, 1);
                }
            }
            g.setValue(oldV.join(p.split));
            g.setText(oldT.join(p.split));
        },
        _setGrid: function (value)
        {
            if (!value) return;
            var g = this, p = this.options;
            var gridOptions = $.extend({
                parms: p.parms
            }, p.grid);
            
            this.bind('buttonClick', function ()
            {
                function getLastSelected()
                {
                    try{
                        return g.getData();
                    } catch (e)
                    {
                        return null;
                    }
                }
                if (!g.popupFn)
                {
                    var options = {
                        grid: gridOptions,
                        condition: p.condition,
                        valueField: p.valueField,
                        textField: p.textField,
                        split: p.split,
                        searchClick : p.searchClick,
                        lastSelected: getLastSelected(),
                        onSelect: function (e)
                        {
                            if (g.trigger('select', e) == false) return;
                            if (p.grid.checkbox)
                            {
                                g.addValue(e.value, e.text);
                                g.removeValue(e.remvoeValue, e.remvoeText);
                            } else
                            {
                                g.setValue(e.value);
                                g.setText(e.text);
                            }
                            g.trigger('selected', e);
                        },
                        selectInit: function (rowdata)
                        {
                            var value = g.getValue();
                            if (!value) return false;
                            if (!p.valueField || !rowdata[p.valueField]) return false;
                            return $.inArray(rowdata[p.valueField].toString(), value.split(p.split)) != -1;
                        }
                    };
                    g.popupFn = $.ligerui.getPopupFn(options, g);
                }
                g.popupFn();
            });
        }
    });



    //创建一个可查询、可分页列表的选取弹出框 需要dialog,grid,form等插件的支持
    $.ligerui.getPopupFn = function (p,master)
    {
        p = $.extend({
            title: '选择数据',     //窗口标题
            width: 700,            //窗口宽度     
            height: 320,           //列表高度
            top: null,
            left: null,
            split: ';',
            valueField: null,    //接收表格的value字段名
            textField: null,     //接收表格的text字段名
            grid: null,          //表格的参数 同ligerGrid
            condition: null,     //搜索表单的参数 同ligerForm
            onSelect: function (p) { },   //选取函数 
            searchClick : p.searchClick,
            selectInit: function (rowdata) { return false }  //选择初始化
        }, p);
        if (!p.grid) return;
        var win, grid, condition, lastSelected = p.lastSelected || [];
        return function ()
        {
            show();
            return false;
        };
        function show()
        {
            function getGridHeight(height)
            {
                height = height || p.height;
                height -= conditionPanel.height();
                return height;
            }
            if (win)
            {
                grid._showData();
                win.show();
                grid.refreshSize();
                lastSelected = grid.selected.concat();
                return;
            }
            var panle = $("<div></div>");
            var conditionPanel = $("<div></div>");
            var gridPanel = $("<div></div>");
            panle.append(conditionPanel).append(gridPanel);
            
            if (p.condition)
            { 
                var conditionParm = $.extend({
                    labelWidth: 60,
                    space: 20
                }, p.condition);
                setTimeout(function ()
                {
                    condition = conditionPanel.ligerForm(conditionParm);
                }, 50);
            } else
            {
                conditionPanel.remove();
            }
            var gridParm = $.extend({
                columnWidth: 120,
                alternatingRow: false,
                frozen: true,
                rownumbers: true
            }, p.grid, {
                width: "100%",
                height: getGridHeight(),
                isChecked: p.selectInit,
                isSelected: p.selectInit,
                inWindow: false
            });
            //grid
            grid = gridPanel.ligerGrid(gridParm);
            //搜索按钮
            if (p.condition)
            {
               
                setTimeout(function ()
                {
                    var containerBtn1 = $('<li style="margin-right:9px"><div></div></li>');
                    $("ul:first", conditionPanel).append(containerBtn1).after('<div class="l-clear"></div>');
                    $("div", containerBtn1).ligerButton({
                        text: '搜索',
                        click: function ()
                        { 
                            var rules = condition.toConditions();
                            if (p.searchClick)
                            {
                                p.searchClick({
                                    grid: grid,
                                    rules: rules
                                });
                            } else
                            {
                                if (grid.get('url'))
                                {
                                    grid.setParm(grid.conditionParmName || 'condition', $.ligerui.toJSON(rules));
                                    grid.reload();
                                } else
                                {
                                    grid.loadData($.ligerFilter.getFilterFunction(rules));
                                }
                            }
                        }
                    });
                }, 100);
            }
            //dialog
            win = $.ligerDialog.open({
                title: p.title,
                width: p.width,
                height: 'auto',
                top: p.top,
                left: p.left,
                target: panle,
                isResize: true,
                cls: 'l-selectorwin',
                onContentHeightChange: function (height)
                {
                    grid.set('height', getGridHeight(height));
                    return false;
                },
                onStopResize: function ()
                {
                    grid.refreshSize();
                },
                buttons: [
                 { text: '选择', onclick: function (item, dialog) { toSelect(); dialog.hide(); } },
                 { text: '取消', onclick: function (item, dialog) { dialog.hide(); } }
                ]
            });

            if (master)
            {
                master.includeControls = master.includeControls || [];
                master.includeControls.push(win);
            }
            grid.refreshSize();
        }
        function exist(value, data)
        {
            for (var i = 0; data && data[i]; i++)
            {
                var item = data[i];
                if (item[p.valueField] == value) return true;
            }
            return false;
        }
        function toSelect()
        {
            var selected = grid.selected || [];
            var value = [], text = [], data = [];
            $(selected).each(function (i, rowdata)
            {
                p.valueField && value.push(rowdata[p.valueField]);
                p.textField && text.push(rowdata[p.textField]);
                var o = $.extend(true, {}, this);
                grid.formatRecord(o, true);
                data.push(o);
            });
            var unSelected = [];
            $(lastSelected).each(function (i, item)
            {
                if (!exist(item[p.valueField], selected) && exist(item[p.valueField], grid.rows))
                {
                    unSelected.push(item);
                }
            });
            var removeValue = [], removeText = [], removeData = [];
            $(unSelected).each(function (i, rowdata)
            {
                p.valueField && removeValue.push(rowdata[p.valueField]);
                p.textField && removeText.push(rowdata[p.textField]);
                var o = $.extend(true, {}, this);
                grid.formatRecord(o, true);
                removeData.push(o);
            });
            p.onSelect({
                value: value.join(p.split),
                text: text.join(p.split),
                data: data,
                remvoeValue: removeValue.join(p.split),
                remvoeText: removeText.join(p.split),
                removeData: removeData
            });
        }
    };

})(jQuery);