<template>
    <div>
        <el-form ref="form"
                 size="mini"
                 :model="form">

            <el-form-item>
                搜索内容：
                <el-input style="width:40%;"
                          prefix-icon="el-icon-search"
                          placeholder="服务名称"
                          value="value"
                          v-model="form.title"></el-input>
                <el-button type="primary" v-on:keyup.enter="onQuery" @click="onQuery">查询</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>
<script>
    var at = null;
    module.exports = {
        props: {
            value: {
                type: String,
                default: function () {
                    return "";
                }
            }
        },
        data:function() {
            return {
                form: { title: this.value }
            };
        },
        watch: {
            value: function (val, oval) {
                this.form.title = val;
            }
        },
        created: function () {//用于数据初始化
            at = this;
           
            this.keyupAnno();
        },
        methods: {
            onQuery: function () {
                this.$parent.onQuery(this.form.title);
            },
            keyupAnno: function () {
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {
                        this.onQuery();
                    }
                }
            }
        }
    };
</script>
<style scoped>
    .el-button {
        font-size: 10px;
    }

    .warning-row {
        background: oldlace;
    }

    .success-row {
        background: #f0f9eb;
    }

    body {
        height: 100%;
        -moz-osx-font-smoothing: grayscale;
        -webkit-font-smoothing: antialiased;
        text-rendering: optimizeLegibility;
        font-family: Helvetica Neue,Helvetica,PingFang SC,Hiragino Sans GB,Microsoft YaHei,Arial,sans-serif;
    }
</style>