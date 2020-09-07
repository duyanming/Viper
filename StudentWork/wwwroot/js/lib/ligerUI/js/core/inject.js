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
    /*
    以html的方式加载组件
    程序会查询以 liger-插件名 类名的Dom,从dom加载相应的参数并调用插件
        比如遇到 .liger-grid 的dom，会找到 liger.defaults.Grid 加载需要的参数
        而在config.Grid中配置了这些参数的类型,会动态得加载data,而columns会设置为数组
    参数处理的优先级为：
    1,ignores 忽略不处理的参数
    2,dom存在 {属性名} 的类名 ,比如 <ul class="columns"></ul> ,便会将这个参数设置为复杂属性(object或array):找到相应的defaults和config来加载
         defaults是先找$.liger.inject.defaults,找不到再找liger.defaults的
         config为{父配置}.{属性名},比如 config.Grid.columns
    3,直接读取 data-{属性名} 或者 {属性名} 的dom属性
    */
    liger.inject = {

        prev: 'liger-',

        /* 
        命名规则：插件名_属性名(包括第N级的属性) (插件名首字母大写,属性名首字母小写) 
        获取规则：获取default时会先找这里,找不到再找liger.defaults,比如 liger.defaults.Grid_columns 
        备注：这里只定义了参数的列表
        */
        defaults: {
            Grid_detail: {
                height: null,
                onShowDetail: null
            },
            Grid_editor: 'ComboBox,DateEditor,Spinner,TextBox,PopupEdit,CheckBoxList,RadioList,Grid_editor',
            Grid_popup: 'PopupEdit',
            Grid_grid: 'Grid',
            Grid_condition: 'Form',
            Grid_toolbar: 'Toolbar',
            Grid_fields: 'Form_fields',
            Form_editor: 'ComboBox,DateEditor,Spinner,TextBox,PopupEdit,CheckBoxList,RadioList,Form_editor',
            Form_grid: 'Grid',
            Form_columns: 'Grid_columns',
            Form_condition: 'Form',
            Form_popup: 'PopupEdit',
            Form_buttons: 'Button',
            Portal_panel: 'Panel'
        },
        /*
        config里面配置了某插件参数或者复杂属性参数的类型(动态加载、数组、默认参数)
        */
        config: {
            Grid: {
                //动态
                dynamics: 'data,isChecked,detail,rowDraggingRender,toolbar,columns',
                //数组
                arrays: 'columns',
                //复杂属性columns
                columns: {
                    dynamics: 'render,totalSummary,headerRender,columns,editor,columns',
                    arrays: 'columns',
                    textProperty: 'display',
                    columns: 'liger.inject.config.Grid.columns',
                    editor: {
                        dynamics: 'data,columns,render,renderItem,grid,condition,ext',
                        grid: 'liger.inject.config.Grid',
                        condition: 'liger.inject.config.Form'
                    }
                },
                toolbar: {
                    arrays: 'items'
                }
            },
            Form: {
                dynamics: 'validate,fields,buttons',
                arrays: 'fields,buttons',
                fields: {
                    textProperty: 'label',
                    dynamics: 'validate,editor',
                    editor: {
                        dynamics: 'data,columns,render,renderItem,grid,condition,attr',
                        grid: 'liger.inject.config.Grid',
                        condition: 'liger.inject.config.Form'
                    }
                },
                buttons: 'liger.inject.config.Button'
            },
            PopupEdit: {
                dynamics: 'grid,condition'
            },
            Button: {
                textProperty: 'text',
                dynamics: 'click'
            },
            ComboBox: {
                dynamics: 'columns,data,tree,grid,condition,render,parms,renderItem'
            },
            ListBox: {
                dynamics: 'columns,data,render,parms'
            },
            RadioList: {
                dynamics: 'data,parms'
            },
            CheckBoxList: {
                dynamics: 'data,parms'
            },
            Panel: {
            },
            Portal: {
                //动态
                dynamics: 'rows,columns',
                //数组
                arrays: 'rows,columns',
                //复杂属性 columns
                columns: {
                    dynamics: 'panels',
                    arrays: 'panels'
                },
                //复杂属性 rows
                rows: {
                    dynamics: 'panels',
                    arrays: 'panels'
                },
                toolbar: {
                    arrays: 'items'
                }
            }
        },

        parse: function (code)
        {
            try
            {
                if (code == null) return null;
                return new Function("return " + code + ";")();
            } catch (e)
            {
                return null;
            }
        },

        parseDefault: function (value)
        {
            var g = this;
            if (!value) return value;
            var result = {};
            $(value.split(',')).each(function (index, name)
            {
                if (!name) return;
                name = name.substr(0, 1).toUpperCase() + name.substr(1);
                $.extend(result, g.parse("liger.defaults." + name));
            });
            return result;
        },

        fotmatValue: function (value, type)
        {
            if (type == "boolean")
                return value == "true" || value == "1";
            if (type == "number" && value)
                return parseFloat(value.toString());
            return value;
        },

        getOptions: function (e)
        {
            var jelement = e.jelement, defaults = e.defaults, config = e.config;
            config = $.extend({
                ignores: "",
                dynamics: "",
                arrays: ""
            }, config);
            var g = this, options = {}, value;
            if (config.textProperty) options[config.textProperty] = jelement.text();
            for (var proName in defaults)
            {
                var className = proName.toLowerCase();
                var subElement = $("> ." + className, jelement);
                //忽略
                if ($.inArray(proName, config.ignores.split(',')) != -1) continue;
                //判断子节点 (复杂属性) 
                if (subElement.length)
                {
                    var defaultName = e.controlName + "_" + proName;
                    var subDefaults = g.defaults[defaultName] || liger.defaults[defaultName], subConfig = config[proName];
                    if (typeof (subDefaults) == "string") subDefaults = g.parseDefault(subDefaults);
                    else if (typeof (subDefaults) == "funcion") subDefaults = subDefaults();
                    if (typeof (subConfig) == "string") subConfig = g.parse(subConfig);
                    else if (typeof (subConfig) == "funcion") subConfig = subConfig();
                    if (subDefaults)
                    {
                        if ($.inArray(proName, config.arrays.split(',')) != -1)
                        {
                            value = [];
                            $(">div,>li,>input", subElement).each(function ()
                            {
                                value.push(g.getOptions({
                                    defaults: subDefaults,
                                    controlName: e.controlName,
                                    config: subConfig,
                                    jelement: $(this)
                                }));
                            });
                            options[proName] = value;
                        } else
                        {
                            options[proName] = g.getOptions({
                                defaults: subDefaults,
                                controlName: e.controlName,
                                config: subConfig,
                                jelement: subElement
                            });
                        }
                    }
                    subElement.remove();
                }
                    //动态值
                else if ($.inArray(proName, config.dynamics.split(',')) != -1 || proName.indexOf('on') == 0)
                {
                    value = g.parse(jelement.attr("data-" + proName) || jelement.attr(proName));
                    if (value)
                    {
                        options[proName] = g.fotmatValue(value, typeof (defaults[proName]));
                    }
                }
                    //默认处理
                else
                {
                    value = jelement.attr("data-" + proName) || jelement.attr(proName);
                    if (value)
                    {
                        options[proName] = g.fotmatValue(value, typeof (defaults[proName]));
                    }
                }
            }
            var dataOptions = jelement.attr("data-options") || jelement.attr("data-property");
            if (dataOptions) dataOptions = g.parse("{" + dataOptions + "}");
            if (dataOptions) $.extend(options, dataOptions);
            return options;
        },

        init: function ()
        {
            var g = this, configs = this.config;
            for (var name in g.defaults)
            {
                if (typeof (g.defaults[name]) == "string")
                {
                    g.defaults[name] = g.parseDefault(g.defaults[name]);
                }
            }
            for (var controlName in liger.controls)
            {
                var config = configs[controlName] || {};
                var className = g.prev + controlName.toLowerCase();
                $("." + className).each(function ()
                {
                    var jelement = $(this), value;
                    var defaults = $.extend({
                        onrender: null,
                        onrendered: null
                    }, liger.defaults[controlName]);
                    var options = g.getOptions({
                        defaults: defaults,
                        controlName: controlName,
                        config: config,
                        jelement: jelement
                    });
                    jelement[liger.pluginPrev + controlName](options);
                });
            }
        }

    }

    $(function ()
    {
        liger.inject.init();
    });

})(jQuery);