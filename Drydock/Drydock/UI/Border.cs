using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drydock.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.UI {
    class Border : IUIElement{
        public float X{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Y{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Width{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Height{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public FloatingRectangle BoundingBox{
            get { throw new NotImplementedException(); }
        }

        public float Opacity{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Depth{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Texture2D Texture{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Identifier{
            get { throw new NotImplementedException(); }
        }

        public IUIComponent[] Components{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public UIElementCollection Owner{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public TComponent GetComponent<TComponent>(){
            throw new NotImplementedException();
        }

        public bool DoesComponentExist<TComponent>(){
            throw new NotImplementedException();
        }

        public void Update(){
            throw new NotImplementedException();
        }

        public void Dispose(){
            throw new NotImplementedException();
        }
    }
}
