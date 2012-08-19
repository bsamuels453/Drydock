using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.UI;
using Drydock.UI.Components;

namespace Drydock.Logic {
    // ReSharper disable InconsistentNaming
    internal enum LinkType{
        DxDy,
        RDxDy,
        DxRDy,
        RDxRDy,
        Dy,
        RDy
    }
    delegate void HandleTranslator(int dx, int dy);

    // ReSharper restore InconsistentNaming
    class CurveHandle {
        public readonly Button HandleButton;
        public CurveHandle CounterHandle;
        readonly Line _originLine1;//we can assume that if one of these isnt null, neither is
        readonly Line _originLine2;
        readonly Line _destLine;


        readonly List<LinkedHandle> _linkedHandles;
        readonly List<LinkedHandle> _symmetricLinkedHandles; 

        public CurveHandle(ButtonGenerator buttonTemplate, UIElementCollection elementCollection, Line originLine1=null,Line originLine2=null, Line destLine=null){
            _linkedHandles = new List<LinkedHandle>();
            _symmetricLinkedHandles = new List<LinkedHandle>();

            HandleButton = elementCollection.Add<Button>(buttonTemplate.GenerateButton());
            HandleButton.GetComponent<DraggableComponent>().DragMovementDispatcher += TranslateToLinks;

            _originLine1 = originLine1;
            _originLine2 = originLine2;
            _destLine = destLine;
        }

        /// <summary>
        /// called by center handle. rawtranslate only translates the object and its connected lines
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void RawTranslate(int dx, int dy){
            HandleButton.X += dx;
            HandleButton.Y += dy;

            if (_destLine != null){
                _destLine.TranslateDestination(dx, dy);
            }

            if (_originLine1 != null) {
                _originLine1.TranslateOrigin(dx, dy);
                _originLine2.TranslateOrigin(dx, dy);
            } 
        }

        /// <summary>
        /// typically called by symmetric handles. translates the object, its connected lines, and all of its links
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(int dx, int dy){
            RawTranslate(dx, dy);
            if (CounterHandle != null) {
                CounterHandle.TranslateBySettingAngle(_destLine.Angle + (float)Math.PI);
            }
            foreach (var handle in _linkedHandles) {
                handle.Translate(
                    handle.ReflectionX * dx,
                    handle.ReflectionY * dy
                    );
            }
        }

        /// <summary>
        /// this is only called by counter handles (raw)
        /// </summary>
        /// <param name="angle"></param>
        public void TranslateBySettingAngle(float angle){
            _destLine.Angle = angle;
            HandleButton.X = _destLine.DestPoint.X - HandleButton.BoundingBox.Width / 2;
            HandleButton.Y = _destLine.DestPoint.Y - HandleButton.BoundingBox.Width / 2;
        }

        /// <summary>
        /// is only called by component OnDrag
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void TranslateToLinks(object caller, int dx, int dy) {
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
                    handle.ReflectionX * dx,
                    handle.ReflectionY * dy
                    );
            }
            foreach (var handle in _symmetricLinkedHandles) {
                handle.Translate(
                    handle.ReflectionX * dx,
                    handle.ReflectionY * dy
                    );
            }
        }

        public void AddLinkedHandle(LinkType linkType, CurveHandle handle, HandleTranslator translator, bool isSymmetric){
            int reflectionX = 0;
            int reflectionY = 0;

            switch (linkType) {
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
            if (isSymmetric) {
                _symmetricLinkedHandles.Add(handleLink);
            }
            else{
                _linkedHandles.Add(handleLink);
            }

        }

        public void RemoveLinkedHandle(CurveHandle handle){
            for (int i = 0; i < _linkedHandles.Count; i++) {
                if (_linkedHandles[i].Handle == handle) {
                    _linkedHandles.RemoveAt(i);
                    return;
                }
            }
        }


        private class LinkedHandle {
            public readonly CurveHandle Handle;
            public readonly HandleTranslator Translate;
            public readonly int ReflectionX;
            public readonly int ReflectionY;

            public LinkedHandle(CurveHandle handle, HandleTranslator translator, int reflectionX, int reflectionY){
                Handle = handle;
                Translate = translator;
                ReflectionX = reflectionX;
                ReflectionY = reflectionY;
            }
        }

        /*private struct ExternalLinkedHandles {
            public HandleAlias Handle;
            public HullEditorPanel Panel;
            public int ReflectionX;
            public int ReflectionY;
        }*/
    }
}
