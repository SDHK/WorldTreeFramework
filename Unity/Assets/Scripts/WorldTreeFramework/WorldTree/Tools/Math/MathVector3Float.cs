/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 3d向量
	/// </summary>
	public static class MathVector3Float
	{
		/// <summary>
		/// 返回最小值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Float Min(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y), Math.Min(lhs.Z, rhs.Z));

		/// <summary>
		/// 返回最大值
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Float Max(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y), Math.Max(lhs.Z, rhs.Z));

		/// <summary>
		/// 返回a和b之间的距离。</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(this Vector3Float a, Vector3Float b) => (a - b).Magnitude;


		/// <summary>
		/// 返回vector的副本，其大小限制为maxLength。</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Float ClampMagnitude(this Vector3Float vector, float maxLength)
		{
			float sqrMagnitude = vector.SqrMagnitude;
			if ((double)sqrMagnitude <= (double)maxLength * (double)maxLength)
				return vector;
			float num1 = (float)Math.Sqrt((double)sqrMagnitude);
			float num2 = vector.X / num1;
			float num3 = vector.Y / num1;
			float num4 = vector.Z / num1;
			return new Vector3Float(num2 * maxLength, num3 * maxLength, num4 * maxLength);
		}


		/// <summary>
		/// 将向量反射出由法线定义的平面。
		/// </summary>
		/// <param name="inDirection">入射向量</param>
		/// <param name="inNormal">平面法线向量</param>
		/// <returns>反射向量</returns>
		public static Vector3Float Reflect(this Vector3Float inDirection, Vector3Float inNormal)
		{
			float num = -2f * inNormal.Dot(inDirection);
			return new Vector3Float(num * inNormal.X + inDirection.X, num * inNormal.Y + inDirection.Y, num * inNormal.Z + inDirection.Z);
		}

		/// <summary>
		/// 两个向量的点积。
		/// </summary>
		public static float Dot(this Vector3Float lhs, Vector3Float rhs) => (float)((double)lhs.X * (double)rhs.X + (double)lhs.Y * (double)rhs.Y + (double)lhs.Z * (double)rhs.Z);

		/// <summary>
		/// 两个向量的外积。
		/// </summary>
		public static Vector3Float Cross(this Vector3Float lhs, Vector3Float rhs) => new Vector3Float((float)((double)lhs.Y * (double)rhs.Z - (double)lhs.Z * (double)rhs.Y), (float)((double)lhs.Z * (double)rhs.X - (double)lhs.X * (double)rhs.Z), (float)((double)lhs.X * (double)rhs.Y - (double)lhs.Y * (double)rhs.X));


		/// <summary>
		/// 将一个向量投影到另一个向量上。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Float Project(this Vector3Float vector, Vector3Float onNormal)
		{
			float num1 = onNormal.Dot(onNormal);
			if (num1 < Vector3Float.K_EPSILON)
				return Vector3Float.Zero;
			float num2 = vector.Dot(onNormal);
			return new Vector3Float(onNormal.X * num2 / num1, onNormal.Y * num2 / num1, onNormal.Z * num2 / num1);
		}

		/// <summary>
		/// 将一个向量投影到一个平面上，这个平面由一个正交于平面的法线定义。
		/// </summary>
		/// <param name="vector">向量在平面上的位置。</param>
		/// <param name="planeNormal">向量指向平面的方向</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Float ProjectOnPlane(this Vector3Float vector, Vector3Float planeNormal)
		{
			float num1 = planeNormal.Dot(planeNormal);
			if (num1 < MathFloat.Epsilon)
				return vector;
			float num2 = vector.Dot(planeNormal);
			return new Vector3Float(vector.X - planeNormal.X * num2 / num1, vector.Y - planeNormal.Y * num2 / num1, vector.Z - planeNormal.Z * num2 / num1);
		}

		/// <summary>
		/// 计算和向量之间的夹角。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(this Vector3Float from, Vector3Float to)
		{
			float num = (float)Math.Sqrt((double)from.SqrMagnitude * (double)to.SqrMagnitude);
			return (double)num < 1.0000000036274937E-15 ? 0.0f : (float)Math.Acos((double)Math.Clamp(from.Dot(to) / num, -1f, 1f)) * 57.29578f;
		}

		/// <summary>
		/// 计算向量from和to之间相对于轴的带符号角度。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignedAngle(this Vector3Float from, Vector3Float to, Vector3Float axis)
		{
			float num1 = from.Angle(to);
			float num2 = (float)((double)from.Y * (double)to.Z - (double)from.Z * (double)to.Y);
			float num3 = (float)((double)from.Z * (double)to.X - (double)from.X * (double)to.Z);
			float num4 = (float)((double)from.X * (double)to.Y - (double)from.Y * (double)to.X);
			float num5 = Math.Sign((float)((double)axis.X * (double)num2 + (double)axis.Y * (double)num3 + (double)axis.Z * (double)num4));
			return num1 * num5;
		}


		//======================================


		/// <summary>
		/// 判断是否平行线
		/// </summary>
		public static bool TryParallel(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d)
		{
			return (b - a).Cross(d - c) == Vector3Float.Zero;
		}

		/// <summary>
		/// 判断点在线上
		/// </summary>
		public static bool TryPointOnLine(Vector3Float node1, Vector3Float node2, Vector3Float point)
		{
			float maxX = node1.X > node2.X ? node1.X : node2.X;
			float minX = node1.X > node2.X ? node2.X : node1.X;
			float maxY = node1.Y > node2.Y ? node1.Y : node2.Y;
			float minY = node1.Y > node2.Y ? node2.Y : node1.Y;
			float maxZ = node1.Z > node2.Z ? node1.Z : node2.Z;
			float minZ = node1.Z > node2.Z ? node2.Z : node1.Z;

			Vector3Float vector1_P = point - node1;
			Vector3Float vector1_2 = node2 - node1;
			return vector1_P.Dot(vector1_2) - vector1_2.Dot(vector1_P) == 0 &&
					(point.X >= minX && point.X <= maxX) &&
					(point.Y >= minY && point.Y <= maxY) &&
					(point.Z >= minZ && point.Z <= maxZ);
		}


		/// <summary>
		/// 快速排斥：true有可能相交，false 为排斥
		/// </summary>
		public static bool RapidRepulsion(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d)
		{
			return Math.Min(a.X, b.X) <= Math.Max(c.X, d.X) ?
					Math.Min(c.X, d.X) <= Math.Max(a.X, b.X) ?
					Math.Min(a.Y, b.Y) <= Math.Max(c.Y, d.Y) ?
					Math.Min(c.Y, d.Y) <= Math.Max(a.Y, b.Y) ?
					Math.Min(a.Z, b.Z) <= Math.Max(c.Z, d.Z) ?
					Math.Min(c.Z, d.Z) <= Math.Max(a.Z, b.Z) : false : false : false : false : false;
		}

		/// <summary>
		/// 跨立实验:只判断交叉，重叠点或线不算
		/// </summary>
		public static bool CrossExperiment(Vector3Float a, Vector3Float b, Vector3Float c, Vector3Float d, out Vector3Float intersectPosition)
		{
			intersectPosition = Vector3Float.Zero;
			Vector3Float ab = b - a;
			Vector3Float ca = a - c;
			Vector3Float cd = d - c;
			Vector3Float ad = d - a;
			Vector3Float cb = b - c;
			// 判断 三角形acb法线 与 三角形abd法线 方向一致 和 三角形cad法线 与 三角形cbd法线 方向一致
			if (-ca.Cross(ab).Dot(ab.Cross(ad)) > 0 && ca.Cross(cd).Dot(cd.Cross(cb)) > 0)
			{
				Vector3Float v1 = ca.Cross(cd);
				Vector3Float v2 = cd.Cross(ab);

				float ratio = v1.Dot(v2) / v2.SqrMagnitude;//获得比值
				intersectPosition = a + ab * ratio;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 在两个向量之间进行线性插值。
		/// </summary>
		public static Vector3Float Lerp(this Vector3Float a, Vector3Float b, float t)
		{
			t = Math.Clamp(t, 0, 1);
			return new Vector3Float(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
		}

		/// <summary>
		/// 贝塞尔曲线
		/// </summary>
		/// <param name="timeRatio">点的位置（时间比例0~1）</param>
		/// <param name="points">曲线拉伸坐标点</param>
		/// <returns> 点的位置 </returns>
		public static Vector3Float BezierCurve(TreeList<Vector3Float> points, float timeRatio)
		{
			while (points.Count > 1)
			{
				points.Parent.AddTemp(out TreeList<Vector3Float> newp);
				for (int i = 0; i < points.Count - 1; i++)
				{
					Vector3Float p0p1 = points[i].Lerp(points[i + 1], timeRatio);
					newp.Add(p0p1);
				}
				points.Dispose();
				points = newp;
			}
			var point = points[0];
			points.Dispose();
			return point;
		}

		/// <summary>
		/// 计算多点中心
		/// </summary>
		/// <param name="points">多点位置集合</param>
		/// <returns>中心点</returns>
		public static Vector3Float Center(TreeList<Vector3Float> points)
		{
			Vector3Float center = new Vector3Float();
			for (int i = 0; i < points.Count; i++) center += points[i];
			return center / points.Count;
		}


		/// <summary>
		/// 获取向量绕某轴旋转x度后的位置（三维）
		/// </summary>
		/// <param name="vector">要旋转的向量</param>
		/// <param name="center">旋转的中心点</param>
		/// <param name="axis">旋转的轴</param>
		/// <param name="angle">要旋转角度（顺时针为正）</param>
		/// <returns>旋转后的位置</returns>
		public static Vector3Float RotateRound(Vector3Float vector, Vector3Float center, Vector3Float axis, float angle)
		{
			Vector3Float point = QuaternionFloat.AngleAxis(angle, axis) * vector; //算出旋转后的向量
			Vector3Float resultVec3 = center + point;        //加上旋转中心位置得到旋转后的位置
			return resultVec3;
		}

		/// <summary>
		/// 三维向量转换为欧拉角：向量不能为0
		/// </summary>
		/// <param name="vector">指向向量</param>
		/// <returns>return : 欧拉角(360度)</returns>
		public static Vector3Float ToEulerAngle(this Vector3Float vector)
		{
			return QuaternionFloat.LookRotation(vector).EulerAngles;
		}

		/// <summary>
		/// 欧拉角转换为三维向量
		/// </summary>
		/// <param name="eulerAngles">欧拉角</param>
		/// <returns>转换的三维向量</returns>
		public static Vector3Float ToVector3(this Vector3Float eulerAngles)
		{
			Vector3Float vector = new Vector3Float(0, 0, 1);
			vector = QuaternionFloat.AngleAxis(eulerAngles.X, -Vector3Float.Left) * vector; //算出旋转后的向量
			vector = QuaternionFloat.AngleAxis(eulerAngles.Y, Vector3Float.Up) * vector; //算出旋转后的向量

			return vector;
		}
	}
}
