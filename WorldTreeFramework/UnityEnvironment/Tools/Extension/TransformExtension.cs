using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
    public static class TransformExtension
    {
        /// <summary>
        /// 将位置旋转和大小变为默认值
        /// </summary>
        public static void Default(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
