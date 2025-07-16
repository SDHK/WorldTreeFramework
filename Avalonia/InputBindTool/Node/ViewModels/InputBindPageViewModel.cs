using System.Collections.Generic;
using System.Linq;

namespace WorldTree
{
	/// <summary>
	/// 页面的输入绑定视图模型
	/// </summary>
	public class InputBindPageViewModel : Node, INodeListener
		, AsComponentBranch
		, ComponentOf<MainWorld>
		, AsAwake<InputBindPage>
		, AsListenerAddRule
	{

		/// <summary>
		/// 绑定存档
		/// </summary>
		public IEnumerable<KeyValuePair<string, INode>> PageNameList
		{
			get
			{

				if (this.World.TryGetComponent(out InputManager manager))
				{
					return manager.GenericBranch<string, InputManager>().GetEnumerable();
				}

				return Enumerable.Empty<KeyValuePair<string, INode>>();
			}
		}




	}
}
