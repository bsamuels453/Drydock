using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drydock.Render {
    static class ScreenData {
        public static int ScreenWidth;
        public static int ScreenHeight;

        static public void Init(int sizeX, int sizeY){
            ScreenWidth = sizeX;
            ScreenHeight = sizeY;
        }

        static public void GetScreenValue(float percentX, float percentY, ref int x, ref int y){
            x = (int) (ScreenWidth * percentX);
            y = (int)(ScreenHeight * percentY);
        }

    }
}
