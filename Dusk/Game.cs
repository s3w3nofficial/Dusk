using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JeremyAnsel.Media.WavefrontObj;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Dusk
{
    class Start
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(800, 600))
            {
                game.Run(20, 60);
            }
        }
    }

    class Game : GameWindow
    {
        public static Game Instance;
        public Camera Camera;

        private BakedModel gun;

        private Point _mouseLast;

        public Game(int width, int height) : base(width, height)
        {
            Instance = this;
            Camera = new Camera();

            gun = ModelLoader.BakeModel(new Shader("lit"), "gun");

            Init();
        }

        public void Init()
        {
            OnResize(null);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ActiveTexture(TextureUnit.Texture0);
        }
        
        private void MoveCamera(double deltaTime)
        {
            if (!Focused)
                return;

            var current = new Point(Mouse.GetState().X, Mouse.GetState().Y);

            var deltaX = _mouseLast.X - current.X;
            var deltaY = _mouseLast.Y - current.Y;

            var centerLocal = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
            var centerScreen = PointToScreen(centerLocal);

            _mouseLast = current;

            //Mouse.SetPosition(centerScreen.X, centerScreen.Y);
            
            Camera.Pitch -= deltaY / 1000f;
            Camera.Yaw -= deltaX / 1000f;

            var dirVec = Vector2.Zero;
            var state = Keyboard.GetState();

            bool w = state.IsKeyDown(Key.W); //might use this later
            bool s = state.IsKeyDown(Key.S);
            bool a = state.IsKeyDown(Key.A);
            bool d = state.IsKeyDown(Key.D);

            if (w) dirVec += Camera.Forward;
            if (s) dirVec += -Camera.Forward;
            if (a) dirVec += Camera.Left;
            if (d) dirVec += -Camera.Left;

            if (dirVec.LengthFast < 0.5f)
                return;

            dirVec = Vector2.Normalize(dirVec);

            Camera.Pos.Xz += dirVec * (float)deltaTime * 4;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            MoveCamera(e.Time);

            CursorVisible = !Focused;

            Camera.UpdateViewMatrix();

            gun.Bind();
            gun.Shader.SetMatrix4("transformationMatrix", Matrix4.Identity);
            gun.Render();
            gun.Unbind();
            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F4 && e.Alt)
                Exit();
            if (e.Key == Key.Escape)
                WindowState = WindowState.Minimized;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            Camera.UpdateProjectionMatrix();
        }
    }

    class Player
    {
        private int _maxHealth;
        private int _maxSpeed;

        public Player(int maxHealth, int maxSpeed)
        {
            _maxHealth = maxHealth;
            _maxSpeed = maxSpeed;
        }
    }
}
