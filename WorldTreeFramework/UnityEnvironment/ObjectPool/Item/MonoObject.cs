using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

//对象池回收，封装组件事件回调,

//实体脚本引用MonoObject

public class MonoObject : MonoBehaviour
{
    /// <summary>
    /// 组件列表
    /// </summary>
    public List<Component> components = new List<Component>();

    /// <summary>
    /// 清除所有组件注册的事件
    /// </summary>
    public void RemoveAllEvent()
    {
        foreach (var component in components)
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
