using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Entity : MonoBehaviour
{
    // Info ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /
	//
	//		This script is used to move and rotate falling tetrominos
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Block_Manager bman;                                         // The BlockManager is where we'll get most of our external variables
    public GameObject axis;                                     // GameObject used to rotate our tetromino
    public Transform[] blocks;                                  // All of the blocks in our tetromino
    int fallTimer = 0;                                          // Countdown timer used to add pauses between falls
    int inputDelayTimer = 0;                                    // Countdown timer used to add pauses during L/R movement
    int rotateDelayTimer = 0;                                   // Countdown timer used in tetromino rotation
    int startRotation = 0;                                      // We give the tetromino a random start rotation on awake
    public bool stopMovement = false;                           // Freezes movement when the tetromino reaches the bottom of the grid
    public bool canrotate = true;                               // Squares can't rotate

    void Awake()
    {// Initialize Objects///////////////////////////////////////////////////////////////////////////////////////////////////////// x /
        bman = GameObject.FindObjectOfType<Block_Manager>();
        fallTimer = bman.falltime;

        // Decide random rotation
        int sr = Random.Range(0,2);
        if(sr == 1) startRotation = 90;

        axis.transform.eulerAngles = 
                new Vector3(axis.transform.eulerAngles.x, axis.transform.eulerAngles.y, axis.transform.eulerAngles.z + startRotation);
    }
    void Update()
    {// Put input code here /////////////////////////////////////////////////////////////////////////////////////////////////////// x /

        if(!stopMovement)
        {
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
                        inputDelayTimer = bman.inputdelay;
                        transform.position += new Vector3(1,0,0);
                    }
                }

                // Moving Right
                if(Input.GetAxis("Horizontal") < 0)
                {
                    if(CheckPositionLeftRight("<"))
                    {
                        inputDelayTimer = bman.inputdelay;
                        transform.position += new Vector3(-1,0,0);
                    }
                }
            }

            // Rotate
            if(rotateDelayTimer <= 0 && canrotate)
            {
                if(Input.GetButtonDown("Fire1"))
                {
                    axis.transform.eulerAngles = 
                        new Vector3(axis.transform.eulerAngles.x, axis.transform.eulerAngles.y, axis.transform.eulerAngles.z - 90);

                    rotateDelayTimer = bman.rotdelay;
                    RotateCorrection();
                }

            }
        }
    }
    void FixedUpdate()
    {// Put time sensitive code here ////////////////////////////////////////////////////////////////////////////////////////////// x /

        if(!stopMovement)
        {
            // Delay between rotations
            if(rotateDelayTimer > 0)
            {
                rotateDelayTimer--;
            }

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
                // Check if we can move down
                if(CheckPositionDown())
                {
                    // Move down
                    transform.position += new Vector3(0,-1,0);

                    // Regular fall speed
                    if(Input.GetAxis("Vertical") >= 0)
                        fallTimer = bman.falltime;
                    
                    // Fast fall speed
                    else
                        fallTimer = bman.fastfalltime;
                }
            }
        }
    }
    void RotateCorrection()
    {// If we've rotated out of bounds, put us back in bounds ///////////////////////////////////////////////////////////////////// x /

        for(int i = 0; i < blocks.Length; i++)
        {
            // Too Far Left
            if(blocks[i].transform.position.x < 0)
            {
                transform.position += new Vector3(1,0,0);
            }

            // Too Far Right
            if(blocks[i].transform.position.x > bman.gridwidth)
            {
                transform.position += new Vector3(-1,0,0);
            }
        }

    }
    bool CheckPositionDown()
    {// Can we still move down? /////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Get all of the blocks in the scene
        GameObject[] megaBlocks = GameObject.FindGameObjectsWithTag("block");

        // Check the y coordinate of each block attached to us
        for(int i = 0; i < blocks.Length; i++)
        {
            // The next block position is outside the grid
            if(blocks[i].transform.position.y - 1 < -bman.gridheight)
            {
                stopMovement = true;
                bman.SpawnTetromino();
                return false;
            }

            // Check for blocks below us that aren't attached to us
            for(int j = 0; j < megaBlocks.Length; j++)
            {
                if(blocks[i].transform.position + new Vector3(0,-1,0) == megaBlocks[j].transform.position)
                {
                    if(megaBlocks[j].transform.parent.gameObject != axis)
                    {
                        stopMovement = true;
                        bman.SpawnTetromino();
                        return false;
                    }
                }
            }
        }

        // No blocks are outside the grid
        return true;
    }
    bool CheckPositionLeftRight(string dir)
    {// Can we still move down? /////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Get all of the blocks in the scene
        GameObject[] megaBlocks = GameObject.FindGameObjectsWithTag("block");

        // Check the x coordinate of each block attached to us
        for(int i = 0; i < blocks.Length; i++)
        {
            // The next position too far forward
            if(dir == ">")
            {
                if(blocks[i].transform.position.x + 1 > bman.gridwidth)
                {
                    return false;
                }

                // Check for blocks while moving right
                for(int j = 0; j < megaBlocks.Length; j++)
                {
                    if(blocks[i].transform.position + new Vector3(1,0,0) == megaBlocks[j].transform.position)
                    {
                        if(megaBlocks[j].transform.parent.gameObject != axis)
                        {
                            return false;
                        }
                    }
                }
            }

            // The next position is too far backward
            if(dir == "<")
            {
                if(blocks[i].transform.position.x  < 0)
                {
                    return false;
                }

                // Check for blocks while moving left
                for(int j = 0; j < megaBlocks.Length; j++)
                {
                    if(blocks[i].transform.position + new Vector3(-1,0,0) == megaBlocks[j].transform.position)
                    {
                        if(megaBlocks[j].transform.parent.gameObject != axis)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        // No blocks are outside the grid
        return true;
    }
}
