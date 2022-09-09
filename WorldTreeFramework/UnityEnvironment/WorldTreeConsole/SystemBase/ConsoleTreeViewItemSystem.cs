﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public interface IConsoleTreeViewItemSystem : ISendSystem<ConsoleTreeView>{}
    public abstract class ConsoleTreeViewItemSystem<T> : SystemBase<T, IConsoleTreeViewItemSystem>, IConsoleTreeViewItemSystem
        where T : Entity
    {
        public void Invoke(Entity self, ConsoleTreeView consoleTreeView) => OnDraw(self as T, consoleTreeView);
        public abstract void OnDraw(T self, ConsoleTreeView consoleTreeView);
    }
}
