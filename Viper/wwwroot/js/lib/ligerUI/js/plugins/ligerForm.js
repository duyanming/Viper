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

    $.fn.ligerForm = function ()
    {
        return $.ligerui.run.call(this, "ligerForm", arguments);
    };

    $.ligerui.getConditions = function (form, options)
    {
        if (!form) return null;
        form = liger.get($(form));
        if (form && form.toConditions) return form.toConditions();
    };

    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Form = {
        width: null,    // 表单的宽度
        //控件宽度
        inputWidth: 180,
        //标签宽度
        labelWidth: 90,
        //间隔宽度
        space: 40,
        rightToken: '：',
        //标签对齐方式
        labelAlign: 'left',
        //控件对齐方式
        align: 'left',

        autoTypePrev : 'ui-',
        //字段
        /*
        数组的集合,支持的类型包括在$.ligerDefaults.Form.editors,这个editors同Grid的editors继承于base.js中提供的编辑器集合,具体可以看liger.editors
        字段的参数参考 127行左右的 $.ligerDefaults.Form_fields,
        ui内置的编辑表单元素都会调用ui的表单插件集合,所以这些字段都有属于自己的"liger对象",可以同liger.get("[ID]")的方式获取，这里的[ID]获取方式优先级如下：
        1,定义了field.id 则取field.id 
        2,如果是下拉框和PopupEdit，并且定义了comboboxName，则取comboboxName(如果表单定义了prefixID,需要加上)
        3,默认取field.name(如果表单定义了prefixID,需要加上) 
        */
        fields: [],
        //创建的表单元素是否附加ID
        appendID: true,
        //生成表单元素ID、Name的前缀
        prefixID: null,
        //json解析函数
        toJSON: $.ligerui.toJSON,
        labelCss: null,
        fieldCss: null,
        spaceCss: null,
        onAfterSetFields: null,
        // 参数同 ligerButton
        buttons: null,              //按钮组
        readonly: false,              //是否只读
        editors: {},              //编辑器集合,使用同$.ligerDefaults.Grid.editors
        //验证
        validate: null,
        //不设置validate属性到inuput
        unSetValidateAttr: false,
        tab: null,
        clsTab: 'ui-tabs-nav ui-helper-clearfix',
        clsTabItem: 'ui-state-default',
        clsTabItemSelected: 'ui-tabs-selected',
        clsTabContent: 'ui-tabs-panel ui-widget-content'
    };

    $.ligerDefaults.FormString = {
        invalidMessage: '存在{errorCount}个字段验证不通过，请检查!',
        detailMessage: '详细',
        okMessage: '确定'
    };

    $.ligerDefaults.Form.editors.textarea =
    {
        create: function (container, editParm, p)
        {
            var editor = $('<textarea class="l-textarea" />');
            var id = (p.prefixID || "") + editParm.field.name;
            if ($("#" + id).length)
            {
                editor = $("#" + id);
            }
            editor.attr({
                id: id,
                name: id
            }); 
            if (editParm.field.editor && editParm.field.editor.height)
            {
                editor.height(editParm.field.editor.height);
            }
            if (p.readonly) editor.attr("readonly", true);
            container.append(editor);
            return editor;
        },
        getValue: function (editor, editParm)
        {
            return editor.val();
        },
        setValue: function (editor, value, editParm)
        {
            editor.val(value);
        },
        resize: function (editor, width, height, editParm)
        {
            editor.css({
                width: width - 2
            }).parent().css("width", "auto");
        }
    };

    $.ligerDefaults.Form.editors.hidden =
    {
        create: function (container, editParm, p)
        {
            var editor = $('<input type = "hidden"  />');
            var id = (p.prefixID || "") + editParm.field.name;
            if ($("#" + id).length)
            {
                editor = $("#" + id);
            }
            //添加options.attr属性的设置
            editor.attr($.extend({
                id: id,
                name: id
            }, editParm.field.attr));
            var eo = editParm.field.editor || editParm.field.options; 
            eo && editor.val(eo.value);
            container.append(editor);
            return editor;
        },
        getValue: function (editor, editParm)
        {
            return editor.val();
        },
        setValue: function (editor, value, editParm)
        {
            editor.val(value);
        }
    };

    $.ligerDefaults.Form_fields = {
        name: null,             //字段name
        textField: null,       //文本框name
        type: null,             //表单类型
        editor: null,           //编辑器扩展
        label: null,            //Label
        labelInAfter: false,  //label显示在后面
        afterContent: null,  //后置内容
        beforeContent : null, //前置内容
        hideSpace: false,
        hideLabel: false,
        rightToken: null,        
        attrRender: null,
        style : null,
        containerCls : null,
        newline: true,          //换行显示
        op: null,               //操作符 附加到input
        vt: null,               //值类型 附加到input
        attr: null,             //属性列表 附加到input
        validate: null          //验证参数，比如required:true
    };

    $.ligerDefaults.Form_editor = {
    };

    //description 自动创建自定义风格的表单-编辑器构造函数
    //格式为：
    //{
    //    name: jinput.attr("name"),
    //    control: {
    //        getValue: function ()
    //        {
    //            return jinput.val();
    //        },
    //        setValue: function (value)
    //        {
    //            jinput.val(value);
    //        }
    //    }
    //}
    //editorBulider -> editorBuilder 命名错误 
    //param {jinput} 表单元素jQuery对象 比如input、select、textarea 
    $.ligerDefaults.Form.editorBulider = function (jinput)
    {
        //这里this就是form的ligerui对象
        var g = this, p = this.options;
        if (!jinput || !jinput.length) return;
        var options = {}, ltype = jinput.attr("ltype"), field = {};
        if (p.readonly) options.readonly = true;
        options = $.extend({
            width: (field.width || p.inputWidth) - 2
        }, field.options, field.editor, options);
        var control = null;

        if (ltype == "autocomplete")
            options.autocomplete = true;
        if (jinput.is("select") && ltype != "none")
        {
            control = jinput.ligerComboBox(options);
        }
        else if (jinput.is(":password") && ltype != "none")
        {
            control = jinput.ligerTextBox(options);
        } 
        else if (jinput.is(":radio") && ltype != "none")
        {
            control = jinput.ligerRadio(options);
        }
        else if (jinput.is(":checkbox") && ltype != "none")
        {
            control = jinput.ligerCheckBox(options);
        }
        else if (jinput.is("textarea") && ltype != "none")
        {
            jinput.addClass("l-textarea");

            control = {
                getValue: function ()
                {
                    return jinput.val();
                },
                setValue: function (value)
                {
                    jinput.val(value);
                }
            };
        }
        else if (ltype && ltype != "none")
        {
            if (jinput.is(":text"))
            {
                switch (ltype)
                {
                    case "select":
                    case "combobox":
                    case "autocomplete":
                        control = jinput.ligerComboBox(options);
                        break;
                    case "spinner":
                        control = jinput.ligerSpinner(options);
                        break;
                    case "date":
                        control = jinput.ligerDateEditor(options);
                        break;
                    case "popup":
                        control = jinput.ligerPopupEdit(options);
                        break;
                    case "currency":
                        control = options.currency = true;
                    case "float":
                    case "number":
                        options.number = true;
                        control = jinput.ligerTextBox(options);
                        break;
                    case "int":
                    case "digits":
                        options.digits = true;
                    default:
                        control = jinput.ligerTextBox(options);
                        break;
                }
            }
            else if (jinput.is(":hidden"))
            {
                switch (ltype)
                {
                    case "select":
                    case "combobox":
                    case "autocomplete":
                        control = jinput.ligerComboBox(options);
                        break;
                    case "checkboxlist":
                        control = jinput.ligerCheckBoxList(options);
                        break;
                    case "radiolist":
                        control = jinput.ligerRadioList(options);
                        break;
                    case "listbox":
                        control = jinput.ligerListBox(options);
                        break; 
                }
            }
        } 
        else //没有定义ltype的 text or hidden
        {
            var classname = jinput.get(0).className;
            if (classname) 
            {
                var autoTypePrev = p.autoTypePrev || "ui-";
                if (classname.indexOf(autoTypePrev) != -1)
                {
                    classname = classname.replace(autoTypePrev, "");
                }
            } 
            if (!classname && (jinput.is(":hidden") || jinput.is(":text")) && jinput.attr("name")) //没有匹配的classname
            {
                control = {
                    getValue: function ()
                    {
                        return jinput.val();
                    },
                    setValue: function (value)
                    {
                        jinput.val(value);
                    }
                };
            }
            else
            {
                switch (classname)
                {
                    case "textbox":
                    case "password":
                        control = jinput.ligerTextBox(options);
                        break;
                    case "datepicker":
                        control = jinput.ligerDateEditor(options);
                        break;
                    case "spinner":
                        control = jinput.ligerSpinner(options);
                        break;
                    case "checkbox":
                        control = jinput.ligerCheckBox(options);
                        break;
                    case "combobox":
                        control = jinput.ligerComboBox(options);
                        break;
                    case "checkboxlist":
                        control = jinput.ligerCheckBoxList(options);
                        break;
                    case "radiolist":
                        control = jinput.ligerRadioList(options);
                        break;
                    case "listbox":
                        control = jinput.ligerListBox(options);
                        break;
                }
            }
        } 
        if (!control) return null;
        return {
            name: jinput.attr("name"),
            control: control
        };
    }

    //表单组件
    $.ligerui.controls.Form = function (element, options)
    {
        $.ligerui.controls.Form.base.constructor.call(this, element, options);
    };

    $.ligerui.controls.Form.ligerExtend($.ligerui.core.UIComponent, {
        __getType: function ()
        {
            return 'Form'
        },
        __idPrev: function ()
        {
            return 'Form';
        },
        _init: function ()
        {
            var g = this, p = this.options;
            $.ligerui.controls.Form.base._init.call(this);
            //编辑构造器初始化
            for (var type in liger.editors)
            {
                var editor = liger.editors[type];
                //如果没有默认的或者已经定义
                if (!editor || type in p.editors) continue;
                p.editors[type] = liger.getEditor($.extend({
                    type: type,
                    master: g
                }, editor));
            }
        },
        _render: function ()
        {
            var g = this, p = this.options;
            var jform = $(this.element);
            g.form = jform.is("form") ? jform : jform.parents("form:first");
            //生成ligerui表单样式
            $("input,select,textarea", jform).each(function ()
            {
                var result = p.editorBulider.call(g, $(this));
                if (result)
                {
                    g.autoEditors = g.autoEditors || [];
                    g.autoEditors.push(result);
                }
            });
            if (g.autoEditors && g.autoEditors.length && p.validate)
            {
                g.validate = g.validate || {};
                $(g.autoEditors).each(function ()
                {
                    var name = this.name;
                    var control = this.control;
                    if (name && control.inputText && control.inputText.attr("validate"))
                    {
                        var v = null, vm = null;
                        eval("v = " + control.inputText.attr("validate"));
                        if (v)
                        {
                            g.validate.rules = g.validate.rules || {};
                            g.validate.rules[name] = v;
                        }
                        if (control.inputText.attr("validateMessage"))
                        {
                            eval("vm = " + control.inputText.attr("validateMessage"));
                            if (vm)
                            {
                                g.validate.messages = g.validate.messages || {};
                                g.validate.messages[name] = vm;
                            }
                        } 
                    }
                });
            }
            

            g.set(p);
            g.initValidate();
            if (p.buttons)
            {
                var jbuttons = $('<ul class="l-form-buttons"></ul>').appendTo(jform);
                $(p.buttons).each(function ()
                {
                    var jbutton = $('<li><div></div></li>').appendTo(jbuttons);
                    $("div:first", jbutton).ligerButton(this);
                });
            }

            if (!g.element.id) g.element.id = g.id;
            //分组 收缩/展开
            $("#" + g.element.id + " .togglebtn").live('click', function ()
            {
                if ($(this).hasClass("togglebtn-down")) $(this).removeClass("togglebtn-down");
                else $(this).addClass("togglebtn-down");
                var boxs = $(this).parent().nextAll("ul,div");
                for (var i = 0; i < boxs.length; i++)
                {
                    var jbox = $(boxs[i]);
                    if (jbox.hasClass("l-group")) break;
                    if ($(this).hasClass("togglebtn-down"))
                    {
                        jbox.hide();
                    } else
                    {
                        jbox.show();
                    }

                }
            });
        },
        _setWidth: function (value)
        {
            var g = this, p = this.options;
            if (value) g.form.width(value);
        },
        getEditor: function (name)
        {
            var g = this, p = this.options;
            if (!g.editors) return;
            var o = find(null);
            if (o) return o;
            if (p.tab && p.tab.items)
            {
                for (var i = 0; i < p.tab.items.length; i++)
                {
                    var item = p.tab.items[i];
                    var o = find(item, i);
                    if (o) return o;
                }
            }
            return null;
            function find(tabitem, tabindex)
            {
                var fields = tabitem == null ? p.fields : tabitem.fields;
                for (var i = 0, l = fields.length; i < l; i++)
                {
                    var field = fields[i];
                    var eIndex = tabindex == null ? i : "tab" + tabindex + "_" + i;
                    if (field.name == name && g.editors[eIndex])
                    {
                        return g.editors[eIndex].control;
                    }
                }
            }
        },
        getField: function (index)
        {
            var g = this, p = this.options;
            if (!p.fields) return null;
            return p.fields[index];
        },
        toConditions: function ()
        {
            var g = this, p = this.options;
            var conditions = [];
             
            $(p.fields).each(function (fieldIndex, field)
            {
                var name = field.name, textField = field.textField, editor = g.editors[fieldIndex];
                if (!editor || !name) return;
                var value = editor.editor.getValue.call(g, editor.control, {
                    field: field
                });
                if (value != null && value !== "")
                {
                    conditions.push({
                        op: field.operator || "like",
                        field: name,
                        value: value,
                        type: field.type || "string"
                    });
                }
            });
            return conditions;
        },
        //预处理字段 , 处理分组
        _preSetFields: function (fields)
        {
            var g = this, p = this.options, lastVisitedGroup = null, lastVisitedGroupIcon = null;
            //分组： 先填充没有设置分组的字段
            $(p.fields).each(function (i, field)
            {
                if (p.readonly || field.readonly || (field.editor && field.editor.readonly))
                {
                    //if (!field.editor) field.editor = {};
                    //field.editor.readonly = field.editor.readonly || true;
                    delete field.validate;
                }
                if (field.type == "hidden") return;
                field.type = field.type || "text";
                if (field.newline == null) field.newline = true;
                if (lastVisitedGroup && !field.group)
                {
                    field.group = lastVisitedGroup;
                    field.groupicon = lastVisitedGroupIcon;
                }
                if (field.group)
                {
                    field.group = field.group.toString().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
                    lastVisitedGroup = field.group;
                    lastVisitedGroupIcon = field.groupicon;
                }
            });

        },
        _setReadonly: function (readonly)
        {
            var g = this, p = this.options;
            if (readonly && g.editors)
            {
                for (var index in g.editors)
                {
                    var control = g.editors[index].control;
                    if (control && control._setReadonly) control._setReadonly(true);
                }
            }
        },
        _setFields: function (fields)
        {
            var g = this, p = this.options; 
            if ($.isFunction(p.prefixID)) p.prefixID = p.prefixID(g);
            var jform = $(g.element).addClass("l-form"); 
            g._initFieldsValidate({
                fields: fields
            });
            g._initFieldsHtml({
                panel: jform,
                fields: fields
            }); 
            g._createEditors({
                fields: fields
            }); 
            g.trigger('afterSetFields');
        },
        _initFieldsValidate: function (e)
        {
            var g = this, p = this.options;
            var fields = e.fields;
            g.validate = g.validate || {};


 
            if (fields && fields.length)
            {
                $(fields).each(function (index, field)
                {
                    var name = field.name,
                    readonly = (field.readonly || (field.editor && field.editor.readonly)) ? true : false,
                    txtInputName = (p.prefixID || "") + (field.textField || field.id || field.name);
                    if (field.validate && !readonly)
                    {
                        g.validate.rules = g.validate.rules || {};
                        g.validate.rules[txtInputName] = field.validate;
                        if (field.validateMessage)
                        {
                            g.validate.messages = g.validate.messages || {};
                            g.validate.messages[txtInputName] = field.validateMessage;
                        }
                    }
                });
            }
        },
        _initFieldsHtml: function (e)
        {
            var g = this, p = this.options;
            var jform = e.panel,
                fields = e.fields,
                idPrev = e.idPrev || g.id,
                tabindex = e.tabindex;
            $(">.l-form-container", jform).remove();
            var lineWidth = 0, maxWidth = 0;
            if (fields && fields.length)
            {
                g._preSetFields(fields);
                var out = ['<div class="l-form-container">'],
                    appendULStartTag = false,
                    lastVisitedGroup = null,
                    groups = []; 
                $(fields).each(function (index, field)
                {
                    if (!field.group) field.group = "";
                    if ($.inArray(field.group, groups) == -1)
                        groups.push(field.group);
                });
                $(groups).each(function (groupIndex, group)
                {
                    $(fields).each(function (i, field)
                    { 
                        if (field.group != group) return;
                        var index = $.inArray(field, fields);
                        var name = field.id || field.name, newline = field.newline;
                        var inputName = (p.prefixID || "") + (field.id || field.name);
                        if (!name) return;
                        if (field.type == "hidden")
                        {
                            if (!$("#" + inputName).length)
                                out.push('<div style="display:none" id="' + (idPrev + "|" + i) + '"></div>');
                            return;
                        }
                        var toAppendGroupRow = field.group && field.group != lastVisitedGroup;
                        if (index == 0 || toAppendGroupRow) newline = true;
                        if (newline)
                        {
                            lineWidth = 0;
                            if (appendULStartTag)
                            {
                                out.push('</ul>');
                                appendULStartTag = false;
                            }
                            if (toAppendGroupRow)
                            {
                                out.push('<div class="l-group');
                                if (field.groupicon)
                                    out.push(' l-group-hasicon');
                                out.push('">');
                                if (field.groupicon)
                                    out.push('<img src="' + field.groupicon + '" />');
                                out.push('<span>' + field.group + '</span></div>');
                                lastVisitedGroup = field.group;
                            }
                            out.push('<ul>');
                            appendULStartTag = true;
                        }
                        out.push('<li class="l-fieldcontainer');
                        if (newline)
                        {
                            out.push(' l-fieldcontainer-first');
                        }
                        if (field.containerCls)
                        {
                            out.push(' ' + field.containerCls);
                        }
                        out.push('"');
                        if (field.style)
                        {
                            out.push(' style="' + field.style + '"');
                        }
                        out.push(' fieldindex="' + index + '"');
                        if (tabindex != null)
                        {
                            out.push(' tabindex="' + tabindex + '"');
                        }
                        if (field.attrRender)
                        {
                            out.push(' ' + field.attrRender());
                        }
                        out.push('><ul>');
                        //append field 编辑后面自定义内容
                        if (field.beforeContent) //前置内容
                        {
                            var beforeContent = $.isFunction(field.beforeContent) ? field.afterContent(field) : field.beforeContent;
                            beforeContent && out.push(beforeContent);
                        }
                        if (!field.hideLabel && !field.labelInAfter)
                        {
                            out.push(g._buliderLabelContainer(field, index));
                        }
                        //append input 
                        out.push(g._buliderControlContainer(field, index, e.idPrev));
                        if (field.labelInAfter)
                        {
                            out.push(g._buliderLabelContainer(field, index));
                        }
                        //append field 编辑后面自定义内容
                        if (field.afterContent)
                        {
                            var afterContent = $.isFunction(field.afterContent) ? field.afterContent(field) : field.afterContent;
                            afterContent && out.push(afterContent);
                        }
                        //append space
                        if (!field.hideSpace)
                        {
                            out.push(g._buliderSpaceContainer(field, index));
                        }
                        out.push('</ul></li>');

                        lineWidth += (field.width || p.inputWidth || 0);
                        lineWidth += (field.space || p.space || 0);
                        lineWidth += (field.labelWidth || p.labelWidth || 0);
                        if (lineWidth > maxWidth) maxWidth = lineWidth;
                    });
                });
                if (appendULStartTag)
                {
                    out.push('</ul>');
                    appendULStartTag = false;
                }
                out.push('<div class="l-clear"></div>');
                out.push('</div>');
                jform.append(out.join(''));
                if (!p.width || maxWidth > p.width)
                {
                    //jform.width(maxWidth + 10);
                }
                $(".l-group .togglebtn", jform).remove();
                $(".l-group", jform).width(jform.width() * 0.95).append("<div class='togglebtn'></div>");
            }
        },
        _createEditors : function(e)
        { 
            var g = this, p = this.options;
            var fields = e.fields,
                idPrev = e.idPrev || g.id,
                editPrev = e.editPrev || "";
            g.editors = g.editors || {}; 
	    var jform = $(g.element);
            $(fields).each(function (fieldIndex, field)
            {
                var container = document.getElementById(idPrev + "|" + fieldIndex),
                    editor = p.editors[field.type],
                    editId = editPrev + fieldIndex; 
                if (!container) return; 
                container = $(container);
                var editorControl = g._createEditor(editor, container, {
                    field: field
                }, container.width(), container.height());
                if (!editorControl) return;
                if (g.editors[editId] && g.editors[editId].control && g.editors[editId].control.destroy)
                {
                    g.editors[editId].control.destroy();
                }
                g.editors[editId] = {
                    control: editorControl,
                    editor: editor
                };
            });
        },
        getChanges: function ()
        {
            //本函数返回当前数据与上一次数据之间的差异. 如果没有差异, 则返回空对象. 
            //注意!! getData会导致数据被刷新. 必须严格控制getData的调用. 
            //调用本函数不会导致刷新数据. 
            var g = this, p = this.options;
            var originData = g.data;
            var curData = g.getData();
            g.data = originData;

            var c = {};
            for (var k in originData)
            {
                if (curData[k] != originData[k])
                    c[k] = curData[k];
            }
            return c;
        },
        setEnabled: function (arg, isEnabled)
        {
            var fieldNames = [];
            if ($.isArray(arg)) fieldNames = arg;
            if (typeof (arg) == "string") fieldNames.push(arg);
            var g = this, p = this.options;
            setEnabledInFields(p.fields);
            if (p.tab && p.tab.items)
            {
                for (var i = 0; i < p.tab.items.length; i++)
                {
                    var item = p.tab.items[i];
                    setEnabledInFields(item.fields, i);
                }
            }
            function setEnabledInFields(fields, tabIndex)
            {
                $(fields).each(function (fieldIndex, field)
                {
                    var name = field.name,
                        textField = field.textField,
                        editPrev = tabIndex == null ? "" : "tab" + tabIndex + "_",
                        editor = g.editors[editPrev + fieldIndex];
                    if (!editor || !name) return;
                    if (!editor.editor || !editor.editor.setEnabled) return;
                    if ($.inArray(name, fieldNames) == -1) return;
                    editor.editor.setEnabled(editor.control, isEnabled);
                });
            }
        },
        setVisible: function (arg, isVisible)
        {
            var fieldNames = [];
            if ($.isArray(arg)) fieldNames = arg;
            if (typeof (arg) == "string") fieldNames.push(arg);
            var g = this, p = this.options; 
            setVisibleInFields(p.fields);
            if (p.tab && p.tab.items)
            {
                for (var i = 0; i < p.tab.items.length; i++)
                {
                    var item = p.tab.items[i];
                    setVisibleInFields(item.fields, i);
                }
            }
            function setVisibleInFields(fields, tabIndex)
            {
                $(fields).each(function (fieldIndex, field)
                {
                    var name = field.name;
                    if ($.inArray(name, fieldNames) == -1) return;
                    g._setFieldPanelVisible(tabIndex, fieldIndex, isVisible);
                });
            }
        },
        _setFieldPanelVisible: function (tabindex, fieldindex, visible)
        {
            var g = this, p = this.options;
            if (tabindex == null)
            {
                $("li.l-fieldcontainer[fieldindex=" + fieldindex + "]", g.element)[visible ? 'show' : 'hide']();
            }
            else
            {
                $("div."+p.clsTabContent+"[data-index=" + tabindex + "] li.l-fieldcontainer[fieldindex=" + fieldindex + "]", g.element)[visible ? 'show' : 'hide']();
            }
        },
        getData: function ()
        {
            var g = this, p = this.options;
            g.data = g.formData || {};

            if (g.autoEditors && g.autoEditors.length)
            {
                $(g.autoEditors).each(function ()
                {
                    var name = this.name;
                    var control = this.control; 
                    if (name && control && control.getValue)
                    {
                        g.data[name] = control.getValue();
                    }
                });
            }
            getFieldValueToData(p.fields);
            if (p.tab && p.tab.items)
            {
                for (var i = 0; i < p.tab.items.length; i++)
                {
                    var item = p.tab.items[i];
                    getFieldValueToData(item.fields, i);
                }
            }
            function getFieldValueToData(fields, tabIndex)
            {
                $(fields).each(function (fieldIndex, field)
                {
                    
                    var name = field.name,
                        textField = field.textField,
                        editPrev = tabIndex == null ? "" : "tab" + tabIndex + "_",
                        editor = g.editors[editPrev + fieldIndex];
                    if (!editor) return;
                    if (name)
                    {
                        var value = editor.editor.getValue.call(g, editor.control, {
                            field: field
                        });
                        g._setValueByName(g.data, name, value);
                    }
                    if (textField && editor.editor.getText)
                    {
                        var value = editor.editor.getText.call(g, editor.control, {
                            field: field
                        });
                        g._setValueByName(g.data, textField, value);
                    }
                });
            }
            return g.data;
        },
        _setData: function (data)
        {
            this.setData(data);
        },
        setData: function (data)
        {
            var g = this, p = this.options;
            g.data = data || {};

            if (g.autoEditors && g.autoEditors.length)
            {
                $(g.autoEditors).each(function ()
                {
                    var name = this.name;
                    var control = this.control;
                    if (name && g.data[name] && control && control.setValue)
                    {
                        control.setValue(g.data[name]);
                    }
                });
            }

            setDataToFields(p.fields);
            if (p.tab && p.tab.items)
            {
                for (var i = 0; i < p.tab.items.length; i++)
                {
                    var item = p.tab.items[i];
                    setDataToFields(item.fields, i);
                }
            }
            function setDataToFields(fields, tabIndex)
            {
                $(fields).each(function (fieldIndex, field)
                {
                    var name = field.name,
                        textField = field.textField,
                        editPrev = tabIndex == null ? "" : "tab" + tabIndex + "_",
                        editor = g.editors[editPrev + fieldIndex];
                    if (!editor) return;
                    if (name && (name in g.data))
                    {
                        var value = g._getValueByName(g.data, name);
                        editor.editor.setValue.call(g,editor.control, value, {
                            field: field
                        });
                    }
                    if (textField && (textField in g.data))
                    {
                        var text = g._getValueByName(g.data, textField);
                        editor.editor.setText.call(g, editor.control, text, {
                            field: field
                        });
                    }
                });
            }
        },
        _setValueByName: function (data, name, value)
        {
            if (!data || !name) return null;
            if (name.indexOf('.') == -1)
            {
                data[name] = value;
            }
            else
            {
                try
                {
                    new Function("data,value", "data." + name + "=value;")(data, value);
                }
                catch (e)
                {
                }
            }
        },
        _getValueByName: function (data, name)
        {
            if (!data || !name) return null;
            if (name.indexOf('.') == -1)
            {
                return data[name];
            }
            else
            {
                try
                {
                    return new Function("data", "return data." + name + ";")(data);
                }
                catch (e)
                {
                    return null;
                }
            }
        },
        //验证
        valid: function ()
        {
            var g = this, p = this.options;
            if (!g.form || !g.validator) return true;
            return g.form.valid();
        },
        showFieldError: function (name, errorText)
        {
            var element = $("[name=" + name + "]", this.element);
            if (element.hasClass("l-textarea"))
            {
                element.addClass("l-textarea-invalid");
            }
            else if (element.hasClass("l-text-field"))
            {
                element.parent().addClass("l-text-invalid");
            }
            $(element).removeAttr("title").ligerHideTip();
            $(element).attr("title", errorText).ligerTip({
                distanceX: 5,
                distanceY: -3,
                auto: true
            });
        },
        hideFieldError: function (name)
        {
            var element = $("[name=" + name + "]", this.element);
            if (element.hasClass("l-textarea"))
            {
                element.removeClass("l-textarea-invalid");
            }
            else
            {
                element.parent().removeClass("l-text-invalid");
            }
            $(element).removeAttr("title").ligerHideTip();
        },
        //设置验证
        initValidate: function ()
        {
            var g = this, p = this.options; 
            if (!g.form || !p.validate || !g.form.validate)
            {
                g.validator = null;
                return;
            }
            var validate = p.validate == true ? {} : p.validate;
            var validateOptions = $.extend({
                errorPlacement: function (lable, element)
                {
                    if (!$(lable).html())
                    {
                        return;
                    }
                    if (!element.attr("id"))
                    {
                        var eleid = new Date().getTime();
                        element.attr("id", eleid);
                        lable.attr("for", eleid);
                    }
                    if (element.hasClass("l-textarea"))
                    {
                        element.addClass("l-textarea-invalid");
                    }
                    else if (element.hasClass("l-text-field"))
                    {
                        element.parent().addClass("l-text-invalid");
                    }
                    $(element).removeAttr("title").ligerHideTip();
                    $(element).attr("title", $(lable).html()).ligerTip({
                        distanceX: 5,
                        distanceY: -3,
                        auto: true
                    });
                },
                success: function (lable)
                {
                    var eleId = lable.attr("for");
                    if (!eleId) return;
                    var element = $("#" + eleId);
                    if (element.hasClass("l-textarea"))
                    {
                        element.removeClass("l-textarea-invalid");
                    }
                    else
                    {
                        element.parent().removeClass("l-text-invalid");
                    }
                    $(element).removeAttr("title").ligerHideTip();
                }
            }, validate, {
                rules: g.validate.rules,
                messages: g.validate.messages
            });
             
            g.validator = g.form.validate(validateOptions);
        },
        setFieldValidate: function (name, validate, messages)
        {
            var jele = $("[name=" + name + "]");
            if (!jele.length || !jele.rules) return;
            var oldRule = jele.rules("remove");
            if (oldRule) //旧的验证移除验证结果
            {
                if (jele.hasClass("l-text-field"))
                {
                    jele.parent().removeClass("l-text-invalid");
                }
                jele.removeAttr("title").ligerHideTip();
                if (oldRule.required)//旧的验证包括必填规则，移除*
                {
                    jele.parents("li:first").next("li:first").find(".l-star").remove();
                }
            }
            if (!validate)//没有定义新的验证规则
            {
                return;
            }
            var rule = $.extend({}, validate, { messages: messages });
            jele.rules("add", rule);
            if (validate.required)
            {
                //验证包括必填规则，添加*
                jele.parents("li:first").next("li:first").append('<span class="l-star">*</span>');
            }
        },
        //提示 验证错误信息
        showInvalid: function ()
        {
            var g = this, p = this.options;
            if (!g.validator) return;
            var invalidMessage = p.invalidMessage.replace('{errorCount}', g.validator.errorList.length); 
            if (p.showInvalid)
            {
                p.showInvalid(invalidMessage);
            } else
            {
                var jmessage = $('<div><div class="invalid">' + invalidMessage + '<a class="viewInvalidDetail" href="javascript:void(0)">' + p.detailMessage + '</a></div><div class="invalidDetail" style="display:none;">' + getInvalidInf(g.validator.errorList) + '</div></div>');
                jmessage.find("a.viewInvalidDetail:first").bind('click', function ()
                {
                    $(this).parent().next("div.invalidDetail").toggle();
                });
                $.ligerDialog.open({
                    type: 'error',
                    width: 350,
                    showMax: false,
                    showToggle: false,
                    showMin: false,
                    target: jmessage,
                    buttons: [
                        {
                            text: p.okMessage, onclick: function (item, dailog)
                            {
                                dailog.close();
                            }
                        }
                    ]
                });
            }
        },
        _createEditor: function (editorBuilder, container, editParm, width, height)
        {
            var g = this, p = this.options;
            try
            {
                var editor = editorBuilder.create.call(this, container, editParm, p);
                if (editor && editorBuilder.resize)
                    editorBuilder.resize.call(this, editor, width, height, editParm); 
                return editor;
            } catch (e)
            {
                return null;
            }
        },
        //标签部分
        _buliderLabelContainer: function (field)
        {
            var g = this, p = this.options;
            var label = field.label || field.display;
            var labelWidth = field.labelWidth || field.labelwidth || p.labelWidth;
            var labelAlign = field.labelAlign || p.labelAlign;
            if (label)
            {
                var rightToken = field.rightToken;
                if (rightToken == null || rightToken == "") rightToken = p.rightToken;
                label += rightToken;
            }
            var out = [];
            out.push('<li');
            if (p.labelCss)
            {
                out.push(' class="' + p.labelCss + '"');
            }
            out.push(' style="');
            if (/px$/i.test(labelWidth) || /auto/i.test(labelWidth) || /%$/i.test(labelWidth))
            {
                out.push('width:' + labelWidth + ';');
            }
            else if (labelWidth)
            {
                out.push('width:' + labelWidth + 'px;');
            }
            if (labelAlign && labelAlign != "top")
            {
                out.push('text-align:' + labelAlign + ';');
            }

            out.push('">');
            if (label)
            {
                out.push(label);
            }
            out.push('</li>');
            return out.join('');
        },
        //控件部分
        _buliderControlContainer: function (field, fieldIndex, idPrev)
        {
            var g = this, p = this.options;
            var width = field.width || p.inputWidth,
                align = field.align || field.textAlign || field.textalign || p.align,
                out = [],
                idPrev = idPrev || g.id;
            var labelAlign = field.labelAlign || p.labelAlign;
            out.push('<li');
            out.push(' id="' + (idPrev + "|" + fieldIndex) + '"');
            if (p.fieldCss)
            {
                out.push(' class="' + p.fieldCss + '"');
            }
            out.push(' style="');
            if (/px$/i.test(width) || /auto/i.test(width) || /%$/i.test(width))
            {
                out.push('width:' + width + ';');
            }
            else if (width)
            {
                out.push('width:' + width + 'px;');
            }
            if (align)
            {
                out.push('text-align:' + align + ';');
            }
            if (labelAlign == "top")
            {
                out.push('clear:both;');
            }
            out.push('">');
            out.push('</li>');
            return out.join('');
        },
        //间隔部分
        _buliderSpaceContainer: function (field)
        {
            var g = this, p = this.options;
            var spaceWidth = field.space || field.spaceWidth || p.space;
            if (field.space === 0 || field.spaceWidth === 0) spaceWidth = 0;
            var out = [];
            out.push('<li');
            if (p.spaceCss)
            {
                out.push(' class="' + p.spaceCss + '"');
            }
            out.push(' style="');
            if (/px$/i.test(spaceWidth) || /auto/i.test(spaceWidth) || /%$/i.test(spaceWidth))
            {
                out.push('width:' + spaceWidth + ';');
            }
            if (spaceWidth)
            {
                out.push('width:' + spaceWidth + 'px;');
            }
            out.push('">');
            if (field.validate && field.validate.required)
            {
                out.push("<span class='l-star'>*</span>");
            }
            out.push('</li>');
            return out.join('');
        },
        _getInputAttrHtml: function (field)
        {
            var out = [], type = (field.type || "text").toLowerCase();
            if (type == "textarea")
            {
                field.cols && out.push('cols="' + field.cols + '" ');
                field.rows && out.push('rows="' + field.rows + '" ');
            }
            out.push('ltype="' + type + '" ');
            field.op && out.push('op="' + field.op + '" ');
            field.vt && out.push('vt="' + field.vt + '" ');
            if (field.attr)
            {
                for (var attrp in field.attr)
                {
                    out.push(attrp + '="' + field.attr[attrp] + '" ');
                }
            }
            return out.join('');
        },
        _setTab: function (tab)
        {
            var g = this, p = this.options;
            if (!tab || !tab.items) return;
            var jtab = $('<div class="l-form-tabs"></div>').appendTo(g.element);
            var jtabNav = $('<ul class="' + p.clsTab + '" original-title="">').appendTo(jtab);
            for (var i = 0; i < tab.items.length; i++)
            {
                var tabItem = tab.items[i],
                    jnavItem = $('<li class="'+p.clsTabItem+'"><a href="javascript:void(0)"></a></li>').appendTo(jtabNav),
                    jcontentItem = $('<div class="'+p.clsTabContent+'">').appendTo(jtab),
                    idPrev = g.id + "|tdb" + i;
                jnavItem.add(jcontentItem).attr("data-index", i);
                if (tabItem.id)
                {
                    jnavItem.attr("data-id", tabItem.id);
                }
                jnavItem.find("a:first").text(tabItem.title);
                g._initFieldsValidate({
                    fields: tabItem.fields
                });
                g._initFieldsHtml({
                    panel: jcontentItem,
                    fields: tabItem.fields,
                    tabindex : i,
                    idPrev: idPrev
                }); 
                g._createEditors({
                    fields: tabItem.fields,
                    idPrev: idPrev,
                    editPrev: 'tab' + i + "_"
                }); 
            }
            jtabNav.find("li").click(function ()
            {
                $(this).addClass(p.clsTabItemSelected);
            }, function ()
            {
                $(this).removeClass(p.clsTabItemSelected);
            }).click(function ()
            {
                var index = $(this).attr("data-index");
                g.selectTab(index);
            });
            g.selectTab(0);
        },
        selectTab: function (index)
        {
            var g = this, p = this.options;
            var jtab = $(g.element).find(".l-form-tabs:first");
            var links = jtab.find(".ui-tabs-nav li"), contents = jtab.find(".ui-tabs-panel");
           
            links.filter("[data-index=" + index + "]")
                .addClass(p.clsTabItemSelected);
            links.filter("[data-index!=" + index + "]")
                .removeClass(p.clsTabItemSelected);
            contents.filter("[data-index=" + index + "]").show();
            contents.filter("[data-index!=" + index + "]").hide();
        },
        destroy: function ()
        {
            try
            {
                var g = this, p = this.options;
                liger.remove(this);
                for (var index in g.editors)
                {
                    var control = g.editors[index].control;
                    if (control && control.destroy) control.destroy();
                }
                $(g.element).remove();
            }
            catch (e)
            {

            }
        }
    });


    function getInvalidInf(errorList)
    {
        var out = [];
        $(errorList).each(function (i, error)
        {
            var label = $(error.element).parents("li:first").prev("li:first").html();
            var message = error.message;
            out.push('<div>' + label + ' ' + message + '</div>');
        });
        return out.join('');
    }

})(jQuery);