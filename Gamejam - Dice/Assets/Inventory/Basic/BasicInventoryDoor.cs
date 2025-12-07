using UnityEngine;

public class BasicInventoryDoor : MonoBehaviour
{
    public BasicInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        if (inventory.hasKey)
        {
            Destroy(gameObject);
        }
    }
}
