<template>
	<div class="reportCustomQuery-container">
		<splitpanes class="default-theme" horizontal>
			<pane size="40">
				<el-card shadow="hover">
					<el-row :gutter="10" style="text-align: center">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
							<el-divider>
								显示列表配置
								<span style="margin-left: 15px">
									<el-button type="primary" icon="ele-Plus" circle @click="addDbConfig"></el-button>
									<el-button type="danger" icon="ele-Delete" circle @click="clearDbConfig"></el-button>
								</span>
							</el-divider>
							<div class="config-container">
								<div v-for="(selectConfig, index) in state.dbConfigs" :key="index" class="config-item-db">
									<div class="config-item-left" style="flex: 1">
										<span>表名</span>
										<el-select v-model="selectConfig.selectedTables" filterable @change="(val: string) => onTableChange(val, selectConfig)">
											<el-option v-for="item in state.dbTables" :key="item.name" :label="`${item.description ? item.description : ''}[${item.name}]`" :value="item.name" />
										</el-select>
									</div>
									<div class="config-item-right" style="flex: 1">
										<span>列名</span>
										<el-select v-model="selectConfig.selectedColumns" multiple filterable @change="(val: string[]) => onColumnChange(val, selectConfig)">
											<el-option
												v-for="column in selectConfig.dbColumns"
												:key="column.dbColumnName"
												:label="`${column.tableDescription ? column.tableDescription : ''}.${column.columnDescription ? column.columnDescription : ''}[${column.tableName}.${column.dbColumnName}]`"
												:value="`${column.tableName}.${column.dbColumnName}`"
											/>
										</el-select>
									</div>
									<!-- 删除按钮 -->
									<div class="config-item-button">
										<el-button type="danger" icon="ele-CloseBold" plain circle @click="removeDbConfig(index)"></el-button>
									</div>
								</div>
							</div>
						</el-col>

						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
							<el-row :gutter="10" style="text-align: center">
								<!-- 连接关系配置 -->
								<el-divider>
									连接关系配置
									<span style="margin-left: 15px">
										<el-button type="primary" icon="ele-Plus" circle @click="addJoinConfig"></el-button>
										<el-button type="danger" icon="ele-Delete" circle @click="clearJoinConfig"></el-button>
									</span>
								</el-divider>
								<div class="config-container">
									<div v-for="(joinConfig, index) in state.joinConfigs" :key="index" class="config-item-join">
										<div class="config-item-left">
											<!-- 连接关系选择项 -->
											<el-select v-model="joinConfig.joinType" style="width: 100px">
												<el-option label="左连接" value="LEFT JOIN" />
												<el-option label="右连接" value="RIGHT JOIN" />
												<el-option label="内连接" value="INNER JOIN" />
											</el-select>
											<!-- 左表选择项 -->
											<el-select v-model="joinConfig.leftTable" filterable style="flex: 1; margin-left: 10px">
												<el-option v-for="item in state.dbTables" :key="item.name" :label="`${item.description ? item.description : ''}[${item.name}]`" :value="item.name" />
											</el-select>
											<!-- 右表选择项 -->
											<el-select v-model="joinConfig.rightTable" filterable style="flex: 1; margin-left: 10px">
												<el-option v-for="item in state.dbTables" :key="item.name" :label="`${item.description ? item.description : ''}[${item.name}]`" :value="item.name" />
											</el-select>
											<!-- 操作按钮：增加和删除 -->
											<el-button-group style="margin-left: 10px">
												<el-button type="primary" icon="ele-Plus" @click="addJoinTable(index)"></el-button>
												<el-button type="danger" icon="ele-Close" @click="delJoinTable(index)"></el-button>
											</el-button-group>
										</div>
										<div class="config-item-right" v-if="joinConfig.tableColumns.length > 0">
											<div v-for="(item, kindex) in joinConfig.tableColumns" :key="kindex" class="config-item-right-item">
												<!-- 左表字段下拉框 -->
												<el-select v-model="item.leftTableColumn" filterable @visible-change="(visible: boolean) => visible && getJoinTableColumns(joinConfig.leftTable, true, index)">
													<el-option
														v-for="column in joinConfig.leftTableColumns"
														:key="column.dbColumnName"
														:label="`${column.columnDescription ? column.columnDescription : ''}[${column.tableName}.${column.dbColumnName}]`"
														:value="`${column.dbColumnName}`"
													/>
												</el-select>
												<!-- 右表字段下拉框 -->
												<el-select
													v-model="item.rightTableColumn"
													filterable
													style="margin-left: 10px"
													@visible-change="(visible: boolean) => visible && getJoinTableColumns(joinConfig.rightTable, false, index)"
												>
													<el-option
														v-for="column in joinConfig.rightTableColumns"
														:key="column.dbColumnName"
														:label="`${column.columnDescription ? column.columnDescription : ''}[${column.tableName}.${column.dbColumnName}]`"
														:value="`${column.dbColumnName}`"
													/>
												</el-select>
												<el-button type="danger" icon="ele-CloseBold" plain circle @click="removeJoinColumn(index, kindex)" style="margin-left: 10px" />
											</div>
										</div>
									</div>
								</div>
							</el-row>
						</el-col>
					</el-row>

					<el-row :gutter="10" style="text-align: center">
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
							<!-- 查询条件配置 -->
							<el-divider>
								查询条件配置
								<span style="margin-left: 15px">
									<el-button type="primary" icon="ele-Plus" circle @click="addWhereConfig"></el-button>
									<el-button type="danger" icon="ele-Delete" circle @click="clearWhereConfig"></el-button>
								</span>
							</el-divider>
							<div class="config-container">
								<div v-for="(condition, index) in state.whereConfigs" :key="index" class="config-item">
									<!-- 逻辑操作符选择项 (隐藏第一个条件的逻辑操作符) -->
									<el-select v-model="condition.joinType" style="width: 200px">
										<el-option label="且" value="AND" />
										<el-option label="或" value="OR" />
									</el-select>
									<!-- 表选择项 -->
									<el-select v-model="condition.table" filterable @change="changeWhereTable(condition)" style="width: 400px">
										<el-option v-for="item in state.dbTables" :key="item.name" :label="`${item.description ? item.description : ''}[${item.name}]`" :value="item.name" />
									</el-select>
									<!-- 列选择项 -->
									<el-select v-model="condition.column" filterable @change="changeWhereColumn(condition)" style="width: 400px">
										<el-option
											v-for="column in condition.columns"
											:key="column.dbColumnName"
											:label="`${column.columnDescription ? column.columnDescription : ''}[${column.dbColumnName}]`"
											:value="`${column.tableName}.${column.dbColumnName}`"
										/>
									</el-select>
									<!-- 操作符选择项 -->
									<el-select v-model="condition.operator" style="width: 250px">
										<el-option label="等于" value="=" />
										<el-option label="不等于" value="!=" />
										<el-option label="大于" value=">" />
										<el-option label="小于" value="<" />
										<el-option label="大于等于" value=">=" />
										<el-option label="小于等于" value="<=" />
										<el-option label="包含" value="LIKE" />
									</el-select>
									<!-- 值填写项 -->
									<el-input-number v-if="condition.dataType == 'bigint' || condition.dataType == 'int8'" v-model="condition.value" controls-position="right" class="w100" />
									<el-date-picker
										v-else-if="condition.dataType == 'datetime' || condition.dataType == 'timestamp'"
										v-model="condition.value"
										type="datetime"
										format="YYYY-MM-DD HH:mm:ss"
										value-format="YYYY-MM-DD HH:mm:ss"
										class="w100"
									/>
									<el-input v-else v-model="condition.value" />
									<!-- 删除按钮 -->
									<el-button type="danger" icon="ele-CloseBold" plain circle @click="removeWhereConfig(index)"></el-button>
								</div>
							</div>
						</el-col>

						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
							<!-- 数据排序配置 -->
							<el-divider>
								数据排序配置
								<span style="margin-left: 15px">
									<el-button type="primary" icon="ele-Plus" circle @click="addSortConfig"></el-button>
									<el-button type="danger" icon="ele-Delete" circle @click="clearSortConfig"></el-button>
								</span>
							</el-divider>
							<div class="config-container">
								<div v-for="(sortCondition, index) in state.orderConfigs" :key="index" class="config-item">
									<!-- 表选择项 -->
									<el-select v-model="sortCondition.table" filterable @change="changeWhereTable(sortCondition)">
										<el-option v-for="item in state.dbTables" :key="item.name" :label="`${item.description ? item.description : ''}[${item.name}]`" :value="item.name" />
									</el-select>
									<!-- 列选择项 -->
									<el-select v-model="sortCondition.column" filterable>
										<el-option
											v-for="column in sortCondition.columns"
											:key="column.dbColumnName"
											:label="`${column.columnDescription ? column.columnDescription : ''}[${column.dbColumnName}]`"
											:value="`${column.tableName}.${column.dbColumnName}`"
										/>
									</el-select>
									<!-- 排序序号 -->
									<el-input-number v-model="sortCondition.sort" style="width: 180px" controls-position="right" />
									<!-- 删除按钮 -->
									<el-button type="danger" icon="ele-CloseBold" plain circle @click="removeSortConfig(index)"></el-button>
								</div>
							</div>
						</el-col>
					</el-row>

					<!-- 操作按钮 -->
					<el-row :gutter="10" style="text-align: center">
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
							<el-button icon="ele-View" type="primary" plain @click="generateSQL">预览SQL</el-button>
							<el-button icon="ele-Position" type="primary" @click="executeSQL">执行查询</el-button>
							<el-button icon="ele-FolderOpened" @click="openSchema">打开方案</el-button>
							<el-button icon="ele-DocumentCopy" @click="saveSchema">保存方案</el-button>
						</el-col>
					</el-row>

					<!-- SQL语句预览 -->
					<el-row :gutter="10">
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="">
							<el-input v-model="state.sqlString" type="textarea" :rows="5" disabled style="color: red" />
						</el-col>
					</el-row>
				</el-card>
			</pane>

			<pane size="60">
				<!-- 查询结果表格显示 -->
				<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options">
					<template #toolbar_buttons>
						<el-icon size="20" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Tickets /> </el-icon>
						<span> 查询结果 </span>
					</template>
					<template #toolbar_tools> </template>
					<template #empty>
						<el-empty :image-size="200" />
					</template>
				</vxe-grid>
			</pane>
		</splitpanes>
	</div>
</template>

<!-- 报表自定义查询 -->
<script lang="ts" setup name="sysReportCustomQuery">
import { reactive, onMounted, computed, ref, watch } from 'vue';
import { ElMessage } from 'element-plus';
// import { format } from 'sql-formatter';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { VxeGridInstance } from 'vxe-table';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/system/api';
import { DbColumnOutput, DbTableInfo } from '/@/api-services/system/models';

const state = reactive({
	// 数据库表和列集合
	dbTables: [] as Array<DbTableInfo>,
	// 数据显示列配置
	dbConfigs: [] as Array<{
		selectedTables: string;
		selectedColumns: any[];
		dbColumns: any[];
	}>,

	selectConfigs: [] as any,
	// 连接关系配置
	joinConfigs: [] as Array<{
		joinType: 'LEFT JOIN'; // 左连接LEFTJOIN、右连接RIGHTJOIN、内连接INNERJOIN
		leftTable: ''; // 左表名称
		rightTable: ''; // 右表名称
		tableColumns: Array<{ leftTableColumn: string; rightTableColumn: string }>; // 左右表列集合（一一对应）
		leftTableColumns: any[];
		rightTableColumns: any[];
	}>,
	// 查询条件配置
	whereConfigs: [] as Array<{
		table: string;
		column: string;
		operator: string; // 操作符
		value: string; // 值
		columns: any;
		dataType: string; // 列数据类型
		joinType: string; // 连接方式
	}>,
	// 数据排序配置
	orderConfigs: [] as Array<{
		table: string;
		column: string;
		sort: number; // 排序（从小到大）
		columns: any;
	}>,
	// SQL语句
	sqlString: 'select * from xxx',
	// 查询结果
	queryResult: [] as any,
	// 主库标识
	mainConfigId: '1300000000001',
});

// 获取xGrid实例引用
const xGrid = ref<VxeGridInstance>();

// 创建响应式的列配置
const dynamicColumns = computed(() => {
	// 基础列 - 序号
	const columns: any = [
		{
			field: 'seq',
			type: 'seq',
			title: '序号',
			width: 60,
			fixed: 'left',
		},
	];
	// 根据selectConfigs动态添加列
	if (state.selectConfigs && state.selectConfigs.length > 0) {
		state.selectConfigs.forEach((config: any) => {
			// 确保配置中包含必要的字段
			if (config.dbColumnName) {
				columns.push({
					field: config.dbColumnName.substring(0, 1).toLowerCase() + (config.dbColumnName?.substring(1) ?? ''), //首字母小写,可根据后端返回的JSON格式修改
					title: config.tableDescription ? `${config.tableDescription}.${config.columnDescription}` : config.dbColumnName,
					showOverflow: 'tooltip',
				});
			}
		});
	}
	return columns;
});

// 监听selectConfigs变化，更新表格列配置
watch(
	() => state.selectConfigs,
	() => {
		// 更新列配置
		if (xGrid.value) {
			xGrid.value.loadColumn(dynamicColumns.value);
		}
	},
	{ deep: true }
);

// 表格参数配置
const options = useVxeTable<DbColumnOutput>(
	{
		id: 'sysCustomQuery',
		name: '自定义查询',
		columns: dynamicColumns.value as any,
	},
	{
		// 分页配置
		pagerConfig: { enabled: false },
		// 工具栏配置
		toolbarConfig: { export: true },
	}
);

// 页面初始化
onMounted(async () => {
	// 查询表列表
	const res: any = await getAPI(SysDatabaseApi).apiSysDatabaseTableListConfigIdGet(state.mainConfigId);
	res.data.result.forEach((element: any) => {
		// 排除zero_开头的表
		if (!element.name.startsWith('zero_')) {
			state.dbTables.push(element);
		}
	});
	updateSelectConfigs();
});

// 当选择表变化时，加载对应的列
const onTableChange = async (value: string, selectConfig: { selectedTables: string; selectedColumns: any[]; dbColumns: any[] }) => {
	selectConfig.dbColumns = [];
	if (!selectConfig.selectedTables) return;

	// 获取当前选择表的详细项
	const tableInfo = state.dbTables.find((table) => table.name === value);
	const res: any = await getAPI(SysDatabaseApi).apiSysDatabaseColumnListTableNameConfigIdGet(value, state.mainConfigId);
	if (res.data.result && Array.isArray(res.data.result)) {
		res.data.result.forEach((newColumn: any) => {
			const columnWithTableDesc = {
				...newColumn,
				tableDescription: tableInfo ? tableInfo.description : '', // 添加表描述
			};
			selectConfig.dbColumns.push(columnWithTableDesc);
		});
	}
};

// 当列选择变化时，实现勾选与列表的关联
const onColumnChange = (val: string[], selectConfig: { selectedTables: string; selectedColumns: any[]; dbColumns: any[] }) => {
	state.sqlString = '';
	updateSelectConfigs();
};

const updateSelectConfigs = () => {
	// 先清空现有数据，避免重复添加
	state.selectConfigs = [];

	// 遍历所有数据库配置
	state.dbConfigs.forEach((config) => {
		config.selectedColumns.forEach((colFullName: string) => {
			// 解析列名，格式为"表名.列名"
			const [tableName, dbColumnName] = colFullName.split('.');
			// 如果解析成功，在当前config的dbColumns中查找匹配的项
			if (tableName && dbColumnName) {
				const matchedColumn = config.dbColumns.find((column: any) => column.tableName === tableName && column.dbColumnName === dbColumnName);
				if (matchedColumn) {
					state.selectConfigs.push(matchedColumn);
				}
			}
		});
	});
};

// 添加数据显示列配置
const addDbConfig = () => {
	state.dbConfigs.push({
		selectedTables: '',
		selectedColumns: [],
		dbColumns: [],
	});
	updateSelectConfigs();
};

// 删除数据显示列配置
const removeDbConfig = (index: number) => {
	state.dbConfigs.splice(index, 1);
	updateSelectConfigs();
};

// 清除数据显示列配置
const clearDbConfig = () => {
	state.dbConfigs = [];
	state.selectConfigs = [];
	updateSelectConfigs();
};

// 添加连接关系配置
const addJoinConfig = () => {
	state.joinConfigs.push({
		joinType: 'LEFT JOIN',
		leftTable: '',
		rightTable: '',
		tableColumns: [
			{
				leftTableColumn: '',
				rightTableColumn: '',
			},
		],
		leftTableColumns: [],
		rightTableColumns: [],
	});
};

// 清除连接关系配置
const clearJoinConfig = () => {
	state.joinConfigs = [];
};

// 添加连接关系配置对应表
const addJoinTable = (index: number) => {
	state.joinConfigs[index].tableColumns.push({
		leftTableColumn: '',
		rightTableColumn: '',
	});
};

// 删除连接关系配置对应表
const delJoinTable = (index: number) => {
	state.joinConfigs.splice(index, 1);
};

// 删除连接关系配置对应列
const removeJoinColumn = (index: number, keyIndex: number) => {
	state.joinConfigs[index].tableColumns.splice(keyIndex, 1);
};

// 获取连接关系配置表列数据
const getJoinTableColumns = async (tableName: string, isLeftTable: boolean = true, joinIndex: number) => {
	if (tableName) {
		try {
			const res: any = await getAPI(SysDatabaseApi).apiSysDatabaseColumnListTableNameConfigIdGet(tableName, state.mainConfigId);
			const columns = res?.data?.result || [];

			// 根据isLeftTable参数决定更新哪个表的字段
			if (isLeftTable) {
				state.joinConfigs[joinIndex].leftTableColumns = columns;
			} else {
				state.joinConfigs[joinIndex].rightTableColumns = columns;
			}

			return columns;
		} catch (error) {
			console.error('获取表字段失败:', error);
			return [];
		}
	}
	return [];
};

// 改变查询条件表时
const changeWhereTable = async (condition: any) => {
	const res: any = await getAPI(SysDatabaseApi).apiSysDatabaseColumnListTableNameConfigIdGet(condition.table, state.mainConfigId);
	condition.columns = res.data.result;
	condition.column = undefined;
	condition.value = undefined;
};

// 改变查询条件列时
const changeWhereColumn = async (condition: any) => {
	// 获取当前列数据类型
	var column = condition.columns.find((column: any) => column.tableName + '.' + column.dbColumnName == condition.column);
	if (column) {
		condition.dataType = column.dataType;
		condition.value = undefined;
	}
};

// 添加查询条件配置
const addWhereConfig = () => {
	state.whereConfigs.push({
		table: '',
		column: '',
		operator: '=',
		value: '',
		columns: [],
		dataType: '',
		joinType: 'AND',
	});
};

// 移除查询条件配置
const removeWhereConfig = (index: number) => {
	state.whereConfigs.splice(index, 1);
};

// 清除所有查询条件配置
const clearWhereConfig = () => {
	state.whereConfigs = [];
};

// 添加排序配置
const addSortConfig = () => {
	state.orderConfigs.push({
		table: '',
		column: '',
		sort: 10,
		columns: [],
	});
};

// 移除排序配置
const removeSortConfig = (index: number) => {
	state.orderConfigs.splice(index, 1);
};

// 清除所有排序配置
const clearSortConfig = () => {
	state.orderConfigs = [];
};

// 预览SQL语句
const generateSQL = async () => {
	var queryJson = {
		selectConfigs: state.selectConfigs
			.filter((config: any) => state.dbConfigs.some((dbConfig: any) => dbConfig.selectedTables.includes(config.tableName))) // 只包含选中表的列
			.map((config: any) => ({
				table: config.tableName,
				column: config.dbColumnName,
			})),
		joinConfigs: state.joinConfigs
			.filter((config: any) => config.leftTable != undefined && config.rightTable != undefined && config.tableColumns != undefined)
			.map((config) => {
				return {
					joinType: config.joinType,
					leftTable: config.leftTable,
					rightTable: config.rightTable,
					tableColumns: config.tableColumns,
				};
			}),
		whereConfigs: state.whereConfigs
			.filter((config: any) => config.table != undefined && config.column != undefined && config.value != undefined)
			.map((config) => {
				return {
					table: config.table,
					column: config.column.includes('.') ? config.column.split('.')[1] : config.column,
					operator: config.operator,
					value: config.value,
					dataType: config.dataType,
					joinType: config.joinType,
				};
			}),
		orderConfigs: state.orderConfigs
			.filter((config: any) => config.table != undefined && config.column != undefined)
			.map((config) => {
				return {
					table: config.table,
					column: config.column.includes('.') ? config.column.split('.')[1] : config.column,
					sort: config.sort,
				};
			}),
	};
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseGenerateSQLPost(queryJson);
	state.sqlString = res.data.result ?? '';
	return queryJson;

	// // 格式化SQL语句
	// state.sqlString = format(state.sqlString, {
	// 	tabWidth: 4,
	// 	useTabs: false,
	// 	keywordCase: 'upper',
	// 	dataTypeCase: 'upper',
	// 	functionCase: 'upper',
	// 	logicalOperatorNewline: 'before',
	// 	language: 'transactsql',
	// });
};

// 执行SQL
const executeSQL = async () => {
	var queryJson = await generateSQL();
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseExecuteSQLPost(queryJson);
	state.queryResult = res.data.result ?? [];

	// 更新表格数据
	xGrid.value?.loadData(state.queryResult);
};

// 打开方案
const openSchema = () => {
	// 创建文件选择器
	const input = document.createElement('input');
	input.type = 'file';
	input.accept = '.json';

	input.onchange = (event) => {
		const file = (event.target as HTMLInputElement).files?.[0];
		if (!file) return;

		const reader = new FileReader();
		reader.onload = async (e) => {
			try {
				// 解析JSON文件内容
				const schemaData = JSON.parse(e.target?.result as string);
				if (schemaData.dbConfigs) {
					state.dbConfigs = schemaData.dbConfigs;
				}
				if (schemaData.selectConfigs) {
					state.selectConfigs = schemaData.selectConfigs;
				}
				if (schemaData.whereConfigs) {
					state.whereConfigs = schemaData.whereConfigs;
				}
				if (schemaData.joinConfigs) {
					state.joinConfigs = schemaData.joinConfigs;
				}
				if (schemaData.orderConfigs) {
					state.orderConfigs = schemaData.orderConfigs;
				}

				ElMessage.success('自定义查询方案加载成功');
			} catch (error) {
				ElMessage.error('自定义查询方案文件格式错误');
			}
		};

		reader.readAsText(file);
	};

	// 触发文件选择对话框
	input.click();
};

// 保存方案
const saveSchema = () => {
	if (state.whereConfigs.length === 0) {
		ElMessage.warning('请先进行配置');
		return;
	}
	// 创建要保存的查询配置对象
	const schemaData = {
		dbConfigs: state.dbConfigs, // 数据库配置
		selectConfigs: state.selectConfigs, // 选中的列的详细信息
		whereConfigs: state.whereConfigs, // 查询条件
		orderConfigs: state.orderConfigs, // 排序条件
		joinConfigs: state.joinConfigs, // 关联键配置
	};

	// 转换为JSON字符串
	const jsonString = JSON.stringify(schemaData, null, 2);

	// 创建Blob对象
	const blob = new Blob([jsonString], { type: 'application/json' });

	// 创建下载链接
	const url = URL.createObjectURL(blob);
	const a = document.createElement('a');
	a.href = url;
	// 设置文件名，使用表名和当前时间作为标识
	const timestamp = new Date().toLocaleString('zh-CN').replace(/[\/\s:]/g, '-');
	a.download = `自定义查询方案_${timestamp}.json`;

	// 触发下载
	document.body.appendChild(a);
	a.click();

	// 清理
	document.body.removeChild(a);
	URL.revokeObjectURL(url);
};
</script>

<style lang="scss" scoped>
.config-container {
	width: 100%;
	padding: 0 10px 10px 10px;
}
:deep(.el-card) {
	height: 100%;
	.el-card__body {
		height: 100%;
		overflow-y: auto;
	}
}
.config-item {
	display: flex;
	align-items: center;
	gap: 10px;
	margin-bottom: 10px;
}
.config-item-join {
	display: flex;
	flex-direction: column;
	border-bottom: 1px dashed #c0c4cc;
	padding: 0 5px;
	margin-bottom: 10px;
	.config-item-left {
		display: flex;
		margin-bottom: 10px;
	}
	// .config-item-right {
	// 	width: 100%;
	// }
	.config-item-right-item {
		display: flex;
		margin-bottom: 5px;
	}
}
.config-item-db {
	display: flex;
	// align-items: center;
	.config-item-left {
		display: flex;
		margin-bottom: 10px;
		align-items: center;
		:deep(.el-select) {
			width: calc(100% - 45px);
			margin-left: 5px;
		}
	}
	.config-item-right {
		display: flex;
		margin-bottom: 10px;
		align-items: center;
		:deep(.el-select) {
			width: calc(100% - 45px);
			margin-left: 5px;
		}
	}
	.config-item-button {
		display: flex;
		margin-bottom: 10px;
		align-items: center;
	}
}
.config-item-db:last-child {
	border-bottom: none;
	margin-bottom: 0;
}
.config-item-join:last-child {
	border-bottom: none;
	margin-bottom: 0;
}
</style>
