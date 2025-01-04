/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldTree
{
    /// <summary>
    /// 实体绑定Mono组件
    /// </summary>
    public class MonoObject : MonoBehaviour
    {
        /// <summary>
        /// 组件列表
        /// </summary>
        public List<Component> ComponentList = new List<Component>();
        
        /// <summary>
        /// 绑定的实体
        /// </summary>
        public INode Node;

        /// <summary>
        /// 清除所有组件注册的事件
        /// </summary>
        public void RemoveAllEvent()
        {
            foreach (var component in ComponentList)
            {
                switch (component)
                {
                    case Button:
                        (component as Button).onClick.RemoveAllListeners();
                        break;
                    case Toggle:
                        Toggle toggle = (component as Toggle);
                        toggle.onValueChanged.RemoveAllListeners();
                        break;
                    case InputField:
                        InputField inputField = (component as InputField);
                        inputField.onValueChanged.RemoveAllListeners();
                        inputField.onEndEdit.RemoveAllListeners();
                        inputField.onSubmit.RemoveAllListeners();
                        break;
                }
            }
        }
    }
}
