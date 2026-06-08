# 氢护眼 HydrogenEyeCare

氢护眼是一个轻量的 Windows 托盘护眼提醒工具。它基于常见的 **20-20-20** 护眼原则：长时间使用屏幕时，每工作约 20 分钟，让眼睛离开屏幕约 20 秒，望向约 20 英尺（约 6 米）外的远处。

这个工具不试图接管你的工作流，也不把护眼做成复杂的健康管理系统。它只做一件事：在合适的时间，用尽量克制的方式提醒你从屏幕里抬头。

## 产品理念

很多护眼工具的问题不是“不够强”，而是“太打扰”。氢护眼的设计取向是：

- **轻量常驻**：启动后驻留系统托盘，不显示主窗口。
- **提醒但不打断**：提醒窗口置顶但不抢焦点，保持右下角轻量出现。
- **确认而不强迫**：完成 20 秒远眺后，需要点击“完成”才计入今日成功远眺；如果没有确认，窗口会自动关闭，不伪造统计。
- **尊重用户节奏**：支持延迟提醒、静音、暂停计时和立即休息。
- **可持续使用**：界面克制、状态清楚、统计简单，不把护眼变成新的负担。

## 关于 20-20-20

数字屏幕使用时间过长时，可能带来眼疲劳、干涩、视物模糊、头痛、肩颈不适等问题。美国验光协会（AOA）对数字眼疲劳的建议中包含 20-20-20 规则：每 20 分钟休息 20 秒，看 20 英尺外的物体。

氢护眼采用的是适合日常桌面使用的本地化节奏：

- 工作 `20 分钟`
- 远眺提醒 `20 秒`
- 完成确认 `10 秒`
- 可延迟 `5 分钟`
- 最多连续延迟 `2 次`

它不是医疗诊断或治疗工具。如果你有持续眼部不适，请咨询专业眼科或视光医生。

参考：

- [American Optometric Association: Computer vision syndrome](https://www.aoa.org/healthy-eyes/eye-and-vision-conditions/computer-vision-syndrome)
- [American Optometric Association: 20-20-20 rule PDF](https://www.aoa.org/AOA/Images/Patients/Eye%20Conditions/20-20-20-rule.pdf)

## 功能特性

氢护眼会在后台计时，每工作约 20 分钟后提醒你远眺休息 20 秒。

## 数据与隐私

氢护眼是本地 Windows 桌面程序，不需要账号，也不会上传使用数据。

本地文件位置：

```text
%LOCALAPPDATA%\HydrogenEyeCare\config.json
%LOCALAPPDATA%\HydrogenEyeCare\daily-stats.json
%LOCALAPPDATA%\HydrogenEyeCare\error.log
```

说明：

- `config.json` 保存开机自启、静音等配置。
- `daily-stats.json` 保存当天成功远眺次数，程序退出后保留，跨零点清零。
- `error.log` 只在程序捕获异常时写入，用于排查问题。

## 下载与升级

请在 GitHub Releases 下载最新版 zip 包：

[HydrogenEyeCare Releases](https://github.com/zhirenc13-lab/HydrogenEyeCare/releases)

下载后解压到本地文件夹，再运行其中的程序。

升级方式：

1. 从托盘菜单退出正在运行的氢护眼。
2. 下载新版 zip 并解压。
3. 用新版解压内容覆盖旧文件夹，或放到新的目录运行。
4. 重新启动。

升级不会清空设置和今日统计，因为配置与统计保存在 `%LOCALAPPDATA%\HydrogenEyeCare`。

## 技术栈

- C# / .NET 10
- WinForms
- 目标框架：`net10.0-windows`

## 开发运行

```powershell
dotnet run
```

## 构建

```powershell
dotnet build
```

## 测试

```powershell
dotnet test .\HydrogenEyeCare.Tests\HydrogenEyeCare.Tests.csproj
```

## 许可证

MIT License
