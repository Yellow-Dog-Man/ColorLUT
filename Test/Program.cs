using ColorLUT.CUBE;

Console.Write("Open File: ");
var readFile = Console.ReadLine();

Console.Write("Write File: ");
var writeFile = Console.ReadLine();

using (var reader = new CUBE_Reader(readFile))
using (var writer = new CUBE_Writer(writeFile))
{
    Console.WriteLine($"Title: {reader.Title}\n" +
        $"Dimensions: {reader.Dimensions}\n" +
        $"Size: {reader.Size}\n" +
        $"MinValue: [{reader.MinValueR}, {reader.MinValueG}, {reader.MinValueB}], MaxValue: [{reader.MaxValueR}, {reader.MaxValueG}, {reader.MaxValueB}]\n" +
        $"DomainMin: [{reader.DomainMinR}, {reader.DomainMinG}, {reader.DomainMinB}], DomainMax: [{reader.DomainMaxR}, {reader.DomainMaxG}, {reader.DomainMaxB}]\n");

    writer.Title = reader.Title;
    writer.Dimensions = reader.Dimensions;
    writer.Size = reader.Size;

    writer.DomainMinR = reader.DomainMinR;
    writer.DomainMinG = reader.DomainMinG;
    writer.DomainMinB = reader.DomainMinB;

    writer.DomainMaxR = reader.DomainMaxR;
    writer.DomainMaxG = reader.DomainMaxG;
    writer.DomainMaxB = reader.DomainMaxB;

    writer.WriteHeader();

    while (!reader.ReadAllValues)
    {
        Console.Write($"[{reader.X};{reader.Y};{reader.Z}] = ");

        reader.ReadColor(out var r, out var g, out var b);
        writer.WriteColor(r, g, b);

        Console.WriteLine($"R={r:F2} G={g:F2} B={b:F2}");
    }

    Console.ReadLine();
}
