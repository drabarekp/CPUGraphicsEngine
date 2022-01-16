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

        public BaseColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
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
    }
}
