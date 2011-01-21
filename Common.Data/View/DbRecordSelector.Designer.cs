namespace Common.Data {
    partial class DbRecordSelector {
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Item 2");
            this.List = new Common.Data.FasterListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.List.FullRowSelect = true;
            this.List.GridLines = true;
            this.List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.List.HideSelection = false;
            this.List.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.List.LabelWrap = false;
            this.List.Location = new System.Drawing.Point(3, 3);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(713, 400);
            this.List.TabIndex = 5;
            this.List.UseCompatibleStateImageBehavior = false;
            this.List.View = System.Windows.Forms.View.Details;
            this.List.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_ItemSelectionChanged);
            this.List.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.List_MouseDoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 252;
            // 
            // DbRecordSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.List);
            this.Name = "DbRecordSelector";
            this.Size = new System.Drawing.Size(719, 406);
            this.ResumeLayout(false);

        }

        #endregion

        protected FasterListView List;
        protected System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
