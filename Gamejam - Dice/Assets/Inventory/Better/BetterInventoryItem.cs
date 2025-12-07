using UnityEngine;

public class BetterInventoryItem : MonoBehaviour
{
    
    public string itemName;
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponentInParent<BetterInventory>();
        Debug.Log("Key triggered by: " + other.name);

        if (inventory != null)
        {
            inventory.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}
