using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace ECSTest
{
	/// <summary>
	/// 线程参数
	/// </summary>
	public class ThreadArg
	{
		/// <summary>
		/// 要放到主线程中执行的方法
		/// </summary>
		public Action<object> Action { get; set; }
		/// <summary>
		/// 方法执行时额外的参数
		/// </summary>
		public object State { get; set; }
		/// <summary>
		/// 是否同步执行
		/// </summary>
		public bool Sync { get; set; }
	}

	/// <summary>
	/// 线程队列控制
	/// </summary>
	public class MyControl
	{
		/// <summary>
		/// 线程处理队列
		/// </summary>
		public BlockingCollection<ThreadArg> blockingCollection = new BlockingCollection<ThreadArg>();

		//记录创建这个控件的线程
		private Thread _createThread = null;

		public MyControl()
		{
			//第一个控件创建时初始化一个SynchronizationContext实例，并将它和当前线程绑定一起
			_createThread = Thread.CurrentThread;
		}

		/// <summary>
		/// 同步调用,让当前线程等待
		/// </summary>
		public void Invoke(Action<object> action, object state)
		{
			BlockingCollection<ThreadArg> queues = null;
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			queues.Add(new ThreadArg()
			{
				Action = obj =>
				{
					action(state);
					manualResetEvent.Set();
				},
				State = state,
				Sync = true
			});
			manualResetEvent.WaitOne();
			manualResetEvent.Dispose();
		}

		/// <summary>
		/// 异步调用
		/// </summary>
		public void BeginInvoke(Action<object> action, object state)
		{
			BlockingCollection<ThreadArg> queues = null;
			queues.Add(new ThreadArg()
			{
				Action = action,
				State = state
			});
		}
	}

	/// <summary>
	/// 同步上下文
	/// </summary>
	public class MySynchronizationContext : SynchronizationContext
	{
		public readonly MyControl ctrl;

		public MySynchronizationContext()
		{
			this.ctrl = new MyControl();
		}

		/// <summary>
		/// 让主线程同步执行d方法
		/// </summary>
		public override void Send(SendOrPostCallback d, object state)
		{
			ctrl.Invoke(state => d(state), state);
		}
		/// <summary>
		/// 让主线程异步执行d方法
		/// </summary>
		public override void Post(SendOrPostCallback d, object state)
		{
			ctrl.BeginInvoke(state => d(state), state);
		}
	}

	/// <summary>
	/// 切线程异步任务
	/// </summary>
	public struct AsyncModeTask : INotifyCompletion
	{
		public bool IsCompleted { get; }
		private SynchronizationContext context;//此节点的上下文

		public AsyncModeTask GetAwaiter() => this; //await需要这个;

		public AsyncModeTask(SynchronizationContext synchronizationContext)
		{
			IsCompleted = false;
			context = synchronizationContext;
		}

		public void OnCompleted(Action continuation)
		{
			context.Post(PostCallBack, continuation);//启动一个线程回调
		}

		private void PostCallBack(object continuation)
		{
			((Action)continuation)();//继续运行
		}
		public void GetResult() { }

	}

	/// <summary>
	/// Mono线程
	/// </summary>
	public static class MonoContext
	{
		public static SynchronizationContext Instance = SynchronizationContext.Current;

	}
	/// <summary>
	/// 多线程
	/// </summary>
	public class ThreadContext : SynchronizationContext
	{
		public static ThreadContext Instance = new ThreadContext();
	}
	public struct AsyncData
	{
		public int id;
		public Vector3 position;
		public Vector3 eulerAngles;
	}

	public class AsyncTaskThread : IDisposable
	{
		Thread thread;

		AsyncTaskTest asyncTaskTest;

		Queue<AsyncData> asyncDatas = new Queue<AsyncData>();

		public SynchronizationContext context;
		public AsyncTaskThread(AsyncTaskTest asyncTaskTest)
		{
			thread = new Thread(async () =>
			{
				this.asyncTaskTest = asyncTaskTest;
				context = ThreadContext.Instance;
				while (true)
				{
					await Event();
					Thread.Sleep(20);
				}
			});
			thread.Start();
		}

		public async Task Event()
		{
			await new AsyncModeTask(MonoContext.Instance);//切主线程


			for (int i = 0; i < asyncTaskTest.list.Count; i++)
			{
				Transform trans = asyncTaskTest.list[i].transform;

				asyncDatas.Enqueue(new AsyncData() { id = i, eulerAngles = trans.eulerAngles, position = trans.position });
			}

			await new AsyncModeTask(ThreadContext.Instance);//切多线程

			for (int i = 0; i < asyncTaskTest.list.Count; i++)
			{
				asyncDatas.TryDequeue(out AsyncData asyncData);
				float angle = (asyncData.eulerAngles.y + asyncTaskTest.offset);
				asyncData.position = ToVector2(angle) * asyncData.eulerAngles.x;
				asyncData.eulerAngles = new Vector3(asyncData.eulerAngles.x, angle, 0);
				asyncDatas.Enqueue(asyncData);
			}
			await new AsyncModeTask(MonoContext.Instance);//切主线程

			Profiler.BeginSample("SDHK AsyncTaskTest");

			while (asyncDatas.TryDequeue(out AsyncData asyncData))
			{
				Transform trans = asyncTaskTest.list[asyncData.id].transform;
				trans.position = asyncData.position;
				trans.eulerAngles = asyncData.eulerAngles;
			}

			Profiler.EndSample();

			await new AsyncModeTask(ThreadContext.Instance);//切多线程
		}

		public void Dispose()
		{
			thread.Abort();
			thread = null;
		}

		/// <summary>
		/// 角度转向量
		/// </summary>
		public Vector2 ToVector2(float angle) => new Vector2(Mathf.Sin(angle / Mathf.Rad2Deg), Mathf.Cos(angle / Mathf.Rad2Deg));

		/// <summary>
		/// 向量转角度 +-180
		/// </summary>
		public float ToAngle(Vector2 vector) => Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
	}

	public class AsyncTaskTest : MonoBehaviour
	{
		public GameObject gameObj;

		public int Count = 0;

		public float offset = 0;

		public float radius = 10;

		public List<GameObject> list = new List<GameObject>();
		AsyncTaskThread asyncTaskThread;

		private int fps = 0;
		private int frameNumber = 0;
		private float lastShowFPSTime = 0f;
		private GUIStyle textStyle = new GUIStyle();

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;//关闭垂直同步
			Application.targetFrameRate = -1;//解除帧率限制
		}

		private void Start()
		{
			textStyle.normal.textColor = Color.red;
			textStyle.fontSize = 60;

			var monoContext = MonoContext.Instance;
			System.Random rand = new System.Random();

			for (int i = 0; i < Count; i++)
			{
				float randRadius = (float)(rand.NextDouble() * (radius - 1) + 1);

				GameObject gameObject = GameObject.Instantiate(gameObj);
				gameObject.transform.SetParent(transform, false);
				float angle = ((i / (float)Count) * 360f + offset);
				gameObject.transform.position = ToVector2(angle) * randRadius;
				gameObject.transform.eulerAngles = new Vector3(randRadius, angle, 0);
				list.Add(gameObject);
			}
			asyncTaskThread = new AsyncTaskThread(this);
		}
		void Update()
		{

			frameNumber += 1;//每秒帧数累计
			float time = Time.realtimeSinceStartup - lastShowFPSTime;
			if (time >= 1)//大于一秒后
			{
				fps = (int)(frameNumber / time);//计算帧数
				frameNumber = 0;//归零
				lastShowFPSTime = Time.realtimeSinceStartup;
			}
		}
		private void OnGUI()
		{
			GUILayout.Label(fps.ToString(), textStyle);
		}

		private void OnDestroy()
		{
			asyncTaskThread?.Dispose();
		}
		/// <summary>
		/// 角度转向量
		/// </summary>
		public Vector2 ToVector2(float angle) => new Vector2(Mathf.Sin(angle / Mathf.Rad2Deg), Mathf.Cos(angle / Mathf.Rad2Deg));

	}
}
