<template>
	<div class="weChatPay-container">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="订单号">
							<el-input v-model="state.queryParams.keyword" clearable="" placeholder="请输入订单号" />
						</el-form-item>
					</el-col>
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="创建时间">
							<el-date-picker placeholder="请选择创建时间" value-format="YYYY/MM/DD" type="daterange" v-model="state.queryParams.createTimeRange" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>

			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

			<el-row>
				<el-col>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery()"> {{ $t('message.list.query') }} </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> {{ $t('message.list.reset') }} </el-button>
					</el-button-group>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog" style="margin-left: 10px">添加模拟数据</el-button>
				</el-col>
			</el-row>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="65" align="center" />
				<el-table-column prop="outTradeNumber" label="商户订单号" width="180"></el-table-column>
				<el-table-column prop="transactionId" label="支付订单号" width="220"></el-table-column>
				<el-table-column prop="description" label="描述" width="180"></el-table-column>
				<el-table-column prop="total" :formatter="amountFormatter" label="金额" width="70"></el-table-column>
				<el-table-column prop="tradeState" label="状态" width="70">
					<template #default="scope">
						<el-tag v-if="scope.row.tradeState == 'SUCCESS'" type="success"> 完成 </el-tag>
						<el-tag v-else-if="scope.row.tradeState == 'REFUND'" type="danger"> 退款 </el-tag>
						<el-tag v-else-if="scope.row.tradeState == 'NOTPAY'" type="warning"> 未支付 </el-tag>
						<el-tag v-else type="info"> 未完成 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="attachment" label="附加信息" width="180"></el-table-column>
				<el-table-column prop="goodsTag" label="GoodsTag" width="90"></el-table-column>
				<el-table-column prop="tags" label="业务类型" width="90"></el-table-column>
				<el-table-column prop="createTime" label="创建时间" width="150"></el-table-column>
				<el-table-column prop="successTime" label="完成时间" width="150"></el-table-column>
				<el-table-column prop="businessId" label="业务ID" width="130"></el-table-column>
				<el-table-column label="操作" align="center" fixed="right">
					<template #default="scope">
						<el-button
							size="small"
							text
							type="primary"
							v-if="scope.row.qrcodeContent != null && scope.row.qrcodeContent != '' && (scope.row.tradeState === '' || !scope.row.tradeState || scope.row.tradeState == 'NOTPAY')"
							@click="openQrDialog(scope.row.qrcodeContent)"
						>
							付款二维码
						</el-button>
						<el-button size="small" text type="primary" v-if="scope.row.tradeState === 'REFUND'" @click="openRefundDialog(scope.row.transactionId)">查看退款</el-button>
						<el-button size="small" text type="primary" v-if="scope.row.tradeState === 'SUCCESS'" @click="doRefund(scope.row)">全额退款</el-button>
						<el-button size="small" text type="primary" @click="doRefreshRecord(scope.row)">刷新</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<el-dialog v-model="showAddDialog" draggable overflow destroy-on-close width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 添加模拟数据 </span>
				</div>
			</template>
			<el-form ref="ruleFormRef" :model="addData" label-width="auto">
				<el-form-item label="商品名称" prop="description" :rules="[{ required: true, message: '商品名称不能为空', trigger: 'blur' }]">
					<el-input v-model="addData.description" placeholder="商品名称" clearable />
				</el-form-item>
				<el-form-item label="金额(分)" prop="total" :rules="[{ required: true, message: '金额(分)不能为空', trigger: 'blur' }]">
					<el-input v-model="addData.total" placeholder="填数字,单位是分" clearable />
				</el-form-item>
				<el-form-item label="附加信息">
					<el-input v-model="addData.attachment" clearable />
				</el-form-item>
				<el-form-item label="GoodsTag">
					<el-input v-model="addData.goodsTag" clearable />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleCloseFilled" @click="closeAddDialog">取 消</el-button>
					<el-button type="primary" icon="ele-CircleCheckFilled" @click="saveData">确 定</el-button>
				</span>
			</template>
		</el-dialog>

		<el-dialog v-model="showQrDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 付款二维码 </span>
				</div>
			</template>
			<div ref="qrDiv"></div>
		</el-dialog>

		<el-dialog v-model="showRefundDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 退款信息 </span>
				</div>
			</template>
			<el-table :data="subTableData" style="width: 100%" tooltip-effect="light" row-key="id" border>
				<el-table-column type="index" label="序号" width="65" align="center" />
				<el-table-column prop="outRefundNumber" label="商户退款号" width="210"></el-table-column>
				<el-table-column prop="transactionId" label="支付订单号" width="220"></el-table-column>
				<el-table-column prop="refund" label="金额(分)" width="70"></el-table-column>
				<el-table-column prop="reason" label="退款原因" width="180"></el-table-column>
				<el-table-column prop="refundStatus" label="状态" width="70">
					<template #default="scope">
						<el-tag v-if="scope.row.refundStatus == 'SUCCESS'" type="success"> 完成 </el-tag>
						<el-tag v-else-if="scope.row.refundStatus == 'REFUND'" type="danger"> 退款 </el-tag>
						<el-tag v-else type="info"> 未完成 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="备注" width="180"></el-table-column>
				<el-table-column prop="createTime" label="创建时间" width="150"></el-table-column>
				<el-table-column prop="successTime" label="完成时间" width="150"></el-table-column>
			</el-table>
		</el-dialog>
	</div>
</template>

<!-- 微信支付 -->
<script setup lang="ts" name="sysWechatPay">
import { ref, nextTick, onMounted, reactive } from 'vue';
import { ElMessageBox, ElMessage, ElForm } from 'element-plus';
import QRCode from 'qrcodejs2-fixes';

import { getAPI } from '/@/utils/axios-utils';
import { SysWechatPayApi } from '/@/api-services/system/api';
import { SysWechatPay } from '/@/api-services/system/models';

const qrDiv = ref<HTMLElement | null>(null);
const showAddDialog = ref(false);
const showQrDialog = ref(false);
const showRefundDialog = ref(false);

const subTableData = ref<any>([]);
const addData = ref<any>({});
const ruleFormRef = ref<InstanceType<typeof ElForm>>();

const state = reactive({
	loading: false,
	tableData: [] as Array<SysWechatPay>,
	queryParams: {
		keyword: undefined,
		createTimeRange: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
});

// 页面初始化
onMounted(async () => {
	await handleQuery();
});

// 查询操作
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysWechatPayApi).apiSysWechatPayPagePost(params);
	let tmpRows = res.data.result?.items ?? [];
	state.tableData = tmpRows;
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams.keyword = undefined;
	state.queryParams.createTimeRange = undefined;
	await handleQuery();
};

// 打开新增页面
const openAddDialog = () => {
	addData.value = {
		description: null,
		total: null,
		attachment: null,
		goodsTag: null,
	};
	showAddDialog.value = true;
};

// 关闭新增页面
const closeAddDialog = () => {
	showAddDialog.value = false;
};

// 打开扫码页面
const openQrDialog = (code: string) => {
	showQrDialog.value = true;
	nextTick(() => {
		(<HTMLElement>qrDiv.value).innerHTML = '';
		new QRCode(qrDiv.value, {
			text: code,
			width: 260,
			height: 260,
			colorDark: '#000000',
			colorLight: '#ffffff',
		});
	});
};

// 打开退款页面
const openRefundDialog = async (transactionId: string) => {
	var res = await getAPI(SysWechatPayApi).apiSysWechatPayRefundListGet(transactionId);
	let tmpRows = res.data.result ?? [];
	subTableData.value = tmpRows;
	showRefundDialog.value = true;
};

// 保存数据
const saveData = async () => {
	ruleFormRef.value?.validate(async (valid) => {
		if (valid) {
			var res = await getAPI(SysWechatPayApi).apiSysWechatPayPayTransactionNativePost(addData.value);
			closeAddDialog();
			let code = res.data.result?.qrcodeUrl;
			openQrDialog(code == undefined ? '' : code);
			await handleQuery();
		}
	});
};

// 退款
const doRefund = async (orderInfo: any) => {
	ElMessageBox.prompt(`确定进行退款：${orderInfo.total / 100}元？请输入退款理由`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
	})
		.then(async ({ value }) => {
			await getAPI(SysWechatPayApi).apiSysWechatPayRefundPost({
				outTradeNumber: orderInfo.outTradeNumber,
				reason: value,
				refund: orderInfo.total,
				total: orderInfo.total,
			});
			ElMessage.success(`【${value}】退款申请成功`);
		})
		.catch(() => {
			ElMessage.error('取消操作');
		});
};

// 刷新支付信息
const doRefreshRecord = async (orderInfo: any) => {
	await getAPI(SysWechatPayApi).apiSysWechatPayPayInfoFromWechatTradeIdGet(orderInfo.outTradeNumber);
	await handleQuery();
};

// 数字转换
const amountFormatter = (row: any, column: any, cellValue: number) => {
	return (cellValue / 100).toFixed(2);
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};
</script>
