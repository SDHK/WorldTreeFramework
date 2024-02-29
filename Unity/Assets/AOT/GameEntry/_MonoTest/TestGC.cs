using UnityEngine;
using UnityEngine.Profiling;

public class TestGC : MonoBehaviour
{
	public GameObject gameObject1;

	public GameObject gameObject2;

	public GameObject gameObject3;

	public int y;
	public int dataY;
	public int byteIndex;

	public int notRemainderY;


	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{


		Profiler.BeginSample("SDHK");
		//向量打到平面上的点
		Vector3 rayDir = gameObject1.transform.forward;
		Vector3 rayOrigin = gameObject1.transform.position;
		Vector3 planeNormal = gameObject2.transform.up;

		float dot = Vector3.Dot(rayDir, planeNormal);
		float d = -Vector3.Dot(planeNormal, gameObject2.transform.position);
		float t = -(Vector3.Dot(planeNormal, rayOrigin) + d) / dot;

		gameObject3.transform.position = rayOrigin + t * rayDir;

		Profiler.EndSample();



	}


}