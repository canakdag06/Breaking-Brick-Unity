using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBrickData", menuName = "Brick/Create BrickData")]
public class BrickData : ScriptableObject
{
    [System.Serializable]
    public class BrickColorData
    {
        public BrickColor color;
        public Sprite defaultSprite;
        public Sprite[] damageSprites;
        public bool isBreakable;
    }

    public List<BrickColorData> bricks;
}

public enum BrickColor
{
    Blue,
    Brown, // unbreakable
    Green,
    Orange,
    Purple,
    Red,
    Stone0, // unbreakable
    Stone1, // unbreakable
    Wood, // unbreakable
    Yellow,
}
