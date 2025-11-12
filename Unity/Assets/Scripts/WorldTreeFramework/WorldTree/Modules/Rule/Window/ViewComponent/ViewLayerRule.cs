namespace WorldTree
{

	public static partial class ViewLayerRule
	{
		[NodeRule(nameof(ViewRegisterRule<ViewLayerBind>))]
		private static void OnViewRegisterRule(this ViewLayerBind self)
		{
			self.Core.PoolGetUnit(out self.ViewList);
			self.Core.PoolGetUnit(out self.IdIndexDict);
		}

		[NodeRule(nameof(ViewUnRegisterRule<ViewLayerBind>))]
		private static void OnViewUnRegisterRule(this ViewLayerBind self)
		{
			self.ViewList.Dispose();
			self.ViewList = null;
			self.IdIndexDict.Dispose();
			self.IdIndexDict = null;
		}

		[NodeRule(nameof(CloseRule<ViewLayer>))]
		private static void OnCloseRule(this ViewLayer self)
		{
			// 关闭时清理所有视图
			self.CloseAllView();
		}


		[NodeRule(nameof(SubViewCloseRule<ViewLayer>))]
		private static void OnSubViewCloseRule(this ViewLayer self, View view)
		{
			// 自动清理 ViewList 和 IdIndexDict
			if (self.ViewBind.IdIndexDict.Remove(view.Id, out int index))
			{
				self.ViewBind.ViewList[index].Value.Layer = 0;
				self.ViewBind.ViewList.RemoveAt(index);

				// 更新后续View的Layer
				for (int i = index; i < self.ViewBind.ViewList.Count; i++)
				{
					var nodeRef = self.ViewBind.ViewList[i];
					if (nodeRef.IsNull)
					{
						self.ViewBind.IdIndexDict.Remove(nodeRef.InstanceId);
						self.ViewBind.ViewList.RemoveAt(i);
						i--; // 移除后需要回退索引
						continue;
					}
					var v = nodeRef.Value;
					self.ViewBind.IdIndexDict[v.Id] = i;
					v.Layer = (byte)(i + 1);
					v.LayerChange();
				}
			}
		}

		/// <summary>
		/// 尝试添加视图:类型码
		/// </summary>
		public static bool TryAddView(this ViewLayer self, long typeCode, out ViewObject view)
		{
			view = null;
			if (NodeBranchHelper.TryGetBranch(self.ViewBind, out ComponentBranch branch) && branch.Contains(typeCode)) return false;
			NodeBranchHelper.AddNode(self.ViewBind, default(ComponentBranch), typeCode, typeCode, out INode node);
			view = (ViewObject)node;
			return true;
		}

		/// <summary>
		/// 尝试添加视图
		/// </summary>
		public static bool TryAddView<T>(this ViewLayer self, out T view)
			where T : ViewObject
		{
			if (self.ViewBind.TryGetComponent(out view)) return false;
			self.ViewBind.AddComponent(out view);
			return true;
		}

		/// <summary>
		/// 打开视图 : 类型码
		/// </summary>
		public static void OpenView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewBind?.ViewList == null) return;
			if (!(NodeBranchHelper.TryGetBranch(self.ViewBind, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewObject view) return;
			if (!view.IsOpen)
			{
				if (self.ViewBind.ViewList.Count == 15) throw new System.Exception("当前视图层级堆叠达到最大15，无法继续打开新视图!");
				self.ViewBind.IdIndexDict[view.Id] = self.ViewBind.ViewList.Count;
				self.ViewBind.ViewList.Add(new(view));
				view.Layer = (byte)self.ViewBind.ViewList.Count;
				view.OnOpen();
			}
			else
			{
				// 已经打开则提升到顶层 
				self.SetTopView(typeCode);
			}
		}

		/// <summary>
		/// 打开视图
		/// </summary>
		public static void OpenView<T>(this ViewLayer self) where T : ViewObject => self.OpenView(self.TypeToCode<T>());

		/// <summary>
		/// 关闭视图 : 类型码
		/// </summary>
		public static void CloseView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewBind?.ViewList == null || self.ViewBind.ViewList.Count == 0) return;
			if (!(NodeBranchHelper.TryGetBranch(self.ViewBind, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewObject view) return;

			view.OnClose();
			// ViewBind 销毁时会自动触发 SubViewClose 事件
		}

		/// <summary>
		/// 关闭所有视图 
		/// </summary>
		public static void CloseAllView(this ViewLayer self)
		{
			if (self?.ViewBind?.ViewList == null || self.ViewBind.ViewList.Count == 0) return;
			// 从顶层向下关闭，并移除空项
			for (int i = self.ViewBind.ViewList.Count - 1; i >= 0; i--)
			{
				var nodeRef = self.ViewBind.ViewList[i];
				if (nodeRef.IsNull)
				{
					self.ViewBind.IdIndexDict.Remove(nodeRef.InstanceId);
					self.ViewBind.ViewList.RemoveAt(i);
					continue;
				}
				var view = nodeRef.Value;
				view.OnClose();
			}
			self.ViewBind.ViewList.Clear();
			self.ViewBind.IdIndexDict.Clear();
		}

		/// <summary>
		/// 关闭视图
		/// </summary>
		public static void CloseView<T>(this ViewLayer self) where T : ViewObject => self.CloseView(self.TypeToCode<T>());

		/// <summary>
		/// 设置视图到顶层 : 类型码
		/// </summary>
		public static void SetTopView(this ViewLayer self, long typeCode)
		{
			if (self?.ViewBind?.ViewList == null || self.ViewBind.ViewList.Count == 0) return;
			if (!(NodeBranchHelper.TryGetBranch(self.ViewBind, out ComponentBranch branch) && branch.TryGetNode(typeCode, out INode node))) return;
			if (node is not ViewObject view) return;

			if (self.ViewBind.IdIndexDict.Remove(view.Id, out int index))
			{
				self.ViewBind.ViewList.RemoveAt(index);
				self.ViewBind.ViewList.Add(new(view));

				// 顺序更新 index 以上的层级，并移除空项
				for (int i = index; i < self.ViewBind.ViewList.Count; i++)
				{
					var nodeRef = self.ViewBind.ViewList[i];
					if (nodeRef.IsNull)
					{
						self.ViewBind.IdIndexDict.Remove(nodeRef.InstanceId);
						self.ViewBind.ViewList.RemoveAt(i);
						i--; // 移除后需要回退索引
						continue;
					}
					var v = nodeRef.Value;
					self.ViewBind.IdIndexDict[v.Id] = i;
					v.Layer = (byte)(i + 1);
					v.LayerChange();
				}
				// 最后更新自己
				var topIndex = self.ViewBind.ViewList.Count - 1;
				self.ViewBind.IdIndexDict[view.Id] = topIndex;
				view.Layer = (byte)(topIndex + 1);
				view.LayerChange();
			}
		}


		/// <summary>
		/// 设置视图到顶层（带空判断并移除空项）
		/// </summary>
		public static void SetTopView<T>(this ViewLayer self) where T : ViewObject => self.SetTopView(self.TypeToCode<T>());

		/// <summary>
		/// 尝试获取顶层窗口
		/// </summary>
		public static bool TryGetTopView<T>(this ViewLayer self, out T view)
			where T : ViewObject
		{
			view = null;

			if (self?.ViewBind?.ViewList == null || self.ViewBind.ViewList.Count == 0) return false;
			// 从顶层向下查找，并移除空项
			for (int i = self.ViewBind.ViewList.Count - 1; i >= 0; i--)
			{
				var nodeRef = self.ViewBind.ViewList[i];
				if (nodeRef.IsNull)
				{
					self.ViewBind.IdIndexDict.Remove(nodeRef.InstanceId);
					self.ViewBind.ViewList.RemoveAt(i);
					continue;
				}
				if (nodeRef.Value is T tWindow)
				{
					view = tWindow;
					return true;
				}
			}
			return false;
		}
	}
}
