using System;
using UnityEngine;

[Serializable]
public class EnemySaveState
{
    public int Health;
    public float AttackTime;
    public TransformSaveState TransformSaveState;
}