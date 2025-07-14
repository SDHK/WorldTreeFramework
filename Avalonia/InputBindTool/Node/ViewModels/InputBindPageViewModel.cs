using System.Collections.ObjectModel;

namespace WorldTree
{
	/// <summary>
	/// 页面的输入绑定视图模型
	/// </summary>
	public class InputBindPageViewModel : Node
		, AsComponentBranch
		, ComponentOf<MainWorld>
		, AsAwake<InputBindPage>
	{

		/// <summary>
		/// 绑定存档
		/// </summary>
		public ObservableCollection<string> PageNameList { get; } = new() { "第一页", "第二页" };


	}
}
