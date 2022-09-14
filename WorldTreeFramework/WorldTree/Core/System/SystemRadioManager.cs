using System;

namespace WorldTree
{
    public class SystemRadioManager : Entity
    {
        UnitDictionary<Type, SystemRadio> radios = new UnitDictionary<Type, SystemRadio>();

        public SystemRadio Get<T>()
        where T : ISystem
        {
            Type type = typeof(T);
            return Get(type);
        }

        public SystemRadio Get(Type type)
        {
            if (!radios.TryGetValue(type, out SystemRadio radio))
            {
                radio = this.AddChildren<SystemRadio>();
                radios.Add(type, radio);
            }
            return radio;
        }
    }
}
