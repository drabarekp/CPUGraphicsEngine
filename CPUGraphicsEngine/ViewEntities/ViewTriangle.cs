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
            Vector<float> centerPoint4 = (p1.model.worldPosition + p2.model.worldPosition + p3.model.worldPosition) / 3;
            Vector<float> centerPoint = Vector<float>.Build.DenseOfEnumerable(centerPoint4.Take(3));
            Vector<float> lightPosition = lights[0].position;

            //float ndotl = ComputeNDotL(centerPoint, normalFace, lightPosition);
            float ndotl = PhongIntensity(centerPoint, normalFace, presentation.camera.position, lights);
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
            var position1in3d = Vector<float>.Build.DenseOfEnumerable(p1.model.worldPosition.Take(3));
            var position2in3d = Vector<float>.Build.DenseOfEnumerable(p2.model.worldPosition.Take(3));
            var position3in3d = Vector<float>.Build.DenseOfEnumerable(p3.model.worldPosition.Take(3));
            /*
            float nl1 = ComputeNDotL(position1in3d, p1.model.normal, lightPosition);
            float nl2 = ComputeNDotL(position2in3d, p2.model.normal, lightPosition);
            float nl3 = ComputeNDotL(position3in3d, p3.model.normal, lightPosition);*/
            float nl1 = PhongIntensity(position1in3d, p1.model.normal, presentation.camera.position, lights);
            float nl2 = PhongIntensity(position2in3d, p2.model.normal, presentation.camera.position, lights);
            float nl3 = PhongIntensity(position3in3d, p3.model.normal, presentation.camera.position, lights);

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

        void ProcessScanLinePhong(int y, ViewPoint pa, ViewPoint pb, ViewPoint pc, ViewPoint pd,
            BaseColor color, Presentation presentation, List<LightSource> lights)
        {
            if (y < 0 || y >= presentation.height) return;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Y != pb.Y ? (float)(y - pa.Y) / (float)(pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (float)(y - pc.Y) / (float)(pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            //added by me:
            //TO REMOVE: 
            if (sx > ex)
            {
                (sx, ex) = (ex, sx);
                (pa, pc) = (pc, pa);
                (pb, pd) = (pd, pb);
            }

            float z1 = Interpolate(pa.Z, pb.Z, gradient1);
            float z2 = Interpolate(pc.Z, pd.Z, gradient2);

            if (sx < 0) sx = 0;
            if (ex >= presentation.width) ex = presentation.width;

            var normalS = (1 - gradient1) * pa.model.normal + (gradient1) * pb.model.normal;
            var normalE = (1 - gradient2) * pc.model.normal + (gradient2) * pd.model.normal;

            var positionS = (1 - gradient1) * pa.model.worldPosition + (gradient1) * pb.model.worldPosition;
            var positionE = (1 - gradient2) * pc.model.worldPosition + (gradient2) * pd.model.worldPosition;

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var normal = (1 - gradient) * normalS + gradient * normalE;
                var position = (1 - gradient) * positionS + gradient * positionE;

                position = Vector<float>.Build.DenseOfEnumerable(position.Take(3));

                //var ndotl = ComputeNDotL(position, normal, lights[0].position);
                var ndotl = PhongIntensity(position, normal, presentation.camera.position, lights);
                presentation.PutPixel(x, y, z, ndotl * color);
            }
        }
        public void DrawTrianglePhong(BaseColor color, Presentation presentation, List<LightSource> lights)
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

                    if (y < p2.Y)
                    {
                        ProcessScanLinePhong(y, p1, p3, p1, p2, color, presentation, lights);
                    }
                    else
                    {
                        ProcessScanLinePhong(y, p1, p3, p2, p3, color, presentation, lights);
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
                        ProcessScanLinePhong(y, p1, p2, p1, p3, color, presentation, lights);
                    }
                    else
                    {
                        ProcessScanLinePhong(y, p2, p3, p1, p3, color, presentation, lights);
                    }
                }
            }
        }

        private float PhongIntensity(Vector<float> pointPosition, Vector<float> normal, Vector<float> cameraPosition, List<LightSource> lights)
        {
            float kd = 0.5f;
            float ks = 0.5f;
            float sumIntensity = 0.0f;
            int alpha = 16;

            Vector<float> cameraDirection = cameraPosition - pointPosition;
            cameraDirection = cameraDirection.Normalize(2);

            //input vectors are unit vectors
            foreach (var light in lights)
            {
                var lightDirection = light.position - pointPosition;
                lightDirection = lightDirection.Normalize(2);

                Vector<float> R = 2 * lightDirection.DotProduct(normal) * normal - lightDirection;
                R = R.Normalize(2);
                sumIntensity += kd * lightDirection.DotProduct(normal) + ks*MathF.Pow(R.DotProduct(cameraDirection), alpha);
            }

            return Clamp(sumIntensity, 0, 1);
        }
    }
}
