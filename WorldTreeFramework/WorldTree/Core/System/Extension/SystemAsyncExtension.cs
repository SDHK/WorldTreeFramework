using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    public static class SystemAsyncExtension
    {
        public static async AsyncTask<bool> TrySendAsyncSystem<S>(this Entity self)
          where S : ICallSystem<AsyncTask>
        {

         return await self.Root.SystemManager.GetSystemGroup<S>().TrySendAsyncSystem<S>(self);

            bool bit = false;
            if (self.Root.SystemManager.TryGetSystems(self.Type, typeof(S), out List<ISystem> sendSystems))
            {
                foreach (ICallSystem<AsyncTask<bool>> sendSystem in sendSystems)
                {
                    await sendSystem.Invoke(self);
                }
                bit = true;
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

        public static async AsyncTask<bool> TrySendAsyncSystem<S>(this SystemGroup group, Entity self)
        where  S: ICallSystem<AsyncTask>
        {
            bool bit = false;

            if (group.TryGetValue(self.Type, out List<ISystem> systems))
            {
                foreach (S system in systems)
                {
                   await system.Invoke(self);
                }
            }
            if (!bit)
            {
                await self.AsyncYield();
            }
            return bit;
        }

    }
}
