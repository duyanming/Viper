<template>
    <div>
        <el-form ref="elForm"
                 :model="formData"
                 :rules="rules"
                 size="mini"
                 label-width="100px"
                 label-position="right">
            <el-card>
                <div slot="header" class="clearfix">
                    <span>基础信息</span>
                </div>
                <el-row>
                    <el-col :span="span">
                        <el-form-item label="工作目录：" prop="workingDirectory">
                            <el-input v-model="formData.workingDirectory"
                                      placeholder="请输入工作目录："
                                      clearable
                                      :style="{ width: '100%' }">
                            </el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="span">
                        <el-form-item label="部署节点：" prop="nodeName">
                            <el-select v-model="formData.nodeName"
                                       :style="{ width: '100%' }"
                                       placeholder="请选择">
                                <el-option v-for="item in nodeOptions"
                                           :key="item.value"
                                           :label="item.label"
                                           :value="item.value">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                    <el-col :span="span">
                        <el-form-item label="启动方式：" prop="autoStart">
                            <el-select v-model="formData.autoStart"
                                       :style="{ width: '100%' }"
                                       placeholder="请选择">
                                <el-option v-for="item in autoStartOptions"
                                           :key="item.value"
                                           :label="item.label"
                                           :value="item.value">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col span="16">
                        <el-form-item label="启动命令：" prop="cmd">
                            <el-input placeholder="请输入启动命令"
                                      v-model="formData.cmd"
                                      clearable>
                            </el-input>
                        </el-form-item>
                    </el-col>
                    <el-col span="8">
                        <el-form-item label="服务描述：" prop="annoProcessDescription">
                            <el-input placeholder="请输入服务描述"
                                      v-model="formData.annoProcessDescription"
                                      clearable>
                            </el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col span="8">
                        <el-form-item label="部署口令：" prop="deploySecret">
                            <el-input show-password
                                      placeholder="请输入部署口令"
                                      v-model="formData.deploySecret"
                                      clearable>
                            </el-input>
                        </el-form-item>
                    </el-col>
                    <el-col span="16">
                        <el-form-item label-width="140"
                                      label="本地部署文件夹："
                                      prop="deployFile">
                            <el-upload name="deployFile"
                                       :auto-upload="false"
                                       :show-file-list="false"
                                       class="upload-demo"
                                       multiple>
                                <i class="el-icon-upload">
                                    <div slot="tip" class="el-upload__tip">
                                        请选择要部署的文件夹,此功能支持Chrome、Edge
                                    </div>
                                </i>
                            </el-upload>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-card>
            <el-form-item size="mini" style="text-align: center">
                <el-button type="primary" @click="submitForm" v-loading.fullscreen.lock="fullscreenLoading">提交</el-button>
                <el-button @click="resetForm">重置</el-button>
            </el-form-item>
            <el-card>
                <div slot="header" class="clearfix">
                    <span>服务输出日志</span>
                </div>
                <el-row>
                    <el-col span="24">
                        <el-input autosize
                                  type="textarea"
                                  placeholder="服务控制台输出"
                                  :value="formData.standardError + formData.standardOutput">
                        </el-input>
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
                fullscreenLoading: false,
                queryArgs: {
                    workingDirectory: "",
                    nodeName: "",
                },
                span: 8,
                files: [],
                formData: {
                    workingDirectory: "AnnoService01",
                    nodeName: null,
                    cmd: "dotnet ViperService.dll -p 7018",
                    autoStart: "1",
                    deploySecret: "",
                    annoProcessDescription: "",
                    standardError: "",
                    standardOutput: "",
                },
                nodeOptions: [],
                autoStartOptions: [
                    {
                        value: "0",
                        label: "手工启动",
                    },
                    {
                        value: "1",
                        label: "自动启动",
                    },
                    {
                        value: "2",
                        label: "Windows服务",
                    }
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
                    deploySecret: [
                        {
                            required: true,
                            message: "请输入部署口令",
                            trigger: "blur",
                        },
                    ],
                },
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
            var args = anno.GetUrlParms();
            if (args.WorkingDirectory != undefined) {
                that.queryArgs.workingDirectory = args.WorkingDirectory;
                that.queryArgs.nodeName = args.NodeName;
                that.formData.workingDirectory = args.WorkingDirectory;
                that.formData.nodeName = args.NodeName;
                that.formData.annoProcessDescription = args.AnnoProcessDescription;
            }
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
                that.fullscreenLoading = true;
                var input = new FormData();
                var url = "Anno.Plugs.Deploy/FileManager/UpLoadFile/" + that.formData.nodeName
                input.append("nodeName", that.formData.nodeName);
                input.append("annoToken", localStorage.annoToken);
                input.append("formData",
                    JSON.stringify(
                        {
                        workingDirectory: that.formData.workingDirectory,
                        nodeName: that.formData.nodeName,
                        cmd: that.formData.cmd,
                        autoStart: that.formData.autoStart,
                        deploySecret: that.formData.deploySecret,
                        annoProcessDescription: that.formData.annoProcessDescription
                        }
                    )
                );
                if (that.queryArgs.workingDirectory === "" && that.files.length <= 0) {
                    that.$message.error("没有找到要部署的文件");
                    that.fullscreenLoading = false;
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
                    url: that.getUrl(url),
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
                        that.fullscreenLoading = false;
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
            getDeployNode: function () {
                var that = this;
                var input = anno.getInput();
                anno.process(input, "DeployService", function (data) {
                    if (data.status) {
                        that.nodeOptions = [];
                        for (var index = 0; index < data.outputData.length; index++) {
                            var node = data.outputData[index];
                            that.nodeOptions.push({
                                value: node.Nickname,
                                label: node.Nickname,
                            });
                            if (index === 0) {
                                that.formData.nodeName = node.Nickname;
                            }
                        }
                        if (that.queryArgs.workingDirectory != "") {
                            that.getServiceByWorkingName();
                        }
                    } else {
                        that.$message.error(data.msg);
                    }
                });
            },
            getServiceByWorkingName: function () {
                var that = this;
                var input = new FormData();
                var url = "Anno.Plugs.Deploy/DeployManager/GetServiceByWorkingName/" + that.queryArgs.nodeName;
                input.append("nodeName", that.queryArgs.nodeName);
                input.append("workingName", that.queryArgs.workingDirectory);
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
                            that.formData.workingDirectory = data.outputData.WorkingDirectory;
                            that.formData.nodeName = data.outputData.NodeName;
                            that.formData.cmd = data.outputData.Cmd;
                            that.formData.autoStart = data.outputData.AutoStart;
                            that.formData.annoProcessDescription =
                                data.outputData.AnnoProcessDescription;
                            that.formData.standardError = data.outputData.StandardError;
                            that.formData.standardOutput = data.outputData.StandardOutput;
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
    .el-upload__tip {
        font-size: 12px;
        color: #606266;
        margin-top: 0px;
    }

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

    .el-textarea {
        margin-bottom: 20px !important;
    }

    .el-textarea__inner {
        min-height: 50px !important;
    }
</style>