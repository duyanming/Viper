<template>
    <div>
        <el-row class="bg-header">
            <el-col :span="16" :offset="4">
                <div class="map-toolbar" style="border: none; box-shadow: none;">
                    <ul class="summary-type">
                        <li><a class="active">今天</a></li>
                        <li><a class="">本周</a></li>
                        <li><a class="">本月</a></li>
                        <li><a class="">今年</a></li>
                    </ul>
                    <div class="stats-bar" style="margin-right: 30px;">服务访问分析</div>
                </div>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="11" :offset="1">
                <el-card class="box-card">
                    <div class="echarts_service" id="all_service"></div>
                </el-card>
            </el-col>
            <el-col :span="11" style=" margin-left:15px;">
                <el-card class="box-card">
                    <div class="echarts_service" id="error_service"></div>
                </el-card>

            </el-col>
        </el-row>
    </div>
</template>

<script>
    module.exports = {
        props: {
            value: {
                type: String,
                default: function () {
                    return "";
                }
            }
        },
        data: function () {
            return { activeIndex: 1 };
        },
        watch: {
        },
        created: function () {//用于数据初始化
            this.keyupAnno();
        },
        mounted: function () {
            this.all_service();

            this.error_service();
        },
        methods: {
            handleSelect(key, keyPath) {
                console.log(key, keyPath);
            },
            keyupAnno: function () {
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {

                    }
                }
            },
            all_service: function () {
                var that = this;
                var input = bif.getInput();
                input.channel = "Anno.Plugs.Analyse";
                input.router = "Trace";
                input.method = "ServiceAnalyse";
                input.startDate = this.fun_date(-5);
                input.endDate = this.fun_date(1);
                bif.process(input, function (data) {
                    var datas = [];
                    if (data != null && data.outputData != null) {
                        for (var i = 0; i < data.outputData.length; i++) {
                            datas.push({ value: parseInt(data.outputData[i].Count), name: data.outputData[i].Name });
                        }
                    }
                    that.init_all_echarts(datas);
                });
            },
            init_all_echarts: function (datas) {
                var chart_all_service = document.getElementById('all_service');
                var all_serviceChart = echarts.init(chart_all_service);
                var option = {
                    tooltip: {
                        trigger: 'item'
                    },
                    series: [
                        {
                            name: '访问统计',
                            type: 'pie',
                            radius: ['40%', '70%'],
                            avoidLabelOverlap: true,
                            itemStyle: {
                                borderRadius: 10,
                                borderColor: '#fff',
                                borderWidth: 2
                            },
                            emphasis: {
                                label: {
                                    show: true,
                                    fontSize: '16',
                                    fontWeight: 'bold'
                                }
                            },
                            data: datas
                        }
                    ]
                };
                all_serviceChart.setOption(option);
            },
            error_service: function () {
                var that = this;
                var input = bif.getInput();
                input.channel = "Anno.Plugs.Analyse";
                input.router = "Trace";
                input.method = "ServiceAnalyse";
                input.startDate = this.fun_date(-5);
                input.endDate = this.fun_date(1);
                bif.process(input, function (data) {
                    var datas = [];
                    if (data != null && data.outputData != null) {
                        for (var i = 0; i < data.outputData.length; i++) {
                            datas.push({ value: parseInt(data.outputData[i].Count), name: data.outputData[i].Name });
                        }
                    }
                    that.init_error_echarts(datas);
                });
            },
            init_error_echarts: function (datas) {
                var chart_error_service = document.getElementById('error_service');
                var error_serviceChart = echarts.init(chart_error_service);
                var option = {
                    tooltip: {
                        trigger: 'item'
                    },
                    series: [
                        {
                            name: '访问来源',
                            type: 'pie',
                            radius: ['40%', '70%'],
                            avoidLabelOverlap: true,
                            itemStyle: {
                                borderRadius: 10,
                                borderColor: '#fff',
                                borderWidth: 2
                            },
                            emphasis: {
                                label: {
                                    show: true,
                                    fontSize: '16',
                                    fontWeight: 'bold'
                                }
                            },
                            data:datas
                        }
                    ]
                };
                error_serviceChart.setOption(option);
            },
            fun_date: function (aa) {
                var now = new Date();
                var date2 = new Date(now);
                date2.setDate(now.getDate() + aa);
                return date2.getFullYear() + "-" + (date2.getMonth() + 1) + "-" + date2.getDate();
            }
        }
    };
</script>
<style scoped>
    .bg-header {
        height: 40px;
    }

    .echarts_service {
        height: 300px;
    }

    .map-toolbar {
        position: absolute;
        top: 10px;
        left: 10px;
        right: 10px;
        background-color: rgb(255 255 255 / 0.80);
        z-index: 100;
        box-shadow: 1px 1px 5px rgb(0 0 0 / 0.30);
        border-radius: 4px;
        padding-top: 10px;
        height: 40px;
    }

    .summary-type {
        list-style: none;
        margin: 0px 10px;
    }

    .stats-bar {
        text-align: right;
        font-size: 10pt;
        display: inline-block;
        float: right;
        padding-bottom: 10px;
    }

    .summary-type li {
        float: left;
    }

    .summary-type a {
        color: #929292;
        cursor: pointer;
        font-size: 10pt;
        display: block;
        text-align: center;
        padding: 5px 15px;
        margin: 0px 4px;
    }

        .summary-type a:hover {
            background-color: #929292;
            color: #fff;
            border-radius: 4px;
            transition: background-color 1s;
        }

    .summary-type .active {
        background-color: #929292;
        color: #fff;
        border-radius: 4px;
        transition: background-color 1s;
    }

    body {
        height: 100%;
        -moz-osx-font-smoothing: grayscale;
        -webkit-font-smoothing: antialiased;
        text-rendering: optimizeLegibility;
        font-family: Helvetica Neue,Helvetica,PingFang SC,Hiragino Sans GB,Microsoft YaHei,Arial,sans-serif;
    }
</style>