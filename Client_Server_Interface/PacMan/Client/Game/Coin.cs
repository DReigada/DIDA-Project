using System;
using System.Security.AccessControl;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Coin : AbstractProp {

        public Coin(int id, Position pos) : base(id, pos) {
        }
            
    }
}