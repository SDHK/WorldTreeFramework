/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WorldTree;

public class TestLine4 : MonoBehaviour
{
    public int index = 10;
    public float border = 1f;
    public List<Transform> line;

    //public Transform last;
    public Transform current;
    public Transform target;


    public MoveAreaLimiter moveAreaLimiter;

    public List<Vector2> points;

    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        moveAreaLimiter = new MoveAreaLimiter();
        points = line.Select((item) => item.position.ToXZ()).ToList();
        moveAreaLimiter.SetPoints(points);
    }

    // Update is called once per frame
    void Update()
    {
        //current.position = target.position;

        moveAreaLimiter.linePoints = line.Select((item) => item.position.ToXZ()).ToList();

        moveAreaLimiter.border = border;
        //current.position = moveAreaLimiter.Limit(current.position.ToXZ()).ToXZ();



    }


    private void OnDrawGizmos()
    {

        for (int i = 0; i < line.Count; i++)
        {
            Vector2 node1 = line[i].position.ToXZ();
            Vector2 node2 = (i == line.Count - 1) ? line[0].position.ToXZ() : line[i + 1].position.ToXZ();

            Debug.DrawLine(new Vector3(node1.x, 0, node1.y), new Vector3(node2.x, 0, node2.y), Color.green);
        }
    }



}
*/