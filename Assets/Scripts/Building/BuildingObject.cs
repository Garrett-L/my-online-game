using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class BuildingObject : MonoBehaviour
{
    public Vector2[] tilesTaken; // An array of coordinates where the object is built; used so that we can't build objects on top of one another
    public bool overrideControls = false;
    public GameObject objCopy;
    
    private void Awake() {
        if (overrideControls) return;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.1f);
    }
}
