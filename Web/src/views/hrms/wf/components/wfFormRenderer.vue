<template>
	<el-form :model="model" label-width="110px" :disabled="readonly">
		<el-row :gutter="12">
			<el-col v-for="f in fields" :key="f.fieldKey" :span="colSpan(f)">
				<el-form-item :label="f.label" :required="!!f.required">
					<el-input
						v-if="f.controlType === 'text' || !f.controlType"
						v-model="model[f.fieldKey]"
						:placeholder="f.placeholder"
						:readonly="readonly || f.readonly"
					/>
					<el-input
						v-else-if="f.controlType === 'textarea'"
						v-model="model[f.fieldKey]"
						type="textarea"
						:rows="3"
						:placeholder="f.placeholder"
						:readonly="readonly || f.readonly"
					/>
					<el-input-number
						v-else-if="f.controlType === 'number'"
						v-model="model[f.fieldKey]"
						class="w100"
						:disabled="readonly || f.readonly"
					/>
					<el-date-picker
						v-else-if="f.controlType === 'date'"
						v-model="model[f.fieldKey]"
						type="date"
						value-format="YYYY-MM-DD"
						class="w100"
						:disabled="readonly || f.readonly"
					/>
					<el-input
						v-else-if="f.controlType === 'dept'"
						v-model="model[f.fieldKey]"
						placeholder="部门内码 CcCode"
						:readonly="readonly || f.readonly"
					/>
					<el-input v-else v-model="model[f.fieldKey]" :readonly="readonly || f.readonly" />
				</el-form-item>
			</el-col>
		</el-row>
	</el-form>
</template>

<script lang="ts" setup>
import { computed } from 'vue';

const props = defineProps<{
	fields: any[];
	modelValue: Record<string, any>;
	readonly?: boolean;
}>();

const emit = defineEmits<{ (e: 'update:modelValue', v: Record<string, any>): void }>();

const model = computed({
	get: () => props.modelValue,
	set: (v) => emit('update:modelValue', v),
});

const fields = computed(() => [...(props.fields || [])].sort((a, b) => (a.sortNo || 0) - (b.sortNo || 0)));

const colSpan = (f: any) => {
	const slots = f.spanSlots ?? 12;
	// 按 24 栅格：1 格≈4，常见 12/24
	if (slots >= 24) return 24;
	if (slots >= 12) return 12;
	if (slots >= 8) return 8;
	return 12;
};
</script>

<style scoped>
.w100 {
	width: 100%;
}
</style>
