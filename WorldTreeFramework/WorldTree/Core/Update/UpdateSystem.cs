namespace WorldTree
{
    /// <summary>
    /// Update系统接口
    /// </summary>
    public interface IUpdateSystem : ISystem
    {
        void Execute(Entity self,float deltaTime);
    }

    /// <summary>
    /// Update系统基类
    /// </summary>
    public abstract class UpdateSystem<T> : SystemBase<T, IUpdateSystem>, IUpdateSystem
        where T : Entity
    {
        public void Execute(Entity self,float deltaTime) => Update(self as T, deltaTime);
        public abstract void Update(T self,float deltaTime);
    }
}
