# 二代证读卡本机代理（公安部标准 sdtapi）

供人员资料页「读取身份证」调用。仅监听 `127.0.0.1`，通过 `sdtapi.dll` 读芯片。

## 运行

1. 将厂商提供的 **`sdtapi.dll`**（及依赖如 `WltRS.dll` 等）放到本程序输出目录，或在 `appsettings.json` 配置 `IdCard:DllDirectory`。
2. 安装 [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)（若未装 SDK）。
3. 启动：

```bat
cd Admin.NET\tools\IdCardReaderAgent
dotnet run
```

或发布后双击：

```bat
dotnet publish -c Release -r win-x86 --self-contained false
```

默认地址：`http://127.0.0.1:19196`

## 接口

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/health` | 探活 |
| POST/GET | `/idcard/read` | 读卡，返回 JSON |

## 配置（appsettings.json）

```json
{
  "Urls": "http://127.0.0.1:19196",
  "IdCard": {
    "CommPort": 1001,
    "DllDirectory": "C:\\Reader\\dll",
    "Mock": false
  }
}
```

- `CommPort`：USB 常见 `1001`
- `Mock: true`：无硬件时返回模拟数据（仅联调前端）

## 与 EHR 的关系

- **是否允许读卡**：在系统「公司参数设置 → 参数明细 → 启用二代身份证」配置，前端按部门自动取参。
- 浏览器固定访问本机 `http://127.0.0.1:19196`，无需再配前端 JSON。

## 注意

- 需 **Windows** + 已驱动读卡器。
- 部分 DLL 为 x86，若加载失败请用 `win-x86` 发布或换对应位数 DLL。
- 浏览器跨域已放开（仅本机服务）；勿把 Agent 绑到非本机网卡。
