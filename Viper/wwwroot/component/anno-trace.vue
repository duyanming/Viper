<template>
    <div>
        <el-form ref="form"
                 size="mini"
                 :model="form">

            <el-form-item>
                搜索内容：
                <el-input style="width:40%;"
                          prefix-icon="el-icon-search"
                          placeholder="目标服务/调用人/路由/管道/方法/IP"
                          v-model="form.title"></el-input>
                <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
            </el-form-item>
        </el-form>
        <el-table :data="xData"
                  border
                  stripe
                  trigger="hover"
                  size="mini"
                  :max-height="window.innerHeight-98"
                  style="width: 100%;"
                  :cell-style="tableCellStyle">
            <el-table-column type="index"
                             fixed
                             :index="indexMethod">
            </el-table-column>
            <el-table-column fixed="left"
                             align="center"
                             width="80"
                             label="操作">
                <template slot-scope="scope">
                    <el-button @click="linkToTrack(scope.row)" type="text" size="small">详细</el-button>
                </template>
            </el-table-column>
            <el-table-column fixed prop="AppName"
                             show-overflow-tooltip
                             label="调用方名称"
                             width="120">
            </el-table-column>
            <el-table-column fixed prop="AppNameTarget"
                             show-overflow-tooltip
                             label="目标服务名称"
                             width="120">
            </el-table-column>
            <el-table-column fixed prop="UseTimeMs"
                             show-overflow-tooltip
                             label="耗时(毫秒)"
                             align="center"
                             width="80">                            
            </el-table-column>
                        <el-table-column prop="Response"
                             show-overflow-tooltip
                             label="错误信息"
                             width="100">
            </el-table-column>
            <el-table-column prop="Uname"
                             show-overflow-tooltip
                             label="调用人编号"
                             width="100">
            </el-table-column>
            <el-table-column prop="Target"
                             show-overflow-tooltip
                             width="135"
                             label="目标服务">
            </el-table-column>
            <el-table-column prop="Askchannel"
                             show-overflow-tooltip
                             label="请求管道"
                             width="140">
            </el-table-column>
            <el-table-column prop="Askrouter"
                             show-overflow-tooltip
                             label="请求路由"
                             width="120">
            </el-table-column>
            <el-table-column prop="Askmethod"
                             show-overflow-tooltip
                             label="请求方法"
                             width="140">
            </el-table-column>
            <el-table-column prop="TraceId"
                             show-overflow-tooltip
                             label="追踪标识"
                             width="180">
            </el-table-column>
            <el-table-column prop="Ip"
                             show-overflow-tooltip
                             label="访问者IP"
                             width="120">
            </el-table-column>
            <el-table-column prop="Timespan"
                             show-overflow-tooltip
                             label="调用时间"
                             width="140">
            </el-table-column>
        </el-table>
        <div class="block" style="text-align:center">
            <el-pagination @size-change="handleSizeChange"
                           @current-change="handleCurrentChange"
                           :current-page="currentPage"
                           :page-sizes="pagesizes"
                           :page-size="pagesize"
                           layout="total, sizes, prev, pager, next, jumper"
                           :total="total">
            </el-pagination>
        </div>
    </div>
</template>
<script>
    var at = null;
    module.exports = {
        props: {
            mData: {
                type: Array,
                default: function () {
                    return [];
                }
            }
        },
        data:function() {
            return {
                form: { title: "" },
                total: 0,
                xData: [],
                currentPage: 1,
                pagesize: 20,
                pagesizes: [10, 20, 30, 40]
            };
        }
        , created: function () {//用于数据初始化
            at = this;
            this.keyupAnno();
            this.loadData(1, 20);
        },
        methods: {
            tableCellStyle:function(scopdata){
                if(scopdata.columnIndex===4){
                    var value=scopdata.row.UseTimeMs;
                    var colorStr = "";
                    if(scopdata.row.Rlt==true)
                    {
                        if (value <= 50) {
                            colorStr = "#40de5a";
                        }
                        else if (value <= 100) {
                            colorStr = "#00e09e";
                        } else if (value <= 300) {
                            colorStr = "#2edfa3";
                        }
                        else if (value <= 500) {
                            colorStr = "#a4e2c6";
                        }
                        else if (value <= 800) {
                            colorStr = "#faff72";
                        } else {
                            colorStr = "#ffa631";
                        }
                    }else{
                        colorStr="red";
                    }
                 return 'background-color:'+colorStr+';'
                }
            },
            handleSizeChange: function (val) {
                this.pagesize = val;
                this.loadData(at.currentPage, val);
            },
            handleCurrentChange: function (val) {
                this.currentPage = val;
                this.loadData(val, at.pagesize);
            },
            linkToTrack: function (row) {
                window.location.href = '../trace/detail.html?GlobalTraceId=' + row.GlobalTraceId;
            }
            ,indexMethod: function (index) {
                return index + 1;
            }
            ,onQuery: function () {
                at.currentPage = 1;
                this.loadData(at.currentPage, at.pagesize);
            },
            keyupAnno: function () {
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {
                        this.onQuery();
                    }
                }
            },
            loadData: function (page, pagesize) {
                var that = this;
                var input = anno.getInput();
                if(that.form.title!==null&&that.form.title!==""){
                    input.where =JSON.stringify(this.builderWhere(null,that.form.title));
                }
                if (page !== null && page !== undefined) {
                    input.page = page;
                } else {
                    input.page = 1;
                }
                if (pagesize !== null && pagesize !== undefined) {
                    input.pagesize = pagesize;
                } else {

                    input.pagesize = 20;
                }
                anno.process(input, "Anno.Plugs.Trace/Trace/GetTrace", function (data) {
                    at.xData = data.Rows;
                    at.total = parseInt(data.Total);
                });
            },
            builderWhere:function(field,value){
                if(field===null){
                return {
                    rules:[
                            {"field":"AppNameTarget","op":"like","value":value,"type":"string"}
                        ]
                    ,groups:[
                        {
                             rules:[ {"field":"Uname","op":"like","value":value,"type":"string"}]
                             ,op:"or"
                        },
                         {
                             rules:[   {"field":"Askchannel","op":"like","value":value,"type":"string"}]
                             ,op:"or"
                        },
                         {
                             rules:[   {"field":"Askrouter","op":"like","value":value,"type":"string"}]
                             ,op:"or"
                        },
                         {
                             rules:[   {"field":"Askmethod","op":"like","value":value,"type":"string"}]
                             ,op:"or"
                        },
                         {
                             rules:[   {"field":"Ip","op":"like","value":value,"type":"string"}]
                             ,op:"or"
                        }
                    ]
                    ,op:"or"};
                }else{
                    return {rules:[{"field":field,"op":"like","value":value,"type":"string"}],op:"or"};
                }
                
            }
        }
    };
</script>
<style scoped>
    .el-button {
        font-size: 10px;
    }

    .warning-row {
        background: oldlace;
    }

    .success-row {
        background: #f0f9eb;
    }

    body {
        height: 100%;
        -moz-osx-font-smoothing: grayscale;
        -webkit-font-smoothing: antialiased;
        text-rendering: optimizeLegibility;
        font-family: Helvetica Neue,Helvetica,PingFang SC,Hiragino Sans GB,Microsoft YaHei,Arial,sans-serif;
    }
</style>