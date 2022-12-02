using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

//对象池回收，封装组件事件回调,

//实体脚本引用MonoObject

public class MonoObject : MonoBehaviour
{
    public List<Component> components = new List<Component>();
}
