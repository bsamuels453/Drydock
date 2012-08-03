using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    internal interface IAdvancedPrimitive{
        Texture2D Texture { get; set; }
        void SetTextureFromString(string textureName);
    }
}