using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    
    public void Open()
    {
        Debug.Log("open door");
        transform.DOMoveY(transform.position.y+3.2f, 2f, false);
    }
}