<template>
	<div class="hr-company-param">
		<el-card shadow="hover" :body-style="{ padding: '8px 12px' }">
			<div class="toolbar">
				<div class="toolbar-left">
					<span class="title">公司参数设置</span>
					<el-tag v-if="state.currentId" size="small" type="info">{{ state.currentSiteCode }} {{ state.currentSiteName }}</el-tag>
				</div>
				<el-button-group>
					<template v-if="state.activeTab !== 'companySite'">
						<el-button type="primary" icon="ele-Plus" v-auth="'hrCompanyParam/add'" @click="openAdd">新增</el-button>
						<el-button type="success" icon="ele-Check" v-auth="'hrCompanyParam/update'" :disabled="!state.detail" :loading="state.saving" @click="handleSave">
							保存
						</el-button>
					</template>
					<template v-else>
						<el-button type="success" icon="ele-Check" v-auth="'hrCompanyParam/update'" :loading="state.saving" @click="handleSaveCompanySite">
							保存
						</el-button>
					</template>
					<el-button icon="ele-Refresh" @click="refresh">刷新</el-button>
				</el-button-group>
			</div>
		</el-card>

		<el-card class="main-card" shadow="hover">
			<el-tabs v-model="state.activeTab" class="cp-tabs" @tab-change="onTabChange">
				<el-tab-pane label="系统选项设置" name="site" class="cp-tab-pane">
					<SiteList ref="siteListRef" @select="onSelect" @open-detail="openDetail" @deleted="onDeleted" />
				</el-tab-pane>
				<el-tab-pane label="参数明细设置" name="detail" class="cp-tab-pane" :disabled="!state.currentId && !state.detail">
					<DetailForm v-model:model="state.detail" />
				</el-tab-pane>
				<el-tab-pane label="归属公司设置" name="companySite" class="cp-tab-pane">
					<CompanySiteList ref="companySiteListRef" />
				</el-tab-pane>
			</el-tabs>
		</el-card>

		<el-dialog v-model="state.addVisible" title="新增公司参数" width="420px" destroy-on-close>
			<el-form :model="state.addForm" label-width="100px">
				<el-form-item label="公司代码" required>
					<el-input v-model="state.addForm.siteCode" maxlength="32" />
				</el-form-item>
				<el-form-item label="公司名称" required>
					<el-input v-model="state.addForm.siteName" maxlength="100" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.addVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="submitAdd">确定</el-button>
			</template>
		</el-dialog>

	</div>
</template>

<script lang="ts" setup name="hrCompanyParam">
import { nextTick, reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import {
	addHrCompanyParam,
	detailHrCompanyParam,
	updateHrCompanyParam,
} from '/@/api/hrms/hrCompanyParam';
import SiteList from './component/siteList.vue';
import DetailForm from './component/detailForm.vue';
import CompanySiteList from './component/companySiteList.vue';

const siteListRef = ref<InstanceType<typeof SiteList>>();
const companySiteListRef = ref<InstanceType<typeof CompanySiteList>>();
const state = reactive({
	activeTab: 'site' as 'site' | 'detail' | 'companySite',
	currentId: null as number | null,
	currentSiteCode: '',
	currentSiteName: '',
	detail: null as any,
	saving: false,
	addVisible: false,
	addForm: { siteCode: '', siteName: '' },
	dirty: false,
});

const onSelect = (row: any) => {
	state.currentId = row?.id ?? null;
	state.currentSiteCode = row?.siteCode ?? '';
	state.currentSiteName = row?.siteName ?? '';
};

const onDeleted = (id: number) => {
	if (state.currentId !== id) return;
	state.currentId = null;
	state.currentSiteCode = '';
	state.currentSiteName = '';
	state.detail = null;
	state.activeTab = 'site';
};

const loadDetail = async (id: number) => {
	const res = await detailHrCompanyParam({ id });
	state.detail = res.data?.result ?? null;
	if (state.detail) {
		state.currentId = state.detail.id;
		state.currentSiteCode = state.detail.siteCode;
		state.currentSiteName = state.detail.siteName;
	}
	state.dirty = false;
};

const openDetail = async (row: any) => {
	if (!row?.id) return;
	onSelect(row);
	await loadDetail(row.id);
	state.activeTab = 'detail';
};

const onTabChange = async (name: string | number) => {
	if (name === 'detail' && state.currentId && !state.detail) {
		await loadDetail(state.currentId);
	}
	if (name === 'companySite') {
		await companySiteListRef.value?.reload();
	}
};

const handleSaveCompanySite = async () => {
	state.saving = true;
	try {
		await companySiteListRef.value?.save();
	} finally {
		state.saving = false;
	}
};

const openAdd = () => {
	state.addForm = { siteCode: '', siteName: '' };
	state.addVisible = true;
};

const submitAdd = async () => {
	const siteCode = state.addForm.siteCode.trim();
	const siteName = state.addForm.siteName.trim();
	if (!siteCode || !siteName) {
		ElMessage.warning('请填写公司代码与名称');
		return;
	}
	state.saving = true;
	try {
		const res = await addHrCompanyParam({ siteCode, siteName });
		const id = res.data?.result as number;
		ElMessage.success('新增成功');
		state.addVisible = false;
		await siteListRef.value?.reload(true);
		await nextTick();
		if (id) {
			await loadDetail(id);
			state.activeTab = 'detail';
		}
	} finally {
		state.saving = false;
	}
};

const handleSave = async () => {
	if (!state.detail?.id) {
		ElMessage.warning('无明细可保存');
		return;
	}
	state.saving = true;
	try {
		await updateHrCompanyParam({ ...state.detail });
		ElMessage.success('保存成功');
		state.dirty = false;
		await siteListRef.value?.reload();
		await loadDetail(state.detail.id);
	} finally {
		state.saving = false;
	}
};

const refresh = async () => {
	if (state.activeTab === 'site') await siteListRef.value?.reload(true);
	else if (state.activeTab === 'companySite') await companySiteListRef.value?.reload(true);
	else if (state.currentId) await loadDetail(state.currentId);
};
</script>

<style scoped>
.hr-company-param {
	height: 100%;
	display: flex;
	flex-direction: column;
	overflow: hidden;
	gap: 5px;
}
.toolbar {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 12px;
}
.toolbar-left {
	display: flex;
	align-items: center;
	gap: 10px;
}
.title {
	font-weight: 600;
}
.main-card {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
.main-card :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	padding: 4px 12px 8px;
	overflow: hidden;
}
.cp-tabs {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
.cp-tabs :deep(.el-tabs__header) {
	margin-bottom: 8px;
	flex-shrink: 0;
}
.cp-tabs :deep(.el-tabs__content) {
	flex: 1;
	min-height: 0;
	overflow: hidden;
}
.cp-tabs :deep(.el-tab-pane) {
	height: 100%;
}
</style>
