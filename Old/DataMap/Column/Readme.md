|                 Method |       Mean |     Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------------------- |-----------:|----------:|-----------:|------------:|------------:|------------:|--------------------:|
|        Runtime2GetName |   228.5 ns |  32.95 ns |  1.8062 ns |      0.0322 |           - |           - |               136 B |
| Runtime2GetNameAndMeta |   447.1 ns | 370.20 ns | 20.2917 ns |      0.0319 |           - |           - |               136 B |
|       Runtime2GetValue |   191.7 ns |  22.99 ns |  1.2602 ns |      0.0055 |           - |           - |                24 B |
|               Runtime2 |   607.5 ns | 288.05 ns | 15.7890 ns |      0.0372 |           - |           - |               160 B |
|              Runtime10 | 2,959.0 ns |  62.87 ns |  3.4459 ns |      0.1793 |           - |           - |               760 B |
|         RuntimeFirst2N |   194.1 ns |  17.48 ns |  0.9584 ns |      0.0095 |           - |           - |                42 B |
|        RuntimeFirst10N | 1,087.1 ns |  97.33 ns |  5.3350 ns |      0.0458 |           - |           - |               194 B |
|                 Cache2 |   567.9 ns |  75.49 ns |  4.1381 ns |      0.0725 |           - |           - |               304 B |
|                Cache10 | 2,418.5 ns | 295.53 ns | 16.1992 ns |      0.2480 |           - |           - |              1056 B |
|          CacheSpecial2 |   489.0 ns | 460.32 ns | 25.2317 ns |      0.0477 |           - |           - |               200 B |
|         CacheSpecial10 | 2,222.1 ns | 172.33 ns |  9.4461 ns |      0.2060 |           - |           - |               864 B |
|         CacheSpecial2N |   195.2 ns |  44.90 ns |  2.4610 ns |      0.0095 |           - |           - |                42 B |
|        CacheSpecial10N | 1,079.2 ns | 137.43 ns |  7.5330 ns |      0.0458 |           - |           - |               194 B |
