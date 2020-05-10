using System;

namespace MusicStoreKeeper.Model
{
    public static class ImageDataConverter
    {
        public static byte[,] ConvertTo16X16GrayScale(byte[] sourceGrayScale)
        {
            if (sourceGrayScale.Length != 256)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceGrayScale), "Source array size is not equal to 256 bytes.");
            }
            var grayScale16X16 = new byte[16, 16];

            Buffer.BlockCopy(sourceGrayScale, 0, grayScale16X16, 0, grayScale16X16.Length);

            return grayScale16X16;
        }

        public static byte[] ConvertFrom16X16GrayScale(byte[,] source16x16GrayScale)
        {
            if (source16x16GrayScale.Length != 256)
            {
                throw new ArgumentOutOfRangeException(nameof(source16x16GrayScale), "Source array size is not equal to 256 bytes.");
            }
            var grayScaleDb = new byte[256];
            Buffer.BlockCopy(source16x16GrayScale, 0, grayScaleDb, 0, source16x16GrayScale.Length);

            return grayScaleDb;
        }
    }
}