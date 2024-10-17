

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	public static partial class TreeDataTestRule
	{
		static unsafe OnAdd<TreeDataTest> OnAdd = (self) =>
		{

			//Array aList = new int[] { };
			//self.Log($"{aList.Rank}:{aList.GetLength(0)}");


			//int[,] as1s = new int[2, 3];

			//DangerousReadUnmanagedArray1(ref Unsafe.AsRef<Array>(Unsafe.AsPointer(ref as1s)));

			//self.Log($"{as1s.Rank}:? {as1s[0, 0]}");

			//self.Log($"TreeDataTestRule  {typeof(int[,])}");

			AData data = new AData();

			data.AInt = 401.5f;
			data.Ints = new int[][,,]{
				new int[2,2,5]{ { { 1, 2, 30, 4, 5 }, { 20, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }, { 2, 5, 9, 5, 2 } } },
				new int[2,1,5]{ { { 1220, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }} },
			};

			self.AddTemp(out TreeDataByteSequence sequenceWrite).Serialize(data);

			byte[] bytes = sequenceWrite.ToBytes();

			self.Log($"序列化字节长度{bytes.Length}");

			self.AddTemp(out TreeDataByteSequence sequenceRead).SetBytes(bytes);
			AData data2 = new AData();
			sequenceRead.Deserialize(ref data2);

			string logText = $"反序列化{data2.AInt} ";

			logText += $"数组数量{data2.Ints.Length} :";
			foreach (var item in data2.Ints)
			{
				logText += $"数组长度{item.Length} :";

				foreach (var item1 in item)
				{
					logText += $"{item1} ";
				}
				//logText += $"{item} ";
			}
			self.Log(logText);
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