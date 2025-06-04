namespace InputMapperTool
{
	partial class InputMapperForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			treeView1 = new TreeView();
			SuspendLayout();
			// 
			// treeView1
			// 
			treeView1.Location = new Point(78, 45);
			treeView1.Name = "treeView1";
			treeView1.Size = new Size(121, 97);
			treeView1.TabIndex = 0;
			treeView1.AfterSelect += treeView1_AfterSelect;
			// 
			// InputMapperForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(30, 30, 30);
			ClientSize = new Size(800, 450);
			Controls.Add(treeView1);
			Name = "InputMapperForm";
			Text = "InputMapperTool";
			Load += InputMapperForm_Load;
			ResumeLayout(false);
		}

		#endregion

		private TreeView treeView1;
	}
}
