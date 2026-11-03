<template>
	<el-dialog v-model="visible" title="语种配置" width="520px" destroy-on-close>
		<el-form label-width="88px">
			<el-form-item v-for="item in langs" :key="item.culture" :label="item.culture">
				<el-input v-model="item.label" maxlength="100" placeholder="公司名称" />
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible = false">取消</el-button>
			<el-button type="primary" :loading="saving" @click="save">保存</el-button>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup name="hrCompanyParamLangDialog">
import { reactive, ref, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { langListHrCompanyParam, saveLangHrCompanyParam } from '/@/api/hrms/hrCompanyParam';
import { hrUiCultures } from '/@/views/hrms/composables/hrUiCulture';

const visible = defineModel<boolean>('visible', { default: false });
const props = defineProps<{ paramId: number | null }>();
const emit = defineEmits<{ (e: 'saved'): void }>();

const saving = ref(false);
const langs = reactive<{ culture: string; label: string }[]>([]);

const load = async () => {
	langs.splice(0, langs.length, ...hrUiCultures.map((c) => ({ culture: c.value, label: '' })));
	if (!props.paramId) return;
	const res = await langListHrCompanyParam(props.paramId);
	const map = new Map((res.data?.result ?? []).map((x: any) => [x.culture, x.label]));
	langs.forEach((row) => {
		row.label = (map.get(row.culture) as string) || '';
	});
};

watch(
	() => visible.value,
	(v) => {
		if (v) load();
	}
);

const save = async () => {
	if (!props.paramId) {
		ElMessage.warning('请先选择公司');
		return;
	}
	saving.value = true;
	try {
		await saveLangHrCompanyParam({
			id: props.paramId,
			labels: langs.map((u) => ({ culture: u.culture, label: u.label })),
		});
		ElMessage.success('保存成功');
		visible.value = false;
		emit('saved');
	} finally {
		saving.value = false;
	}
};
</script>
