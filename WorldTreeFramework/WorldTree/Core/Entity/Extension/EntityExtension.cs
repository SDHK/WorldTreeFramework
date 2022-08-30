namespace WorldTree
{
    public static class EntityExtension
    {
        /// <summary>
        /// 返回用字符串绘制的树
        /// </summary>
        public static string ToStringDrawTree(this Entity entity, string t = "\t")
        {
            string t1 = "\t" + t;
            string str = "";

            str += t1 + $"[{entity.id}] " + entity.ToString() + "\n";

            if (entity.children != null)
            {
                if (entity.children.Count > 0)
                {
                    str += t1 + "   Children:\n";
                    foreach (var item in entity.Children.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            if (entity.components != null)
            {
                if (entity.components.Count > 0)
                {
                    str += t1 + "   Components:\n";
                    foreach (var item in entity.Components.Values)
                    {
                        str += item.ToStringDrawTree(t1);
                    }
                }
            }

            return str;
        }
    }
}
