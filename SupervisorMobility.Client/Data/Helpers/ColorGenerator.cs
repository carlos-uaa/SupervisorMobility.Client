using System.Globalization;

namespace SupervisorMobility.Client.Data.Helpers
{
    public class ColorGenerator
    {
        public static (int R, int G, int B) HexToRgb(string hex)
        {
            hex = hex.Replace("#", "");
            return (
                int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber)
            );
        }

        // --- RGB → HSL ---
        public static (double H, double S, double L) RgbToHsl(int r, int g, int b)
        {
            double rD = r / 255.0, gD = g / 255.0, bD = b / 255.0;
            double max = Math.Max(rD, Math.Max(gD, bD));
            double min = Math.Min(rD, Math.Min(gD, bD));
            double h = 0, s, l = (max + min) / 2;

            double d = max - min;
            if (d == 0) h = 0;
            else if (max == rD) h = (60 * ((gD - bD) / d) + 360) % 360;
            else if (max == gD) h = (60 * ((bD - rD) / d) + 120) % 360;
            else if (max == bD) h = (60 * ((rD - gD) / d) + 240) % 360;

            s = (d == 0) ? 0 : d / (1 - Math.Abs(2 * l - 1));
            return (h, s, l);
        }

        // --- HSL → HEX ---
        public static string HslToHex(double h, double s, double l)
        {
            double C = (1 - Math.Abs(2 * l - 1)) * s;
            double X = C * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - C / 2;
            double r = 0, g = 0, b = 0;

            if (h < 60) { r = C; g = X; b = 0; }
            else if (h < 120) { r = X; g = C; b = 0; }
            else if (h < 180) { r = 0; g = C; b = X; }
            else if (h < 240) { r = 0; g = X; b = C; }
            else if (h < 300) { r = X; g = 0; b = C; }
            else { r = C; g = 0; b = X; }

            int R = (int)((r + m) * 255);
            int G = (int)((g + m) * 255);
            int B = (int)((b + m) * 255);
            return $"#{R:X2}{G:X2}{B:X2}";
        }

        // --- Generate Analogous/Complementary Palette ---
        public static List<string> GeneratePaletteFromBase(string baseHex, int count)
        {
            var (R, G, B) = HexToRgb(baseHex);
            var (H, S, L) = RgbToHsl(R, G, B);

            // Keep saturation slightly reduced for muted colors
            S = Math.Min(S * 0.8, 0.6);
            L = Math.Min(Math.Max(L, 0.4), 0.6);

            var colors = new List<string>();
            double step = 30; // hue step for variation

            for (int i = 0; i < count; i++)
            {
                double newHue = (H + (i * step)) % 360; // shift hue around the base
                colors.Add(HslToHex(newHue, S, L));
            }
            return colors;
        }
    }
}
