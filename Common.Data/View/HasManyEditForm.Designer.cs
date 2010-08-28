namespace Common.Data {
    partial class HasManyEditForm {
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Item 2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Item 2");
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.SelectedList = new Common.Data.FasterListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.UnselectedList = new Common.Data.FasterListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(12, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.SelectedList);
            this.splitContainer.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.UnselectedList);
            this.splitContainer.Panel2.Controls.Add(this.label2);
            this.splitContainer.Size = new System.Drawing.Size(570, 286);
            this.splitContainer.SplitterDistance = 291;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.Resize += new System.EventHandler(this.splitContainer_Resize);
            // 
            // SelectedList
            // 
            this.SelectedList.AllowDrop = true;
            this.SelectedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.SelectedList.FullRowSelect = true;
            this.SelectedList.GridLines = true;
            this.SelectedList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.SelectedList.HideSelection = false;
            this.SelectedList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.SelectedList.LabelWrap = false;
            this.SelectedList.Location = new System.Drawing.Point(3, 19);
            this.SelectedList.Name = "SelectedList";
            this.SelectedList.Size = new System.Drawing.Size(285, 277);
            this.SelectedList.TabIndex = 1;
            this.SelectedList.UseCompatibleStateImageBehavior = false;
            this.SelectedList.View = System.Windows.Forms.View.Details;
            this.SelectedList.Resize += new System.EventHandler(this.List_Resize);
            this.SelectedList.DragDrop += new System.Windows.Forms.DragEventHandler(this.List_DragDrop);
            this.SelectedList.DragEnter += new System.Windows.Forms.DragEventHandler(this.List_DragEnter);
            this.SelectedList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.List_ItemDrag);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 252;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ausgewählt:";
            // 
            // UnselectedList
            // 
            this.UnselectedList.AllowDrop = true;
            this.UnselectedList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.UnselectedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.UnselectedList.FullRowSelect = true;
            this.UnselectedList.GridLines = true;
            this.UnselectedList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.UnselectedList.HideSelection = false;
            this.UnselectedList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.UnselectedList.LabelWrap = false;
            this.UnselectedList.Location = new System.Drawing.Point(3, 19);
            this.UnselectedList.Name = "UnselectedList";
            this.UnselectedList.Size = new System.Drawing.Size(269, 264);
            this.UnselectedList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.UnselectedList.TabIndex = 2;
            this.UnselectedList.UseCompatibleStateImageBehavior = false;
            this.UnselectedList.View = System.Windows.Forms.View.Details;
            this.UnselectedList.Resize += new System.EventHandler(this.List_Resize);
            this.UnselectedList.DragDrop += new System.Windows.Forms.DragEventHandler(this.List_DragDrop);
            this.UnselectedList.DragEnter += new System.Windows.Forms.DragEventHandler(this.List_DragEnter);
            this.UnselectedList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.List_ItemDrag);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 252;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Verfügbar:";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(39, 13);
            this.lblText.TabIndex = 4;
            this.lblText.Text = "<Title>";
            // 
            // HasManyEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 323);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.splitContainer);
            this.KeyPreview = true;
            this.Name = "HasManyEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Title>";
            this.Load += new System.EventHandler(this.HasManyEditForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HasManyEditForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HasManyEditForm_KeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Common.Data.FasterListView SelectedList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Common.Data.FasterListView UnselectedList;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lblText;
    }
}