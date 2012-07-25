namespace Drydock.Render{
    internal static class ScreenData{
        public static int ScreenWidth;
        public static int ScreenHeight;

        public static void Init(int sizeX, int sizeY){
            ScreenWidth = sizeX;
            ScreenHeight = sizeY;
        }

        public static void GetScreenValue(float percentX, float percentY, ref int x, ref int y){
            x = (int) (ScreenWidth*percentX);
            y = (int) (ScreenHeight*percentY);
        }
    }
}