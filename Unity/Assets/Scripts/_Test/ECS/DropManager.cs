using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class DropTest
{
	public GameObject gameObject;
	public DropManager manager;
	public float mass;
	public float delay;
	public float velocity;

	public void Update1()
	{
		if (this.delay > 0)
		{
			this.delay -= Time.deltaTime;
		}
		else
		{
			Vector3 pos = gameObject.transform.position;
			float v = this.velocity + manager.G * this.mass * Time.deltaTime;
			pos.y += v;
			if (pos.y < manager.bottomY)
			{
				pos.y = manager.topY;
				this.velocity = 0f;
				this.delay = Random.Range(0, 3f);
			}
			gameObject.transform.position = pos;
		}
	}

}

public class DropManager : MonoBehaviour
{
	public int G;
	public int bottomY;
	public int topY;
	public int spawnCount;
	public GameObject balls;

	public List<DropTest> bts = new();

	void Start()
	{
		StartLagacyMethod();


	}

	void Update()
	{
		Profiler.BeginSample("SDHK");

		for (int i = 0; i < bts.Count; i++)
		{
			bts[i].Update1();
		}

		Profiler.EndSample();

	}

	void StartLagacyMethod()
	{
		var rootGo = this;
		rootGo.transform.position = Vector3.zero;

		for (int i = 0; i < spawnCount; ++i)
		{
			var go = Instantiate(balls);
			go.name = "Drop_" + i;

			var dropComponent = new DropTest();
			dropComponent.manager = this;
			dropComponent.gameObject = go;
			bts.Add(dropComponent);
			//dropComponent.delay = 0.02f * i;
			dropComponent.mass = Random.Range(0.5f, 3f);

			Vector3 pos = Random.insideUnitSphere * 40;
			go.transform.parent = rootGo.transform;
			pos.y = topY;
			go.transform.position = pos;
		}
	}
}
