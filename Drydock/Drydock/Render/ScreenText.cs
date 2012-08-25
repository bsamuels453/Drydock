#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    internal struct StringData{
        public string Str;
        public int X;
        public int Y;

        public StringData(string str, int x, int y){
            Str = str;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    ///   handles the drawing of all text onto the screen. Static methods exist as a controller for all instances of ScreenText. DO NOT THREAD THIS.
    /// </summary>
    internal class ScreenText{
        #region class methods and fields

        readonly int _id;

        public ScreenText(int x, int y, string str){
            _id = -1;
            _id = AddString(str, x, y);
        }

        public void DeleteText(){
            if (_id != -1){
                RemoveString(_id);
            }
        }

        public void EditText(string str){
            if (_id != -1){
                EditString(_id, str);
            }
        }

        #endregion

        #region static methods and fields

        const int _maxStringsDisplayable = 100;
        static bool[] _isStringSlotAvail;
        static StringData[] _stringTable;
        static SpriteFont _font;

        public static void Init(ContentManager content){
            _isStringSlotAvail = new bool[_maxStringsDisplayable];
            _stringTable = new StringData[_maxStringsDisplayable];

            for (int i = 0; i < _maxStringsDisplayable; i++){
                _isStringSlotAvail[i] = true;
            }

            _font = content.Load<SpriteFont>("SpriteFont");
        }

        public static void Draw(SpriteBatch spriteBatch){
            spriteBatch.Begin();
            for (int i = 0; i < _maxStringsDisplayable; i++){
                if (_isStringSlotAvail[i] == false){
                    spriteBatch.DrawString(
                        _font,
                        _stringTable[i].Str,
                        new Vector2(_stringTable[i].X, _stringTable[i].Y),
                        Color.Black
                        );
                }
            }
            spriteBatch.End();
        }

        static int AddString(string str, int x, int y){
            int i = 0;
            //get next avail string slot
            while (!_isStringSlotAvail[i]){
                i++;
            }
            _isStringSlotAvail[i] = false;
            _stringTable[i] = new StringData(str, x, y);
            return i;
        }

        static void RemoveString(int id){
            _isStringSlotAvail[id] = true;
        }

        static void EditString(int id, string newstr){
            int tempX = _stringTable[id].X;
            int tempY = _stringTable[id].Y;

            _stringTable[id] = new StringData(newstr, tempX, tempY);
        }

        #endregion
    }
}