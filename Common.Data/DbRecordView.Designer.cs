namespace Common.Data {
    partial class DbRecordView {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ConfigControl = new Common.Configuration.ConfigurationControl();
            this.SuspendLayout();
            // 
            // ConfigControl
            // 
            this.ConfigControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigControl.AutoSave = true;
            this.ConfigControl.BackColor = System.Drawing.SystemColors.Window;
            this.ConfigControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ConfigControl.Configuration = null;
            this.ConfigControl.Location = new System.Drawing.Point(3, 3);
            this.ConfigControl.MultipleConfigs = null;
            this.ConfigControl.Name = "ConfigControl";
            this.ConfigControl.Size = new System.Drawing.Size(812, 480);
            this.ConfigControl.TabIndex = 1;
            // 
            // DbRecordView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ConfigControl);
            this.Name = "DbRecordView";
            this.Size = new System.Drawing.Size(818, 486);
            this.ResumeLayout(false);

        }

        #endregion

        protected Configuration.ConfigurationControl ConfigControl;
    }
}
