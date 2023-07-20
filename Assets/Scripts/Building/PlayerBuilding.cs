using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class PlayerBuilding : NetworkBehaviour
{

    [SerializeField] private InputAction buildButton; // "B" button
    [SerializeField] private InputAction placeButton; // Left click button
    [SerializeField] private bool buildingMode = false; // The toggle for whether the player is building at this moment (e.g. press B to enter building mode, press B again to exit)
    public Transform buildingObj; // A version of the object to instantiate that appears on hover (effects only)
    [SerializeField] private Camera cam; // The player's camera
    [SerializeField] private GameObject treePrefab; // The object to instantiate
    public Transform buildingObjParent;
    [SerializeField] private BuildUIAnim UIAnim;
    private Transform objParent; // The parent to the object to instantiate
    private bool canBuild; // Client reference for whether the player can build
    private int buildID;

    void OnEnable() {
        buildButton.Enable(); 
        placeButton.Enable();
    }

    void OnDisable() { 
        buildButton.Disable();
        placeButton.Disable();  
    }

    void Start() {
        objParent = GameObject.FindWithTag("TilemapParent").transform;
        buildButton.started += SwitchBuildingMode;
        placeButton.performed += PlaceObj;
    }

    private void PlaceObj(InputAction.CallbackContext context) // Runs the server rpc to place an object 
    {
        if (!buildingMode || !canBuild || EventSystem.current.IsPointerOverGameObject()) return; // Cancel if not in building mode or can't build

        SpawnObjServerRpc(GetRoundedCursorPos(), buildID); // Call server rpc
    }

    [ServerRpc(RequireOwnership=false)]
    private void SpawnObjServerRpc(Vector3 position, int objID) {
        Debug.Log(objID);
        GameObject objPrefab = buildingObjParent.GetChild(objID).GetComponent<BuildingObject>().objCopy;
        GameObject spawnedObj = Instantiate(objPrefab, position, Quaternion.identity, objParent); // Instantiate an object
        spawnedObj.GetComponent<NetworkObject>().Spawn(true);
        spawnedObj.transform.parent = objParent; // Set parent

        if (!spawnedObj.TryGetComponent<BuildingObject>(out BuildingObject buildingObject)) { // Check if object has BuildingObject component, throw error if not
            spawnedObj.GetComponent<NetworkObject>().Despawn(); // Despawn
            Destroy(spawnedObj); // Destroy
            Debug.LogError("Spawned Object does not have BuildingObject component!"); // Error
            return; // Cancel function
        }
        
        foreach(Vector2 tile in buildingObject.tilesTaken) { // Make sure we can build in this location (no other object taking space)
            if(BuildingClass.unavailableTiles.Contains(tile + new Vector2(position.x, position.y))) {
                Debug.Log("Unable to spawn object, tile taken");
                spawnedObj.GetComponent<NetworkObject>().Despawn();
                Destroy(spawnedObj);
                return;
            }
        }

        Debug.Log("Build succeeded");
        AddTilesTakenClientRpc(buildingObject.tilesTaken, position); // Send to all clients that these tiles are now taken, can't build there
    }

    [ClientRpc]
    private void AddTilesTakenClientRpc(Vector2[] tiles, Vector2 origin)
    {
        foreach (Vector2 tile in tiles)
        {
            BuildingClass.unavailableTiles.Add(tile + origin); // Update the unavailable tiles
        }    
    }

    private void SwitchBuildingMode(InputAction.CallbackContext context) { // Update building mode on button press and show/don't show object
        if (!UIAnim.Animate()) return;
        buildingMode = !buildingMode; 
        Color color;
        if (buildingObj == null) buildingObj = buildingObjParent.GetChild(0);
        if (buildingMode) color = new Color(1,1,1,0.7f); else color = new Color(1,1,1,0);

        foreach(Tilemap renderer in buildingObj.GetComponentsInChildren<Tilemap>()) {
            renderer.color = color;
        }
    }

    void Update()
    {
        if (!buildingMode) return; // Only run update in building mode

        Vector2 roundedPos = GetRoundedCursorPos();

        buildingObj.position = new Vector3(roundedPos.x, roundedPos.y, -5f); // Place the preview of the object on the tiles
        if (!UpdateCanBuild(buildingObj.position)) return; // Figure out whether the player can build an object here or not; returns if cursor is over UI
        if (canBuild) { // Turn red if can't build
            foreach(Tilemap map in buildingObj.GetComponentsInChildren<Tilemap>()) {
                map.color = new Color(1,1,1,0.7f);
            }
        } else {
            foreach(Tilemap map in buildingObj.GetComponentsInChildren<Tilemap>()) {
                map.color = new Color(1,0,0,0.7f);
            }
        }
    }

    Vector2 GetRoundedCursorPos() {
        Vector3 cursorPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // Get cursor position
        Vector3 roundedPos = new Vector3(Mathf.Round(cursorPos.x), Mathf.Round(cursorPos.y), cursorPos.z); // Round
        return new Vector2(roundedPos.x, roundedPos.y);
    }

    bool UpdateCanBuild(Vector2 offset)
        {   
            if (EventSystem.current.IsPointerOverGameObject()) {
                foreach(Tilemap renderer in buildingObj.GetComponentsInChildren<Tilemap>()) {
                    renderer.color = new Color(1,1,1,0);
                }
                return false;
            }
            foreach (Vector2 tile in buildingObj.GetComponent<BuildingObject>().tilesTaken) // Update canBuild based on whether the tiles are taken
            {
                if (BuildingClass.unavailableTiles.Contains(tile + offset))
                {
                    canBuild = false;
                    return true;
                }
            }
            canBuild = true;
            return true;
        }

    public void UpdateBuildID() 
    { 
        buildID = buildingObj.GetSiblingIndex(); 
    }

}
