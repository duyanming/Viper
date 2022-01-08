<template>
  <div>
    <el-form ref="form" size="mini">
      <el-form-item>
        角色名称：
        <el-input
          style="width: 20%"
          prefix-icon="el-icon-search"
          placeholder="角色名称"
          v-model="formData.roleName"
        ></el-input>
        <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery"
          >查询</el-button
        >
        <el-button icon="el-icon-plus" type="success" @click="openAddUser"
          >增加角色</el-button
        >
      </el-form-item>
    </el-form>
    <el-table
      :data="roleData"
      border
      stripe
      size="mini"
      :max-height="window.innerHeight-68"
      style="width: 100%"
    >
      <el-table-column type="index" fixed :index="indexMethod">
      </el-table-column>
      <el-table-column
        fixed
        prop="ID"
        show-overflow-tooltip
        label="ID"
        width="180"
      >
      </el-table-column>

      <el-table-column prop="name" show-overflow-tooltip label="角色名称">
      </el-table-column>
      <el-table-column fixed="right" align="center" width="180" label="操作">
        <template slot-scope="scope">
          <el-button @click="deleteRole(scope.row)" type="text" size="small"
            >删除</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <el-dialog :title="currentData.title" :visible.sync="currentData.show">
      <el-form>
        <el-form-item label="角色名称" label-width="120px">
          <el-input v-model="currentData.name" autocomplete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="currentData.show = false">取 消</el-button>
        <el-button type="primary" @click="addUser">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
module.exports = {
  data: function () {
    return {
      roleData: [],
      formData: {
        roleName: "",
      },
      currentData: {
        show: false,
        title: "增加角色",
        ID: -1,
        name: "",
      },
      total: 0,
      currentPage: 1,
      pagesize: 20,
      pagesizes: [10, 20, 30, 40],
    };
  },
  created: function () {
    var that = this;

    //用于数据初始化
    document.title = "角色列表";
    this.keyupAnno();
  },
  mounted: function () {
    this.getRoleData();
  },
  methods: {
    deleteRole: function (row) {
      var that = this;
      this.$confirm("确定要删除角色", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
      }).then(function (rlt) {
        var input = anno.getInput();
        input.ID = row.ID;
          anno.process(input,"Anno.Plugs.Logic/Platform/DelRole", function (data) {
          if (data.status) {
            that.onQuery();
          } else {
            that.$message.error(data.msg);
          }
        });
      });
    },
    onQuery: function () {
      this.getRoleData();
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
    getRoleData: function () {
      var that = this;
      var page = that.currentPage;
      var pagesize = that.pagesize;

      var input = anno.getInput();
      input.roleName = that.formData.roleName;
      if (page !== null && page !== undefined) {
        input.page = page;
      } else {
        input.page = 1;
      }
      if (pagesize !== null && pagesize !== undefined) {
        input.pagesize = pagesize;
      } else {
        input.pagesize = 20;
      }
        anno.process(input,"Anno.Plugs.Logic/Platform/GetRolePage", function (data) {
        if (data.status) {
          that.roleData = data.outputData.Data;
          that.total = parseInt(data.outputData.Total);
        } else {
          that.$message.error(data.msg);
        }
      });
    },
    openAddUser: function () {
      this.currentData.title = "增加角色";
      this.currentData.ID = -1;
      this.currentData.name = "";
      this.currentData.show = true;
    },
    addUser: function () {
      this.currentData.show = false;
      var that = this;
      var input = anno.getInput();
      input.Name =  this.currentData.name;
        anno.process(input,"Anno.Plugs.Logic/Platform/AddRole", function (data) {
        if (data.status===true) {
         that.onQuery();
        } else {
          that.$message.error("添加角色失败：" + data.msg);
        }
      });
    },
  },
};
</script>
<style scoped>
</style>