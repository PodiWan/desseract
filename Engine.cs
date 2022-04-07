using System.Diagnostics;

namespace desseract;

public class Engine : IDisposable
{
    private bool _disposed;
    public string InputFile { get; set; } = null!;
    public string OutputFile { get; set; } = null!;
    private string OutputFilePath => OutputFile + ".txt";

    public Engine() { }

    public Engine(string inputFile)
    {
        InputFile = inputFile;
    }

    public StatusObject ProcessFile()
    {
        var processStartInfo = InitializeProcess();
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

    private ProcessStartInfo InitializeProcess()
    {
        return new ProcessStartInfo
        {
            FileName = "tesseract",
            Arguments = $"{InputFile} {OutputFile}",
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
}