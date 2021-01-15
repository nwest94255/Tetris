using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Block_Manager : MonoBehaviour
{
    // Info ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /
	//
	//		This script is where global variables and functions are stored.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int falltime = 20;                       // The speed at which tetrominos fall
    public int fastfalltime = 5;                    // The speed that tetrominos fall if the player is holding down
    public int inputdelay = 5;                      // Delays player's left and right input to keep block movement speed under control
    public int rotdelay = 5;                        // Delay's rotation inputs to keep speed under control
    public int gridwidth = 12;                      // Width of the grid
    public int gridheight = 22;                     // Height of the grid
    public Vector3 spawnPos = new Vector3(6,0,0);   // The position that new tetrominos are spawned
    GameObject[] megaBlocks;                        // Stores references to all the blocks in the scene
    int downtime = 0;                               // Time between when a tetromino is placed and when another is spawned
    bool downcheck = false;                         // Signal to spawn the next tetromino
    public Text GameOverText;                       // Text used to display title / game over text
    public bool ingameover = false;                 // Has the player gotten a game over?
    public bool startgame = false;                  // Has the player started the game?

    void Awake()
    {// Good Morning ////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Show title text
        GameOverText.text = "Tetris \n \n Press Z";
    }
    void Update()
    {// Brain Salad /////////////////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Title Screen
        if(!startgame)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                startgame = true;
                GameOverText.text = "";
                SpawnTetromino();
            }
        }

        // Game Over Screen
        if(ingameover)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            }
        }
    }

    void FixedUpdate()
    {// Put time sensitive code here ////////////////////////////////////////////////////////////////////////////////////////////// x /

        // Wait for blocks to clear before spawning more
        if(downtime > 0)
        {
            downtime--;
        }
        else
        {
            if(downcheck)
            {
                if(!ingameover)
                {
                    downcheck = false;
                    SpawnTetromino();
                }
            }
        }
    }
    public void GameOver()
    {// Set up game over screen variables ///////////////////////////////////////////////////////////////////////////////////////// x /

        GameOverText.text = "Game Over";
    }
    public void SpawnTetromino()
    {// Spawn a new tetromino with a random rotation ////////////////////////////////////////////////////////////////////////////// x /

        int tetnum = Random.Range(0,6);
        Instantiate(Resources.Load(tetnum.ToString()), spawnPos, new Quaternion(0,0,0,0));
    }
    public void RefreshBlocks()
    {// Get all of the blocks in the scene //////////////////////////////////////////////////////////////////////////////////////// x /
    
        megaBlocks = GameObject.FindGameObjectsWithTag("block");
    }
    public void CheckAllRows()
    {// Check each row in the grid to see if any of them are full ///////////////////////////////////////////////////////////////// x /
    
        downcheck = true;

        for(int i = 0; i < 26; i++)
        {
            CheckRow(i);
        }
    }
    public void CheckRow(int num)
    {// Is this row full? ///////////////////////////////////////////////////////////////////////////////////////////////////////// x /

        // The number of blocks in a row
        int fillrate = 0;
        int row = 0;

        // The Y coordinates in Unity are flipped, so we need to check for negative y values
        num *= -1;

        // Get all blocks in the scene
        RefreshBlocks();
        
        for(int i = 0; i < megaBlocks.Length; i++)
        {
            // Iterate through each block and find the ones in our row
            if(megaBlocks[i].transform.position.y == num)
            {
                // Reset fillrate on each new row
                if(row != megaBlocks[i].transform.position.y)
                {
                    row = (int) megaBlocks[i].transform.position.y;
                    fillrate = 0;
                }

                // Add to the number of blocks in a row if the block's y coordinate is equal to the row's y value
                if(megaBlocks[i].transform.position.x > -1 && megaBlocks[i].transform.position.x < 13)
                    fillrate += 1;
            }

            // If a row is full, clear it and move the other blocks down
            if(fillrate >= 13)
            {
                DeleteBlocksInRow(num);
                MoveAllBlocksDown(num, 1);
                fillrate = 0;
            }
        }
    }
    public void DeleteBlocksInRow(int row)
    {// Delete all blocks in a selected row /////////////////////////////////////////////////////////////////////////////////////// x /

        // Loop through all of the blocks
        for(int i = 0; i < megaBlocks.Length; i++)
        {
            // Find the blocks in our row
            if(megaBlocks[i].transform.position.y == row 

            // Check if blocks are on the grid
            && megaBlocks[i].transform.position.x > -1 
            && megaBlocks[i].transform.position.x < 13)
            {
                GameObject.Destroy(megaBlocks[i]);
            }
        }
    }
    public void MoveAllBlocksDown(int above, int steps)
    {// Move blocks above the deleted row down //////////////////////////////////////////////////////////////////////////////////// x /

        // Our list of blocks to move down
        List<GameObject> aboveBlocks = new List<GameObject>();

        // Find all blocks above the cleared row and add them to the list
        for(int i = 0; i < megaBlocks.Length; i++)
        {
            if(megaBlocks[i].transform.position.y > above)
                aboveBlocks.Add(megaBlocks[i]);
        }

        // Move blocks in the list down one row
        for(int j = 0; j < aboveBlocks.Count; j++)
        {
            // Make sure that the blocks are on the grid
            if(aboveBlocks[j].transform.position.x > -1 
            && aboveBlocks[j].transform.position.x < 13)

                // Move the blocks down
                aboveBlocks[j].transform.position = 
                    new Vector3(aboveBlocks[j].transform.position.x, aboveBlocks[j].transform.position.y - steps, aboveBlocks[j].transform.position.z);
        }

        // Check for null tetrominos
        RemoveNull();
    }
    public void RemoveNull()
    {// Are there any tetrominos that don't have any blocks left? ///////////////////////////////////////////////////////////////// x /

        // Check for null tetrominos
        Block_Entity[] be = GameObject.FindObjectsOfType<Block_Entity>();

        for(int i = 0; i < be.Length; i++)
        {
            be[i].Nullify();
        }
    }
}
