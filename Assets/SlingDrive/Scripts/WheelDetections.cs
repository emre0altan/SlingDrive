using UnityEngine;

public class WheelDetections : MonoBehaviour
{
    public PlayerCar playerCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NormalTurn"))
        {
            playerCar.carMovement.StopCar();
            playerCar.retryButton.SetActive(true);
        }
    }
}
