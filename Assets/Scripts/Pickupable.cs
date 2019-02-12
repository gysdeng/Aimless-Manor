using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Pickupable : MonoBehaviour
{
    private string status;
    private int start;
    private int end = -1;

    public string End_Room;

    private int prev;
    private int curr;

    //public Dictionary<string, GameObject> Related;
    public GameObject pair;

    // Use this for initialization
    void Start()
    {
        this.status = "unused";

        // TODO
        this.start = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).buildIndex;
        this.prev = this.start;
        this.curr = this.start;

        //for (int n = 0; n < SceneManager.sceneCountInBuildSettings; n++) {
        //    if (string.Equals(SceneManager.GetSceneAt(n).name, this.End_Room)) {
        //        this.end = SceneManager.GetSceneAt(n).buildIndex;
        //        break;
        //    }
        //}

        for (int n = 0; n < EditorBuildSettings.scenes.Length; n++)
        {
            if (EditorBuildSettings.scenes[n].path.Contains(this.End_Room))
            {
                this.end = SceneUtility.GetBuildIndexByScenePath(EditorBuildSettings.scenes[n].path);
            }
        }

        if (this.end == -1 && !string.Equals(this.End_Room, ""))
        {
            Debug.Log(string.Format("ENDING SCENE FOR {0} NOT PRESENT", this.gameObject.name));
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /***
     * Set the status of the Object
     * Can be one of three following:
     * - unused
     * - holding
     * - used
     ***/
    public void SetStatus(string status)
    {
        this.status = status;
    }

    /***
     * Get the status of the Object
     ***/
    public string GetStatus()
    {
        return this.status;
    }

    /***
     * Get the current room that the Object is in
     ***/
    public int GetCurrRoom()
    {
        return this.curr;
    }

    /***
     * Update the current room of the object as it moves with the player
     ***/
    public void ChangeRoom(int room)
    {
        this.prev = this.curr;
        this.curr = room;
    }

    public int GetTargetRoom()
    {
        if (this.pair != null && string.Equals(this.pair.GetComponent<Pickupable>().GetStatus(), "unused"))
        {
            return this.pair.GetComponent<Pickupable>().GetCurrRoom();
            //} else if (this.pair != null && string.Equals(this.pair.GetComponent<Pickupable>().GetStatus(), "used")) {
            //    Debug.Log("Should probably just lead to the ugh IDK man");
            //    return -1;
            //} else if (this.pair != null) {
            //    return this.pair.GetComponent<Pickupable>().GetCurrRoom();
        }
        else
        {
            return this.end;
        }
    }
}
