﻿namespace WorldTree
{
    /// <summary>
    /// FixedUpdate系统接口
    /// </summary>
    public interface IFixedUpdateSystem : ISendSystem<float> { }

    /// <summary>
    /// FixedUpdate系统基类
    /// </summary>
    public abstract class FixedUpdateSystem<T> : SystemBase<T, IFixedUpdateSystem>, IFixedUpdateSystem
       where T : Entity
    {
        public void Invoke(Entity self, float deltaTime) => OnFixedUpdate(self as T, deltaTime);
        public abstract void OnFixedUpdate(T self, float deltaTime);
    }
}
