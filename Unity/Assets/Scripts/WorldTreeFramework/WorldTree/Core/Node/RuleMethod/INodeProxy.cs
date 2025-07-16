/****************************************

* ���ߣ�����ڿ�
* ���ڣ�2025/7/10 11:42

* ������INode�ӿڴ���ʵ��

* ����ʵ��INode�ӿڷ�����Ϊ�˷�������Node���������

*/

namespace WorldTree
{



	/// <summary>
	/// INode�ӿڴ���ʵ��
	/// </summary>
	public static partial class INodeProxyRule
	{
		/// <summary>
		/// ���ü���
		/// </summary>
		public static void SetActive(INode self, bool value)
		{
			if (self.ActiveToggle != value)
			{
				self.ActiveToggle = value;
				self.RefreshActive();
			}
		}

		/// <summary>
		/// ˢ�µ�ǰ�ڵ㼤��״̬��������������ӽڵ�
		/// </summary>
		public static void RefreshActive(INode self)
		{
			//���״̬��ͬ������Ҫˢ��
			if (self.IsActive == ((self.Parent == null) ? self.ActiveToggle : self.Parent.IsActive && self.ActiveToggle)) return;

			//������������ӽڵ�
			using (self.Core.PoolGetUnit(out UnitQueue<INode> queue))
			{
				queue.Enqueue(self);
				while (queue.Count != 0)
				{
					// ������ȣ�����
					var current = queue.Dequeue();
					if (current.IsActive != ((current.Parent == null) ? current.ActiveToggle : current.Parent.IsActive && current.ActiveToggle))
					{
						current.IsActive = !current.IsActive;

						if (current.BranchDict != null)
						{
							foreach (var branchs in current.BranchDict)
							{
								foreach (INode node in branchs.Value)
								{
									if (node.BranchType == branchs.Value.Type)
									{
										queue.Enqueue(node);
									}
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// �ַ�������ǰ�ڵ�
		/// </summary>
		public static string ToString(INode self)
		{
			return self.GetType().ToString();
		}

		#region �ڵ㴦��

		#region ����

		/// <summary>
		/// ����ʱ��Node��Id��ȡ�ͷ���֧��
		/// </summary>
		public static void OnCreate(INode self)
		{
			self.InstanceId = self.Core.IdManager.GetId();
			self.Id = self.InstanceId;
			self.Core.RuleManager?.SupportNodeRule(self.Type);
		}

		#endregion

		#region ���

		/// <summary>
		/// ���Խ�������ӵ����ṹ����������
		/// </summary>
		public static bool TryAddSelfToTree<B, K>(this INode self, K key, INode parent)
			where B : class, IBranch<K>
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, self))
			{
				self.BranchType = self.Core.TypeToCode<B>();
				self.Parent = parent;
				self.Core = parent.Core;
				self.World = parent.World;
				self.SetActive(true);//����ڵ�
				AddNodeView(self);
				return true;
			}
			return false;
		}

		/// <summary>
		/// ���ʱ
		/// </summary>
		/// <summary>
		/// �ڵ�������ṹʱ�Ĵ�����������
		/// </summary>
		public static void OnAddSelfToTree(this INode self)
		{
			self.Core.ReferencedPoolManager.TryAdd(self);//��ӵ����ó�
			if (self is not IListenerIgnorer)//�㲥��ȫ��������
			{
				IRuleExecutor<ListenerAdd> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<ListenerAdd>(self);
				ruleActuator?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)//��������Ƿ�Ϊ������
			{
				self.Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}
			if (self.IsActive != self.activeEventMark)//������
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self);//�����¼�֪ͨ
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //�����¼�֪ͨ
				}
			}
			self.Core.AddRuleGroup?.Send(self);//�ڵ�����¼�֪ͨ
		}

		#endregion

		#region �Ƴ�

		/// <summary>
		/// �ͷ����з�֧�����нڵ㣨��������
		/// </summary>
		public static void RemoveAllNode(INode self)
		{
			if (self.BranchDict == null) return;
			using (self.Core.PoolGetUnit(out UnitStack<IBranch> branchs))
			{
				foreach (var item in self.BranchDict) branchs.Push(item.Value);
				while (branchs.Count != 0) self.RemoveAllNode(branchs.Pop().Type);
			}

			//�����ڷ�֧�Ƴ������У��ڵ���������µķ�֧����ô���Ǵ���ģ�������֧���޷����ա�
			if (self.BranchDict.Count != 0)
			{
				foreach (var item in self.BranchDict)
				{
					self.Log($"�Ƴ���֧����������·�֧���ڵ㣺{self} ��֧:{item.GetType()}");
				}
			}
		}


		/// <summary>
		/// �ͷ�ָ����֧���͵����нڵ㣨��������
		/// </summary>
		public static void RemoveAllNode(INode self, long branchType)
		{
			if (NodeBranchHelper.TryGetBranch(self, branchType, out IBranch branch))
			{
				if (branch.Count != 0)
				{
					// �������޷�һ�ߵ���һ��ɾ����������ջ�洢��Ҫɾ���Ľڵ�
					using (self.Core.PoolGetUnit(out UnitStack<INode> nodes))
					{
						foreach (var item in branch) nodes.Push(item);
						while (nodes.Count != 0) nodes.Pop().Dispose();
					}

					// ����ڽڵ��Ƴ���������������½ڵ㣬���޷�����
					if (branch.Count != 0)
					{
						foreach (var item in branch)
						{
							self.LogError($"�Ƴ��ڵ����������½ڵ㣬����:{self.GetType()} ��֧: {branch.GetType()} �ڵ�:{item.GetType()}:{item.Id}");
						}
					}
				}
			}
		}

		/// <summary>
		/// �ͷŽڵ�
		/// </summary>
		public static void Dispose(INode self)
		{
			//�Ƿ��Ѿ��ͷ�
			if (self.IsDisposed) return;

			//�ڵ��ͷ�ǰ���������,�ڵ��ͷź�����������
			NodeBranchTraversalHelper.TraversalPrePostOrder(self, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		/// <summary>
		/// �ͷ�ǰ�Ĵ�����������
		/// </summary>
		public static void OnBeforeDispose(INode self) => self.Core.BeforeRemoveRuleGroup?.Send(self);


		/// <summary>
		/// �ڵ��ͷ�ʱ�Ĵ�����������
		/// </summary>
		public static void OnDispose(INode self)
		{
			self.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderDispose);
			NodeBranchHelper.RemoveNode(self); // �Ӹ��ڵ��֧�Ƴ�
			self.SetActive(false); // ������
			self.Core.DisableRuleGroup?.Send(self); // �����¼�֪ͨ
			if (self is INodeListener nodeListener && self is not IListenerIgnorer) // �������Ϊ������
			{
				self.Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			self.Core.RemoveRuleGroup?.Send(self); // �Ƴ��¼�֪ͨ
			if (self is not IListenerIgnorer) // �㲥��ȫ��������֪ͨ
			{
				NodeListenerExecutorHelper.GetListenerExecutor<ListenerRemove>(self)?.Send(self);
			}
			self.Core.ReferencedPoolManager.Remove(self); // ���ó��Ƴ�

			//self.DisposeDomain(); //�����ڵ�
			self.Parent = null; // ������ڵ�
			self.Core.PoolRecycle(self); // ���յ���
		}

		#endregion

		#region �޽�

		/// <summary>
		/// �ڵ�޽ӵ����ṹ����������
		/// </summary>
		public static bool TryGraftSelfToTree<B, K>(INode self, K key, INode parent)
			where B : class, IBranch<K>
			=> self.TryGraftSelfToTree(self.TypeToCode<B>(), key, parent);


		/// <summary>
		/// �ڵ�޽ӵ����ṹ����Լ����������������
		/// </summary>
		public static bool TryGraftSelfToTree<K>(this INode self, long branchType, K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch(parent, branchType) is not IBranch<K> branch) return false;
			if (!branch.TryAddNode(key, self)) return false;

			self.BranchType = branch.Type;
			self.Parent = parent;
			self.Core = parent.Core;
			self.World = parent.World;

			self.RefreshActive();
			NodeBranchTraversalHelper.TraversalPrePostOrder(self, current => current.OnBeforeGraftSelfToTree(), current => current.OnGraftSelfToTree());
			return true;
		}

		/// <summary>
		/// �ڵ�޽�ǰ�Ĵ�����������
		/// </summary>
		public static void OnBeforeGraftSelfToTree(this INode self)
		{
			self.Core = self.Parent.Core;
			self.World = self.Parent.World;
			// ���л�ʱ����Ҫ�����������нڵ�ĸ��ڵ�
			if (self.IsSerialize)
			{
				if (self.BranchDict != null)
				{
					foreach (var brancItem in self.BranchDict)
					{
						if (brancItem.Value == null) continue;
						foreach (var nodeItem in brancItem.Value)
						{
							nodeItem.Parent = self;
							nodeItem.BranchType = brancItem.Value.Type;
						}
					}
				}
			}
			AddNodeView(self);
		}

		/// <summary>
		/// �ڵ�޽ӵĴ�����������
		/// </summary>
		public static void OnGraftSelfToTree(this INode self)
		{
			self.Core.ReferencedPoolManager.TryAdd(self);//��ӵ����ó�
			if (self is not IListenerIgnorer)//�㲥��ȫ��������
			{
				IRuleExecutor<ListenerAdd> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<ListenerAdd>(self);
				ruleActuator?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)//�����Ӿ�̬����
			{
				self.Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}

			if (self.IsSerialize)
			{
				self.Core.DeserializeRuleGroup?.Send(self);//�����л��¼�֪ͨ
				self.IsSerialize = false;
			}

			if (self.IsActive != self.activeEventMark)//������
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self);//�����¼�֪ͨ
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //�����¼�֪ͨ
				}
			}
			if (!self.IsSerialize) self.Core.GraftRuleGroup?.Send(self);//�޽��¼�֪ͨ
		}

		#endregion

		#region �ü�

		/// <summary>
		/// �����Ͻ��Լ��ü���������������
		/// </summary>
		public static INode CutSelf(this INode self)
		{
			if (self.IsDisposed) return null; // �Ƿ��Ѿ�����
			if (self.Parent == null) return self;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.OnCutSelf());
			NodeBranchHelper.RemoveNode(self); // �Ӹ��ڵ��֧�Ƴ�
			return self;
		}

		/// <summary>
		/// �����Ͻ��Լ��ü�����ʱ�Ĵ�����������
		/// </summary>
		public static void OnCutSelf(this INode self)
		{
			self.ViewBuilder?.Dispose();
			self.ViewBuilder = null;
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)
			{
				// ����Ƴ���̬����
				self.Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			self.Core.CutRuleGroup?.Send(self); // �ü��¼�֪ͨ
			if (self is not IListenerIgnorer) // �㲥��ȫ��������֪ͨ
			{
				NodeListenerExecutorHelper.GetListenerExecutor<ListenerRemove>(self)?.Send(self);
			}
			self.Core.ReferencedPoolManager.Remove(self); // ���ó��Ƴ�
			self.Parent = null; // ������ڵ�
		}

		#endregion

		/// <summary>
		/// ��ӽڵ���ӻ�
		/// </summary>
		public static void AddNodeView(INode self)
		{
			self.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderDispose);
			self.Core.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderCreate);
		}

		/// <summary>
		/// �����ڵ���ӻ�
		/// </summary>
		private static void ViewBuilderCreate(this INode self)
		{
			if (self.Parent?.ViewBuilder == null)
			{
				self.ViewBuilder = null;
				return;
			}
			// �õ����ڵ�Ŀ��ӻ��������ĸ����ڵ�
			INode viewParent = self.Parent.ViewBuilder.Parent;

			// ��������Ŀ��ӻ�������
			INode nodeView = viewParent.Core.PoolGetNode(self.Parent.ViewBuilder.Type);

			// ��������ӵ����ڵ�Ŀ��ӻ��������У������ӻ���ҵ����ӻ������ڵ���
			self.ViewBuilder = NodeBranchHelper.AddNodeToTree(viewParent, default(ChildBranch), nodeView.Id, nodeView, (INode)self, (INode)self.Parent) as IWorldTreeNodeViewBuilder;
		}

		/// <summary>
		/// �ͷŽڵ���ӻ�
		/// </summary>
		private static void ViewBuilderDispose(this INode self)
		{
			self.ViewBuilder.Dispose();
			self.ViewBuilder = null;
		}

		#endregion
	}
}