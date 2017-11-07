using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using OGPPacManClient.Properties;

namespace OGPPacManClient.Interface {
    internal class BoardController {
        private readonly Dictionary<int, PictureBox> coins;
        private readonly Form1 form;

        private readonly Dictionary<int, PictureBox> ghosts;
        private readonly Dictionary<int, PictureBox> players;


        public BoardController(Form1 form) {
            ghosts = new Dictionary<int, PictureBox>();
            players = new Dictionary<int, PictureBox>();
            coins = new Dictionary<int, PictureBox>();

            this.form = form;
        }

        public void Update(Board board) {
            Task.Run(() =>
                UpdatePositions(board));
        }


        private void UpdatePositions(Board updatedBoard) {
            UpdateProps(updatedBoard.Ghosts, ghosts, initGhost);
            UpdateProps(updatedBoard.Players, players, initPlayer);
            UpdateProps(updatedBoard.Coins, coins, initCoin);
            RemoveCoins(updatedBoard.Coins);
        }

        private void RemoveCoins(List<Coin> coinsToKeep) {
            lock (coins){
                var keyValuesToKeep = coinsToKeep.Select(a => new KeyValuePair<int, PictureBox>(a.Id, coins[a.Id]));
                var removedCoins = coins.Except(keyValuesToKeep).ToList();
                removedCoins.ForEach(keyValue => {
                    coins.Remove(keyValue.Key);
                    form.Invoke((MethodInvoker) (() => form.Controls.Remove(keyValue.Value)));
                });
            }
        }


        private void UpdateProps<A>(List<A> props, Dictionary<int, PictureBox> dict, Func<A, PictureBox> initProp)
            where A : AbstractProp {
            lock (props){
                props.ForEach(prop => {
                    form.Invoke((MethodInvoker) (
                        () => {
                            if (dict.TryGetValue(prop.Id, out var maybeProp)){
                                maybeProp.Left = prop.Position.X;
                                maybeProp.Top = prop.Position.Y;
                            }
                            else{
                                var pic = initProp(prop);
                                dict.Add(prop.Id, pic);
                                form.Controls.Add(pic);
                            }

                            switch (prop){
                                case PacManPlayer player:
                                    var pic = dict[player.Id];
                                    pic.Image = GetNewImage(player.Direction, pic);
                                    break;
                            }
                        }
                    ));
                });
            }
        }

        private Image GetNewImage(Movement.Direction dir, PictureBox pic) {
            switch (dir){
                case Movement.Direction.Down:
                    return Resources.Down;
                case Movement.Direction.Left:
                    return Resources.Left;
                case Movement.Direction.Up:
                    return Resources.Up;
                case Movement.Direction.Right:
                    return Resources.Right;
                default:
                    return pic.Image;
            }
        }

        private PictureBox initGhost(Ghost ghost) {
            Bitmap GhostImage() {
                switch (ghost.Color){
                    case GhostColor.Pink:
                        return Resources.pink_guy;
                    case GhostColor.Red:
                        return Resources.red_guy;
                    case GhostColor.Yellow:
                        return Resources.yellow_guy;
                    default:
                        return Resources.yellow_guy;
                }
            }

            var ghostPicture = new PictureBox {
                BackColor = Color.Transparent,
                Image = GhostImage(),
                Name = $"ghost{ghost.Id}",
                Size = new Size(30, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabIndex = 1,
                TabStop = false,
                Tag = "ghost",
                Left = ghost.Position.X,
                Top = ghost.Position.Y
            };

            return ghostPicture;
        }

        private PictureBox initPlayer(PacManPlayer player) {
            return new PictureBox {
                BackColor = Color.Transparent,
                Image = Resources.Left,
                Margin = new Padding(0),
                Name = $"pacman{player.Id}",
                Size = new Size(25, 25),
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 4,
                TabStop = false,
                Left = player.Position.X,
                Top = player.Position.Y
            };
        }


        private PictureBox initCoin(Coin coin) {
            return new PictureBox {
                BackColor = Color.Transparent,
                Image = Resources.cccc,
                Margin = new Padding(0),
                Name = $"coin{coin.Id}",
                Size = new Size(22, 22),
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 4,
                TabStop = false,
                Left = coin.Position.X,
                Top = coin.Position.Y
            };
        }
    }
}