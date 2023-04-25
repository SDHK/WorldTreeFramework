
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/15 15:48

* 描述： 

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WorldTree
{
    /// <summary>
    /// Tile坐标计算工具：使用前先设置Tile尺寸
    /// </summary>
    public static class TileTool
    {
        /// <summary>
        /// Tile比例
        /// </summary>
        public static Vector2 CellSize => cellSize;

        /// <summary>
        /// 原点世界坐标
        /// </summary>
        public static Vector3 Origin = Vector3.zero;


        private static Vector2 cellSize;
        private static float cellEdge;

        private static Triangle triangle;

        /// <summary>
        /// 设置Tile固定尺寸：（解三角计算）
        /// </summary>
        public static void SetCellSize(Vector2 CellSize)
        {
            cellSize = CellSize;
            if (triangle == null)
            {
                triangle = new Triangle();
            }
            triangle.SolutionsFromEAE(CellSize.x, 90, CellSize.y);
            cellEdge = triangle.EdgeB * 0.5f;
        }

        /// <summary>
        /// Tile转世界坐标
        /// </summary>
        public static Vector2 TileToWorld(Vector2Int point)
        {
            float x = point.x;
            float y = point.y;
            float edge = cellEdge;
            float edgeA = triangle.EdgeA;
            float edgeB = triangle.EdgeB;
            float edgeC = triangle.EdgeC;

            //y三角形斜边
            float yTriangleEdge = y * edge;

            //x三角形斜边
            float xTriangleEdge = x * edge - yTriangleEdge;

            //x三角形比例
            float xRatio = xTriangleEdge / edgeB;

            //y三角形2倍比例
            float yRatio = yTriangleEdge / edge;

            //世界X轴坐标 = 底边 * x三角形比例
            float worldX = edgeA * xRatio;

            //世界Y轴坐标 = x三角形高度 - y三角形2倍高度
            float worldY = edgeC * xRatio + (edgeC * yRatio);

            return new Vector2(worldX + Origin.x, worldY + Origin.y);
        }

        /// <summary>
        /// 世界坐标转Tile
        /// </summary>
        public static Vector2Int WorldToTile(Vector2 point)
        {
            float x = point.x - Origin.x;
            float y = point.y - Origin.y;
            float edge = cellEdge;
            float edgeA = triangle.EdgeA;
            float edgeB = triangle.EdgeB;
            float edgeC = triangle.EdgeC;

            //x三角形比例
            float xRatio = x / edgeA;

            //x三角形高度
            float xTriangleHigh = edgeC * xRatio;

            //x三角形斜边
            float xTriangleEdge = edgeB * xRatio;

            //y三角形高度
            float yTriangleHigh = (y - xTriangleHigh) * 0.5f;

            //y三角形比例
            float yRatio = yTriangleHigh / edgeC;

            //y三角形斜边 ， Tile的Y轴长度
            float yTriangleEdge = edgeB * yRatio;

            //Tile的X轴长度
            float tileX = xTriangleEdge + yTriangleEdge;

            //四舍五入算下标
            int tileIndexX = Mathf.RoundToInt(tileX / edge);
            int tileIndexY = Mathf.RoundToInt(yTriangleEdge / edge);

            return new Vector2Int(tileIndexX, tileIndexY);
        }

        #region mesh

        /// <summary>
        /// tile坐标转mesh坐标
        /// </summary>
        public static Vector2Int TileToMesh(Vector2Int point)
        {
            return TileToMesh(point.x, point.y);
        }

        /// <summary>
        /// tile坐标转mesh坐标
        /// </summary>
        public static Vector2Int TileToMesh(int x, int y)
        {
            return new Vector2Int(x - y, x + y);
        }

        /// <summary>
        /// mesh坐标转tile坐标
        /// </summary>
        public static Vector2Int MeshToTile(Vector2Int point)
        {
            return MeshToTile(point.x, point.y);
        }

        /// <summary>
        /// mesh坐标转tile坐标
        /// </summary>
        public static Vector2Int MeshToTile(int x, int y)
        {
            int draw = (int)((y - x) * 0.5f);
            return new Vector2Int(x + draw, draw);
        }

        /// <summary>
        /// 世界坐标转mesh坐标
        /// </summary>
        public static Vector2Int WorldToMesh(Vector2 point)
        {
            return TileToMesh(WorldToTile(point));
        }

        /// <summary>
        /// mesh坐标转世界坐标
        /// </summary>
        public static Vector2 MeshToWorld(Vector2Int point)
        {
            return TileToWorld(MeshToTile(point));
        }

        /// <summary>
        /// 通过Tile坐标获取mash数组下标，未测试
        /// </summary>
        public static int TileToMashArray(Vector2Int point, int height)
        {
            int value = point.x - point.y;
            return (height / 2 + 1) * value + point.y + value;
        }

        #endregion
    }
}
