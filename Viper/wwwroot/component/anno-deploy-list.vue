<template>
    <div>
        <el-form ref="form" size="mini">
            <el-form-item>
                服务节点：
                <el-select v-model="formData.nodeName"
                           :style="{ width: '250px' }"
                           placeholder="请选择">
                    <el-option v-for="item in formData.nodeOptions"
                               :key="item.value"
                               :label="item.label"
                               :value="item.value">
                    </el-option>
                </el-select>
                <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
            </el-form-item>
        </el-form>
        <el-table :data="deployData"
                  border
                  stripe
                  size="mini"
                  :max-height="window.innerHeight-68"
                  style="width: 100%">
            <el-table-column show-overflow-tooltip="true"
                             type="index"
                             fixed
                             :index="indexMethod">
            </el-table-column>
            <el-table-column prop="Id"
                             show-overflow-tooltip="true"
                             label="进程PID"
                             width="70">
            </el-table-column>
            <el-table-column prop="Running"
                             show-overflow-tooltip="true"
                             label="服务状态"
                             width="100">
                <template slot-scope="scope">
                    <el-tag :type="scope.row.Running ? 'success' : 'danger'">
                        {{
            scope.row.Running ? "Running" : "Stopped"
                        }}
                    </el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="WorkingDirectory"
                             show-overflow-tooltip="true"
                             label="工作目录"
                             width="150">
            </el-table-column>
            <el-table-column prop="AutoStart"
                             show-overflow-tooltip
                             label="启动方式"
                             width="100">
                <template slot-scope="scope">
                    {{ scope.row.AutoStart === "1" ? "自动启动" : ( scope.row.AutoStart === "0" ? "手工启动":"Windows服务") }}
                </template>
            </el-table-column>
            <el-table-column prop="Cmd"
                             show-overflow-tooltip="true"
                             label="启动命令"
                             min-width="320">
            </el-table-column>
            <el-table-column prop="AnnoProcessDescription"
                             show-overflow-tooltip="true"
                             label="描述"
                             width="120">
            </el-table-column>
            <el-table-column prop="NodeName"
                             show-overflow-tooltip="true"
                             label="服务节点"
                             width="120">
            </el-table-column>
            <el-table-column show-overflow-tooltip="true"
                             fixed="right"
                             align="center"
                             width="170"
                             label="操作">
                <template slot-scope="scope">
                    <el-button v-if="!scope.row.Running"
                               @click="Start(scope.row)"
                               type="text"
                               size="small">启动</el-button>
                    <el-button v-if="scope.row.Running"
                               @click="Stop(scope.row)"
                               type="text"
                               size="small">停止</el-button>
                    <el-button @click="Restart(scope.row)" type="text" size="small">重启</el-button>
                    <el-button @click="Recycle(scope.row)" type="text" size="small">回收</el-button>
                    <el-button @click="GoToDeploy(scope.row)" type="text" size="small">部署</el-button>
                    <el-button @click="CopyService(scope.row)" type="text" size="small">复制</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-dialog title="生成副本" :visible.sync="copyServiceVisible">
            <el-divider></el-divider>
            <el-form :model="copyService"
                     :rules="rules"
                     size="mini" :inline="true"
                     label-position="left"
                     ref="newService">
                <el-row>
                    <el-col :span="9">
                        <el-form-item label="原服务"
                                      :label-width="copyServiceformLabelWidth">
                            <el-input class="col1_width"
                                      :disabled="true"
                                      v-model="copyService.WorkingName"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="15">
                        <el-form-item label="启动命令"
                                      :label-width="copyServiceformLabelWidth">
                            <el-input :disabled="true"
                                      class="col2_width"
                                      v-model="copyService.Cmd" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="9">
                        <el-form-item prop="NewWorkingName"
                                      label="新服务" :label-width="copyServiceformLabelWidth">
                            <el-input class="col1_width"
                                      v-model="copyService.NewWorkingName"
                                      autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="15">
                        <el-form-item label="启动命令"
                                      prop="NewCmd"
                                      :label-width="copyServiceformLabelWidth">
                            <el-input class="col2_width" v-model="copyService.NewCmd" autocomplete="off"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="9">
                        <el-form-item prop="ReceiverdeploySecret"
                                      label="部署口令" :label-width="copyServiceformLabelWidth">
                            <el-input show-password
                                      class="col1_width"
                                      v-model="copyService.ReceiverdeploySecret"
                                      autocomplete="off">
                            </el-input>
                    </el-col>
                    <el-col :span="15">
                        <el-form-item prop="ReceiverNodeName"
                                      label="部署节点" :label-width="copyServiceformLabelWidth">
                            <el-select v-model="copyService.ReceiverNodeName"
                                       :style="{ width: '250px' }"
                                       placeholder="请选择">
                                <el-option v-for="item in formData.nodeOptions"
                                           :key="item.value"
                                           :label="item.label"
                                           :value="item.value">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>

            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button @click="copyServiceVisible = false">取 消</el-button>
                <el-button type="primary" @click="CopyServiceOk">确 定</el-button>
            </div>
        </el-dialog>
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
                copyServiceVisible: false,
                copyServiceformLabelWidth: "80px",
                copyService: {
                    WorkingName: "",
                    NewWorkingName: "",
                    NodeName: "",
                    ReceiverNodeName: "",
                    ReceiverdeploySecret: "",
                    Cmd: "",
                    NewCmd: "",
                },
                rules: {
                    NewWorkingName: [
                        {
                            required: true,
                            message: "请输入新服务名称",
                            trigger: "blur",
                        },
                    ],
                    NewCmd: [
                        {
                            required: true,
                            message: "请输入启动命令",
                            trigger: "blur",
                        },
                    ],
                    ReceiverdeploySecret: [
                        {
                            required: true,
                            message: "请输入部署口令",
                            trigger: "blur",
                        }
                    ],
                    ReceiverNodeName: [
                        {
                            required: true,
                            message: "请选择部署节点",
                            trigger: "blur",
                        }
                    ]
                }
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
                var url = "Anno.Plugs.Deploy/DeployManager/" + action + "/" + row.NodeName;
                input.append("workingName", row.WorkingDirectory);
                input.append("nodeName", row.NodeName);
                input.append("deploySecret", deploySecret);
                input.append("annoToken", localStorage.annoToken);
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: that.getUrl(url),
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
                var input = anno.getInput();
                anno.process(input, "DeployService", function (data) {
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
                var url = "Anno.Plugs.Deploy/DeployManager/GetServices/" + that.formData.nodeName
                var input = new FormData();
                input.append("nodeName", that.formData.nodeName);
                input.append("annoToken", localStorage.annoToken);
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: that.getUrl(url),
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
            CopyService: function (row) {
                this.copyServiceVisible = true;
                this.copyService = {
                    WorkingName: "",
                    NewWorkingName: "",
                    Cmd: "",
                    NewCmd: "",
                    NodeName: "",
                    ReceiverNodeName: "",
                    ReceiverdeploySecret: "",
                };
                this.copyService.NodeName = row.NodeName;
                this.copyService.WorkingName = row.WorkingDirectory;
                this.copyService.Cmd = row.Cmd;
                this.copyService.NewWorkingName = row.WorkingDirectory;
                this.copyService.NewCmd = row.Cmd;
            },
            CopyServiceOk: function () {
                var that = this;
                this.$refs["newService"].validate(function (valid) {
                    if (!valid) return;
                    that.CopyServiceHandle();
                });
            },
            CopyServiceHandle: function () {
                var that = this;
                var input = new FormData();
                var url = "Anno.Plugs.Deploy/DeployManager/DispatchService/" + that.copyService.NodeName;
                input.append("nodeName", that.copyService.NodeName);

                input.append("ReceiverNodeName", that.copyService.ReceiverNodeName);
                input.append("WorkingName", that.copyService.WorkingName);
                input.append("NewWorkingName", that.copyService.NewWorkingName);
                input.append("Cmd", that.copyService.NewCmd);
                input.append("ReceiverdeploySecret", that.copyService.ReceiverdeploySecret);
                input.append("annoToken", localStorage.annoToken);
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url:that.getUrl(url),
                    data: input,
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        if (data.status == true) {
                            that.$message({
                                showClose: true,
                                message: data.msg,
                                type: "success",
                            });
                            that.copyServiceVisible = false;
                        } else {
                            that.$message.error(data.msg);
                        }
                    },
                });
            },
            getUrl(url) {
                //兼容老版本IE origin
                if (window.location.origin === undefined) {
                    return window.location.protocol + "//" + window.location.host + "/AnnoApi/" + url;
                } else {
                    return window.location.origin + "/AnnoApi/" + url;
                }
            }
        },
    };
</script>
<style scoped>
    .el-table__body {
        width: 100%;
        table-layout: fixed !important;
    }

    .el-button + .el-button {
        margin-left: 0px;
    }

    .el-divider--horizontal {
        margin-bottom: 20px;
        margin-top: 0px;
    }

    .el-dialog__body {
        padding: 0px 20px;
    }

    .col1_width {
        width: 150px;
    }

    .col2_width {
        width: 310px;
    }
</style>