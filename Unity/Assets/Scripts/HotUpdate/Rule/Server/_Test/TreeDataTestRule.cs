

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
			//int value1 = nameof(self).GetFNV1aHash32();
			//self.Log(value1 + " :: " + Value);

			//if (self != null) return;

			AData data = self.AddComponent(out AData _);

			data.AddComponent(out TreeDataNode1 dataNode1);

			dataNode1.BInt = 789.4f;

			data.AInt = 401.5f;
			data.Ints = new int[][,,]{
				new int[2,2,5]{ { { 1, 2, 30, 4, 5 }, { 20, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }, { 2, 5, 9, 5, 2 } } },
				new int[2,1,5]{ { { 1220, 45, 90, 75, 23 } }, { { 1, 23, 360, 84, 5 }} },
			};

			data.DataDict = new UnitDictionary<int, string>()
			{
				{ 1, "1.1f测" },
				{ 2, "2.2f测试" },
				{ 3, "3.3f" },
				{ 4, "4.4f" },
				{ 5, "5.5f"},
			};

			AData aDataBase = data;
			self.AddTemp(out TreeDataByteSequence sequenceWrite).Serialize(aDataBase);

			self.RemoveComponent<TreeDataTest, AData>();

			byte[] bytes = sequenceWrite.ToBytes();

			self.Log($"序列化字节长度{bytes.Length}\n");

			self.AddTemp(out TreeDataByteSequence sequenceRead).SetBytes(bytes);
			AData aDataBase2 = self.AddComponent(out AData _);
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
			if (data2.DataDict != null)
			{
				logText += $"\n字典数量{data2.DataDict.Count} :\n";
				foreach (var item in data2.DataDict)
				{
					logText += $"[{item.Key}: {item.Value}],";
				}
			}
			self.Log(logText);
		};

	}


}