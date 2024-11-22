using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DSpace
{
    public class GraphicsMath
    {

        double[] sinFromDeg = new double[1024]; //Precision globally understood as 1024 units between 0-360 degrees
        double[] cosFromDeg = new double[1024];

        public GraphicsMath()
        {
            for (int i = 0; i < sinFromDeg.Length; i++)
            {
                double rad = (2 * Math.PI) * (i / (double)sinFromDeg.Length);
                sinFromDeg[i] = Math.Sin(rad);
                cosFromDeg[i] = Math.Cos(rad);
            }
        }
        public double sin(int d) { return sinFromDeg[d]; }
        public double cos(int d) { return cosFromDeg[d]; }

        public void rot(int yawAmount, int pitchAmount, int rollAmount, vec3D[] vec3s)
        {
            //a = roll; B = pitch; y = yaw 
            double sinRoll = sin(rollAmount);
            double cosRoll = cos(rollAmount);
            double sinPitch = sin(pitchAmount);
            double cosPitch = cos(pitchAmount);
            double sinYaw = sin(yawAmount);
            double cosYaw = cos(yawAmount);

            double x_ = cosRoll * cosPitch;
            double xx = (cosRoll * sinPitch * sinYaw) - (sinRoll * cosYaw);
            double xxx = (cosRoll * sinPitch * cosYaw) + (sinRoll * sinYaw);
            double xy = sinRoll * cosPitch;
            double xxy = (sinRoll * sinPitch * sinYaw) + (cosRoll * cosYaw);
            double xxxy = (sinRoll * sinPitch * cosYaw) - (cosRoll * sinYaw);
            double xyy = -sinPitch;
            double xxyy = cosPitch * sinYaw;
            double xxxyy = cosPitch * cosYaw;

            for (int i = 0; i < vec3s.Length; i++)
            {
                double x = vec3s[i].x;
                double y = vec3s[i].y;
                double z = vec3s[i].z;

                vec3s[i].x = (x_ * x) + (xx * y) + (xxx * z);
                vec3s[i].y = (xy * x) + (xxy * y) + (xxxy * z);
                vec3s[i].z = (xyy * x) + (xxyy * y) + (xxxyy * z);
            }
        }
        public void trans(double x, double y, double z, vec3D[] vec3s)
        {
            for (int i = 0; i < vec3s.Length; i++)
            {
                vec3s[i].x += x;
                vec3s[i].y += y;
                vec3s[i].z += z;
            }
        }
        public void ConvertToScreen(vec3D[] vec3s, vec2D[] returnToScreen, double DEPTH, int xCenter, int yCenter)
        {
            for (int i = 0; i < vec3s.Length; i++)
            {
                double scaleProjected = DEPTH / (DEPTH + vec3s[i].z);
                returnToScreen[i].x = (int)((vec3s[i].x * scaleProjected) + xCenter);
                returnToScreen[i].y = (int)((vec3s[i].y * scaleProjected) + yCenter);
            }
        }
    }
}
