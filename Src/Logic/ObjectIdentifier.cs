#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace Drydock.Logic{
    internal class ObjectIdentifier : IEquatable<ObjectIdentifier>{
        public readonly int Deck;
        public readonly ObjectType ObjectType;
        public readonly Vector3 Position;

        public ObjectIdentifier(ObjectType objectType, Vector3 position, int deck){
            ObjectType = objectType;
            Position = position;
            Deck = deck;
        }

        #region IEquatable<ObjectIdentifier> Members

        public bool Equals(ObjectIdentifier other){
            return ObjectType == other.ObjectType && Position == other.Position;
        }

        #endregion
    }

    internal enum ObjectType{
        Ladder
    }
}