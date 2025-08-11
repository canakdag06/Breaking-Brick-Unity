using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAnimationSet
{
    public EnemyType enemyType;
    public Sprite[] animationSprites;
    public float animationFrameRate = 0.1f;
}

[CreateAssetMenu(fileName = "AllEnemyAnimations", menuName = "Game/All Enemy Animations")]
public class AllEnemyAnimations : ScriptableObject
{
    public List<EnemyAnimationSet> enemyAnimations;
}