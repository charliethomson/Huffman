using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Huffman.Infra.Services;
using Huffman.Infra.Services.Generation;

namespace Huffman;

public class App
{
    private readonly IHuffmanService _huffmanService;

    // TODO: Remove
    private readonly ITreeGenerationService _treeGenerationService;

    private const int NumChars = 100000;
    private const int NumRuns = 10_000_000 / NumChars;
    private const int NumRunsHamlet = 50;

    private List<long> encodeTimes = new();
    private List<long> decodeTimes = new();
    private long totalInitialBytes = 0;
    private long totalEncodedBytes = 0;

    public App(IHuffmanService huffmanService, ITreeGenerationService treeGenerationService)
    {
        _huffmanService = huffmanService ?? throw new ArgumentNullException(nameof(huffmanService));
        _treeGenerationService =
            treeGenerationService ?? throw new ArgumentNullException(nameof(treeGenerationService));
    }

    public void Run()
    {
        try
        {
            Console.Write($"\nGenerating string length {(NumChars.ToString() + "..."),-15}");
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

            var compressionRate = (serializedData.Count / (float)data.Length) * 100f;
            Console.WriteLine($"Encoded byte count:             {serializedData.Count,12};");
            Console.WriteLine($"Compression rate:                     {("%" + compressionRate.ToString("0.##")),6};");

            Console.WriteLine($"Expected                        {data.Length + " bytes",12};");
            Console.WriteLine($"Got                             {deserializedData.Length + " bytes",12};");
            var lossless = deserializedData == data;
            Console.WriteLine($"Lossless?                       {lossless,12};");

            if (lossless) return;


            var path = Environment.CurrentDirectory +
                       $@"\lossy-debug-data-{DateTime.UtcNow.ToLongTimeString().Replace(' ', '-')}.json";
            Console.WriteLine($"Lossy compression detected, dumping in {path}");
            var debugData = new Dictionary<string, string>()
            {
                { "data", data },
                { "serializedData", $"[{string.Join(',', serializedData.Select(b => b.ToString()).ToList())}]" },
                { "deserializedData", deserializedData },
                { "compressionRate", compressionRate.ToString() }
            };

            using var sw = new StreamWriter(path);
            var json = JsonSerializer.Serialize(debugData);
            sw.Write(json);
            Console.WriteLine("Done.");
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }


    public async Task RunHamlet()
    {
        var client = new HttpClient();
        const string uri =
            @"https://gist.githubusercontent.com/provpup/2fc41686eab7400b796b/raw/b575bd01a58494dfddc1d6429ef0167e709abf9b/hamlet.txt";
        // const string uri = @"https://rainbowfluffysheep.wordpress.com/the-longest-text-ever/";
        //
        var data = await client.GetStringAsync(uri);
        //
        try
        {
            var runs = 0;

            void PrintStatus(string message) =>
                Console.Write(
                    $"\r{runs.ToString().PadLeft(NumRunsHamlet.ToString().Length, '0')} / {NumRunsHamlet} -- {message}");

            var lossless = true;
            ICollection<byte> serializedData = new List<byte>();
            while (lossless && runs < NumRunsHamlet)
            {
                runs++;

                PrintStatus("Encoding                                ");
                var encodeTimer = new Stopwatch();
                encodeTimer.Start();
                serializedData = _huffmanService.SerializeData(data);
                encodeTimer.Stop();

                PrintStatus("Decoding                                ");
                var decodeTimer = new Stopwatch();
                decodeTimer.Start();
                var deserializedData = _huffmanService.DeserializeData(serializedData);
                decodeTimer.Stop();

                PrintStatus("Verifying                               ");
                lossless = deserializedData == data;
                encodeTimes.Add(encodeTimer.ElapsedMilliseconds);
                decodeTimes.Add(decodeTimer.ElapsedMilliseconds);
                totalInitialBytes += data.Length;
                totalEncodedBytes += serializedData.Count;
            }

            if (lossless)
            {
                Console.WriteLine($"Fully lossless after {runs} runs.");

                var totalCompressionRate = (totalEncodedBytes / (float)totalInitialBytes) * 100f;

                var table = new Table("Compression rate (%)", "Encode time (avg)", "Decode time (avg)",
                    "Average encoded file size")
                {
                    Config = TableConfig.UnicodeAlt()
                };
                table.AddRow(totalCompressionRate.ToString("0.##") + '%',
                    $"{encodeTimes.Average(),6:0.##} ms",
                    $"{decodeTimes.Average(),6:0.##} ms",
                    $"{totalEncodedBytes / NumRunsHamlet} bytes");
                Console.WriteLine(table.ToString());

                return;
            }

            var jsonPath = Environment.CurrentDirectory +
                           $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.json";
            var binPath = Environment.CurrentDirectory +
                          $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin";

            Console.WriteLine(
                $"\n\nEncountered lossy compression after {runs} runs, dumping debug data in {jsonPath}, binary in {binPath}");

            var debugData = new DebugData(data, _huffmanService, _treeGenerationService);
            using var jsonSw = new StreamWriter(jsonPath);
            using var binSw = new FileStream(binPath, FileMode.OpenOrCreate);
            using var bw = new BinaryWriter(binSw);
            var json = JsonSerializer.Serialize(debugData);
            jsonSw.Write(json);
            bw.Write(serializedData.ToArray());
            Console.WriteLine("Done.");
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }

    public void RunWithStringAndDebugInfo(string data)
    {
        try
        {
            var runs = 0;

            var PrintStatus = (string message) =>
                Console.Write($"\r{runs.ToString().PadLeft(NumRuns.ToString().Length, '0')} / {NumRuns} -- {message}");

            var lossless = true;
            ICollection<byte> serializedData = new List<byte>();
            while (lossless && runs < NumRuns)
            {
                runs++;
                PrintStatus("Encoding                                ");
                var encodeTimer = new Stopwatch();
                encodeTimer.Start();
                serializedData = _huffmanService.SerializeData(data);
                encodeTimer.Stop();

                PrintStatus("Decoding                                ");
                var decodeTimer = new Stopwatch();
                decodeTimer.Start();
                var deserializedData = _huffmanService.DeserializeData(serializedData);
                decodeTimer.Stop();

                PrintStatus("Verifying                               ");
                lossless = deserializedData == data;
                encodeTimes.Add(encodeTimer.ElapsedMilliseconds);
                decodeTimes.Add(decodeTimer.ElapsedMilliseconds);
                totalInitialBytes += data.Length;
                totalEncodedBytes += serializedData.Count;
            }

            if (lossless)
            {
                Console.WriteLine($"Fully lossless after {runs} runs.");

                var totalCompressionRate = (totalEncodedBytes / (float)totalInitialBytes) * 100f;

                var table = new Table("Compression rate (%)", "Encode time (avg)", "Decode time (avg)",
                    "Average encoded file size")
                {
                    Config = TableConfig.UnicodeAlt()
                };
                table.AddRow(totalCompressionRate.ToString("0.##") + '%',
                    $"{encodeTimes.Average(),6:0.##} ms",
                    $"{decodeTimes.Average(),6:0.##} ms",
                    $"{totalEncodedBytes / NumRuns} bytes");
                Console.WriteLine(table.ToString());

                return;
            }

            var jsonPath = Environment.CurrentDirectory +
                           $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.json";
            var binPath = Environment.CurrentDirectory +
                          $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin";

            Console.WriteLine(
                $"\n\nEncountered lossy compression after {runs} runs, dumping debug data in {jsonPath}, binary in {binPath}");

            var debugData = new DebugData(data, _huffmanService, _treeGenerationService);
            using var jsonSw = new StreamWriter(jsonPath);
            using var binSw = new FileStream(binPath, FileMode.OpenOrCreate);
            using var bw = new BinaryWriter(binSw);
            var json = JsonSerializer.Serialize(debugData);
            jsonSw.Write(json);
            bw.Write(serializedData.ToArray());
            Console.WriteLine("Done.");
        }
        catch (IOException e)
        {
            Console.WriteLine($"The file could not be written: {e.Message}");
        }
    }

    public void RunUntilLossy()
    {
        try
        {
            var runs = 0;

            var PrintStatus = (string message) =>
                Console.Write($"\r{runs.ToString().PadLeft(NumRuns.ToString().Length, '0')} / {NumRuns} -- {message}");

            var lossless = true;
            ICollection<byte> serializedData = new List<byte>();
            var data = "";
            while (lossless && runs < NumRuns)
            {
                runs++;
                data = "";
                PrintStatus("Generating string                       ");
                data = GenerateRandomString();
                PrintStatus("Encoding                                ");
                var encodeTimer = new Stopwatch();
                encodeTimer.Start();
                serializedData = _huffmanService.SerializeData(data);
                encodeTimer.Stop();

                PrintStatus("Decoding                                ");
                var decodeTimer = new Stopwatch();
                decodeTimer.Start();
                var deserializedData = _huffmanService.DeserializeData(serializedData);
                decodeTimer.Stop();

                PrintStatus("Verifying                               ");
                lossless = deserializedData == data;
                encodeTimes.Add(encodeTimer.ElapsedMilliseconds);
                decodeTimes.Add(decodeTimer.ElapsedMilliseconds);
                totalInitialBytes += data.Length;
                totalEncodedBytes += serializedData.Count;
            }

            if (lossless)
            {
                Console.WriteLine($"Fully lossless after {runs} runs.");

                var totalCompressionRate = (totalEncodedBytes / (float)totalInitialBytes) * 100f;

                var table = new Table("Compression rate (%)", "Encode time (avg)", "Decode time (avg)",
                    "Average encoded file size")
                {
                    Config = TableConfig.UnicodeAlt()
                };
                table.AddRow(totalCompressionRate.ToString("0.##") + '%',
                    $"{encodeTimes.Average(),6:0.##} ms",
                    $"{decodeTimes.Average(),6:0.##} ms",
                    $"{totalEncodedBytes / NumRuns} bytes");
                Console.WriteLine(table.ToString());

                return;
            }

            var jsonPath = Environment.CurrentDirectory +
                           $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.json";
            var binPath = Environment.CurrentDirectory +
                          $@"\lossy-debug-data-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin";

            Console.WriteLine(
                $"\n\nEncountered lossy compression after {runs} runs, dumping debug data in {jsonPath}, binary in {binPath}");

            var debugData = new DebugData(data, _huffmanService, _treeGenerationService);
            using var jsonSw = new StreamWriter(jsonPath);
            using var binSw = new FileStream(binPath, FileMode.OpenOrCreate);
            using var bw = new BinaryWriter(binSw);
            var json = JsonSerializer.Serialize(debugData);
            jsonSw.Write(json);
            bw.Write(serializedData.ToArray());
            Console.WriteLine("Done.");
        }
        catch (IOException e)
        {
            Console.WriteLine($"The file could not be written: {e.Message}");
        }
    }

    private string GenerateRandomString()
    {
        var sb = new StringBuilder(NumChars);
        for (var i = 0; i < NumChars; i++)
            sb.Append((char)Random.Shared.Next(65, 90)); /* ASCII uppercase */
        return sb.ToString();
    }

    public void RunProfiling()
    {
        var runs = 0;

        var lossless = true;
        var data = GenerateRandomString();

        while (lossless && runs < NumRuns)
        {
            runs++;
            var serializedData = _huffmanService.SerializeData(data);
            var deserializedData = _huffmanService.DeserializeData(serializedData);
            lossless = deserializedData == data;
        }

        Console.WriteLine($"Lossless={lossless}");
    }
    
    public void ProfileDeserialization()
    {
        var runs = 0;

        var lossless = true;
        var data = GenerateRandomString();
        var serializedData = _huffmanService.SerializeData(data);

        while (lossless && runs < NumRuns * 100)
        {
            Console.WriteLine($"{runs} / {NumRuns * 100}");
            runs++;
            var deserializedData = _huffmanService.DeserializeData(serializedData);
            lossless = deserializedData == data;
        }

        Console.WriteLine($"Lossless={lossless}");
    }
    public void ProfileSerialization()
    {
        var runs = 0;

        var lossless = true;
        var data = GenerateRandomString();
        var serializedData = _huffmanService.SerializeData(data);
        var deserializedData = _huffmanService.DeserializeData(serializedData);
        lossless = deserializedData == data;

        while (lossless && runs < NumRuns * 100)
        {
            Console.WriteLine($"{runs} / {NumRuns * 100}");
            runs++;
            serializedData = _huffmanService.SerializeData(data);
        }

        Console.WriteLine($"Lossless={lossless}");
    }

    internal class DebugData
    {
        public string Data { get; set; }
        public string DeserializedData { get; set; }
        public float CompressionRate { get; set; }
        public IDictionary<char, string> CharacterMappings { get; set; }
        public ICollection<byte> SerializedData { get; set; }

        public uint MagicBytes { get; set; }
        public uint TreeSize { get; set; }
        public uint DataSize { get; set; }
        public uint DataEndPadding { get; set; }

        public IEnumerable<string> SerializedDataBits =>
            SerializedData.Select(b => Convert.ToString(b, 2).PadLeft(8, '0'));

        #region Reads

        private static byte ReadU8(ref List<byte> bytes)
        {
            var b = bytes.First();
            bytes.RemoveAt(0);
            return b;
        }

        private static uint ReadU32(ref List<byte> bytes)
        {
            var data = BitConverter.ToUInt32(bytes.Take(4).ToArray());
            bytes.RemoveRange(0, 4);
            return data;
        }

        private static List<byte> ReadBytes(ref List<byte> bytes, int nBytes)
        {
            var data = bytes.Take(nBytes).ToArray();
            bytes.RemoveRange(0, nBytes);
            return data.ToList();
        }

        #endregion

        public DebugData(string data, IHuffmanService huffmanService, ITreeGenerationService treeGenerationService)
        {
            Data = data;
            SerializedData = huffmanService.SerializeData(data);
            DeserializedData = huffmanService.DeserializeData(SerializedData);
            CharacterMappings = new Dictionary<char, string>();
            var tree = treeGenerationService.GenerateHuffmanTree(data);
            foreach (var c in data)
            {
                if (CharacterMappings.TryGetValue(c, out _)) continue;

                if (!tree.TryFindChildWithItem(c, out var node)) throw new UnreachableException();

                var (path, depth) = node.GetPathFromRootString();
                CharacterMappings.Add(c, path);
            }

            CompressionRate = SerializedData.Count / (float)data.Length * 100f;

            var bytes = new List<byte>(SerializedData);

            MagicBytes = BitConverter.ToUInt32(bytes.Take(4).Reverse().ToArray());
            bytes.RemoveRange(0, 4);


            TreeSize = ReadU32(ref bytes);
            var _consume_treeData = ReadBytes(ref bytes, (int)TreeSize);

            DataSize = ReadU32(ref bytes);
            DataEndPadding = ReadU8(ref bytes);
        }
    }
}