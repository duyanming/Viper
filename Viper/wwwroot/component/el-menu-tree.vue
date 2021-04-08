<template>
    <div>
        <el-menu-item v-for="(subitem,iindex) in item.children" :key="iindex" :index="subitem.id" @click="$root.openMenuItem(subitem)">
            <i :class="(subitem.icon==null||subitem.icon=='')?'el-icon-notebook-1':subitem.icon"></i>
            <span slot="title">{{subitem.fname}}</span>
        </el-menu-item>
        <el-submenu v-for="(subitem,iindex) in item.childrenDir" :key="iindex" :index="subitem.id">
            <template slot="title">
                <i :class="(subitem.icon==null||subitem.icon=='')?'el-icon-folder':subitem.icon"></i>
                <span slot="title">{{subitem.fname}}</span>
            </template>
            <el-menu-tree :item="subitem"></el-menu-tree>
        </el-submenu>
    </div>
</template>
<script>
    module.exports = {
        props: {
            item: {
                type: Object,
                default: function () {
                    return {
                        item: { children: [] }
                    }
                }
            }
        },
        methods: {
            
        },
        components: {
            'el-menu-tree': httpVueLoader('/component/el-menu-tree.vue')
        }
    };
</script>
<style>
</style>