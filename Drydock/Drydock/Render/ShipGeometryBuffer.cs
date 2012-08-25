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

        public ShipGeometryBuffer(int numIndicies, int numVerticies, int numPrimitives, string textureName)
            : base(numIndicies, numVerticies, numPrimitives){
            _rasterizerState = new RasterizerState();
            _rasterizerState.CullMode = CullMode.None;

            _texture = Singleton.ContentManager.Load<Texture2D>(textureName);
            _effect = Singleton.ContentManager.Load<Effect>("StandardEffect");
            _effect.Parameters["Projection"].SetValue(Singleton.ProjectionMatrix);
            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["AmbientIntensity"].SetValue(0.5f);
            _effect.Parameters["AmbientColor"].SetValue(new Vector4(1, 1, 1, 1));
            _effect.Parameters["Texture"].SetValue(_texture);
        }

        protected override Effect BufferEffect{
            get { return _effect; }
        }

        protected override RasterizerState BufferRasterizer{
            get { return _rasterizerState; }
        }

        protected override void UpdateEffectParams(Matrix viewMatrix){
            _effect.Parameters["View"].SetValue(viewMatrix);
        }
    }
}