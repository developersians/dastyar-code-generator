using System.Reflection;

namespace Dastyar.CodeGenerator;

public static class ProjectRootFinder
{
    public static string GetProjectRootPath()
    {
        // 1. ابتدا مسیر اسمبلی در حال اجرا را پیدا می‌کنیم
        var assemblyPath = Assembly.GetEntryAssembly()?.Location;

        // 2. اگر اسمبلی وجود نداشت (مثلاً در تست‌ها)، از مسیر اجرایی استفاده می‌کنیم
        if (string.IsNullOrEmpty(assemblyPath))
        {
            assemblyPath = Environment.ProcessPath;
        }

        // 3. تبدیل به DirectoryInfo برای پیمایش سلسله مراتبی
        var directory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath)!);

        // 4. پیمایش به سمت بالا تا یافتن فایل csproj یا sln
        while (directory != null &&
              !directory.GetFiles("*.csproj").Any() &&
              !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        // 5. اگر پیدا نشد، از مسیر جاری استفاده می‌کنیم
        directory ??= new DirectoryInfo(Directory.GetCurrentDirectory());

        return directory.FullName;
    }
}
