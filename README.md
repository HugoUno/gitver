# GitVer - Git 版本自動化工具

基於 Git 的 .NET 版本自動化系統，在編譯時自動生成包含 Git 標籤、提交哈希和建置時間的版本字串。

## 功能特色

- ✅ **完全自動化** - 無需手動修改版本號
- ✅ **Git 原生整合** - 基於 tag、commit hash、建置時間戳
- ✅ **編譯時生成** - 每次 build 自動更新版本信息
- ✅ **多重後備策略** - 確保在各種 Git 狀態下都能生成有效版本
- ✅ **台灣時區** - 使用 +08:00 時區顯示建置時間

## 版本格式

版本字串會根據 Git 狀態自動選擇最適合的格式：

- **標準格式**：`v1.0.0-db3ebe00 @ 2025-10-08T15:32:34+08:00`
- **僅有標籤**：`v1.0.0 @ 2025-10-08T15:32:34+08:00`
- **僅有提交**：`db3ebe00 @ 2025-10-08T15:32:34+08:00`
- **無 Git 信息**：`2025-10-08T15:32:34+08:00`

## 快速開始

### 建置和執行

```bash
dotnet build
dotnet run
```

輸出範例：
```
Application Version: v1.0.0-db3ebe00 @ 2025-10-08T15:32:34+08:00
```

### 發布新版本

```bash
# 創建新版本標籤
git tag v1.1.0

# 重新建置即可獲得新版本
dotnet build && dotnet run
```

## 如何整合到其他專案

### 方法一：複製核心程式碼

1. **複製版本解析邏輯**到目標專案的 `.csproj` 檔案：

```xml
<!-- 在 <Project> 標籤內加入以下內容 -->

<!-- 1. 版本解析目標 -->
<Target Name="ResolveVersion" BeforeTargets="GenerateVersionSource">
  <PropertyGroup>
    <DefineConstantsOriginal>$(DefineConstants)</DefineConstantsOriginal>
    <VersionEffective Condition="'$(VersionEffective)' == '' and '$(Version)' != '' and '$(Version)' != '1.0.0'">$(Version)</VersionEffective>
    <VersionFromConstants Condition="'$(VersionFromConstants)' == '' and '$(DefineConstantsOriginal)' != ''">$([System.Text.RegularExpressions.Regex]::Match('$(DefineConstantsOriginal)', 'VERSION=(?:&quot;(?&lt;value&gt;[^"]+)&quot;|(?&lt;value&gt;[^;]+))').Groups['value'].Value)</VersionFromConstants>
    <VersionEffective Condition="'$(VersionEffective)' == '' and '$(VersionFromConstants)' != ''">$(VersionFromConstants)</VersionEffective>
  </PropertyGroup>

  <!-- Git 命令執行 -->
  <Exec
    Command="git -C &quot;$(MSBuildProjectDirectory)&quot; describe --tags --always"
    ConsoleToMSBuild="true"
    IgnoreExitCode="true"
    StandardOutputImportance="Low">
    <Output TaskParameter="ConsoleOutput" PropertyName="GitDescribeOutputRaw" />
  </Exec>

  <Exec
    Command="git -C &quot;$(MSBuildProjectDirectory)&quot; describe --tags --abbrev=0"
    ConsoleToMSBuild="true"
    IgnoreExitCode="true"
    StandardOutputImportance="Low">
    <Output TaskParameter="ConsoleOutput" PropertyName="GitTagRaw" />
  </Exec>

  <Exec
    Command="git -C &quot;$(MSBuildProjectDirectory)&quot; rev-parse --short=8 HEAD"
    ConsoleToMSBuild="true"
    IgnoreExitCode="true"
    StandardOutputImportance="Low">
    <Output TaskParameter="ConsoleOutput" PropertyName="GitCommitRaw" />
  </Exec>

  <!-- 版本字串組裝 -->
  <PropertyGroup>
    <GitDescribeOutput>$([System.String]::Copy('$(GitDescribeOutputRaw)').Trim())</GitDescribeOutput>
    <GitTag>$([System.String]::Copy('$(GitTagRaw)').Trim())</GitTag>
    <GitTag Condition="'$(GitTag)' != '' and $([System.String]::Copy('$(GitTag)').StartsWith('fatal:'))"></GitTag>
    <GitCommit>$([System.String]::Copy('$(GitCommitRaw)').Trim())</GitCommit>
    <BuildTimestamp>$([System.DateTime]::UtcNow.AddHours(8).ToString(&quot;yyyy-MM-ddTHH:mm:ss+08:00&quot;))</BuildTimestamp>
    <AutoVersion Condition="'$(VersionEffective)' == '' and '$(GitTag)' != '' and '$(GitCommit)' != ''">$(GitTag)-$(GitCommit) @ $(BuildTimestamp)</AutoVersion>
    <AutoVersion Condition="'$(VersionEffective)' == '' and '$(AutoVersion)' == '' and '$(GitTag)' != ''">$(GitTag) @ $(BuildTimestamp)</AutoVersion>
    <AutoVersion Condition="'$(VersionEffective)' == '' and '$(AutoVersion)' == '' and '$(GitCommit)' != ''">$(GitCommit) @ $(BuildTimestamp)</AutoVersion>
    <AutoVersion Condition="'$(VersionEffective)' == '' and '$(AutoVersion)' == '' and '$(GitDescribeOutput)' != ''">$(GitDescribeOutput) @ $(BuildTimestamp)</AutoVersion>
    <AutoVersion Condition="'$(VersionEffective)' == '' and '$(AutoVersion)' == ''">$(BuildTimestamp)</AutoVersion>
    <VersionEffective Condition="'$(VersionEffective)' == ''">$(AutoVersion)</VersionEffective>
    <DefineConstantsSanitized Condition="'$(DefineConstantsOriginal)' != ''">$([System.Text.RegularExpressions.Regex]::Replace('$(DefineConstantsOriginal)', '(^|;)VERSION=(?:&quot;[^"]+&quot;|(?&lt;value&gt;[^;]+)', '$1'))</DefineConstantsSanitized>
    <DefineConstantsSanitized Condition="'$(DefineConstantsSanitized)' != ''">$([System.Text.RegularExpressions.Regex]::Replace('$(DefineConstantsSanitized)', ';;+', ';'))</DefineConstantsSanitized>
    <DefineConstantsSanitized Condition="'$(DefineConstantsSanitized)' != ''">$([System.Text.RegularExpressions.Regex]::Replace('$(DefineConstantsSanitized)', '^;|;$', ''))</DefineConstantsSanitized>
    <DefineConstants Condition="'$(DefineConstantsSanitized)' != ''">$(DefineConstantsSanitized)</DefineConstants>
    <DefineConstants Condition="'$(DefineConstantsSanitized)' == '' and '$(DefineConstantsOriginal)' != ''"></DefineConstants>
  </PropertyGroup>
</Target>

<!-- 2. 版本源碼生成目標 -->
<Target Name="GenerateVersionSource" BeforeTargets="BeforeBuild">
  <PropertyGroup>
    <GeneratedVersionPath>$(IntermediateOutputPath)GeneratedVersion.g.cs</GeneratedVersionPath>
    <VersionLiteral>$([MSBuild]::Escape('$(VersionEffective)'))</VersionLiteral>
  </PropertyGroup>

  <ItemGroup>
    <VersionSourceLine Include="namespace YourNamespace%3b" />
    <VersionSourceLine Include="internal static partial class Program" />
    <VersionSourceLine Include="{" />
    <VersionSourceLine Include="    static partial void ApplyVersionOverride(ref string version)" />
    <VersionSourceLine Include="    {" />
    <VersionSourceLine Include="        version = &quot;$(VersionLiteral)&quot;%3b" />
    <VersionSourceLine Include="    }" />
    <VersionSourceLine Include="}" />
    <Compile Include="$(GeneratedVersionPath)" />
  </ItemGroup>

  <WriteLinesToFile
    File="$(GeneratedVersionPath)"
    Lines="@(VersionSourceLine)"
    Overwrite="true"
    Encoding="UTF-8" />
</Target>
```

2. **修改目標類別**（如 Program.cs）：

```csharp
namespace YourNamespace;

internal static partial class Program
{
    // 預設版本字串，若編譯時帶入參數會被覆寫
    internal static string Version = "dev";

    // 由編譯時生成的部分方法
    static partial void ApplyVersionOverride(ref string version);

    private static void Main()
    {
        // 套用自動生成的版本
        ApplyVersionOverride(ref Version);

        Console.WriteLine($"Application Version: {Version}");
    }
}
```

### 方法二：NuGet 套件（推薦）

**注意**：這個專案目前是概念驗證版本，建議包裝成 NuGet 套件以便重複使用。

### 方法三：Git Submodule

如果你有多個專案需要相同的版本控制邏輯：

```bash
# 在目標專案中加入 submodule
git submodule add https://github.com/youruser/gitver.git tools/gitver

# 參考 tools/gitver/gitver.csproj 的實作
```

## 自訂配置

### 修改時區

預設使用台灣時區 (+08:00)，如需修改：

```xml
<BuildTimestamp>$([System.DateTime]::UtcNow.AddHours(YOUR_OFFSET).ToString(&quot;yyyy-MM-ddTHH:mm:ssZZ&quot;))</BuildTimestamp>
```

### 修改版本格式

可以自訂 `AutoVersion` 的格式：

```xml
<!-- 範例：只顯示標籤和時間 -->
<AutoVersion Condition="'$(VersionEffective)' == '' and '$(GitTag)' != ''">$(GitTag) ($(BuildTimestamp))</AutoVersion>
```

### 修改命名空間

將生成目標中的 `YourNamespace` 替換為你的專案命名空間：

```xml
<VersionSourceLine Include="namespace YourProject.Core%3b" />
```

## 故障排除

### Git 命令失敗

如果 Git 命令執行失敗，系統會自動降級到時間戳版本：

```
2025-10-08T15:32:34+08:00
```

### 編譯錯誤

確保：
1. 目標類別宣告為 `partial`
2. 包含正確的 `ApplyVersionOverride` 部分方法宣告
3. 命名空間與生成的程式碼一致

## 授權

MIT License