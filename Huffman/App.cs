using Huffman.Infra.Services;

namespace Huffman;

public class App
{
    private readonly IHuffmanService _huffmanService;

    public App(IHuffmanService huffmanService)
    {
        _huffmanService = huffmanService ?? throw new ArgumentNullException(nameof(huffmanService));
    }

    public void Run()
    {
        try
        {
            const string data = "aabbccabcabbcaabbccaabbccabcabbcaabbccaabbccabcabbcaabbccaabbccabcabbcaabbcc";

            var serializedData = _huffmanService.SerializeData(data);
            var deserializedData = _huffmanService.DeserializeData(serializedData);

            Console.WriteLine("Expected:");
            Console.WriteLine(data.Length);
            Console.WriteLine("Actual:");
            Console.WriteLine(deserializedData.Length);
            Console.WriteLine("Success?:");
            Console.WriteLine(deserializedData == data);


            Console.ReadKey();
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }
}