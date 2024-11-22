using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace _3DSpace
{
    public struct vec3D
    {
        public double x, y, z;
    }
    public struct vec2D
    {
        public double x, y;
    }
    public struct Cube
    {
        public vec3D loc;
        public vec3D[] verts;
        public int[] edges;
        public int color;
        public double x_velocity;
        public double y_velocity;
        public double z_velocity;
        public int yaw_velocity;
        public int pitch_velocity;
        public int roll_velocity;
        public int yaw_deg;
        public int pitch_deg;
        public int roll_deg;
        public double scale;
    }
    public struct Origin
    {
        public vec3D loc;
        public vec3D[] verts;
        public int[] edges;
        public int scale;
    }
    public struct Camera
    {
        public vec3D loc;
        public double yaw_velocity;
        public double pitch_velocity;
        public double roll_velocity;
        public int yaw_deg;
        public int pitch_deg;
        public int roll_deg;
        public double x_velocity;
        public double y_velocity;
        public double z_velocity;
    }

    public class Game
    {
        Stopwatch Tick_watch;
        const int TICKS_PER_SECOND = 60;
        const int TICK_CheckInterval = 1;
        private int TICKS_AWAIT_TIME_MS;
        private double TICK_free_ms;

        private double FPS;

        Graphics g_Stream;
        Graphics g_Buffer;
        GraphicsMath graphicsMath;
        Pen[] linePen;
        Font fontDebug;
        RectangleF FPS_screen_loc;
        RectangleF CAM_screen_loc;
        RectangleF CUBE_screen_loc;
        RectangleF CAM_screen_loc1;
        Bitmap bitmapStream;
        Bitmap bitmapBuffer;
        const int DEPTH = 300;
        PictureBox pictureBox1;
        private bool isPaused;
        private bool gameProc;
        private int width;
        private int height;
        private int width_half;
        private int height_half;

        Thread game_thread;
        Thread gameTick_thread;
        Thread gameImg_thread;
        const int Img_refresh_rate = 60;
        public Controls controls;

        public World world;
        vec3D[] verts_cam;
        vec2D[] screenDraw;

        public Game(Controls controls, PictureBox pictureBox1, int width, int height)
        {
            this.pictureBox1 = pictureBox1;
            this.width = width;
            this.height = height;
            width_half = width >> 1;
            height_half = height >> 1;

            this.controls = controls;
            bitmapStream = new Bitmap(width, height);
            bitmapBuffer = new Bitmap(width, height);
            g_Stream = Graphics.FromImage(bitmapStream);
            g_Buffer = Graphics.FromImage(bitmapBuffer);
            graphicsMath = new GraphicsMath();
            linePen = new Pen[8] {new Pen(Color.White, 1), new Pen(Color.Red, 1), new Pen(Color.Green, 1), new Pen(Color.Blue, 1), new Pen(Color.Yellow, 1), new Pen(Color.Violet, 1), new Pen(Color.Turquoise, 1), new Pen(Color.Orange)};
            screenDraw = new vec2D[8];
            verts_cam = new vec3D[8];
            Tick_watch = new Stopwatch();
            world = new World();
            fontDebug = new Font("Arial", 12, FontStyle.Bold);
            FPS_screen_loc = new RectangleF(10, 300, 0, 0);
            CAM_screen_loc = new RectangleF(10, 320, 0, 0);
            CUBE_screen_loc = new RectangleF(10, 340, 0, 0);
            CAM_screen_loc1 = new RectangleF(10, 360, 0, 0);
            isPaused = false;
            game_thread = new Thread(new ParameterizedThreadStart(game_Thread));
            gameTick_thread = new Thread(new ParameterizedThreadStart(gameTick_Thread));
            gameImg_thread = new Thread(new ParameterizedThreadStart(gameImg_Thread));
            game_thread.Start();
            gameTick_thread.Start();
            gameImg_thread.Start();
        }
        void game_Thread(object x)
        {
            int CPU_PAUSED_CHECK_INTERVAL = 100;
            TICK_free_ms = (1000 / TICKS_PER_SECOND) - TICK_CheckInterval;

            while (true)
            {
                while (!isPaused)
                {
                    if (gameProc) //Proceed with Game_Tick()
                    {
                        gameProc = false;
                        Tick_watch.Reset();
                        Tick_watch.Start();
                        Game_(); //Tick
                        Tick_watch.Stop();
                        Draw(Tick_watch.Elapsed.TotalMilliseconds);
                        if (gameProc) MessageBox.Show("System Behind!");
                    }
                    else Thread.Sleep(TICK_CheckInterval);

                }
                Thread.Sleep(CPU_PAUSED_CHECK_INTERVAL);
            }
        }
        void gameTick_Thread(object x)
        {
            TICKS_AWAIT_TIME_MS = 1000 / TICKS_PER_SECOND;
            while (true)
            {
                gameProc = true;
                Thread.Sleep(TICKS_AWAIT_TIME_MS);
            }
        }
        void gameImg_Thread(object x)
        {
            int REFRESH_RATE = 1000 / Img_refresh_rate;
            while (true)
            {
                pictureBox1.Image = bitmapBuffer;
                //Thread.Sleep(1);
                Thread.Sleep(REFRESH_RATE);
            }
        }
        void Game_()
        {
            CamMovement();
            world.CamTranslate();
            world.CamRotate();
            if (controls.isPhysicsPaused)
            {
                world.CubeTranslate();
                world.CubeRotate();
            }
        }
        void Draw(double gameTime)
        {
            Tick_watch.Reset();
            Tick_watch.Start();
            _Draw(0);
            Tick_watch.Stop();
            double drawTime = Tick_watch.Elapsed.TotalMilliseconds;
            double maxDraws = (TICK_free_ms - gameTime) / drawTime;
            int framesPerTick = (int)maxDraws - 20; //Can try -1 or -99 to make game not behind
            double inc = 1.0 / framesPerTick;
            for (int i = 1; i < framesPerTick; i++)
            {
                _Draw(i * inc);
            }
            FPS = maxDraws * TICKS_PER_SECOND;
        }
        void _Draw(double perc)
        {
            g_Stream.FillRectangle(Brushes.Black, 0, 0, width, height); //Draw black

            _DrawCubes(perc);
            _DrawOrigin(perc);

            g_Stream.DrawString("FPS: " + FPS, fontDebug, Brushes.White, FPS_screen_loc, null);
            g_Stream.DrawString("CAM: " + world.GetCameraLook(), fontDebug, Brushes.White, CAM_screen_loc, null);
            //g_Stream.DrawString("CUBE: " + world.GetCubeCoords(), fontDebug, Brushes.White, CUBE_screen_loc, null);
            g_Stream.DrawString("CAM_LOC: " + world.GetCameraCoords(), fontDebug, Brushes.White, CAM_screen_loc1, null);
            g_Buffer.DrawImage(bitmapStream, 0, 0);
        }
        void _DrawCubes(double perc)
        {
            for (int i = 0; i < world.cube.Length; i++) 
            {
                _SetVertsCam(world.cube[i].verts); //Use the required verticies of cube
                graphicsMath.rot(   //Rotate cubes (at their origin)
                    (world.cube[i].yaw_deg + (int)(perc * world.cube[i].yaw_velocity) + 1024) & 1023,
                    (world.cube[i].pitch_deg + (int)(perc * world.cube[i].pitch_velocity) + 1024) & 1023,
                    (world.cube[i].roll_deg + (int)(perc * world.cube[i].roll_velocity) + 1024) & 1023,
                    verts_cam);
                graphicsMath.trans( //Translate cubes (relative to camera and their world space)
                    world.camera.loc.x + world.cube[i].loc.x + (perc * world.camera.x_velocity),
                    world.camera.loc.y + world.cube[i].loc.y + (perc * world.camera.y_velocity),
                    world.camera.loc.z + world.cube[i].loc.z + (perc * world.camera.z_velocity),
                    verts_cam
                    );
                graphicsMath.rot(   //Rotate cubes (around camera origin)
                    (world.camera.pitch_deg + (int)(perc * world.camera.pitch_velocity) + 1024) & 1023,
                    (-world.camera.yaw_deg + (int)(perc * world.camera.yaw_velocity) + 1024) & 1023,
                    (world.camera.roll_deg + (int)(perc * world.camera.roll_velocity) + 1024) & 1023,
                    verts_cam
                    );
                graphicsMath.ConvertToScreen(verts_cam, screenDraw, DEPTH, width_half, height_half); //Convert the cube to screen coordinates
                if (_inframe(screenDraw, 8))
                {
                    for (int j = 0; j < world.cube[i].edges.Length; j += 2)
                    {
                        g_Stream.DrawLine(
                            linePen[world.cube[i].color & 7],
                            (int)screenDraw[world.cube[i].edges[j]].x,
                            (int)screenDraw[world.cube[i].edges[j]].y,
                            (int)screenDraw[world.cube[i].edges[j + 1]].x,
                            (int)screenDraw[world.cube[i].edges[j + 1]].y
                            );
                    }
                }
            }
        }
        void _DrawOrigin(double perc)
        {
            _SetVertsCam(world.origin.verts); //Use the required verticies of cube
            graphicsMath.trans( //Translate origin (relative to camera and their world space)
                world.camera.loc.x + world.origin.loc.x + (perc * world.camera.x_velocity),
                world.camera.loc.y + world.origin.loc.y + (perc * world.camera.y_velocity),
                world.camera.loc.z + world.origin.loc.z + (perc * world.camera.z_velocity),
                verts_cam
                );
            graphicsMath.rot(   //Rotate origin (around camera origin)
                (world.camera.pitch_deg + (int)(perc * world.camera.pitch_velocity) + 1024) & 1023,
                (-world.camera.yaw_deg + (int)(perc * world.camera.yaw_velocity) + 1024) & 1023,
                (world.camera.roll_deg + (int)(perc * world.camera.roll_velocity) + 1024) & 1023,
                verts_cam
                );
            graphicsMath.ConvertToScreen(verts_cam, screenDraw, DEPTH, width_half, height_half); //Convert the cube to screen coordinates
            if (_inframe(screenDraw, 4))
            {
                for (int j = 0; j < world.origin.edges.Length; j += 2)
                {
                    g_Stream.DrawLine(
                        linePen[(1 + (j >> 1)) & 7],
                        (int)screenDraw[world.origin.edges[j]].x,
                        (int)screenDraw[world.origin.edges[j]].y,
                        (int)screenDraw[world.origin.edges[j + 1]].x,
                        (int)screenDraw[world.origin.edges[j + 1]].y
                        );
                }
            }
        }
        void _SetVertsCam(vec3D[] verts)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                verts_cam[i].x = verts[i].x;
                verts_cam[i].y = verts[i].y;
                verts_cam[i].z = verts[i].z;
            }
        }
        bool _inframe(vec2D[] screenPoints, int len)
        {
            return true;
            for (int i = 0; i < len; i++)
            {
                if (screenPoints[i].x < 0 || screenPoints[i].x > width) return false;
                if (screenPoints[i].y < 0 || screenPoints[i].y > height) return false;
            }
            return true;
        }
        public void SetCameraLook(double x_vel, double y_vel)
        {
            //Console.WriteLine("Camera Speed: " + x_vel + " " + y_vel);
            world.SetCameraLookSpeed(x_vel, y_vel);
        }
        public void CamMovement()
        {
            int max_speed = 50;
            int x_vel = 0;
            int y_vel = 0;
            int z_vel = 0;
            if ((Convert.ToInt32(controls.isMovingFoward) ^ Convert.ToInt32(controls.isMovingBackward)) == 1)
            {
                if (controls.isMovingFoward) z_vel = max_speed;
                else z_vel = -max_speed;
            }
            if ((Convert.ToInt32(controls.isMovingLeft) ^ Convert.ToInt32(controls.isMovingRight)) == 1)
            {
                if (controls.isMovingLeft)
                {
                    x_vel = (int)(-max_speed * graphicsMath.cos(world.camera.yaw_deg));
                    z_vel = (int)(max_speed * graphicsMath.sin(world.camera.yaw_deg));
                    //z_vel = 
                }
                else
                {
                    x_vel = (int)(max_speed * graphicsMath.cos(world.camera.yaw_deg));
                    z_vel = (int)(-max_speed * graphicsMath.sin(world.camera.yaw_deg));
                }
            }
            if ((Convert.ToInt32(controls.isMovingUp) ^ Convert.ToInt32(controls.isMovingDown)) == 1)
            {
                if (controls.isMovingUp) y_vel = -max_speed;
                else y_vel = max_speed;
            }
            world.SetCameraMoveSpeed(x_vel, y_vel, z_vel);
        }
    }
}
