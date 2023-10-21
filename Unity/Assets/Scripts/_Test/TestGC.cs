using ET;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using WorldTree;

public class TestGC : MonoBehaviour
{
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

		if (Input.GetKeyDown(KeyCode.A))
		{

			Profiler.BeginSample("SDHK");
			Test1().Coroutine();
			Profiler.EndSample();
			UnityEditor.EditorApplication.isPaused = true;
		}


	}

	public async ETTask Test1(bool b = false)
	{
		Debug.Log("1！");
		await ETTask.CompletedTask;
		await Test2();
		Debug.Log("1结束！");

	}

	public async ETTask Test2(bool b = false)
	{
		Debug.Log("2！");
		await ETTask.CompletedTask;
		await Test3();
		Debug.Log("2结束！");
	}

	public async ETTask Test3(bool b = false)
	{
		Debug.Log("3！");
		if (b) await ETTask.CompletedTask;
		Debug.Log("3结束！");
	}



}