import {useBaseApi} from '@/api/base'

// appAuth 接口服务
export const useappAuthApi = () => {
    const baseApi = useBaseApi('appAuth')
    return {
        // 修改密码
        changePwd: baseApi.custom('changePwd', 'POST'),
    }
}
