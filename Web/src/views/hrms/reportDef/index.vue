<template>
	<div class="hr-report-def-container">
		<el-card shadow="hover" :body-style="{ padding: '20px' }">
			<template #header>
				<span>人事资料定义</span>
			</template>
			<p class="tip">请从左侧菜单进入子功能：</p>
			<ul class="link-list">
				<li><strong>人事资料列表定义</strong>（一期）：配置人员列表显示列、宽度与顺序</li>
				<li><strong>人事资料表单定义</strong>（二期）：配置新增/编辑表单字段、占位格数与分组</li>
			</ul>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrReportDef">
</script>

<style scoped>
.hr-report-def-container {
	padding: 8px;
}
.tip {
	margin: 0 0 12px;
	color: var(--el-text-color-regular);
}
.link-list {
	margin: 0;
	padding-left: 20px;
	line-height: 1.9;
	color: var(--el-text-color-secondary);
}
</style>
