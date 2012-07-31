using System;
using Drydock.Render;
using Microsoft.Xna.Framework;

namespace Drydock.UI{
    internal class Line : IUIElement{
        #region fields and properties

        private readonly Line2D _lineSprite;
        public float Depth;
        public float Length;
        private float _angle;
        private Vector2 _point1;
        private Vector2 _point2;
        private Vector2 _uvVectors;

        public float Angle{
            get { return _angle; }
            set{
                _angle = value;
                _uvVectors = Common.GetComponentFromAngle(value, 1);
                CalculateDestFromUnitVector();
            }
        }

        public Vector2 OriginPoint{
            get { return _point1; }
            set{
                _point1 = value;
                CalculateInfoFromPoints();
            }
        }

        public Vector2 DestPoint{
            get { return _point2; }
            set{
                _point2 = value;
                CalculateInfoFromPoints();
            }
        }

        public int Identifier{
            get { throw new NotImplementedException(); }
        }

        public int X{
            get { return (int) _point1.X; }
            set { throw new NotImplementedException(); }
        }

        public int Y{
            get { return (int) _point1.Y; }
            set { throw new NotImplementedException(); }
        }

        public Rectangle BoundingBox{
            get { throw new NotImplementedException(); }
        }

        public IAdvancedPrimitive Sprite{
            get { return _lineSprite; }
        }

        public void TranslateOrigin(int dx, int dy){
            _point1.X += dx;
            _point1.Y += dy;
            CalculateInfoFromPoints();
        }

        public void TranslateDestination(int dx, int dy){
            _point2.X += dx;
            _point2.Y += dy;
            CalculateInfoFromPoints();
        }

        #endregion

        #region ctor

        public Line(Vector2 v1, Vector2 v2, float layerDepth, IUIElementComponent[] components){
            _lineSprite = new Line2D(this, 1);
            _point1 = v1;
            _point2 = v2;
            Depth = layerDepth;
            CalculateInfoFromPoints();

            Components = components;
            foreach (IUIElementComponent component in Components){
                component.Owner = this;
            }
        }

        #endregion

        #region private calculation functions

        /// <summary>
        /// calculates the line's destination point from the line's unit vector and length
        /// </summary>
        private void CalculateDestFromUnitVector(){
            _point2.X = _uvVectors.X*Length + _point1.X;
            _point2.Y = _uvVectors.Y*Length + _point1.Y;
        }

        /// <summary>
        /// calculates the line's blit location, angle, length, and unit vector based on the origin point and destination point
        /// </summary>
        private void CalculateInfoFromPoints(){
            _angle = (float) Math.Atan2(_point2.Y - _point1.Y, _point2.X - _point1.X);
            _uvVectors = Common.GetComponentFromAngle(_angle, 1);
            Length = Vector2.Distance(_point1, _point2);
        }

        #endregion

        #region IUIElement Members

        public IUIElementComponent[] Components { get; set; }

        public TComponent GetComponent<TComponent>(){
            foreach (IUIElementComponent component in Components){
                if (component.GetType() == typeof (TComponent)){
                    return (TComponent) component;
                }
            }
            throw new Exception("Request made to a Line object for a component that did not exist.");
        }

        public void Update(){
            foreach (IUIElementComponent component in Components){
                component.Update();
            }
        }

        #endregion
    }
}