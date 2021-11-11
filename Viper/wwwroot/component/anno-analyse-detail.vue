<template>
  <div>
    <el-row type="flex" class="bg-header">
      <el-col :span="16" :offset="4">
        <div>
          <ul class="summary-type">
            <li>
              <a
                @click="toggleTag(1)"
                v-bind:class="{ active: activeIndex === 1 }"
                >今天</a
              >
            </li>
            <li>
              <a
                @click="toggleTag(2)"
                v-bind:class="{ active: activeIndex === 2 }"
                >本周</a
              >
            </li>
            <li>
              <a
                @click="toggleTag(3)"
                v-bind:class="{ active: activeIndex === 3 }"
                >本月</a
              >
            </li>
            <li>
              <a
                @click="toggleTag(4)"
                v-bind:class="{ active: activeIndex === 4 }"
                >今年</a
              >
            </li>
          </ul>
          <div class="stats-bar" style="margin-right: 30px">
            服务访问详情分析
          </div>
        </div>
      </el-col>
    </el-row>
    <el-row type="flex">
      <el-col :span="11" :offset="1">
        <el-card class="box-card">
          <div class="echarts_service" id="ServiceModuleAnalyse"></div>
        </el-card>
      </el-col>
      <el-col :span="11" style="margin-left: 15px">
        <el-card class="box-card">
          <div class="echarts_service" id="ServiceRouterAnalyse"></div>
        </el-card>
      </el-col>
    </el-row>
    <el-row type="flex">
      <el-col :span="11" :offset="1">
        <el-card class="box-card">
          <div class="echarts_service" id="ServiceMethodAnalyse"></div>
        </el-card>
      </el-col>
      <el-col :span="11" style="margin-left: 15px">
        <el-card class="box-card">
          <div class="echarts_service" id="ServiceMethodErrorAnalyse"></div>
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
      },
    },
  },
  data: function () {
    return {
      activeIndex: 1,
    };
  },
  watch: {},
  created: function () {
    //用于数据初始化
  },
  mounted: function () {
    this.toggleTag(1);
  },
  methods: {
    search: function () {
      debugger;
    },
    toggleTag: function (tag) {
      this.activeIndex = tag;
      var startDate = this.fun_date(0);
      var endDate = this.fun_date(1);
      var now = new Date();
      if (tag === 2) {
        var day = now.getDay();
        var deltaDay;
        if (day == 0) {
          deltaDay = 6;
        } else {
          deltaDay = day - 1;
        }
        //周一
        var monday = new Date(now.getTime() - deltaDay * 24 * 60 * 60 * 1000);
        startDate = monday.toLocaleDateString();
        //下周一
        monday.setDate(monday.getDate() + 7);
        endDate = monday.toLocaleDateString();
      } else if (tag === 3) {
        startDate = now.getFullYear() + "-" + (now.getMonth() + 1) + "-01";
        var day01 = new Date(startDate);
        //下月1号
        day01.setMonth(day01.getMonth() + 1);
        endDate = day01.toLocaleDateString();
      } else if (tag === 4) {
        startDate = now.getFullYear() + "-01-01";
        endDate = now.getFullYear() + 1 + "-01-01";
      }
      this.serviceModuleAnalyse(startDate, endDate);
      this.serviceRouterAnalyse(startDate, endDate);
      this.serviceMethodAnalyse(startDate, endDate);
      this.serviceMethodErrorAnalyse(startDate, endDate);
    },
    serviceModuleAnalyse: function (startDate, endDate) {
      var that = this;
      var input = anno.getInput();
      input.startDate = startDate;
      input.endDate = endDate;
        anno.process(input, "Anno.Plugs.Analyse/Trace/ServiceModuleAnalyse",function (data) {
        that.init_echarts(data.outputData, "ServiceModuleAnalyse", "管道 Top 10");
      });
    },
    serviceRouterAnalyse: function (startDate, endDate) {
      var that = this;
      var input = anno.getInput();
      input.startDate = startDate;
      input.endDate = endDate;
        anno.process(input, "Anno.Plugs.Analyse/Trace/ServiceRouterAnalyse", function (data) {
        that.init_echarts(data.outputData, "ServiceRouterAnalyse", "路由 Top 10");
      });
    },
    serviceMethodAnalyse: function (startDate, endDate) {
      var that = this;
      var input = anno.getInput();
      input.startDate = startDate;
      input.endDate = endDate;
        anno.process(input,"Anno.Plugs.Analyse/Trace/ServiceMethodAnalyse", function (data) {
        that.init_echarts(data.outputData, "ServiceMethodAnalyse", "方法 Top 10");
      });
    },
    serviceMethodErrorAnalyse: function (startDate, endDate) {
      var that = this;
      var input = anno.getInput();
      input.startDate = startDate;
      input.endDate = endDate;
        anno.process(input,"Anno.Plugs.Analyse/Trace/ServiceMethodErrorAnalyse", function (data) {
        that.init_echarts(data.outputData, "ServiceMethodErrorAnalyse", "Error方法 Top 10");
      });
    },
    init_echarts: function (datas, id, title) {
      var chart_service = document.getElementById(id);
      var serviceChart = echarts.init(chart_service);
      var option = {
        tooltip: {
          trigger: "item",
        },
        title: {
          text: title,
        },
        series: [
          {
            name: title,
            type: "pie",
            radius: ["40%", "70%"],
            avoidLabelOverlap: true,
            itemStyle: {
              borderRadius: 10,
              borderColor: "#fff",
              borderWidth: 2,
            },
            emphasis: {
              label: {
                show: true,
                fontSize: "16",
                fontWeight: "bold",
              },
            },
            data: datas,
          },
        ],
      };
      serviceChart.setOption(option);
    },
    fun_date: function (aa) {
      var now = new Date();
      var date2 = new Date(now);
      date2.setDate(now.getDate() + aa);
      return (
        date2.getFullYear() +
        "-" +
        (date2.getMonth() + 1) +
        "-" +
        date2.getDate()
      );
    },
  },
};
</script>
<style scoped>
.bg-header {
  margin-bottom: 1em;
}
.echarts_service {
  height: 300px;
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
.el-row{
  margin-bottom: 10px;
}
body {
  height: 100%;
  -moz-osx-font-smoothing: grayscale;
  -webkit-font-smoothing: antialiased;
  text-rendering: optimizeLegibility;
  font-family: Helvetica Neue, Helvetica, PingFang SC, Hiragino Sans GB,
    Microsoft YaHei, Arial, sans-serif;
}
</style>