using Drydock.Control;
using Drydock.Logic;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drydock{
    public class Drydock : Game{
        private readonly GraphicsDeviceManager _graphics;
        private Renderer _renderer;
       // private EditorLogic _editorLogic;
        private EditorLogic _editorLogic;

        public Drydock(){
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize(){
            
            
         //   _editorLogic = new EditorLogic();
            _renderer = new Renderer(_graphics.GraphicsDevice, Content);
            _editorLogic = new EditorLogic(_renderer);
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
        }


        protected override void Update(GameTime gameTime){
            _editorLogic.Update();

            base.Update(gameTime);
        }
 
        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _renderer.Draw();
            base.Draw(gameTime);
        }
    }
}