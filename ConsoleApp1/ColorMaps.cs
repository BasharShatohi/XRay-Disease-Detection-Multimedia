using System;
using System.Drawing;

namespace ImageApp
{
    public static class ColorMaps
    {
        public static Color GetMappedColor(int intensity, string colorMap)
        {
            switch (colorMap)
            {
                case "Cool":
                    return GetCoolColor(intensity);
                case "Hot":
                    return GetHotColor(intensity);
                case "Gray":
                    return GetGrayColor(intensity);
                case "Jet":
                    return GetJetColor(intensity);
                default:
                    return GetDefaultColor(intensity);
            }
        }

        private static Color GetDefaultColor(int intensity)
        {
            if (intensity <= 50)
                return Color.Blue;
            else if (intensity >= 200)
                return Color.Red;
            else
                return Color.Green;
        }

        private static Color GetCoolColor(int intensity)
        {
            int blue = Math.Min(255, intensity * 2);
            return Color.FromArgb(0, 0, blue);
        }

        private static Color GetHotColor(int intensity)
        {
            int red = Math.Min(255, intensity * 2);
            return Color.FromArgb(red, 0, 0);
        }

        private static Color GetGrayColor(int intensity)
        {
            return Color.FromArgb(intensity, intensity, intensity);
        }

        private static Color GetJetColor(int intensity)
        {
            if (intensity <= 85)
                return Color.FromArgb(0, 0, intensity * 3);
            else if (intensity <= 170)
                return Color.FromArgb(0, intensity * 3 - 255, 255 - (intensity - 85) * 3);
            else
                return Color.FromArgb((intensity - 170) * 3, 255 - (intensity - 170) * 3, 0);
        }
    }
}
