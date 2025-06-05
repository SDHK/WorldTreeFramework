namespace WorldTree
{
	/// <summary>
	/// WinForm日志
	/// </summary>
	public class WinFormWorldLog : Unit, ILog
	{
		public void Trace(string msg) => MessageBox.Show(msg);

		public void Debug(string msg) => MessageBox.Show(msg);

		public void Todo(string msg) => MessageBox.Show("TODO: " + msg);

		public void Info(string msg) => MessageBox.Show(msg);

		public void Warning(string msg) => MessageBox.Show(msg);

		public void Error(string msg) => MessageBox.Show(msg);

		public void Error(Exception e) => MessageBox.Show(e.ToString());
	}

}