using UnityEngine;

public class BasicKey : MonoBehaviour
{
    public BasicInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        inventory.hasKey = true;
        Destroy(gameObject);
    }
}
