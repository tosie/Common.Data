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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.RecordList = new Common.Data.DbRecordList();
            this.Collection = new Common.Data.DbRecordCollectionView();
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
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(12, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.RecordList);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.Collection);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Size = new System.Drawing.Size(694, 333);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 2;
            // 
            // RecordList
            // 
            this.RecordList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecordList.Location = new System.Drawing.Point(3, 3);
            this.RecordList.Name = "RecordList";
            this.RecordList.PreloadedRecords = null;
            this.RecordList.RecordType = null;
            this.RecordList.Size = new System.Drawing.Size(244, 327);
            this.RecordList.TabIndex = 0;
            this.RecordList.AddingRecord += new Common.Data.RecordEventHandler(this.RecordList_AddingRecord);
            this.RecordList.AddedRecord += new Common.Data.RecordEventHandler(this.RecordList_AddedRecord);
            this.RecordList.DeletingRecord += new Common.Data.RecordEventHandler(this.RecordList_DeletingRecord);
            this.RecordList.DeletedRecord += new Common.Data.RecordEventHandler(this.RecordList_DeletedRecord);
            this.RecordList.RecordSelected += new Common.Data.RecordEventHandler(this.RecordList_RecordSelected);
            // 
            // Collection
            // 
            this.Collection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Collection.Location = new System.Drawing.Point(3, 3);
            this.Collection.Name = "Collection";
            this.Collection.SelectedRecord = null;
            this.Collection.Size = new System.Drawing.Size(434, 327);
            this.Collection.TabIndex = 0;
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
            this.Load += new System.EventHandler(this.CollectionEditForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CollectionEditForm_KeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.SplitContainer splitContainer;
        protected System.Windows.Forms.Label lblText;
        private DbRecordCollectionView Collection;
        private DbRecordList RecordList;

    }
}