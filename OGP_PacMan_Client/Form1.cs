using System.Windows.Forms;
using pacman.Properties;

namespace pacman {
    public partial class Form1 : Form {
        private bool godown;
        private bool goleft;
        private bool goright;
        private bool goup;

        public Form1() {
            InitializeComponent();
            label2.Visible = false;
        }

        private void keyisdown(object sender, KeyEventArgs e) {
            switch (e.KeyCode){
                case Keys.Left:
                    goleft = true;
                    pacman.Image = Resources.Left;
                    break;
                case Keys.Right:
                    goright = true;
                    pacman.Image = Resources.Right;
                    break;
                case Keys.Up:
                    goup = true;
                    pacman.Image = Resources.Up;
                    break;
                case Keys.Down:
                    godown = true;
                    pacman.Image = Resources.Down;
                    break;
                case Keys.Enter:
                    tbMsg.Enabled = true;
                    tbMsg.Focus();
                    break;
            }
        }

        private void keyisup(object sender, KeyEventArgs e) {
            switch (e.KeyCode){
                case Keys.Left:
                    goleft = false;
                    break;
                case Keys.Right:
                    goright = false;
                    break;
                case Keys.Up:
                    goup = false;
                    break;
                case Keys.Down:
                    godown = false;
                    break;
            }
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter){
                tbChat.Text += "\r\n" + tbMsg.Text;
                tbMsg.Clear();
                tbMsg.Enabled = false;
                Focus();
            }
        }
    }
}