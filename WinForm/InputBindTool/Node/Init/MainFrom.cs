using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WorldTree
{

	/// <summary>
	/// WinForm入口法则
	/// </summary>
	public interface WinFromEntry : ISendRule<Form>, IGlobalRule { }


	/// <summary>
	/// 窗口根节点
	/// </summary>
	public class MainFrom : Node
		, AsComponentBranch
		, ComponentOf<MainWorld>
		, AsAwake
		, AsWinFromEntry
	{


	}

	/// <summary>
	/// 123
	/// </summary>
	public class TabControl1 : TabControl
	{
		/// <summary>
		/// a
		/// </summary>
		public Color TabBorderColor { get; set; } = Color.FromArgb(60, 60, 60);
		/// <summary>
		/// a
		/// </summary>
		public Color TabSelectedColor { get; set; } = Color.FromArgb(60, 60, 60);
		/// <summary>
		/// a
		/// </summary>
		public Color TabUnselectedColor { get; set; } = Color.FromArgb(30, 30, 30);
		/// <summary>
		/// a
		/// </summary>
		public Color TabTextColor { get; set; } = Color.White;

		/// <summary>
		/// 攒机
		/// </summary>
		public Color BackColor1 = Color.FromArgb(30, 30, 30);

		/// <summary>
		/// 文本颜色
		/// </summary>
		public Color TextColor = Color.White;

		/// <summary>
		/// 攒机
		/// </summary>
		public Pen Pen;

		/// <summary>
		/// q
		/// </summary>
		public SolidBrush Brush;

		public TabControl1() : base()
		{
			this.DrawMode = TabDrawMode.OwnerDrawFixed; // 必须加上这一行！

			Brush = new SolidBrush(BackColor1);
			Pen = new Pen(BackColor1, 4);
			this.DrawItem += (sender, e) =>
			{


				TabControl tab = sender as TabControl;
				Rectangle rect = tab.GetTabRect(e.Index);

				// 示例：根据索引设置不同的边框颜色
				Color borderColor = e.Index == 0 ? Color.Coral : Color.MediumPurple;


				// 设置选中和未选中的背景颜色
				Rectangle recMain = this.ClientRectangle;        //获取Table控件的工作区域
				e.Graphics.FillRectangle(Brush, recMain);          //绘制TabControl背景




				e.Graphics.FillRectangle(Brush, rect);
				e.Graphics.DrawRectangle(Pen, rect.X, rect.Y, rect.Width, rect.Height);


				TextRenderer.DrawText(
				e.Graphics,
				tab.TabPages[e.Index].Text,
				tab.Font,
				rect,
					TextColor,
					TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
			};
		}



	}
	/// <summary>
	/// a
	/// </summary>
	public class CustomTabPage : TabPage
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (Pen pen = new Pen(Color.FromArgb(30, 30, 30), 2))
			{
				e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
			}
		}
	}

	/// <summary>
	/// a
	/// </summary>
	public class NavigationPage : Panel
	{
		/// <summary>
		/// 初始化新的 <see cref="NavigationPage"/> 实例。
		/// </summary>
		public NavigationPage()
		{
			BackColor = SystemColors.Window;
			Visible = false; // 默认就不可见，可见度应该由那个 TreeView 控制，不然会增加 UI 压力
		}
	}

	/// <summary>
	/// a
	/// </summary>
	public sealed class ListBox1 : ListBox
	{


	}


	/// <summary>
	/// a
	/// </summary>
	public sealed class NavigationBar : TreeView
	{
		/// <summary>
		/// 初始化新的竖直导航栏实例。
		/// </summary>
		public NavigationBar(params string[] navItems)
		{
			BorderStyle = BorderStyle.None;
			Dock = DockStyle.Fill;
			FullRowSelect = true;
			HideSelection = false;
			HotTracking = true;
			ShowLines = false;
			ItemHeight = 25;
			Indent = 5;
			for (int i = 0; i < navItems.Length; i++)
			{
				Nodes.Add(navItems[i]); // 添加导航项

				Nodes[i].Nodes.Add("子项1"); // 添加子项
			}

			//ExpandAll();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			SetWindowTheme(Handle, "Explorer", null); // 保持原生外观
			base.OnHandleCreated(e);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		// 这个是关键，当你选中一个节点后，这个事件就会触发，e 里面包含了选中的是哪个节点
		{
			SwitchTo(e.Node);
			base.OnAfterSelect(e);
		}

		/// <summary>
		/// a
		/// </summary>
		/// <param name="navItem"></param>
		private void SwitchTo(TreeNode navItem)
		{
			var index = navItem.Index; // TreeNode (导航项) 也有索引，且与导航页的索引一致
			SelectedNode = navItem; // 同步导航栏选中项，用于编程设置选中的导航页
		}


		/// <summary>
		/// 设置窗口主题。
		/// </summary>
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

	}


}