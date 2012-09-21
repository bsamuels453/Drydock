#region

using Drydock.Control;
using Drydock.Logic;
using Drydock.Logic.HullEditorState;
using Drydock.Logic.TestState;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Drydock{
    public class Drydock : Game{
        readonly GraphicsDeviceManager _graphics;
        public ContentManager ContentManager;
        // private EditorLogic _editorLogic;

        public Drydock(){
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this){
                PreferredBackBufferWidth = 1200,
                PreferredBackBufferHeight = 800,
                SynchronizeWithVerticalRetrace = false,
            };
        }

        protected override void Initialize(){
            ContentManager = Content;
            Singleton.ContentManager = ContentManager;
            Renderer.Init(_graphics.GraphicsDevice, Content);
            GamestateManager.Init();
            GamestateManager.SetGameState(new TestState());

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
            GamestateManager.ClearGameState();
        }


        protected override void Update(GameTime gameTime){
            InputEventDispatcher.Update();
            GamestateManager.Update();
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