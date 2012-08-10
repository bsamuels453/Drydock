﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.UI{
    internal class Line : IUIElement{
        #region fields and properties, and element modification methods
        public const int DefaultIdentifier = 1;
        private readonly Line2D _lineSprite;
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
        public int LineWidth { get; set; }
        public UIElementCollection Owner { get; set; }

        public IUIComponent[] Components { get; set; }
        public float Opacity { get; set; }
        public float Depth { get; set; }

        public String Texture{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Identifier { get; set; }
        public float X{
            get { return (int) _point1.X; }
            set { throw new NotImplementedException(); }
        }
        public float Y{
            get { return (int) _point1.Y; }
            set { throw new NotImplementedException(); }
        }
        public float Width {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public float Height {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public FloatingRectangle BoundingBox{
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

        public Line(Vector2 v1, Vector2 v2, Color color, DepthLevel depth, UIElementCollection owner, int identifier = DefaultIdentifier, IUIComponent[] components=null){
            _lineSprite = new Line2D(this, color);
            _point1 = v1;
            _point2 = v2;
            Depth = owner.DepthManager.GetDepth(depth);
            Opacity = 1;
            LineWidth = 1;
            Identifier = identifier;
            CalculateInfoFromPoints();

            Components = components;
            if (Components != null) {
                foreach (IUIComponent component in Components) {
                    component.Owner = this;
                }
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

        public TComponent GetComponent<TComponent>(){
            if (Components != null){
                foreach (IUIComponent component in Components){
                    if (component.GetType() == typeof (TComponent)){
                        return (TComponent) component;
                    }
                }
            }
            throw new Exception("Request made to a Line object for a component that did not exist.");
        }

        public bool DoesComponentExist<TComponent>() {
            if (Components != null){
                return Components.OfType<TComponent>().Any();
            }
            return false;
        }

        public void Update(){
            if (Components != null){
                foreach (IUIComponent component in Components){
                    component.Update();
                }
            }
        }

        public void Dispose(){
            Sprite.Dispose();
            Owner.DisposeElement(this);
        }

        #endregion
    }
    internal class LineGenerator : ComponentGenerator {
        public Dictionary<string, object[]> Components;
        public Color? Color;
        public UIElementCollection Owner;
        public DepthLevel? Depth;
        public Vector2? V1;
        public Vector2? V2;
        public int? Identifier;

        public Line GenerateLine() {
            //make sure we have all the data required
            if (Depth == null ||
                V1 == null ||
                V2 == null ||
                Color == null ||
                Depth == null ||
                Owner == null) {
                throw new Exception("Template did not contain all of the basic variables required to generate a button.");
            }
            //generate component list
            IUIComponent[] components = null;
            if (Components != null) {
                components = GenerateComponents(Components);
            }

            //now we handle optional parameters
            int identifier;
            if (Identifier != null)
                identifier = (int)Identifier;
            else
                identifier = Button.DefaultIdentifier;

            return new Line(
                (Vector2)V1,
                (Vector2)V2,
                (Color)Color,
                (DepthLevel)Depth,
                Owner,
                identifier,
                components
                );
        }
    }
}