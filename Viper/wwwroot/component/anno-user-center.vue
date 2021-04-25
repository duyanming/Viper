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
          <el-col :span="8">
            <el-form-item label="用户：" prop="name">
              <el-input
                v-model="formData.name"
                placeholder="请输入用户"
                :disabled="true"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="登录名：" prop="account">
              <el-input
                v-model="formData.account"
                placeholder="请输入登录名"
                :disabled="true"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="职位：" prop="position">
              <el-input
                v-model="formData.position"
                placeholder="请输入职位"
                :disabled="true"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="状态：" prop="state">
              <el-select  
                 v-model="formData.state"
                 :disabled="true"
                 clearable
                  filterable placeholder="请选择">
                <el-option
                  v-for="item in stateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                  :style="{ width: '100%' }"
                >
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="最近登录：" prop="timespan">
              <el-input
                v-model="formData.timespan"
                :disabled="true"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>
      <el-card>
        <div slot="header" class="clearfix">
          <span>修改密码</span>
        </div>
        <el-row>
          <el-col :span="8">
            <el-form-item label="旧密码：" prop="opwd">
              <el-input
              show-password
                v-model="pwdData.opwd"
                placeholder="请输入状态"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="新密码：" prop="pwd">
              <el-input
              show-password
                v-model="pwdData.pwd"
                clearable
                :style="{ width: '100%' }"
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
      <el-table :data="roleData"
        stripe
         size="mini" border style="width: 100%">
        <el-table-column prop="ID" label="角色编号" width="200">
        </el-table-column>
        <el-table-column prop="name" label="角色"> </el-table-column>
      </el-table>
    </el-form>
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
      roleData: [
      ],
      formData: {
        
      },
      pwdData:{
        pwd:"",
        opwd:""
      },
      stateOptions:[
        {
          value: '0',
          label: '禁用'
        }, {
          value: '1',
          label: '启用'
        }
      ],
      rules: {
        field101: [
          {
            required: true,
            message: "请输入单行文本单行文本",
            trigger: "blur",
          },
        ],
      },
    };
  },
  watch: {},
  mounted: function () {},
  created: function () {
    //用于数据初始化
    document.title = "用户中心";
    this.getUserData();
    this.keyupAnno();
  },
  methods: {
    getUserData: function () {
      var that = this;
      var input = bif.getInput();
      input.channel = "Anno.Plugs.Logic";
      input.router = "Platform";
      input.method = "PCenter";
      bif.process(input, function (data) {
        if (data.status) {
          that.formData = data.outputData;
        } else {
          that.$message.error(data.msg);
        }
      });
      input.channel = "Anno.Plugs.Logic";
      input.router = "Platform";
      input.method = "GetcurRoles";
      input.uid = JSON.parse(localStorage.profile).ID;
      bif.process(input, function (data) {
          if (data.status) {
              that.roleData =  data.outputData.cur;
          } else {
             that.$message.error("获取角色：" +data.msg);
          }
      });
    },
    submitForm: function () {
      this.$refs["elForm"].validate((valid) => {
        if (!valid) return;
        this.EditPwd();
      });
    },
    resetForm: function () {
      // this.$refs["elForm"].resetFields();
      this.pwdData.pwd="";
       this.pwdData.opwd="";
    },
    keyupAnno: function () {
      document.onkeydown = function (e) {
        var _key = window.event.keyCode;
        if (_key === 13) {
          this.submitForm();
        }
      };
    },
    EditPwd:function() {
      var that=this;
      var input = bif.getInput();
      input.channel = "Anno.Plugs.Logic";
      input.router = "Platform";
      input.method = "ChangePwd";
      input.dto = JSON.stringify({
          pwd: that.pwdData.pwd,
          oldPwd: that.pwdData.opwd,
          ID: JSON.parse(localStorage.profile).ID
      });
      input.opwd =that.pwdData.opwd;
      input.pwd =that.pwdData.pwd;
      bif.process(input, function (data) {
          if (data.status) {
            that.$message({
            showClose: true,
            message: '修改成功',
            type: 'success'
            });
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
</style>