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


namespace Luauth_Winform_App_V2 {
    public partial class AddUser : Form {
        HttpTypes.ScriptDetails sdata;
        LuauthWrapper api;
        public AddUser(string sname, HttpTypes.ScriptDetails sdata, LuauthWrapper api) {
            InitializeComponent();
            Text = "Adding user to " + sname;
            this.sdata = sdata;
            this.api = api;
        }
        public bool done = false;
        private void AddUser_Load(object sender, EventArgs e) {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            dateTimePicker1.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox2.Text.Length < 3) {
                MessageBox.Show("Invalid identifier");
                return;
            }
            string d = Http.RawPost(Endpoints.WhitelistIdentifier(sdata.script_id), "POST", JsonConvert.SerializeObject(new HttpTypes.AddIdentifier() { identifier = textBox2.Text, note = richTextBox1.Text, auth_expire = ((!checkBox1.Checked) ? 1 : (int)(dateTimePicker1.Value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds))}) ,api.RawData.key);
            HttpTypes.APIResponse resp = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(d);
            if (resp.success == false) {
                MessageBox.Show(resp.message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            done = true;
            MessageBox.Show(resp.message);
            this.Close();
        }
    }
}
