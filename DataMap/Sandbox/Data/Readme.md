|     Method |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
| QueryFirst | 11.237 us | 0.9144 us | 0.0501 us |      0.4425 |      0.1068 |           - |              1920 B |
|      Query | 48.740 us | 5.2771 us | 0.2893 us |      3.3569 |      0.7935 |           - |             14184 B |
|     Scalar |  5.103 us | 1.0687 us | 0.0586 us |      0.2289 |      0.0687 |           - |               992 B |
|    Execute |  8.971 us | 2.3897 us | 0.1310 us |      0.4730 |      0.1068 |           - |              2016 B |