# PingBox

一个基于 Avalonia UI 的跨平台程序启动器，支持多页面管理和快捷方式组织。

## 功能特性

- **多页面管理** - 支持创建多个标签页，分类管理不同的程序和文件夹
- **拖拽添加** - 支持拖拽文件和文件夹到窗口中快速添加
- **快捷方式支持** - 自动提取文件/文件夹图标，支持.lnk快捷方式
- **程序启动** - 支持启动程序、传递命令行参数、管理员权限运行
- **配置持久化** - 使用XML格式保存配置，支持自动备份
- **热键功能** - 支持全局热键显示/隐藏窗口（Windows平台）
- **外观自定义** - 支持自定义颜色、图标间距、标签位置等

## 技术栈

- **.NET 8** - 现代跨平台运行时
- **Avalonia UI 11.x** - 跨平台UI框架
- **MVVM模式** - 使用CommunityToolkit.Mvvm实现
- **C# 12** - 最新语言特性

## 项目结构

```
PingBox/
├── src/
│   └── PingBox/
│       ├── PingBox.csproj          # 项目文件
│       ├── Program.cs              # 程序入口
│       ├── App.axaml               # 应用程序定义
│       ├── App.axaml.cs
│       ├── app.manifest            # Windows清单文件
│       │
│       ├── Views/                  # 视图层
│       │   ├── MainWindow.axaml
│       │   └── MainWindow.axaml.cs
│       │
│       ├── ViewModels/             # 视图模型层
│       │   ├── ViewModelBase.cs
│       │   └── MainViewModel.cs
│       │
│       ├── Models/                 # 数据模型层
│       │   ├── AppConfig.cs        # 应用配置
│       │   ├── PageInfo.cs         # 页面信息
│       │   ├── PageItem.cs         # 页面项
│       │   └── IcoFileInfo.cs      # 文件信息
│       │
│       ├── Services/               # 服务层
│       │   ├── IConfigService.cs
│       │   ├── ConfigService.cs    # 配置读写服务
│       │   ├── IProcessService.cs
│       │   ├── ProcessService.cs   # 进程启动服务
│       │   ├── IIconService.cs
│       │   ├── IconService.cs      # 图标提取服务
│       │   ├── IHotkeyService.cs
│       │   └── HotkeyService.cs    # 热键服务
│       │
│       ├── Helpers/                # 辅助类
│       │   └── RelayCommand.cs     # MVVM命令
│       │
│       └── Assets/                 # 资源文件
│           └── pingbox.ico
│
├── logo/                           # Logo文件
├── PingBox.sln                     # 解决方案文件
└── README.md
```

## 构建要求

- .NET 8 SDK 或更高版本
- Avalonia 11.x

## 构建方法

```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行项目
dotnet run --project src/PingBox/PingBox.csproj

# 发布应用
dotnet publish -c Release -o ./publish
```

## 配置文件

配置文件保存在程序同目录下的 `PingBox.xml` 文件中，包含以下设置：

- 窗口位置和大小
- 页面列表和快捷方式
- 外观设置（颜色、间距等）
- 热键配置

## 快捷键

- **Ctrl+F2** - 默认热键，用于显示/隐藏窗口（可自定义）

## 许可证

本软件为免费开源软件。

## 致谢

感谢所有贡献者和用户！