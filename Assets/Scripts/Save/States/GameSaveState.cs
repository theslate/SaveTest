using System;

[Serializable]
public class GameSaveState
{
    public PlayerSaveState PlayerSaveState;
    public int Scene;
    public TransformSaveState CameraSaveState;
    public SpawnerSaveState[] SpawnerSaveState;
}