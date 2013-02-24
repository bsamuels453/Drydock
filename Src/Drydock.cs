#region

using Drydock.Logic;
using Drydock.Logic.HullEditorState;
using Drydock.Render;
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
            Gbl.ContentManager = ContentManager;
            Gbl.Device = _graphics.GraphicsDevice;
            Gbl.ContentManager = Content;
            Gbl.ScreenSize = new Point(1200, 800);
            var aspectRatio = Gbl.Device.Viewport.Bounds.Width / (float)Gbl.Device.Viewport.Bounds.Height;
            Gbl.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView: 3.14f / 4,
                aspectRatio: aspectRatio,
                nearPlaneDistance: 1,
                farPlaneDistance: 50000
                );
            ScreenData.Init(1200, 800);
            GamestateManager.AddGameState(new HullEditor());



            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
            Gbl.CommitHashChanges();
        }


        protected override void Update(GameTime gameTime){
            GamestateManager.Update();
            //Thread.Sleep(10);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            RenderTarget.BeginDraw();
            GamestateManager.Draw();
            RenderTarget.EndDraw();
            base.Draw(gameTime);
        }
    }
}