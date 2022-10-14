using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
    [Serializable]
    public class EditorAssetNode2 : EditorAssetBase2
    {

        /// <summary>
        /// 父节点
        /// </summary>
        public EditorAssetBase2 parentNode;

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold;

        /// <summary>
        /// 水平排列
        /// </summary>
        public bool isHorizontal;

     

        [ShowInInspector]

        //类型，键值
        public KeyValue<EditorFieldType, string>[] list = new KeyValue<EditorFieldType, string>[0];


        /*
         
            Node
                *string -type -value 

            Class
                *string -type -value 
            
            Dict
                -string -value
            
            List
                *int -value
         */
    }
}

[Serializable]
[ShowInInspector]

public class EditorAssetBase2
{
    public string name1;
    public int int1;
}
