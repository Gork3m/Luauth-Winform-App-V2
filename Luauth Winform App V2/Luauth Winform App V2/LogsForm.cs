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
    public partial class LogsForm : Form {
        public LogsForm(string data) {
            InitializeComponent();
            richTextBox1.Text = data;
            richTextBox1.ReadOnly = true;
        }

        private void LogsForm_Load(object sender, EventArgs e) {

        }
    }
}
