# Huffman
<hr/>

### Resources
- [Huffman Coding wikipedia](https://en.wikipedia.org/wiki/Huffman_coding)
- [Inspo (Tom scott)](https://www.youtube.com/watch?v=JsTptu56GM8)
- [Binary file spec (warning - gross)](https://github.com/charliethomson/Huffman/blob/main/Huffman/spec.md)
``` ini
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1555/22H2/2022Update/SunValley2)
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
```
|                   Method |   NumBytes |                 Mean |              Error |             StdDev |        Gen0 |        Gen1 |      Gen2 |    Allocated |
|------------------------- |----------- |---------------------:|-------------------:|-------------------:|------------:|------------:|----------:|-------------:|
|      **BenchStringEncoding** |       **1000** |         **24,974.16 ns** |         **141.683 ns** |         **110.617 ns** |      **1.7090** |      **0.0305** |         **-** |      **28960 B** |
|      BenchStringDecoding |       1000 |         11,220.93 ns |         153.668 ns |         143.741 ns |      0.3815 |           - |         - |       6576 B |
|      BenchTreeGeneration |       1000 |          5,012.03 ns |          45.151 ns |          42.235 ns |      0.3738 |           - |         - |       6304 B |
|   BenchTreeSerialization |       1000 |          1,444.55 ns |          17.399 ns |          16.275 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |       1000 |             64.16 ns |           1.294 ns |           1.490 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** |      **10000** |        **125,011.64 ns** |       **1,106.551 ns** |         **980.928 ns** |      **3.1738** |           **-** |         **-** |      **53960 B** |
|      BenchStringDecoding |      10000 |        209,773.18 ns |       1,135.150 ns |       1,061.820 ns |      3.4180 |           - |         - |      60496 B |
|      BenchTreeGeneration |      10000 |         11,407.17 ns |         112.313 ns |         105.057 ns |      0.3662 |           - |         - |       6304 B |
|   BenchTreeSerialization |      10000 |          1,438.18 ns |          19.776 ns |          17.531 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |      10000 |             68.04 ns |           0.750 ns |           0.626 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** |     **100000** |      **1,110,567.61 ns** |      **19,290.659 ns** |      **17,100.666 ns** |     **15.6250** |           **-** |         **-** |     **274817 B** |
|      BenchStringDecoding |     100000 |      2,353,522.18 ns |      21,524.549 ns |      17,973.973 ns |     58.5938 |     58.5938 |   58.5938 |     470374 B |
|      BenchTreeGeneration |     100000 |         73,406.48 ns |         841.216 ns |         745.716 ns |      0.3662 |           - |         - |       6304 B |
|   BenchTreeSerialization |     100000 |          1,425.15 ns |          28.059 ns |          33.402 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |     100000 |             65.41 ns |           1.335 ns |           1.998 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** |    **1000000** |     **11,338,011.90 ns** |      **72,101.962 ns** |      **60,208.404 ns** |    **671.8750** |    **671.8750** |  **671.8750** |    **3303073 B** |
|      BenchStringDecoding |    1000000 |     23,513,525.67 ns |     115,050.455 ns |     101,989.231 ns |    531.2500 |    500.0000 |  437.5000 |    4602380 B |
|      BenchTreeGeneration |    1000000 |        930,196.07 ns |      16,291.465 ns |      15,239.046 ns |           - |           - |         - |       6305 B |
|   BenchTreeSerialization |    1000000 |          1,345.37 ns |          26.935 ns |          25.195 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |    1000000 |             69.18 ns |           1.354 ns |           2.576 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** |   **10000000** |    **118,219,415.71 ns** |   **1,447,778.288 ns** |   **1,283,417.742 ns** |    **200.0000** |    **200.0000** |  **200.0000** |   **28602259 B** |
|      BenchStringDecoding |   10000000 |    235,705,366.67 ns |   4,586,876.964 ns |   5,633,096.839 ns |   1000.0000 |    666.6667 |         - |   45992152 B |
|      BenchTreeGeneration |   10000000 |      7,062,441.20 ns |      80,006.296 ns |      74,837.939 ns |           - |           - |         - |       6309 B |
|   BenchTreeSerialization |   10000000 |          1,470.23 ns |          29.375 ns |          74.769 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |   10000000 |             60.95 ns |           1.227 ns |           2.050 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** |  **100000000** |  **1,091,571,900.00 ns** |  **15,697,183.505 ns** |  **13,915,144.313 ns** |           **-** |           **-** |         **-** |  **252240088 B** |
|      BenchStringDecoding |  100000000 |  2,257,791,892.00 ns |  40,725,733.531 ns |  54,367,684.498 ns |  15000.0000 |  14000.0000 | 3000.0000 |  459926384 B |
|      BenchTreeGeneration |  100000000 |     95,847,146.15 ns |   1,753,589.277 ns |   1,464,326.467 ns |           - |           - |         - |       6404 B |
|   BenchTreeSerialization |  100000000 |          1,640.56 ns |          32.801 ns |          42.651 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization |  100000000 |             71.13 ns |           1.451 ns |           2.580 ns |      0.0367 |           - |         - |        616 B |
|      **BenchStringEncoding** | **1000000000** | **11,359,382,413.33 ns** | **102,518,627.631 ns** |  **95,895,987.893 ns** |   **1000.0000** |   **1000.0000** | **1000.0000** | **3327495840 B** |
|      BenchStringDecoding | 1000000000 | 23,238,309,493.75 ns | 445,766,167.239 ns | 437,801,980.480 ns | 126000.0000 | 125000.0000 | 6000.0000 | 4599040520 B |
|      BenchTreeGeneration | 1000000000 |    915,191,064.29 ns |  17,474,345.289 ns |  15,490,551.945 ns |           - |           - |         - |       6904 B |
|   BenchTreeSerialization | 1000000000 |          1,407.23 ns |          12.129 ns |          10.752 ns |      0.4158 |      0.0019 |         - |       6984 B |
| BenchTreeDeserialization | 1000000000 |             66.79 ns |           1.088 ns |           1.018 ns |      0.0367 |           - |         - |        616 B |
