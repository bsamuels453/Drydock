using System;
using Drydock.UI;
using Drydock.UI.Components;
using Microsoft.Xna.Framework;

namespace Drydock.Logic{
    internal class CurveController{

        private readonly Button _centerHandle;
        private readonly Button _handle1;
        private readonly Button _handle2;
        private readonly Button _rotateHandle;

        private readonly Line _line1;
        private readonly Line _line2;
        private readonly Line _rotationHandleLine;

        #region properties

        public Vector2 CenterHandlePos{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 PrevHandlePos{
            get { return _handle1.CentPosition; }
        }

        public Vector2 NextHandlePos{
            get { return _handle2.CentPosition; }
        }

        #endregion

        // private bool _isSelected;

        public CurveController(int initX, int initY, float length1, float length2, float angle1){
            Vector2 component1 = Common.GetComponentFromAngle(angle1, length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (angle1 - Math.PI), length2); // minus math.pi to reverse direction

            _handle1 = UIContext.Add<Button>(
                new Button(
                    identifier: 1,
                    width: 9,
                    height: 9,
                    x: (int) component1.X + initX,
                    y: (int) component1.Y + initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(ClampHandleMovement, ReactToDragMovement),
                        new FadeComponent(FadeComponent.FadeState.Faded, FadeComponent.FadeTrigger.EntryExit)
                    }
                    )
                );

            _handle2 = UIContext.Add<Button>(
                new Button(
                    identifier: 2,
                    width: 9,
                    height: 9,
                    x: (int) component2.X + initX,
                    y: (int) component2.Y + initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(ClampHandleMovement, ReactToDragMovement),
                        new FadeComponent(FadeComponent.FadeState.Faded, FadeComponent.FadeTrigger.EntryExit)
                    }
                    )
                );

            _centerHandle = UIContext.Add<Button>(
                new Button(
                    identifier: 0,
                    width: 9,
                    height: 9,
                    x: initX,
                    y: initY,
                    layerDepth: 1.0f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(ClampHandleMovement, ReactToDragMovement),
                        new FadeComponent(FadeComponent.FadeState.Faded, FadeComponent.FadeTrigger.EntryExit)
                    }
                    )
                );


            //_line1 = new Line2D(_centerHandle.CentPosition, _handle1.CentPosition, 0.5f);
            // _line2 = new Line2D(_centerHandle.CentPosition, _handle2.CentPosition, 0.5f);

            _line1 = UIContext.Add<Line>(
                new Line(
                    v1: _centerHandle.CentPosition,
                    v2: _handle1.CentPosition,
                    layerDepth: 1.0f,
                    components: new IUIElementComponent[]{
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                );

            _line2 = UIContext.Add<Line>(
                new Line(
                    v1: _centerHandle.CentPosition,
                    v2: _handle2.CentPosition,
                    layerDepth: 1.0f,
                    components: new IUIElementComponent[]{
                        new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                );
            InterlinkButtonEvents();
        }

        private void InterlinkButtonEvents(){
            _handle1.OnMouseEntry.Add(_handle2.GetComponent<FadeComponent>().ForceFadein);
            _handle1.OnMouseEntry.Add(_centerHandle.GetComponent<FadeComponent>().ForceFadein);
            _handle1.OnMouseEntry.Add(_line1.GetComponent<FadeComponent>().ForceFadein);
            _handle1.OnMouseEntry.Add(_line2.GetComponent<FadeComponent>().ForceFadein);

            _handle1.OnMouseExit.Add(_handle2.GetComponent<FadeComponent>().ForceFadeout);
            _handle1.OnMouseExit.Add(_centerHandle.GetComponent<FadeComponent>().ForceFadeout);
            _handle1.OnMouseExit.Add(_line1.GetComponent<FadeComponent>().ForceFadeout);
            _handle1.OnMouseExit.Add(_line2.GetComponent<FadeComponent>().ForceFadeout);

            _handle2.OnMouseEntry.Add(_handle1.GetComponent<FadeComponent>().ForceFadein);
            _handle2.OnMouseEntry.Add(_centerHandle.GetComponent<FadeComponent>().ForceFadein);
            _handle2.OnMouseEntry.Add(_line1.GetComponent<FadeComponent>().ForceFadein);
            _handle2.OnMouseEntry.Add(_line2.GetComponent<FadeComponent>().ForceFadein);

            _handle2.OnMouseExit.Add(_handle1.GetComponent<FadeComponent>().ForceFadeout);
            _handle2.OnMouseExit.Add(_centerHandle.GetComponent<FadeComponent>().ForceFadeout);
            _handle2.OnMouseExit.Add(_line1.GetComponent<FadeComponent>().ForceFadeout);
            _handle2.OnMouseExit.Add(_line2.GetComponent<FadeComponent>().ForceFadeout);

            _centerHandle.OnMouseEntry.Add(_handle2.GetComponent<FadeComponent>().ForceFadein);
            _centerHandle.OnMouseEntry.Add(_handle1.GetComponent<FadeComponent>().ForceFadein);
            _centerHandle.OnMouseEntry.Add(_line1.GetComponent<FadeComponent>().ForceFadein);
            _centerHandle.OnMouseEntry.Add(_line2.GetComponent<FadeComponent>().ForceFadein);

            _centerHandle.OnMouseExit.Add(_handle2.GetComponent<FadeComponent>().ForceFadeout);
            _centerHandle.OnMouseExit.Add(_handle1.GetComponent<FadeComponent>().ForceFadeout);
            _centerHandle.OnMouseExit.Add(_line1.GetComponent<FadeComponent>().ForceFadeout);
            _centerHandle.OnMouseExit.Add(_line2.GetComponent<FadeComponent>().ForceFadeout);
        }

        /// <summary>
        /// this function balances handle movement so that they stay in a straight line and their movements translate to other handles
        /// </summary>
        public void ReactToDragMovement(IUIInteractiveElement owner, int dx, int dy){
            switch (owner.Identifier){
                case 0:
                    _line1.TranslateOrigin(dx, dy);
                    _line1.TranslateDestination(dx, dy);
                    _line2.TranslateOrigin(dx, dy);
                    _line2.TranslateDestination(dx, dy);
                    _handle1.X += dx;
                    _handle1.Y += dy;

                    _handle2.X += dx;
                    _handle2.Y += dy;

                    //_handle1.ManualTranslation(dx, dy);
                    //_handle2.ManualTranslation(dx, dy);
                    break;
                case 1:
                    _line1.TranslateDestination(dx, dy);
                    _line2.Angle = (float) (_line1.Angle + Math.PI);
                    _handle2.X = (int) _line2.DestPoint.X - _handle2.BoundingBox.Width/2;
                    _handle2.Y = (int) _line2.DestPoint.Y - _handle2.BoundingBox.Height/2;

                    break;
                case 2:
                    _line2.TranslateDestination(dx, dy);
                    _line1.Angle = (float) (_line2.Angle + Math.PI);
                    _handle1.X = (int) _line1.DestPoint.X - _handle1.BoundingBox.Width/2;
                    _handle1.Y = (int) _line1.DestPoint.Y - _handle1.BoundingBox.Height/2;
                    break;
            }
        }

        private void ClampHandleMovement(IUIInteractiveElement owner, ref int x, ref int y){
        }
    }
}