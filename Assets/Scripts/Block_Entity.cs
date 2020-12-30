using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Entity : MonoBehaviour
{
    // Info ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /
	//
	//		This script is used to move and rotate falling blocks
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Block_Manager bman;                                         // The BlockManager is where we'll get most of our external variables
    public GameObject axis;                                     // GameObject used to rotate our tetromino
    public Transform[] blocks;                                  // All of the blocks in our tetromino
    public int fallTimer = 0;                                   // Countdown timer used to add steps between falls
    public int inputDelayTimer = 0;

    void Awake()
    {// Initialize Objects///////////////////////////////////////////////////////////////////////////////////////////////////////// x /
        bman = GameObject.FindObjectOfType<Block_Manager>();
        fallTimer = bman.falltime;
    }
    void Update()
    {// Put input code here /////////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Reset input delay when no direction is being held
        if(Mathf.Abs(Input.GetAxis("Horizontal")) < 0.02)
        {
            inputDelayTimer = 0;
        }
        
        if(inputDelayTimer <= 0)
        {
            // Moving Left
            if(Input.GetAxis("Horizontal") > 0)
            {
                if(CheckPositionLeftRight(">"))
                {
                    Debug.Log("Move Left");
                    inputDelayTimer = bman.inputdelay;
                    transform.position += new Vector3(1,0,0);
                }
            }

            // Moving Right
            if(Input.GetAxis("Horizontal") < 0)
            {
                if(CheckPositionLeftRight("<"))
                {
                    Debug.Log("Move Right");
                    inputDelayTimer = bman.inputdelay;
                    transform.position += new Vector3(-1,0,0);
                }
            }
        }

    }
    void FixedUpdate()
    {// Put time sensitive code here ////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Delay between inputs
        if(inputDelayTimer > 0)
        {
            inputDelayTimer--;
        }

        // Delay time between falls
        if(fallTimer > 0)
        {
            fallTimer--;
        }
        else
        {
            // Gravity
            if(CheckPositionDown())
            {
                transform.position += new Vector3(0,-1,0);
                fallTimer = bman.falltime;
            }
        }
    }
    bool CheckPositionDown()
    {// Can we still move down? /////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Check the y coordinate of each block
        for(int i = 0; i < blocks.Length; i++)
        {
            // The next block position is outside the grid
            if(blocks[i].transform.position.y - 1 < -bman.gridheight)
            {
                return false;
            }
        }

        // No blocks are outside the grid
        return true;
    }
    bool CheckPositionLeftRight(string dir)
    {// Can we still move down? /////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Check the X coordinate of each block
        for(int i = 0; i < blocks.Length; i++)
        {
            // The next block position too far forward
            if(dir == ">")
            {
                if(blocks[i].transform.position.x + 1 > bman.gridwidth)
                {
                    return false;
                }
            }

            // The next block position is too far backward
            if(dir == "<")
            {
                if(blocks[i].transform.position.x - 1 < 0)
                {
                    return false;
                }
            }
        }

        // No blocks are outside the grid
        return true;
    }
}
