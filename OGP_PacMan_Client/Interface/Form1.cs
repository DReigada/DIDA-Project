using System;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Server;

namespace OGPPacManClient.Interface {
    public partial class Form1 : Form {
        public Form1() {
            Direction = Movement.Direction.Stopped;
            InitializeComponent();
            label2.Visible = false;
        }

        public Movement.Direction Direction { get; private set; }
        public event Action<string> NewMessage;


        private void KeyIsDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Left:
                    Direction = Movement.Direction.Left;
                    break;
                case Keys.Right:
                    Direction = Movement.Direction.Right;
                    break;
                case Keys.Up:
                    Direction = Movement.Direction.Up;
                    break;
                case Keys.Down:
                    Direction = Movement.Direction.Down;
                    break;
                case Keys.Enter:
                    tbMsg.Enabled = true;
                    Direction = Movement.Direction.Stopped;
                    tbMsg.Focus();
                    break;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    Direction = Movement.Direction.Stopped;
                    break;
            }
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && NewMessage != null) {
                var text = tbMsg.Text;
                NewMessage.Invoke(text);
                tbMsg.Clear();
                tbMsg.Enabled = false;
                Focus();
            }
        }

        public void AddMessage(string msg) {
            Invoke((MethodInvoker) (() =>
                tbChat.Text += "\r\n" + msg));
        }

        public void UpdateScore(int score) {
            Invoke((MethodInvoker) (() =>
                ScoreLabel.Text = $"Score: {score}"
            ));
        }
    }
}