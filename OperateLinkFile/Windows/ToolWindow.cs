using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace OperateLinkFile
{
	/// <summary>
	/// This class implements the tool window exposed by this package and hosts a user control.
	/// </summary>
	/// <remarks>
	/// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
	/// usually implemented by the package implementer.
	/// <para>
	/// This class derives from the ToolWindowPane class provided from the MPF in order to use its
	/// implementation of the IVsUIElementPane interface.
	/// </para>
	/// </remarks>
	[Guid("674a1bc1-c43e-4b8a-a7cc-bf45f01600e2")]
	public class ToolWindow : ToolWindowPane
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolWindow"/> class.
		/// </summary>
		public ToolWindow() : base(null)
		{
			this.Caption = "链接文件工具";

			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
			// the object returned by the Content property.
			this.Content = new ToolWindowControl();
		}
	}
}
