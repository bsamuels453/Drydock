#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal abstract class StandardEffect : BaseBufferObject<VertexPositionNormalTexture>{
        protected StandardEffect(int numIndicies, int numVerticies, int numPrimitives, string settingsFileName) :
            base(numIndicies, numVerticies, numPrimitives, PrimitiveType.TriangleList){
            
            Gbl.LoadShader(settingsFileName, out BufferEffect);
            BufferEffect.Parameters["Projection"].SetValue(Gbl.ProjectionMatrix);
            BufferEffect.Parameters["World"].SetValue(Matrix.Identity);
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

        public Vector3 AmbientColor{
            set { BufferEffect.Parameters["AmbientColor"].SetValue(value); }
        }

        public float DiffuseColor{
            set { BufferEffect.Parameters["DiffuseColor"].SetValue(value); }
        }

        public Texture2D Texture{
            set { BufferEffect.Parameters["Texture"].SetValue(value); }
        }
    }
}