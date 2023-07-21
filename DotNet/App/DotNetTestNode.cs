using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    class DotNetTestNode : Node, ComponentOf<INode>
    {

    }

    public static partial class DotNetTestNodeRule
    {
        class AddRule : AddRule<DotNetTestNode>
        {
            public override void OnEvent(DotNetTestNode self)
            {
                World.Log(" 初始化！！！");


            }
        }

    }


  
}
