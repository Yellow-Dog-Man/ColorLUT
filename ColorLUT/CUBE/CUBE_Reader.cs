using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace ColorLUT.CUBE
{
    public class CUBE_Reader : IDisposable
    {
        public string Title { get; private set; }
        public int Dimensions { get; private set; }
        public int Size { get; private set; }

        // Not all LUT's include these, so initialize them to some sane defaults
        public float MinValueR { get; private set; } = 0f;
        public float MinValueG { get; private set; } = 0f;
        public float MinValueB { get; private set; } = 0f;
        public float MaxValueR { get; private set; } = 1f;
        public float MaxValueG { get; private set; } = 1f;
        public float MaxValueB { get; private set; } = 1f;

        public float MinValue { set => MinValueR = MinValueG = MinValueB = value; }
        public float MaxValue { set => MaxValueR = MaxValueG = MaxValueB = value; }

        public float DomainMinR { get; private set; } = 0f;
        public float DomainMinG { get; private set; } = 0f;
        public float DomainMinB { get; private set; } = 0f;

        public float DomainMaxR { get; private set; } = 1f;
        public float DomainMaxG { get; private set; } = 1f;
        public float DomainMaxB { get; private set; } = 1f;

        public bool ReadAllValues
        {
            get
            {
                switch(Dimensions)
                {
                    case 1: return X == Size;
                    case 3: return Z == Size;

                    default:
                        throw new NotImplementedException("Dimensions: " + Dimensions);
                }
            }
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        StreamReader _stream;
        string _unprocessedLine;

        public CUBE_Reader(string filePath)
        {
            _stream = File.OpenText(filePath);

            ReadHeader();
        }

        public CUBE_Reader(StreamReader stream)
        {
            _stream = stream;

            ReadHeader();
        }

        public void ReadColor(out float r, out float g, out float b)
        {
            if (ReadAllValues)
                throw new InvalidOperationException("All color values have been read");

            var line = ReadLine();

            if (line == null)
                throw new Exception("Reached unexpected end of file");

            if (!TryParseRGB(line, out r, out g, out b))
                throw new Exception("Failed to parse color value");

            X++;

            if(Dimensions == 3)
            {
                if(X == Size)
                {
                    X = 0;
                    Y++;
                }

                if(Y == Size)
                {
                    Y = 0;
                    Z++;
                }
            }
        }

        void ReadHeader()
        {
            while (ReadHeaderLine()) ;

            if (Dimensions != 1 && Dimensions != 3)
                throw new Exception("Unsupported number of dimensions for CUBE file: " + Dimensions);

            if (Size <= 0)
                throw new Exception("Invalid size for CUBE file: " + Size);
        }

        bool ReadHeaderLine()
        {
            var line = ReadLine();

            if (line == null)
                return false;

            if(GetHeaderData(line, "TITLE", out var title))
            {
                if (title.StartsWith("\"") && title.EndsWith("\""))
                    title = title.Substring(1, title.Length - 2);

                Title = title;

                return true;
            }

            string data;

            if (GetHeaderData(line, "LUT_1D_SIZE", out data))
            {
                if (!int.TryParse(data, out var size))
                    throw new Exception("Invalid size: " + data);

                Dimensions = 1;
                Size = size;

                return true;
            }

            if (GetHeaderData(line, "LUT_3D_SIZE", out data))
            {
                if (!int.TryParse(data, out var size))
                    throw new Exception("Invalid size: " + data);

                Dimensions = 3;
                Size = size;

                return true;
            }

            if(GetHeaderData(line, "LUT_1D_INPUT_RANGE", out data) ||
                GetHeaderData(line, "LUT_3D_INPUT_RANGE", out data))
            {
                var values = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length != 2)
                    throw new Exception("Input range expects two values");

                if (!int.TryParse(values[0], out var minValue))
                    throw new Exception("Failed to parse min value");

                if (!int.TryParse(values[1], out var maxValue))
                    throw new Exception("Failed to parse max value");

                MinValue = minValue;
                MaxValue = maxValue;
            }

            if(GetHeaderData(line, "DOMAIN_MIN", out data))
            {
                if (!TryParseRGB(data, out var r, out var g, out var b))
                    throw new Exception("Failed to parse DOMAIN_MIN");

                DomainMinR = r;
                DomainMinG = g;
                DomainMinB = b;

                return true;
            }

            if (GetHeaderData(line, "DOMAIN_MAX", out data))
            {
                if (!TryParseRGB(data, out var r, out var g, out var b))
                    throw new Exception("Failed to parse DOMAIN_MAX");

                DomainMaxR = r;
                DomainMaxG = g;
                DomainMaxB = b;

                return true;
            }

            // This is not a processed line, we must save it so it get processed properly when parsing the text
            _unprocessedLine = line;

            return false;
        }

        string ReadLine()
        {
            if(_unprocessedLine != null)
            {
                var l = _unprocessedLine;

                _unprocessedLine = null;

                return l;
            }

            // Read line and skip comments and empty lines
            string line;

            do
            {
                line = _stream.ReadLine();
            } while (line != null && (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)));

            return line;
        }

        static bool TryParseRGB(string str, out float r, out float g, out float b)
        {
            var values = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 3)
                throw new Exception("Expected three values for color value");

            if (!TryParse(values[0], out r) ||
                !TryParse(values[1], out g) ||
                !TryParse(values[2], out b))
            {
                r = 0;
                g = 0;
                b = 0;

                return false;
            }

            return true;
        }

        static bool GetHeaderData(string line, string header, out string data)
        {
            if(!line.StartsWith(header))
            {
                data = null;
                return false;
            }

            data = line.Substring(header.Length).Trim();

            return true;
        }

        static bool TryParse(string str, out float value) => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        public void Dispose()
        {
            _stream.Dispose();
            _stream = null;
        }
    }
}
