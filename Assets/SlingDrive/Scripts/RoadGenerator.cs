using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadGenerator : MonoBehaviour
{
    public PlayerCar playerCar;
    public CarMovement carMovement;
    public Transform road;
    public GameObject shortRoad, longRoad, turnRoad, uTurnRoad;
    [Range(0, 10)]
    public int uTurnPossibility = 0;

    private Stack<GameObject> shortRoadPool, longRoadPool, turnRoadPool, uTurnRoadPool;
    private int levelLength;
    private GameObject tempObject;
    private Vector3 levelLastPosition;
    private int levelLastDirection;


    Vector3 tmpPos = Vector3.zero;
    int tmpDir = 2;

    private void Awake()
    {
        Time.timeScale = 0;
        levelLength = playerCar.levelLength;

        shortRoadPool = new Stack<GameObject>();
        turnRoadPool = new Stack<GameObject>();
        longRoadPool = new Stack<GameObject>();
        uTurnRoadPool = new Stack<GameObject>();

        //Pooling
        for(int i = 0; i < levelLength*2+5; i++)
        {         
            //Normal turn
            tempObject = GameObject.Instantiate(turnRoad, road);
            turnRoadPool.Push(tempObject);
            tempObject.SetActive(false);

            //U turn
            tempObject = GameObject.Instantiate(uTurnRoad, road);
            uTurnRoadPool.Push(tempObject);
            tempObject.SetActive(false);

            //Normal straight road            
            tempObject = GameObject.Instantiate(shortRoad, road);
            tempObject.SetActive(false);
            shortRoadPool.Push(tempObject);            

            //Long roads for start and level transitions
            if(i > levelLength * 2)
            {
                tempObject = GameObject.Instantiate(longRoad, road);
                tempObject.SetActive(false);
                longRoadPool.Push(tempObject);
            }
        }
        /////////

        ///Create 2 Level
        CreateOneLevel(tmpPos, tmpDir, out levelLastPosition, out levelLastDirection);
        tmpPos = levelLastPosition;
        tmpDir = levelLastDirection;
        CreateOneLevel(tmpPos, tmpDir, out levelLastPosition, out levelLastDirection);
        tmpPos = levelLastPosition;
        tmpDir = levelLastDirection;
    }


    /// <summary>
    /// Creates random rood with length of levelLength. Takes initial point parameters and return out parameters as next initial point
    /// </summary>
    private bool CreateOneLevel(Vector3 longRoadPos, int longRoadDirection, out Vector3 levelLastPos, out int levelLastDirec)//0 for Right, 1 for Forward, 2 for Left
    {
        if (longRoadPool.Count < 1 || shortRoadPool.Count < 14 || turnRoadPool.Count < 15 || uTurnRoadPool.Count < 15)
        {
            Debug.Log("Not enough objects, populate pools!");
            levelLastPos = Vector3.zero;
            levelLastDirec = 0;
            return false;
        }

        GameObject tempObj, lastObject;
        int lastDirection = 0; //0 for Left, 1 for Right, 2 for Forward

        tempObj = longRoadPool.Pop();

        if (longRoadDirection == 0)
        {
            tempObj.transform.rotation = Quaternion.Euler(0, 270, 0);
            tempObj.transform.position = longRoadPos + new Vector3(-20, 0, 20);
        }
        else if (longRoadDirection == 1)
        {
            tempObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            tempObj.transform.position = longRoadPos + new Vector3(20, 0, 20);
        }
        else if (longRoadDirection == 2)
        {
            tempObj.transform.rotation = Quaternion.identity;
            tempObj.transform.position = longRoadPos;
        }
        lastObject = tempObj;
        lastDirection = longRoadDirection;
        lastObject.SetActive(true);

        int tmpChoise = 0;
        for (int i = 1; i < levelLength * 2; i++)
        {
            //Turn roads are settled. U Turn or Normal Turn
            if (i % 2 == 1)
            {
                //If random integer is smaller than uTurnPossibility then create U turn, else create normal turn
                tmpChoise = Random.Range(0, 11);

                if (tmpChoise < uTurnPossibility && i / 2 > 2 && i / 2 < levelLength - 2 && lastDirection != 2)
                {
                    tempObj = uTurnRoadPool.Pop();
                    if (lastDirection == 0)
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                        lastDirection = 1;
                        tempObj.transform.position = lastObject.transform.position + new Vector3(-20, 0, 20);
                        
                    }
                    else if (lastDirection == 1)
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                        lastDirection = 0;
                        tempObj.transform.position = lastObject.transform.position + new Vector3(60, 0, 20);
                        RestoreUTurnColliders(tempObj);
                    }
                }
                else
                {
                    tempObj = turnRoadPool.Pop();
                    if (lastDirection == 0)
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                        tempObj.transform.localScale = new Vector3(1, 1, -1);

                        RestoreNormalTurnColliders(tempObj);

                        lastDirection = 2;
                        if (lastObject.CompareTag("LongRoad"))
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(-100, 0, 20);
                        }
                        else if (lastObject.CompareTag("ShortRoad"))
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(-20, 0, 20);
                        }
                    }
                    else if (lastDirection == 1)
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                        tempObj.transform.localScale = new Vector3(1, 1, 1);
                        lastDirection = 2;

                        RestoreNormalTurnColliders(tempObj);

                        if (lastObject.CompareTag("LongRoad"))
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(100, 0, 20);
                        }
                        else if (lastObject.CompareTag("ShortRoad"))
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(60, 0, 20);
                        }
                    }
                    else
                    {
                        int newDirec = Random.Range(0,2);
                        if (newDirec == 0)
                        {
                            tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                            tempObj.transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else
                        {
                            tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                            tempObj.transform.localScale = new Vector3(1, 1, 1);
                        }
                        lastDirection = newDirec;
                        if (lastObject.tag == "LongRoad")
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(0, 0, 80);
                        }
                        else if (lastObject.tag == "ShortRoad")
                        {
                            tempObj.transform.position = lastObject.transform.position + new Vector3(0, 0, 40);
                        }
                    }
                }
            }
            //Short road are settled
            else
            {
                tempObj = shortRoadPool.Pop();
                if (lastDirection == 0)
                {
                    if (lastObject.CompareTag("NormalTurn"))
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 90, 0);
                        tempObj.transform.position = lastObject.transform.position + new Vector3(-60, 0, 20);
                    }
                    else if (lastObject.CompareTag("UTurn"))
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 90, 0);
                        tempObj.transform.position = lastObject.transform.position + new Vector3(-60, 0, 20);
                    }
                }
                else if (lastDirection == 1)
                {
                    if (lastObject.CompareTag("NormalTurn"))
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 90, 0);
                        tempObj.transform.position = lastObject.transform.position + new Vector3(20, 0, 20);
                    }
                    else if (lastObject.CompareTag("UTurn"))
                    {
                        tempObj.transform.rotation = Quaternion.Euler(0, 90, 0);
                        tempObj.transform.position = lastObject.transform.position + new Vector3(20, 0, 20);
                    }
                }
                else
                {
                    tempObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    tempObj.transform.position = lastObject.transform.position + new Vector3(0, 0, 0);
                }
            }
            lastObject = tempObj;
            lastObject.SetActive(true);
        }

        levelLastPos = lastObject.transform.position;
        levelLastDirec = lastDirection;

        return true;
    }

    /// <summary>
    /// Recycles passed road
    /// </summary>
    public void ReleaseRoad(GameObject road, int roadType)
    {
        //road.SetActive(false);
        if (roadType == 0) turnRoadPool.Push(road);
        else if (roadType == 1) uTurnRoadPool.Push(road.transform.parent.gameObject);
        else if (roadType == 2) shortRoadPool.Push(road);
        else if (roadType == 3) longRoadPool.Push(road);
    }

    /// <summary>
    /// Next level roads settled when level up
    /// </summary>
    public void MoveToNextLevel()
    {
        CreateOneLevel(tmpPos, tmpDir, out levelLastPosition, out levelLastDirection);
        tmpPos = levelLastPosition;
        tmpDir = levelLastDirection;
    }

    /// <summary>
    /// Restarts scene
    /// </summary>
    public void Restart()
    {        
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Restores colliders' positions for enter and exit triggering
    /// </summary>
    public void RestoreNormalTurnColliders(GameObject tempObj)
    {
        Vector3 myTmpPos;
        Quaternion myTmpRot;
        myTmpPos = tempObj.transform.GetChild(0).position;
        myTmpRot = tempObj.transform.GetChild(0).rotation;
        tempObj.transform.GetChild(0).position = tempObj.transform.GetChild(1).position;
        tempObj.transform.GetChild(0).rotation = tempObj.transform.GetChild(1).rotation;
        tempObj.transform.GetChild(1).position = myTmpPos;
        tempObj.transform.GetChild(1).rotation = myTmpRot;
    }

    /// <summary>
    /// Restores colliders' positions for enter and exit triggering
    /// </summary>
    public void RestoreUTurnColliders(GameObject tempObj)
    {
        Vector3 myTmpPos;
        Quaternion myTmpRot;
        myTmpPos = tempObj.transform.GetChild(2).position;
        myTmpRot = tempObj.transform.GetChild(2).rotation;
        tempObj.transform.GetChild(2).position = tempObj.transform.GetChild(3).position;
        tempObj.transform.GetChild(2).rotation = tempObj.transform.GetChild(3).rotation;
        tempObj.transform.GetChild(3).position = myTmpPos;
        tempObj.transform.GetChild(3).rotation = myTmpRot;
    }
}
