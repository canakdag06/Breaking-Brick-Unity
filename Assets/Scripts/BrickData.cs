using UnityEngine;


[CreateAssetMenu(fileName = "NewBrickData", menuName = "Brick/Create BrickData")]
public class BrickData : ScriptableObject
{
    public string brickName;
    public bool isBreakable = true;
    public Sprite[] damageSprites;
    public Color brickColor;
}
