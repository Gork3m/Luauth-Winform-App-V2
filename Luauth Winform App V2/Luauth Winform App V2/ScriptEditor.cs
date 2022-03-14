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
    public partial class ScriptEditor : Form {
        bool updatin = false;
        public ScriptEditor(string script_name = "Unnamed script", bool ffa = false, string platform = "roblox", bool updating = false) {
            InitializeComponent();
            checkBox1.Checked = ffa;
            textBox1.Text = script_name;
            comboBox1.Text = platform;

            if (updating) {
                comboBox1.Enabled = false;
                textBox1.ReadOnly = true;                
            }
            updatin = updating;
        }

        private void ScriptEditor_Load(object sender, EventArgs e) {
            
        }
        public bool done = false;
        public HttpTypes.UpdateScript updateData;
        public HttpTypes.CreateScript createData;
        private void button1_Click(object sender, EventArgs e) {
            if (textBox2.Text.Length < 5 && !updatin) {
                MessageBox.Show("Provide an alert webhook");
                return;
            }
            if (textBox3.Text.Length < 5 && !updatin) {
                MessageBox.Show("Provide a logs webhook");
                return;
            }
            if (updatin) {
                updateData = new HttpTypes.UpdateScript() {
                    ffa = checkBox1.Checked,
                    alerts_webhook = textBox2.Text,
                    logs_webhook = textBox3.Text,
                    script = richTextBox1.Text
                };
            } else {
                createData = new HttpTypes.CreateScript() {
                    ffa = checkBox1.Checked,
                    alerts_webhook = textBox2.Text,
                    logs_webhook = textBox3.Text,
                    platform = comboBox1.Text,
                    script_name = textBox1.Text,
                    script = richTextBox1.Text
                };
            }
            done = true;
            this.Close();
        }
    }
}
