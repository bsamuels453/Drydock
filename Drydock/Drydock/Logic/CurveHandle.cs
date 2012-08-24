#region

using System;
using System.Collections.Generic;
using Drydock.UI;
using Drydock.UI.Components;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic{
    internal class CurveHandle{
        private enum HandleType{
            Center,
            Prev,
            Next
        }

        readonly Button _prevHandle;
        readonly Button _centerHandle;
        readonly Button _nextHandle;
        readonly Line _prevLine;
        readonly Line _nextLine;
        float _restrictionAngle;
        int _reflectionX;
        int _reflectionY;
        bool _dontTranslateHandles;
        

        public TranslateDragToExtern TranslateToExtern;
        public CurveHandle SymmetricHandle;

        public Vector2 CentPosition{
            get { return _centerHandle.CentPosition; }
        }

        public Vector2 NextPosition{
            get { return _nextHandle.CentPosition; }
        }

        public Vector2 PrevPosition{
            get { return _prevHandle.CentPosition; }
        }

        public float PrevLength{
            get { return _prevLine.Length; }
        }

        public float NextLength{
            get { return _nextLine.Length; }
        }

        public float Angle {
            set {
                _prevLine.Angle = value;
                _nextLine.Angle = (float)(value + Math.PI);
                _prevHandle.CentPosition = _prevLine.DestPoint;
                _nextHandle.CentPosition = _nextLine.DestPoint;
            }
            get { return _prevLine.Angle; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonTemplate"></param>
        /// <param name="lineTemplate"></param>
        /// <param name="panelType"></param>
        /// <param name="uicollection"></param>
        /// <param name="pos"></param>
        /// <param name="prevComponent"></param>
        /// <param name="nextComponent"></param>
        public CurveHandle(ButtonGenerator buttonTemplate,LineGenerator lineTemplate, UIElementCollection uicollection, Vector2 pos, Vector2 prevComponent, Vector2 nextComponent) {

            buttonTemplate.Identifier = (int)HandleType.Center;
            buttonTemplate.X = pos.X;
            buttonTemplate.Y = pos.Y;
            _centerHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _centerHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;

            buttonTemplate.Identifier = (int)HandleType.Prev;
            buttonTemplate.X = prevComponent.X + pos.X;
            buttonTemplate.Y = prevComponent.Y + pos.Y;
            _prevHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _prevHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;

            buttonTemplate.Identifier = (int)HandleType.Next;
            buttonTemplate.X = nextComponent.X + pos.X;
            buttonTemplate.Y = nextComponent.Y + pos.Y;
            _nextHandle = uicollection.Add<Button>(buttonTemplate.GenerateButton());
            _nextHandle.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;

            _prevLine = uicollection.Add<Line>(lineTemplate.GenerateLine());
            _nextLine = uicollection.Add<Line>(lineTemplate.GenerateLine());

            _prevLine.OriginPoint = _centerHandle.CentPosition;
            _prevLine.DestPoint = _prevHandle.CentPosition;

            _nextLine.OriginPoint = _centerHandle.CentPosition;
            _nextLine.DestPoint = _nextHandle.CentPosition;

            InterlinkButtonEvents();
        }

        public void SetReflectionType(PanelAlias panelType){
            switch (panelType) {
                case PanelAlias.Side:
                    _restrictionAngle = (float)Math.PI / 2;
                    _reflectionX = 0;
                    _reflectionY = 1;
                    _dontTranslateHandles = true;
                    break;
                case PanelAlias.Top:
                    _restrictionAngle = (float)Math.PI / 2;
                    _reflectionX = 1;
                    _reflectionY = -1;
                    _dontTranslateHandles = false;
                    break;
                case PanelAlias.Back:
                    _restrictionAngle = 0;
                    _reflectionX = -1;
                    _reflectionY = 1;
                    _dontTranslateHandles = false;
                    break;
            }
        }

        public void TranslatePosition(int dx, int dy){
            RawCenterTranslate(dx, dy);
        }

        private void RawCenterTranslate(int dx, int dy){
            _centerHandle.X += dx;
            _centerHandle.Y += dy;
            _prevHandle.X += dx;
            _prevHandle.Y += dy;

            _nextHandle.X += dx;
            _nextHandle.Y += dy;

            _prevLine.TranslateDestination(dx, dy);
            _prevLine.TranslateOrigin(dx, dy);
            _nextLine.TranslateDestination(dx, dy);
            _nextLine.TranslateOrigin(dx, dy);
        }

        private void RawPrevTranslate(int dx, int dy){
            _prevHandle.X += dx;
            _prevHandle.Y += dy;

            _prevLine.TranslateDestination(dx, dy);
            _nextLine.Angle = (float)(_prevLine.Angle + Math.PI);

            _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width / 2;
            _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height / 2;
        }

        private void RawNextTranslate(int dx, int dy) {
            _nextHandle.X += dx;
            _nextHandle.Y += dy;
            _nextLine.TranslateDestination(dx, dy);
            _prevLine.Angle = (float)(_nextLine.Angle + Math.PI);

            _prevHandle.X = _prevLine.DestPoint.X - _prevHandle.BoundingBox.Width / 2;
            _prevHandle.Y = _prevLine.DestPoint.Y - _prevHandle.BoundingBox.Height / 2;
        }

        private void TranslateToLinks(object caller, int dx, int dy){
            var obj = (Button) caller;
            switch ((HandleType) obj.Identifier){
                case HandleType.Center:
                    _prevHandle.X += dx;
                    _prevHandle.Y += dy;

                    _nextHandle.X += dx;
                    _nextHandle.Y += dy;

                    _prevLine.TranslateDestination(dx, dy);
                    _prevLine.TranslateOrigin(dx, dy);
                    _nextLine.TranslateDestination(dx, dy);
                    _nextLine.TranslateOrigin(dx, dy);
                    if (SymmetricHandle != null){
                        SymmetricHandle.RawCenterTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    break;
                case HandleType.Prev:
                    _prevLine.TranslateDestination(dx, dy);
                    _nextLine.Angle = (float) (_prevLine.Angle + Math.PI);

                    _nextHandle.X = _nextLine.DestPoint.X - _nextHandle.BoundingBox.Width/2;
                    _nextHandle.Y = _nextLine.DestPoint.Y - _nextHandle.BoundingBox.Height/2;

                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.RawNextTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    break;
                case HandleType.Next:
                    _nextLine.TranslateDestination(dx, dy);
                    _prevLine.Angle = (float) (_nextLine.Angle + Math.PI);

                    _prevHandle.X = _prevLine.DestPoint.X - _prevHandle.BoundingBox.Width/2;
                    _prevHandle.Y = _prevLine.DestPoint.Y - _prevHandle.BoundingBox.Height/2;
                    if (SymmetricHandle != null && !_dontTranslateHandles){
                        SymmetricHandle.RawPrevTranslate(_reflectionX*dx, _reflectionY*dy);
                    }
                    break;
            }

            if (TranslateToExtern != null){
                float dxf = dx;
                float dyf = dy;

                TranslateToExtern(this,ref dxf, ref dyf, true);
            }
        }

        private void InterlinkButtonEvents() {
            FadeComponent.LinkFadeComponentTriggers(_prevHandle, _nextHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_prevHandle, _centerHandle, FadeComponent.FadeTrigger.EntryExit);
            FadeComponent.LinkFadeComponentTriggers(_nextHandle, _centerHandle, FadeComponent.FadeTrigger.EntryExit);


            FadeComponent.LinkOnewayFadeComponentTriggers(
                eventProcElements: new IUIElement[]{
                    _prevHandle,
                    _nextHandle,
                    _centerHandle
                },
                eventRecieveElements: new IUIElement[]{
                    _prevLine,
                    _nextLine
                },
                state: FadeComponent.FadeTrigger.EntryExit
                );
        }
    }

    /*internal class CurveHandle{
        public readonly Button HandleButton;
        private readonly Line _destLine;


        private readonly List<LinkedHandle> _linkedHandles;
        private readonly Line _originLine1; //we can assume that if one of these isnt null, neither is
        private readonly Line _originLine2;
        private readonly List<LinkedHandle> _symmetricLinkedHandles;
        public CurveHandle CounterHandle;
        private ExternalLinkedHandle _externalLink;

        public CurveHandle(ButtonGenerator buttonTemplate, UIElementCollection elementCollection, Line originLine1 = null, Line originLine2 = null, Line destLine = null){
            _linkedHandles = new List<LinkedHandle>();
            _symmetricLinkedHandles = new List<LinkedHandle>();

            HandleButton = elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            HandleButton.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;

            _originLine1 = originLine1;
            _originLine2 = originLine2;
            _destLine = destLine;
        }

        public bool VerifyNewPosition(int dx, int dy){
            return false;
        }

        /// <summary>
        ///   called by center handle. rawtranslate only translates the object and its connected lines
        /// </summary>
        /// <param name="dx"> </param>
        /// <param name="dy"> </param>
        public void RawTranslate(int dx, int dy){
            HandleButton.X += dx;
            HandleButton.Y += dy;

            if (_destLine != null){
                _destLine.TranslateDestination(dx, dy);
            }

            if (_originLine1 != null){
                _originLine1.TranslateOrigin(dx, dy);
                _originLine2.TranslateOrigin(dx, dy);
            }
        }

        /// <summary>
        ///   typically called by symmetric handles. translates the object, its connected lines, and all of its links
        /// </summary>
        /// <param name="dx"> </param>
        /// <param name="dy"> </param>
        public void Translate(int dx, int dy){
            RawTranslate(dx, dy);
            if (CounterHandle != null){
                CounterHandle.TranslateBySettingAngle(_destLine.Angle + (float) Math.PI);
            }
            foreach (var handle in _linkedHandles){
                handle.Translate(
                    handle.ReflectionX*dx,
                    handle.ReflectionY*dy
                    );
            }
            //if (_externalLink != null){
            //    _externalLink.TranslateToExternal(dx,dy);
            //}
        }

        /// <summary>
        ///   this is only called by counter handles (raw)
        /// </summary>
        /// <param name="angle"> </param>
        public void TranslateBySettingAngle(float angle){
            _destLine.Angle = angle;
            HandleButton.X = _destLine.DestPoint.X - HandleButton.BoundingBox.Width/2;
            HandleButton.Y = _destLine.DestPoint.Y - HandleButton.BoundingBox.Width/2;
        }

        /// <summary>
        ///   is only called by component OnDrag
        /// </summary>
        /// <param name="caller"> </param>
        /// <param name="dx"> </param>
        /// <param name="dy"> </param>
        private void TranslateToLinks(object caller, int dx, int dy){
            if (_destLine != null){
                _destLine.TranslateDestination(dx, dy);
                if (CounterHandle != null){
                    CounterHandle.TranslateBySettingAngle(_destLine.Angle + (float) Math.PI);
                }
            }
            if (_originLine1 != null){
                _originLine1.TranslateOrigin(dx, dy);
                _originLine2.TranslateOrigin(dx, dy);
            }
            foreach (var handle in _linkedHandles){
                handle.Translate(
                    handle.ReflectionX*dx,
                    handle.ReflectionY*dy
                    );
            }
            foreach (var handle in _symmetricLinkedHandles){
                handle.Translate(
                    handle.ReflectionX*dx,
                    handle.ReflectionY*dy
                    );
            }
            if (_externalLink != null){
                _externalLink.TranslateToExternal(dx, dy);
            }
        }

        public void AddLinkedHandle(LinkType linkType, CurveHandle handle, HandleTranslator translator, bool isSymmetric){
            int reflectionX = 0;
            int reflectionY = 0;

            switch (linkType){
                case LinkType.DxDy:
                    reflectionX = 1;
                    reflectionY = 1;
                    break;
                case LinkType.RDxDy:
                    reflectionX = -1;
                    reflectionY = 1;
                    break;
                case LinkType.RDxRDy:
                    reflectionX = -1;
                    reflectionY = -1;
                    break;
                case LinkType.DxRDy:
                    reflectionX = 1;
                    reflectionY = -1;
                    break;
                case LinkType.Dy:
                    reflectionX = 0;
                    reflectionY = 1;
                    break;
            }

            var handleLink = new LinkedHandle(handle, translator, reflectionX, reflectionY);
            if (isSymmetric){
                _symmetricLinkedHandles.Add(handleLink);
            }
            else{
                _linkedHandles.Add(handleLink);
            }
        }

        public void RemoveLinkedHandle(CurveHandle handle){
            for (int i = 0; i < _linkedHandles.Count; i++){
                if (_linkedHandles[i].Handle == handle){
                    _linkedHandles.RemoveAt(i);
                    return;
                }
            }
        }

        public void AddExternalLinkedHandle(LinkType linkType, OnComponentDrag translator, bool axisReflect){
            int reflectionX = 0;
            int reflectionY = 0;

            switch (linkType){
                case LinkType.DxDy:
                    reflectionX = 1;
                    reflectionY = 1;
                    break;
                case LinkType.RDxDy:
                    reflectionX = -1;
                    reflectionY = 1;
                    break;
                case LinkType.RDxRDy:
                    reflectionX = -1;
                    reflectionY = -1;
                    break;
                case LinkType.DxRDy:
                    reflectionX = 1;
                    reflectionY = -1;
                    break;
                case LinkType.Dy:
                    reflectionX = 0;
                    reflectionY = 1;
                    break;
            }
            _externalLink = new ExternalLinkedHandle(translator, reflectionX, reflectionY, axisReflect, this);
        }

        #region Nested type: ExternalLinkedHandle

        private class ExternalLinkedHandle{
            private readonly bool _axisReflect;
            private readonly CurveHandle _handle;
            private readonly int _reflectionX;
            private readonly int _reflectionY;
            private readonly OnComponentDrag _translate;

            public ExternalLinkedHandle(OnComponentDrag translator, int reflectionX, int reflectionY, bool axisReflect, CurveHandle handle){
                _translate = translator;
                _reflectionX = reflectionX;
                _reflectionY = reflectionY;
                _axisReflect = axisReflect;
                _handle = handle;
            }

            public void TranslateToExternal(int dx, int dy){
                if (_axisReflect){
                    _translate(_handle, dx*_reflectionX, dy*_reflectionY);
                }
                else{
                    _translate(_handle, dy*_reflectionY, dx*_reflectionX);
                }
            }
        }

        #endregion

        #region Nested type: LinkedHandle

        private class LinkedHandle{
            public readonly CurveHandle Handle;
            public readonly int ReflectionX;
            public readonly int ReflectionY;
            public readonly HandleTranslator Translate;

            public LinkedHandle(CurveHandle handle, HandleTranslator translator, int reflectionX, int reflectionY){
                Handle = handle;
                Translate = translator;
                ReflectionX = reflectionX;
                ReflectionY = reflectionY;
            }
        }

        #endregion
    }*/
}