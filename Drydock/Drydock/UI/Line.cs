using System;
using Drydock.Render;
using Microsoft.Xna.Framework;

namespace Drydock.UI {
    class Line : IUIElement{
        private IUIElementComponent[] _components;
        private readonly Line2D _lineSprite;

        public int Identifier{
            get { throw new NotImplementedException(); }
        }

        public IUIElementComponent[] Components{
            get { return _components; }
            set { _components = value; }
        }

        #region ctor
        public Line(Vector2 v1, Vector2 v2, float layerDepth){
            _lineSprite = new Line2D(v1, v2, layerDepth);

        }
        #endregion

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components) {
                if (component.GetType() == typeof(TComponent)) {
                    return (TComponent)component;
                }
            }
            throw new Exception("Request made to a Button object for a component that did not exist.");
        }

        public IAdvancedPrimitive Sprite{
            get { return _lineSprite; }
        }

        public void Update(){
            throw new NotImplementedException();
        }

        public int X{
            get { throw new NotImplementedException(); }
            set { }
        }

        public int Y{
            get { throw new NotImplementedException(); }
            set { }
        }

        public Rectangle BoundingBox{
            get { throw new NotImplementedException(); }
        }
    }
}
