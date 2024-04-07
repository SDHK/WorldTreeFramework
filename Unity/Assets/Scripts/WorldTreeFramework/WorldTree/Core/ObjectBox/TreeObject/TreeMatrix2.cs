/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/13 19:57

* 描述： 二维矩阵

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 二维矩阵
	/// </summary>
	public class TreeMatrix2<T> : Node, ChildOf<INode>
		, AsRule<IAwakeRule<int, int>>

	{
		/// <summary>
		/// x轴长度
		/// </summary>
		public int xLength;
		/// <summary>
		/// y轴长度
		/// </summary>
		public int yLength;

		public TreeArray<INode> m_Array;



		public T this[int x, int y]
		{
			get
			{
				if (x < xLength && y < yLength)
				{
					m_Array[x] ??= this.AddChild(out TreeArray<T> _, yLength);
					return ((TreeArray<T>)m_Array[x])[y];
				}
				else
				{
					this.LogError("下标溢出");
					return default(T);
				}
			}
			set
			{
				if (x < xLength && y < yLength)
				{
					m_Array[x] ??= this.AddChild(out TreeArray<T> _, yLength);
					((TreeArray<T>)m_Array[x])[y] = value;
				}
				else
				{
					this.LogError("下标溢出");
				}
			}
		}
	}
	public class TreeMatrix2AwakeRule<T> : AwakeRule<TreeMatrix2<T>, int, int>
	{
		protected override void Execute(TreeMatrix2<T> self, int xLength, int yLength)
		{
			self.xLength = xLength;
			self.yLength = yLength;
			self.AddChild(out self.m_Array, xLength);
		}
	}

	class TreeMatrix2RemoveRule<T> : RemoveRule<TreeMatrix2<T>>
	{
		protected override void Execute(TreeMatrix2<T> self)
		{
			self.xLength = 0;
			self.yLength = 0;
			self.m_Array = null;
		}
	}

	public static class TreeMatrix2Rule
	{
		/// <summary>
		/// 点积计算
		/// </summary>
		public static TreeMatrix2<double> Dot(this TreeMatrix2<double> matrixA, TreeMatrix2<double> matrixB)
		{
			int aRows = matrixA.xLength;//行数
			int aCols = matrixA.yLength;//列数

			int bRows = matrixB.xLength;//行数
			int bCols = matrixB.yLength;//列数

			if (aCols != bRows)
				throw new Exception("不能做矩阵乘法。不正确的尺寸。");

			matrixA.Parent.AddChild(out TreeMatrix2<double> result, aRows, bCols);

			for (int i = 0; i < aRows; i++)
				for (int j = 0; j < bCols; j++)
					for (int k = 0; k < aCols; k++)
						result[i, j] += (matrixA[i, k] * matrixB[k, j]);

			return result;
		}

		/// <summary>
		/// 乘法
		/// </summary>
		public static TreeMatrix2<double> Multiplication(this TreeMatrix2<double> matrixA, TreeMatrix2<double> matrixB)
		{
			if (matrixA.xLength != matrixB.xLength || matrixA.yLength != matrixB.yLength)
			{
				matrixA.LogError("矩阵大小不相等");
				return null;
			}
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = matrixA[i, j] * matrixB[i, j];
			return result;
		}

		/// <summary>
		/// 指数运算
		/// </summary>
		public static TreeMatrix2<double> Pow(this TreeMatrix2<double> matrixA, double value)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = (double)Math.Pow(matrixA[i, j], value);
			return result;
		}


		/// <summary>
		/// 加法
		/// </summary>
		public static TreeMatrix2<double> Additive(this TreeMatrix2<double> matrixA, double value)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = matrixA[i, j] + value;
			return result;
		}
		/// <summary>
		/// 加法
		/// </summary>
		public static void Additive1(this TreeMatrix2<double> matrixA, TreeMatrix2<double> matrixB)
		{
			if (matrixA.xLength != matrixB.xLength || matrixA.yLength != matrixB.yLength)
			{
				matrixA.LogError("矩阵大小不相等");
			}
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					matrixA[i, j] += matrixB[i, j];
		}


		/// <summary>
		/// 减法
		/// </summary>
		public static TreeMatrix2<double> Subtraction(this TreeMatrix2<double> matrixA, TreeMatrix2<double> matrixB)
		{
			if (matrixA.xLength != matrixB.xLength || matrixA.yLength != matrixB.yLength)
			{
				matrixA.LogError("矩阵大小不相等");
				return null;
			}
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = matrixA[i, j] - matrixB[i, j];
			return result;
		}


		/// <summary>
		/// 减法
		/// </summary>
		public static TreeMatrix2<double> Subtraction_(this double value, TreeMatrix2<double> matrixA)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = value - matrixA[i, j];
			return result;
		}

		/// <summary>
		/// 除法运算
		/// </summary>
		public static TreeMatrix2<double> Division(this TreeMatrix2<double> matrixA, double value)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = matrixA[i, j] / value;
			return result;
		}


		/// <summary>
		/// 除法运算
		/// </summary>
		public static TreeMatrix2<double> Division(this double value, TreeMatrix2<double> matrixA)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = value / matrixA[i, j];
			return result;
		}

		/// <summary>
		/// 指数次幂
		/// </summary>
		public static TreeMatrix2<double> Exp(this TreeMatrix2<double> matrixA)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = (double)Math.Exp(matrixA[i, j]);
			return result;
		}

		/// <summary>
		/// 指数次幂 负
		/// </summary>
		public static TreeMatrix2<double> Exp_(this TreeMatrix2<double> matrixA)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.xLength, matrixA.yLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[i, j] = Math.Exp(-matrixA[i, j]);
			return result;
		}

		/// <summary>
		/// 扭转
		/// </summary>
		public static TreeMatrix2<double> Turn(this TreeMatrix2<double> matrixA)
		{
			matrixA.Parent.AddChild(out TreeMatrix2<double> result, matrixA.yLength, matrixA.xLength);
			for (int i = 0; i < matrixA.xLength; i++)
				for (int j = 0; j < matrixA.yLength; j++)
					result[j, i] = matrixA[i, j];
			return result;
		}


		/// <summary>
		/// 打印
		/// </summary>
		public static void Print(this TreeMatrix2<double> matrix)
		{
			string t = "";
			for (int i = 0; i < matrix.xLength; i++)
			{
				for (int i1 = 0; i1 < matrix.yLength; i1++)
				{
					t += matrix[i, i1] + "\t";
				}
				t += " \n";
			}
			matrix.Log(t);
		}


	}
}
