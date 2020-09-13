using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public CarMovement carMovement;

    public void OnPointerDown(PointerEventData eventData)
    {
        carMovement.SetPointer(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carMovement.SetPointer(false);
    }
}
