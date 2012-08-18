#region

using System;
using Drydock.Utilities;

#endregion

namespace Drydock.UI{
    //get around to this -eventually-
    internal class Border : IUIElement{
        public IUIComponent[] Components{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #region IUIElement Members

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

        public String Texture{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Identifier{
            get { throw new NotImplementedException(); }
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

        #endregion
    }
}