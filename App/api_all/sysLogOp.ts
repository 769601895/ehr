import {useBaseApi} from '@/api/base';

// 操作日志接口服务
export const usesysLogOpApi = () => {
    const baseApi = useBaseApi("sysLogOp");
    return {
        // 清空操作日志
        clear: baseApi.custom("clear", "POST"),
        // 获取操作日志详情
        detail: baseApi.detail,
        // 导出操作日志
        export: baseApi.custom("export", "POST"),
        // 获取操作日志分页列表
        page: baseApi.page,
        // 按年按天数统计消息日志
        yearDayStats: baseApi.custom("yearDayStats", "GET"),
    }
}
