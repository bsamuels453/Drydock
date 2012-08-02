#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Drydock.UI.Components;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic{
    internal class CurveController{

        private readonly Button _centerHandle;
        private readonly Button _handle1;
        private readonly Button _handle2;

        private readonly Line _line1;
        private readonly Line _line2;

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

        public CurveController(int initX, int initY, float length1, float length2,float length3, float angle){
            Vector2 component1 = Common.GetComponentFromAngle(angle, length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (angle - Math.PI), length2); // minus math.pi to reverse direction
            Vector2 component3 = Common.GetComponentFromAngle((float) (angle - Math.PI/2f), length3); // minus math.pi to reverse direction

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
            FadeComponent.LinkFadeComponentTriggers(_handle1, _handle2, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_handle1, _centerHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_handle2, _centerHandle, FadeComponent.FadeTrigger.EntryExit);


            FadeComponent.LinkOnewayFadeComponentTriggers(
                eventProcElements: new IUIElement[]{
                    _handle1,
                    _handle2,
                    _centerHandle,
                },
                eventRecieveElements: new IUIElement[]{
                    _line1,
                    _line2,
                },
                state: FadeComponent.FadeTrigger.EntryExit
            );
            
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


                    break;
                case 1:
                    //_line1.TranslateDestination(dx, dy);
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

        private void ClampHandleMovement(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
        }

        private Vector2 GetPerpendicularBisector(Vector2 originPoint, Vector2 destPoint, Vector2 perpendicularPoint){

            destPoint *= 10;

            var list = Common.Bresenham(originPoint, destPoint);
            var distances = new List<float>(list.Count);

            for (int i = 0; i < list.Count; i++){
                distances.Add(Vector2.Distance(list[i], perpendicularPoint));
            }
            float lowestValue = 9999999999; //my condolences to players with screens larger than 9999999999x9999999999
            int lowestIndex = -1;

            for(int i=0; i<distances.Count; i++){
                if (distances[i] < lowestValue){
                    lowestIndex = i;
                    lowestValue = distances[i];
                }
            }

            return list[lowestIndex];
        }
    }
}