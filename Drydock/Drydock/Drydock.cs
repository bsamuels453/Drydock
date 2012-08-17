#region

using System.Diagnostics;
using System.Threading;
using Drydock.Control;
using Drydock.Logic;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Drydock{
    public class Drydock : Game{
        private readonly GraphicsDeviceManager _graphics;
        // private EditorLogic _editorLogic;
        private EditorLogic _editorLogic;
        private Renderer _renderer;

        public ContentManager ContentManager;

        public Drydock(){
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this){
                PreferredBackBufferWidth = 1200,
                PreferredBackBufferHeight = 800,
                SynchronizeWithVerticalRetrace = false,
            };
        }

        protected override void Initialize(){
            //   _editorLogic = new EditorLogic();
            ContentManager = Content;
            Singleton.ContentManager = ContentManager;
            _renderer = new Renderer(_graphics.GraphicsDevice, Content);
            _editorLogic = new EditorLogic(_renderer);
            IsMouseVisible = true;
            //var cur = new Bitmap("D:/Projects/assets/untitled-4.png", true);
            //Graphics g = Graphics.FromImage(cur);
            // IntPtr ptr = cur.GetHicon();
            // var c = new Cursor(ptr);
            //  System.Windows.Forms.Control.FromHandle(this.Window.Handle).Cursor = c;
            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
        }


        protected override void Update(GameTime gameTime){
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _editorLogic.Update();
            InputEventDispatcher.Update(_renderer);
            //Thread.Sleep(10);
            sw.Stop();
            System.Console.WriteLine("time:" + sw.ElapsedMilliseconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _renderer.Draw();
            base.Draw(gameTime);
        }
    }
}