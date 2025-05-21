/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 序列化帮助类
	/// </summary>
	public static class TreeDataHelper
	{
		/// <summary>
		/// 序列化数据节点类型
		/// </summary>
		public static byte[] SerializeNode(INode self)
		{
			if (self.IsDisposed) return null;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.Core.SerializeRuleGroup.Send(current));

			self.AddTemp(out TreeDataByteSequence sequence);
			if (self?.Parent == null) return null;
			sequence.Serialize(self);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 序列化数据类型
		/// </summary>
		public static byte[] Serialize<T>(INode self, T data)
		{
			if (self.IsDisposed) return null;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.Core.SerializeRuleGroup.Send(current));

			self.AddTemp(out TreeDataByteSequence sequence);
			if (self?.Parent == null) return null;
			sequence.Serialize(data);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 反序列化数据类型
		/// </summary>
		public static T Deseralize<T>(INode self, byte[] bytes)
		{
			T obj = default;
			return Deseralize(self, bytes, ref obj);
		}


		/// <summary>
		/// 反序列化数据类型
		/// </summary>
		public static T Deseralize<T>(INode self, byte[] bytes, ref T obj)
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			sequence.Deserialize(ref obj);
			sequence.Dispose();
			return obj;
		}
		/// <summary>
		/// 反序列化数据类型
		/// </summary>
		public static T Deseralize<T>(INode self, Type type, byte[] bytes)
		{
			object obj = default;
			return Deseralize<T>(self, type, bytes, ref obj);
		}

		/// <summary>
		/// 反序列化数据类型
		/// </summary>
		public static T Deseralize<T>(INode self, Type type, byte[] bytes, ref object obj)
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			sequence.Deserialize(type, ref obj);
			sequence.Dispose();
			return (T)obj;
		}


		/// <summary>
		/// 序列化树数据
		/// </summary>
		public static byte[] SerializeTreeData(TreeData treeData)
		{
			treeData.Parent.AddTemp(out TreeDataByteSequence sequence);
			sequence.SerializeTreeData(treeData);
			byte[] bytes = sequence.ToBytes();
			sequence.Dispose();
			return bytes;
		}

		/// <summary>
		/// 反序列化为通用数据结构
		/// </summary>
		public static TreeData DeserializeTreeData(INode self, byte[] bytes)
		{
			self.AddTemp(out TreeDataByteSequence sequence).SetBytes(bytes);
			TreeData treeData = sequence.DeserializeTreeData();
			sequence.Dispose();
			return treeData;
		}
	}

}
