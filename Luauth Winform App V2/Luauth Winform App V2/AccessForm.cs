using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Net;


namespace Luauth_Winform_App_V2 {
    public partial class AccessForm : Form {
        HttpTypes.ScriptDetails sdata;
        LuauthWrapper api;
        HttpTypes.AllIdentifiers identifiers;
        public AccessForm(string sname, HttpTypes.ScriptDetails sdata, LuauthWrapper api) {
            InitializeComponent();
            Text = "Managing access of " + sname;
            this.sdata = sdata;
            this.api = api;
        }

        private void AccessForm_Load(object sender, EventArgs e) {
            string d = Http.RawGet(Endpoints.GetAllIdentifiers(sdata.script_id), api.RawData.key);
            HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
            if (resp.success == false) {
                MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return;
            }
            identifiers = JsonConvert.DeserializeObject<HttpTypes.AllIdentifiers>(d);
            RefreshForm();
        }

        HttpTypes.IdentifierDetails selectedIdentifier;
        private void RefreshForm() {
            groupBox2.Visible = false;
            listBox1.Items.Clear();
            foreach (var x in identifiers.whitelisted_users) {
                listBox1.Items.Add((x.banned ? "❌":"") + x.identifier);
            }

        }
        private void LoadPanel() {
            textBox1.Text = selectedIdentifier.identifier;
            idlbl.Text = "(" + selectedIdentifier.identifier_type + ")";
            textBox2.Text = selectedIdentifier.whitelisted.ToString();
            textBox6.Text = selectedIdentifier.banned.ToString();
            textBox3.Text = selectedIdentifier.ban_reason;
            textBox4.Text = selectedIdentifier.total_executions.ToString();
            textBox5.Text = selectedIdentifier.auth_expire == 1 ? "Never" : MainForm.ts2d(selectedIdentifier.auth_expire).ToString();
            textBox7.Text = selectedIdentifier.unban_token;
            
            richTextBox1.Text = selectedIdentifier.note;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            groupBox2.Visible = listBox1.SelectedIndex != -1;
            if (listBox1.SelectedIndex != -1) {
                selectedIdentifier = identifiers.whitelisted_users[listBox1.SelectedIndex];
                LoadPanel();
                unbanUserToolStripMenuItem.Visible = selectedIdentifier.banned;
                 
              
            }
            removeUserToolStripMenuItem.Visible = listBox1.SelectedIndex != -1;
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e) {
            AddUser au = new AddUser(sdata.script_name, sdata, api);
            au.ShowDialog();
            if (au.done) {
                string d = Http.RawGet(Endpoints.GetAllIdentifiers(sdata.script_id), api.RawData.key);
                HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
                if (resp.success == false) {
                    MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                identifiers = JsonConvert.DeserializeObject<HttpTypes.AllIdentifiers>(d);
                RefreshForm();
            }
        }

        private void removeUserToolStripMenuItem_Click(object sender, EventArgs e) {
            string d = Http.RawPost(Endpoints.UnwhitelistIdentifier(sdata.script_id, selectedIdentifier.identifier), "DELETE", "", api.RawData.key);
            HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
            if (resp.success == false) {
                MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            d = Http.RawGet(Endpoints.GetAllIdentifiers(sdata.script_id), api.RawData.key);
            resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
            if (resp.success == false) {
                MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            identifiers = JsonConvert.DeserializeObject<HttpTypes.AllIdentifiers>(d);
            RefreshForm();
        }

        private void unbanUserToolStripMenuItem_Click(object sender, EventArgs e) {
            string d = Http.RawGet(Endpoints.UnbanIdentifier(sdata.script_id, selectedIdentifier.unban_token), "");
            
            MessageBox.Show(d);
            d = Http.RawGet(Endpoints.GetAllIdentifiers(sdata.script_id), api.RawData.key);
            HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
            if (resp.success == false) {
                MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            identifiers = JsonConvert.DeserializeObject<HttpTypes.AllIdentifiers>(d);
            RefreshForm();
        }
    }
}
