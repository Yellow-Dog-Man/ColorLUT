using ColorLUT.CUBE;

using (var reader = new CUBE_Reader("FILE"))
{
    Console.WriteLine($"Title: {reader.Title}\n" +
        $"Dimensions: {reader.Dimensions}\n" +
        $"Size: {reader.Size}\n" +
        $"MinValue: [{reader.MinR}, {reader.MinG}, {reader.MinB}], MaxValue: [{reader.MaxR}, {reader.MaxG}, {reader.MaxB}]");

    while(!reader.ReadAllValues)
    {
        Console.Write($"[{reader.X};{reader.Y};{reader.Z}] = ");

        reader.ReadColor(out var r, out var g, out var b);

        Console.WriteLine($"R={r:F2} G={g:F2} B={b:F2}");
    }

    Console.ReadLine();
}