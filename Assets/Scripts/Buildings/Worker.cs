

public class Worker
{
    public Character character;
    public WorkerState state;
    public float time;
}
public enum WorkerState
{
    init,
    wait,
    empty,
    pickup,
    full,
    drop,
    move,
    building
}