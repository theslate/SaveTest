using System;

[Serializable]
public class SpawnerSaveState
{
    public string Name;
    public float NextSpawnTime;
    public EnemySaveState[] Enemies;
}