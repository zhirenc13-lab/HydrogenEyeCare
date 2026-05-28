# HydrogenEyeCare

氢护眼是一个 Windows 托盘护眼工具。第一版采用固定节奏：工作 20 分钟后提醒休息 20 秒；休息提醒可延迟 5 分钟，最多连续延迟 2 次。

## 技术栈

- C# / .NET 10
- WinForms
- 目标框架：`net10.0-windows`
- 发布目标：`win-x64` 自包含单文件

## 本版功能

- 启动后直接驻留系统托盘
- 单实例运行
- 20 分钟工作计时
- 20 秒极简系统风提醒窗
- 延迟 5 分钟，最多连续 2 次
- 托盘菜单：开机自启、静音、暂停/恢复计时、立即休息、退出程序
- 配置保存到 `%LOCALAPPDATA%\HydrogenEyeCare\config.json`
- 错误日志保存到 `%LOCALAPPDATA%\HydrogenEyeCare\error.log`
- 使用系统提示音作为临时休息提示音
- 界面主题：森林绿、迷雾蓝、暗岩灰
- 关于窗口显示版本、GitHub 链接、今日成功远眺次数和动态评语
- 今日远眺统计仅内存保存，程序退出或跨零点清零
- `error.log` 超过 5MB 时自动清空重写

## 开发运行

```powershell
dotnet run
```

## 构建

```powershell
dotnet build
```

## 发布单文件 exe

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o .\artifacts\publish\win-x64
```

发布产物位于：

```text
artifacts\publish\win-x64\HydrogenEyeCare.exe
```
