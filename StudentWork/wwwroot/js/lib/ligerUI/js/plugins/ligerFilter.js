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
    $.fn.ligerFilter = function ()
    {
        return $.ligerui.run.call(this, "ligerFilter", arguments);
    };

    $.fn.ligerGetFilterManager = function ()
    {
        return $.ligerui.run.call(this, "ligerGetFilterManager", arguments);
    };

    $.ligerDefaults.Filter = {
        //字段列表
        fields: [],
        //字段类型 - 运算符 的对应关系
        operators: {},
        //自定义输入框(如下拉框、日期)
        editors: {},
        buttonCls : null
    };
    $.ligerDefaults.FilterString = {
        strings: {
            "and": "并且",
            "or": "或者",
            "equal": "相等",
            "notequal": "不相等",
            "startwith": "以..开始",
            "endwith": "以..结束",
            "like": "相似",
            "greater": "大于",
            "greaterorequal": "大于或等于",
            "less": "小于",
            "lessorequal": "小于或等于",
            "in": "包括在...",
            "notin": "不包括...",
            "addgroup": "增加分组",
            "addrule": "增加条件",
            "deletegroup": "删除分组"
        }
    };

    $.ligerDefaults.Filter.operators['string'] =
    $.ligerDefaults.Filter.operators['text'] =
    ["equal", "notequal", "startwith", "endwith", "like", "greater", "greaterorequal", "less", "lessorequal", "in", "notin"];

    $.ligerDefaults.Filter.operators['number'] =
    $.ligerDefaults.Filter.operators['int'] =
    $.ligerDefaults.Filter.operators['float'] =
    $.ligerDefaults.Filter.operators['date'] =
    ["equal", "notequal", "greater", "greaterorequal", "less", "lessorequal", "in", "notin"];

    $.ligerDefaults.Filter.editors['string'] =
    {
        create: function (container, field)
        {
            var input = $("<input type='text'/>");
            container.append(input);
            input.ligerTextBox(field.editor.options || {});
            return input;
        },
        setValue: function (input, value)
        {
            input.val(value);
        },
        getValue: function (input)
        {
            return input.liger('option', 'value');
        },
        destroy: function (input)
        {
            input.liger('destroy');
        }
    };

    $.ligerDefaults.Filter.editors['date'] =
    {
        create: function (container, field)
        {
            var input = $("<input type='text'/>");
            container.append(input);
            input.ligerDateEditor(field.editor.options || {});
            return input;
        },
        setValue: function (input, value)
        {
            input.liger('option', 'value', value);
        },
        getValue: function (input, field)
        {
            return input.liger('option', 'value');
        },
        destroy: function (input)
        {
            input.liger('destroy');
        }
    };

    $.ligerDefaults.Filter.editors['number'] =
    {
        create: function (container, field)
        {
            var input = $("<input type='text'/>");
            container.append(input);
            var options = {
                minValue: field.editor.minValue,
                maxValue: field.editor.maxValue
            };
            input.ligerSpinner($.extend(options, field.editor.options || {}));
            return input;
        },
        setValue: function (input, value)
        {
            input.val(value);
        },
        getValue: function (input, field)
        {
            var isInt = field.editor.type == "int";
            if (isInt)
                return parseInt(input.val(), 10);
            else
                return parseFloat(input.val());
        },
        destroy: function (input)
        {
            input.liger('destroy');
        }
    };

    $.ligerDefaults.Filter.editors['combobox'] =
    {
        create: function (container, field)
        {
            var input = $("<input type='text'/>");
            container.append(input);
            var options = {
                data: field.data,
                slide: false,
                valueField: field.editor.valueField || field.editor.valueColumnName,
                textField: field.editor.textField || field.editor.displayColumnName
            };
            $.extend(options, field.editor.options || {});
            input.ligerComboBox(options);
            return input;
        },
        setValue: function (input, value)
        {
            input.liger('option', 'value', value);
        },
        getValue: function (input)
        {
            return input.liger('option', 'value');
        },
        destroy: function (input)
        {
            input.liger('destroy');
        }
    };

    //过滤器组件
    $.ligerui.controls.Filter = function (element, options)
    {
        $.ligerui.controls.Filter.base.constructor.call(this, element, options);
    };

    $.ligerui.controls.Filter.ligerExtend($.ligerui.core.UIComponent, {
        __getType: function ()
        {
            return 'Filter'
        },
        __idPrev: function ()
        {
            return 'Filter';
        },
        _init: function ()
        {
            var g = this, p = this.options;
            $.ligerui.controls.Filter.base._init.call(this);
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
             
            g.set(p); 
            //事件：增加分组
                      $(g.element).bind("click", function (e)
            {
                e.preventDefault(); 
                var jthis = $((e.target || e.srcElement));
                var cn = jthis.get(0).className;
                if (cn.indexOf("addgroup") >= 0)
                {
                    var jtable = jthis.parent().parent().parent().parent();
                    g.addGroup(jtable);
                }
                else if (cn.indexOf("deletegroup") >= 0)
                {
                    var jtable = jthis.parent().parent().parent().parent();
                    g.deleteGroup(jtable);
                }
                else if (cn.indexOf("addrule") >= 0)
                {
                    var jtable = jthis.parent().parent().parent().parent();
                    g.addRule(jtable);
                }
                else if (cn.indexOf("deleterole") >= 0)
                { 
                    var rulerow = jthis.parent().parent();
                    g.deleteRule(rulerow);
                }
            });

        },

        //设置字段列表
        _setFields: function (fields)
        {
            var g = this, p = this.options;
            if (g.group) g.group.remove();
            g.group = $(g._bulidGroupTableHtml()).appendTo(g.element);
            p.buttonCls && g.group.find(".addgroup,.addrule,.deletegroup").addClass(p.buttonCls);
        },

        //输入框列表
        editors: {},

        //输入框计算器
        editorCounter: 0,

        //增加分组
        //parm [jgroup] jQuery对象(主分组的table dom元素)
        addGroup: function (jgroup)
        {
            var g = this, p = this.options;
            jgroup = $(jgroup || g.group);
            var lastrow = $(">tbody:first > tr:last", jgroup);
            var groupHtmlArr = [];
            groupHtmlArr.push('<tr class="l-filter-rowgroup"><td class="l-filter-cellgroup" colSpan="4">');
            var altering = !jgroup.hasClass("l-filter-group-alt");
            groupHtmlArr.push(g._bulidGroupTableHtml(altering, true));
            groupHtmlArr.push('</td></tr>');
            var row = $(groupHtmlArr.join(''));
            p.buttonCls && row.find(".addgroup,.addrule,.deletegroup").addClass(p.buttonCls);
            lastrow.before(row); 
            var jtable = row.find("table:first");
            if (p.addDefult)
            {
                g.addRule(jtable);
            }
            return jtable;
        },

        //删除分组 
        deleteGroup: function (group)
        {
            var g = this, p = this.options;
            $("td.l-filter-value", group).each(function ()
            {
                var rulerow = $(this).parent();
                $("select.fieldsel", rulerow).unbind();
                g.removeEditor(rulerow);
            });
            $(group).parent().parent().remove();
        },


        //删除编辑器
        removeEditor: function (rulerow)
        {
            var g = this, p = this.options;
            var type = $(rulerow).attr("editortype");
            var id = $(rulerow).attr("editorid");
            var editor = g.editors[id];
            if (editor && p.editors[type].destroy)
            {
                p.editors[type].destroy(editor);
            }
            $("td.l-filter-value:first", rulerow).html("");
        },

        //设置规则
        //parm [group] 分组数据
        //parm [jgruop] 分组table dom jQuery对象
        setData: function (group, jgroup)
        {
            var g = this, p = this.options;
            jgroup = jgroup || g.group;
            var lastrow = $(">tbody:first > tr:last", jgroup);
            if (jgroup)
            {
                jgroup.find(">tbody:first > tr").not(lastrow).remove();
            }
            $("select:first", lastrow).val(group.op);
            if (group.rules)
            {
                $(group.rules).each(function ()
                {
                    var rulerow = g.addRule(jgroup); 
                    var fieldName = this.field;

                    var field = (function ()
                    {
                        for (var i = 0; i < p.fields.length; i++)
                        {
                            if (p.fields[i].name == fieldName)
                            {
                                return p.fields[i];
                            } 
                        }
                        return null;
                    })();
                    $("select.opsel", rulerow).html(g._bulidOpSelectOptionsHtml(this.type || "string", field.operator ));
                    rulerow.attr("fieldtype", this.type || "string");
                    $("select.opsel", rulerow).val(this.op);
                    $("select.fieldsel", rulerow).val(this.field).trigger('change');
                    var editorid = rulerow.attr("editorid");
                    if (editorid && g.editors[editorid])
                    {
                        var field = g.getField(this.field);
                        if (field && field.editor)
                        {
                            var editParm = {
                                field: field,
                                filter: g
                            };
                            p.editors[field.editor.type].setValue.call(g, g.editors[editorid], this.value, editParm);
                        }
                    }
                    else
                    {
                        $(":text", rulerow).val(this.value);
                    }
                });
            }
            if (group.groups)
            {
                $(group.groups).each(function ()
                {
                    var subjgroup = g.addGroup(jgroup);
                    g.setData(this, subjgroup);
                });
            }
        },

        //增加一个条件
        //parm [jgroup] 分组的jQuery对象
        addRule: function (jgroup)
        {
            var g = this, p = this.options;
            jgroup = jgroup || g.group;
            var lastrow = $(">tbody:first > tr:last", jgroup);
            var rulerow = $(g._bulidRuleRowHtml());
            lastrow.before(rulerow);
            if (p.fields.length)
            {
                //如果第一个字段启用了自定义输入框
                g.appendEditor(rulerow, p.fields[0]);
            }

            //事件：字段列表改变时
            $("select.fieldsel", rulerow).bind('change', function ()
            {
                var jopsel = $(this).parent().next().find("select:first");
                var fieldName = $(this).val();
                if (!fieldName) return;
                var field = g.getField(fieldName);
                //字段类型处理
                var fieldType = field.type || "string";
                var oldFieldtype = rulerow.attr("fieldtype");
                if (fieldType != oldFieldtype)
                {
                    jopsel.html(g._bulidOpSelectOptionsHtml(fieldType,field.operator ));
                    rulerow.attr("fieldtype", fieldType);
                }
                //当前的编辑器
                var editorType = null;
                //上一次的编辑器
                var oldEditorType = rulerow.attr("editortype");
                if (g.enabledEditor(field)) editorType = field.editor.type;
                if (oldEditorType)
                {
                    //如果存在旧的输入框 
                    g.removeEditor(rulerow);
                }
                if (editorType)
                {
                    //如果当前选择的字段定义了输入框
                    g.appendEditor(rulerow, field);
                } else
                {
                    rulerow.removeAttr("editortype").removeAttr("editorid");
                    $("td.l-filter-value:first", rulerow).html('<input type="text" class="valtxt l-text" />');
                }
            });
            return rulerow;
        },

        //删除一个条件
        deleteRule: function (rulerow)
        {
            $("select.fieldsel", rulerow).unbind();
            this.removeEditor(rulerow);
            $(rulerow).remove();
        },

        //附加一个输入框
        appendEditor: function (rulerow, field)
        {
            var g = this, p = this.options;
            if (g.enabledEditor(field))
            {
                var container = $("td.l-filter-value:first", rulerow).html("");
                var editor = p.editors[field.editor.type];

                var editorTag = ++g.editorCounter;

                var editParm = {  
                    filter: g
                };
                editParm.field = $.extend(true, {}, field);
                editParm.field.name = field.name + "_" + editorTag;
                g.editors[editorTag] = editor.create.call(this, container, editParm);
                rulerow.attr("editortype", field.editor.type).attr("editorid", editorTag);
            }
        },

        //获取分组数据
        getData: function (group)
        {
            var g = this, p = this.options;
            group = group || g.group;

            var groupData = {};

            $("> tbody > tr", group).each(function (i, row)
            {
                var rowlast = $(row).hasClass("l-filter-rowlast");
                var rowgroup = $(row).hasClass("l-filter-rowgroup");
                if (rowgroup)
                {
                    var groupTable = $("> td:first > table:first", row);
                    if (groupTable.length)
                    {
                        if (!groupData.groups) groupData.groups = [];
                        groupData.groups.push(g.getData(groupTable));
                    }
                }
                else if (rowlast)
                {
                    groupData.op = $(".groupopsel:first", row).val();
                }
                else
                {
                    var fieldName = $("select.fieldsel:first", row).val();
                    var field = g.getField(fieldName);
                    var op = $(".opsel:first", row).val();
                    var value = g._getRuleValue(row, field);
                    var type = $(row).attr("fieldtype") || "string";
                    if (!groupData.rules) groupData.rules = []; 
                    if (value != null)
                    {
                        groupData.rules.push({
                            field: fieldName, op: op, value: value, type: type
                        });
                    }
                }
            });

            return groupData;
        },

        _getRuleValue: function (rulerow, field)
        {
            var g = this, p = this.options;
            var editorid = $(rulerow).attr("editorid");
            var editortype = $(rulerow).attr("editortype");
            var editor = g.editors[editorid];
            var editParm = {
                field: field,
                filter: g
            };
            if (editor)
            {
                return p.editors[editortype].getValue.call(g, editor, editParm);
            }
            return $(".valtxt:first", rulerow).val();
        },

        //判断某字段是否启用自定义的输入框  
        enabledEditor: function (field)
        {
            var g = this, p = this.options;
            if (!field.editor || !field.editor.type) return false;
            return (field.editor.type in p.editors);
        },

        //根据fieldName 获取 字段
        getField: function (fieldname)
        {
            var g = this, p = this.options;
            for (var i = 0, l = p.fields.length; i < l; i++)
            {
                var field = p.fields[i];
                if (field.name == fieldname) return field;
            }
            return null;
        },

        //获取一个分组的html
        _bulidGroupTableHtml: function (altering, allowDelete)
        {
            var g = this, p = this.options;
            var tableHtmlArr = [];
            tableHtmlArr.push('<table cellpadding="0" cellspacing="0" border="0" class="l-filter-group');
            if (altering)
                tableHtmlArr.push(' l-filter-group-alt');
            tableHtmlArr.push('"><tbody>');
            tableHtmlArr.push('<tr class="l-filter-rowlast"><td class="l-filter-rowlastcell" align="right" colSpan="4">');
            //and or
            tableHtmlArr.push('<select class="groupopsel">');
            tableHtmlArr.push('<option value="and">' + p.strings['and'] + '</option>');
            tableHtmlArr.push('<option value="or">' + p.strings['or'] + '</option>');
            tableHtmlArr.push('</select>');

            //add group
            tableHtmlArr.push('<input type="button" value="' + p.strings['addgroup'] + '" class="addgroup">');
            //add rule
            tableHtmlArr.push('<input type="button" value="' + p.strings['addrule'] + '" class="addrule">');
            if (allowDelete)
                tableHtmlArr.push('<input type="button" value="' + p.strings['deletegroup'] + '" class="deletegroup">');

            tableHtmlArr.push('</td></tr>');

            tableHtmlArr.push('</tbody></table>');
            return tableHtmlArr.join('');
        },

        //获取字段值规则的html
        _bulidRuleRowHtml: function (fields)
        {
            var g = this, p = this.options;
            fields = fields || p.fields;
            var rowHtmlArr = [];
            var fieldType = fields && fields.length && fields[0].type ? fields[0].type : "string";
            rowHtmlArr.push('<tr fieldtype="' + fieldType + '"><td class="l-filter-column">');
            rowHtmlArr.push('<select class="fieldsel">');
            for (var i = 0, l = fields.length; i < l; i++)
            {
                var field = fields[i];
                rowHtmlArr.push('<option value="' + field.name + '"');
                if (i == 0) rowHtmlArr.push(" selected ");
                rowHtmlArr.push('>');
                rowHtmlArr.push(field.display);
                rowHtmlArr.push('</option>');
            }
            rowHtmlArr.push("</select>");
            rowHtmlArr.push('</td>');

            rowHtmlArr.push('<td class="l-filter-op">');
            rowHtmlArr.push('<select class="opsel">'); 
            rowHtmlArr.push(g._bulidOpSelectOptionsHtml(fieldType, fields && fields.length ? fields[0].operator : null));
            rowHtmlArr.push('</select>');
            rowHtmlArr.push('</td>');
            rowHtmlArr.push('<td class="l-filter-value">');
            rowHtmlArr.push('<input type="text" class="valtxt" />');
            rowHtmlArr.push('</td>');
            rowHtmlArr.push('<td>');
            rowHtmlArr.push('<div class="l-icon-cross deleterole"></div>');
            rowHtmlArr.push('</td>');
            rowHtmlArr.push('</tr>');
            return rowHtmlArr.join('');
        },

        //获取一个运算符选择框的html
        _bulidOpSelectOptionsHtml: function (fieldType, operator)
        {
            var g = this, p = this.options;
            var ops = p.operators[fieldType];
            var opHtmlArr = [];
            if (operator && operator.length)
            {
                ops = operator.split(',');
            }
            if (!ops || !ops.length)
            {
                ops = ["equal", "notequal"];
            }
            for (var i = 0, l = ops.length; i < l; i++)
            {
                var op = ops[i];
                opHtmlArr[opHtmlArr.length] = '<option value="' + op + '">';
                opHtmlArr[opHtmlArr.length] = p.strings[op];
                opHtmlArr[opHtmlArr.length] = '</option>';
            }
            return opHtmlArr.join('');
        }


    });

    $.ligerFilter = function () { };
    $.ligerFilter.filterTranslator = {
        translateGroup: function (group)
        {
            var out = [];
            if (group == null) return " 1==1 ";
            var appended = false;
            out.push('(');
            if (group.rules != null)
            {
                for (var i in group.rules)
                {
                    if (i == "indexOf")
                        continue;
                    var rule = group.rules[i];
                    if (appended)
                        out.push(this.getOperatorQueryText(group.op));
                    out.push(this.translateRule(rule));
                    appended = true;
                }
            }
            if (group.groups != null)
            {
                for (var j in group.groups)
                {
                    var subgroup = group.groups[j];
                    if (appended)
                        out.push(this.getOperatorQueryText(group.op));
                    out.push(this.translateGroup(subgroup));
                    appended = true;
                }
            }
            out.push(')');
            if (appended == false) return " 1==1 ";
            return out.join('');
        },

        translateRule: function (rule)
        {
            var out = [];
            if (rule == null) return " 1==1 ";
            if (rule.op == "like" || rule.op == "startwith" || rule.op == "endwith")
            {
                out.push('/');
                if (rule.op == "startwith")
                    out.push('^');
                out.push(rule.value);
                if (rule.op == "endwith")
                    out.push('$');
                out.push('/i.test(');
                out.push('o["');
                out.push(rule.field);
                out.push('"]');
                out.push(')');
                return out.join('');
            }
            out.push('o["');
            out.push(rule.field);
            out.push('"]');
            out.push(this.getOperatorQueryText(rule.op));
            out.push('"');
            out.push(rule.value);
            out.push('"');
            return out.join('');
        },

        getOperatorQueryText: function (op)
        {
            switch (op)
            {
                case "equal":
                    return " == ";
                case "notequal":
                    return " != ";
                case "greater":
                    return " > ";
                case "greaterorequal":
                    return " >= ";
                case "less":
                    return " < ";
                case "lessorequal":
                    return " <= ";
                case "and":
                    return " && ";
                case "or":
                    return " || ";
                default:
                    return " == ";
            }
        }

    };
    $.ligerFilter.getFilterFunction = function (condition)
    {
        if ($.isArray(condition))
            condition = { op: "and", rules: condition };
        var fnbody = ' return  ' + $.ligerFilter.filterTranslator.translateGroup(condition);
        return new Function("o", fnbody);
    };


})(jQuery);