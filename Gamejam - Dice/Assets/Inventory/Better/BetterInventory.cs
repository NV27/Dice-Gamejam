using System.Collections.Generic;
using UnityEngine;

public class BetterInventory : MonoBehaviour
{
    private List<string> inventory = new List<string>();

    public void AddItem(string name)
    {
        inventory.Add(name);
    }

    public bool HasItem(string name)
    {
        return inventory.Contains(name);
    }
}
