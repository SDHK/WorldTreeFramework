
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
    public class ArrayPool<T> : GenericPool<T[]>, IAwake, ChildOf<INode>
    where T : struct
    {
        public override string ToString()
        {
            return $"[ArrayPool<{ObjectType}>] : {Count} ";
        }

        public T[] ObjectNew(IPool pool)
        {
            T[] obj = Activator.CreateInstance(ObjectType, true) as T[];
            return obj;
        }

        public void ObjectOnRecycle(T[] obj)
        {
            Array.Clear(obj, 0, obj.Length);
        }
    }

    class ArrayPoolAwakeRule<T> : AwakeRule<ArrayPool<T>>
    where T : struct
    {
        public override void OnEvent(ArrayPool<T> self)
        {
            self.ObjectType = typeof(T[]);
            self.NewObject = self.ObjectNew;
            self.objectOnRecycle = self.ObjectOnRecycle;
        }
    }
}
