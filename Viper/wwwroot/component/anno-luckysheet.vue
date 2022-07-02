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
      editor: null,
    };
  },
  watch: {},
  created: function () {
    //用于数据初始化

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
        var options = {
          container: "luckysheet", //luckysheet为容器id
          title: "Anno 测试webExcel表格", // 设定表格名称
          lang: "zh", // 设定表格语言
        };
        that.editor = luckysheet.create(options);
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
      console.log(sheetsData);
      debugger;
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