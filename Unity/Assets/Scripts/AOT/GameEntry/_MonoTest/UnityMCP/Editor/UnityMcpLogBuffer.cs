/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/16
* 描述： Unity 控制台日志的常驻环形缓冲

* 供 unity_logs 工具读取 —— 因为是常驻的，所以能拿到"调用之前"的日志。
* 限制：存在静态字段里，**域重载会清空**（每次重编译后缓冲就空了）。

****************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMCP.Editor
{
	/// <summary>
	/// 日志环形缓冲
	/// </summary>
	public static class UnityMcpLogBuffer
	{
		/// <summary>
		/// 保留条数上限
		/// </summary>
		public const int Max = 500;

		/// <summary>
		/// 一条日志
		/// </summary>
		public class Item
		{
			public LogType Type;
			public string Message;
			public string Stack;
			public DateTime Time;
		}

		private static readonly object gate = new object();
		private static readonly Queue<Item> ring = new Queue<Item>();

		/// <summary>
		/// 挂上日志监听（由服务器在静态构造里调用一次；域重载后会重新挂）
		/// </summary>
		public static void Install()
		{
			Application.logMessageReceived += OnLog;
		}

		private static void OnLog(string message, string stack, LogType type)
		{
			lock (gate)
			{
				ring.Enqueue(new Item { Type = type, Message = message, Stack = stack, Time = DateTime.Now });
				while (ring.Count > Max) ring.Dequeue();
			}
		}

		/// <summary>
		/// 取一份快照
		/// </summary>
		public static List<Item> Snapshot()
		{
			lock (gate) { return new List<Item>(ring); }
		}

		/// <summary>
		/// 清空
		/// </summary>
		public static void Clear()
		{
			lock (gate) { ring.Clear(); }
		}
	}
}
