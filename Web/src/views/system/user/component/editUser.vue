<template>
	<div class="sys-user-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-tabs v-loading="state.loading" v-model="state.selectedTabName">
				<el-tab-pane :label="$t('message.list.basicInfo')" style="height: 600px; overflow-y: auto; overflow-x: hidden">
					<el-form :model="state" ref="ruleFormRef" label-width="auto">
						<el-row :gutter="10">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.employeeId')" prop="ruleForm.jobNum" :rules="[{ required: true, message: $t('message.list.employeeIdRequired'), trigger: 'blur' }, { validator: validateJobNum, trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.jobNum" :placeholder="$t('message.list.employeeId')" clearable @clear="onJobNumClear" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.realName')">
									<el-input v-model="state.empName" :placeholder="$t('message.list.realName')" readonly />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.account')" prop="ruleForm.account" :rules="[{ required: true, message: $t('message.list.accountNameRequired'), trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.account" :placeholder="$t('message.list.account')" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.nickname')">
									<el-input v-model="state.ruleForm.nickName" :placeholder="$t('message.list.nickname')" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.loginPassword')" prop="loginPassword" :rules="[{ validator: validateLoginPassword, trigger: 'blur' }]">
									<el-input v-model="state.loginPassword" type="password" :placeholder="$t('message.list.loginPassword')" autocomplete="new-password" show-password clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.confirmPassword')" prop="confirmPassword" :rules="[{ validator: validateConfirmPassword, trigger: 'blur' }]">
									<el-input v-model="state.confirmPassword" type="password" :placeholder="$t('message.list.confirmPassword')" autocomplete="new-password" show-password clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.accountType')" prop="ruleForm.accountType" :rules="[{ required: true, message: $t('message.list.accountTypeRequired'), trigger: 'blur' }]">
									<el-select v-model="state.ruleForm.accountType" :placeholder="$t('message.list.accountType')" collapse-tags collapse-tags-tooltip class="w100">
										<el-option :label="$t('message.list.systemAdmin')" :value="888" :disabled="userInfos.accountType != 888 && userInfos.accountType != 999" />
										<el-option :label="$t('message.list.normalAccount')" :value="777" />
										<el-option :label="$t('message.list.member')" :value="666" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.email')">
									<el-input v-model="state.ruleForm.email" :placeholder="$t('message.list.email')" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb5">
								<el-form-item :label="$t('message.list.orderNo')">
									<el-input-number v-model="state.ruleForm.orderNo" :placeholder="$t('message.list.orderNo')" class="w100" />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">{{ $t('message.list.orgStructure') }}</div>
							</el-divider>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.organization')" prop="ruleForm.orgId" :rules="[{ required: true, message: $t('message.list.organizationRequired'), trigger: 'change' }]">
									<div class="org-pick-field">
										<el-input :model-value="state.orgDisplayName" :placeholder="$t('message.list.organization')" readonly class="org-pick-input" />
										<el-button class="org-pick-btn" @click="openOrgPick()">....</el-button>
									</div>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.jobTitle')">
									<el-select v-model="state.ruleForm.posId" :placeholder="$t('message.list.jobTitle')" class="w100">
										<el-option v-for="d in state.posData" :key="d.id" :label="d.name" :value="d.id">
											<span style="float: left"> {{ d.name }} </span>
											<span style="float: right; color: var(--el-text-color-secondary)"> {{ d.type }} </span>
										</el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item :label="$t('message.list.domainAccount')">
									<el-input v-model="state.ruleForm.domainAccount" :placeholder="$t('message.list.domainAccount')" clearable />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">{{ $t('message.list.affiliatedOrg') }}</div>
							</el-divider>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb10">
								<el-button icon="ele-Plus" type="primary" @click="addExtOrgRow"> {{ $t('message.list.addAffiliatedOrg') }} </el-button>
								<span style="font-size: 12px; color: gray; padding-left: 5px"> {{ $t('message.list.orgDataPermission') }} </span>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<template v-if="state.ruleForm.extOrgIdList != undefined && state.ruleForm.extOrgIdList.length > 0">
									<el-row :gutter="10" v-for="(v, k) in state.ruleForm.extOrgIdList" :key="k">
										<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
											<el-form-item :label="$t('message.list.belongingOrg')" :prop="`ruleForm.extOrgIdList[${k}].orgId`" :rules="[{ required: true, message: $t('message.list.orgRequired'), trigger: 'change' }]">
												<template #label>
													<el-button icon="ele-Delete" type="danger" circle size="small" @click="deleteExtOrgRow(k)" />
													<span class="ml5">{{ $t('message.list.belongingOrg') }}</span>
												</template>
												<div class="org-pick-field">
													<el-input :model-value="state.extOrgDisplayNames[k] || ''" :placeholder="$t('message.list.belongingOrg')" readonly class="org-pick-input" />
													<el-button class="org-pick-btn" @click="openOrgPick(k)">....</el-button>
												</div>
											</el-form-item>
										</el-col>
										<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
											<el-form-item :label="$t('message.list.jobTitle')">
												<el-select v-model="state.ruleForm.extOrgIdList[k].posId" :placeholder="$t('message.list.jobTitle')" class="w100">
													<el-option v-for="d in state.posData" :key="d.id" :label="d.name" :value="d.id">
														<span style="float: left"> {{ d.name }} </span>
														<span style="float: right; color: var(--el-text-color-secondary)"> {{ d.type }} </span>
													</el-option>
												</el-select>
											</el-form-item>
										</el-col>
									</el-row>
								</template>
								<el-empty :image-size="50" :description="$t('message.list.noData')" v-else></el-empty>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane :label="$t('message.list.roleAuth')" style="height: 600px; overflow-y: auto; overflow-x: hidden">
					<Transfer :left-title="$t('message.list.unauthorized')" :right-title="$t('message.list.authorized')" v-model:leftData="state.availableRoles" v-model:rightData="state.grantedRoles" />
				</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleCloseFilled" @click="cancel">{{ $t('message.list.cancelButtonText') }}</el-button>
					<el-button type="primary" icon="ele-CircleCheckFilled" @click="submit">{{ $t('message.list.confirmButtonText') }}</el-button>
				</span>
			</template>
		</el-dialog>

		<OrgPickDialog
			v-model:visible="state.orgPickVisible"
			:org-data="(props.orgData as SysOrg[]) || []"
			:model-value="state.orgPickTargetIndex == null ? state.ruleForm.orgId : state.ruleForm.extOrgIdList?.[state.orgPickTargetIndex]?.orgId"
			@confirm="onOrgPickConfirm"
		/>
	</div>
</template>

<script lang="ts" setup name="sysEditUser">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import Transfer from '/@/components/transfer/index.vue';
import OrgPickDialog from '/@/components/selector/orgPickDialog.vue';
import { useI18n } from 'vue-i18n';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi, SysRoleApi, SysUserApi } from '/@/api-services/system/api';
import { SysOrg, PagePosOutput, UpdateUserInput } from '/@/api-services/system/models';
import { getHrEmpById } from '/@/api/hrms/hrOrg';

const { t } = useI18n();
const props = defineProps({
	title: String,
	orgData: Array<SysOrg>,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const storesUserInfo = useUserInfo();
const { userInfos } = storeToRefs(storesUserInfo);
const state = reactive({
	loading: false,
	isShowDialog: false,
	selectedTabName: '0', // 选中的 tab 页
	ruleForm: {} as UpdateUserInput,
	loginPassword: '',
	confirmPassword: '',
	/** 人事姓名（h_emp_mstr.emp_name），仅展示不落库 */
	empName: '' as string,
	/** 工号是否已在人事资料中校验通过 */
	jobNumVerified: false,
	posData: [] as Array<PagePosOutput>, // 职位数据
	availableRoles: [] as any, // 可授权角色
	grantedRoles: [] as any, // 已授权角色
	orgDisplayName: '' as string,
	extOrgDisplayNames: [] as string[],
	orgPickVisible: false,
	/** null=主管辖部门；数字=附属管辖部门下标 */
	orgPickTargetIndex: null as number | null,
});

/** 工号须存在于人事资料，并带出姓名（仅展示） */
const validateJobNum = (_rule: any, _value: string, callback: any) => {
	const jobNum = (state.ruleForm.jobNum || '').trim();
	if (!jobNum) {
		state.empName = '';
		state.jobNumVerified = false;
		callback();
		return;
	}
	getHrEmpById(jobNum)
		.then((res) => {
			const emp = res.data?.result;
			if (emp?.empId) {
				state.empName = emp.empName || '';
				state.jobNumVerified = true;
				callback();
			} else {
				state.empName = '';
				state.jobNumVerified = false;
				callback(new Error(t('message.list.employeeNotFound')));
			}
		})
		.catch(() => {
			state.empName = '';
			state.jobNumVerified = false;
			callback(new Error(t('message.list.employeeNotFound')));
		});
};

/** 新增必填；编辑留空表示不修改 */
const validateLoginPassword = (_rule: any, value: string, callback: any) => {
	const isEdit = state.ruleForm.id != undefined && state.ruleForm.id > 0;
	if (!state.loginPassword) {
		if (isEdit) callback();
		else callback(new Error(t('message.list.loginPasswordRequired')));
		return;
	}
	callback();
};

const validateConfirmPassword = (_rule: any, _value: string, callback: any) => {
	const isEdit = state.ruleForm.id != undefined && state.ruleForm.id > 0;
	if (!state.loginPassword && !state.confirmPassword) {
		if (isEdit) callback();
		else callback(new Error(t('message.list.confirmPasswordRequired')));
		return;
	}
	if (!state.confirmPassword) {
		callback(new Error(t('message.list.confirmPasswordRequired')));
		return;
	}
	if (state.confirmPassword !== state.loginPassword) {
		callback(new Error(t('message.list.passwordNotMatch')));
		return;
	}
	callback();
};

/** 按 Id 解析机构路径名称 */
const resolveOrgPath = (id: number | string | undefined | null): string => {
	if (id == null || id === '') return '';
	const walk = (nodes: SysOrg[], path: string[]): string => {
		for (const n of nodes) {
			const next = [...path, n.name ?? ''];
			if (n.id == id) return next.filter(Boolean).join(' / ');
			if (n.children?.length) {
				const found = walk(n.children as SysOrg[], next);
				if (found) return found;
			}
		}
		return '';
	};
	return walk((props.orgData as SysOrg[]) || [], []);
};

const refreshOrgDisplayNames = () => {
	state.orgDisplayName = resolveOrgPath(state.ruleForm.orgId) || (state.ruleForm as any).orgName || '';
	const list = state.ruleForm.extOrgIdList ?? [];
	state.extOrgDisplayNames = list.map((item: any) => resolveOrgPath(item?.orgId));
};

/** 编辑回填时静默带出人事姓名 */
const loadEmpNameByJobNum = async () => {
	const jobNum = (state.ruleForm.jobNum || '').trim();
	state.empName = '';
	state.jobNumVerified = false;
	if (!jobNum) return;
	try {
		const res = await getHrEmpById(jobNum);
		const emp = res.data?.result;
		if (emp?.empId) {
			state.empName = emp.empName || '';
			state.jobNumVerified = true;
		}
	} catch {
		/* 静默：编辑打开时不弹错，提交时再校验 */
	}
};

const onJobNumClear = () => {
	state.empName = '';
	state.jobNumVerified = false;
};

// 页面初始化
onMounted(async () => {
	state.loading = true;
	const { data } = await getAPI(SysPosApi).apiSysPosListGet();
	state.posData = data.result ?? [];
	state.loading = false;
});

// 打开弹窗
const openDialog = async (row: any) => {
	ruleFormRef.value?.resetFields();

	state.selectedTabName = '0'; // 重置为第一个 tab 页
	state.loginPassword = '';
	state.confirmPassword = '';
	state.empName = '';
	state.jobNumVerified = false;
	state.ruleForm = JSON.parse(JSON.stringify(row));
	if (row.id != undefined) {
		const { data } = await getAPI(SysUserApi).apiSysUserOwnRoleListUserIdGet(row.id);
		state.availableRoles = data.result?.availableRoles;
		state.grantedRoles = data.result?.grantedRoles;
		var resExtOrg = await getAPI(SysUserApi).apiSysUserOwnExtOrgListUserIdGet(row.id);
		state.ruleForm.extOrgIdList = resExtOrg.data.result;
	} else {
		state.ruleForm.accountType = 777; // 默认普通账号类型
		// state.ruleForm.posId = 1300000000106; // 默认职位
		const { data } = await getAPI(SysRoleApi).apiSysRoleListGet();
		state.availableRoles = data.result ?? [];
		state.grantedRoles = [];
		// state.grantedRoles = [{ id: 1300000000104, name: '仅本人数据' }]; // 默认仅本人数据角色
	}
	refreshOrgDisplayNames();
	state.isShowDialog = true;
	if (state.ruleForm.jobNum) await loadEmpNameByJobNum();
};

const openOrgPick = (extIndex: number | null = null) => {
	state.orgPickTargetIndex = extIndex;
	state.orgPickVisible = true;
};

const onOrgPickConfirm = (payload: { id: number; name: string; path: string }) => {
	if (state.orgPickTargetIndex == null) {
		state.ruleForm.orgId = payload.id;
		state.orgDisplayName = payload.path || payload.name;
		ruleFormRef.value?.validateField?.('ruleForm.orgId');
	} else {
		const idx = state.orgPickTargetIndex;
		if (!state.ruleForm.extOrgIdList) state.ruleForm.extOrgIdList = [];
		const row = state.ruleForm.extOrgIdList[idx] ?? {};
		row.orgId = payload.id;
		state.ruleForm.extOrgIdList[idx] = row;
		state.extOrgDisplayNames[idx] = payload.path || payload.name;
		ruleFormRef.value?.validateField?.(`ruleForm.extOrgIdList[${idx}].orgId`);
	}
};

// 关闭弹窗
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 提交
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.grantedRoles?.length > 0) state.ruleForm.roleIdList = state.grantedRoles.map((e: any) => e.id);
		else {
			ElMessage.error(t('message.list.pleaseAssignRole'));
			return;
		}
		// 姓名仅展示人事 emp_name，不落库；SysUser.RealName 缺省用昵称/账号兜底
		if (!state.ruleForm.realName) {
			state.ruleForm.realName = state.ruleForm.nickName || state.ruleForm.account;
		}
		const payload = {
			...state.ruleForm,
			jobNum: (state.ruleForm.jobNum || '').trim(),
			loginPassword: state.loginPassword || undefined,
		} as any;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysUserApi).apiSysUserUpdatePost(payload);
			if (state.loginPassword) {
				await getAPI(SysUserApi).apiSysUserResetPwdPost({ id: state.ruleForm.id, newPassword: state.loginPassword });
			}
		} else {
			await getAPI(SysUserApi).apiSysUserAddPost(payload);
		}
		closeDialog();
	});
};

// 增加附属机构行
const addExtOrgRow = () => {
	if (state.ruleForm.extOrgIdList == undefined) state.ruleForm.extOrgIdList = [];
	state.ruleForm.extOrgIdList?.push({});
	state.extOrgDisplayNames.push('');
};

// 删除附属机构行
const deleteExtOrgRow = (k: number) => {
	state.ruleForm.extOrgIdList?.splice(k, 1);
	state.extOrgDisplayNames.splice(k, 1);
};

// 导出对象
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.org-pick-field {
	display: flex;
	align-items: center;
	width: 100%;
	gap: 6px;
	.org-pick-input {
		flex: 1;
		min-width: 0;
	}
	.org-pick-btn {
		flex-shrink: 0;
		padding: 8px 10px;
		letter-spacing: 1px;
	}
}
</style>
