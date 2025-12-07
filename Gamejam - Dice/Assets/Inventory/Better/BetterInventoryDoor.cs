using UnityEngine;

public class BetterInventoryDoor : MonoBehaviour
{
    public string requiredItem = "Key";

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<BetterInventory>();

        if (inventory != null && inventory.HasItem(requiredItem))
        {
            Destroy(gameObject);
        }
    }
}
