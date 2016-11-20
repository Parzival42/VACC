using UnityEngine;
using System.Collections.Generic;

public class BeamAllignment : MonoBehaviour {

    private List<MassPoint> pointList;

    private bool isInitialized = false;


    [SerializeField]
    private bool manipulate = false;
    // Use this for initialization
    void Start () {
        pointList = new List<MassPoint>();
        MassPoint mp;

        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j< 15; j++)
            {
                if (j == 0)
                {
                    mp = new MassPoint(new Vector3(-10 + (j * 0.5f), 0.5f * i, 0), 20.0f, true);
                }
                else
                {
                    mp = new MassPoint(new Vector3(-10 + (j * 0.5f), 0.5f * i, 0), 20.0f, false);
                    mp.ConnectTo(pointList[pointList.Count - 1]);
                }
                pointList.Add(mp);
            }

           
        }
        for (int i = 0; i < 15; i++)
        {
            {
                pointList[i].ConnectTo(pointList[i + 15]);
            }
            if(i!=0)
            {
                pointList[i].ConnectTo(pointList[i + 14]);
            }
            if (i != 14)
            {
                pointList[i].ConnectTo(pointList[i + 16]);
            }
        }

        isInitialized = true;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (manipulate)
        {
            manipulate = false;
            pointList[8].ApplyForce(new Vector3(0, 1000 * (1 / pointList[8].InverseMass), 0));
            pointList[22].ApplyForce(new Vector3(0, 200, 0));
        }

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].UpdateConnections();
            }
        }


        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].Update(Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Vector3 size = new Vector3(0.05f, 0.05f, 0.05f);
            for (int i = 0; i < pointList.Count; i++)
            {
                //if (i != 0)
                //{
                //    Gizmos.color = Color.black;
                //    Gizmos.DrawLine(pointList[i].Position, pointList[i - 1].Position);
                //}
                Gizmos.color = Color.white;
                Gizmos.DrawCube(pointList[i].Position, size);
            }
        }
    }
}
