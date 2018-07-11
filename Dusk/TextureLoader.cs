using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dusk
{
    class TextureLoader
    {
        // private static Dictionary<EnumBlock, TextureBlockUV> _blockUVs = new Dictionary<EnumBlock, TextureBlockUV>();

        private static readonly List<int> _allTextures = new List<int>();

        public static readonly int TEXTURE_MISSING = LoadTexture(CreateMissingTexture());

        private static Bitmap CreateMissingTexture()
        {
            Bitmap bmp = new Bitmap(16, 16);

            SolidBrush pink = new SolidBrush(Color.FromArgb(228, 0, 228));

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(pink, 0, 0, 8, 8);
                g.FillRectangle(pink, 8, 8, 8, 8);

                g.FillRectangle(Brushes.Black, 8, 0, 8, 8);
                g.FillRectangle(Brushes.Black, 0, 8, 8, 8);
            }

            return bmp;
        }

        public static int LoadTexture(Bitmap texture, bool smooth = false)
        {
            int texID = GL.GenTexture();
            _allTextures.Add(texID);

            GL.BindTexture(TextureTarget.Texture2D, texID);

            BitmapData data = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            texture.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)(smooth ? TextureMinFilter.Linear : TextureMinFilter.Nearest));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)(smooth ? TextureMagFilter.Linear : TextureMagFilter.Nearest));//TODO correct this in the main branch
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return texID;
        }

        public static int LoadTexture(string textureFile, bool smooth = false)
        {
            try
            {
                var image = (Bitmap)Image.FromFile(textureFile);

                return LoadTexture(image, smooth);
            }
            catch
            {
                return TEXTURE_MISSING;
            }
        }

        public static int LoadTextureWithMipMap(Bitmap textureMap)
        {
            int texID = GL.GenTexture();
            _allTextures.Add(texID);

            GL.BindTexture(TextureTarget.Texture2D, texID);

            BitmapData data = textureMap.LockBits(new Rectangle(0, 0, textureMap.Width, textureMap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            textureMap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, (int)Math.Floor(Math.Log(16) / Math.Log(2)));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, 0);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        private static void DrawToBitmap(Bitmap to, int x, int y, string file)
        {
            using (Bitmap bmp = (Bitmap)Image.FromFile(file))
            {
                DrawToBitmap(to, x, y, bmp);
            }
        }

        private static void DrawToBitmap(Bitmap to, int x, int y, Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(to))
            {
                g.DrawImage(bmp, x, y, 16, 16);
            }
        }

        public static void Reload()
        {
            //DestroyTexture(TEXTURE_BLOCKS.ID);

            //_blockUVs.Clear();
        }

        public static void Destroy()
        {
            //DestroyTexture(TEXTURE_BLOCKS.ID);

            for (int i = 0; i < _allTextures.Count; i++)
            {
                GL.DeleteTexture(_allTextures[i]);
            }
        }
    }
}