using UnityEngine;

public class GripLine : MonoBehaviour
{
    public GameObject gameObject1;          // Reference to the first GameObject
    public GameObject gameObject2;          // Reference to the second GameObject
    public float angularSpeed = 150f;
    public float angle = 0;

    public LineRenderer line;                           // Line Renderer
    private bool turning = false;
    private int direc = 1;
    private float length = 0;
     

    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        // Set the number of vertex fo the Line Renderer
        line.positionCount = 2;
    }

    private void Update()
    {
        if (gameObject1 != null && gameObject2 != null && turning)
        {
            if (direc == 1) angle += angularSpeed * Time.deltaTime;
            else angle -= angularSpeed * Time.deltaTime;

            line.SetPosition(0, gameObject2.transform.position + new Vector3(0, 1, 0));            
            line.SetPosition(1, gameObject2.transform.position + new Vector3(-length * Mathf.Cos(angle * Mathf.Deg2Rad), 0, length * Mathf.Sin(angle * Mathf.Deg2Rad)) + new Vector3(0, 1, 0));            
        }
    }    

    private void OnDisable()
    {
        turning = false;
    }

    private void OnEnable()
    {
        turning = true;        
    }

    /// <summary>
    /// Takes position of car after car entered turn area
    /// </summary>
    public int SetObj(GameObject x)
    {
        line.SetPosition(1, x.transform.position + new Vector3(0, 1, 0));
        UpdateLength(x);

        if (Vector3.Cross(x.transform.forward,gameObject2.transform.position-x.transform.position).normalized.y < 0)
        {
            direc = -1;
            if (x.transform.forward.x > 0.7f)
            {
                angle = Vector3.Angle(x.transform.forward, gameObject2.transform.position - x.transform.position) - 180;
                angle = 180 - angle % 180;
            }
            else if (x.transform.forward.z > 0.7f)
            {
                angle = Vector3.Angle(x.transform.forward, gameObject2.transform.position - x.transform.position) + 90;
                if (angle > 180) angle = 180 - angle % 180;
            }
        }
        else
        {
            direc = 1;
            if (x.transform.forward.z > 0.7f)
            {
                angle = Vector3.Angle(x.transform.forward, gameObject2.transform.position - x.transform.position) - 90;
                
            }
            else if (x.transform.forward.x < -0.7f)
            {
                angle = Vector3.Angle(x.transform.forward, gameObject2.transform.position - x.transform.position) - 180;                
            }            
        }


        return direc;
    }

    /// <summary>
    /// Returns length of line
    /// </summary>
    public float GetLength() { return length; }

    /// <summary>
    /// Updates length of line
    /// </summary>
    public void UpdateLength(GameObject x) 
    {
        while (gameObject2 == null) ;
        length = (gameObject2.transform.position - x.transform.position).magnitude;
    }
}
