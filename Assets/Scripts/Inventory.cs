using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class InventoryItem
{
    public enum Location
    {
        LEFT=0,
        RIGHT=1
    };

    public GameObject item;
    public Location gameObjectLocationInInventory;

    public InventoryItem(GameObject inventory_item, Location itemLocation)
    {
        this.item = inventory_item;
        this.gameObjectLocationInInventory = itemLocation;
    }
}

public class Inventory
{

    private static int inventory_size = 2;
    private InventoryItem[] inventory = new InventoryItem[inventory_size];

    private Scene[] scenes;

    public Inventory()
    {
        for (int x = 0; x < inventory_size; x++)
        {
            this.inventory[x] = null;
        }

        this.scenes = new Scene[SceneManager.sceneCountInBuildSettings];

        for (int n = 0; n < EditorBuildSettings.scenes.Length; n++)
        {
            if (!string.Equals(EditorBuildSettings.scenes[n].path, ""))
            {
                int idx = SceneUtility.GetBuildIndexByScenePath(EditorBuildSettings.scenes[n].path);
                this.scenes[idx] = SceneManager.GetSceneByBuildIndex(idx);
            }
        }

    }

    private bool IsObjectAlreadyInInventory(GameObject obj)
    {
        if (this.inventory[0] != null && this.inventory[0].item != null && this.inventory[0].item == obj)
        {
            return true;
        }
        if (this.inventory[1] != null && this.inventory[1].item != null && this.inventory[1].item == obj)
        {
            return true;
        }
        return false;
    }

    public void AddGameObjectToInventory(GameObject obj)
    {
        if (this.IsObjectAlreadyInInventory(obj))
        {
            return;
        }

        int index = this.GetNextAvailableOpenIndex();
        if (index < 0)
        {
            return;
        }

        if(index == 0)
        {
            this.inventory[index] = new InventoryItem(obj, InventoryItem.Location.LEFT);
        }
        else if (index == 1)
        {
            this.inventory[index] = new InventoryItem(obj, InventoryItem.Location.RIGHT);
        }

        obj.GetComponent<Pickupable>().SetStatus("holding");
    }

    public void RemoveGameObjectFromInventory(GameObject obj)
    {
        for (int ind = 0; ind < inventory.Length; ind++)
        {
            if (inventory[ind] != null && inventory[ind].item == obj)
            {
                this.inventory[ind] = null;
            }
        }

        obj.GetComponent<Pickupable>().SetStatus("unused");
    }

    public bool IsEmpty()
    {
        return this.inventory[0] == null && this.inventory[1] == null;
    }

    public InventoryItem[] GetInventory()
    {
        return this.inventory;
    }

    public bool IsInventoryFull()
    {
        return this.inventory[0] != null && this.inventory[1] != null;
    }

    public GameObject FindFirstObject()
    {
        if (this.inventory[0] != null)
        {
            return this.inventory[0].item;
        } else if (this.inventory[1] != null)
        {
            return this.inventory[1].item;
        }
        return null;
    }

    public InventoryItem FindFirstInventoryItem()
    {
        if (this.inventory[0] != null)
        {
            return this.inventory[0];
        }
        else if (this.inventory[0] != null)
        {
            return this.inventory[1];
        }
        return null;
    }

    /*
     * Given an index find the next available object
     * 
     * Used for filtering inventory
     */
    public InventoryItem FindNextObject(GameObject obj)
    {

        int index = GetIndexOfCurrentGameObject(obj);
        InventoryItem firstObj = this.FindFirstInventoryItem();
        if (index < 0)
        {
            return null;
        } else if (!this.IsInventoryFull() && firstObj != null)
        {
            //If its not full that means that 
            return firstObj;
        }

        if (index == 0 && this.inventory[1] != null)
        {
            return this.inventory[1];
        } else if (index == 1 && this.inventory[0] != null)
        {
            return this.inventory[0];
        } else
        {
            return null;
        }
    }

    private int GetIndexOfCurrentGameObject(GameObject obj)
    {
        int index = 0;
        foreach (InventoryItem i in this.inventory)
        {
            if (obj == i.item)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    /*
     * Find the next available open space in the inventory
     * 
     * Returns:   
     * If there is space in the inventory then return the index
     * Else return -1 if there is no space
     */
    private int GetNextAvailableOpenIndex()
    {
        if (this.inventory[0] == null)
        {
            return 0;
        }
        else if (this.inventory[1] == null)
        {
            return 1;
        }
        return -1;
    }

    public void ChangeItemsRoom(int room)
    {
        if (this.inventory[0] != null && this.inventory[0].item != null)
        {
            this.inventory[0].item.GetComponent<Pickupable>().ChangeRoom(room);
        }
        if (this.inventory[1] != null && this.inventory[1].item != null)
        {
            this.inventory[1].item.GetComponent<Pickupable>().ChangeRoom(room);
        }
    }

    public int[] NextTargetRoom(int curr)
    {
        List<int> targets = new List<int>();

        // if the left and right hand are pairs, get something else
        // implement pickup!!!

        // get target room in left hand
        if (this.inventory[0] != null && this.inventory[0].item != null)
        {
            targets.Add(this.inventory[0].item.GetComponent<Pickupable>().GetTargetRoom());
        }
        // get target room in right hand
        if (this.inventory[1] != null && this.inventory[1].item != null)
        {
            targets.Add(this.inventory[1].item.GetComponent<Pickupable>().GetTargetRoom());
        }

        if (targets.Count == 0)
        {
            Debug.Log("Current room is: " + curr);
            // add random rooms
            for (int n = 1; n < this.scenes.Length; n++)
            {
                if (n != curr)
                {
                    targets.Add(n);
                    Debug.Log("Added: " + n + " " + this.scenes[n].name);
                }
            }
        }

        return targets.ToArray();
    }
}
