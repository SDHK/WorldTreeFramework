using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"System.Core.dll",
		"System.Runtime.CompilerServices.Unsafe.dll",
		"UnityEngine.CoreModule.dll",
		"WorldTree.Core.dll",
		"WorldTree.Module.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<System.TimeSpan>
	// System.Action<UnityEngine.Vector2>
	// System.Action<WorldTree.ByteSequenceSegment>
	// System.Action<WorldTree.InputInfo>
	// System.Action<WorldTree.StepAssemblyData>
	// System.Action<WorldTree.VarValue>
	// System.Action<byte>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,object>
	// System.Action<object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<long,byte>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<long,byte>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<long,byte>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<long,byte>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<long,byte>
	// System.Collections.Concurrent.ConcurrentDictionary<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ArraySortHelper<WorldTree.InputInfo>
	// System.Collections.Generic.ArraySortHelper<WorldTree.StepAssemblyData>
	// System.Collections.Generic.ArraySortHelper<WorldTree.VarValue>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.Comparer<WorldTree.InputInfo>
	// System.Collections.Generic.Comparer<WorldTree.StepAssemblyData>
	// System.Collections.Generic.Comparer<WorldTree.VarValue>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,long>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,long>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,long>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.EqualityComparer<WorldTree.TreeDataNodeDataTest3>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<long>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<long>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ICollection<WorldTree.InputInfo>
	// System.Collections.Generic.ICollection<WorldTree.StepAssemblyData>
	// System.Collections.Generic.ICollection<WorldTree.VarValue>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IComparer<WorldTree.InputInfo>
	// System.Collections.Generic.IComparer<WorldTree.StepAssemblyData>
	// System.Collections.Generic.IComparer<WorldTree.VarValue>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<long,byte>
	// System.Collections.Generic.IDictionary<long,object>
	// System.Collections.Generic.IDictionary<object,long>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IEnumerable<WorldTree.InputInfo>
	// System.Collections.Generic.IEnumerable<WorldTree.StepAssemblyData>
	// System.Collections.Generic.IEnumerable<WorldTree.VarValue>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IEnumerator<WorldTree.InputInfo>
	// System.Collections.Generic.IEnumerator<WorldTree.StepAssemblyData>
	// System.Collections.Generic.IEnumerator<WorldTree.VarValue>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IList<WorldTree.InputInfo>
	// System.Collections.Generic.IList<WorldTree.StepAssemblyData>
	// System.Collections.Generic.IList<WorldTree.VarValue>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,byte>
	// System.Collections.Generic.KeyValuePair<long,long>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,long>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.List.Enumerator<WorldTree.InputInfo>
	// System.Collections.Generic.List.Enumerator<WorldTree.StepAssemblyData>
	// System.Collections.Generic.List.Enumerator<WorldTree.VarValue>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.List<WorldTree.InputInfo>
	// System.Collections.Generic.List<WorldTree.StepAssemblyData>
	// System.Collections.Generic.List<WorldTree.VarValue>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ObjectComparer<WorldTree.InputInfo>
	// System.Collections.Generic.ObjectComparer<WorldTree.StepAssemblyData>
	// System.Collections.Generic.ObjectComparer<WorldTree.VarValue>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.ObjectEqualityComparer<WorldTree.TreeDataNodeDataTest3>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<WorldTree.ByteSequenceSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<WorldTree.InputInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<WorldTree.StepAssemblyData>
	// System.Collections.ObjectModel.ReadOnlyCollection<WorldTree.VarValue>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<WorldTree.ByteSequenceSegment>
	// System.Comparison<WorldTree.InputInfo>
	// System.Comparison<WorldTree.StepAssemblyData>
	// System.Comparison<WorldTree.VarValue>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Func<int>
	// System.Func<long,byte,byte>
	// System.Func<long,byte>
	// System.Func<long,object,object>
	// System.Func<long,object>
	// System.Func<object,int>
	// System.Func<object,long,long>
	// System.Func<object,long>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Nullable<int>
	// System.Nullable<long>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<WorldTree.ByteSequenceSegment>
	// System.Predicate<WorldTree.InputInfo>
	// System.Predicate<WorldTree.StepAssemblyData>
	// System.Predicate<WorldTree.VarValue>
	// System.Predicate<byte>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.TaskCompletionSource<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory<int>
	// WorldTree.AsBranch<object>
	// WorldTree.AsGenericBranch<long>
	// WorldTree.AsRule<object>
	// WorldTree.AwaiterBase<int>
	// WorldTree.AwaiterBase<object>
	// WorldTree.AwakeRule<object,int>
	// WorldTree.AwakeRule<object,object,object>
	// WorldTree.Branch<long>
	// WorldTree.Branch<object>
	// WorldTree.CallRule<object,object,float,int,object>
	// WorldTree.CallRuleAsync<object,object,object,object>
	// WorldTree.ConcurrentList<object>
	// WorldTree.Config<int>
	// WorldTree.ConfigGroup<int,object>
	// WorldTree.GenericOf<long,object>
	// WorldTree.IBranch<int>
	// WorldTree.IBranch<long>
	// WorldTree.IBranch<object>
	// WorldTree.ICallRule<float,int,object>
	// WorldTree.ICallRuleAsync<object,object>
	// WorldTree.IDataCollection<object>
	// WorldTree.INodeProxyRule.<>c__11<object,object>
	// WorldTree.ISendRefRule<object,int>
	// WorldTree.ISendRefRule<object>
	// WorldTree.ISendRule<System.TimeSpan>
	// WorldTree.ISendRule<WorldTree.InputData>
	// WorldTree.ISendRule<int>
	// WorldTree.ISendRule<object>
	// WorldTree.Internal.TreeTaskMethodBuilder<int>
	// WorldTree.Internal.TreeTaskMethodBuilder<object>
	// WorldTree.NodeRuleHelper.<CallRuleAsync>d__53<object,object,object,object>
	// WorldTree.PoolManagerBase<object>
	// WorldTree.Rule<WorldTree.NodeDataTest,object>
	// WorldTree.Rule<WorldTree.TreeDataNodeDataTest3,object>
	// WorldTree.Rule<object,object>
	// WorldTree.SendRule<object,object,System.TimeSpan>
	// WorldTree.SendRule<object,object,WorldTree.InputData>
	// WorldTree.SendRule<object,object,int>
	// WorldTree.SendRule<object,object,object,object>
	// WorldTree.SendRule<object,object>
	// WorldTree.TreeArray<object>
	// WorldTree.TreeDataDeserializeRule<WorldTree.TreeDataNodeDataTest3>
	// WorldTree.TreeDataDeserializeRule<object>
	// WorldTree.TreeDataSerializeRule<WorldTree.TreeDataNodeDataTest3>
	// WorldTree.TreeDataSerializeRule<object>
	// WorldTree.TreeList.Enumerator<object>
	// WorldTree.TreeList<object>
	// WorldTree.TreePackDeserializeRule<WorldTree.NodeDataTest>
	// WorldTree.TreePackDeserializeRule<object>
	// WorldTree.TreePackSerializeRule<WorldTree.NodeDataTest>
	// WorldTree.TreePackSerializeRule<object>
	// WorldTree.TreeTaskExtension.<>c__DisplayClass5_0<int>
	// WorldTree.TreeTaskExtension.<GetAwaiter>d__5<int>
	// WorldTree.TreeValueBase<float>
	// WorldTree.TreeValueBase<object>
	// WorldTree.TypeInfo<object>
	// WorldTree.UnitDictionary<UnityEngine.Color,object>
	// WorldTree.UnitDictionary<long,long>
	// WorldTree.UnitDictionary<long,object>
	// WorldTree.UnitDictionary<object,object>
	// WorldTree.UnitList<object>
	// WorldTree.UnitStack<object>
	// WorldTree.View<object>
	// }}

	public void RefMethods()
	{
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<object>()
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// byte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(byte&)
		// double System.Runtime.CompilerServices.Unsafe.ReadUnaligned<double>(byte&)
		// float System.Runtime.CompilerServices.Unsafe.ReadUnaligned<float>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(byte&)
		// long System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>(byte&)
		// object System.Runtime.CompilerServices.Unsafe.ReadUnaligned<object>(byte&)
		// short System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<byte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<double>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<float>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<long>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<object>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<short>()
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<byte>(byte&,byte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<double>(byte&,double)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<float>(byte&,float)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<int>(byte&,int)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<long>(byte&,long)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<object>(byte&,object)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<short>(byte&,short)
		// System.Threading.Tasks.Task<int> System.Threading.Tasks.Task.FromResult<int>(int)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// long WorldTree.BasicTypeInfoRule.TypeToCode<object>(WorldTree.IBasic)
		// byte WorldTree.ByteSequence.ReadUnmanaged<byte>(byte&)
		// double WorldTree.ByteSequence.ReadUnmanaged<double>(double&)
		// float WorldTree.ByteSequence.ReadUnmanaged<float>(float&)
		// int WorldTree.ByteSequence.ReadUnmanaged<int>(int&)
		// long WorldTree.ByteSequence.ReadUnmanaged<long>(long&)
		// object WorldTree.ByteSequence.ReadUnmanaged<object>(object&)
		// short WorldTree.ByteSequence.ReadUnmanaged<short>(short&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<byte>(byte&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<double>(double&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<float>(float&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<int>(int&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<long>(long&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<object>(object&)
		// System.Void WorldTree.ByteSequence.WriteUnmanaged<short>(short&)
		// object WorldTree.ChildBranchSupplement.AddChild<object,object,object>(object,object&,object)
		// object WorldTree.ComponentBranchSupplement.AddComponent<object,object,int>(object,object&,int)
		// object WorldTree.ComponentBranchSupplement.AddComponent<object,object,object>(object,object&,object)
		// long WorldTree.CoreObjectTypeInfoRule.TypeToCode<object>(WorldTree.ICoreObject)
		// object WorldTree.DataBaseProxyRule.Find<object>(WorldTree.DataBaseProxy,long)
		// System.Void WorldTree.DataBaseProxyRule.Insert<object>(WorldTree.DataBaseProxy,long,object)
		// bool WorldTree.DataBaseProxyRule.Update<object>(WorldTree.DataBaseProxy,long,object)
		// object WorldTree.DictionaryExtension.GetOrAdd<object,object>(System.Collections.Concurrent.ConcurrentDictionary<object,object>,object)
		// object WorldTree.GenericBranchSupplement.AddGeneric<int,object,object>(object,int,object&)
		// object WorldTree.GenericBranchSupplement.AddGeneric<long,object,object>(object,long,object&)
		// object WorldTree.GenericBranchSupplement.AddGeneric<object,object,object>(object,object,object&)
		// bool WorldTree.GenericBranchSupplement.TryGetGeneric<long,object,object>(object,long,object&)
		// bool WorldTree.GenericBranchSupplement.TryGetGeneric<object,object,object>(object,object,object&)
		// bool WorldTree.IBranch<object>.TryAddNode<object>(object,object)
		// WorldTree.IDataCollection<object> WorldTree.IDataBase.GetCollection<object>()
		// bool WorldTree.INode.TryAddSelfToTree<object,int>(int,WorldTree.INode)
		// bool WorldTree.INode.TryAddSelfToTree<object,long>(long,WorldTree.INode)
		// bool WorldTree.INode.TryAddSelfToTree<object,object>(object,WorldTree.INode)
		// bool WorldTree.INode.TryGraftSelfToTree<object,long>(long,WorldTree.INode)
		// bool WorldTree.INodeProxyRule.TryGraftSelfToTree<object,object>(WorldTree.INode,object,WorldTree.INode)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6>(object&,WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5>(object&,WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7>(object&,WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<CatchException>d__9>(object&,WorldTree.Server.TreeTaskTestRule.<CatchException>d__9&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19>(object&,WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15>(object&,WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11>(object&,WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13>(object&,WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12>(object&,WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18>(object&,WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4>(object&,WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<Run>d__1>(object&,WorldTree.Server.TreeTaskTestRule.<Run>d__1&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14>(object&,WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10>(object&,WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17>(object&,WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16>(object&,WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3>(object&,WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.TaskTest.<TaskC>d__5>(object&,WorldTree.TaskTest.<TaskC>d__5&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.TaskTest.<TaskD>d__6>(object&,WorldTree.TaskTest.<TaskD>d__6&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.TaskTest.<Test>d__3>(object&,WorldTree.TaskTest.<Test>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.TaskTest.<TestB>d__4>(object&,WorldTree.TaskTest.<TestB>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.YooAssetTest.<InitializeYooAsset1>d__3>(object&,WorldTree.YooAssetTest.<InitializeYooAsset1>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.YooAssetTest.<InitializeYooAsset2>d__4>(object&,WorldTree.YooAssetTest.<InitializeYooAsset2>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.AwaitUnsafeOnCompleted<object,WorldTree.YooAssetTest.<InitializeYooAsset>d__2>(object&,WorldTree.YooAssetTest.<InitializeYooAsset>d__2&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2>(object&,WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8>(object&,WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>>(object&,WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>>(object&,WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>>(object&,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>>(object&,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>>(object&,WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6>(WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5>(WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7>(WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<CatchException>d__9>(WorldTree.Server.TreeTaskTestRule.<CatchException>d__9&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19>(WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15>(WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11>(WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13>(WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12>(WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18>(WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4>(WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<Run>d__1>(WorldTree.Server.TreeTaskTestRule.<Run>d__1&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14>(WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10>(WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17>(WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16>(WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3>(WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.TaskTest.<TaskC>d__5>(WorldTree.TaskTest.<TaskC>d__5&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.TaskTest.<TaskD>d__6>(WorldTree.TaskTest.<TaskD>d__6&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.TaskTest.<Test>d__3>(WorldTree.TaskTest.<Test>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.TaskTest.<TestB>d__4>(WorldTree.TaskTest.<TestB>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.YooAssetTest.<InitializeYooAsset1>d__3>(WorldTree.YooAssetTest.<InitializeYooAsset1>d__3&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.YooAssetTest.<InitializeYooAsset2>d__4>(WorldTree.YooAssetTest.<InitializeYooAsset2>d__4&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder.Start<WorldTree.YooAssetTest.<InitializeYooAsset>d__2>(WorldTree.YooAssetTest.<InitializeYooAsset>d__2&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<int>.Start<WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2>(WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<int>.Start<WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8>(WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<int>.Start<object>(object&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>>(WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>>(WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>>(WorldTree.AddressablesManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>>(WorldTree.AssetManager.<LoadAssetAsync>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__0<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__1<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>>(WorldTree.GameObjectNodeHelper.<AddGameObjectNode>d__2<object,object>&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6>(WorldTree.Server.TreeTaskTestRule.<BridgeLink>d__6&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5>(WorldTree.Server.TreeTaskTestRule.<BridgeTask>d__5&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7>(WorldTree.Server.TreeTaskTestRule.<CancelWait>d__7&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<CatchException>d__9>(WorldTree.Server.TreeTaskTestRule.<CatchException>d__9&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19>(WorldTree.Server.TreeTaskTestRule.<EnterQueue>d__19&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15>(WorldTree.Server.TreeTaskTestRule.<LinkBody>d__15&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2>(WorldTree.Server.TreeTaskTestRule.<NestedAdd>d__2&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11>(WorldTree.Server.TreeTaskTestRule.<PropagateBody>d__11&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13>(WorldTree.Server.TreeTaskTestRule.<PropagateInner>d__13&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12>(WorldTree.Server.TreeTaskTestRule.<PropagateMid>d__12&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18>(WorldTree.Server.TreeTaskTestRule.<QueueLock>d__18&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4>(WorldTree.Server.TreeTaskTestRule.<RealDelay>d__4&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<Run>d__1>(WorldTree.Server.TreeTaskTestRule.<Run>d__1&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8>(WorldTree.Server.TreeTaskTestRule.<ThrowInside>d__8&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14>(WorldTree.Server.TreeTaskTestRule.<TokenLink>d__14&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10>(WorldTree.Server.TreeTaskTestRule.<TokenPropagate>d__10&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17>(WorldTree.Server.TreeTaskTestRule.<WhileBody>d__17&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16>(WorldTree.Server.TreeTaskTestRule.<WhileLoop>d__16&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3>(WorldTree.Server.TreeTaskTestRule.<YieldFrames>d__3&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.TaskTest.<TaskC>d__5>(WorldTree.TaskTest.<TaskC>d__5&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.TaskTest.<TaskD>d__6>(WorldTree.TaskTest.<TaskD>d__6&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.TaskTest.<Test>d__3>(WorldTree.TaskTest.<Test>d__3&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.TaskTest.<TestB>d__4>(WorldTree.TaskTest.<TestB>d__4&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.YooAssetTest.<InitializeYooAsset1>d__3>(WorldTree.YooAssetTest.<InitializeYooAsset1>d__3&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.YooAssetTest.<InitializeYooAsset2>d__4>(WorldTree.YooAssetTest.<InitializeYooAsset2>d__4&)
		// System.Void WorldTree.Internal.TreeTaskStateMachine.SetStateMachine<WorldTree.YooAssetTest.<InitializeYooAsset>d__2>(WorldTree.YooAssetTest.<InitializeYooAsset>d__2&)
		// bool WorldTree.Node.TryGraftSelfToTree<object,object>(object,WorldTree.INode)
		// object WorldTree.NodeBranchHelper.AddNode<object,object,int,object>(object,object,int,object&)
		// object WorldTree.NodeBranchHelper.AddNode<object,object,long,object,int>(object,object,long,object&,int)
		// object WorldTree.NodeBranchHelper.AddNode<object,object,long,object,object>(object,object,long,object&,object)
		// object WorldTree.NodeBranchHelper.AddNode<object,object,long,object>(object,object,long,object&)
		// object WorldTree.NodeBranchHelper.AddNode<object,object,object,object>(object,object,object,object&)
		// WorldTree.INode WorldTree.NodeBranchHelper.AddNodeToTree<object,int>(WorldTree.INode,object,int,WorldTree.INode)
		// WorldTree.INode WorldTree.NodeBranchHelper.AddNodeToTree<object,long,int>(WorldTree.INode,object,long,WorldTree.INode,int)
		// WorldTree.INode WorldTree.NodeBranchHelper.AddNodeToTree<object,long,object>(WorldTree.INode,object,long,WorldTree.INode,object)
		// WorldTree.INode WorldTree.NodeBranchHelper.AddNodeToTree<object,long>(WorldTree.INode,object,long,WorldTree.INode)
		// WorldTree.INode WorldTree.NodeBranchHelper.AddNodeToTree<object,object>(WorldTree.INode,object,object,WorldTree.INode)
		// long WorldTree.NodeBranchHelper.GetKey<long>(WorldTree.INode)
		// object WorldTree.NodeRule.GetParent<object,object>(object,object&)
		// WorldTree.TreeTask<object> WorldTree.NodeRuleHelper.CallRuleAsync<object,object,object,object>(object,object,object,object)
		// bool WorldTree.NodeRuleHelper.TrySendRule<object,int>(WorldTree.INode,object,int)
		// bool WorldTree.NodeRuleHelper.TrySendRule<object,object>(WorldTree.INode,object,object)
		// bool WorldTree.PoolManagerBase<object>.TryGet<object>(object&)
		// bool WorldTree.PoolManagerBase<object>.TryNewOrGetPool<object>(object&)
		// System.Void WorldTree.RuleExecutorSendRule.Send<object,System.TimeSpan>(WorldTree.IRuleExecutor<object>,System.TimeSpan)
		// System.Void WorldTree.RuleExecutorSendRule.Send<object>(WorldTree.IRuleExecutor<object>)
		// System.Void WorldTree.RuleGroupSendRule.Send<object>(WorldTree.IRuleGroup<object>,WorldTree.INode)
		// bool WorldTree.RuleGroupSendRule.TrySend<object>(WorldTree.IRuleGroup<object>,WorldTree.INode)
		// System.Void WorldTree.RuleListSendRefRule.SendRef<object,object,int>(WorldTree.IRuleList<object>,WorldTree.INode,object&,int&)
		// System.Void WorldTree.RuleListSendRefRule.SendRef<object,object>(WorldTree.IRuleList<object>,WorldTree.INode,object&)
		// System.Void WorldTree.RuleListSendRule.Send<object,System.TimeSpan>(WorldTree.IRuleList<object>,WorldTree.INode,System.TimeSpan)
		// System.Void WorldTree.RuleListSendRule.Send<object,int>(WorldTree.IRuleList<object>,WorldTree.INode,int)
		// System.Void WorldTree.RuleListSendRule.Send<object,object>(WorldTree.IRuleList<object>,WorldTree.INode,object)
		// System.Void WorldTree.RuleListSendRule.Send<object>(WorldTree.IRuleList<object>,WorldTree.INode)
		// System.Void WorldTree.RuleManager.SupportGenericRule<object>(System.Type)
		// bool WorldTree.RuleManager.TryGetRuleList<object>(long,WorldTree.IRuleList<object>&)
		// bool WorldTree.RuleManager.TryGetRuleList<object>(long,WorldTree.RuleList&)
		// System.Void WorldTree.RuleMulticastRule.Add<object,object>(WorldTree.RuleMulticast<object>,object)
		// System.Void WorldTree.TreeDataByteSequence.Deserialize<object>(object&)
		// System.Void WorldTree.TreeDataByteSequence.ReadValue<WorldTree.NodeRef<object>>(WorldTree.NodeRef<object>&)
		// System.Void WorldTree.TreeDataByteSequence.ReadValue<float>(float&)
		// System.Void WorldTree.TreeDataByteSequence.ReadValue<int>(int&)
		// System.Void WorldTree.TreeDataByteSequence.ReadValue<long>(long&)
		// System.Void WorldTree.TreeDataByteSequence.ReadValue<object>(object&)
		// byte WorldTree.TreeDataByteSequence.ReadValue<byte>()
		// long WorldTree.TreeDataByteSequence.ReadValue<long>()
		// object WorldTree.TreeDataByteSequence.ReadValue<object>()
		// ushort WorldTree.TreeDataByteSequence.ReadValue<ushort>()
		// bool WorldTree.TreeDataByteSequence.TryWriteDataHead<WorldTree.TreeDataNodeDataTest3>(object&,WorldTree.SerializedTypeMode,int,WorldTree.TreeDataNodeDataTest3&,bool,bool,System.Type)
		// bool WorldTree.TreeDataByteSequence.TryWriteDataHead<object>(object&,WorldTree.SerializedTypeMode,int,object&,bool,bool,System.Type)
		// bool WorldTree.TreeDataByteSequence.WriteCheckNull<WorldTree.TreeDataNodeDataTest3>(object&,int,WorldTree.TreeDataNodeDataTest3&)
		// bool WorldTree.TreeDataByteSequence.WriteCheckNull<object>(object&,int,object&)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<WorldTree.NodeRef<object>>(WorldTree.NodeRef<object>&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<byte>(byte&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<float>(float&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<int>(int&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<long>(long&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<object>(object&,WorldTree.SerializedTypeMode)
		// System.Void WorldTree.TreeDataByteSequence.WriteValue<ushort>(ushort&,WorldTree.SerializedTypeMode)
		// object WorldTree.TreeDataHelper.Deseralize<object>(WorldTree.INode,byte[])
		// object WorldTree.TreeDataHelper.Deseralize<object>(WorldTree.INode,byte[],object&)
		// System.Void WorldTree.TreePackByteSequence.ReadValue<object>(object&)
		// System.Void WorldTree.TreePackByteSequence.WriteValue<object>(object&)
		// System.Void WorldTree.TreePackByteSequenceRule.Deserialize<object>(WorldTree.TreePackByteSequence,object&)
		// System.Void WorldTree.TreePackByteSequenceRule.Serialize<object>(WorldTree.TreePackByteSequence,object&)
		// WorldTree.TreeTask<int> WorldTree.TreeTaskExtension.GetAwaiter<int>(WorldTree.INode,System.Threading.Tasks.Task<int>)
		// WorldTree.Internal.TreeTaskLink<int> WorldTree.TreeTaskExtension.TreeTaskLink<int>(WorldTree.INode,System.Threading.Tasks.Task<int>)
		// WorldTree.TreeTweenBase WorldTree.TreeTweenBase.SetCurve<object>()
		// WorldTree.TreeTween<float> WorldTree.TreeTweenRule.GetTween<float>(WorldTree.TreeValueBase<float>,float,float)
		// WorldTree.TreeTween<object> WorldTree.TreeTweenRule.GetTween<object>(WorldTree.TreeValueBase<object>,object,float)
		// WorldTree.TreeTween<float> WorldTree.TreeTweenRule.Set<float>(WorldTree.TreeTween<float>,float,float)
		// WorldTree.TreeTween<object> WorldTree.TreeTweenRule.Set<object>(WorldTree.TreeTween<object>,object,float)
		// System.Void WorldTree.TreeValueBaseRule.Bind<float,int>(WorldTree.TreeValueBase<float>,WorldTree.TreeValueBase<int>)
		// System.Void WorldTree.TreeValueBaseRule.Bind<float,object>(WorldTree.TreeValueBase<float>,WorldTree.TreeValueBase<object>)
		// bool WorldTree.ViewLayerRule.TryAddView<object>(WorldTree.ViewLayer,object&)
		// WorldTree.RuleBroadcast<object> WorldTree.WorldTreeCoreExtension.GetRuleBroadcast<object>(WorldTree.World,WorldTree.RuleBroadcast<object>&)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
	}
}