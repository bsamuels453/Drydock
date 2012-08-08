namespace Drydock.Logic {
    class HullEditor {
        CurveControllerCollection _sideColl;


        public HullEditor(){
            _sideColl = new CurveControllerCollection("Config Files/SideCurveControllerDefaults.xml");
        }
    }
}
