#region

using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal class ShipGeometryBuffer : BufferObject<VertexPositionNormalTexture>{
        readonly Effect _effect;
        readonly RasterizerState _rasterizerState;
        readonly Texture2D _texture;

        public ShipGeometryBuffer(int numIndicies, int numVerticies, int numPrimitives, string textureName, CullMode cullMode = CullMode.None)
            : base(numIndicies, numVerticies, numPrimitives){
            _rasterizerState = new RasterizerState();
            _rasterizerState.CullMode = cullMode;

            _texture = Singleton.ContentManager.Load<Texture2D>(textureName);
            _effect = Singleton.ContentManager.Load<Effect>("StandardEffect").Clone();
            _effect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["AmbientIntensity"].SetValue(1.25f);
            _effect.Parameters["AmbientColor"].SetValue(new Vector4(1, 1, 1, 1));
            _effect.Parameters["Texture"].SetValue(_texture);
        }

        protected override Effect BufferEffect{
            get { return _effect; }
        }

        protected override RasterizerState BufferRasterizer{
            get { return _rasterizerState; }
        }

        public CullMode CullMode{
            set { _rasterizerState.CullMode = value; }
        }

        public Effect Effect { get { return _effect; }
        }

        public Vector3 DiffuseDirection{
            set { _effect.Parameters["DiffuseLightDirection"].SetValue(value);  }
        }

        public float AmbientIntensity {
            set { _effect.Parameters["AmbientIntensity"].SetValue(value); }
        }

        protected override void UpdateEffectParams(Matrix viewMatrix){
            _effect.Parameters["View"].SetValue(viewMatrix);
        }
    }
}