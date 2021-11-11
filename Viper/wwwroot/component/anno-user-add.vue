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
            <el-form-item label="用户：" prop="name">
              <el-input
                v-model="formData.name"
                placeholder="请输入用户"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="span">
            <el-form-item label="登录名：" prop="account">
              <el-input
                v-model="formData.account"
                placeholder="请输入登录名"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="span">
            <el-form-item label="职位：" prop="position">
              <el-input
                v-model="formData.position"
                placeholder="请输入职位"
                clearable
                :style="{ width: '100%' }"
              >
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="span">
            <el-form-item label="状态：" prop="state">
              <el-select
                v-model="formData.state"
                :style="{ width: '100%' }"
                placeholder="请选择"
              >
                <el-option
                  v-for="item in stateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="span">
            <el-form-item label="密码：" prop="pwd">
              <el-input
                show-password
                v-model="formData.pwd"
                placeholder="请输入密码："
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
        ref="roleDataTable"
        stripe 
        @selection-change="handleSelectionChange"
        size="mini"
        border style="width: 100%">
        <el-table-column type="selection" width="55"> </el-table-column>
        <el-table-column prop="ID" label="角色编号" width="200">
        </el-table-column>
        <el-table-column prop="name" label="角色"> </el-table-column>
      </el-table>
    </el-form>
  </div>
</template>

<script>
module.exports = {
  data: function () {
    return {
      span: 8,
      roleData: [],
      formData: {
        name:"",
        account:"",
        position:"",
        state:"1",
        pwd:""
      },
      stateOptions: [
        {
          value: "0",
          label: "禁用"
        },
        {
          value: "1",
          label: "启用"
        }
      ],
      rules: {
        name: [
          {
            required: true,
            message: "请输入用户名",
            trigger: "blur",
          }
        ],
        account: [
          {
            required: true,
            message: "请输入登录名",
            trigger: "blur",
          }
        ],
        state: [
          {
            required: true,
            message: "请选择状态",
            trigger: "blur",
          }
        ],
        pwd: [
          {
            required: true,
            message: "请输入密码",
            trigger: "blur",
          }
        ]
      },
      roleDataTable:[]
    };
  },
  created: function () {
    if (this.isMobile()===true) {
      this.span = 23;
    }
    //用于数据初始化
    document.title = "新增用户";
    this.getRoleData();
    this.keyupAnno();
  },
  methods: {
    getRoleData: function () {
      var that = this;
      var input = anno.getInput();
      input.uid = -1;
          anno.process(input, "Anno.Plugs.Logic/Platform/GetcurRoles", function (data) {
        if (data.status===true) {
          that.roleData = data.outputData.lr;
        } else {
          that.$message.error("获取角色：" + data.msg);
        }
      });
    },
    submitForm: function () {
      var that=this;
      this.$refs["elForm"].validate(function(valid){
        if (!valid) return;
        if(that.roleDataTable.length<=0){
           that.$message.error("至少选择一个角色");
          return;
        }
        that.addUser();
      });
    },
    resetForm: function () {
      this.$refs["elForm"].resetFields();
    },
    keyupAnno: function () {
      var that=this;
      document.onkeydown = function (e) {
        var _key = window.event.keyCode;
        if (_key === 13) {
          that.submitForm();
        }
      };
    },
    addUser: function () {
      var that = this; 
      var input = anno.getInput();
      input.ubase = JSON.stringify(that.formData);
      input.uroles = JSON.stringify(that.roleDataTable);
        anno.process(input, "Anno.Plugs.Logic/Platform/AddUser", function (data) {
        if (data.status===true) {
          that.$message({
            showClose: true,
            message: "保存成功",
            type: "success",
          });
          window.location.href ='./mlist/index.html';
        } else {
          that.$message.error(data.msg);
        }
      });
    },
    handleSelectionChange:function(val) {
        this.roleDataTable = val;
    },
    isMobile: function () {
      if(window.navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i)) {
        return true; // 移动端
      } else {
        return false; // PC端
      }
    }
  }
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