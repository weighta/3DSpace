using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace _3DSpace
{
    public class Generator
    {
        Random ran;
        public Generator()
        {
            ran = new Random();
        }
        public Camera MakeCamera(vec3D loc)
        {
            Camera camera = new Camera()
            {
                loc = loc,
                yaw_deg = 0,
                pitch_deg = 0,
                roll_deg = 0
            };
            return camera;
        }
        public Origin MakeOrigin(int scale)
        {
            Origin origin = new Origin()
            {
                loc = new vec3D() { x=0, y=0, z=0 },
                verts = new vec3D[4] //
                {
                    new vec3D() { x=0,y=0,z=0 },
                    new vec3D() { x=1,y=0,z=0 },
                    new vec3D() { x=0,y=1,z=0 },
                    new vec3D() { x=0,y=0,z=1 },
                },
                edges = new int[6]
                {
                    0,1, 0,2, 0,3
                },
                scale = scale
            };
            origin.verts[1].x *= origin.scale;
            origin.verts[2].y *= origin.scale;
            origin.verts[3].z *= origin.scale;
            return origin;
        }
        public Cube MakeCube(int scale, int color_index)
        {
            Cube cube = new Cube()
            {
                loc = new vec3D() { x = 0, y = 0, z = 0 }, //at origin
                verts = new vec3D[8]
                {
                    new vec3D() { x = -1, y = -1, z = 1 },
                    new vec3D() { x = -1, y = 1, z = 1 },
                    new vec3D() { x = 1, y = -1, z = 1 },
                    new vec3D() { x = 1, y = 1, z = 1 },
                    new vec3D() { x = -1, y = -1, z = -1 },
                    new vec3D() { x = -1, y = 1, z = -1 },
                    new vec3D() { x = 1, y = -1, z = -1 },
                    new vec3D() { x = 1, y = 1, z = -1 },
                },
                edges = new int[24]
                {
                    0, 1, 2, 3, 4, 5, 6, 7,
                    1, 3, 5, 7, 1, 5, 3, 7,
                    0, 2, 4, 6, 0, 4, 2, 6
                },
                color = color_index,
                x_velocity = 0,
                y_velocity = 0,
                z_velocity = 0,
                yaw_velocity = getRanRange(-15,15),
                pitch_velocity = getRanRange(-15, 15),
                roll_velocity = getRanRange(-15, 15),
                yaw_deg = 0,
                pitch_deg = 0,
                roll_deg = 0,
                scale = scale
            };
            CubeApplySpeedRan(ref cube);
            CubeApplyScale(cube);
            return cube;
        }
        public void CubeApplyScale(Cube cube)
        {
            for (int i = 0; i < cube.verts.Length; i++)
            {
                cube.verts[i].x *= cube.scale;
                cube.verts[i].y *= cube.scale;
                cube.verts[i].z *= cube.scale;
            }
        }
        public void CubeApplySpeedRan(ref Cube cube)
        {
            int a = -25;
            int b = 25;
            cube.x_velocity = getRanRange(a, b);
            cube.y_velocity = getRanRange(a, b);
            cube.z_velocity = getRanRange(a, b);
        }
        public int getRanRange(int a, int b)
        {
            return ran.Next(0, b - a) + a;
        }
    }
}
