using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "Dialogue/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    [TextArea(2, 6)]
    public string[] dialogueLines;
    public Sprite portrait;
}
