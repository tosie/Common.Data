namespace Common.Data {
    partial class DbRecordList {
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
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Item 2");
            this.List = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ListToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.smiRenameRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.smiDuplicateRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.ListToolStrip.SuspendLayout();
            this.SuspendLayout();
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
            listViewItem3,
            listViewItem4});
            this.List.LabelEdit = true;
            this.List.LabelWrap = false;
            this.List.Location = new System.Drawing.Point(3, 3);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(462, 422);
            this.List.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.List.TabIndex = 3;
            this.List.UseCompatibleStateImageBehavior = false;
            this.List.View = System.Windows.Forms.View.Details;
            this.List.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.List_AfterLabelEdit);
            this.List.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_ItemSelectionChanged);
            this.List.KeyDown += new System.Windows.Forms.KeyEventHandler(this.List_KeyDown);
            this.List.Resize += new System.EventHandler(this.List_Resize);
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
            this.btnAdvanced});
            this.ListToolStrip.Location = new System.Drawing.Point(0, 428);
            this.ListToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ListToolStrip.Name = "ListToolStrip";
            this.ListToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ListToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ListToolStrip.Size = new System.Drawing.Size(468, 28);
            this.ListToolStrip.TabIndex = 4;
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
            this.btnAdvanced.DropDownOpening += new System.EventHandler(this.btnAdvanced_DropDownOpening);
            // 
            // smiRenameRecord
            // 
            this.smiRenameRecord.Name = "smiRenameRecord";
            this.smiRenameRecord.Size = new System.Drawing.Size(152, 22);
            this.smiRenameRecord.Tag = "SelectedRecord != null";
            this.smiRenameRecord.Text = "Umbenennen";
            this.smiRenameRecord.Click += new System.EventHandler(this.smiRenameRecord_Click);
            // 
            // smiDuplicateRecord
            // 
            this.smiDuplicateRecord.Name = "smiDuplicateRecord";
            this.smiDuplicateRecord.Size = new System.Drawing.Size(152, 22);
            this.smiDuplicateRecord.Tag = "SelectedRecord != null";
            this.smiDuplicateRecord.Text = "Duplizieren";
            this.smiDuplicateRecord.Click += new System.EventHandler(this.smiDuplicateRecord_Click);
            // 
            // DbRecordList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.List);
            this.Controls.Add(this.ListToolStrip);
            this.Name = "DbRecordList";
            this.Size = new System.Drawing.Size(468, 456);
            this.ListToolStrip.ResumeLayout(false);
            this.ListToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ListView List;
        protected System.Windows.Forms.ColumnHeader columnHeader1;
        protected System.Windows.Forms.ToolStrip ListToolStrip;
        protected System.Windows.Forms.ToolStripButton btnAddRecord;
        protected System.Windows.Forms.ToolStripButton btnRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnAdvanced;
        protected System.Windows.Forms.ToolStripMenuItem smiRenameRecord;
        protected System.Windows.Forms.ToolStripMenuItem smiDuplicateRecord;
    }
}
