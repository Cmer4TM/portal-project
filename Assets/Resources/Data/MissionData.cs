using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Missions/Mission Data")]
public class MissionData : ScriptableObject
{
    public string id;
    [TextArea] public string text;
    public bool showIconNow = true;
}
