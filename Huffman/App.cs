using System.Diagnostics;
using Huffman.Infra.Services;

namespace Huffman;

public class App
{
    private readonly IHuffmanService _huffmanService;

    private const int NumChars = 1000;

    public App(IHuffmanService huffmanService)
    {
        _huffmanService = huffmanService ?? throw new ArgumentNullException(nameof(huffmanService));
    }

    public void Run()
    {
        try
        {
            
            Console.Write($"Generating string length {(NumChars.ToString() + "..."),-15}");
            Console.Out.Flush();
            var data = "";
            for (var i = 0; i < NumChars; i++)
                data += (char)Random.Shared.Next(65, 90); /* ASCII uppercase */
            Console.WriteLine($"Done;");


            
            Console.Write($"Encoding Data...                        ");
            Console.Out.Flush();
            var encodeTimer = new Stopwatch();
            encodeTimer.Start();
            var serializedData = _huffmanService.SerializeData(data);
            encodeTimer.Stop();
            Console.WriteLine($"Done;");

            Console.Write($"Decoding Data...                        ");
            Console.Out.Flush();
            
            var decodeTimer = new Stopwatch();
            decodeTimer.Start();
            var deserializedData = _huffmanService.DeserializeData(serializedData);
            decodeTimer.Stop();
            Console.WriteLine($"Done;");

            Console.WriteLine($"Encoded in:                      {encodeTimer.ElapsedMilliseconds.ToString(),8} ms;");
            Console.WriteLine($"Decoded in:                      {decodeTimer.ElapsedMilliseconds.ToString(),8} ms;");

            Console.WriteLine($"Encoded byte count:                      {serializedData.Count};");
            Console.WriteLine($"Compression rate:                      %{(serializedData.Count / (float)data.Length) * 100f:0.##};");
            
            Console.WriteLine($"Expected                        {data.Length + " bytes",12};");
            Console.WriteLine($"Got                             {deserializedData.Length + " bytes",12};");
            Console.WriteLine($"Lossless?                       {deserializedData == data,12};");
            
            Console.WriteLine($"\nPress any key to exit...");
            // Console.ReadKey();
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }
}