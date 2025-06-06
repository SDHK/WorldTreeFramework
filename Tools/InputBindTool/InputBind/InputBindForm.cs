namespace InputBind
{
	public partial class InputBindForm : Form
	{
		public bool IsOpen = true;

		public InputBindForm()
		{
			InitializeComponent();

			this.FormClosed += (s, args) =>
			{
				IsOpen = false;
			};

		}

		private void Form1_Load(object sender, EventArgs e)
		{


		}

		private void button1_Click(object sender, EventArgs e)
		{
			//Controls
		}
	}
}
