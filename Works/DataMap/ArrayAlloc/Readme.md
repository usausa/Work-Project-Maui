|        Method |     Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |---------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
|         Alloc | 41.88 ns | 22.18 ns | 1.216 ns |      0.0247 |           - |           - |               104 B |
|          Pool | 87.35 ns | 46.48 ns | 2.548 ns |           - |           - |           - |                   - |
|  ThreadStatic | 39.79 ns | 20.37 ns | 1.117 ns |           - |           - |           - |                   - |
| ThreadStatic2 | 35.60 ns | 43.31 ns | 2.374 ns |           - |           - |           - |                   - |
