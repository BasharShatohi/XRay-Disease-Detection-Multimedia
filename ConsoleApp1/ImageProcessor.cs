
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageApp
{
    public static class ImageProcessor
    {
        public static Bitmap LoadImageWithoutIndexedFormat(string imagePath)
        {
            using (Bitmap original = new Bitmap(imagePath))
            {
                Bitmap nonIndexed = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(nonIndexed))
                {
                    g.DrawImage(original, 0, 0, original.Width, original.Height);
                }
                return nonIndexed;
            }
        }

        public static bool CompareImages(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    if (image1.GetPixel(x, y) != image2.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
