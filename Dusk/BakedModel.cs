using OpenTK.Graphics.OpenGL;

namespace Dusk
{
    internal class BakedModel
    {
        private readonly int _vaoID;
        private readonly int _vertexCount;
        private readonly int _textureID;

        public readonly Shader Shader;

        public BakedModel(Shader shader, int textureID, int vaoID, int vertexCount)
        {
            Shader = shader;

            _vaoID = vaoID;
            _vertexCount = vertexCount;

            _textureID = textureID;
        }

        public void Bind()
        {
            Shader.Bind();

            GL.BindTexture(TextureTarget.Texture2D, _textureID);

            GL.BindVertexArray(_vaoID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
        }

        public void Unbind()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            Shader.Unbind();
        }

        public void Render()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }

        public void Destroy()
        {
            GL.DeleteVertexArray(_vaoID);
        }
    }
}