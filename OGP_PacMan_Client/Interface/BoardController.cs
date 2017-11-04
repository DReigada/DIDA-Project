using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Properties;

namespace OGPPacManClient.Interface {
    internal class BoardController {
        private readonly Form1 form;

        private readonly Dictionary<int, PictureBox> ghosts;
        private readonly Dictionary<int, PictureBox> players;

        public BoardController(Form1 form) {
            ghosts = new Dictionary<int, PictureBox>();
            players = new Dictionary<int, PictureBox>();
            this.form = form;
        }

        public void Update(Board b) {
            form.Invoke((MethodInvoker) (() => UpdatePositions(b.Ghosts, b.Players)));
        }


        private void UpdatePositions(List<Ghost> updatedGhosts, List<PacManPlayer> updatedPlayers) {
            updatedGhosts.ForEach(ghost => {
                    if (ghosts.TryGetValue(ghost.Id, out var maybeGhost)){
                        maybeGhost.Left = ghost.Position.X;
                        maybeGhost.Top = ghost.Position.Y;
                    }
                    else{
                        var pic = initGhost(ghost);
                        form.Controls.Add(pic);
                        ghosts.Add(ghost.Id, pic);
                    }
                }
            );

            updatedPlayers.ForEach(player => {
                    if (players.TryGetValue(player.Id, out var maybePlayer)){
                        maybePlayer.Left = player.Position.X;
                        maybePlayer.Top = player.Position.Y;
                    }
                    else{
                        var pic = initPlayer(player);
                        form.Controls.Add(pic);
                        players.Add(player.Id, pic);
                    }
                }
            );
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
                Name = "pacman",
                Size = new Size(25, 25),
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 4,
                TabStop = false,
                Left = player.Position.X,
                Top = player.Position.Y
            };
        }
    }
}