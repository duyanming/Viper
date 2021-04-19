<template>
  <div>
    <el-row type="flex">
      <el-col :span="14">
        <el-card class="box-card">
          <div slot="header" class="clearfix">全国用户分布</div>
          <div id="anno_user_map"></div>
        </el-card>
      </el-col>
      <el-col :span="10" style="margin-left: 15px">
        <el-card class="box-card">
          <div slot="header" class="clearfix">全国用户分布</div>
          <div></div>
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
    return {};
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
      var datas = [
        { name: "西藏", value: 60 },
        { name: "青海", value: 167 },
        { name: "宁夏", value: 210 },
        { name: "海南", value: 252 },
        { name: "甘肃", value: 502 },
        { name: "贵州", value: 570 },
        { name: "新疆", value: 661 },
        { name: "云南", value: 8890 },
        { name: "重庆", value: 10010 },
        { name: "吉林", value: 5056 },
        { name: "山西", value: 2123 },
        { name: "天津", value: 9130 },
        { name: "江西", value: 10170 },
        { name: "广西", value: 6172 },
        { name: "陕西", value: 9251 },
        { name: "黑龙江", value: 5125 },
        { name: "内蒙古", value: 1435 },
        { name: "安徽", value: 9530 },
        { name: "北京", value: 51919 },
        { name: "福建", value: 3756 },
        { name: "上海", value: 59190 },
        { name: "湖北", value: 37109 },
        { name: "湖南", value: 8966 },
        { name: "四川", value: 31020 },
        { name: "辽宁", value: 7222 },
        { name: "河北", value: 3451 },
        { name: "河南", value: 9693 },
        { name: "浙江", value: 62310 },
        { name: "山东", value: 39231 },
        { name: "江苏", value: 35911 },
        { name: "广东", value: 55891 },
      ];
      this.init_anno_user(datas);
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
          max: 6e4,
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