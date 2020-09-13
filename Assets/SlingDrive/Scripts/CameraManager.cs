using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Vector3 distance;

    private void Update()
    {
        transform.position = player.position - distance;
    }
}
