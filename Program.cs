using SolidMatrix.Toolkits.QrSheets;
using System.Diagnostics;

QrSheets qrSheets = new();
Stopwatch sw = new();

Console.WriteLine($"生成二维码-开始");
sw.Start();
qrSheets.PrepareQrCode();
sw.Stop();
Console.WriteLine($"生成二维码-完成 {sw.ElapsedMilliseconds}ms");

sw.Reset();

Console.WriteLine($"生成PDF-开始");
sw.Start();
qrSheets.Gen();
sw.Stop();
Console.WriteLine($"生成PDF-完成 {sw.ElapsedMilliseconds}ms");

Console.ReadKey();