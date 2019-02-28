``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
Intel Core i7-4771 CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.100
  [Host]   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  ShortRun : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|       Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
|        Alloc | 43.30 ns |  7.293 ns | 0.3997 ns |      0.0247 |           - |           - |               104 B |
|         Pool | 87.25 ns | 93.443 ns | 5.1219 ns |           - |           - |           - |                   - |
| ThreadStatic | 38.07 ns |  1.387 ns | 0.0760 ns |           - |           - |           - |                   - |
