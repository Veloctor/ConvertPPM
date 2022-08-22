using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

string fileName;
if (args.Length == 1) fileName = args[0];
else
{
start:
    Console.WriteLine("请输入要转换的ppm,留空认为\"binary.ppm\":");
    fileName = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(fileName)) fileName = "binary.ppm";
}
int resX, resY;
try
{
    using StreamReader openText = File.OpenText(fileName);
    string MagicNumber = openText.ReadLine();
    if (MagicNumber != "P6")
    {
        Console.WriteLine("Magic number is not P6. It's " + MagicNumber);
        goto start;
    }
    string[] res = openText.ReadLine().Split(' ');
    resX = Convert.ToInt32(res[0]);
    resY = Convert.ToInt32(res[1]);
}
catch(FileNotFoundException)
{
    Console.WriteLine(fileName + "文件未找到.");
    goto start;
}
using var inputFS = File.OpenRead(fileName);
int enterCount = 3;
while (enterCount > 0)
{
    if (inputFS.ReadByte() == '\n') enterCount--;
}
WriteableBitmap bitmap = new(resX, resY, 96, 96, PixelFormats.Rgb24, null);
bitmap.Lock();
Span<byte> sp;
unsafe
{
    void* bufferPtr = (void*)bitmap.BackBuffer;
    sp = new(bufferPtr, resX * resY * 3);
}
inputFS.Read(sp);
bitmap.Unlock();
PngBitmapEncoder encoder = new();
encoder.Frames.Add(BitmapFrame.Create(bitmap));
string outputName = Path.ChangeExtension(fileName, "png");
using FileStream outputFS = new(outputName, FileMode.Create);
encoder.Save(outputFS);
Console.WriteLine(outputName + "输出完毕.");
