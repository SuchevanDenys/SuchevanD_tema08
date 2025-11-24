using System.Drawing;
using OpenTK.Graphics.OpenGL;


namespace Main_2
{

    /// <summary>
    /// Deplasează obiectul pe ecran manipulându-i coordonatele vertexurilor. Evident, se recomandă utilizarea mecanismului TRANSLATE(), fiind mult mai eficient!
    /// </summary>
    internal class ManualTriangle
    {
        public VertexPoint A, B, C;
        private readonly int OFFSET = 1;
        public bool IsDrawable { get; set; }

        public void Hide() {
            IsDrawable = false;
        }

        public void Show() {
            IsDrawable = true;
        }

        public void ToggleVisibility() {
            if (IsDrawable == true) {
                Hide();
            } else {
                Show();
            }
        }

        public ManualTriangle() {
            IsDrawable = true;

            // coordonate hardcoded - se poate înlocui cu încărcare din fișier text specificat;
            A = new VertexPoint(5, 2, 0, Color.DeepPink);
            B = new VertexPoint(15, 20, 0, Color.DeepPink);
            C = new VertexPoint(10, 20, 0, Color.DeepPink);
        }

        public void ManualMoveMe(bool _relativeForward, bool _relativeBackward, bool _relativeLeft, bool _relativeRight, bool _relativeUp, bool _relativeDown) {
            if (IsDrawable == false) {
                return;
            }

            if (_relativeForward == true) {
                A.coordZ -= OFFSET;
                B.coordZ -= OFFSET;
                C.coordZ -= OFFSET;
            }

            if (_relativeBackward == true) {
                A.coordZ += OFFSET;
                B.coordZ += OFFSET;
                C.coordZ += OFFSET;
            }

            if (_relativeLeft == true) {
                A.coordX -= OFFSET;
                B.coordX -= OFFSET;
                C.coordX -= OFFSET;
            }

            if (_relativeRight == true) {
                A.coordX += OFFSET;
                B.coordX += OFFSET;
                C.coordX += OFFSET;
            }

            if (_relativeUp == true) {
                A.coordY += OFFSET;
                B.coordY += OFFSET;
                C.coordY += OFFSET;
            }

            if (_relativeDown == true) {
                A.coordY -= OFFSET;
                B.coordY -= OFFSET;
                C.coordY -= OFFSET;
            }


        }

        public void DrawMe() {
            if (IsDrawable == false) {
                return;
            }

            GL.Begin(PrimitiveType.Triangles);

            GL.Color3(A.pointColor);
            GL.Vertex3(A.coordX, A.coordY, A.coordZ);
            GL.Color3(B.pointColor);
            GL.Vertex3(B.coordX, B.coordY, B.coordZ);
            GL.Color3(C.pointColor);
            GL.Vertex3(C.coordX, C.coordY, C.coordZ);

            GL.End();
        }

    }
}
