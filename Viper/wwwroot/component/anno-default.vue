<template>
    <div>
        <el-calendar v-model="value">
        </el-calendar>
    </div>
</template>
<script>
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
            return { value: new Date()};
        },
        watch: {
        },
        created: function () {//用于数据初始化
           
            this.keyupAnno();
        },
        methods: {
            keyupAnno: function () {
                document.onkeydown = function (e) {
                    var _key = window.event.keyCode;
                    if (_key === 13) {

                    }
                }
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
        font-family: Helvetica Neue,Helvetica,PingFang SC,Hiragino Sans GB,Microsoft YaHei,Arial,sans-serif;
    }
</style>