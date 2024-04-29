namespace WorldTree
{
	public partial class TestNode1 : INode
	{
		public long Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public WorldTreeRoot Root { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public INode Domain { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public INode Parent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public IWorldTreeNodeView View { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool ActiveToggle { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool IsActive { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool m_ActiveEventMark { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public UnitDictionary<long, IRattan> m_Rattans { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public UnitDictionary<long, IRattan> Rattans => throw new System.NotImplementedException();

		public long BranchType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public UnitDictionary<long, IBranch> m_Branchs { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public UnitDictionary<long, IBranch> Branchs => throw new System.NotImplementedException();

		public WorldTreeCore Core { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool IsFromPool { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool IsRecycle { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public long Type { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public bool IsDisposed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public INode CutSelf()
		{
			throw new System.NotImplementedException();
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public void OnAddSelfToTree()
		{
			throw new System.NotImplementedException();
		}

		public void OnBeforeDispose()
		{
			throw new System.NotImplementedException();
		}

		public void OnCutSelf()
		{
			throw new System.NotImplementedException();
		}

		public void OnDispose()
		{
			throw new System.NotImplementedException();
		}

		public void OnGraftSelfToTree()
		{
			throw new System.NotImplementedException();
		}

		public void RemoveAllNode()
		{
			throw new System.NotImplementedException();
		}

		public void RemoveAllNode(long branchType)
		{
			throw new System.NotImplementedException();
		}

		bool INode.TryAddSelfToTree<B, K>(K Key, INode parent)
		{
			throw new System.NotImplementedException();
		}

		bool INode.TryGraftSelfToTree<B, K>(K key, INode parent)
		{
			throw new System.NotImplementedException();
		}
	}
}