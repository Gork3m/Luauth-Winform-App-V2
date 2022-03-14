using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Luauth_Winform_App_V2 {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            string key = "";
            if (File.Exists("apikey.txt")) {
                key = File.ReadAllText("apikey.txt");
            }
            KeyForm f = new KeyForm(key);
            Application.Run(f);

            if (f != null && f.session != null && f.session.Initialized) {
                File.WriteAllText("apikey.txt", f.session.RawData.key);
                Application.Run(new MainForm(f.session));
            }
        }
    }
}
