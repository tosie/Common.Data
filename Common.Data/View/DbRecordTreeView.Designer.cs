namespace Common.Data {
    partial class DbRecordTreeView {
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
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.smiRenameRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.smiDuplicateRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.Tree = new System.Windows.Forms.TreeView();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddRecord,
            this.btnRemoveRecord,
            this.btnAdvanced});
            this.ToolStrip.Location = new System.Drawing.Point(0, 297);
            this.ToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ToolStrip.Size = new System.Drawing.Size(235, 28);
            this.ToolStrip.TabIndex = 5;
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
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.AutoSize = false;
            this.btnAdvanced.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smiRenameRecord,
            this.smiDuplicateRecord});
            this.btnAdvanced.Image = global::Common.Data.Properties.Resources.ActionGlyph;
            this.btnAdvanced.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAdvanced.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.ShowDropDownArrow = false;
            this.btnAdvanced.Size = new System.Drawing.Size(40, 22);
            // 
            // smiRenameRecord
            // 
            this.smiRenameRecord.Name = "smiRenameRecord";
            this.smiRenameRecord.Size = new System.Drawing.Size(148, 22);
            this.smiRenameRecord.Tag = "SelectedRecord != null";
            this.smiRenameRecord.Text = "Umbenennen";
            // 
            // smiDuplicateRecord
            // 
            this.smiDuplicateRecord.Name = "smiDuplicateRecord";
            this.smiDuplicateRecord.Size = new System.Drawing.Size(148, 22);
            this.smiDuplicateRecord.Tag = "SelectedRecord != null";
            this.smiDuplicateRecord.Text = "Duplizieren";
            // 
            // Tree
            // 
            this.Tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Tree.FullRowSelect = true;
            this.Tree.HideSelection = false;
            this.Tree.LabelEdit = true;
            this.Tree.Location = new System.Drawing.Point(3, 3);
            this.Tree.Name = "Tree";
            this.Tree.ShowNodeToolTips = true;
            this.Tree.Size = new System.Drawing.Size(229, 291);
            this.Tree.TabIndex = 6;
            this.Tree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.Tree_BeforeLabelEdit);
            this.Tree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.Tree_AfterLabelEdit);
            this.Tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.Tree_BeforeExpand);
            this.Tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Tree_AfterSelect);
            // 
            // DbRecordTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tree);
            this.Controls.Add(this.ToolStrip);
            this.Name = "DbRecordTreeView";
            this.Size = new System.Drawing.Size(235, 325);
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip ToolStrip;
        protected System.Windows.Forms.ToolStripButton btnAddRecord;
        protected System.Windows.Forms.ToolStripButton btnRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnAdvanced;
        protected System.Windows.Forms.ToolStripMenuItem smiRenameRecord;
        protected System.Windows.Forms.ToolStripMenuItem smiDuplicateRecord;
        private System.Windows.Forms.TreeView Tree;
    }
}
