using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAnimationSet
{
    public EnemyType enemyType;
    public Sprite[] animationSprites;
}

[CreateAssetMenu(fileName = "EnemyAnimations", menuName = "Game/Enemy Animations")]
public class EnemyAnimations : ScriptableObject
{
    public List<EnemyAnimationSet> sprites;
}