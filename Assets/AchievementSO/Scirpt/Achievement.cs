using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class Achievement : ScriptableObject
{
    public Sprite mySprite;
    public string myName;
    [TextArea(3, 10)]
    public string myDescription;
}
