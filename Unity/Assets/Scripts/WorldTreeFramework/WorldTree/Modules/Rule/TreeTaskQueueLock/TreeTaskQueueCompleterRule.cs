﻿/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 21:08

* 描述： 

*/

namespace WorldTree
{
    public static partial class TreeTaskQueueCompleterRule
    {
        class AwakeRule : AwakeRule<TreeTaskQueueCompleter, long, TreeTaskQueueLockManager>
        {
            protected override void Execute(TreeTaskQueueCompleter self, long key, TreeTaskQueueLockManager queueLock)
            {
                self.key = key;
                self.queueLock = queueLock;
            }
        }

        class RemoveRule : RemoveRule<TreeTaskQueueCompleter>
        {
            protected override void Execute(TreeTaskQueueCompleter self)
            {
                self.queueLock.RunNext(self.key);
                self.queueLock = default;
                self.key = default;
            }
        }
    }
}
