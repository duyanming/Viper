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

    $.fn.ligerPortal = function (options)
    {
        return $.ligerui.run.call(this, "ligerPortal", arguments);
    };

    $.ligerDefaults.Portal = {
        width: null,
        /*行元素：组件允许以纵向方式分割为几块
        每一块(行)允许自定义N个列(column)
        每一列允许自定义N个Panel(最小元素)
        rows:[
            {columns:[ 
                {
                   width : '50%',
                   panels : [{width:'100%',content:'内容'},{width:'100%',url:@url1}]
                },{
                   width : '50%',
                   panels : [{width:'100%',url:@url2}]
                }
            ]}
        ]
        */
        rows: null,
        /* 列元素： 组件将认为只存在一个row(块),
       这一块 允许自定义N个列(column),结构同上
        */
        columns:null,
        url: null,          //portal结构定义URL   
        method: 'get',                         //获取数据http方式
        parms: null,                         //提交到服务器的参数
        draggable: false,   //是否允许拖拽
        onLoaded:null       //url模式 加载完事件
    };
    $.ligerDefaults.Portal_rows = {
        width: null,
        height: null 
    };
    $.ligerDefaults.Portal_columns = {
        width: null,
        height: null 
    };

    $.ligerMethos.Portal = {};

 

    $.ligerui.controls.Portal = function (element, options)
    {
        $.ligerui.controls.Portal.base.constructor.call(this, element, options);
    };
    $.ligerui.controls.Portal.ligerExtend($.ligerui.core.UIComponent, {
        __getType: function ()
        {
            return 'Portal';
        },
        __idPrev: function ()
        {
            return 'Portal';
        },
        _extendMethods: function ()
        {
            return $.ligerMethos.Portal;
        },
        _init: function ()
        {
            var g = this, p = this.options;
            $.ligerui.controls.Portal.base._init.call(this); 
            if ($(">div", g.element).length) //如果已经定义了DIV子元素,那么这些元素将会转换为columns,这里暂时保存到tempInitPanels
            { 
                p.columns = [];
                $(">div", g.element).each(function (i, jpanel)
                {
                    p.columns[i] = {
                        panels :[]
                    };
                });

                g.tempInitPanels = $("<div></div>");
                $(">div", g.element).appendTo(g.tempInitPanels); 
            }
            if (!p.rows && p.columns)
            {
                p.rows = [{
                    columns: p.columns
                }];
            }
        },
        _render: function ()
        {
            var g = this, p = this.options;
            
            g.portal = $(g.element).addClass("l-portal").html(""); 
             
            g.set(p);
             
        }, 
        _setRows: function (rows)
        {
            var g = this, p = this.options;
            g.rows = [];
            if (rows && rows.length)
            {
                for (var i = 0; i < rows.length; i++)
                { 
                    var row = rows[i];
                    var jrow = $('<div class="l-row"></div>').appendTo(g.portal); 
                    g.rows[i] = g._renderRow({
                        row: row,
                        rowIndex: i,
                        jrow: jrow
                    });
                    jrow.append('<div class="l-clear"></div>');
                }
            }
        },
        _renderRow : function(e)
        {
            var row = e.row, rowIndex = e.rowIndex, jrow = e.jrow;
            var g = this, p = this.options;
            var rowObj = {
                element : jrow[0]
            };
            if (row.width)
            {
                if (typeof (row.width) == "string")
                {
                    if (row.width.indexOf('%') > -1 )
                    {
                        jrow.width(row.width);
                    }
                    else
                    {
                        jrow.width(parseInt(row.width));
                    }
                } else
                {
                    jrow.width(row.width);
                }
            }
            if (row.height) jrow.height(row.height);
            if (row.columns) rowObj.columns = [];
            if (row.columns && row.columns.length)
            {  
                for (var i = 0; i < row.columns.length; i++)
                {
                    var column = row.columns[i];
                    var jcolumn = $('<div class="l-column"></div>').appendTo(jrow);
                    rowObj.columns[i] = g._renderColumn({
                        column: column,
                        columnIndex: i,
                        jcolumn: jcolumn,
                        rowIndex : rowIndex
                    });  
                }
            }
            return rowObj;
        },
        remove: function (e)
        {
            var g = this, p = this.options;
            var rowIndex = e.rowIndex, columnIndex = e.columnIndex, index = e.index;
            if (index == null) index = -1; 
            if (index >= 0 && g.rows[rowIndex] && g.rows[rowIndex].columns && g.rows[rowIndex].columns[columnIndex] && g.rows[rowIndex].columns[columnIndex].panels)
            {
                var panel = g.rows[rowIndex].columns[columnIndex].panels[index]; 
                panel && panel.close();
                g._updatePortal();
            }  
        },
        add: function (e)
        {
            var g = this, p = this.options;
            var rowIndex = e.rowIndex, columnIndex = e.columnIndex, index = e.index, panel = e.panel;
            if (index == null) index = -1;
            if (!(g.rows[rowIndex] && g.rows[rowIndex].columns && g.rows[rowIndex].columns[columnIndex])) return;
            var gColumn = g.rows[rowIndex].columns[columnIndex], pColumn = p.rows[rowIndex].columns[columnIndex], ligerPanel, jcolumn = $(gColumn.element);
            pColumn.panels = pColumn.panels || [];
            gColumn.panels = gColumn.panels || [];
            pColumn.panels.splice(index, 0, panel); 
            if (index < 0)
            { 
                var jpanel = $('<div></div>').insertBefore(gColumn.jplace);
                ligerPanel = jpanel.ligerPanel(panel); 
            } else if(gColumn.panels[index])
            {
                var jpanel = $('<div></div>').insertBefore(gColumn.panels[index].panel);
                ligerPanel = jpanel.ligerPanel(panel);
            }
            if (ligerPanel)
            {
                ligerPanel.bind('closed', g._createPanelClosed());
                g.setPanelEvent({
                    panel: ligerPanel
                });
                gColumn.panels.splice(index, 0, ligerPanel);
            }
            g._updatePortal();
        },
        _createPanelClosed : function ()
        {
            var g = this, p = this.options;
            return function ()
            {
                var panel = this;//ligerPanel对象
                var panels = g.getPanels();
                var rowIndex, columnIndex, index;
                $(panels).each(function ()
                {
                    if (this.panel == panel)
                    {
                        rowIndex = this.rowIndex;
                        columnIndex = this.columnIndex;
                        index = this.index;
                    }
                });
                p.rows[rowIndex].columns[columnIndex].panels.splice(index, 1);
                g.rows[rowIndex].columns[columnIndex].panels.splice(index, 1);
            };
        },
        _renderColumn: function (e)
        {
            var column = e.column, columnIndex = e.columnIndex, jcolumn = e.jcolumn;
            var rowIndex = e.rowIndex;
            var g = this, p = this.options;
            var columnObj = {
                element : jcolumn[0]
            };
            if (column.width) jcolumn.width(column.width);
            if (column.height) jcolumn.height(column.height);
            if (column.panels) columnObj.panels = [];
            if (column.panels && column.panels.length)
            {
                for (var i = 0; i < column.panels.length; i++)
                {
                    var panel = column.panels[i]; 
                    var jpanel = $('<div></div>').appendTo(jcolumn);
                    columnObj.panels[i] = jpanel.ligerPanel(panel);
                    columnObj.panels[i].bind('closed', g._createPanelClosed());
                    g.setPanelEvent({ 
                        panel: columnObj.panels[i]
                    });
                }
            } else if(g.tempInitPanels)
            {
              
                var tempPanel = g.tempInitPanels.find(">div:eq(" + columnIndex + ")");
                if (tempPanel.length)
                {
                    columnObj.panels = [];
                    var panelOptions = {};
                    var jelement = tempPanel.clone();
                    if (liger.inject && liger.inject.getOptions)
                    {
                        panelOptions = liger.inject.getOptions({
                            jelement: jelement,
                            defaults: $.ligerDefaults.Panel,
                            config: liger.inject.config.Panel
                        });
                    }
                    columnObj.panels[0] = jelement.appendTo(jcolumn).ligerPanel(panelOptions);
                    columnObj.panels[0].bind('closed', g._createPanelClosed());
                    g.setPanelEvent({ 
                        panel: columnObj.panels[0]
                    });
                } 
            }
            columnObj.jplace = $('<div class="l-column-place"></div>').appendTo(jcolumn);
            return columnObj; 
        },
        setPanelEvent: function(e)
        {  
            //panel:ligerui对象,jpanel:jQuery dom对象
            var panel = e.panel, jpanel = panel.panel;
            var g = this, p = this.options;
            //拖拽支持
            if ($.fn.ligerDrag && p.draggable)
            { 
                jpanel.addClass("l-panel-draggable").ligerDrag({
                    proxy: false, revert: true,
                    handler: ".l-panel-header span:first",
                    onRendered: function ()
                    { 
                    },
                    onStartDrag: function (current, e)
                    {
                        g.portal.find(">.l-row").addClass("l-row-dragging");
                        this.jplace = $('<div class="l-panel-place"></div>');
                        this.jplace.height(jpanel.height());
                        jpanel.width(jpanel.width());
                        jpanel.addClass("l-panel-dragging"); 
                        jpanel.css("position", "absolute"); 
                        jpanel.after(this.jplace); 
                        g._updatePortal();
                    },
                    onDrag: function (current, e)
                    {
                        var pageX = e.pageX || e.screenX, pageY = e.pageY || e.screenY;
                        var height = jpanel.height(), width = jpanel.width(), offset = jpanel.offset();
                        var centerX = offset.left + width / 2, centerY = offset.top + 10; 
                        var panels = g.getPanels(), emptyColumns = g.getEmptyColumns();
                        var result = getPositionIn(panels, emptyColumns, centerX, centerY);
                        if (result)
                        { 
                            //判断是否跟上次匹配的位置一致
                            if (this.placeStatus)
                            {
                                if (this.placeStatus.panel && result.panel)
                                {
                                    if (this.placeStatus.panel.rowIndex == result.panel.rowIndex &&
                                this.placeStatus.panel.columnIndex == result.panel.columnIndex &&
                                this.placeStatus.panel.index == result.panel.index &&
                                this.placeStatus.position == result.position)
                                    {
                                        return;
                                    }
                                }
                                if (this.placeStatus.column && result.column) //定位到空元素行
                                {
                                    if (this.placeStatus.column.rowIndex == result.column.rowIndex && this.placeStatus.column.columnIndex == result.column.columnIndex && this.placeStatus.position == result.position)
                                    {
                                        return;
                                    }
                                }
                            }
                            if (result.position == "top")
                            { 
                                this.jplace.insertBefore(result.panel ? result.panel.jpanel : result.column.jplace);
                                this.savedPosition = result.panel ? result.panel : result.column
                                this.savedPosition.inTop = true;
                            } else if (result.position == "bottom")
                            {
                                this.jplace.insertAfter(result.panel.jpanel);
                                this.savedPosition = result.panel;
                                this.savedPosition.inTop = false;
                            }
                            this.placeStatus = result; 
                        } 
                        else//没有匹配到
                        {
                            this.placeStatus = null; 
                        }

                        //从指定的元素集合匹配位置
                        function getPositionIn(panels, columns, x, y)
                        {
                            for (i = 0, l = panels.length; i < l; i++)
                            {
                                var o = panels[i];
                                if (o.panel == panel) //如果是本身
                                {
                                    continue;
                                }
                                var r = positionIn(o, null, x, y);
                                if (r) return r;
                            }
                            for (i = 0, l = columns.length; i < l; i++)
                            {
                                var column = columns[i];
                                var r = positionIn(null, column, x, y);
                                if (r) return r;
                            }
                            return null;
                        }
                        //坐标在目标区域范围内 x,y为panel标题栏中间的位置
                        function positionIn(panel, column, x, y)
                        {
                            var jelement = panel ? panel.jpanel : column.jplace;
                            if (!jelement) return null;
                            var height = jelement.height(), width = jelement.width();
                            var left = jelement.offset().left, top = jelement.offset().top;
                            var diff = 3;
                            if (x > left - diff && x < left + width + diff)
                            {
                                if (y > top - diff && y < top + height / 2 + diff)
                                {
                                    return {
                                        panel: panel,
                                        column: column,
                                        position: "top"
                                    };
                                }
                                if (y > top + height / 2 - diff && y < top + height + diff)
                                {
                                    return {
                                        panel: panel,
                                        column: column,
                                        position: panel ? "bottom" : "top"
                                    };
                                }
                            }
                            return null;
                        }
                    },
                    onStopDrag: function (current, e)
                    {
                        g.portal.find(">.l-row").removeClass("l-row-dragging");
                        panel.set('width', panel.get('width')); 
                        jpanel.removeClass("l-panel-dragging");
                        //将jpanel替换到jplace的位置 
                        if (this.jplace)
                        {
                            jpanel.css({
                                "position": "relative",
                                "left": null,
                                "top": null
                            });
                            jpanel.insertAfter(this.jplace); 
                            g.portal.find(">.l-row > .l-column >.l-panel-place").remove();

                            if (this.savedPosition)
                            {
                                var panels = g.getPanels();
                                var rowIndex, columnIndex, index;
                                $(panels).each(function ()
                                {
                                    if (this.panel == panel)
                                    {
                                        rowIndex = this.rowIndex;
                                        columnIndex = this.columnIndex;
                                        index = this.index;
                                    }
                                });
                                var oldPanelOptions = p.rows[rowIndex].columns[columnIndex].panels[index];
                                var oldPanel = g.rows[rowIndex].columns[columnIndex].panels[index];
                                p.rows[rowIndex].columns[columnIndex].panels.splice(index, 1);
                                g.rows[rowIndex].columns[columnIndex].panels.splice(index, 1);

                                if (this.savedPosition.panel)
                                { 
                                  
                                    p.rows[this.savedPosition.rowIndex].columns[this.savedPosition.columnIndex].panels.splice(this.savedPosition.index + this.savedPosition.inTop ? -1 : 0, 0, oldPanelOptions); 
                                    g.rows[this.savedPosition.rowIndex].columns[this.savedPosition.columnIndex].panels.splice(this.savedPosition.index + this.savedPosition.inTop ? -1 : 0, 0, oldPanel);
                                } else
                                {
                                    p.rows[this.savedPosition.rowIndex].columns[this.savedPosition.columnIndex].panels = [oldPanelOptions];
                                    g.rows[this.savedPosition.rowIndex].columns[this.savedPosition.columnIndex].panels = [oldPanel];
                                } 
                            }
                        }
                        g._updatePortal();
                       
                        return false;
                    }
                });
            }
         
        },
        _updatePortal:function()
        {
            var g = this, p = this.options;
            $(g.rows).each(function (rowIndex)
            {
                $(this.columns).each(function (columnIndex)
                {
                    if (this.panels && this.panels.length)
                    {
                        $(this.element).removeClass("l-column-empty");
                    } else
                    {
                        $(this.element).addClass("l-column-empty");
                    }
                });
            });
        },
        getPanels : function ()
        {
            var g = this, p = this.options;
            var panels = []; 
            $(g.rows).each(function (rowIndex)
            { 
                $(this.columns).each(function (columnIndex)
                { 
                    $(this.panels).each(function (index)
                    {
                        panels.push({
                            rowIndex: rowIndex,
                            columnIndex: columnIndex,
                            index: index,
                            panel : this,
                            jpanel : this.panel
                        });
                    }); 
                }); 
            }); 
            return panels;
        },
        getPanel: function (e)
        {
            var g = this, p = this.options;
            e = $.extend({
                rowIndex: 0,
                columnIndex: 0,
                index : 0
            }, e);
            var panel = null;
            $(g.rows).each(function (rowIndex)
            {
                $(this.columns).each(function (columnIndex)
                {
                    $(this.panels).each(function (index)
                    {
                        if (panel) return;
                        if (rowIndex == e.rowIndex && columnIndex == e.columnIndex && index == e.index)
                        {
                            panel = this;
                        } 
                    });
                });
            });
            return panel;
        },
        getEmptyColumns:function(){
            var g = this, p = this.options;
            var columns = [];
            $(g.rows).each(function (rowIndex)
            { 
                $(this.columns).each(function (columnIndex)
                {
                    if (!this.panels || !this.panels.length)
                    {
                        columns.push({
                            rowIndex: rowIndex,
                            columnIndex: columnIndex, 
                            jplace : this.jplace
                        });
                    }
                }); 
            });
            return columns;
        },
        _setUrl: function (url)
        {
            var g = this, p = this.options;
            if (!url) return;
            $.ajax({
                url: url, data: p.parms, type: p.method, dataType: 'json',
                success: function (rows)
                {
                    g.set('rows', rows);
                }
            });
        },  
        _setWidth: function (value)
        { 
            value && this.portal.width(value);
        },
        collapseAll: function ()
        {
            var g = this, p = this.options;
            var panels = g.getPanels();
            $(panels).each(function (i,o)
            {
                var panel = o.panel; 
                panel.collapse();
            });
        },
        expandAll: function ()
        {
            var g = this, p = this.options;
            var panels = g.getPanels();
            $(panels).each(function (i, o)
            {
                var panel = o.panel;
                panel.expand();
            });
        }
    }); 


})(jQuery);