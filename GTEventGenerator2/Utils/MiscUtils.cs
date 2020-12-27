using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GTEventGenerator.Utils
{
    public static class MiscUtils
    {
        public static Image ResizeImage(Image imgToResize, Size destinationSize)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public static byte[] ZlibCompress(byte[] input)
        {
            using (var ms = new MemoryStream(input.Length))
            using (var bs = new BinaryStream(ms))
            {
                bs.WriteUInt32(0xFFF7EEC5);
                bs.WriteInt32(-input.Length);

                var d = new Deflater(Deflater.DEFAULT_COMPRESSION, true);
                d.SetInput(input);
                d.Finish();

                byte[] output = new byte[input.Length];
                int count = d.Deflate(output);
                bs.Write(output, 0, count);
                return ms.ToArray();
            }
        }

        public static byte[] Deflate(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var bs = new BinaryStream(ms))
            {
                bs.ReadUInt32(); // Magic
                int outSize = -bs.ReadInt32();

                byte[] deflatedData = new byte[outSize];
                using (var ds = new DeflateStream(bs, CompressionMode.Decompress))
                    ds.Read(deflatedData, 0, deflatedData.Length);
                
                return deflatedData;   
            }
        }
    }
}
