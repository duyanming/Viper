<template>
  <div>
    <el-form
      ref="elForm"
      :model="formData"
      :rules="rules"
      size="mini"
      label-width="100px"
      label-position="right"
    >
      <el-card>
        <div slot="header" class="clearfix">
          <span>基础信息</span>
        </div>
        <el-row>
          <el-col :span="span">
            <el-form-item label="工作目录：" prop="workingDirectory">
              <el-input
                v-model="formData.workingDirectory"
                placeholder="请输入工作目录："
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="span">
            <el-form-item label="部署节点：" prop="nodeName">
              <el-select
                v-model="formData.nodeName"
                :style="{ width: '100%' }"
                placeholder="请选择"
              >
                <el-option
                  v-for="item in nodeOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="span">
            <el-form-item label="启动方式：" prop="autoStart">
              <el-select
                v-model="formData.autoStart"
                :style="{ width: '100%' }"
                placeholder="请选择"
              >
                <el-option
                  v-for="item in autoStartOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col span="16">
            <el-form-item label="启动命令：" prop="cmd">
              <el-input
                placeholder="请输入启动命令"
                v-model="formData.cmd"
                clearable
              >
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col span="8">
            <el-form-item label="部署口令：" prop="deploySecret">
              <el-input
                show-password
                placeholder="请输入部署口令"
                v-model="formData.deploySecret"
                clearable
              >
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>
      <el-form-item size="mini" style="text-align: center">
        <el-button type="primary" @click="submitForm">提交</el-button>
        <el-button @click="resetForm">重置</el-button>
      </el-form-item>
      <el-card>
        <div slot="header" class="clearfix">
          <span>部署文件列表</span>
        </div>
        <el-row>
          <el-col span="16">
            <el-form-item
              label-width="150px"
              label="本地部署文件夹："
              prop="deployFile"
            >
              <el-upload
                name="deployFile"
                :auto-upload="false"
                :show-file-list="false"
                class="upload-demo"
                multiple
              >
                <i class="el-icon-upload"></i>
                <div slot="tip" class="el-upload__tip">
                  请选择要部署的文件夹,此功能支持Chrome、Edge
                </div>
              </el-upload>
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>
    </el-form>
  </div>
</template>

<script>
module.exports = {
  data: function () {
    return {
      span: 8,
      files: [],
      formData: {
        workingDirectory: "AnnoService01",
        nodeName: null,
        cmd: "dotnet ViperService.dll -p 7018",
        autoStart: "1",
        deploySecret:""
      },
      nodeOptions: [       
      ],
      autoStartOptions: [
        {
          value: "0",
          label: "手工启动",
        },
        {
          value: "1",
          label: "自动启动",
        },
      ],
      rules: {
        workingDirectory: [
          {
            required: true,
            message: "请输入工作目录名称",
            trigger: "blur",
          },
        ],
        cmd: [
          {
            required: true,
            message: "请输入启动命令",
            trigger: "blur",
          },
        ],
        deploySecret:[
          {
            required: true,
            message: "请输入部署口令",
            trigger: "blur",
          }
        ]
      }
    };
  },
  created: function () {
    var that = this;
    this.$nextTick(function () {
      document.getElementsByClassName(
        "el-upload__input"
      )[0].webkitdirectory = true;
    });
    if (this.isMobile() === true) {
      this.span = 23;
    }
    //用于数据初始化
    document.title = "上传部署文件";
    this.keyupAnno();
  },
  mounted: function () {
    var that = this;
    $("input[name=deployFile]").change(function () {
      that.files = this.files;
    });
    this.getDeployNode();
  },
  methods: {
    submitForm: function () {
      var that = this;
      this.$refs["elForm"].validate(function (valid) {
        if (!valid) return;
        that.deployService();
      });
    },
    resetForm: function () {
      this.$refs["elForm"].resetFields();
    },
    keyupAnno: function () {
      var that = this;
      document.onkeydown = function (e) {
        var _key = window.event.keyCode;
        if (_key === 13) {
          that.submitForm();
        }
      };
    },
    deployService: function () {
      var that = this;
      var input = new FormData();
      input.append("profile", localStorage.token);
      input.append("uname", localStorage.account);
      input.append("channel", "Anno.Plugs.Deploy");
      input.append("router", "FileManager");
      input.append("method", "UpLoadFile");
      input.append("nodeName", that.formData.nodeName);
      input.append("formData", JSON.stringify(that.formData));
      if (that.files.length <= 0) {
        that.$message.error("没有找到要部署的文件");
        return;
      }
      for (var i = 0; i < that.files.length; i++) {
        input.append(
          "annoDeploy___" + that.files[i].webkitRelativePath,
          that.files[i]
        );
      }
      $.ajax({
        type: "post",
        dataType: "json",
        url:
          window.location.origin === undefined
            ? window.location.protocol +
              "//" +
              window.location.host +
              "/Deploy/Api"
            : window.location.origin + "/SysMg/Api", //兼容老版本IE origin
        data: input,
        contentType: false,
        processData: false,
        success: function (data) {
          if (data.status) {
            that.$message({
              showClose: true,
              message: data.msg,
              type: "success",
            });
          } else {
            that.$message.error(data.msg);
          }
        },
      });
    },
    isMobile: function () {
      if (
        window.navigator.userAgent.match(
          /(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i
        )
      ) {
        return true; // 移动端
      } else {
        return false; // PC端
      }
    },
    getDeployNode:function(){
      var that = this;
      var input = bif.getInput();
      input.channel = "Anno.Plugs.Deploy";
      input.router = "DeployManager";
      input.method = "GetDeployServices";
      bif.process(input, function (data) {
        if (data.status) {
          that.nodeOptions =[];
          for (var index = 0; index < data.outputData.length; index++) {
            var node = data.outputData[index];
            that.nodeOptions.push(
              {
                value: node.Nickname,
                label:node.Nickname
              }
            );
            if(index===0){
              that.formData.nodeName= node.Nickname;
            }
          }
        } else {
          that.$message.error(data.msg);
        }
      });
    }
  },
};
</script>
<style scoped>
.el-card {
  margin-bottom: 10px;
}
.el-card__header {
  padding: 5px 20px;
  font-size: 14px;
}
.el-card__body {
  padding-bottom: 0px;
}
.el-upload {
  font-size: 48px;
}
</style>