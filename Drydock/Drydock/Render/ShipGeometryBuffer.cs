#region

using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class ShipGeometryBuffer : BaseBufferObject<VertexPositionNormalTexture>{
        readonly Texture2D _texture;

        public ShipGeometryBuffer(int numIndicies, int numVerticies, int numPrimitives, string textureName, CullMode cullMode = CullMode.None)
            : base(numIndicies, numVerticies, numPrimitives, PrimitiveType.TriangleList){
            BufferRasterizer = new RasterizerState();
            BufferRasterizer.CullMode = cullMode;

            _texture = Singleton.ContentManager.Load<Texture2D>(textureName);
            BufferEffect = Singleton.ContentManager.Load<Effect>("StandardEffect").Clone();
            BufferEffect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
            BufferEffect.Parameters["Texture"].SetValue(_texture);
        }

        public CullMode CullMode{
            set{
                BufferRasterizer = new RasterizerState();
                BufferRasterizer.CullMode = value;
            }
        }

        public Vector3 DiffuseDirection{
            set { BufferEffect.Parameters["DiffuseLightDirection"].SetValue(value); }
        }

        public float AmbientIntensity{
            set { BufferEffect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        public new IndexBuffer Indexbuffer {
            get { return base.Indexbuffer; }
        }

        public new VertexBuffer Vertexbuffer {
            get { return base.Vertexbuffer; }
        }
    }
}