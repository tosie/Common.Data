namespace Common.Data {
    partial class DbRecordEditForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Item 2");
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.List = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ListToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRecordAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.smiRenameRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.smiDuplicateRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigControl = new Common.Configuration.ConfigurationControl();
            this.lblText = new System.Windows.Forms.Label();
            this.cmsLinkLabels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.ListToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(12, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.List);
            this.splitContainer.Panel1.Controls.Add(this.ListToolStrip);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.ConfigControl);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Size = new System.Drawing.Size(694, 333);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 2;
            // 
            // List
            // 
            this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.List.FullRowSelect = true;
            this.List.GridLines = true;
            this.List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.List.HideSelection = false;
            this.List.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.List.LabelEdit = true;
            this.List.LabelWrap = false;
            this.List.Location = new System.Drawing.Point(6, 6);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(238, 293);
            this.List.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.List.TabIndex = 0;
            this.List.UseCompatibleStateImageBehavior = false;
            this.List.View = System.Windows.Forms.View.Details;
            this.List.Resize += new System.EventHandler(this.List_Resize);
            this.List.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.List_AfterLabelEdit);
            this.List.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_ItemSelectionChanged);
            this.List.KeyDown += new System.Windows.Forms.KeyEventHandler(this.List_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 252;
            // 
            // ListToolStrip
            // 
            this.ListToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddRecord,
            this.btnRemoveRecord,
            this.btnRecordAdvanced});
            this.ListToolStrip.Location = new System.Drawing.Point(3, 302);
            this.ListToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ListToolStrip.Name = "ListToolStrip";
            this.ListToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ListToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ListToolStrip.Size = new System.Drawing.Size(244, 28);
            this.ListToolStrip.TabIndex = 2;
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
            this.btnAddRecord.Click += new System.EventHandler(this.btnAddRecord_Click);
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
            this.btnRemoveRecord.Click += new System.EventHandler(this.btnDeleteRecord_Click);
            // 
            // btnRecordAdvanced
            // 
            this.btnRecordAdvanced.AutoSize = false;
            this.btnRecordAdvanced.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRecordAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smiRenameRecord,
            this.smiDuplicateRecord});
            this.btnRecordAdvanced.Image = global::Common.Data.Properties.Resources.ActionGlyph;
            this.btnRecordAdvanced.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRecordAdvanced.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecordAdvanced.Name = "btnRecordAdvanced";
            this.btnRecordAdvanced.ShowDropDownArrow = false;
            this.btnRecordAdvanced.Size = new System.Drawing.Size(40, 22);
            this.btnRecordAdvanced.DropDownOpening += new System.EventHandler(this.btnRecordAdvanced_DropDownOpening);
            // 
            // smiRenameRecord
            // 
            this.smiRenameRecord.Name = "smiRenameRecord";
            this.smiRenameRecord.Size = new System.Drawing.Size(148, 22);
            this.smiRenameRecord.Tag = "SelectedRecord != null";
            this.smiRenameRecord.Text = "Umbenennen";
            this.smiRenameRecord.Click += new System.EventHandler(this.smiRenameScenario_Click);
            // 
            // smiDuplicateRecord
            // 
            this.smiDuplicateRecord.Name = "smiDuplicateRecord";
            this.smiDuplicateRecord.Size = new System.Drawing.Size(148, 22);
            this.smiDuplicateRecord.Tag = "SelectedRecord != null";
            this.smiDuplicateRecord.Text = "Duplizieren";
            this.smiDuplicateRecord.Click += new System.EventHandler(this.smiDuplicateScenario_Click);
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
            this.ConfigControl.Location = new System.Drawing.Point(6, 6);
            this.ConfigControl.MultipleConfigs = null;
            this.ConfigControl.Name = "ConfigControl";
            this.ConfigControl.Size = new System.Drawing.Size(428, 321);
            this.ConfigControl.TabIndex = 0;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(39, 13);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "<Title>";
            // 
            // cmsLinkLabels
            // 
            this.cmsLinkLabels.Name = "cmsModels";
            this.cmsLinkLabels.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsLinkLabels.ShowCheckMargin = true;
            this.cmsLinkLabels.ShowImageMargin = false;
            this.cmsLinkLabels.Size = new System.Drawing.Size(61, 4);
            // 
            // DbRecordEditFormForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 370);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.lblText);
            this.KeyPreview = true;
            this.Name = "DbRecordEditFormForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Title>";
            this.Load += new System.EventHandler(this.ScenarioForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DbRecordEditFormForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DbRecordEditFormForm_KeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ListToolStrip.ResumeLayout(false);
            this.ListToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView List;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStrip ListToolStrip;
        private System.Windows.Forms.ToolStripButton btnAddRecord;
        private System.Windows.Forms.ToolStripButton btnRemoveRecord;
        private System.Windows.Forms.ToolStripDropDownButton btnRecordAdvanced;
        private System.Windows.Forms.ToolStripMenuItem smiDuplicateRecord;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ContextMenuStrip cmsLinkLabels;
        private System.Windows.Forms.ToolStripMenuItem smiRenameRecord;
        private Common.Configuration.ConfigurationControl ConfigControl;
    }
}