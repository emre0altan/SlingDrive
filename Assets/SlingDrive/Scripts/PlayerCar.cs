using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCar : MonoBehaviour
{
    public RoadGenerator roadGenerator;
    public Text scoreText;
    public CarMovement carMovement;
    public GameObject retryButton,levelUpText;
    public int levelLength = 15, score = 0;
    public bool restarted = false;


    /// <summary>
    /// Detects necessary objects
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NormalTurnPass"))
        {            
            score++;
            scoreText.text = score.ToString();
            if (score % levelLength == 0)
            {
                roadGenerator.MoveToNextLevel();
            }
            StartCoroutine(CheckVisible(other.transform.parent.GetComponent<MeshRenderer>(), 0));            
        }
        else if (other.CompareTag("UTurn"))
        {
            score++;
            StartCoroutine(CheckVisible(other.transform.parent.GetChild(0).GetComponent<MeshRenderer>(), 1));
        }
        else if (other.CompareTag("ShortRoad"))
        {
            StartCoroutine(CheckVisible(other.transform.parent.GetComponent<MeshRenderer>(), 2));
        }
        else if (other.CompareTag("LongRoad"))
        {
            StartCoroutine(CheckVisible(other.transform.parent.GetComponent<MeshRenderer>(), 3));
            StartCoroutine(LevelTransition());            
        }
        else if (other.CompareTag("Barrier"))
        {
            carMovement.StopCar();
            retryButton.SetActive(true);
        }
        else if (other.CompareTag("DriftAreaEnter"))
        {
            carMovement.EnterDriftArea(other.gameObject.transform.parent.gameObject);
        }
        else if (other.CompareTag("DriftAreaExit"))
        {
            carMovement.ExitDriftArea(other.gameObject.transform.parent.gameObject);
        }
    }


    /// <summary>
    /// Checks if passed road is visible
    /// </summary>
    IEnumerator CheckVisible(MeshRenderer x, int roadType)
    {
        while (true)
        {
            if (restarted) break;
            if (x == null) break;

            if (!x.isVisible)
            {
                roadGenerator.ReleaseRoad(x.gameObject, roadType);
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Initial click for game start
    /// </summary>
    public void TapToStart()
    {
        Time.timeScale = 1f;
        carMovement.StartCar();
        restarted = false;
    }

    /// <summary>
    /// Called when level up
    /// </summary>
    IEnumerator LevelTransition()
    {
        levelUpText.SetActive(true);
        Time.timeScale = 3;     
        yield return new WaitForSeconds(1f);        
        Time.timeScale = 1;
        yield return new WaitForSeconds(1f);
        levelUpText.SetActive(false);
    }
}
