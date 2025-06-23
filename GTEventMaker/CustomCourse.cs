using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Buffers.Binary;
using Syroot.BinaryData;

using GTEventMaker.Utils;
using PDTools.Utils;
using PDTools.Structures;
using PDTools.Compression;

namespace GTEventMaker;

public class CustomCourse
{
    public SceneryType Scenery { get; set; }
    public float RoadWidth { get; set; }
    public float StartPoint { get; set; }
    public DateTime Time { get; set; }
    public bool IsCircuit { get; set; }
    public float HomeStraightLength { get; set; }
    public float ElevationDifference { get; set; }
    public int CornerCount { get; set; }
    public float FinishLine { get; set; }
    public float StartLine { get; set; }

    public byte[] Data { get; set; }

    public static CustomCourse FromBase64File(string path)
    {
        string bytes = File.ReadAllText(path);
        return FromBase64(bytes);
    }

    public static CustomCourse FromBase64(string b64)
    {
        /*
        int magicIsh = BinaryPrimitives.ReadInt32LittleEndian(b64);
        if (magicIsh != 0x33376578)
            throw new FileFormatException("Not a valid compressed Base64 file.");
        */
        byte[] decoded = Convert.FromBase64String(b64);

        return Read(decoded);
    }

    public static CustomCourse FromTED(string path)
    {
        byte[] ted = File.ReadAllBytes(path);
        return Read(ted);
    }


    public static CustomCourse Read(byte[] bytes)
    {
        var course = new CustomCourse();

        if (BinaryPrimitives.ReadUInt32LittleEndian(bytes) == 0xFFF7EEC5)
            bytes = PS2ZIP.Deflate(bytes);

        course.Data = bytes;
        using (var bs = new BinaryStream(new MemoryStream(bytes), ByteConverter.Big))
        {
            var magic = bs.ReadString(6);
            if (magic != "GT6TED")
                throw new FileFormatException($"Not a valid Custom Track File. (Magic needed is GT6TED, got {magic})");
            bs.Position += 2;

            bs.Position += 4;

            course.Scenery = (SceneryType)bs.ReadInt32();
            course.RoadWidth = bs.ReadSingle();
            bs.Position += 8;
            course.StartPoint = bs.ReadSingle();

            var time = new PDIDATETIME32();
            time.SetRawData(bs.ReadUInt32());
            course.Time = time.GetDateTime();
            course.IsCircuit = bs.ReadBoolean(BooleanCoding.Dword);
            bs.Position += 8;

            course.HomeStraightLength = bs.ReadSingle();
            course.ElevationDifference = bs.ReadSingle();
            course.CornerCount = bs.ReadInt32();
            course.FinishLine = bs.ReadSingle();
            course.StartLine = bs.ReadSingle();

            return course;
        }

    }

    private static readonly byte[] k = {
        0x45, 0x32, 0x35, 0x67, 0x65, 0x69, 0x72, 0x45, 0x50, 0x48, 0x70, 0x63,
        0x34, 0x57, 0x47, 0x32, 0x46, 0x6E, 0x7A, 0x61, 0x63, 0x4D, 0x71, 0x72, 0x75
    };

    public static bool Decrypt(string path)
    {
        byte[] src = File.ReadAllBytes(path);


        if (Encoding.ASCII.GetString(src, 0, 6) == "GT6TED")
            return false;
        else if (src.AsSpan(0, 4).SequenceEqual(new byte[] { 0xC5, 0xEE, 0xF7, 0xFF }))
        {
            src = PS2ZIP.Inflate(src);
            File.WriteAllBytes(path, src);
            return true;
        }
        else
        {
            int j = 1;
            for (int i = 0; i < src.Length; i++)
            {
                src[i] ^= k[j++ - 1];
                if (j > k.Length)
                    j = 1;
            }

            if (src.AsSpan(0, 4).SequenceEqual(new byte[] { 0xC5, 0xEE, 0xF7, 0xFF }))
                src = PS2ZIP.Inflate(src);

            File.WriteAllBytes(path, src);
            return true;
        }
    }

    public static bool Encrypt(string path)
    {
        byte[] src = File.ReadAllBytes(path);

        if (Encoding.ASCII.GetString(src, 0, 6) != "GT6TED")
            return false;
        else
        {
            src = PS2ZIP.Deflate(src);
            int j = 1;
            for (int i = 0; i < src.Length; i++)
            {
                src[i] ^= k[j++ - 1];
                if (j > k.Length)
                    j = 1;
            }

            File.WriteAllBytes(path, src);
            return true;
        }
    }
}

public enum SceneryType
{
    Death_Valley = 1,
    Eifel = 2,
    Andalusia = 3,
    Eifel_Flat = 5,
}
