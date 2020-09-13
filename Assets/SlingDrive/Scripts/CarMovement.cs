using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for car movement
/// </summary>

public class CarMovement : MonoBehaviour
{
    public GripLine gripLine;
    public bool InDriftArea  = false;
    public int carSpeed = 40;
    public float damping = 1f, n = 1f, offset = -37.85f;


    private bool isPressing = false,isCarMoving = true, isCarDrifting = false, isCarSwerving = false, isRoadChanged = false, isUTurning = false;
    private Transform carBody;
    private int turnDirection = 0;
    private float targetAngle = 0, carAngle = 0,carAngularAcc = 0, carAngularVel = 0, roadAngle = 0, roadTurnCounter = 0f, preRoadAngle = 0;

    private void Start()
    {
        carBody = transform.GetChild(0);
    }

    void Update()
    {
        if (isCarMoving)
        {
            if (isPressing && InDriftArea)
            {
                if (!isCarDrifting)
                {
                    //Starts drifting
                    if (!isRoadChanged)
                    {
                        turnDirection = gripLine.SetObj(gameObject);
                        ChangeRoadDirection();
                        isRoadChanged = true;
                    }
                    isCarDrifting = true;
                    StopCoroutine("CarSwerve");                    
                    gripLine.gameObject.SetActive(true);                    
                }
            }            
            else if (!isPressing)
            {
                if (isCarDrifting)
                {
                    //Stops drifting
                    isCarDrifting = false;
                    StartCoroutine("CarSwerve");
                    gripLine.gameObject.SetActive(false);                    
                }
            }

            //Position and rotation settings
            if (!isCarDrifting)
            {
                targetAngle = roadAngle;
                carAngle = transform.rotation.y;

                if (Mathf.Abs(carAngle - targetAngle) < Mathf.Abs(targetAngle - carAngle)) carAngularAcc = Mathf.Abs(carAngle - targetAngle) * n;
                else carAngularAcc = Mathf.Abs(targetAngle - carAngle) * n;

                carAngularVel += carAngularAcc;
                carAngularVel *= damping;

                if (targetAngle < 0 || (turnDirection == -1 && roadAngle == -90)) carAngle -= carAngularVel;
                else carAngle += carAngularVel;

                if(!isCarSwerving)  transform.rotation = Quaternion.Euler(transform.rotation.x, carAngle, transform.rotation.z);

                transform.position = transform.position + new Vector3(carSpeed * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad), 0, carSpeed * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) * Time.deltaTime;
            }
            else
            {
                if (turnDirection == 1 && roadAngle == 90) targetAngle = gripLine.angle + 90 + offset;
                else if(turnDirection == 1 && roadAngle == 0) targetAngle = gripLine.angle + 90 + offset;
                else if(turnDirection == -1 && roadAngle == -90) targetAngle = gripLine.angle -180 + offset;
                else if(turnDirection == -1 && roadAngle == 0) targetAngle = gripLine.angle -180 + offset;

                carAngle = transform.rotation.y;
                if (Mathf.Abs(carAngle - targetAngle) < Mathf.Abs(targetAngle - carAngle)) carAngularAcc = Mathf.Abs(carAngle - targetAngle) * n;
                else carAngularAcc = Mathf.Abs(targetAngle - carAngle) * n;
                carAngularVel += carAngularAcc;
                carAngularVel *= damping;
                if (targetAngle < 0 || (turnDirection == -1 && roadAngle == -90)) carAngle -= carAngularVel;
                else carAngle += carAngularVel;
                transform.rotation = Quaternion.Euler(transform.rotation.x, carAngle, transform.rotation.z);

                transform.position = gripLine.line.GetPosition(1) + new Vector3(0, -0.79f, 0);
                
                if(roadTurnCounter > 1f && gripLine.gameObject2.transform.parent.CompareTag("UTurn"))
                {
                    roadAngle = -preRoadAngle;
                    roadTurnCounter = 0f;
                    isUTurning = false;
                }
                if(isUTurning) roadTurnCounter += Time.deltaTime;
            }
        }        
    }


    /// <summary>
    /// Stops car for restart
    /// </summary>
    public void StopCar()
    {
        isCarMoving = false;
        gripLine.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    /// <summary>
    /// Starts car again
    /// </summary>
    public void StartCar()
    {
        isCarMoving = true;
    }

    /// <summary>
    /// Called when car entered turn area
    /// </summary>
    public void EnterDriftArea(GameObject road)
    {
        if (!InDriftArea)
        {            
            gripLine.gameObject2 = road.transform.GetChild(road.transform.childCount - 1).gameObject;
            StopCoroutine("ResetTower");
            InDriftArea = true;
        }      
    }
    /// <summary>
    /// Called when car exited turn area 
    /// </summary>
    public void ExitDriftArea(GameObject road)
    {
        if (InDriftArea)
        {
            InDriftArea = false;
            isRoadChanged = false;
            StartCoroutine("ResetTower");            
        }        
    }

    /// <summary>
    /// Reset other point of line
    /// </summary>
    IEnumerator ResetTower()
    {
        yield return new WaitForSeconds(1f);
        gripLine.gameObject2 = null;
    }

    /// <summary>
    /// Creates car swerve after drift finished
    /// </summary>
    IEnumerator CarSwerve()
    {
        isCarSwerving = true;
        float carAng = carAngle, tmpAngle = carAngle, roadAng = roadAngle;
        int turnDir = turnDirection;

        if(roadAng == 90 && turnDir == 1)
        {
            while (tmpAngle > roadAng - (carAng - roadAng) * 0.5f)
            {
                tmpAngle -= 3.2f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }
            
            while (tmpAngle < roadAng + (carAng-roadAng) * 0.25f)
            {
                tmpAngle += 1.6f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > roadAng - (carAng - roadAng) * 0.064f)
            {
                tmpAngle -= 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle < roadAng)
            {
                tmpAngle += 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }
        }
        else if (roadAng == 0 && turnDir == -1)
        {
            while (tmpAngle <  -carAng * 0.5f)
            {
                tmpAngle += 3.2f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > carAng * 0.25f)
            {
                tmpAngle -= 1.6f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle < -carAng * 0.064f)
            {
                tmpAngle += 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > roadAng)
            {
                tmpAngle -= 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }
        }
        else if (roadAng == -90 && turnDir == -1)
        {
            while (tmpAngle < roadAng - (carAng- roadAng) * 0.5f)
            {
                tmpAngle += 3.2f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > roadAng + (carAng - roadAng) * 0.25f)
            {
                tmpAngle -= 1.6f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle < roadAng - (carAng - roadAng) * 0.064f)
            {
                tmpAngle += 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > roadAng)
            {
                tmpAngle -= 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }
        }
        else if (roadAng == 0 && turnDir == 1)
        {
            while (tmpAngle > -carAng * 0.5f)
            {
                tmpAngle -= 3.2f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle < carAng * 0.25f)
            {
                tmpAngle += 1.6f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > -carAng * 0.064f)
            {
                tmpAngle -= 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }

            while (tmpAngle > roadAng)
            {
                tmpAngle += 0.8f;
                transform.rotation = Quaternion.Euler(transform.rotation.x, tmpAngle, transform.rotation.z);
                //Debug.Log("CarAngle: " + carAngle + " RoadAngle: " + roadAngle + " TmpAngle: " + tmpAngle);
                yield return null;
            }
        }

        isCarSwerving = false;
    }

    /// <summary>
    /// Changes road direction while turning
    /// </summary>
    public void ChangeRoadDirection()
    {
        if (gripLine.gameObject2.transform.parent.CompareTag("NormalTurn"))
        {
            if (turnDirection == 1) roadAngle = roadAngle % 360 + 90;
            else roadAngle = roadAngle % 360 - 90;
        }
        else
        {
            preRoadAngle = roadAngle;
            roadAngle = 0;
            isUTurning = true;
        }
    }

    /// <summary>
    /// Changes isPressing for inputs
    /// </summary>
    public void SetPointer(bool state)
    {
        isPressing = state;
    }
}
