

using System;

namespace WorldTree
{

	public static partial class TreeDataTestRule
	{
		static OnAdd<TreeDataTest> OnAdd = (self) =>
		{
			AData data = new AData();

			data.AInt = 401;

			self.AddTemp(out TreeDataByteSequence sequenceWrite).Serialize(data);

			byte[] bytes = sequenceWrite.ToBytes();

			self.Log($"序列化字节长度{bytes.Length}");

			self.AddTemp(out TreeDataByteSequence sequenceRead).SetBytes(bytes);

			AData data2 = null;
			sequenceRead.Deserialize(ref data2);

			string logText = $"反序列化{data2.AInt} ";

			self.Log(logText);
		};
	}


}