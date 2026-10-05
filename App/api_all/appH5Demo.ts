import {useBaseApi} from '@/api/base';

// pyStatistics接口服务
export const usepyStatisticsApi = () => {
    const baseApi = useBaseApi("appH5Demo");
    return {
        // 级营情况统计
        businessStatus: baseApi.custom("businessStatus", "GET"),
    }
}
