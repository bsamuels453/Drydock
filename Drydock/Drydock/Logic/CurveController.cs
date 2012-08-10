#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Drydock.UI.Components;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic{
    internal class CurveController {
        #region fields and properties
        private readonly Button _centerHandle;
        private readonly UIElementCollection _elementCollection;
        private readonly Button _handle1;
        private readonly Button _handle2;

        private readonly Line _line1;
        private readonly Line _line2;

        public Vector2 CenterHandlePos{
            get { return _centerHandle.CentPosition; }
        }
        public Vector2 PrevHandlePos{
            get { return _handle1.CentPosition; }
        }
        public Vector2 NextHandlePos{
            get { return _handle2.CentPosition; }
        }

        public float PrevHandleLength { get; set; }
        public float NextHandleLength { get; set; }

        public float Angle1 {
            set { 
                _line1.Angle = value;
                _line2.Angle = (float)(value + Math.PI);
                _handle1.CentPosition = _line1.DestPoint;
                _handle2.CentPosition = _line2.DestPoint;
            }
        }
        public float Angle2 {
            set {
                _line2.Angle = value;
                _line1.Angle = (float)(value + Math.PI);
                _handle1.CentPosition = _line1.DestPoint;
                _handle2.CentPosition = _line2.DestPoint;
            }
        }

        #endregion

        // private bool _isSelected;

        public CurveController(float initX, float initY,UIElementCollection collection, float length1, float length2, float angle){
            _elementCollection = collection;
            Vector2 component1 = Common.GetComponentFromAngle(angle, length1);
            Vector2 component2 = Common.GetComponentFromAngle((float) (angle - Math.PI), length2); // minus math.pi to reverse direction
            #region crap
            var buttonTemplate = new ButtonGenerator();
            buttonTemplate.Width = 9;
            buttonTemplate.Height = 9;
            buttonTemplate.Depth = DepthLevel.Medium;
            buttonTemplate.Owner = _elementCollection;
            buttonTemplate.TextureName = "whitebox";
            buttonTemplate.Components = new Dictionary<string, object[]>{
                    {"DraggableComponent",null},
                    {"FadeComponent", new object[] { FadeComponent.FadeState.Faded, FadeComponent.FadeTrigger.EntryExit } },
                    {"SelectableComponent", new object[]{"bigbox", 15, 15}}
            };

            buttonTemplate.Identifier = 1;
            buttonTemplate.X = component1.X + initX;
            buttonTemplate.Y = component1.Y + initY;
            _handle1 = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _handle1.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            buttonTemplate.Identifier = 2;
            buttonTemplate.X = component2.X + initX;
            buttonTemplate.Y = component2.Y + initY;
            _handle2 = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _handle2.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            buttonTemplate.Identifier = 0;
            buttonTemplate.X = initX;
            buttonTemplate.Y = initY;
            _centerHandle = _elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            _centerHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += ReactToDragMovement;

            var lineTemplate = new LineGenerator();
            lineTemplate.Depth = DepthLevel.Medium;
            lineTemplate.Owner = _elementCollection;
            lineTemplate.Color = Color.White;
            lineTemplate.Components = new Dictionary<string, object[]>{
                {"FadeComponent", new object[]{FadeComponent.FadeState.Faded}}
            };


            lineTemplate.V1 = _centerHandle.CentPosition;
            lineTemplate.V2 = _handle1.CentPosition;
            _line1 = _elementCollection.Add<Line>(lineTemplate.GenerateLine());

            lineTemplate.V2 = _handle2.CentPosition;
            _line2 = _elementCollection.Add<Line>(lineTemplate.GenerateLine());
            #endregion
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