using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Hazdryx.Drawing;

namespace CPUGraphicsEngine.ViewEntities
{
    internal class ViewTriangle
    {

        ViewPoint p1;
        ViewPoint p2;
        ViewPoint p3;

        public BaseColor baseColor = new BaseColor(200, 200, 200, false);

        public ViewTriangle(ViewPoint p1, ViewPoint p2, ViewPoint p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        // Interpolating the value between 2 vertices 
        // min is the starting point, max the ending point
        // and gradient the % between the 2 points
        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        void ProcessScanLineFlat(ScanLineData data, ViewPoint pa, ViewPoint pb, ViewPoint pc, ViewPoint pd, BaseColor color, Presentation presentation)
        {
            if (data.currentY < 0 || data.currentY >= presentation.height) return;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Y != pb.Y ? (float)(data.currentY - pa.Y) / (float)(pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (float)(data.currentY - pc.Y) / (float)(pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            //added by me:
            //TO REMOVE: 
            if (sx > ex)
            {
                (sx, ex) = (ex, sx);
            }

            float z1 = Interpolate(pa.Z, pb.Z, gradient1);
            float z2 = Interpolate(pc.Z, pd.Z, gradient2);

            if (sx < 0) sx = 0;
            if (ex >= presentation.width) ex = presentation.width;
            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var ndotl = data.ndotla;
                if (ndotl > 0)
                {
                    ;
                }
                presentation.PutPixel(x, data.currentY, z, ndotl*color);
            }
        }

        public void DrawTriangleFlat(BaseColor color, Presentation presentation, List<LightSource> lights)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            Vector<float> normalFace = (p1.model.normal + p2.model.normal + p3.model.normal) / 3;
            Vector<float> centerPoint4 = (p1.model.position + p2.model.position + p3.model.position) / 3;
            Vector<float> centerPoint = Vector<float>.Build.DenseOfEnumerable(centerPoint4.Take(3));
            Vector<float> lightPosition = lights[0].position;

            float ndotl = ComputeNDotL(centerPoint, normalFace, lightPosition);
            var data = new ScanLineData { ndotla = ndotl };

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (float)(p2.X - p1.X) / (float)(p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (float)(p3.X - p1.X) / (float)(p3.Y - p1.Y);
            else
                dP1P3 = 0;

            //DEBUG:

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        ProcessScanLineFlat(data, p1, p3, p1, p2, color, presentation);
                    }
                    else
                    {
                        ProcessScanLineFlat(data, p1, p3, p2, p3, color, presentation);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;
                    if (y < p2.Y)
                    {
                        ProcessScanLineFlat(data, p1, p2, p1, p3, color, presentation);
                    }
                    else
                    {
                        ProcessScanLineFlat(data, p2, p3, p1, p3, color, presentation);
                    }
                }
            }
        }
        float ComputeNDotL(Vector<float> pointPosition, Vector<float> faceNormal, Vector<float> lightPosition)
        {
            var lightDirection = lightPosition - pointPosition;

            lightDirection = lightDirection.Normalize(2);

            var dot = faceNormal.DotProduct(lightDirection);
            return Math.Max(0, dot);
        }

        void ProcessScanLineGouraud(ScanLineData data, ViewPoint pa, ViewPoint pb, ViewPoint pc, ViewPoint pd, BaseColor color, Presentation presentation)
        {
            if (data.currentY < 0 || data.currentY >= presentation.height) return;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Y != pb.Y ? (float)(data.currentY - pa.Y) / (float)(pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (float)(data.currentY - pc.Y) / (float)(pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            //added by me:
            

            //DEBUG
            if(data.ndotla != 0)
            {
                ;
            }

            float z1 = Interpolate(pa.Z, pb.Z, gradient1);
            float z2 = Interpolate(pc.Z, pd.Z, gradient2);

            var snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            var enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            if (sx > ex)
            {
                (sx, ex) = (ex, sx);
                (snl, enl) = (enl, snl); //THIS IS HOTFIX, TO REMOVE IN THE FUTURE
            }


            if (sx < 0) sx = 0;
            if (ex >= presentation.width) ex = presentation.width;

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var ndotl = Interpolate(snl, enl, gradient);
                if (ndotl > 0)
                {
                    ;
                }

                presentation.PutPixel(x, data.currentY, z, ndotl * color);
            }
        }
        public void DrawTriangleGouraud(BaseColor color, Presentation presentation, List<LightSource> lights)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            Vector<float> lightPosition = lights[0].position;
            var position1in3d = Vector<float>.Build.DenseOfEnumerable(p1.model.position.Take(3));
            var position2in3d = Vector<float>.Build.DenseOfEnumerable(p2.model.position.Take(3));
            var position3in3d = Vector<float>.Build.DenseOfEnumerable(p3.model.position.Take(3));

            float nl1 = ComputeNDotL(position1in3d, p1.model.normal, lightPosition);
            float nl2 = ComputeNDotL(position2in3d, p2.model.normal, lightPosition);
            float nl3 = ComputeNDotL(position3in3d, p3.model.normal, lightPosition);

            //DEBUG
            if (nl1 != 0)
            {
                ;
            }

            //float ndotl = ComputeNDotL(centerPoint, normalFace, lightPosition);
            var data = new ScanLineData();

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (float)(p2.X - p1.X) / (float)(p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (float)(p3.X - p1.X) / (float)(p3.Y - p1.Y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;
                        ProcessScanLineGouraud(data, p1, p3, p1, p2, color, presentation);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;
                        ProcessScanLineGouraud(data, p1, p3, p2, p3, color, presentation);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;
                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLineGouraud(data, p1, p2, p1, p3, color, presentation);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLineGouraud(data, p2, p3, p1, p3, color, presentation);
                    }
                }
            }
        }
    }
}
