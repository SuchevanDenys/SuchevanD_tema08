using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Main
{

    internal class Window3D : GameWindow
    {

        const float rotation_speed = 180.0f;
        float angle;
        bool showCube = true; 
        KeyboardState lastKeyPress;
        private const int XYZ_SIZE = 75;
        private bool axesControl = true;
        private int transStep = 0;    
        private int radStep = 8;        
        private int attStep = 6;        

        private bool newStatus = true; 

        private bool orbitLightEnabled = true;
        private float orbitRadius = 8.0f;
        private float orbitSpeed = 1.0f; 
        private float orbitAngle = 0.0f;
        private float orbitHeight = 4.0f;

        private Window3D() : base(800, 600, new GraphicsMode(32, 24, 0, 8)) {
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1); 

            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            GL.Enable(EnableCap.Normalize);

            float[] ambient0 = { 0.15f, 0.15f, 0.15f, 1.0f };
            float[] diffuse0 = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] specular0 = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Ambient, ambient0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse0);
            GL.Light(LightName.Light0, LightParameter.Specular, specular0);

            float[] ambient1 = { 0.06f, 0.04f, 0.0f, 1.0f };
            float[] diffuse1 = { 1.0f, 0.85f, 0.35f, 1.0f };
            float[] specular1 = { 1.0f, 0.9f, 0.6f, 1.0f };
            GL.Light(LightName.Light1, LightParameter.Ambient, ambient1);
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse1);
            GL.Light(LightName.Light1, LightParameter.Specular, specular1);

            GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, 1.0f);
            GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 0.09f);
            GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, 0.032f);

            GL.Light(LightName.Light1, LightParameter.ConstantAttenuation, 1.0f);
            GL.Light(LightName.Light1, LightParameter.LinearAttenuation, 0.09f);
            GL.Light(LightName.Light1, LightParameter.QuadraticAttenuation, 0.032f);

            float[] matSpecular = { 0.9f, 0.9f, 0.9f, 1.0f };
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, matSpecular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 32.0f);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(30, 30, 30, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            showCube = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // advance orbit angle
            orbitAngle += (float)e.Time * orbitSpeed;

            if (mouse[MouseButton.Left]) {
                Console.WriteLine("Click non-accelerat (" + mouse.X + "," + mouse.Y + "); accelerat (" + Mouse.X + "," + Mouse.Y + ")");
            }

            if (keyboard[Key.Escape]) {
                Exit();
                return;
            }

            if (keyboard[Key.P] && !keyboard.Equals(lastKeyPress)) {
                showCube = !showCube;
            }

            if (keyboard[Key.R] && !keyboard.Equals(lastKeyPress)) {
                newStatus = !newStatus;
                if (newStatus) GL.Enable(EnableCap.Lighting); else GL.Disable(EnableCap.Lighting);
            }

            if (keyboard[Key.O] && !keyboard.Equals(lastKeyPress)) {
                orbitLightEnabled = !orbitLightEnabled;
                if (orbitLightEnabled) GL.Enable(EnableCap.Light1); else GL.Disable(EnableCap.Light1);
            }

            if (keyboard[Key.Plus] || keyboard[Key.KeypadPlus]) orbitSpeed += 0.2f;
            if (keyboard[Key.Minus] || keyboard[Key.KeypadMinus]) orbitSpeed = Math.Max(0.0f, orbitSpeed - 0.2f);
            if (keyboard[Key.A]) transStep--;
            if (keyboard[Key.D]) transStep++;
            if (keyboard[Key.W]) radStep--;
            if (keyboard[Key.S]) radStep++;
            if (keyboard[Key.Up]) attStep++;
            if (keyboard[Key.Down]) attStep--;

            lastKeyPress = keyboard;
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = Matrix4.LookAt(30f, 30f, 30f, 0f, 0f, 0f, 0f, 1f, 0f);
GL.MatrixMode(MatrixMode.Modelview);
GL.LoadMatrix(ref view);

            SetLightPosition();
            if (orbitLightEnabled) SetOrbitLightPosition();
            if (newStatus) {
                DrawLitCenteredCube(6.0f, Color.SkyBlue);
            } else {
                DrawUnlitCenteredCube(6.0f, Color.SkyBlue);
            }

            bool lightingWasEnabled = GL.IsEnabled(EnableCap.Lighting);
            if (lightingWasEnabled) GL.Disable(EnableCap.Lighting);
            if (axesControl) DrawAxes();
            if (lightingWasEnabled && newStatus) GL.Enable(EnableCap.Lighting);

            if (showCube) {
                if (GL.IsEnabled(EnableCap.Lighting)) GL.Disable(EnableCap.Lighting);
                GL.PushMatrix();
                GL.Translate(transStep, attStep, radStep);
                float lightCubeScale = 0.5f;
                GL.Scale(lightCubeScale, lightCubeScale, lightCubeScale);
                DrawUnlitCenteredCube(1.0f, Color.Yellow);
                GL.PopMatrix();
                if (newStatus) GL.Enable(EnableCap.Lighting);
            }

            if (GL.IsEnabled(EnableCap.Lighting)) GL.Disable(EnableCap.Lighting);
            if (orbitLightEnabled) {
                Vector3 orbitPos = ComputeOrbitPosition();
                GL.PushMatrix();
                GL.Translate(orbitPos);
                GL.Scale(0.4f, 0.4f, 0.4f);
                DrawUnlitCenteredCube(1.0f, Color.Orange);
                GL.PopMatrix();
            }
            if (newStatus) GL.Enable(EnableCap.Lighting);

            SwapBuffers();
        }

        private void SetLightPosition() {
            float[] position = { transStep, attStep, radStep, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Position, position);
        }

        private Vector3 ComputeOrbitPosition() {
            float x = orbitRadius * (float)Math.Cos(orbitAngle);
            float z = orbitRadius * (float)Math.Sin(orbitAngle);
            float y = orbitHeight;
            return new Vector3(x, y, z);
        }

        private void SetOrbitLightPosition() {
            Vector3 p = ComputeOrbitPosition();
            float[] position = { p.X, p.Y, p.Z, 1.0f };
            GL.Light(LightName.Light1, LightParameter.Position, position);
        }

        private void DrawAxes() {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(XYZ_SIZE, 0, 0);

            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, XYZ_SIZE, 0);

            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, XYZ_SIZE);
            GL.End();
        }

        private void DrawLitCenteredCube(float size, Color faceColor) {
            float h = size / 2.0f;
            GL.Color3(faceColor);

            GL.Begin(PrimitiveType.Quads);

            // Front (+Z)
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-h, -h, h);
            GL.Vertex3(h, -h, h);
            GL.Vertex3(h, h, h);
            GL.Vertex3(-h, h, h);

            // Back (-Z)
            GL.Normal3(0, 0, -1);
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(-h, h, -h);
            GL.Vertex3(h, h, -h);

            // Left (-X)
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(-h, -h, h);
            GL.Vertex3(-h, h, h);
            GL.Vertex3(-h, h, -h);

            // Right (+X)
            GL.Normal3(1, 0, 0);
            GL.Vertex3(h, -h, h);
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(h, h, -h);
            GL.Vertex3(h, h, h);

            // Top (+Y)
            GL.Normal3(0, 1, 0);
            GL.Vertex3(-h, h, h);
            GL.Vertex3(h, h, h);
            GL.Vertex3(h, h, -h);
            GL.Vertex3(-h, h, -h);

            // Bottom (-Y)
            GL.Normal3(0, -1, 0);
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(h, -h, h);
            GL.Vertex3(-h, -h, h);

            GL.End();
        }

        private void DrawUnlitCenteredCube(float size, Color color) {
            float h = size / 2.0f;
            GL.Color3(color);

            GL.Begin(PrimitiveType.Quads);

            // Front
            GL.Vertex3(-h, -h, h);
            GL.Vertex3(h, -h, h);
            GL.Vertex3(h, h, h);
            GL.Vertex3(-h, h, h);

            // Back
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(-h, h, -h);
            GL.Vertex3(h, h, -h);

            // Left
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(-h, -h, h);
            GL.Vertex3(-h, h, h);
            GL.Vertex3(-h, h, -h);

            // Right
            GL.Vertex3(h, -h, h);
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(h, h, -h);
            GL.Vertex3(h, h, h);

            // Top
            GL.Vertex3(-h, h, h);
            GL.Vertex3(h, h, h);
            GL.Vertex3(h, h, -h);
            GL.Vertex3(-h, h, -h);

            // Bottom
            GL.Vertex3(-h, -h, -h);
            GL.Vertex3(h, -h, -h);
            GL.Vertex3(h, -h, h);
            GL.Vertex3(-h, -h, h);

            GL.End();
        }

        [STAThread]
        static void Main(string[] args) {

            using (Window3D example = new Window3D()) {
                example.Run(60.0, 0.0);
            }

        }
    }

}
