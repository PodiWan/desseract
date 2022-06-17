# desseract
Your friendly neighbourhood tessaract wrapper for dotnet.

### Dependencies
This package is a [Tesseract](https://github.com/tesseract-ocr/tesseract) wrapper. Understandably, you are required to have it installed on your machine.

### Using the API
The OCR exposes two `Engine` constructors, one accepting an input parameter and a parameterless one.
```csharp
var engine = new Engine();
engine.InputSource = "example.png";

var response = await engine.ProcessAsync();
if (response.Status == EngineStatus.Success)
    Console.WriteLine(response.Output);
else
    Console.WriteLine("Could not parse file");
```

You could also do
```csharp
var engine = new Engine("example.png");

var response = await engine.ProcessAsync();
if (response.Status == EngineStatus.Success)
    Console.WriteLine(response.Output);
else
    Console.WriteLine("Could not parse file");
```

Running these snippets on the `example.png` image give the following result:
```text
Image

1234567890

Results

1234567890
```