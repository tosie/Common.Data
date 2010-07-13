using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Data.View {
    public partial class StatusPanel : UserControl {

        #region Properties

        [DefaultValue("Status")]
        public String Message {
            get {
                return lblProgress.Text;
            }

            set {
                lblProgress.Text = value;
            }
        }

        [DefaultValue(false)]
        public Boolean SupportsCancellation {
            get {
                return btnCancel.Visible;
            }

            set {
                btnCancel.Visible = value;
            }
        }

        #endregion

        #region Events

        public event EventHandler Cancel;
        protected void RaiseCancel() {
            if (Cancel == null)
                return;

            Cancel(this, new EventArgs());
        }

        #endregion

        #region Constructor

        public StatusPanel() {
            InitializeDefaultValues();
            InitializeComponent();
        }

        protected void InitializeDefaultValues() {
            lblProgress.Text = "Status";
            btnCancel.Hide();
        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e) {
            RaiseCancel();
        }

    }
}
