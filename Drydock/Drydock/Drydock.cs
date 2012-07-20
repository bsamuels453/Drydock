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
            _keyboardHandler = new KeyboardHandler();
            _mouseHandler = new MouseHandler();
            _editorLogic = new EditorLogic();
            _renderer = new Renderer(_graphics, Content);

            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
        }


        protected override void Update(GameTime gameTime){
            base.Update(gameTime);
        }
 
        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}