<template>
  <div id="luckysheet" style="
      margin: 0px;
      padding: 0px;
      position: absolute;
      width: 100%;
      height: 100%;
      left: 0px;
      top: 0px;
    "></div>
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
      value: new Date(),
      luckysheetName: "Anno"
    };
  },
  watch: {},
  created: function () {
    //用于数据初始化
    if (args.name != undefined && args.name != null && args.name != "") {
      this.name = args.name;
    } else {
      this.name = "Anno";
    }
    this.keyupAnno();
  },
  mounted: function () {
    var that = this;
    LoadStyleToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/plugins/css/pluginsCss.css"
    );
    LoadStyleToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/plugins/plugins.css"
    );
    LoadStyleToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/css/luckysheet.css"
    );
    LoadStyleToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/assets/iconfont/iconfont.css"
    );
    LoadScriptToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/plugins/js/plugin.js"
    );
    LoadScriptToHead(
      "//cdn.jsdelivr.net/npm/luckysheet@latest/dist/luckysheet.umd.js",
      function () {
        // var options = {
        //   container: "luckysheet", //luckysheet为容器id
        //   title: "Anno 测试webExcel表格", // 设定表格名称
        //   lang: "zh", // 设定表格语言
        // };
        // that.editor = luckysheet.create(options);
        that.getAllSheets();
        setInterval(function(){
          that.saveAllSheets(that);
        },1000*2);
      }
    );
  },
  methods: {
    keyupAnno: function () {
      var that = this;
      document.onkeydown = function (e) {
        var _key = window.event.keyCode;
        if (_key === 13) {
          that.saveAllSheets(that);
        }
      };
    },
    saveAllSheets: function (that) {
      var sheetsData = luckysheet.getAllSheets();
      var input = anno.getInput();
      input.name = that.name;
      input.data = JSON.stringify(sheetsData);
      anno.process(input, "Anno.Plugs.LuckySheet/LuckySheet/Save", function (data) {
        if (data.status) {
        } else {
          that.$message.error(data.msg);
        }
      });
    }
    , getAllSheets: function () {
      var that = this;
      var input = anno.getInput();
      if (that.name != undefined && that.name != null && that.name != "") {
        input.name = that.name;
      } else {
        input.name = "Anno";
      }
      anno.process(input, "Anno.Plugs.LuckySheet/LuckySheet/Get", function (data) {
        if (data.status) {
          var options = {
              container: "luckysheet", //luckysheet为容器id
              title: that.name, // 设定表格名称
              lang: "zh", // 设定表格语言
              data: eval(data.outputData)
            };
          luckysheet.create(options);
        } else {
          that.$message.error(data.msg);
        }
      });
    }
  }
};
</script>
<style scoped>
body {
  height: 100%;
  -moz-osx-font-smoothing: grayscale;
  -webkit-font-smoothing: antialiased;
  text-rendering: optimizeLegibility;
  font-family: Helvetica Neue, Helvetica, PingFang SC, Hiragino Sans GB,
    Microsoft YaHei, Arial, sans-serif;
}

.luckysheet-share-logo {
  height: 32px;
  width: 152px;
  z-index: 1;
  background-image: url("");
}
</style>