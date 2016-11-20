using UnityEngine;
using System.Collections.Generic;

public class RopeAllignment : MonoBehaviour {

    private List<MassPoint> pointList;

    private bool isInitialized = false;


    [SerializeField]
    private bool manipulate = false;
    // Use this for initialization
    void Start () {
        pointList = new List<MassPoint>();
        MassPoint mp;
        for (int i = 0; i < 30; i++)
        {
            if(i==0)
            {
                mp = new MassPoint(new Vector3(-10+(i*0.5f),0,0), 0.05f, true);
                pointList.Add(mp);
            }
            else
            {
                if (i == 20)
                {
                    mp = new MassPoint(new Vector3(-10 + (i*0.5f), 0, 0), 0.05f, true);

                }
                else
                {
                    mp = new MassPoint(new Vector3(-10+(i*0.5f),0,0), 0.05f, false);
                }
                mp.ConnectTo(pointList[pointList.Count - 1]);
                pointList.Add(mp);
            }
        }

        isInitialized = true;
	    
	}

    void Update()
    {
        if (manipulate)
        {
            manipulate = false;
            pointList[8].ApplyForce(new Vector3(0, 1000*(1/pointList[8].InverseMass),0));
            pointList[22].ApplyForce(new Vector3(0, 200, 0));
        }

        for(int j = 0; j < 15; j++)
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
                if (i != 0)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(pointList[i].Position, pointList[i - 1].Position);
                }
                Gizmos.color = Color.white;
                Gizmos.DrawCube(pointList[i].Position, size);
            }
        }
    }
}
