

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	public static partial class TreeDataTestRule
	{
		static int Key = nameof(Key).GetFNV1aHash32();
		static int Value = nameof(Value).GetFNV1aHash32();
		static unsafe OnAdd<TreeDataTest> OnAdd = (self) =>
		{
			//self.Log(Key + " :: " + Value);

			AData data = new AData();

			data.AInt = 401.5f;
			data.Ints = new int[][,,]{
				new int[2,2,5]{ { { 1, 2, 30, 4, 5 }, { 20, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }, { 2, 5, 9, 5, 2 } } },
				new int[2,1,5]{ { { 1220, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }} },
			};

			data.DataDict = new Dictionary<int, string>()
			{
				{ 1, "1.1f测" },
				{ 2, "2.2f测试" },
				{ 3, "3.3f" },
				{ 4, "4.4f" },
				{ 5, "5.5f"},
			};

			ADataBase aDataBase = data;
			self.AddTemp(out TreeDataByteSequence sequenceWrite).Serialize(aDataBase);

			byte[] bytes = sequenceWrite.ToBytes();

			self.Log($"序列化字节长度{bytes.Length}\n");

			self.AddTemp(out TreeDataByteSequence sequenceRead).SetBytes(bytes);
			ADataBase aDataBase2 = new AData();
			sequenceRead.Deserialize(ref aDataBase2);
			AData data2 = (AData)aDataBase2;

			string logText = $"\n反序列化{data2.AInt} \n";

			logText += $"\n数组数量{data2.Ints.Length} :\n";
			foreach (var item in data2.Ints)
			{
				logText += $"数组长度{item.Length} :";

				foreach (var item1 in item)
				{
					logText += $"{item1} ";
				}
				//logText += $"{item} ";
			}
			logText += $"\n字典数量{data2.DataDict.Count} :\n";
			foreach (var item in data2.DataDict)
			{
				logText += $"[{item.Key}: {item.Value}],";
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