<template>
  <div v-loading.fullscreen.lock="fullscreenLoading">
    <el-row type="flex">
      <el-col :span="14">
        <el-card class="box-card">
          <div slot="header" class="clearfix">最近6个月用户分布</div>
          <div id="anno_user_map"></div>
        </el-card>
      </el-col>
      <el-col :span="9" style="margin-left: 15px">
        <el-card class="box-card">
          <div slot="header" class="clearfix">排名前7用户分布</div>
          <div>
            <el-table
             :data="userData"
              v-loading="fullscreenLoading"
              stripe
              style="width: 100%">
              <el-table-column type="index" label="排名" width="50"> </el-table-column>
              <el-table-column prop="name" label="地区">
              </el-table-column>
              <el-table-column prop="value" label="数量" width="100"> </el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
<script>
var user_map_chart = null;
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
      fullscreenLoading:true,
      userData: []
    };
  },
  watch: {},
  created: function () {
    //用于数据初始化
  },
  mounted: function () {
    var that = this;
    LoadScriptToHead("../js/lib/china.js", function () {
      that.anno_user();
    });
  },
  methods: {
    anno_user: function () {
      var that = this;
      var input = anno.getInput();
          anno.process(input,"Anno.Plugs.Analyse/Trace/UserAnalyse", function (data) {
        that.fullscreenLoading=false;
        for(var i=0;i<7;i++){
          that.userData.push(data.outputData[i]);
        }
        that.init_anno_user(data.outputData);
      });
    },
    init_anno_user: function (datas) {
      var anno_user_map = document.getElementById("anno_user_map");
      user_map_chart = echarts.init(anno_user_map);
      var option = {
        tooltip: {
          trigger: "item",
        },
        dataRange: {
          min: 0,
          max: 2000,
          text: ["高", "低"],
          color: ["#2395FF", "#f2f2f2"],
          itemHeight: 60,
          itemWidth: 12,
        },
        series: [
          {
            name: "用户数量",
            type: "map",
            mapType: "china",
            itemStyle: {
              normal: {
                label: { show: true, color: "#262626" },
                borderColor: "#dddddd",
              },
            },
            emphasis: {
              label: { show: true, color: "#fff" },
              itemStyle: { areaColor: "#FACF20" },
            },
            top: "0px",
            left: "15px",
            bottom: "0px",
            data: datas,
          },
        ],
      };
      user_map_chart.setOption(option);
    },
  },
};
/** 窗口大小改变事件 */
window.onresize = function () {
  user_map_chart.resize();
};
</script>
<style scoped>
.el-card__header {
  padding: 8px 14px;
}
#anno_user_map {
  height: 400px;
}
</style>