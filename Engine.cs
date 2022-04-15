using System.Diagnostics;

namespace desseract;

public class Engine : IDisposable
{
    private bool _disposed;
    public object InputFile { get; set; }
    public string OutputFile { get; set; } = null!;
    private string OutputFilePath => OutputFile + ".txt";

    public Engine() { }

    public async Task<StatusObject> ProcessAsync()
    {
        if (InputFile is not MemoryStream memStream) return ProcessFile((InputFile as string)!);
        
        const string streamToFilePath = "temp.png";
        await using var file = new FileStream(streamToFilePath, FileMode.Create, FileAccess.Write);
        memStream.Seek(0, SeekOrigin.Begin);
        await memStream.CopyToAsync(file);

        return ProcessFile(streamToFilePath);
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

    private ProcessStartInfo InitializeProcess(string input)
    {
        return new ProcessStartInfo
        {
            FileName = "tesseract",
            Arguments = $"{input} {OutputFile}",
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
        var processStartInfo = InitializeProcess(input);
        var tesseractResponse = RunProcess(processStartInfo);
        File.Delete(input);
        
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

        var response = File.ReadAllText(OutputFilePath);
        File.Delete(OutputFilePath);

        return new StatusObject
        {
            Status = EngineStatus.Success,
            Output = response
        };
    }
}