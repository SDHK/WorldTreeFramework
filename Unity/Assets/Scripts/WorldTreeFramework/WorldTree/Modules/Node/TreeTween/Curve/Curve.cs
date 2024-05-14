
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/16 10:56

* 描述： 曲线

*/




namespace WorldTree
{
    /// <summary>
    /// 曲线节点基类
    /// </summary>
    public class CurveBase : Node, ComponentOf<CurveManager>
        , AsRule<IAwakeRule>
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
