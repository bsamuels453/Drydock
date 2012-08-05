namespace Drydock.UI {
    internal enum Depth{//10 max items
        Tooltip,
        Dialogue,
        High,
        Medium,
        Low,
        Background
    }

    internal enum SubDepth{
        Highlight,
        High,
        Medium,
        Low,
        Background
    }

    internal static class DepthHelper{
        public static float GetValueFromDepth(Depth d, SubDepth sd){
            var di = (int)d;
            var sdi = (int)sd;
            return di / 10f + sdi / 100f;
        }

        public static Depth GetDepthFromValue(float v){
            int i = 0;
            while (v-0.1 >= 0){
                v -= 0.1f;
                i++;
            }
            return (Depth)i;
        }

        public static SubDepth GetSubDepthFromValue(float v) {
            while (v - 0.1 > 0) {
                v -= 0.1f;
            }
            int i = 0;

            while (v - 0.01 > 0) {
                v -= 0.01f;
                i++;
            }
            return (SubDepth)i;
        }

        public static float GetNewValueFromDepth(Depth d, float v){

            return (int)d / 100f + v;
        }
    }
}
