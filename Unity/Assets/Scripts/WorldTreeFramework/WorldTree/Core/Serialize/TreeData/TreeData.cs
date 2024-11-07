/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeData : Node
		, StringNodeOf<TreeData>
		, NumberNodeOf<TreeData>
		, TempOf<INode>
		, AsNumberNodeBranch
		, AsStringNodeBranch
		, AsAwake
	{
		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName;

		/// <summary>
		/// 是否默认值
		/// </summary>
		public bool IsDefault = true;
	}
	
	/// <summary>
	/// 树数值
	/// </summary>
	public class TreeValue : TreeData
	{
		/// <summary>
		/// 数据
		/// </summary>
		public object Value;
	}



	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeDataClass : Node
		, TempOf<INode>
		, AsNumberNodeBranch
		, AsStringNodeBranch
		, AsAwake
	{

		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName;

		/// <summary>
		/// 是否默认值
		/// </summary>
		public bool IsDefault = true;

		/// <summary>
		/// 字段名称
		/// </summary>
		public List<string> NameList;

		/// <summary>
		/// 字段
		/// </summary>
		public List<TreeDataClass> TupleList;
	}

	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeDataArray : Node
	{
		/// <summary>
		/// 数组项
		/// </summary>
		public List<TreeDataClass> TupleList;
	}


}
