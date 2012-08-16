#region

using System;
using System.IO;
using System.Xml;
using Drydock.UI;
using Drydock.Utilities;

#endregion


namespace Drydock.Logic {
    #region namespace panel stuff
    internal delegate void ModifyHandlePosition(HandleAlias handle, float dx, float dy);

    internal enum HandleAlias{
        First,
        Middle,
        Last,
        ExtremaY //the handle this alias cooresponds to changes depending on which handle has the highest Y value
    }
    #endregion

    #region abstract panel class
    internal abstract class HullEditorPanel{
        protected readonly Button Background;
        protected readonly FloatingRectangle BoundingBox;
        public readonly CurveControllerCollection Curves;
        protected readonly UIElementCollection ElementCollection;

        protected HullEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration){
            BoundingBox = new FloatingRectangle(x, y, width, height);
            ElementCollection = new UIElementCollection();
            Curves = new CurveControllerCollection(
                defaultConfig: defaultCurveConfiguration,
                areaToFill: new FloatingRectangle(
                    x + width*0.1f,
                    y + height*0.1f,
                    width - width*0.2f,
                    height - height*0.2f
                    ),
                parentCollection: ElementCollection
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
                    spriteTexRepeatY: height/(Curves.PixelsPerMeter*10)
                    )
                );
        }

        public void SaveCurves(string fileName){
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            Stream outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var writer = XmlWriter.Create(outputStream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Data");
            writer.WriteElementString("NumControllers", null, Curves.CurveList.Count.ToString());

            float minX = 9999999, minY = 9999999;
            foreach (var curve in Curves.CurveList){
                if (curve.HandlePos.X < minX){
                    minX = curve.HandlePos.X;
                }
                if (curve.HandlePos.Y < minY){
                    minY = curve.HandlePos.Y;
                }
            }
            for (int i = 0; i < Curves.CurveList.Count; i++){
                writer.WriteStartElement("Handle" + i, null);
                writer.WriteElementString("PosX", null, ((Curves.CurveList[i].HandlePos.X - minX)/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("PosY", null, ((Curves.CurveList[i].HandlePos.Y - minY)/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("Angle", null, Curves.CurveList[i].Angle.ToString());
                writer.WriteElementString("PrevLength", null, (Curves.CurveList[i].PrevHandleLength/Curves.PixelsPerMeter).ToString());
                writer.WriteElementString("NextLength", null, (Curves.CurveList[i].NextHandleLength/Curves.PixelsPerMeter).ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
            outputStream.Close();

        }

        private void ClampChildElements(IUIInteractiveElement owner, ref int x, ref int y, int oldX, int oldY){
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
        /// this function accepts modifications in METERS
        /// </summary>
        /// <param name="handle"> </param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void ModifyHandlePosition(HandleAlias handle, float dx, float dy){
            dx *= Curves.PixelsPerMeter;
            dy *= Curves.PixelsPerMeter;
            var dxi = (int) Math.Round(dx);
            var dyi = (int) Math.Round(dy);

            switch (handle){
                case HandleAlias.First:
                    Curves.CurveList[0].Controller.TranslateControllerPos(dxi, dyi);
                    break;
                case HandleAlias.Middle:
                    Curves.CurveList[Curves.CurveList.Count/2].Controller.TranslateControllerPos(dxi, dyi);
                    break;
                case HandleAlias.Last:
                    Curves.CurveList[Curves.CurveList.Count - 1].Controller.TranslateControllerPos(dxi, dyi);
                    break;
                case HandleAlias.ExtremaY:
                    var extremaController = Curves.CurveList[0];
                    foreach (var curve in Curves.CurveList){
                        if (curve.HandlePos.Y > extremaController.HandlePos.Y){
                            extremaController = curve;
                        }
                    }
                    extremaController.Controller.TranslateControllerPos(dxi, dyi);
                    break;
            }
        }

        abstract protected void OnCurveDrag(object caller, int dx, int dy);
    }
    #endregion

    #region sidepanel impl
    internal class SideEditorPanel : HullEditorPanel{
        public ModifyHandlePosition TopPanelModifier;
        public ModifyHandlePosition BackPanelModifier;

        public SideEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration){

            foreach (var curve in Curves.CurveList){
                curve.Controller.ReactToControllerMovement += OnCurveDrag;
            }
        }

        override protected void OnCurveDrag(object caller, int dx, int dy) {
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter;

            var controller = (CurveController) caller;

            //Curves.CurveList[0].Controller is the frontmost controller that represents the limit of the bow
            if (controller == Curves.CurveList[0].Controller){
                Curves.CurveList[Curves.CurveList.Count - 1].Controller.TranslateControllerPos(0, dy);

                if (TopPanelModifier != null) {
                    TopPanelModifier(HandleAlias.Middle, dxf, 0);
                }
                if (BackPanelModifier != null){
                    BackPanelModifier(HandleAlias.First, 0, dyf);
                    BackPanelModifier(HandleAlias.Last, 0, dyf);
                }
            }

            //Curves.CurveList[Curves.CurveList.Count-1] is the hindmost controller that represents the limit of the stern
            if (controller == Curves.CurveList[Curves.CurveList.Count-1].Controller) {
                Curves.CurveList[0].Controller.TranslateControllerPos(0, dy);

                if (TopPanelModifier != null) {
                    TopPanelModifier(HandleAlias.Last, dxf, 0);
                    TopPanelModifier(HandleAlias.First, dxf, 0);
                }
                if (BackPanelModifier != null) {
                    BackPanelModifier(HandleAlias.First, 0, dyf);
                    BackPanelModifier(HandleAlias.Last, 0, dyf);
                }
            }

            BezierCurve deepestController = Curves.CurveList[0];
            foreach (var curve in Curves.CurveList){
                if (curve.HandlePos.Y > deepestController.HandlePos.Y){
                    deepestController = curve;
                }
            }
            if (controller == deepestController.Controller){
                if (BackPanelModifier != null) {
                    BackPanelModifier(HandleAlias.Middle, 0, dyf);
                }
            }
        }
    }
    #endregion

    #region toppanel impl
    internal class TopEditorPanel : HullEditorPanel{
        public ModifyHandlePosition BackPanelModifier;
        public ModifyHandlePosition SidePanelModifier;

        public TopEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration){
            Curves.CurveList[0].Controller.ReactToControllerMovement += OnCurveDrag;
            Curves.CurveList[Curves.CurveList.Count - 1].Controller.ReactToControllerMovement += OnCurveDrag;
            Curves.CurveList[Curves.CurveList.Count/2].Controller.ReactToControllerMovement += OnCurveDrag;
        }

        override protected void OnCurveDrag(object caller, int dx, int dy){
            float dxf = dx/Curves.PixelsPerMeter;
            float dyf = dy/Curves.PixelsPerMeter; 

            var controller = (CurveController) caller;
            if (controller == Curves.CurveList[0].Controller){
                Curves.CurveList[Curves.CurveList.Count - 1].Controller.TranslateControllerPos(dx, -dy);

                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.Last, dxf, 0);
                }
                if (BackPanelModifier != null){
                    BackPanelModifier(HandleAlias.Last, -dyf, 0);
                    BackPanelModifier(HandleAlias.First, dyf, 0);
                }
            }
            if (controller == Curves.CurveList[Curves.CurveList.Count - 1].Controller){
                Curves.CurveList[0].Controller.TranslateControllerPos(dx, -dy);

                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.Last, dxf, 0);
                }
                if (BackPanelModifier != null) {
                    BackPanelModifier(HandleAlias.Last, dyf, 0);
                    BackPanelModifier(HandleAlias.First, -dyf, 0);
                }
            }
            if (controller == Curves.CurveList[Curves.CurveList.Count/2].Controller){
                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.First, dxf, 0);
                }
            }
        }
    }
    #endregion

    #region backpanel impl
    internal class BackEditorPanel : HullEditorPanel{
        public ModifyHandlePosition TopPanelModifier;
        public ModifyHandlePosition SidePanelModifier;

        public BackEditorPanel(int x, int y, int width, int height, string defaultCurveConfiguration)
            : base(x, y, width, height, defaultCurveConfiguration){
            Curves.CurveList[0].Controller.ReactToControllerMovement += OnCurveDrag;
            Curves.CurveList[Curves.CurveList.Count - 1].Controller.ReactToControllerMovement += OnCurveDrag;
            Curves.CurveList[Curves.CurveList.Count/2].Controller.ReactToControllerMovement += OnCurveDrag;
        }

        override protected void OnCurveDrag(object caller, int dx, int dy){
            float dxf = dx / Curves.PixelsPerMeter;
            float dyf = dy / Curves.PixelsPerMeter;

            var controller = (CurveController)caller;
            if (controller == Curves.CurveList[0].Controller) {
                Curves.CurveList[Curves.CurveList.Count - 1].Controller.TranslateControllerPos(-dx, dy);

                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.First, 0,dyf);
                    SidePanelModifier(HandleAlias.Last, 0, dyf);
                }
                if (TopPanelModifier != null) {
                    TopPanelModifier(HandleAlias.First, 0, dxf);
                    TopPanelModifier(HandleAlias.Last, 0, -dxf);
                }
            }
            if (controller == Curves.CurveList[Curves.CurveList.Count - 1].Controller) {
                Curves.CurveList[0].Controller.TranslateControllerPos(-dx, dy);

                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.First, 0, dyf);
                    SidePanelModifier(HandleAlias.Last, 0, dyf);
                }
                if (TopPanelModifier != null) {
                    TopPanelModifier(HandleAlias.First, 0, -dxf);
                    TopPanelModifier(HandleAlias.Last, 0, dxf);
                }
            }
            if (controller == Curves.CurveList[Curves.CurveList.Count / 2].Controller) {
                if (SidePanelModifier != null) {
                    SidePanelModifier(HandleAlias.ExtremaY, 0, dyf);
                }
            }

        }
    }
    #endregion

}