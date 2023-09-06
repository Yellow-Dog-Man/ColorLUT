using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ColorLUT.CUBE
{
    public class CUBE_Writer : CUBE_Base, IDisposable
    {
        public string Title { get; set; }
        public int Dimensions { get; set; }
        public int Size { get; set; }

        public float MinValueR { get; set; } = 0f;
        public float MinValueG { get; set; } = 0f;
        public float MinValueB { get; set; } = 0f;
        public float MaxValueR { get; set; } = 1f;
        public float MaxValueG { get; set; } = 1f;
        public float MaxValueB { get; set; } = 1f;

        public float MinValue { set => MinValueR = MinValueG = MinValueB = value; }
        public float MaxValue { set => MaxValueR = MaxValueG = MaxValueB = value; }

        public float DomainMinR { get; set; } = 0f;
        public float DomainMinG { get; set; } = 0f;
        public float DomainMinB { get; set; } = 0f;

        public float DomainMaxR { get; set; } = 1f;
        public float DomainMaxG { get; set; } = 1f;
        public float DomainMaxB { get; set; } = 1f;

        public bool NeedsMoreValues => !WrittenAllValues;
        public bool WrittenAllValues => ReachedEnd(Dimensions, Size);

        StreamWriter _stream;
        bool _headerWritten;

        public CUBE_Writer(StreamWriter stream)
        {
            _stream = stream;
        }

        public CUBE_Writer(string filePath)
        {
            _stream = new StreamWriter(File.OpenWrite(filePath), Encoding.UTF8);
        }

        public void WriteHeader()
        {
            if (_headerWritten)
                throw new InvalidOperationException("Header is already written!");

            if(!string.IsNullOrWhiteSpace(Title))
                _stream.WriteLine($"TITLE \"{Title}\"");

            switch(Dimensions)
            {
                case 1:
                    _stream.WriteLine($"LUT_1D_SIZE {Size}");
                    break;

                case 3:
                    _stream.WriteLine($"LUT_3D_SIZE {Size}");
                    break;

                default:
                    throw new InvalidOperationException($"Invalid dimensions: {Dimensions}");
            }

            _stream.WriteLine(FormattableString.Invariant($"DOMAIN_MIN {DomainMinR} {DomainMinG} {DomainMinB}"));
            _stream.WriteLine(FormattableString.Invariant($"DOMAIN_MAX {DomainMaxR} {DomainMaxG} {DomainMaxB}"));

            _headerWritten = true;
        }

        public void WriteColor(float r, float g, float b)
        {
            if (!_headerWritten)
                throw new InvalidOperationException("Header must be written first!");

            if (WrittenAllValues)
                throw new InvalidOperationException("All values have been written!");

            _stream.WriteLine(FormattableString.Invariant($"{r} {g} {b}"));

            IncrementPosition(Dimensions, Size);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _stream = null;
        }
    }
}
