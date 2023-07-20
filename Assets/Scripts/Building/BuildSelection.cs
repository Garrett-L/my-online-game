using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildSelection : MonoBehaviour
{

    [SerializeField] private ObjectDatabase database;
    [SerializeField] private Image icon;
    public PlayerBuilding playerRef;
    private BuildingObject buildingObj;

    // Start is called before the first frame update
    void Start()
    {
        ObjectDatabase.ObjectData obj = database.objects[transform.GetSiblingIndex()];
        icon.sprite = obj.icon;
        buildingObj = Instantiate(obj.prefab, playerRef.buildingObjParent).GetComponent<BuildingObject>();
        buildingObj.overrideControls = true;
        buildingObj.objCopy = obj.prefab;
        foreach (Tilemap renderer in buildingObj.GetComponentsInChildren<Tilemap>()) {
            renderer.color = new Color(1,1,1,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelected() {
        playerRef.buildingObj = buildingObj.transform;
        playerRef.UpdateBuildID();
    }
}
