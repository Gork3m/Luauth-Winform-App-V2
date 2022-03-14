using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Luauth_Winform_App_V2 {
    public partial class KeyForm : Form {
        public KeyForm(string key = "") {
            InitializeComponent();
            fade.Start();
            textBox1.Text = key;
        }

        private void fade_Tick(object sender, EventArgs e) {
            Opacity += .02;
            if (Opacity == 1) {
                pictureBox1.Dispose();
                FormBorderStyle = FormBorderStyle.FixedSingle;
                label1.Visible = true;
                pictureBox2.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
                fade.Stop();
            }
            
        }
        public LuauthWrapper session { get; set; }
        private void button1_Click(object sender, EventArgs e) {
            if (textBox1.Text.Length == 0) return;
            session = new LuauthWrapper(textBox1.Text);
            session.Initialize();
            this.Close();
        }
    }
}
