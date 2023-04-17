/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/10 15:41

* 描述： 神经网络：感知机单元

*/

using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 感知机单元
    /// </summary>
    public class Perceptron : Node
    {
        private float Bias;//偏置值,将多次同时训练变成单次训练的值累计


        /// <summary>
        /// 输入
        /// </summary>
        public List<float> Inputs;

        /// <summary>
        /// 权重
        /// </summary>
        public List<float> weight;


        public float[][] Dot(float[][] matrix1, float[][] matrix2)
        {
            return new float[0][];
        }



        public static float[,] Dot(float[,] matrixA, float[,] matrixB)
        {
            int aRows = matrixA.GetLength(0);//行数
            int aCols = matrixA.GetLength(1);//列数

            int bRows = matrixB.GetLength(0);//行数
            int bCols = matrixB.GetLength(1);//列数

            if (aCols != bRows)
                throw new Exception("不能做矩阵乘法。不正确的尺寸。");

            float[,] result = new float[aRows, bCols];

            for (int i = 0; i < aRows; i++)
                for (int j = 0; j < bCols; j++)
                    for (int k = 0; k < aCols; k++)
                        result[i, j] += matrixA[i, k] * matrixB[k, j];

            return result;
        }

        public static TreeList<TreeList<float>> Dot(TreeList<TreeList<float>> matrixA, TreeList<TreeList<float>> matrixB)
        {
            int aRows = matrixA[0].Count;//行数
            int aCols = matrixA[1].Count;//列数

            int bRows = matrixB[0].Count;//行数
            int bCols = matrixB[1].Count;//列数

            if (aCols != bRows)
                throw new Exception("不能做矩阵乘法。不正确的尺寸。");



            return null;
        }

    }
}
