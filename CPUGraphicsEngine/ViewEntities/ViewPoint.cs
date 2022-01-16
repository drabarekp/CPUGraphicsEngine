using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Hazdryx.Drawing;
using System.Drawing;

using CPUGraphicsEngine.Models;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class ViewPoint
    {
        public ModelPoint model;
        public int X { get => screenPosition.X; }
        public int Y { get => screenPosition.Y; }
        public float Z { get => position[2]; }

        public Vector<float> position;
        
        public (int X, int Y) screenPosition;
        public ViewPoint(ModelPoint model)
        {
            this.model = model;
        }
        public ViewPoint(Vector<float> position, ModelPoint model)
        {
            position = this.position;
            this.model = model;
        }
        public void UpdateScreenPosition(int width, int height)
        {
            screenPosition = ((int)((width / 2) + (width / 2) * (position[0] / position[3])), (int)((height / 2) + (height / 2) * (position[1] / position[3])));
        }

        public void SetPosition(Vector<float> position)
        {
            this.position = position;
        }

        public void Draw(FastBitmap fastBitmap, int width, int height)
        {
            if (screenPosition.X < 0 || screenPosition.X >= width || screenPosition.Y < 0 || screenPosition.Y >= height)
                return;

            fastBitmap.Set(screenPosition.X, screenPosition.Y, System.Drawing.Color.Red);
        }
        public void Draw(Graphics g, int width, int height)
        {
            var r = new Rectangle(screenPosition.X, screenPosition.Y, 10, 10);
            g.FillEllipse(new SolidBrush(Color.Green), r);
        }
    }
}
