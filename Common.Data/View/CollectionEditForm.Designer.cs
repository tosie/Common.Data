namespace Common.Data {
    partial class CollectionEditForm {
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
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Item 2");
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.List = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ListToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.smiRenameRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.smiDuplicateRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectedListToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnSelectedAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnSelectedRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnSelectedAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.SelectedList = new Common.Data.FasterListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblText = new System.Windows.Forms.Label();
            this.SelectedListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.ListToolStrip.SuspendLayout();
            this.SelectedListToolStrip.SuspendLayout();
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
            this.splitContainer.Panel2.Controls.Add(this.SelectedListToolStrip);
            this.splitContainer.Panel2.Controls.Add(this.SelectedList);
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
            this.smiRenameRecord.Size = new System.Drawing.Size(148, 22);
            this.smiRenameRecord.Tag = "SelectedRecord != null";
            this.smiRenameRecord.Text = "Umbenennen";
            this.smiRenameRecord.Click += new System.EventHandler(this.smiRenameRecord_Click);
            // 
            // smiDuplicateRecord
            // 
            this.smiDuplicateRecord.Name = "smiDuplicateRecord";
            this.smiDuplicateRecord.Size = new System.Drawing.Size(148, 22);
            this.smiDuplicateRecord.Tag = "SelectedRecord != null";
            this.smiDuplicateRecord.Text = "Duplizieren";
            this.smiDuplicateRecord.Click += new System.EventHandler(this.smiDuplicateRecord_Click);
            // 
            // SelectedListToolStrip
            // 
            this.SelectedListToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SelectedListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.SelectedListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelectedAddRecord,
            this.btnSelectedRemoveRecord,
            this.btnSelectedAdvanced});
            this.SelectedListToolStrip.Location = new System.Drawing.Point(3, 302);
            this.SelectedListToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SelectedListToolStrip.Name = "SelectedListToolStrip";
            this.SelectedListToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.SelectedListToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.SelectedListToolStrip.Size = new System.Drawing.Size(434, 28);
            this.SelectedListToolStrip.TabIndex = 3;
            // 
            // btnSelectedAddRecord
            // 
            this.btnSelectedAddRecord.AutoSize = false;
            this.btnSelectedAddRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelectedAddRecord.Image = global::Common.Data.Properties.Resources.CircularPlus;
            this.btnSelectedAddRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSelectedAddRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectedAddRecord.Name = "btnSelectedAddRecord";
            this.btnSelectedAddRecord.Size = new System.Drawing.Size(30, 22);
            this.btnSelectedAddRecord.Visible = false;
            this.btnSelectedAddRecord.Click += new System.EventHandler(this.btnSelectedAddRecord_Click);
            // 
            // btnSelectedRemoveRecord
            // 
            this.btnSelectedRemoveRecord.AutoSize = false;
            this.btnSelectedRemoveRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelectedRemoveRecord.Image = global::Common.Data.Properties.Resources.CircularMinus;
            this.btnSelectedRemoveRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSelectedRemoveRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectedRemoveRecord.Name = "btnSelectedRemoveRecord";
            this.btnSelectedRemoveRecord.Size = new System.Drawing.Size(30, 22);
            this.btnSelectedRemoveRecord.Visible = false;
            this.btnSelectedRemoveRecord.Click += new System.EventHandler(this.btnSelectedRemoveRecord_Click);
            // 
            // btnSelectedAdvanced
            // 
            this.btnSelectedAdvanced.AutoSize = false;
            this.btnSelectedAdvanced.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelectedAdvanced.Image = global::Common.Data.Properties.Resources.ActionGlyph;
            this.btnSelectedAdvanced.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSelectedAdvanced.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectedAdvanced.Name = "btnSelectedAdvanced";
            this.btnSelectedAdvanced.ShowDropDownArrow = false;
            this.btnSelectedAdvanced.Size = new System.Drawing.Size(40, 22);
            this.btnSelectedAdvanced.DropDownOpening += new System.EventHandler(this.btnSelectedAdvanced_DropDownOpening);
            // 
            // SelectedList
            // 
            this.SelectedList.AllowDrop = true;
            this.SelectedList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.SelectedList.ContextMenuStrip = this.SelectedListContextMenu;
            this.SelectedList.FullRowSelect = true;
            this.SelectedList.GridLines = true;
            this.SelectedList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SelectedList.HideSelection = false;
            this.SelectedList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.SelectedList.LabelWrap = false;
            this.SelectedList.Location = new System.Drawing.Point(6, 6);
            this.SelectedList.Name = "SelectedList";
            this.SelectedList.Size = new System.Drawing.Size(428, 293);
            this.SelectedList.TabIndex = 2;
            this.SelectedList.UseCompatibleStateImageBehavior = false;
            this.SelectedList.View = System.Windows.Forms.View.Details;
            this.SelectedList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.SelectedList_ItemDrag);
            this.SelectedList.DragDrop += new System.Windows.Forms.DragEventHandler(this.SelectedList_DragDrop);
            this.SelectedList.DragEnter += new System.Windows.Forms.DragEventHandler(this.SelectedList_DragEnter);
            this.SelectedList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectedList_KeyDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 252;
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
            // SelectedListContextMenu
            // 
            this.SelectedListContextMenu.Name = "SelectedListContextMenu";
            this.SelectedListContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // CollectionEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 370);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.lblText);
            this.KeyPreview = true;
            this.Name = "CollectionEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Title>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CollectionEditForm_FormClosing);
            this.Load += new System.EventHandler(this.ScenarioForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CollectionEditForm_KeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.ListToolStrip.ResumeLayout(false);
            this.ListToolStrip.PerformLayout();
            this.SelectedListToolStrip.ResumeLayout(false);
            this.SelectedListToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.SplitContainer splitContainer;
        protected System.Windows.Forms.ListView List;
        protected System.Windows.Forms.ColumnHeader columnHeader1;
        protected System.Windows.Forms.ToolStrip ListToolStrip;
        protected System.Windows.Forms.ToolStripButton btnAddRecord;
        protected System.Windows.Forms.ToolStripButton btnRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnAdvanced;
        protected System.Windows.Forms.ToolStripMenuItem smiDuplicateRecord;
        protected System.Windows.Forms.Label lblText;
        protected System.Windows.Forms.ToolStripMenuItem smiRenameRecord;
        protected FasterListView SelectedList;
        protected System.Windows.Forms.ColumnHeader columnHeader2;
        protected System.Windows.Forms.ToolStrip SelectedListToolStrip;
        protected System.Windows.Forms.ToolStripButton btnSelectedAddRecord;
        protected System.Windows.Forms.ToolStripButton btnSelectedRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnSelectedAdvanced;
        private System.Windows.Forms.ContextMenuStrip SelectedListContextMenu;

    }
}