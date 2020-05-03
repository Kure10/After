public interface IWorkSource
{
    bool Register(Character who);
    void Unregister(Character who);
    void Update();
}