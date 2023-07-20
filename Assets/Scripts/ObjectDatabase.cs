using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDatabase", menuName = "Game/Object Database")]
public class ObjectDatabase : ScriptableObject
{
    [System.Serializable]
    public class ObjectData
    {
        public string name;
        public Sprite icon;
        public int cost;
        public string description;
        public GameObject prefab;
    }

    public ObjectData[] objects;
}
