using System.Diagnostics;
using desseract.Enums;
using desseract.Extensions;
using desseract.Models;

namespace desseract;

public class Engine : IDisposable
{
    private bool _disposed;
    public object InputSource { get; set; } = null!;
    private LogLevel LogLevel { get; }
    private static string OutputFileName => $"~__dessertemp__~";
    private static string OutputFilePath => $"{OutputFileName}.txt";

    public Engine(LogLevel logLevel = LogLevel.Fatal)
    {
        LogLevel = logLevel;
    }

    public Engine(object inputSource, LogLevel logLevel = LogLevel.Fatal) : this(logLevel)
    {
        InputSource = inputSource;
    }

    public async Task<StatusObject> ProcessAsync()
    {
        if (InputSource is not MemoryStream memStream) return ProcessFile((InputSource as string)!);
        
        await using var file = new FileStream(OutputFilePath, FileMode.Create, FileAccess.Write);
        memStream.Seek(0, SeekOrigin.Begin);
        await memStream.CopyToAsync(file);

        return ProcessFile(OutputFilePath);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (!disposing) return;
        
        if (!File.Exists(OutputFilePath))
            return;
            
        File.Delete(OutputFilePath);
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private static ProcessStartInfo InitializeProcess(string input, LogLevel logLevel)
    {
        return new ProcessStartInfo
        {
            FileName = "tesseract",
            Arguments = $"--loglevel {logLevel.ToTesseractFlag()} {input} {OutputFileName}",
            RedirectStandardOutput = false,
            UseShellExecute = false
        };
    }

    private static StatusObject RunProcess(ProcessStartInfo processStartInfo)
    {
        var process = Process.Start(processStartInfo);
        if (process is null)
            return new StatusObject
            {
                Status = EngineStatus.TesseractFail,
                Output = string.Empty
            };
        
        process.WaitForExit();
        return new StatusObject
        {
            Status = EngineStatus.Success,
            Output = string.Empty
        };
    }

    private StatusObject ProcessFile(string input)
    {
        var processStartInfo = InitializeProcess(input, LogLevel);
        var tesseractResponse = RunProcess(processStartInfo);
        
        if  (tesseractResponse.Status == EngineStatus.TesseractFail)
            return new StatusObject
            {
                Status = EngineStatus.FileReadFail,
                Output = string.Empty
            };

        if (!File.Exists(OutputFilePath))
        {
            return new StatusObject
            {
                Status = EngineStatus.FileReadFail,
                Output = string.Empty
            };
        }

        return new StatusObject
        {
            Status = EngineStatus.Success,
            Output = File.ReadAllText(OutputFilePath)
        };
    }
}