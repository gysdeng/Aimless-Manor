using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour {
	public PortalScript[] m_Portals;

  public void Start() {
    RoomController.m_staticRef.SetupRoom(this);
  }

  public void OnTriggerExit(Collider other) {
        if(other.name == "Player")
        {
            /*
             * This will be needed when we have more rooms but for now its
             * fine       
             */
            //SceneManager.UnloadSceneAsync(this.gameObject.scene);
            Destroy(gameObject);
            Destroy(this);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            int room = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).buildIndex;
            other.GetComponent<PickupObject>().ChangeInventoryRoom(room);
        }
    }
}
