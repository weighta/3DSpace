using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;

namespace _3DSpace
{
    public class World
    {
        public Generator generator;
        public Cube[] cube;
        public Origin origin;
        public Camera camera;
        public World()
        {
            generator = new Generator();
            cube = new Cube[20];
            for (int i = 0; i < cube.Length; i++) { cube[i] = generator.MakeCube(50, i); };
            camera = generator.MakeCamera(new vec3D() { x = 0, y = 0, z = -2000 });
            origin = generator.MakeOrigin(200);
        }
        public void CamTranslate()
        {
            camera.loc.x += camera.x_velocity;
            camera.loc.y += camera.y_velocity;
            camera.loc.z += camera.z_velocity;
        }
        public void CamRotate()
        {
            camera.yaw_deg = ((int)(camera.yaw_deg + camera.yaw_velocity) + 1024) & 1023;
            camera.pitch_deg = ((int)(camera.pitch_deg + camera.pitch_velocity) + 1024) & 1023;
        }
        public void CubeTranslate()
        {
            int border = 1000;
            for (int i = 0; i < cube.Length; i++)
            {
                cube[i].loc.x += cube[i].x_velocity;
                if (cube[i].loc.x < -border || cube[i].loc.x > border) cube[i].x_velocity *= -1;
                cube[i].loc.y += cube[i].y_velocity;
                if (cube[i].loc.y < -border || cube[i].loc.y > border) cube[i].y_velocity *= -1;
                cube[i].loc.z += cube[i].z_velocity;
                if (cube[i].loc.z < -border || cube[i].loc.z > border) cube[i].z_velocity *= -1;
            }
        }
        public void CubeRotate()
        {
            for (int i = 0; i < cube.Length; i++)
            {
                cube[i].yaw_deg = (cube[i].yaw_deg + cube[i].yaw_velocity + 1024) & 1023;
                cube[i].pitch_deg = (cube[i].pitch_deg + cube[i].pitch_velocity + 1024) & 1023;
                cube[i].roll_deg = (cube[i].roll_deg + cube[i].roll_velocity + 1024) & 1023;
            }
        }

        public void SetCameraLookSpeed(double x_vel, double y_vel)
        {
            camera.yaw_velocity = x_vel;
            camera.pitch_velocity = y_vel;
        }
        public void SetCameraMoveSpeed(double x_vel, double y_vel, double z_vel)
        {
            camera.x_velocity = x_vel;
            camera.y_velocity = y_vel;
            camera.z_velocity = z_vel;
        }
        public string GetCameraCoords() { return camera.loc.x + " " + camera.loc.y + " " + camera.loc.z; }
        public string GetCameraLook() { return camera.yaw_deg + " " + camera.pitch_deg + " " + camera.roll_deg; }
        public string GetCubeCoords() { return cube[0].loc.x + " " + cube[0].loc.y + " " + cube[0].loc.z; }
    }
}
