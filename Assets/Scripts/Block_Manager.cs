using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Manager : MonoBehaviour
{
    // Info ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /
	//
	//		This script is where global variables are stored.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int falltime = 20;
    public int fastfalltime = 5;
    public int inputdelay = 5;
    public int rotdelay = 5;
    public int gridwidth = 12;
    public int gridheight = 22;
    public Vector3 spawnPos = new Vector3(6,0,0);

    public void SpawnTetromino()
    {
        int tetnum = Random.Range(0,6);
        Instantiate(Resources.Load(tetnum.ToString()), spawnPos, new Quaternion(0,0,0,0));
    }
}
