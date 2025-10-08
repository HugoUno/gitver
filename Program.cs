namespace GitVer;

internal static partial class Program
{
    // 預設版本字串，若編譯時帶入參數會被覆寫
    internal static string Version = "dev";

    static partial void ApplyVersionOverride(ref string version);

    private static void Main()
    {
        // 透過編譯產生的部分方法套用最新版本
        ApplyVersionOverride(ref Version);

        Console.WriteLine($"App version: {Version}");
    }
}
