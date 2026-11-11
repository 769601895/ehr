/**
 * 高级查询通用控件（列表页复用）
 *
 * 用法概要：
 * 1. useAdvQuery(moduleCode) 管理 filter / fields / 打开关闭
 * 2. 模板挂载 <AdvQueryDrawer :module-code :fields @search @update:active-count />
 * 3. 分页请求把 filter 传给后端（BasePageInput.Filter）
 * 4. 后端按模块 FieldRegistry + HrAdvancedFilter（或同类白名单 Apply）消毒执行
 *
 * 规范：.cursor/rules/adv-query-required.mdc
 */
export { default as AdvQueryDrawer } from './advQueryDrawer.vue';
export { useAdvQuery } from './useAdvQuery';
export type { AdvFilterLogic, AdvFilterNode, AdvFilterOperator, AdvQueryField, AdvValueType } from './types';
export {
	collectStale,
	defaultOperators,
	emptyGroup,
	emptyLeaf,
	markStale,
	sanitizeForSubmit,
} from './types';
