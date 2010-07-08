using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data {
    class Messages {

        public static void ErrorMessage(IWin32Window Owner, String Message) {
            MessageBox.Show(Owner, Message, "Fehler",
                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        public static bool YesNoQuestion(IWin32Window Owner, String Title, String Message) {
            return MessageBox.Show(Owner, Message, Title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        public static void Info(IWin32Window Owner, String Title, String Message) {
            MessageBox.Show(Owner, Message, Title,
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

    }
}
