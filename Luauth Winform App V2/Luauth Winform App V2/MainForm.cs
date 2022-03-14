using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Web;
using Newtonsoft.Json;


namespace Luauth_Winform_App_V2 {
    public partial class MainForm : Form {
        public LuauthWrapper api;
        public MainForm(LuauthWrapper api) {
            InitializeComponent();
            this.api = api;
        }
        public HttpTypes.ScriptDetails selectedScript;
        private void MainForm_Load(object sender, EventArgs e) {
            RefreshForm();

        }
        public static DateTime ts2d(double unixTimeStamp) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        private void RefreshForm() {
            groupBox2.Visible = false;
            listBox1.Items.Clear();
            foreach(var x in api.RawData.scripts) {
                listBox1.Items.Add(x.script_name);
            }
            label1.Text = api.RawData.owner + " | " + ts2d(api.RawData.expires_at).ToString();
        }
        private void LoadPanel() {
            textBox1.Text = selectedScript.script_name;
            textBox2.Text = selectedScript.platform;
            textBox3.Text = selectedScript.script_id;
            textBox4.Text = selectedScript.script_version;
            textBox5.Text = selectedScript.disabled.ToString();
            ffalabel.Visible = selectedScript.ffa;
            richTextBox1.Text = "";
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            groupBox2.Visible = listBox1.SelectedIndex != -1;
            if (listBox1.SelectedIndex != -1) {
                selectedScript = api.RawData.scripts[listBox1.SelectedIndex];
                LoadPanel();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            richTextBox1.Text = (new WebClient()).DownloadString($"https://api.luauth.xyz/files/v2/loaders/{selectedScript.script_id}.lua");
        }

        private void addScriptToolStripMenuItem_Click(object sender, EventArgs e) {
            ScriptEditor se = new ScriptEditor();
            se.ShowDialog();
            if (se.done) {
                upl.Visible = true;

                string d = Http.RawPost(Endpoints.NewScript(), "POST", JsonConvert.SerializeObject(se.createData), api.RawData.key);
                HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
                if (resp.success == false) {
                    MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    upl.Visible = false;
                    return;
                }
                api.Initialize();
                selectedScript = null;
                RefreshForm();
                upl.Visible = false;
            }
        }

        private void removeScriptToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1 || selectedScript == null) {
                MessageBox.Show("Select a script first!");
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure? This will delete your script, loader, and all whitelisted users", "Hold up", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                string d = Http.RawPost(Endpoints.DeleteScript(selectedScript.script_id), "DELETE", "", api.RawData.key);
                HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
                if (resp.success == false) {
                    MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                api.Initialize();
                selectedScript = null;
                RefreshForm();
            }
        }

        private void updateScriptToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1 || selectedScript == null) {
                MessageBox.Show("Select a script first!");
                return;
            }
            ScriptEditor se = new ScriptEditor(selectedScript.script_name, selectedScript.ffa, selectedScript.platform, true);
            se.ShowDialog();
            if (se.done) {
                upl.Visible = true;

                string d = Http.RawPost(Endpoints.UpdateScript(selectedScript.script_id), "PUT", JsonConvert.SerializeObject(se.updateData), api.RawData.key);
                HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
                if (resp.success == false) {
                    MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    upl.Visible = false;
                    return;
                }
                api.Initialize();
                selectedScript = null;
                RefreshForm();
                upl.Visible = false;
            }
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1 || selectedScript == null) {
                MessageBox.Show("Select a script first!");
                return;
            }
            LogsForm l = new LogsForm(new WebClient().DownloadString(Endpoints.GetScriptLogs(api.RawData.key, selectedScript.script_id)));
            l.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void manageAccessToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1 || selectedScript == null) {
                MessageBox.Show("Select a script first!");
                return;
            }
            AccessForm f = new AccessForm(selectedScript.script_name, selectedScript, api);
            f.ShowDialog();
        }
    }
}
