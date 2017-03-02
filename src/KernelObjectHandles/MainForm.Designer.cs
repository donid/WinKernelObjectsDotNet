namespace KernelObjectHandles
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.rowViewModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colPID = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colHandle = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colHandleAttributes = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colObjectTypeIndex = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colObjectTypeString = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colObject = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colGrantedAccess = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colPrettyName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colProcessName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.simpleButtonShowShareFlags = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButtonRefresh = new DevExpress.XtraEditors.SimpleButton();
			this.checkEditHideInvalidHandles = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rowViewModelBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditHideInvalidHandles.Properties)).BeginInit();
			this.SuspendLayout();
			// 
			// gridControl1
			// 
			this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridControl1.DataSource = this.rowViewModelBindingSource;
			this.gridControl1.Location = new System.Drawing.Point(0, 37);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(1290, 602);
			this.gridControl1.TabIndex = 0;
			this.gridControl1.UseEmbeddedNavigator = true;
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			// 
			// rowViewModelBindingSource
			// 
			this.rowViewModelBindingSource.DataSource = typeof(KernelObjectHandles.RowViewModel);
			// 
			// gridView1
			// 
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPID,
            this.colHandle,
            this.colHandleAttributes,
            this.colObjectTypeIndex,
            this.colObjectTypeString,
            this.colObject,
            this.colName,
            this.colGrantedAccess,
            this.colPrettyName,
            this.colProcessName});
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsView.ShowAutoFilterRow = true;
			// 
			// colPID
			// 
			this.colPID.FieldName = "PID";
			this.colPID.Name = "colPID";
			this.colPID.OptionsColumn.FixedWidth = true;
			this.colPID.OptionsColumn.ReadOnly = true;
			this.colPID.Visible = true;
			this.colPID.VisibleIndex = 0;
			this.colPID.Width = 47;
			// 
			// colHandle
			// 
			this.colHandle.DisplayFormat.FormatString = "X";
			this.colHandle.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.colHandle.FieldName = "Handle";
			this.colHandle.Name = "colHandle";
			this.colHandle.OptionsColumn.FixedWidth = true;
			this.colHandle.OptionsColumn.ReadOnly = true;
			this.colHandle.Visible = true;
			this.colHandle.VisibleIndex = 2;
			this.colHandle.Width = 54;
			// 
			// colHandleAttributes
			// 
			this.colHandleAttributes.Caption = "Attributes";
			this.colHandleAttributes.FieldName = "HandleAttributes";
			this.colHandleAttributes.Name = "colHandleAttributes";
			this.colHandleAttributes.OptionsColumn.FixedWidth = true;
			this.colHandleAttributes.OptionsColumn.ReadOnly = true;
			this.colHandleAttributes.Visible = true;
			this.colHandleAttributes.VisibleIndex = 5;
			this.colHandleAttributes.Width = 69;
			// 
			// colObjectTypeIndex
			// 
			this.colObjectTypeIndex.Caption = "TypeIndex";
			this.colObjectTypeIndex.FieldName = "ObjectTypeIndex";
			this.colObjectTypeIndex.Name = "colObjectTypeIndex";
			this.colObjectTypeIndex.OptionsColumn.FixedWidth = true;
			this.colObjectTypeIndex.OptionsColumn.ReadOnly = true;
			this.colObjectTypeIndex.Visible = true;
			this.colObjectTypeIndex.VisibleIndex = 4;
			this.colObjectTypeIndex.Width = 66;
			// 
			// colObjectTypeString
			// 
			this.colObjectTypeString.Caption = "ObjectType";
			this.colObjectTypeString.FieldName = "ObjectTypeString";
			this.colObjectTypeString.Name = "colObjectTypeString";
			this.colObjectTypeString.OptionsColumn.FixedWidth = true;
			this.colObjectTypeString.OptionsColumn.ReadOnly = true;
			this.colObjectTypeString.Visible = true;
			this.colObjectTypeString.VisibleIndex = 3;
			this.colObjectTypeString.Width = 78;
			// 
			// colObject
			// 
			this.colObject.AppearanceCell.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colObject.AppearanceCell.Options.UseFont = true;
			this.colObject.DisplayFormat.FormatString = "X16";
			this.colObject.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.colObject.FieldName = "Object";
			this.colObject.Name = "colObject";
			this.colObject.OptionsColumn.FixedWidth = true;
			this.colObject.OptionsColumn.ReadOnly = true;
			this.colObject.Visible = true;
			this.colObject.VisibleIndex = 7;
			this.colObject.Width = 135;
			// 
			// colName
			// 
			this.colName.FieldName = "Name";
			this.colName.Name = "colName";
			this.colName.Visible = true;
			this.colName.VisibleIndex = 8;
			this.colName.Width = 367;
			// 
			// colGrantedAccess
			// 
			this.colGrantedAccess.Caption = "Access";
			this.colGrantedAccess.DisplayFormat.FormatString = "X8";
			this.colGrantedAccess.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.colGrantedAccess.FieldName = "GrantedAccess";
			this.colGrantedAccess.Name = "colGrantedAccess";
			this.colGrantedAccess.OptionsColumn.FixedWidth = true;
			this.colGrantedAccess.Visible = true;
			this.colGrantedAccess.VisibleIndex = 6;
			this.colGrantedAccess.Width = 64;
			// 
			// colPrettyName
			// 
			this.colPrettyName.FieldName = "PrettyName";
			this.colPrettyName.Name = "colPrettyName";
			this.colPrettyName.Visible = true;
			this.colPrettyName.VisibleIndex = 9;
			this.colPrettyName.Width = 392;
			// 
			// colProcessName
			// 
			this.colProcessName.Caption = "Process";
			this.colProcessName.FieldName = "ProcessName";
			this.colProcessName.Name = "colProcessName";
			this.colProcessName.Visible = true;
			this.colProcessName.VisibleIndex = 1;
			// 
			// simpleButtonShowShareFlags
			// 
			this.simpleButtonShowShareFlags.Location = new System.Drawing.Point(333, 8);
			this.simpleButtonShowShareFlags.Name = "simpleButtonShowShareFlags";
			this.simpleButtonShowShareFlags.Size = new System.Drawing.Size(98, 23);
			this.simpleButtonShowShareFlags.TabIndex = 1;
			this.simpleButtonShowShareFlags.Text = "ShowShareFlags";
			this.simpleButtonShowShareFlags.Click += new System.EventHandler(this.simpleButtonShowShareFlags_Click);
			// 
			// simpleButtonRefresh
			// 
			this.simpleButtonRefresh.Location = new System.Drawing.Point(12, 8);
			this.simpleButtonRefresh.Name = "simpleButtonRefresh";
			this.simpleButtonRefresh.Size = new System.Drawing.Size(75, 23);
			this.simpleButtonRefresh.TabIndex = 2;
			this.simpleButtonRefresh.Text = "Refresh";
			this.simpleButtonRefresh.Click += new System.EventHandler(this.simpleButtonRefresh_Click);
			// 
			// checkEditHideInvalidHandles
			// 
			this.checkEditHideInvalidHandles.EditValue = true;
			this.checkEditHideInvalidHandles.Location = new System.Drawing.Point(112, 10);
			this.checkEditHideInvalidHandles.Name = "checkEditHideInvalidHandles";
			this.checkEditHideInvalidHandles.Properties.Caption = "HideInvalidHandles";
			this.checkEditHideInvalidHandles.Size = new System.Drawing.Size(117, 19);
			this.checkEditHideInvalidHandles.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1290, 640);
			this.Controls.Add(this.checkEditHideInvalidHandles);
			this.Controls.Add(this.simpleButtonRefresh);
			this.Controls.Add(this.simpleButtonShowShareFlags);
			this.Controls.Add(this.gridControl1);
			this.Name = "Form1";
			this.Text = "KernelObjectHandles";
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rowViewModelBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditHideInvalidHandles.Properties)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraGrid.GridControl gridControl1;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private System.Windows.Forms.BindingSource rowViewModelBindingSource;
		private DevExpress.XtraGrid.Columns.GridColumn colPID;
		private DevExpress.XtraGrid.Columns.GridColumn colHandle;
		private DevExpress.XtraGrid.Columns.GridColumn colHandleAttributes;
		private DevExpress.XtraGrid.Columns.GridColumn colObjectTypeIndex;
		private DevExpress.XtraGrid.Columns.GridColumn colObjectTypeString;
		private DevExpress.XtraGrid.Columns.GridColumn colObject;
		private DevExpress.XtraGrid.Columns.GridColumn colName;
		private DevExpress.XtraGrid.Columns.GridColumn colGrantedAccess;
		private DevExpress.XtraGrid.Columns.GridColumn colPrettyName;
		private DevExpress.XtraGrid.Columns.GridColumn colProcessName;
		private DevExpress.XtraEditors.SimpleButton simpleButtonShowShareFlags;
		private DevExpress.XtraEditors.SimpleButton simpleButtonRefresh;
		private DevExpress.XtraEditors.CheckEdit checkEditHideInvalidHandles;
	}
}

