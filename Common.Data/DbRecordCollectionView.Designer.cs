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
            this.SelectedListToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnSelectedAddRecord = new System.Windows.Forms.ToolStripButton();
            this.btnSelectedRemoveRecord = new System.Windows.Forms.ToolStripButton();
            this.btnSelectedAdvanced = new System.Windows.Forms.ToolStripDropDownButton();
            this.SelectedList = new Common.Data.FasterListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SelectedListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SelectedListToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectedListToolStrip
            // 
            this.SelectedListToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SelectedListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.SelectedListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelectedAddRecord,
            this.btnSelectedRemoveRecord,
            this.btnSelectedAdvanced});
            this.SelectedListToolStrip.Location = new System.Drawing.Point(0, 374);
            this.SelectedListToolStrip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.SelectedListToolStrip.Name = "SelectedListToolStrip";
            this.SelectedListToolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.SelectedListToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.SelectedListToolStrip.Size = new System.Drawing.Size(692, 28);
            this.SelectedListToolStrip.TabIndex = 5;
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
            listViewItem1,
            listViewItem2});
            this.SelectedList.LabelWrap = false;
            this.SelectedList.Location = new System.Drawing.Point(3, 3);
            this.SelectedList.Name = "SelectedList";
            this.SelectedList.Size = new System.Drawing.Size(686, 368);
            this.SelectedList.TabIndex = 4;
            this.SelectedList.UseCompatibleStateImageBehavior = false;
            this.SelectedList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 252;
            // 
            // SelectedListContextMenu
            // 
            this.SelectedListContextMenu.Name = "SelectedListContextMenu";
            this.SelectedListContextMenu.Size = new System.Drawing.Size(153, 26);
            this.SelectedListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.SelectedListContextMenu_Opening);
            // 
            // DbRecordCollectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SelectedListToolStrip);
            this.Controls.Add(this.SelectedList);
            this.Name = "DbRecordCollectionView";
            this.Size = new System.Drawing.Size(692, 402);
            this.SelectedListToolStrip.ResumeLayout(false);
            this.SelectedListToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip SelectedListToolStrip;
        protected System.Windows.Forms.ToolStripButton btnSelectedAddRecord;
        protected System.Windows.Forms.ToolStripButton btnSelectedRemoveRecord;
        protected System.Windows.Forms.ToolStripDropDownButton btnSelectedAdvanced;
        protected FasterListView SelectedList;
        protected System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip SelectedListContextMenu;
    }
}
