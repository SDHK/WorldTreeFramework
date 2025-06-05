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
			listBox1 = new ListBox();
			listView1 = new ListView();
			SuspendLayout();
			// 
			// treeView1
			// 
			treeView1.BackColor = Color.FromArgb(40, 40, 40);
			treeView1.Location = new Point(420, 12);
			treeView1.Name = "treeView1";
			treeView1.Size = new Size(121, 97);
			treeView1.TabIndex = 0;
			treeView1.AfterSelect += treeView1_AfterSelect;
			// 
			// listBox1
			// 
			listBox1.BackColor = Color.FromArgb(40, 40, 40);
			listBox1.FormattingEnabled = true;
			listBox1.ItemHeight = 17;
			listBox1.Location = new Point(421, 115);
			listBox1.Name = "listBox1";
			listBox1.Size = new Size(120, 89);
			listBox1.TabIndex = 1;
			listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
			// 
			// listView1
			// 
			listView1.BackColor = Color.FromArgb(40, 40, 40);
			listView1.Location = new Point(547, 12);
			listView1.Name = "listView1";
			listView1.Size = new Size(241, 192);
			listView1.TabIndex = 2;
			listView1.UseCompatibleStateImageBehavior = false;
			listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
			// 
			// InputMapperForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(30, 30, 30);
			ClientSize = new Size(800, 450);
			Controls.Add(listView1);
			Controls.Add(listBox1);
			Controls.Add(treeView1);
			Name = "InputMapperForm";
			Text = "InputMapperTool";
			Load += InputMapperForm_Load;
			ResumeLayout(false);
		}

		#endregion

		private TreeView treeView1;
		private ListBox listBox1;
		private ListView listView1;
	}
}
