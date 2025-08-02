/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{

	/// <summary>
	/// 输入测试事件
	/// </summary>
	public interface InputTestEvent : InputEvent { }


	/// <summary>
	/// 测试
	/// </summary>
	public class InputMapperTest : Node
	, AsComponentBranch
	, AsChildBranch
	, ComponentOf<InitialDomain>
	, AsRule<Awake>
	, AsRule<InputTestEvent>
	{

	}
}