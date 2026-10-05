import type { FormInstance } from 'element-plus';
import { ElMessage } from 'element-plus';

/**
 * 表单验证并自动滚动到第一个错误字段
 * @param formRef - Element Plus Form 实例的 ref
 * @param options - 配置选项
 * @returns Promise<boolean> - 验证是否通过
 */
export const validateFormWithScroll = async (
	formRef: FormInstance | undefined,
	options?: {
		showMessage?: boolean; // 是否显示错误提示，默认 true
		message?: string; // 自定义错误提示信息
	}
): Promise<boolean> => {
	if (!formRef) {
		console.warn('Form ref is undefined');
		return false;
	}

	const { showMessage = true, message = '请检查表单填写是否正确' } = options || {};

	try {
		await formRef.validate();
		return true;
	} catch (invalidFields: any) {
		// 获取第一个错误字段的 prop
		if (invalidFields && typeof invalidFields === 'object') {
			const firstErrorField = Object.keys(invalidFields)[0];
			if (firstErrorField) {
				// 滚动到第一个错误字段
				formRef.scrollToField(firstErrorField);
			}
		}

		if (showMessage) {
			ElMessage.warning(message);
		}

		return false;
	}
};

/**
 * 表单验证并自动滚动到第一个错误字段（带回调方式）
 * @param formRef - Element Plus Form 实例的 ref
 * @param onSuccess - 验证成功后的回调
 * @param options - 配置选项
 */
export const validateFormWithScrollCallback = async (
	formRef: FormInstance | undefined,
	onSuccess: () => void | Promise<void>,
	options?: {
		showMessage?: boolean;
		message?: string;
	}
): Promise<void> => {
	const isValid = await validateFormWithScroll(formRef, options);
	if (isValid) {
		await onSuccess();
	}
};
