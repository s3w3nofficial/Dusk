using System.Collections.Generic;
using System.Linq;
using JeremyAnsel.Media.WavefrontObj;
using OpenTK.Graphics.OpenGL;

namespace Dusk
{
    internal class ModelLoader
    {
        private static readonly List<int> VaOs = new List<int>();
        private static readonly List<int> VbOs = new List<int>();

        public static BakedModel BakeModel(Shader shader, string textureFile, float[] vertexes, float[] normals, float[] uVs, int coordCount = 3)
        {
            int vaoId = CreateVao();

            StoreDataInAttribList(0, 3, vertexes);
            StoreDataInAttribList(1, 2, uVs);
            StoreDataInAttribList(2, 3, normals);

            GL.BindVertexArray(0);

            var textureID = TextureLoader.LoadTexture(textureFile, false);

            return new BakedModel(shader, textureID, vaoId, vertexes.Length / coordCount);
        }

        public static BakedModel BakeModel(Shader shader, string modelName)
        {
            var obj = ObjFile.FromFile($"assets/models/{modelName}.obj");
            var mtl = ObjMaterialFile.FromFile($"assets/models/{modelName}.mtl");

            var materials = new List<ObjMaterial>(mtl.Materials);

            var mat = materials.SingleOrDefault(m => m.DiffuseMap != null);

            List<float> vertexes = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();

            foreach (var face in obj.Faces)
            {
                foreach (var triple in face.Vertices) //for each vertice of the triangle
                {
                    var vertexIndex = triple.Vertex - 1;
                    var normalIndex = triple.Normal - 1;
                    var uvIndex = triple.Texture - 1;

                    var vertex = obj.Vertices[vertexIndex];
                    var normal = obj.VertexNormals[normalIndex];
                    var uv = obj.TextureVertices[uvIndex];

                    vertexes.Add(vertex.X);
                    vertexes.Add(vertex.Y);
                    vertexes.Add(vertex.Z);

                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);

                    uvs.Add(uv.X);
                    uvs.Add(1 - uv.Y);

                    //uvs.Add(uv.Y);
                }
            }

            return BakeModel(shader, $"assets/models/{mat.DiffuseMap.FileName}", vertexes.ToArray(), normals.ToArray(), uvs.ToArray());
        }

        private static int CreateVao()
        {
            int vaoId = GL.GenVertexArray();

            VaOs.Add(vaoId);

            GL.BindVertexArray(vaoId);

            return vaoId;
        }

        private static void StoreDataInAttribList(int attrib, int coordSize, float[] data)
        {
            int vboId = GL.GenBuffer();

            VbOs.Add(vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib, coordSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static void Destroy()
        {
            foreach (int vao in VaOs)
                GL.DeleteVertexArray(vao);

            foreach (int vbo in VbOs)
                GL.DeleteBuffer(vbo);
        }
    }
}