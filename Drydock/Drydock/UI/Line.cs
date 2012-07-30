using System;
using Drydock.Render;
using Microsoft.Xna.Framework;

namespace Drydock.UI{
    internal class Line : IUIElement{
        private readonly Line2D _lineSprite;

        public Line2D LineSprite{
            get { return _lineSprite; }
        }

        #region IUIElement Members

        public int Identifier{
            get { throw new NotImplementedException(); }
        }

        public IUIElementComponent[] Components { get; set; }

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component.GetType() == typeof (TComponent)){
                    return (TComponent) component;
                }
            }
            throw new Exception("Request made to a Line object for a component that did not exist.");
        }

        public IAdvancedPrimitive Sprite{
            get { return _lineSprite; }
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
        }

        public int X{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Y{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Rectangle BoundingBox{
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ctor

        public Line(Vector2 v1, Vector2 v2, float layerDepth, IUIElementComponent[] components){
            _lineSprite = new Line2D(v1, v2, layerDepth);

            Components = components;
            foreach (IUIElementComponent component in Components){
                component.Owner = this;
            }
        }

        #endregion
    }
}