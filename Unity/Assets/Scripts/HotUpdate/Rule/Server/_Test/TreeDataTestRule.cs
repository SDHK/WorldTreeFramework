

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	public static partial class TreeDataTestRule
	{
		static unsafe OnAdd<TreeDataTest> OnAdd = (self) =>
		{

			Array aList = new int[] { };
			self.Log($"{aList.Rank}:{aList.GetLength(0)}");


			int[,] as1s = new int[2, 3];

			DangerousReadUnmanagedArray1(ref Unsafe.AsRef<Array>(Unsafe.AsPointer(ref as1s)));

			self.Log($"{as1s.Rank}:? {as1s[0, 0]}");

			//self.Log($"TreeDataTestRule  {typeof(int[,])}");

			//AData data = new AData();

			//data.AInt = 401;

			//self.AddTemp(out TreeDataByteSequence sequenceWrite).Serialize(data);

			//byte[] bytes = sequenceWrite.ToBytes();

			//self.Log($"序列化字节长度{bytes.Length}");

			//self.AddTemp(out TreeDataByteSequence sequenceRead).SetBytes(bytes);

			//AData data2 = null;
			//sequenceRead.Deserialize(ref data2);

			//string logText = $"反序列化{data2.AInt} ";

			//self.Log(logText);
		};

		/// <summary>
		/// 危险写入非托管数组
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		public static unsafe void DangerousReadUnmanagedArray1(ref Array value)
		{
		}

	}


}