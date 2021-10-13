<template>
  <div>
    <el-form ref="form" size="mini">
      <el-form-item>
        服务节点：
        <el-select
          v-model="formData.nodeName"
          :style="{ width: '250px' }"
          placeholder="请选择"
        >
          <el-option
            v-for="item in formData.nodeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          >
          </el-option>
        </el-select>
        <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery"
          >查询</el-button
        >
      </el-form-item>
    </el-form>
    <el-table
      :data="deployData"
      border
      stripe
      size="mini"
      max-height="429"
      style="width: 100%"
    >
      <el-table-column type="index" fixed :index="indexMethod">
      </el-table-column>
      <el-table-column
        prop="Id"
        show-overflow-tooltip
        label="进程PID"
        width="70"
      >
      </el-table-column>
      <el-table-column
        prop="Running"
        show-overflow-tooltip
        label="服务状态"
        width="100"
      >
        <template slot-scope="scope">
          <el-tag :type="scope.row.Running ? 'success' : 'danger'">{{
            scope.row.Running ? "Running" : "Stopped"
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column
        prop="WorkingDirectory"
        show-overflow-tooltip
        label="工作目录"
        width="120"
      >
      </el-table-column>
      <el-table-column
        prop="AutoStart"
        show-overflow-tooltip
        label="启动方式"
        width="80"
      >
        <template slot-scope="scope">
          {{ scope.row.AutoStart === "1" ? "自动启动" : "手工启动" }}
        </template>
      </el-table-column>
      <el-table-column 
      prop="Cmd" 
      show-overflow-tooltip 
      label="启动命令"
      min-width="320"
       >
      </el-table-column>
      <el-table-column
        prop="AnnoProcessDescription"
        show-overflow-tooltip
        label="描述"
        width="120"
      >
      </el-table-column>
      <el-table-column
        prop="NodeName"
        show-overflow-tooltip
        label="服务节点"
        width="120"
      >
      </el-table-column>
      <el-table-column fixed="right" align="center" width="150" label="操作">
        <template slot-scope="scope">
          <el-button
            v-if="!scope.row.Running"
            @click="Start(scope.row)"
            type="text"
            size="small"
            >启动</el-button
          >
          <el-button
            v-if="scope.row.Running"
            @click="Stop(scope.row)"
            type="text"
            size="small"
            >停止</el-button
          >
          <el-button @click="Restart(scope.row)" type="text" size="small"
            >重启</el-button
          >
          <el-button @click="Recycle(scope.row)" type="text" size="small"
            >回收</el-button
          >
          <el-button @click="GoToDeploy(scope.row)" type="text" size="small"
            >部署</el-button
          >
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
module.exports = {
  data: function () {
    return {
      deployData: [],
      formData: {
        title: "",
        nodeName: null,
        nodeOptions: [],
      },
    };
  },
  created: function () {
    var that = this;

    //用于数据初始化
    document.title = "部署服务列表";
    this.keyupAnno();
  },
  mounted: function () {
    this.getDeployNode();
  },
  methods: {
    GoToDeploy: function (row) {
      window.location.href =
        "./component.html?anno_component_name=anno-deploy-detail&WorkingDirectory=" +
        row.WorkingDirectory +
        "&NodeName=" +
        row.NodeName;
    },
    Start: function (row) {
      var that = this;
      this.$prompt("请输入部署口令", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputType: "password",
      }).then(function (rlt) {
        that.ExeCmd(row, "StartServiceByWorkingDirectory", rlt.value);
      });
    },
    Restart: function (row) {
      this.Start(row);
    },
    Stop: function (row) {
      var that = this;
      this.$prompt("请输入部署口令", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputType: "password",
      }).then(function (rlt) {
        that.ExeCmd(row, "StopServiceByWorkingDirectory", rlt.value);
      });
    },
    Recycle: function (row) {
      var that = this;
      this.$prompt("请输入部署口令", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputType: "password",
      }).then(function (rlt) {
        that.ExeCmd(row, "StopAndRemoveServiceByWorkingDirectory", rlt.value);
      });
    },
    ExeCmd: function (row, action, deploySecret) {
      var that = this;
      var input = new FormData();
      input.append("profile", localStorage.token);
      input.append("uname", localStorage.account);
      input.append("channel", "Anno.Plugs.Deploy");
      input.append("router", "DeployManager");
      input.append("method", action);

      input.append("workingName", row.WorkingDirectory);
      input.append("nodeName", row.NodeName);
      input.append("deploySecret", deploySecret);
      $.ajax({
        type: "post",
        dataType: "json",
        url:
          window.location.origin === undefined
            ? window.location.protocol +
              "//" +
              window.location.host +
              "/Deploy/Api"
            : window.location.origin + "/Deploy/Api", //兼容老版本IE origin
        data: input,
        contentType: false,
        processData: false,
        success: function (data) {
          if (data.status) {
            that.$message({
              showClose: true,
              message: "执行成功！",
              type: "success",
            });
            that.getDeployServiceByNode();
          } else {
            that.$message.error(data.msg);
          }
        },
      });
    },
    onQuery: function () {
      this.getDeployServiceByNode();
    },
    indexMethod: function (index) {
      return index + 1;
    },
    keyupAnno: function () {
      var that = this;
      document.onkeydown = function (e) {
        var _key = window.event.keyCode;
        if (_key === 13) {
          that.onQuery();
        }
      };
    },
    getDeployNode: function () {
      var that = this;
      var input = bif.getInput();
      input.channel = "Anno.Plugs.Deploy";
      input.router = "DeployManager";
      input.method = "GetDeployServices";
      bif.process(input, function (data) {
        if (data.status) {
          that.nodeOptions = [];
          for (var index = 0; index < data.outputData.length; index++) {
            var node = data.outputData[index];
            that.formData.nodeOptions.push({
              value: node.Nickname,
              label: node.Nickname,
            });
            if (index === 0) {
              that.formData.nodeName = node.Nickname;
              that.getDeployServiceByNode();
            }
          }
        } else {
          that.$message.error(data.msg);
        }
      });
    },
    getDeployServiceByNode: function () {
      var that = this;
      var input = new FormData();
      input.append("profile", localStorage.token);
      input.append("uname", localStorage.account);
      input.append("channel", "Anno.Plugs.Deploy");
      input.append("router", "DeployManager");
      input.append("method", "GetServices");
      input.append("nodeName", that.formData.nodeName);
      $.ajax({
        type: "post",
        dataType: "json",
        url:
          window.location.origin === undefined
            ? window.location.protocol +
              "//" +
              window.location.host +
              "/Deploy/Api"
            : window.location.origin + "/Deploy/Api", //兼容老版本IE origin
        data: input,
        contentType: false,
        processData: false,
        success: function (data) {
          if (data.status) {
            that.deployData = data.outputData;
          } else {
            that.$message.error(data.msg);
          }
        },
      });
    },
  },
};
</script>
<style scoped>
.el-button+.el-button {
    margin-left: 0px;
}
</style>