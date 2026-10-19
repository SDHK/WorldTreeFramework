/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 曲线节点基类
	/// </summary>
	public class CurveBase : Node, ComponentOf<CurveManager>
		, AsRule<Awake>
		, AsRule<CurveEvaluate>
	{ }


	class CurveBaseCurveEvaluateRule : CurveEvaluateRule<CurveBase>
	{
		protected override float Execute(CurveBase self, float arg1)
		{
			return arg1;
		}
	}
}
