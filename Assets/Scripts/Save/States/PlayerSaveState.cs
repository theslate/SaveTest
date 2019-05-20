using System;

[Serializable]
public class PlayerSaveState
{
    public int Score;
    public TransformSaveState Transform;
    public int Health;
}