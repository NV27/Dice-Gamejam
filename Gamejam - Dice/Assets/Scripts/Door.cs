using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float move_distance = 3.2f;
    public void Open()
    {
        Debug.Log("open door");
        transform.DOMoveY(transform.position.y+move_distance, 2f, false);
    }
}