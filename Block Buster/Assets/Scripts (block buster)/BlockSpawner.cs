using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{

    public GameObject blockPrefab, barrierImage, canvas, ballToSpawnPrefab;
    public GameObject[] blocksList, feedList;
    public int spaceBetweenSquares = 75;

    public int blocksPerRow = 7, blocksPerColumn = 12;
    private float conversionFactorY, conversionFactorX, barrierImageHeight, barrierImageWidth;
    private Camera cam = null;
    private int numBlocksToSpawn, iBlockTimes2;
    private bool rowHasRoundX2, spawnGem;
    List<int> usedPositions;

    /*
     * Class Description: BlockSpawner
     * gets height and width of black boundry lines and scales each box to fit perfectly inside of those lines 
     * as well as randomizing number of blocks spawned each round (max 7) and randdomizing position of block in one of the 7 previously defined positions
     */

    // ***********Forumlas***********
    // width of one block = Screen width / number of blocks
    // Screen width (unity units) = cam.orthographicSize * 2 * Screen.width/Screen.height;

    void OnEnable()
    {
        // get the components of main camera in screen to adjust size of characters and obejcts
        if (cam == null)
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (usedPositions == null)
            usedPositions = new List<int>();

        if (PlayerPrefManager.GetBool("gameRunning") == false)
        {
            SpawnBlocks();
        }

    }


    public void SpawnBlocks()
    {
        Vector3 tmpPos = cam.ViewportToWorldPoint(transform.position); // get world position of camera


        //adjust boundries compared to a canvas of 750 x 1334 (iphone 6 Tall)
        if (Screen.height != 1334)
        {
            barrierImageHeight = barrierImage.GetComponent<RectTransform>().rect.height * ((float)Screen.height / 1334f);
        }
        else
            barrierImageHeight = barrierImage.GetComponent<RectTransform>().rect.height;

        if (Screen.width != 750)
        {
            barrierImageWidth = barrierImage.GetComponent<RectTransform>().rect.width * ((float)canvas.GetComponent<RectTransform>().rect.width / 750f);
        }
        else
            barrierImageWidth = barrierImage.GetComponent<RectTransform>().rect.width;


        float moveOverHeight = Screen.height - barrierImageHeight;
        float moveOverWidth = (float)canvas.GetComponent<RectTransform>().rect.width - barrierImage.GetComponent<RectTransform>().rect.width;

        float moveOverHeightPixToWorld = (moveOverHeight / (float)Screen.height) * (-tmpPos.y * 2);
        float moveOverWidthPixToWorld = (moveOverWidth / (float)canvas.GetComponent<RectTransform>().rect.width) * (-tmpPos.x * 2);

        if (moveOverWidthPixToWorld < 0)
            moveOverWidthPixToWorld = moveOverWidthPixToWorld * -1;
        if (moveOverHeightPixToWorld < 0)
            moveOverHeightPixToWorld = moveOverHeightPixToWorld * -1;
        //*******************************************************************************************

        float widthOfBlock = (-tmpPos.x * 2 - (moveOverWidthPixToWorld)) / blocksPerRow;

        float heightOfBlock = cam.orthographicSize * 2;


        /* chance of spawning X blocks */
        float spawnNum = Random.Range(0, 100);
        if (spawnNum >= 0 && spawnNum <= 10)
            numBlocksToSpawn = 1;
        else if (spawnNum >= 11 && spawnNum <= 26)
            numBlocksToSpawn = 2;
        else if (spawnNum >= 27 && spawnNum <= 59)
            numBlocksToSpawn = 3;
        else if (spawnNum >= 60 && spawnNum <= 75)
            numBlocksToSpawn = 4;
        else if (spawnNum >= 76 && spawnNum <= 89)
            numBlocksToSpawn = 5;
        else if (spawnNum >= 90 && spawnNum <= 100)
            numBlocksToSpawn = 6;



        //chance of times 2 number
        if (RandomBool() == true)
        {
            iBlockTimes2 = Random.Range(0, numBlocksToSpawn);
            rowHasRoundX2 = true;
        }
        else
            rowHasRoundX2 = false;
        //****************************


        // Build Row (Blocks, Circles, Gems, Balls)
        for (int i = 0; i < numBlocksToSpawn; i++)
        {
            int k;

            for (k = 0; k < blocksList.Length; k++)
            {
                if (blocksList[k].activeSelf == false)
                {
                    break;
                }
            }


            if (k < 63 && blocksList[k] != null) // total of 63 blocks spawned when app is opened to increase performance during gameplay
            {
                blocksList[k].transform.localScale = new Vector3(widthOfBlock - (widthOfBlock / spaceBetweenSquares), heightOfBlock / blocksPerColumn, 1);
                float addPosX = -tmpPos.x * 2 / blocksPerRow;

                if (rowHasRoundX2 == true && iBlockTimes2 == i)
                    blocksList[k].GetComponent<Block>().timesTwo = true;


                //calculate where to place blocks
                int j = Random.Range(0, blocksPerRow);
                while (usedPositions.Contains(j) == true) // look for position that is not yet used
                {
                    j = Random.Range(0, blocksPerRow);
                }
                usedPositions.Add(j);

                blocksList[k].transform.position = new Vector3((tmpPos.x + (j * widthOfBlock) + (blocksList[k].transform.localScale.x / 2) + (moveOverWidthPixToWorld / 2)), blocksList[k].transform.parent.position.y, 0);

                blocksList[k].SetActive(true);
                Debug.Log(blocksList[k].name);
            }
        }


        //spawn ball
        int h;

        for (h = 0; h < feedList.Length; h++)
        {
            if (feedList[h].activeSelf == false)
            {
                break;
            }
        }

        if (h < 10 && feedList[h] != null)
        {
            feedList[h].transform.localScale = new Vector3((widthOfBlock - (widthOfBlock / spaceBetweenSquares)) / 4.8f, (heightOfBlock / blocksPerColumn) / 6f, 1);

            //calculate where to place blocks
            int j = Random.Range(0, blocksPerRow);
            while (usedPositions.Contains(j) == true)
            {
                j = Random.Range(0, blocksPerRow);
            }


            feedList[h].transform.position = new Vector3((tmpPos.x + (j * widthOfBlock) + (blockPrefab.transform.localScale.x / 2) + (moveOverWidthPixToWorld / 2) - 0.1f), feedList[h].transform.parent.position.y, 0);
            //no need to add to usedPositions since this is the last spawned prefab

            feedList[h].SetActive(true);
        }


        //clear used Positions for next row
        usedPositions.Clear();
    }

    bool RandomBool()
    {
        float num = Random.Range(0, 2);
        if (num == 0)
            return false;
        else
            return true;
    }



}
