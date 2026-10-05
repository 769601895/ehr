import {useBaseApi} from '@/api/base';

// 登录授权接口服务
export const usesysAuthApi = () => {
    const baseApi = useBaseApi("sysAuth");
    return {
        // 获取验证码
        captcha: baseApi.custom("captcha", "GET"),
        // 账号密码登录
        login: baseApi.custom("login", "POST"),
        // 手机号登录
        loginPhone: baseApi.custom("loginPhone", "POST"),
        // 退出系统
        logout: baseApi.custom("logout", "POST"),
        // Swagger登录检查
        swaggerCheckUrl: baseApi.custom("swaggerCheckUrl", "POST"),
        // Swagger登录提交
        swaggerSubmitUrl: baseApi.custom("swaggerSubmitUrl", "POST"),
        // 验证锁屏密码
        unLockScreen: baseApi.custom("unLockScreen", "POST"),
        // 获取当前登陆用户信息
        userInfo: baseApi.custom("userInfo", "GET"),
    }
}
