using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"LiteDB.dll",
		"MemoryPack.dll",
		"System.Core.dll",
		"System.Runtime.CompilerServices.Unsafe.dll",
		"System.dll",
		"UnityEngine.CoreModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// LiteDB.ILiteCollection<object>
	// LiteDB.ILiteQueryable<object>
	// LiteDB.ILiteQueryableResult<object>
	// LiteDB.LiteCollection.<GetBsonDocs>d__52<object>
	// LiteDB.LiteCollection<object>
	// LiteDB.LiteQueryable.<>c<object>
	// LiteDB.LiteQueryable.<ToDocuments>d__26<object>
	// LiteDB.LiteQueryable<object>
	// MemoryPack.Formatters.ArrayFormatter<WorldTree.MemoryPackDataTest<object>>
	// MemoryPack.Formatters.ArrayFormatter<object>
	// MemoryPack.Formatters.ListFormatter<int>
	// MemoryPack.IMemoryPackFormatter<WorldTree.MemoryPackDataTest<object>>
	// MemoryPack.IMemoryPackFormatter<int>
	// MemoryPack.IMemoryPackFormatter<object>
	// MemoryPack.IMemoryPackable<WorldTree.MemoryPackDataTest<object>>
	// MemoryPack.IMemoryPackable<object>
	// MemoryPack.MemoryPackFormatter<System.UIntPtr>
	// MemoryPack.MemoryPackFormatter<WorldTree.MemoryPackDataTest<object>>
	// MemoryPack.MemoryPackFormatter<object>
	// System.Action<MemoryPack.Internal.BufferSegment>
	// System.Action<System.TimeSpan>
	// System.Action<UnityEngine.Vector2>
	// System.Action<WorldTree.ByteSequenceSegment>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment.Enumerator<int>
	// System.ArraySegment.Enumerator<object>
	// System.ArraySegment.Enumerator<ushort>
	// System.ArraySegment<byte>
	// System.ArraySegment<int>
	// System.ArraySegment<object>
	// System.ArraySegment<ushort>
	// System.Buffers.ArrayPool<byte>
	// System.Buffers.ConfigurableArrayPool.Bucket<byte>
	// System.Buffers.ConfigurableArrayPool<byte>
	// System.Buffers.IBufferWriter<byte>
	// System.Buffers.MemoryManager<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<byte>
	// System.ByReference<byte>
	// System.ByReference<int>
	// System.ByReference<object>
	// System.ByReference<ushort>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<long,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,long>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<WorldTree.WorldContextWorkRequest>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<WorldTree.WorldContextWorkRequest>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<WorldTree.WorldContextWorkRequest>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.Comparer<WorldTree.NodeRef<object>>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,int>
	// System.Collections.Generic.Dictionary.KeyCollection<long,long>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,int>
	// System.Collections.Generic.Dictionary.ValueCollection<long,long>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary<UnityEngine.Color,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,int>
	// System.Collections.Generic.Dictionary<long,long>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<object,ushort>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.EqualityComparer<WorldTree.NodeRef<object>>
	// System.Collections.Generic.EqualityComparer<WorldTree.Vector3Float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<long>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<long>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<long>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,WorldTree.NodeRef<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.DateTime>
	// System.Collections.Generic.ICollection<System.Decimal>
	// System.Collections.Generic.ICollection<System.Guid>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ICollection<WorldTree.Vector3Float>
	// System.Collections.Generic.ICollection<WorldTree.WorldContextWorkRequest>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<double>
	// System.Collections.Generic.ICollection<float>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<short>
	// System.Collections.Generic.IComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<long,object>
	// System.Collections.Generic.IDictionary<object,long>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,WorldTree.NodeRef<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.DateTime>
	// System.Collections.Generic.IEnumerable<System.Decimal>
	// System.Collections.Generic.IEnumerable<System.Guid>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IEnumerable<WorldTree.Vector3Float>
	// System.Collections.Generic.IEnumerable<WorldTree.WorldContextWorkRequest>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<double>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<short>
	// System.Collections.Generic.IEnumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,WorldTree.NodeRef<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.DateTime>
	// System.Collections.Generic.IEnumerator<System.Decimal>
	// System.Collections.Generic.IEnumerator<System.Guid>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IEnumerator<WorldTree.Vector3Float>
	// System.Collections.Generic.IEnumerator<WorldTree.WorldContextWorkRequest>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<double>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<short>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IList<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.IReadOnlyList<object>
	// System.Collections.Generic.KeyValuePair<System.UIntPtr,object>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Color,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,int>
	// System.Collections.Generic.KeyValuePair<long,long>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,WorldTree.NodeRef<object>>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,long>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<object,ushort>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.List.Enumerator<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<MemoryPack.Internal.BufferSegment>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<WorldTree.ByteSequenceSegment>
	// System.Collections.Generic.ObjectComparer<WorldTree.NodeRef<object>>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Color>
	// System.Collections.Generic.ObjectEqualityComparer<WorldTree.NodeRef<object>>
	// System.Collections.Generic.ObjectEqualityComparer<WorldTree.Vector3Float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<WorldTree.NodeRef<object>,object>>
	// System.Collections.Generic.Queue.Enumerator<WorldTree.NodeRef<object>>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<System.ValueTuple<WorldTree.NodeRef<object>,object>>
	// System.Collections.Generic.Queue<WorldTree.NodeRef<object>>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<object,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<object,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<object,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<object,object>
	// System.Collections.Generic.SortedDictionary<object,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<MemoryPack.Internal.BufferSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<WorldTree.ByteSequenceSegment>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<MemoryPack.Internal.BufferSegment>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<WorldTree.ByteSequenceSegment>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Converter<object,object>
	// System.Dynamic.Utils.CacheDict.Entry<object,object>
	// System.Dynamic.Utils.CacheDict<object,object>
	// System.Func<System.DateTime,byte>
	// System.Func<System.DateTime>
	// System.Func<System.Decimal,byte>
	// System.Func<System.Guid,byte>
	// System.Func<byte,byte>
	// System.Func<double,byte>
	// System.Func<float,byte>
	// System.Func<int,byte>
	// System.Func<long,byte>
	// System.Func<long,object,object>
	// System.Func<long,object>
	// System.Func<object,System.DateTime>
	// System.Func<object,System.Decimal>
	// System.Func<object,System.Guid>
	// System.Func<object,byte>
	// System.Func<object,double>
	// System.Func<object,float>
	// System.Func<object,int>
	// System.Func<object,long,long>
	// System.Func<object,long>
	// System.Func<object,object,byte,object,object>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object,short>
	// System.Func<object>
	// System.Func<short,byte>
	// System.IEquatable<WorldTree.Matrix4x4Float>
	// System.IEquatable<WorldTree.QuaternionFloat>
	// System.IEquatable<WorldTree.Triangle>
	// System.IEquatable<WorldTree.Vector2Float>
	// System.IEquatable<WorldTree.Vector3Float>
	// System.IEquatable<WorldTree.Vector4Float>
	// System.IEquatable<object>
	// System.Linq.Buffer<System.DateTime>
	// System.Linq.Buffer<System.Decimal>
	// System.Linq.Buffer<System.Guid>
	// System.Linq.Buffer<byte>
	// System.Linq.Buffer<double>
	// System.Linq.Buffer<float>
	// System.Linq.Buffer<int>
	// System.Linq.Buffer<long>
	// System.Linq.Buffer<object>
	// System.Linq.Buffer<short>
	// System.Linq.Enumerable.Iterator<System.DateTime>
	// System.Linq.Enumerable.Iterator<System.Decimal>
	// System.Linq.Enumerable.Iterator<System.Guid>
	// System.Linq.Enumerable.Iterator<byte>
	// System.Linq.Enumerable.Iterator<double>
	// System.Linq.Enumerable.Iterator<float>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<long>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.Iterator<short>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.DateTime>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Decimal>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Guid>
	// System.Linq.Enumerable.WhereEnumerableIterator<byte>
	// System.Linq.Enumerable.WhereEnumerableIterator<double>
	// System.Linq.Enumerable.WhereEnumerableIterator<float>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<long>
	// System.Linq.Enumerable.WhereEnumerableIterator<short>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,System.DateTime>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,System.Decimal>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,System.Guid>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,byte>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,double>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,float>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,long>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,short>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,System.DateTime>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,System.Decimal>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,System.Guid>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,byte>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,double>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,float>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,long>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,short>
	// System.Linq.Enumerable.WhereSelectListIterator<object,System.DateTime>
	// System.Linq.Enumerable.WhereSelectListIterator<object,System.Decimal>
	// System.Linq.Enumerable.WhereSelectListIterator<object,System.Guid>
	// System.Linq.Enumerable.WhereSelectListIterator<object,byte>
	// System.Linq.Enumerable.WhereSelectListIterator<object,double>
	// System.Linq.Enumerable.WhereSelectListIterator<object,float>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,long>
	// System.Linq.Enumerable.WhereSelectListIterator<object,short>
	// System.Memory<byte>
	// System.Nullable<int>
	// System.Nullable<long>
	// System.Predicate<MemoryPack.Internal.BufferSegment>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<WorldTree.ByteSequenceSegment>
	// System.Predicate<WorldTree.Vector3Float>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.ReadOnlyMemory<byte>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan.Enumerator<int>
	// System.ReadOnlySpan.Enumerator<object>
	// System.ReadOnlySpan.Enumerator<ushort>
	// System.ReadOnlySpan<byte>
	// System.ReadOnlySpan<int>
	// System.ReadOnlySpan<object>
	// System.ReadOnlySpan<ushort>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.DateTime>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.DateTime>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.ReadOnlyCollectionBuilder.Enumerator<object>
	// System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.DateTime>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.TrueReadOnlyCollection<object>
	// System.Span.Enumerator<byte>
	// System.Span.Enumerator<int>
	// System.Span.Enumerator<object>
	// System.Span.Enumerator<ushort>
	// System.Span<byte>
	// System.Span<int>
	// System.Span<object>
	// System.Span<ushort>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.DateTime>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<System.DateTime>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.DateTime>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<System.DateTime>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<WorldTree.NodeRef<object>,object>
	// }}

	public void RefMethods()
	{
		// LiteDB.ILiteCollection<object> LiteDB.LiteDatabase.GetCollection<object>(string,LiteDB.BsonAutoId)
		// byte[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<byte>(int,bool)
		// byte& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<byte>(byte[])
		// MemoryPack.MemoryPackFormatter<WorldTree.MemoryPackDataTest<object>> MemoryPack.MemoryPackFormatterProvider.GetFormatter<WorldTree.MemoryPackDataTest<object>>()
		// MemoryPack.MemoryPackFormatter<object> MemoryPack.MemoryPackFormatterProvider.GetFormatter<object>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<WorldTree.MemoryPackDataTest<object>>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<object>()
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<WorldTree.MemoryPackDataTest<object>>(MemoryPack.MemoryPackFormatter<WorldTree.MemoryPackDataTest<object>>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<object>(MemoryPack.MemoryPackFormatter<object>)
		// MemoryPack.IMemoryPackFormatter<WorldTree.MemoryPackDataTest<object>> MemoryPack.MemoryPackReader.GetFormatter<WorldTree.MemoryPackDataTest<object>>()
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackReader.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadPackable<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadPackable<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<float>(float&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int,int>(int&,int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long,int>(long&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long>(long&)
		// System.Void MemoryPack.MemoryPackReader.ReadValue<WorldTree.MemoryPackDataTest<object>>(WorldTree.MemoryPackDataTest<object>&)
		// System.Void MemoryPack.MemoryPackReader.ReadValue<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadValue<object>()
		// int MemoryPack.MemoryPackSerializer.Deserialize<WorldTree.MemoryPackDataTest<object>>(System.ReadOnlySpan<byte>,WorldTree.MemoryPackDataTest<object>&,MemoryPack.MemoryPackSerializerOptions)
		// System.Void MemoryPack.MemoryPackSerializer.Serialize<WorldTree.MemoryPackDataTest<object>>(MemoryPack.MemoryPackWriter&,WorldTree.MemoryPackDataTest<object>&)
		// byte[] MemoryPack.MemoryPackSerializer.Serialize<WorldTree.MemoryPackDataTest<object>>(WorldTree.MemoryPackDataTest<object>&,MemoryPack.MemoryPackSerializerOptions)
		// MemoryPack.IMemoryPackFormatter<WorldTree.MemoryPackDataTest<object>> MemoryPack.MemoryPackWriter.GetFormatter<WorldTree.MemoryPackDataTest<object>>()
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackWriter.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackWriter.WritePackable<object>(object&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<float>(float&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long,int>(long&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<int,int,int>(byte,int&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<WorldTree.MemoryPackDataTest<object>>(WorldTree.MemoryPackDataTest<object>&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<object>(object&)
		// object System.Activator.CreateInstance<object>()
		// int System.Array.BinarySearch<object>(object[],int,int,object,System.Collections.Generic.IComparer<object>)
		// byte[] System.Array.Empty<byte>()
		// object[] System.Array.Empty<object>()
		// int System.Array.IndexOf<object>(object[],object,int,int)
		// int System.Array.IndexOfImpl<object>(object[],object,int,int)
		// int System.Array.LastIndexOf<object>(object[],object,int,int)
		// int System.Array.LastIndexOfImpl<object>(object[],object,int,int)
		// System.Void System.Array.Reverse<object>(object[],int,int)
		// System.Void System.Array.Sort<object>(object[],int,int,System.Collections.Generic.IComparer<object>)
		// bool System.Collections.Generic.CollectionExtensions.Remove<object,long>(System.Collections.Generic.IDictionary<object,long>,object,long&)
		// bool System.Collections.Generic.CollectionExtensions.Remove<object,object>(System.Collections.Generic.IDictionary<object,object>,object,object&)
		// System.Collections.ObjectModel.ReadOnlyCollection<object> System.Dynamic.Utils.CollectionExtensions.ToReadOnly<object>(System.Collections.Generic.IEnumerable<object>)
		// int System.HashCode.Combine<float,float,float,float,float,float,float>(float,float,float,float,float,float,float)
		// bool System.Linq.Enumerable.Contains<System.Collections.Generic.KeyValuePair<long,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>,System.Collections.Generic.KeyValuePair<long,object>)
		// bool System.Linq.Enumerable.Contains<System.Collections.Generic.KeyValuePair<long,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>,System.Collections.Generic.KeyValuePair<long,object>,System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.IEnumerable<System.DateTime> System.Linq.Enumerable.Select<object,System.DateTime>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.DateTime>)
		// System.Collections.Generic.IEnumerable<System.Decimal> System.Linq.Enumerable.Select<object,System.Decimal>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Decimal>)
		// System.Collections.Generic.IEnumerable<System.Guid> System.Linq.Enumerable.Select<object,System.Guid>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Guid>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Select<object,byte>(System.Collections.Generic.IEnumerable<object>,System.Func<object,byte>)
		// System.Collections.Generic.IEnumerable<double> System.Linq.Enumerable.Select<object,double>(System.Collections.Generic.IEnumerable<object>,System.Func<object,double>)
		// System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Select<object,float>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Select<object,long>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
		// System.Collections.Generic.IEnumerable<short> System.Linq.Enumerable.Select<object,short>(System.Collections.Generic.IEnumerable<object>,System.Func<object,short>)
		// System.DateTime[] System.Linq.Enumerable.ToArray<System.DateTime>(System.Collections.Generic.IEnumerable<System.DateTime>)
		// System.Decimal[] System.Linq.Enumerable.ToArray<System.Decimal>(System.Collections.Generic.IEnumerable<System.Decimal>)
		// System.Guid[] System.Linq.Enumerable.ToArray<System.Guid>(System.Collections.Generic.IEnumerable<System.Guid>)
		// byte[] System.Linq.Enumerable.ToArray<byte>(System.Collections.Generic.IEnumerable<byte>)
		// double[] System.Linq.Enumerable.ToArray<double>(System.Collections.Generic.IEnumerable<double>)
		// float[] System.Linq.Enumerable.ToArray<float>(System.Collections.Generic.IEnumerable<float>)
		// int[] System.Linq.Enumerable.ToArray<int>(System.Collections.Generic.IEnumerable<int>)
		// long[] System.Linq.Enumerable.ToArray<long>(System.Collections.Generic.IEnumerable<long>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// short[] System.Linq.Enumerable.ToArray<short>(System.Collections.Generic.IEnumerable<short>)
		// System.Collections.Generic.IEnumerable<System.DateTime> System.Linq.Enumerable.Iterator<object>.Select<System.DateTime>(System.Func<object,System.DateTime>)
		// System.Collections.Generic.IEnumerable<System.Decimal> System.Linq.Enumerable.Iterator<object>.Select<System.Decimal>(System.Func<object,System.Decimal>)
		// System.Collections.Generic.IEnumerable<System.Guid> System.Linq.Enumerable.Iterator<object>.Select<System.Guid>(System.Func<object,System.Guid>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Iterator<object>.Select<byte>(System.Func<object,byte>)
		// System.Collections.Generic.IEnumerable<double> System.Linq.Enumerable.Iterator<object>.Select<double>(System.Func<object,double>)
		// System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Iterator<object>.Select<float>(System.Func<object,float>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Iterator<object>.Select<long>(System.Func<object,long>)
		// System.Collections.Generic.IEnumerable<short> System.Linq.Enumerable.Iterator<object>.Select<short>(System.Func<object,short>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,System.Linq.Expressions.ParameterExpression[])
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,bool,System.Collections.Generic.IEnumerable<System.Linq.Expressions.ParameterExpression>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,string,bool,System.Collections.Generic.IEnumerable<System.Linq.Expressions.ParameterExpression>)
		// System.Memory<byte> System.MemoryExtensions.AsMemory<byte>(byte[],int,int)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[])
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[],int,int)
		// System.Span<object> System.MemoryExtensions.AsSpan<object>(object[])
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<object>(object&)
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<System.Collections.Generic.KeyValuePair<object,object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<WorldTree.MemoryPackDataTest<object>>()
		// bool System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<object>()
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// object& System.Runtime.CompilerServices.Unsafe.Add<object>(object&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// byte& System.Runtime.CompilerServices.Unsafe.As<object,byte>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<byte,object>(byte&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Collections.Generic.KeyValuePair<object,object>& System.Runtime.CompilerServices.Unsafe.AsRef<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.KeyValuePair<object,object>&)
		// WorldTree.MemoryPackDataTest<object>& System.Runtime.CompilerServices.Unsafe.AsRef<WorldTree.MemoryPackDataTest<object>>(WorldTree.MemoryPackDataTest<object>&)
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// bool System.Runtime.CompilerServices.Unsafe.IsAddressLessThan<object>(object&,object&)
		// System.Collections.Generic.KeyValuePair<object,object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<System.Collections.Generic.KeyValuePair<object,object>>(byte&)
		// WorldTree.MemoryPackDataTest<object> System.Runtime.CompilerServices.Unsafe.ReadUnaligned<WorldTree.MemoryPackDataTest<object>>(byte&)
		// byte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(byte&)
		// double System.Runtime.CompilerServices.Unsafe.ReadUnaligned<double>(byte&)
		// float System.Runtime.CompilerServices.Unsafe.ReadUnaligned<float>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(byte&)
		// long System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>(byte&)
		// object System.Runtime.CompilerServices.Unsafe.ReadUnaligned<object>(byte&)
		// short System.Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<System.Collections.Generic.KeyValuePair<object,object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<WorldTree.MemoryPackDataTest<object>>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<byte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<double>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<float>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<long>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<object>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<short>()
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<System.Collections.Generic.KeyValuePair<object,object>>(byte&,System.Collections.Generic.KeyValuePair<object,object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<WorldTree.MemoryPackDataTest<object>>(byte&,WorldTree.MemoryPackDataTest<object>)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<byte>(byte&,byte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<double>(byte&,double)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<float>(byte&,float)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<int>(byte&,int)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<long>(byte&,long)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<object>(byte&,object)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<short>(byte&,short)
		// System.ReadOnlySpan<byte> System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan<byte>(byte&,int)
		// System.Span<byte> System.Runtime.InteropServices.MemoryMarshal.CreateSpan<byte>(byte&,int)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.ReadOnlySpan<byte>)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.Span<byte>)
		// object& System.Runtime.InteropServices.MemoryMarshal.GetReference<object>(System.Span<object>)
		// System.Threading.Tasks.Task<System.DateTime> System.Threading.Tasks.Task.Run<System.DateTime>(System.Func<System.DateTime>)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
	}
}