using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CPUGraphicsEngine
{
    internal struct BaseColor
    {
        float r;
        float g;
        float b;

        public int R { get => (int)(255 * r); }
        public int G { get => (int)(255 * g); }
        public int B { get => (int)(255 * b); }
        public int A { get => (int)(255); }

        public BaseColor(float rInDecimal, float gInDecimal, float bInDecimal)
        {
            this.r = rInDecimal;
            this.g = gInDecimal;
            this.b = bInDecimal;
        }
        public BaseColor(int R, int G, int B, bool integers)
        {
            this.r = (float)R / 255.0f;
            this.g = (float)G / 255.0f;
            this.b = (float)B / 255.0f;
        }

        public Color GetColor()
        {
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }
        public static BaseColor operator * (float a, BaseColor baseColor)
        {
            var result = new BaseColor() { r = a * baseColor.r, g = a * baseColor.g, b = a * baseColor.b };
            return result;
        }

        public void ApplyFog(float z)
        {
            if (z < 5) z = 0.0f;
            else z = (z - 5.0f) / 25.0f;

            r = r + (1.0f - r) * z;
            g = g + (1.0f - g) * z;
            b = b + (1.0f - b) * z;
        }

        public void Clamp()
        {
            if (r > 1.0f) r = 1.0f;
            if (g > 1.0f) g = 1.0f;
            if(b > 1.0f) b = 1.0f;
        }
    }
}
