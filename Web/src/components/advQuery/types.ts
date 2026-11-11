/** 与后端 Filter / FilterOperatorEnum / FilterLogicEnum 对齐 */

export type AdvFilterLogic = 'And' | 'Or';
export type AdvFilterOperator =
	| 'EQ'
	| 'NEQ'
	| 'LT'
	| 'LTE'
	| 'GT'
	| 'GTE'
	| 'Contains'
	| 'NotContains'
	| 'StartsWith'
	| 'EndsWith';
export type AdvValueType = 'string' | 'number' | 'date' | 'datetime' | 'enum' | 'boolean';

export interface AdvQueryField {
	field: string;
	label: string;
	valueType: AdvValueType;
	operators?: AdvFilterOperator[];
	options?: { label: string; value: string | number | boolean }[];
	origin?: string;
}

export interface AdvFilterNode {
	/**
	 * 组：组内逻辑（旧方案）；叶子：与下一行的连接符（截图一行式：并且/或者）
	 */
	logic?: AdvFilterLogic;
	filters?: AdvFilterNode[];
	field?: string;
	operator?: AdvFilterOperator;
	value?: unknown;
	/** 前端校验：字段已从可查清单消失 */
	stale?: boolean;
}

/** 顺序对齐业务常用筛选：小等于/大等于/不等于/右包含/小于/大于/等于/包含/左包含/不包含 */
export const defaultOperators = (vt: AdvValueType): AdvFilterOperator[] => {
	if (vt === 'number' || vt === 'date' || vt === 'datetime') return ['LTE', 'GTE', 'NEQ', 'LT', 'GT', 'EQ'];
	return ['LTE', 'GTE', 'NEQ', 'EndsWith', 'LT', 'GT', 'EQ', 'Contains', 'StartsWith', 'NotContains'];
};

export const emptyGroup = (logic: AdvFilterLogic = 'And'): AdvFilterNode => ({
	logic,
	filters: [emptyLeaf()],
});

/** 空白条件行；勿带 filters，否则会被当成括号组 */
export const emptyLeaf = (): AdvFilterNode => ({
	field: undefined,
	operator: 'Contains',
	value: '',
	logic: 'And',
});

/** 打开方案时对照当前 fields 标失效 */
export const markStale = (node: AdvFilterNode | null | undefined, fieldSet: Set<string>): AdvFilterNode | null => {
	if (!node) return null;
	const next: AdvFilterNode = { ...node };
	if (next.filters?.length) {
		next.filters = next.filters.map((c) => markStale(c, fieldSet)!).filter(Boolean);
		return next;
	}
	if (next.field) {
		next.stale = !fieldSet.has(next.field);
	}
	return next;
};

export const collectStale = (node: AdvFilterNode | null | undefined, out: string[] = []): string[] => {
	if (!node) return out;
	if (node.filters?.length) {
		node.filters.forEach((c) => collectStale(c, out));
		return out;
	}
	if (node.stale && node.field) out.push(node.field);
	return out;
};

const isGroup = (n: AdvFilterNode) => !!(n.filters && n.filters.length);

/** 将「行末连接符」平铺列表折叠为后端条件树（左结合） */
const foldWithJoins = (items: { joinAfter: AdvFilterLogic; node: AdvFilterNode }[]): AdvFilterNode | null => {
	if (!items.length) return null;
	let acc = items[0].node;
	for (let i = 1; i < items.length; i++) {
		const join = items[i - 1].joinAfter || 'And';
		const next = items[i].node;
		if (acc.filters && acc.logic === join && !acc.field) {
			acc = { logic: join, filters: [...acc.filters, next] };
		} else {
			acc = { logic: join, filters: [acc, next] };
		}
	}
	return acc;
};

/** 提交前清洗：去掉 stale；叶子 logic 仅作行间连接，不传给后端叶子 */
export const sanitizeForSubmit = (node: AdvFilterNode | null | undefined): AdvFilterNode | null => {
	if (!node) return null;
	if (isGroup(node)) {
		const pairs: { joinAfter: AdvFilterLogic; node: AdvFilterNode }[] = [];
		for (const c of node.filters || []) {
			const s = sanitizeForSubmit(c);
			if (!s) continue;
			pairs.push({ joinAfter: (c.logic as AdvFilterLogic) || 'And', node: s });
		}
		if (!pairs.length) return null;
		if (pairs.length === 1) return pairs[0].node;
		return foldWithJoins(pairs);
	}
	if (!node.field || node.stale) return null;
	return {
		field: node.field,
		operator: node.operator || 'EQ',
		value: node.value ?? null,
	};
};
