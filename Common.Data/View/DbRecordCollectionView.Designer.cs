namespace Common.Data {
    partial class DbRecordCollectionView {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Item 2");
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.List = new Common.Data.FasterListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.ToolStrip.Location = new System.Drawing.Point(0, 374);
            this.ToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ToolStrip.Size = new System.Drawing.Size(692, 28);
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
            // contextMenu
            // 
            this.contextMenu.Name = "SelectedListContextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // List
            // 
            this.List.AllowDrop = true;
            this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.List.ContextMenuStrip = this.contextMenu;
            this.List.FullRowSelect = true;
            this.List.GridLines = true;
            this.List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.List.HideSelection = false;
            this.List.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.List.LabelWrap = false;
            this.List.Location = new System.Drawing.Point(3, 3);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(686, 368);
            this.List.TabIndex = 4;
            this.List.UseCompatibleStateImageBehavior = false;
            this.List.View = System.Windows.Forms.View.Details;
            this.List.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.SelectedList_ItemDrag);
            this.List.DragDrop += new System.Windows.Forms.DragEventHandler(this.SelectedList_DragDrop);
            this.List.DragEnter += new System.Windows.Forms.DragEventHandler(this.SelectedList_DragEnter);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 252;
            // 
            // DbRecordCollectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.List);
            this.Name = "DbRecordCollectionView";
            this.Size = new System.Drawing.Size(692, 402);
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
        protected FasterListView List;
        protected System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
    }
}
