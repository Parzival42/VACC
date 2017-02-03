﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the base mass spring system. this class is concerned with 
/// updating mass point positions that are connected by springs
/// </summary>
public abstract class CoreMSDSystem : MonoBehaviour {

    #region variables
    /// <summary>
    /// stores the point masses
    /// </summary>
    protected List<PointMass> pointList;

    /// <summary>
    /// stores the spring constraints
    /// </summary>
    protected List<Constraint> constraintList;
    #endregion

    #region abstract methods
    /// <summary>
    /// lets the sub class run specific initialization code 
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// generates the point masses and saves them in the pointList member
    /// </summary>
    public abstract void GeneratePointMasses();

    /// <summary>
    /// generates the spring connections and saves them in the constraintList member
    /// </summary>
    public abstract void GenerateConstraints();

    /// <summary>
    /// is called before the actual system update cycle is started
    /// </summary>
    public abstract void PreSimulationStep();

    /// <summary>
    /// is called after the update cycle of the system is finished
    /// </summary>
    public abstract void PostSimulationStep();
    #endregion

    #region methods
    
    /// <summary>
    /// initialization. calls the generation methods
    /// </summary>
    void Start () {
        GeneratePointMasses();
        GenerateConstraints();
        Initialize();
    }
	
	/// <summary>
    /// update cycle of the system. calculates the new point mass positions
    /// </summary>
	void FixedUpdate () {
        PreSimulationStep();

        //update constraints
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].Solve();
            }
        }

        //update pointmasses
        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].Simulate(Time.deltaTime);
        }

        PostSimulationStep();
    }

    /// <summary>
    /// drawing the point masses and their connections
    /// for debugging
    /// </summary>
    void OnDrawGizmos()
    {
        if (pointList!=null)
        {
            Vector3 size = new Vector3(0.05f, 0.05f, 0.05f);
            for (int i = 0; i < pointList.Count; i++)
            {
                pointList[i].DrawPoint();
            }

            for (int i = 0; i < constraintList.Count; i++)
            {
                constraintList[i].DrawConnection();
            }
        }
    }

    #endregion
}
