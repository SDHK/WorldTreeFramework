using System;

namespace WorldTree
{
	public class TestNode001 : INode
	{
		public long Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public WorldTreeRoot Root { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public INode Domain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public INode Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public IWorldTreeNodeView View { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ActiveToggle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool m_ActiveEventMark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public UnitDictionary<long, IRattan> m_Rattans { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public UnitDictionary<long, IRattan> Rattans => throw new NotImplementedException();

		public long BranchType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public UnitDictionary<long, IBranch> m_Branchs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public UnitDictionary<long, IBranch> Branchs => throw new NotImplementedException();

		public WorldTreeCore Core { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsFromPool { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsRecycle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public long Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool IsDisposed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public INode CutSelf()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void OnAddSelfToTree()
		{
			throw new NotImplementedException();
		}

		public void OnBeforeDispose()
		{
			throw new NotImplementedException();
		}

		public void OnCutSelf()
		{
			throw new NotImplementedException();
		}

		public void OnDispose()
		{
			throw new NotImplementedException();
		}

		public void OnGraftSelfToTree()
		{
			throw new NotImplementedException();
		}

		public void RemoveAllNode()
		{
			throw new NotImplementedException();
		}

		public void RemoveAllNode(long branchType)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K>(K key, INode parent)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K, T1>(K key, INode parent, T1 arg1)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K, T1, T2>(K key, INode parent, T1 arg1, T2 arg2)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K, T1, T2, T3>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K, T1, T2, T3, T4>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			throw new NotImplementedException();
		}

		INode INode.AddSelfToTree<B, K, T1, T2, T3, T4, T5>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			throw new NotImplementedException();
		}

		bool INode.Contains<B, K>(K key)
		{
			throw new NotImplementedException();
		}

		bool INode.ContainsId<B>(long id)
		{
			throw new NotImplementedException();
		}

		INode INode.CutNode<B, K>(K key)
		{
			throw new NotImplementedException();
		}

		INode INode.CutNodeById<B>(long id)
		{
			throw new NotImplementedException();
		}

		INode INode.GetNode<B, K>(K key)
		{
			throw new NotImplementedException();
		}

		INode INode.GetNodeById<B>(long id)
		{
			throw new NotImplementedException();
		}

		void INode.RemoveAllNode<B>()
		{
			throw new NotImplementedException();
		}

		void INode.RemoveNode<B, K>(K key)
		{
			throw new NotImplementedException();
		}

		void INode.RemoveNodeById<B>(long id)
		{
			throw new NotImplementedException();
		}

		bool INode.TryAddSelfToTree<B, K>(K Key, INode parent)
		{
			throw new NotImplementedException();
		}

		bool INode.TryCutNode<B, K>(K key, out INode node)
		{
			throw new NotImplementedException();
		}

		bool INode.TryCutNodeById<B>(long id, out INode node)
		{
			throw new NotImplementedException();
		}

		bool INode.TryGetNode<B, K>(K key, out INode node)
		{
			throw new NotImplementedException();
		}

		bool INode.TryGetNodeById<B>(long id, out INode node)
		{
			throw new NotImplementedException();
		}

		bool INode.TryGraftSelfToTree<B, K>(K key, INode parent)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public class DotNetTestNode : Node, ComponentOf<INode>
		, AsRule<IAwakeRule>
	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
		private class AwakeRule : AwakeRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("唤醒！！");
			}
		}

		private class EnableRule : EnableRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("激活！！");
			}
		}

		private class AddRule : AddRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log(" 初始化！！！!!");

				//self.AddComponent(out RefeshCsProjFileCompileInclude _);
				//self.Log(self.Core.ToStringDrawTree());
			}
		}

		private class DisableRule : DisableRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("失活！！");
			}
		}

		private class UpdateTimeRule : UpdateTimeRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self, TimeSpan timeSpan)
			{
				self.Log($"初始更新！！{timeSpan.TotalSeconds}");
			}
		}

		private class RemoveRule : RemoveRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log($"初始关闭！！");
			}
		}
	}
}