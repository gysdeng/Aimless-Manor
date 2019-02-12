using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RoomController : MonoBehaviour {
	RoomScript[] m_RoomList;
	public GameObject m_DoorPrefab;
	public static RoomController m_staticRef;
    bool m_loadingLevel;
    PortalScript m_sourcePortal;

    static System.Random rand;
    public GameObject player;

    void Start () {
		m_staticRef = this;
        m_loadingLevel = false;

        rand = new System.Random();

        for (int n = 0; n < EditorBuildSettings.scenes.Length; n++)
        {
            if (!string.Equals(EditorBuildSettings.scenes[n].path, ""))
            {
                
                int idx = SceneUtility.GetBuildIndexByScenePath(EditorBuildSettings.scenes[n].path);
                Debug.Log(SceneManager.GetSceneByBuildIndex(idx).name);
            }
        }

        LoadRoom(null);

    }

    int levelIndex;
    public void LoadRoom(PortalScript portal) {
        
        if(portal == null)
        {
            levelIndex = 1;
        }
        else
        {
            int[] next_rooms = this.player.GetComponent<PickupObject>().GetNextRooms();
            int next_index = rand.Next(next_rooms.Length);
            levelIndex = next_rooms[next_index];

            // need to work on lock in
            Debug.Log("Possible rooms: " + next_rooms.Length);
            Debug.Log("Next Room is: " + levelIndex);
        }

        // call Scene Manager to load the selected scene
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Additive);

        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelIndex));
        //Debug.Log(SceneManager.GetActiveScene().name);

        // SceneManager.UnloadSceneAsync();

        m_loadingLevel = true;
        m_sourcePortal = portal;
      }

      public void SetupRoom(RoomScript room) {
        room.tag = "Processed";

        if (m_sourcePortal)
        {
          PortalScript destPortal = room.m_Portals[0];
          destPortal.m_LinkedPortal = m_sourcePortal;
          m_sourcePortal.m_LinkedPortal = destPortal;

          room.transform.rotation = Quaternion.LookRotation(
            destPortal.transform.InverseTransformDirection(-m_sourcePortal.transform.forward), 
            destPortal.transform.InverseTransformDirection(m_sourcePortal.transform.up));
          room.transform.position = m_sourcePortal.transform.position + (room.transform.position - destPortal.transform.position);

          m_sourcePortal = null;
        }

        m_loadingLevel = false;
      }
	
}
