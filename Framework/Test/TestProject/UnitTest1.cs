using WorldTree;

namespace TestProject
{
	public class UnitTest1 : IDisposable
	{
		public long num = 4;

		public UnitTest1()
		{
			num = 1;
		}


		[Fact]
		public void Test0()
		{
			Assert.Equal(0, MathBit.GetHighestBitIndex(num));
		}

		[Theory]
		[InlineData(1ul)]
		[InlineData(10ul)]
		[InlineData(20ul)]
		public void Test1(ulong num)
		{
			MathBit.LeadingZeroCount(num);
		}


		public void Dispose()
		{
		}
	}
}