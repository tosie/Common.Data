namespace Common.Data {
    partial class DbRecordCollectionWithValueView {
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
            this.Matrix = new Common.Configuration.ValueMatrix();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Matrix
            // 
            this.Matrix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Matrix.ConfigEntry = null;
            this.Matrix.Location = new System.Drawing.Point(0, 0);
            this.Matrix.Margin = new System.Windows.Forms.Padding(0);
            this.Matrix.Name = "Matrix";
            this.Matrix.Size = new System.Drawing.Size(485, 268);
            this.Matrix.TabIndex = 0;
            // 
            // ToolStrip
            // 
            this.ToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddRecord,
            this.btnRemoveRecord,
            this.btnAdvanced});
            this.ToolStrip.Location = new System.Drawing.Point(0, 268);
            this.ToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ToolStrip.Size = new System.Drawing.Size(485, 28);
            this.ToolStrip.TabIndex = 6;
            // 
            // btnAddRecord
            // 
            this.btnAddRecord.AutoSize = false;
            this.btnAddRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddRecord.Image = global::Common.Data.Properties.Resources.CircularPlus;
            this.btnAddRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddRecord.Name = "btnAddRecord";
            this.btnAddRecord.Size = new System.Drawing.Size(30, 22);
            this.btnAddRecord.Visible = false;
            // 
            // btnRemoveRecord
            // 
            this.btnRemoveRecord.AutoSize = false;
            this.btnRemoveRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveRecord.Image = global::Common.Data.Properties.Resources.CircularMinus;
            this.btnRemoveRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRemoveRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveRecord.Name = "btnRemoveRecord";
            this.btnRemoveRecord.Size = new System.Drawing.Size(30, 22);
            this.btnRemoveRecord.Visible = false;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.AutoSize = false;
            this.btnAdvanced.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdvanced.Image = global::Common.Data.Properties.Resources.ActionGlyph;
            this.btnAdvanced.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAdvanced.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.ShowDropDownArrow = false;
            this.btnAdvanced.Size = new System.Drawing.Size(40, 22);
            // 
            // DbRecordCollectionWithValueView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.Matrix);
            this.Name = "DbRecordCollectionWithValueView";
            this.Size = new System.Drawing.Size(485, 296);
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Configuration.ValueMatrix Matrix;
        protected System.Windows.Forms.ToolStrip ToolStrip;
        protected System.Windows.Forms.ToolStripButton btnAddRecord;
        protected System.Windows.Forms.ToolStripButton btnRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnAdvanced;
    }
}
