
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/16 15:52

* 描述： 2D移动区域限制器,线段数必须大于2

*/

using System.Collections.Generic;
using UnityEngine;
using WorldTree;

/// <summary>
/// 2D移动区域限制器
/// </summary>
public class MoveAreaLimiter
{
    public List<Vector2> linePoints;

    private Vector2 Last;

    public float border = 0.01f;

    private bool isRefresh = false;

    public void SetPoints(List<Vector2> points)
    {
        if (points.Count > 2)
        {
            linePoints = points;
            Last = points[0];
            Refresh();
        }
    }
    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh()
    {
        isRefresh = true;
    }

    /// <summary>
    /// 移动限制计算
    /// </summary>
    public Vector2 Limit(Vector2 Current)
    {
        if (linePoints == null) return Current;
        if (linePoints.Count < 3) return Current;

        if (isRefresh)
        {
            Last = Current;
            isRefresh = false;
        }

        if (TryCollisionLines(linePoints, Current, Last, out int IntersectIndex, out Vector2 intersectPosition))
        {
            Last = GetFulcrum(Current, Last,
                  linePoints[(IntersectIndex - 1).Loop(linePoints.Count)],
                  linePoints[IntersectIndex],
                  linePoints[(IntersectIndex + 1).Loop(linePoints.Count)],
                  linePoints[(IntersectIndex + 2).Loop(linePoints.Count)]);

            return intersectPosition;
        }
        else
        {
            if ((Last - Current).sqrMagnitude > 0.1f) Last = Current;
            return Current;
        }
    }

    /// <summary>
    /// 阻拦支点计算
    /// </summary>
    private Vector2 GetFulcrum(Vector2 Current, Vector2 Last, Vector2 node0, Vector2 node1, Vector2 node2, Vector2 node3)
    {
        Vector2 VectorNode1_2 = node2 - node1;
        Vector2 VectorNode0_1 = node1 - node0;
        Vector2 VectorNode2_3 = node3 - node2;

        Vector2 VectorNode1_Last = Last - node1;
        //获取上一帧位置在线上的投影位置，原因是上一帧并没有进入，点不会出现在线上
        Vector2 LastProjectPosition = VectorNode1_Last.Project(VectorNode1_2) + node1;

        //获取上一帧垂直于线1-2的法线向量 并乘以 边界大小
        Vector2 VectorProject_Last = (Last - LastProjectPosition).normalized ;

        Vector2 LimitPosition1 = GetLimitBorder2(VectorNode1_2, VectorNode0_1, node1, VectorProject_Last);
        Vector2 LimitPosition2 = GetLimitBorder2(VectorNode1_2, VectorNode2_3, node2, VectorProject_Last);
        Vector2 VectorLimitPosition1_2 = LimitPosition2 - LimitPosition1;//获得限制线向量

        //点再次投影到限制线上
        Vector2 VectorLimit1_Project = (VectorProject_Last * border + Current - LimitPosition1).Project(VectorLimitPosition1_2);
        Vector2 LimitProjectPosition = VectorLimit1_Project + LimitPosition1;


        float maxX, minX, maxY, minY;
        maxX = LimitPosition1.x > LimitPosition2.x ? LimitPosition1.x : LimitPosition2.x;
        minX = LimitPosition1.x > LimitPosition2.x ? LimitPosition2.x : LimitPosition1.x;
        maxY = LimitPosition1.y > LimitPosition2.y ? LimitPosition1.y : LimitPosition2.y;
        minY = LimitPosition1.y > LimitPosition2.y ? LimitPosition2.y : LimitPosition1.y;

        bool LastInLine = (LimitProjectPosition.x >= minX && LimitProjectPosition.x <= maxX) && (LimitProjectPosition.y >= minY && LimitProjectPosition.y <= maxY);

        bool LastInAngle = Vector2.Dot(VectorLimitPosition1_2, VectorLimit1_Project) >= 0;

        //Debug.DrawLine(LimitPosition1.ToXZ(), LimitPosition2.ToXZ(), Color.cyan);
        //Debug.DrawLine(node1.ToXZ(), LimitPosition1.ToXZ(), Color.red);
        //Debug.DrawLine(node2.ToXZ(), LimitPosition2.ToXZ(), Color.blue);
        //Debug.DrawLine(Last.ToXZ(), Current.ToXZ(), Color.gray);

        if (LastInLine)//在线段内
        {
            return LimitProjectPosition;
        }
        else if (LastInAngle) //越过节点2
        {
            return LimitPosition2;
        }
        else//越过节点1
        {
            return LimitPosition1;
        }
    }

    /// <summary>
    /// 尝试获取最近碰撞线段的下标和交点
    /// </summary>
    private bool TryCollisionLines(List<Vector2> LinePoints, Vector2 current, Vector2 last, out int IntersectIndex, out Vector2 intersectPosition)
    {
        float distance = float.MaxValue;
        float angle = float.MaxValue;

        bool isIntersect = false;
        IntersectIndex = 0;
        intersectPosition = Vector2.zero;

        Vector2 node1, node2;
        Vector2 Vector1_2, VectorL_I;

        for (int i = 0; i < LinePoints.Count; i++)
        {
            node1 = LinePoints[i];
            node2 = (i == LinePoints.Count - 1) ? LinePoints[0] : LinePoints[i + 1];

            if (TryCollisionLine(node1, node2, last, current, out Vector2 intersectPos, out bool isLineReverse))//当前线段发生碰撞
            {
                Vector1_2 = node2 - node1;
                VectorL_I = intersectPos - last;

                //翻转线段向量
                if (isLineReverse) Vector1_2 = -Vector1_2;

                float sqrMagnitude = VectorL_I.sqrMagnitude;
                if (sqrMagnitude < distance)//交点长度比较
                {
                    distance = sqrMagnitude;
                    float dot = Vector2.Dot(VectorL_I, Vector1_2);
                    angle = dot;
                    IntersectIndex = i;
                    intersectPosition = intersectPos;

                }
                else if (sqrMagnitude == distance)
                {
                    float dot = Vector2.Dot(VectorL_I, Vector1_2);

                    if (dot < angle)//夹角比较
                    {
                        angle = dot;
                        distance = sqrMagnitude;
                        IntersectIndex = i;
                        intersectPosition = intersectPos;
                    }
                }
                isIntersect = true;
            }
        }
        return isIntersect;
    }

    /// <summary>
    /// 判断碰撞发生并获取交点
    /// </summary>
    private bool TryCollisionLine(Vector2 node1, Vector2 node2, Vector2 last, Vector2 current, out Vector2 intersectPosition, out bool isLineReverse)
    {
        intersectPosition = current;
        isLineReverse = false;

        return !MathVector2.TryParallel(node1, node2, last, current) ?//判断 不是平行线
                TryPointCollisionExtremeCase(node1, node2, current, out isLineReverse) ? true ://端点 是否 重叠
                MathVector2.RapidRepulsion(node1, node2, last, current) ?//判断 快速排斥
                TryLineCollisionExtremeCase(node1, node2, last, current, out intersectPosition, out isLineReverse) ? true ://点 是否 在线上
                MathVector2.CrossExperiment(node1, node2, last, current, out intersectPosition)//两线 是否 交叉
                : false : false;
    }


    /// <summary>
    /// 判断点与点碰撞的极端情况
    /// </summary>
    /// <param name="isReverse">是否需要反转线段</param>
    /// <returns>发生点重叠极端碰撞</returns>
    private bool TryPointCollisionExtremeCase(Vector2 node1, Vector2 node2, Vector2 current, out bool isReverse)
    {
        isReverse = false;

        if (node2 == current)
        {
            isReverse = true;
            return true;
        }
        else if (node1 == current)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断点与线碰撞的极端情况 
    /// </summary>
    /// <param name="isReverse">是否需要反转线段</param>
    /// <returns>发生点在线上极端碰撞</returns>
    private bool TryLineCollisionExtremeCase(Vector2 node1, Vector2 node2, Vector2 last, Vector2 current, out Vector2 intersectPosition, out bool isReverse)
    {
        isReverse = false;
        intersectPosition = current;
        if (MathVector2.TryPointOnLine(last, current, node2))
        {
            isReverse = true;
            intersectPosition = node2;
            return true;
        }
        else if (MathVector2.TryPointOnLine(last, current, node1))
        {
            intersectPosition = node1;

            return true;
        }
        else if (MathVector2.TryPointOnLine(node1, node2, current))
        {
            intersectPosition = current;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得限制边缘
    /// </summary>
    private Vector2 GetLimitBorder(Vector2 VectorEdgeA_B, Vector2 VectorEdgeB_C, Vector2 NodeB, Vector2 vector)
    {
        float AngleB = Vector3.Angle(VectorEdgeA_B, VectorEdgeB_C);
        float TriangleNormal = VectorEdgeA_B.Cross(VectorEdgeB_C);//三角形abc平面法线向量
        return MathVector2.RotateRound(vector, NodeB, (TriangleNormal >= 0 ? AngleB : -AngleB) * 0.5f);
    }


    /// <summary>
    /// 获得限制边缘
    /// </summary>
    private Vector2 GetLimitBorder2(Vector2 VectorEdgeA_B, Vector2 VectorEdgeB_C, Vector2 NodeB, Vector2 vector)
    {
        float AngleB = Vector3.Angle(VectorEdgeA_B, VectorEdgeB_C);
        float TriangleNormal = VectorEdgeA_B.Cross(VectorEdgeB_C);//三角形abc平面法线向量
        float angle = AngleB * 0.5f;
        return MathVector2.RotateRound(vector * MathTriangle.GetDiameter( 90 - angle, border), NodeB, (TriangleNormal >= 0 ? angle : -angle));
    }
}
