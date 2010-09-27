using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Common.Data.Test.GUI {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // Setup the default repository
            SharedObjects.Instance.SetupRepository("Data");

            // FormData belongs to user-specific settings and uses a special repository for that
            FormData.ActiveRepository = SharedObjects.Instance.OpenRepository("Preferences");
            AppSetting.ActiveRepository = FormData.ActiveRepository;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
