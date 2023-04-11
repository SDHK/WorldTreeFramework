
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/11 20:16

* 描述： 数组对象池
* 
*  

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 数组对象池
    /// </summary>
    public class ArrayPool : GenericPool<Array>, IAwake<Type>, ChildOf<INode>
    {
        public override string ToString()
        {
            return $"[ArrayPool<{ObjectType}>] : {Count} ";
        }

        public Array ObjectNew(IPool pool)
        {
            return Activator.CreateInstance(ObjectType, true) as Array;
        }

        public void ObjectOnRecycle(Array obj)
        {
            Array.Clear(obj, 0, obj.Length);
        }
    }

    class ArrayPoolAwakeRule : AwakeRule<ArrayPool, Type>
    {
        public override void OnEvent(ArrayPool self, Type type)
        {
            self.ObjectType = type;
            self.NewObject = self.ObjectNew;
            self.objectOnRecycle = self.ObjectOnRecycle;
        }
    }
}
