using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TextCore.Text;

namespace ECSTest
{

	public struct MailData
	{
		public int id;
		public Vector3 position;
		public Vector3 eulerAngles;
	}
	public class MailThread : IDisposable
	{
		List<MailData> list = new List<MailData>();

		Thread thread;

		public MailThread(MailTest mailTest, int Count, float offset, float radius)
		{
			System.Random rand = new System.Random();

			//初始化数据
			for (int i = 0; i < Count; i++)
			{
				float angle = ((i / (float)Count) * 360f + offset);
				float randRadius = (float)(rand.NextDouble() * (radius - 1) + 1);
				list.Add(new MailData() { id = i, position = ToVector2(angle) * randRadius, eulerAngles = new Vector3(randRadius, angle, 0) });
			}


			thread = new Thread(() =>
			{
				while (true)
				{
					for (int i = 0; i < list.Count; i++)
					{
						MailData trans = list[i];
						float angle = (trans.eulerAngles.y + offset);
						trans.position = ToVector2(angle) * trans.eulerAngles.x;
						trans.eulerAngles = new Vector3(trans.eulerAngles.x, angle, 0);
						list[i] = trans;
					}
					if (mailTest.mailDatas.Count <= list.Count)
					{
						for (int i = 0; i < list.Count; i++)
						{
							mailTest.mailDatas.Enqueue(list[i]);
						}
					}
					Thread.Sleep(20);
				}
			});
			thread.Start();
		}

		/// <summary>
		/// 角度转向量
		/// </summary>
		public Vector2 ToVector2(float angle) => new Vector2(Mathf.Sin(angle / Mathf.Rad2Deg), Mathf.Cos(angle / Mathf.Rad2Deg));

		/// <summary>
		/// 向量转角度 +-180
		/// </summary>
		public float ToAngle(Vector2 vector) => Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

		public void Dispose()
		{
			thread.Abort();
		}
	}

	public class MailTest : MonoBehaviour
	{
		public GameObject gameObj;

		public int Count = 0;

		public float offset = 0;

		public float radius = 10;

		List<GameObject> list = new List<GameObject>();

		/// <summary>
		/// 邮箱
		/// </summary>
		public Queue<MailData> mailDatas = new Queue<MailData>();
		MailThread mailThread = null;


		private int fps = 0;
		private int frameNumber = 0;
		private float lastShowFPSTime = 0f;
		private GUIStyle textStyle = new GUIStyle();

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;//关闭垂直同步
			Application.targetFrameRate = -1;//解除帧率限制
		}

		void Start()
		{
			textStyle.normal.textColor = Color.red;
			textStyle.fontSize = 60;
			for (int i = 0; i < Count; i++)
			{
				GameObject gameObject = GameObject.Instantiate(gameObj);
				gameObject.transform.SetParent(transform, false);
				list.Add(gameObject);
			}

			//线程启动
			mailThread = new MailThread(this, Count, offset, radius);
		}

		void Update()
		{
			Profiler.BeginSample("SDHK MailTest");
			while (mailDatas.TryDequeue(out MailData data))
			{
				Transform trans = list[data.id].transform;
				trans.position = data.position;
				trans.eulerAngles = data.eulerAngles;
			}
			Profiler.EndSample();

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
			mailThread?.Dispose();
		}

	}
}


