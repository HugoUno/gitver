// GitVer 套件測試
using TestApp;

Console.WriteLine($"應用程式版本: {GitVersionInfo.Version}");
Console.WriteLine($"Git 標籤: {GitVersionInfo.Tag}");
Console.WriteLine($"Git 提交: {GitVersionInfo.Commit}");
Console.WriteLine($"建置時間: {GitVersionInfo.BuildTimestamp}");
