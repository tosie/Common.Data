namespace Common.Data {
    partial class TreeViewForm {
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.lblText = new System.Windows.Forms.Label();
            this.Tree = new Common.Data.DbRecordTreeView();
            this.CollectionView = new Common.Data.DbRecordCollectionView();
            this.RecordView = new Common.Data.DbRecordView();
            this.CollectionWithValueView = new Common.Data.DbRecordCollectionWithValueView();
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
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(12, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.Tree);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.CollectionWithValueView);
            this.splitContainer.Panel2.Controls.Add(this.CollectionView);
            this.splitContainer.Panel2.Controls.Add(this.RecordView);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Size = new System.Drawing.Size(673, 322);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 4;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(39, 13);
            this.lblText.TabIndex = 5;
            this.lblText.Text = "<Title>";
            // 
            // Tree
            // 
            this.Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tree.Location = new System.Drawing.Point(3, 3);
            this.Tree.Name = "Tree";
            this.Tree.RootRecords = null;
            this.Tree.Size = new System.Drawing.Size(244, 316);
            this.Tree.TabIndex = 0;
            this.Tree.RecordSelected += new Common.Data.RecordEventHandler(this.Tree_RecordSelected);
            this.Tree.CollectionSelected += new Common.Data.CollectionEventHandler(this.Tree_CollectionSelected);
            // 
            // CollectionView
            // 
            this.CollectionView.Location = new System.Drawing.Point(28, 40);
            this.CollectionView.Name = "CollectionView";
            this.CollectionView.PropertyName = null;
            this.CollectionView.SelectedRecord = null;
            this.CollectionView.Size = new System.Drawing.Size(359, 232);
            this.CollectionView.TabIndex = 1;
            // 
            // RecordView
            // 
            this.RecordView.Location = new System.Drawing.Point(6, 6);
            this.RecordView.Name = "RecordView";
            this.RecordView.SelectedRecord = null;
            this.RecordView.Size = new System.Drawing.Size(324, 245);
            this.RecordView.TabIndex = 0;
            // 
            // CollectionWithValueView
            // 
            this.CollectionWithValueView.Location = new System.Drawing.Point(118, 99);
            this.CollectionWithValueView.Name = "CollectionWithValueView";
            this.CollectionWithValueView.Size = new System.Drawing.Size(235, 152);
            this.CollectionWithValueView.TabIndex = 2;
            // 
            // TreeViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 359);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.lblText);
            this.KeyPreview = true;
            this.Name = "TreeViewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Title>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeViewForm_FormClosing);
            this.Load += new System.EventHandler(this.TreeViewForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeViewForm_KeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.SplitContainer splitContainer;
        protected System.Windows.Forms.Label lblText;
        protected DbRecordTreeView Tree;
        protected DbRecordCollectionView CollectionView;
        protected DbRecordView RecordView;
        private DbRecordCollectionWithValueView CollectionWithValueView;

    }
}