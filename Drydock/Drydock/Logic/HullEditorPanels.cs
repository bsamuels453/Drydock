#region

using System;
using System.IO;
using System.Xml;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Components;
using Drydock.Utilities;

#endregion

namespace Drydock.Logic{
    //this class and its subordinates make me want to kill myself, never reuse it anywhere and schedule a rewrite

    #region namespace panel stuff

    internal delegate void TranslateDragToExtern(object caller, ref float dx, ref float dy, bool doApplyChange);

    internal delegate void RecieveDragFromExtern(HandleAlias handleAlias, ref float dx, ref float dy, bool doApplyChange);

    internal enum HandleAlias{
        First,
        Middle,
        Last,
        ExtremaY //the handle this alias cooresponds to changes depending on which handle has the highest Y value
    }

    internal enum PanelAlias{
        Side,
        Top,
        Back
    }

    #endregion

    #region abstract panel class

    internal abstract class HullEditorPanel{
        protected readonly Button Background;
        protected readonly FloatingRectangle BoundingBox;
        public readonly BezierCurveCollection Curves;
        protected readonly UIElementCollection ElementCollection;
        protected readonly RenderPanel PanelRenderTarget;

        protected HullEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration, PanelAlias panelType){
            BoundingBox = new FloatingRectangle(x, y, width, height);
            PanelRenderTarget = new RenderPanel(x, y, width, height, DepthLevel.Medium);
            RenderPanel.SetRenderPanel(PanelRenderTarget);

            ElementCollection = new UIElementCollection(DepthLevel.Medium);
            Curves = new BezierCurveCollection(
                defaultConfig: defaultCurveConfiguration,
                areaToFill: new FloatingRectangle(
                    x + width*0.1f,
                    y + height*0.1f,
                    width - width*0.2f,
                    height - height*0.2f
                    ),
                parentCollection: ElementCollection,
                panelType: panelType
                );
            Curves.ElementCollection.AddDragConstraintCallback(ClampChildElements);
            Background = ElementCollection.Add<Button>(
                new Button(
                    x: x,
                    y: y,
                    width: width,
                    height: height,
                    depth: DepthLevel.Background,
                    owner: ElementCollection,
                    textureName: "panelBG",
                    spriteTexRepeatX: width/(Curves.PixelsPerMeter*10),
                    spriteTexRepeatY: height/(Curves.PixelsPerMeter*10),
                    components: new IUIComponent[]{new PanelComponent()}
                    )
                );
            Update();
        }

        public void SaveCurves(string fileName){
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            Stream outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var writer = XmlWriter.Create(outputStream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteElementString("NumControllers", null, Curves.Count.ToString());

            for (int i = 0; i < Curves.Count; i++){
                writer.WriteStartElement("Handle" + i, null);
                writer.WriteElementString("PosX", null, ((Curves[i].CenterHandlePos.X - Curves.MinX)/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("PosY", null, ((Curves[i].CenterHandlePos.Y - Curves.MinY)/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("Angle", null, Curves[i].Handle.Angle.ToString());
                writer.WriteElementString("PrevLength", null, (Curves[i].PrevHandleLength/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("NextLength", null, (Curves[i].NextHandleLength/Curves.PixelsPerMeter).ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
            outputStream.Close();
        }

        void ClampChildElements(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
            if (x > BoundingBox.X + BoundingBox.Width || x < BoundingBox.X){
                x = oldX;
            }
            if (y > BoundingBox.Y + BoundingBox.Height || y < BoundingBox.Y){
                y = oldY;
            }
        }

        public void Update(){
            Curves.Update();
        }

        /// <summary>
        ///   this function accepts modifications in METERS
        /// </summary>
        /// <param name="handle"> </param>
        /// <param name="dx"> </param>
        /// <param name="dy"> </param>
        public void ModifyHandlePosition(HandleAlias handle, float dx, float dy){
            dx *= Curves.PixelsPerMeter;
            dy *= Curves.PixelsPerMeter;
            var dxi = (int) Math.Round(dx);
            var dyi = (int) Math.Round(dy);

            switch (handle){
                case HandleAlias.First:
                    Curves[0].Handle.TranslatePosition(dxi, dyi);
                    break;
                case HandleAlias.Middle:
                    Curves[Curves.Count/2].Handle.TranslatePosition(dxi, dyi);
                    break;
                case HandleAlias.Last:
                    Curves[Curves.Count - 1].Handle.TranslatePosition(dxi, dyi);
                    break;
                case HandleAlias.ExtremaY:
                    var extremaController = Curves[0];
                    foreach (var curve in Curves){
                        if (curve.CenterHandlePos.Y > extremaController.CenterHandlePos.Y){
                            extremaController = curve;
                        }
                    }
                    extremaController.Handle.TranslatePosition(dxi, dyi);
                    break;
            }
        }

        public bool VerifyNewHandlePosition(HandleAlias handle, float dx, float dy){
            dx *= Curves.PixelsPerMeter;
            dy *= Curves.PixelsPerMeter;
            var dxi = (int) Math.Round(dx);
            var dyi = (int) Math.Round(dy);

            /*switch (handle) {
                case HandleAlias.First:
                    return Curves[0].CenterHandle.VerifyNewPosition(dxi, dyi);
                case HandleAlias.Middle:
                    return Curves[Curves.Count / 2].CenterHandle.VerifyNewPosition(dxi, dyi);
                case HandleAlias.Last:
                    return Curves[Curves.Count - 1].CenterHandle.VerifyNewPosition(dxi, dyi);
                case HandleAlias.ExtremaY:
                    var extremaController = Curves[0];
                    foreach (var curve in Curves) {
                        if (curve.CenterHandlePos.Y > extremaController.CenterHandlePos.Y) {
                            extremaController = curve;
                        }
                    }
                    return extremaController.CenterHandle.VerifyNewPosition(dxi, dyi);
            }*/
            return false;
        }

        protected abstract void ProcExternalDrag(object caller, ref float dx, ref float dy, bool doApplyChange);
        protected abstract bool VerifyExternalDrag(object caller, int dx, int dy);
    }

    #endregion

    #region sidepanel impl

    internal class SideEditorPanel : HullEditorPanel{
        public BackEditorPanel BackPanel;
        public TopEditorPanel TopPanel;

        public SideEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration, PanelAlias.Side){
            foreach (var curve in Curves){
                curve.Handle.TranslateToExtern = ProcExternalDrag;
            }
        }

        protected override void ProcExternalDrag(object caller, ref float dx, ref float dy, bool doApplyChange){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;

            //Curves[0] is the frontmost controller that represents the limit of the bow
            if (controller == Curves[0].Handle){
                if (TopPanel != null){
                    TopPanel.ModifyHandlePosition(HandleAlias.Middle, dxf, 0);
                }
                if (BackPanel != null){
                    BackPanel.ModifyHandlePosition(HandleAlias.First, 0, dyf);
                    BackPanel.ModifyHandlePosition(HandleAlias.Last, 0, dyf);
                }
            }

            //Curves[Curves.Count-1] is the hindmost controller that represents the limit of the stern
            if (controller == Curves[Curves.Count - 1].Handle){
                if (TopPanel != null){
                    TopPanel.ModifyHandlePosition(HandleAlias.Last, dxf, 0);
                    TopPanel.ModifyHandlePosition(HandleAlias.First, dxf, 0);
                }
                if (BackPanel != null){
                    BackPanel.ModifyHandlePosition(HandleAlias.First, 0, dyf);
                    BackPanel.ModifyHandlePosition(HandleAlias.Last, 0, dyf);
                }
            }

            if (controller == Curves.MaxYCurve.Handle){
                if (BackPanel != null){
                    BackPanel.ModifyHandlePosition(HandleAlias.Middle, 0, dyf);
                }
            }
        }

        protected override bool VerifyExternalDrag(object caller, int dx, int dy){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;

            //Curves[0] is the frontmost controller that represents the limit of the bow
            if (controller == Curves[0].Handle){
                if (TopPanel != null){
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.Middle, dxf, 0)){
                        return false;
                    }
                }
                if (BackPanel != null){
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.First, 0, dyf)){
                        return false;
                    }

                    if (BackPanel.VerifyNewHandlePosition(HandleAlias.Last, 0, dyf)){
                        return false;
                    }
                }
            }

            //Curves[Curves.Count-1] is the hindmost controller that represents the limit of the stern
            if (controller == Curves[Curves.Count - 1].Handle){
                if (TopPanel != null){
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.Last, dxf, 0)){
                        return false;
                    }
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.First, dxf, 0)){
                        return false;
                    }
                }
                if (BackPanel != null){
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.First, 0, dyf)){
                        return false;
                    }
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.Last, 0, dyf)){
                        return false;
                    }
                }
            }

            if (controller == Curves.MaxYCurve.Handle){
                if (BackPanel != null){
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.Middle, 0, dyf)){
                        return false;
                    }
                }
            }
            return true;
        }
    }

    #endregion

    #region toppanel impl

    internal class TopEditorPanel : HullEditorPanel{
        public BackEditorPanel BackPanel;
        public SideEditorPanel SidePanel;

        public TopEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration, PanelAlias.Top){
            Curves[0].Handle.TranslateToExtern = ProcExternalDrag;
            Curves[Curves.Count - 1].Handle.TranslateToExtern = ProcExternalDrag;
            Curves[Curves.Count/2].Handle.TranslateToExtern = ProcExternalDrag;
        }

        protected override void ProcExternalDrag(object caller, ref float dx, ref float dy, bool doApplyChange){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;
            if (controller == Curves[0].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.Last, dxf, 0);
                }
                if (BackPanel != null){
                    BackPanel.ModifyHandlePosition(HandleAlias.Last, -dyf, 0);
                    BackPanel.ModifyHandlePosition(HandleAlias.First, dyf, 0);
                }
            }
            if (controller == Curves[Curves.Count - 1].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.Last, dxf, 0);
                }
                if (BackPanel != null){
                    BackPanel.ModifyHandlePosition(HandleAlias.Last, dyf, 0);
                    BackPanel.ModifyHandlePosition(HandleAlias.First, -dyf, 0);
                }
            }
            if (controller == Curves[Curves.Count/2].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.First, dxf, 0);
                }
            }
        }

        protected override bool VerifyExternalDrag(object caller, int dx, int dy){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;

            //Curves[0] is the frontmost controller that represents the limit of the bow
            if (controller == Curves[0].Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.Last, dxf, 0)){
                        return false;
                    }
                }
                if (BackPanel != null){
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.Last, -dyf, 0)){
                        return false;
                    }

                    if (BackPanel.VerifyNewHandlePosition(HandleAlias.First, dyf, 0)){
                        return false;
                    }
                }
            }

            //Curves[Curves.Count-1] is the hindmost controller that represents the limit of the stern
            if (controller == Curves[Curves.Count - 1].Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.Last, dxf, 0)){
                        return false;
                    }
                }
                if (BackPanel != null){
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.Last, dyf, 0)){
                        return false;
                    }
                    if (!BackPanel.VerifyNewHandlePosition(HandleAlias.First, -dyf, 0)){
                        return false;
                    }
                }
            }

            if (controller == Curves.MaxYCurve.Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.First, dxf, 0)){
                        return false;
                    }
                }
            }
            return true;
        }
    }

    #endregion

    #region backpanel impl

    internal class BackEditorPanel : HullEditorPanel{
        public SideEditorPanel SidePanel;
        public TopEditorPanel TopPanel;

        public BackEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration, PanelAlias.Back){
            Curves[0].Handle.TranslateToExtern = ProcExternalDrag;
            Curves[Curves.Count - 1].Handle.TranslateToExtern = ProcExternalDrag;
            Curves[Curves.Count/2].Handle.TranslateToExtern = ProcExternalDrag;
        }

        protected override void ProcExternalDrag(object caller, ref float dx, ref float dy, bool doApplyChange){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;
            if (controller == Curves[0].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.First, 0, dyf);
                    SidePanel.ModifyHandlePosition(HandleAlias.Last, 0, dyf);
                }
                if (TopPanel != null){
                    TopPanel.ModifyHandlePosition(HandleAlias.First, 0, dxf);
                    TopPanel.ModifyHandlePosition(HandleAlias.Last, 0, -dxf);
                }
            }
            if (controller == Curves[Curves.Count - 1].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.First, 0, dyf);
                    SidePanel.ModifyHandlePosition(HandleAlias.Last, 0, dyf);
                }
                if (TopPanel != null){
                    TopPanel.ModifyHandlePosition(HandleAlias.First, 0, -dxf);
                    TopPanel.ModifyHandlePosition(HandleAlias.Last, 0, dxf);
                }
            }
            if (controller == Curves[Curves.Count/2].Handle){
                if (SidePanel != null){
                    SidePanel.ModifyHandlePosition(HandleAlias.ExtremaY, 0, dyf);
                }
            }
        }

        protected override bool VerifyExternalDrag(object caller, int dx, int dy){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveHandle) caller;

            //Curves[0] is the frontmost controller that represents the limit of the bow
            if (controller == Curves[0].Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.First, 0, dyf)){
                        return false;
                    }
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.Last, 0, dyf)){
                        return false;
                    }
                }
                if (TopPanel != null){
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.First, 0, dxf)){
                        return false;
                    }

                    if (TopPanel.VerifyNewHandlePosition(HandleAlias.Last, 0, -dxf)){
                        return false;
                    }
                }
            }

            //Curves[Curves.Count-1] is the hindmost controller that represents the limit of the stern
            if (controller == Curves[Curves.Count - 1].Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.First, 0, dyf)){
                        return false;
                    }
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.Last, 0, dyf)){
                        return false;
                    }
                }
                if (TopPanel != null){
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.First, 0, -dxf)){
                        return false;
                    }
                    if (!TopPanel.VerifyNewHandlePosition(HandleAlias.Last, 0, dxf)){
                        return false;
                    }
                }
            }

            if (controller == Curves.MaxYCurve.Handle){
                if (SidePanel != null){
                    if (!SidePanel.VerifyNewHandlePosition(HandleAlias.ExtremaY, 0, dyf)){
                        return false;
                    }
                }
            }
            return true;
        }
    }

    #endregion
}