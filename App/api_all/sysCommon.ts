import {useBaseApi} from '@/api/base';

// 通用接口接口服务
export const usesysCommonApi = () => {
    const baseApi = useBaseApi("sysCommon");
    return {
        // 获取所有接口/动态API 【兼容旧UI】
        apiList: baseApi.custom("apiList", "GET"),
        // 下载标记错误的临时 Excel
        downloadErrorExcelTemp: baseApi.custom("downloadErrorExcelTemp", "POST"),
        // 生成所有移动端接口文件
        generateAllApi: baseApi.custom("generateAllApi", "GET"),
        // 生成所有移动端接口文件
        generateAppApi: baseApi.custom("generateAppApi", "GET"),
        // 获取机器序列号
        machineSerialKey: baseApi.custom("machineSerialKey", "GET"),
        // 获取MD5加密字符串
        mD5Encrypt: baseApi.custom("mD5Encrypt", "GET"),
        // 获取文本简称
        nameAbbr: baseApi.custom("nameAbbr", "POST"),
        // 国密SM2解密字符串
        sM2Decrypt: baseApi.custom("sM2Decrypt", "POST"),
        // 国密SM2加密字符串
        sM2Encrypt: baseApi.custom("sM2Encrypt", "POST"),
        // 获取国密公钥私钥对
        smKeyPair: baseApi.custom("smKeyPair", "GET"),
        // 性能压力测试
        stressTest: baseApi.custom("stressTest", "POST"),
        // 获取所有接口/动态API
        sysAllApiList: baseApi.custom("sysAllApiList", "GET"),
        // 获取所有接口根限
        systemPermissions: baseApi.custom("systemPermissions", "GET"),
    }
}
