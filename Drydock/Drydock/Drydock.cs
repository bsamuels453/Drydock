#region

using System;
using System.Diagnostics;
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
        public ContentManager ContentManager;
        // private EditorLogic _editorLogic;
        private EditorLogic _editorLogic;

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
            Renderer.Init(_graphics.GraphicsDevice, Content);
            _editorLogic = new EditorLogic();
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
            _editorLogic.Update();
            InputEventDispatcher.Update();
            //Thread.Sleep(10);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Renderer.Draw();
            base.Draw(gameTime);
        }
    }
}