using Drydock.Control;
using Drydock.Logic;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drydock{
    public class Drydock : Game{
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Renderer _renderer;
        private EditorLogic _editorLogic;
        private KeyboardHandler _keyboardHandler;
        private MouseHandler _mouseHandler;

        public Drydock(){
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize(){
            
            
            _editorLogic = new EditorLogic();
            _renderer = new Renderer(_graphics.GraphicsDevice, Content);
            _mouseHandler = new MouseHandler(_renderer);
            _keyboardHandler = new KeyboardHandler(_renderer);
            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
        }


        protected override void Update(GameTime gameTime){
            _mouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();

            base.Update(gameTime);
        }
 
        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _renderer.Draw();
            base.Draw(gameTime);
        }
    }
}