using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DSpace
{
    public struct Controls
    {
        public bool isMovingFoward;
        public bool isMovingBackward;
        public bool isMovingUp;
        public bool isMovingDown;
        public bool isMovingLeft;
        public bool isMovingRight;
        public bool isPhysicsPaused;
        public bool isCamZ;
        public bool isMouseMove;
    }
    public partial class Form1 : Form
    {
        Controls controls;
        Game game;

        const int GAME_WIDTH = 640;
        const int GAME_HEIGHT = 480;
        int GAME_WIDTH_HALF;
        int GAME_HEIGHT_HALF;
        const int x_FORM_Div = 16;
        const int y_FORM_Div = 39;
        double CURSOR_PREV_X;
        double CURSOR_PREV_Y;
        bool CURSOR_PREV_USED;
        bool CURSOR_PREV_MOVED;

        public Form1()
        {
            InitializeComponent();
            inst();
        }

        void inst()
        {
            PictureBox pictureBox = new PictureBox() { Width = GAME_WIDTH, Height = GAME_HEIGHT, Location = new Point(0, 0) };
            Controls.Add(pictureBox);
            Width = GAME_WIDTH + x_FORM_Div;
            Height = GAME_HEIGHT + y_FORM_Div;
            GAME_WIDTH_HALF = GAME_WIDTH >> 1;
            GAME_HEIGHT_HALF = GAME_HEIGHT >> 1;

            controls = new Controls();
            game = new Game(controls, pictureBox, GAME_WIDTH, GAME_HEIGHT);

            KeyUp += new KeyEventHandler(KeyReleased);
            KeyDown += new KeyEventHandler(KeyDowned);

            Timer mouseThread = new Timer() { Interval = 50, Enabled = true };
            mouseThread.Tick += mouseThread_Tick;
            mouseThread.Start();
        }
        void KeyDowned(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z) controls.isCamZ = Convert.ToBoolean(Convert.ToInt32(controls.isCamZ) ^ 1);
            if (controls.isCamZ)
            {
                if (e.KeyCode == Keys.W) controls.isMovingFoward = true;
                else if (e.KeyCode == Keys.A) controls.isMovingLeft = true;
                else if (e.KeyCode == Keys.S) controls.isMovingBackward = true;
                else if (e.KeyCode == Keys.D) controls.isMovingRight = true;
                else if (e.KeyCode == Keys.ControlKey) controls.isMovingDown = true;
                else if (e.KeyCode == Keys.Space) controls.isMovingUp = true;
                else if (e.KeyCode == Keys.P) controls.isPhysicsPaused = Convert.ToBoolean(Convert.ToInt32(controls.isPhysicsPaused) ^ 1);
                game.controls = controls;
            }

        }
        void KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) controls.isMovingFoward = false;
            else if (e.KeyCode == Keys.A) controls.isMovingLeft = false;
            else if (e.KeyCode == Keys.S) controls.isMovingBackward = false;
            else if (e.KeyCode == Keys.D) controls.isMovingRight = false;
            else if (e.KeyCode == Keys.ControlKey) controls.isMovingDown = false;
            else if (e.KeyCode == Keys.Space) controls.isMovingUp = false;
            game.controls = controls;
        }
        void mouseThread_Tick(object sender, EventArgs e)
        {
            //controls.isCamZ = true;
            if (controls.isCamZ)
            {
                double X = Cursor.Position.X;
                double Y = Cursor.Position.Y;
                double cursor_origin_X = Location.X + GAME_WIDTH_HALF;
                double cursor_origin_Y = Location.Y + GAME_HEIGHT_HALF;
                double DiffX = X - CURSOR_PREV_X;
                double DiffY = Y - CURSOR_PREV_Y;
                CURSOR_PREV_X = X;
                CURSOR_PREV_Y = Y;
                if (!CURSOR_PREV_USED) { DiffX = 0; DiffY = 0; CURSOR_PREV_X = cursor_origin_X; CURSOR_PREV_Y = cursor_origin_Y; SetCursorPos((int)cursor_origin_X, (int)cursor_origin_Y); }
                if (CURSOR_PREV_MOVED) { DiffX = 0; DiffY = 0; }
                else if (DiffX == 0 && DiffY == 0 && controls.isMouseMove) { controls.isMouseMove = false; game.SetCameraLook(0, 0); }
                CURSOR_PREV_USED = true;

                if (DiffX != 0 || DiffY != 0)
                {
                    SetCursorPos((int)cursor_origin_X, (int)cursor_origin_Y);
                    CURSOR_PREV_MOVED = true;
                    game.SetCameraLook(DiffX, DiffY);
                    controls.isMouseMove = true;
                }
                else { CURSOR_PREV_MOVED = false; }
                
                //Console.WriteLine(DiffX + " " + DiffY);
            }
            else { CURSOR_PREV_USED = false; if (controls.isMouseMove) { game.SetCameraLook(0, 0); controls.isMouseMove = false; } }
        }

        /// <summary>
        /// Stack overflow
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SetCursorPosition(int a, int b)
        {
            SetCursorPos(a, b); //https://stackoverflow.com/questions/8185916/set-mouse-position-in-wpf
        }
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
    }
}
