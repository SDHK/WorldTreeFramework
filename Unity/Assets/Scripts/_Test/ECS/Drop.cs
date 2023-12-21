using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
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
			Vector3 pos = transform.position;
			float v = this.velocity + manager.G * this.mass * Time.deltaTime;
			pos.y += v;
			if (pos.y < manager.bottomY)
			{
				pos.y = manager.topY;
				this.velocity = 0f;
				this.delay = Random.Range(0, 3f);
			}
			transform.position = pos;
		}
	}
}
