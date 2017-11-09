using System;
using System.Security.AccessControl;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Coin : AbstractProp, IEquatable<Coin> {

        public Coin(int id, Position pos) : base(id, pos) {
        }

        public bool Equals(Coin other) {
            if (other != null){
                return Id == other.Id && Position == other.Position;
            }
            return false;
        }
        

    }
}