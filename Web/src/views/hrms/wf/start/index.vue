<template>
	<div class="hr-wf-start">
		<el-card shadow="hover">
			<template #header>发起申请</template>
			<el-form label-width="110px" class="mb16">
				<el-form-item label="业务类型" required>
					<el-select
						v-model="state.bizType"
						style="width: 280px"
						filterable
						placeholder="请选择业务类型"
						:loading="state.loadingBiz"
						@change="loadSchema"
					>
						<el-option
							v-for="item in state.bizOptions"
							:key="item.bizType"
							:label="item.name"
							:value="item.bizType"
						/>
					</el-select>
				</el-form-item>
				<el-form-item label="单据部门" required>
					<el-input v-model="state.subjectCcCode" placeholder="SubjectCcCode 部门内码" style="width: 280px" />
				</el-form-item>
				<el-form-item label="当事人工号" required>
					<el-input v-model="state.subjectEmpId" placeholder="SubjectEmpId，用于定位汇报路径" style="width: 280px" />
				</el-form-item>
			</el-form>
			<wf-form-renderer v-if="state.fields.length" v-model="state.formData" :fields="state.fields" />
			<el-empty v-else description="请先配置该业务类型的启用表单" />
			<div class="actions">
				<el-button type="primary" :loading="state.saving" :disabled="!state.fields.length" @click="submit">提交</el-button>
			</div>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrWfStart">
import { onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import { getHrWfFormByBizType, pageHrWfForm } from '/@/api/hrms/hrWfForm';
import { startHrWf } from '/@/api/hrms/hrWfRuntime';
import WfFormRenderer from '/@/views/hrms/wf/components/wfFormRenderer.vue';
import { bizTypeLabel, loadHrWfBizTypeOptions } from '/@/views/hrms/wf/wfBizType';

type BizOption = { bizType: string; name: string };

const router = useRouter();
const state = reactive({
	bizType: '' as string,
	bizOptions: [] as BizOption[],
	loadingBiz: false,
	subjectCcCode: '',
	subjectEmpId: '',
	fields: [] as any[],
	formData: {} as Record<string, any>,
	saving: false,
});

const loadBizOptions = async () => {
	state.loadingBiz = true;
	try {
		const [codeOpts, res] = await Promise.all([
			loadHrWfBizTypeOptions(),
			pageHrWfForm({
				page: 1,
				pageSize: 200,
				enabled: true,
				field: 'id',
				order: 'asc',
				descStr: 'desc',
			}),
		]);
		const page = res.data?.result ?? res.data;
		const rows: any[] = page?.items ?? page?.Items ?? [];
		const map = new Map<string, BizOption>();
		for (const row of rows) {
			const bizType = (row.bizType ?? row.BizType ?? '').trim();
			if (!bizType || map.has(bizType)) continue;
			const formName = (row.name ?? row.Name ?? '').trim();
			map.set(bizType, {
				bizType,
				name: bizTypeLabel(codeOpts, bizType) || formName || bizType,
			});
		}
		state.bizOptions = Array.from(map.values());
		if (!state.bizOptions.length) {
			state.bizType = '';
			state.fields = [];
			state.formData = {};
			ElMessage.warning('暂无启用中的审批表单，请先在「审批表单」中配置并启用');
			return;
		}
		if (!state.bizOptions.some((o) => o.bizType === state.bizType)) {
			state.bizType = state.bizOptions[0].bizType;
		}
		await loadSchema();
	} finally {
		state.loadingBiz = false;
	}
};

const loadSchema = async () => {
	state.fields = [];
	state.formData = {};
	if (!state.bizType) return;
	try {
		const res = await getHrWfFormByBizType(state.bizType);
		const d = res.data?.result ?? res.data;
		state.fields = d.fields || [];
		for (const f of state.fields) {
			if (f.defaultValue != null && f.defaultValue !== '') state.formData[f.fieldKey] = f.defaultValue;
			else state.formData[f.fieldKey] = undefined;
		}
	} catch {
		ElMessage.warning('未找到启用表单，请先在「审批表单」中配置');
	}
};

const submit = async () => {
	if (!state.bizType) {
		ElMessage.warning('请选择业务类型');
		return;
	}
	if (!state.subjectCcCode?.trim()) {
		ElMessage.warning('请填写单据部门内码');
		return;
	}
	if (!state.subjectEmpId?.trim()) {
		ElMessage.warning('请填写当事人（单据主体）工号');
		return;
	}
	state.saving = true;
	try {
		const res = await startHrWf({
			bizType: state.bizType,
			subjectCcCode: state.subjectCcCode.trim(),
			subjectEmpId: state.subjectEmpId.trim(),
			formData: state.formData,
			title: state.formData.subject,
		});
		const out = res.data?.result ?? res.data;
		ElMessage.success(`已发起，实例 ${out.instId}`);
		router.push({ path: '/moduleFlowConfig/wfTodo', query: { tab: 'applied' } });
	} finally {
		state.saving = false;
	}
};

onMounted(loadBizOptions);
</script>

<style scoped>
.mb16 { margin-bottom: 16px; }
.actions { margin-top: 16px; }
</style>
