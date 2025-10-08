# GitVer.MSBuild - 輕量級 Git 版本自動化工具

> 零配置的 Git 版本自動化 MSBuild 套件，專為台灣開發者優化

## 🚀 快速開始

### 1. 安裝套件

```bash
dotnet add package GitVer.MSBuild
```

或在 `.csproj` 中加入：

```xml
<PackageReference Include="GitVer.MSBuild" Version="1.0.0">
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

### 2. 建置專案

```bash
dotnet build
```

### 3. 使用版本信息

```csharp
using YourNamespace; // 或 using GitVer;

Console.WriteLine($"版本: {GitVersionInfo.Version}");
Console.WriteLine($"標籤: {GitVersionInfo.Tag}");
Console.WriteLine($"提交: {GitVersionInfo.Commit}");
Console.WriteLine($"建置時間: {GitVersionInfo.BuildTimestamp}");
```

## 📋 版本格式

自動根據 Git 狀態選擇最適合的版本格式：

- **完整版本**: `v1.0.0-abc12345 @ 2025-10-08T15:32:34+08:00`
- **僅標籤**: `v1.0.0 @ 2025-10-08T15:32:34+08:00`  
- **僅提交**: `abc12345 @ 2025-10-08T15:32:34+08:00`
- **後備版本**: `dev @ 2025-10-08T15:32:34+08:00`

## ⚙️ 自訂配置

在 `.csproj` 中加入屬性來自訂行為：

```xml
<PropertyGroup>
  <!-- 停用 GitVer -->
  <GitVerEnabled>false</GitVerEnabled>
  
  <!-- 自訂時區偏移 (預設: 8 = 台灣時區) -->
  <GitVerTimezone>0</GitVerTimezone>
  
  <!-- 自訂命名空間 (預設: RootNamespace) -->
  <GitVerNamespace>MyApp.Version</GitVerNamespace>
  
  <!-- 自訂類別名稱 (預設: GitVersionInfo) -->
  <GitVerClassName>AppVersion</GitVerClassName>
  
  <!-- 自訂後備版本 (預設: dev) -->
  <GitVerFallbackVersion>unknown</GitVerFallbackVersion>
</PropertyGroup>
```

## 🎯 使用場景

### 主控台應用程式

```csharp
namespace MyApp;

class Program
{
    static void Main()
    {
        Console.WriteLine($"MyApp {GitVersionInfo.Version}");
        if (args.Contains("--version"))
        {
            Console.WriteLine($"Tag: {GitVersionInfo.Tag}");
            Console.WriteLine($"Commit: {GitVersionInfo.Commit}"); 
            Console.WriteLine($"Built: {GitVersionInfo.BuildTimestamp}");
            return;
        }
        // 應用程式邏輯...
    }
}
```

### Web API

```csharp
[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    public object GetVersion()
    {
        return new
        {
            version = GitVersionInfo.Version,
            tag = GitVersionInfo.Tag,
            commit = GitVersionInfo.Commit,
            buildTime = GitVersionInfo.BuildTimestamp
        };
    }
}
```

### 記錄檔輸出

```csharp
logger.LogInformation("Application started: {Version}", GitVersionInfo.Version);
```

## 🔧 進階功能

### 條件編譯

```csharp
#if DEBUG
Console.WriteLine($"Debug Build: {GitVersionInfo.Version}");
#else
Console.WriteLine($"Release Build: {GitVersionInfo.Version}");
#endif
```

### 版本比較

```csharp
public static class VersionHelper
{
    public static bool IsDevBuild => GitVersionInfo.Version.Contains("dev");
    public static bool HasTag => !string.IsNullOrEmpty(GitVersionInfo.Tag);
    public static DateTime BuildTime => DateTime.Parse(GitVersionInfo.BuildTimestamp);
}
```

## 🆚 與其他套件比較

| 特色 | GitVer.MSBuild | GitVersion.MsBuild | Nerdbank.GitVersioning |
|------|----------------|--------------------|-----------------------|
| 零配置 | ✅ | ❌ 需要 GitVersion.yml | ❌ 需要 version.json |
| 台灣時區 | ✅ 預設 +08:00 | ❌ UTC | ❌ UTC |
| 輕量級 | ✅ 純 MSBuild | ❌ 外部工具 | ❌ 複雜設定 |
| 即用即走 | ✅ 安裝即用 | ❌ 需要學習 | ❌ 需要配置 |
| 語義化版本 | ❌ 簡單格式 | ✅ 完整 SemVer | ✅ 完整 SemVer |

## 📄 授權

MIT License

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request！

- 專案首頁: https://github.com/HugoUno/gitver
- 問題回報: https://github.com/HugoUno/gitver/issues