/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/04 08:19:28

* 描述： 节点引用

*/

namespace WorldTree
{
	/// <summary>
	/// 数据引用
	/// </summary>
	public struct DataRef<N>
		where N : class, INodeData
	{
		/// <summary>
		/// 世界树核心
		/// </summary>
		public WorldTreeCore Core;

		/// <summary>
		/// 数据UID
		/// </summary>
		public readonly long DataUID;

		/// <summary>
		/// 数据
		/// </summary>
		private N data;

		/// <summary>
		/// 获取数据，如果当前数据为空或数据UID对不上，那么尝试从核心引用池查找。
		/// 如果找到对应UID的数据，则更新并返回该数据；否则，返回null。
		/// </summary>
		//public N Value => (data == null || DataUID != data.UID) ? data = Core?.GetNodeData<N>(DataUID) : data;
		public N Value => (data is null || DataUID == data.Id) ? data : data = null;

		public DataRef(N data)
		{
			if (data is null)
			{
				DataUID = 0;
				this.data = null;
				Core = null;
				return;
			}
			DataUID = data.UID;
			this.data = data;
			Core = data.Core;
		}

		/// <summary>
		/// 尝试获取数据
		/// </summary>
		public bool TryGet(out N data) => (data = Value) is not null;

		/// <summary>
		/// 清空引用
		/// </summary>
		public void Clear() => data = null;

		public static implicit operator DataRef<N>(N data) => new(data);

		public static implicit operator N(DataRef<N> dataRef) => dataRef.Value;

		public static bool operator ==(DataRef<N> a, N b) => a.DataUID == b?.UID;
		public static bool operator !=(DataRef<N> a, N b) => a.DataUID != b?.UID;

		public static implicit operator bool(DataRef<N> dataRef) => dataRef.data is not null;
	}
}
