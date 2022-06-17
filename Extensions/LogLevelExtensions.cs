using desseract.Enums;

namespace desseract.Extensions;

public static class LogLevelExtensions
{
    public static string ToTesseractFlag(this LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Off => "OFF",
            LogLevel.Fatal => "FATAL",
            LogLevel.Error => "ERROR",
            LogLevel.Warning => "WARN",
            LogLevel.Information => "INFO",
            LogLevel.Debug => "DEBUG",
            LogLevel.Trace => "TRACE",
            LogLevel.All => "ALL",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
}