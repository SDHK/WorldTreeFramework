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
	async void Update()
	{

		if (Input.GetKeyDown(KeyCode.A))
		{

			Profiler.BeginSample("SDHK");

			Profiler.EndSample();
			UnityEditor.EditorApplication.isPaused = true;
		}

	}



}