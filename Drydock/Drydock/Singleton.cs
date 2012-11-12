#region

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

#endregion

namespace Drydock{
    internal static class Singleton{
        public static ContentManager ContentManager;
        public static Dictionary<string, string> ContentStrLookup;
        public static GraphicsDevice Device;
        public static Matrix ProjectionMatrix;

        static Singleton(){
            var sr = new StreamReader("Raws/ContentReferences.json");
            ContentStrLookup = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            sr.Close();
        }
    }
}