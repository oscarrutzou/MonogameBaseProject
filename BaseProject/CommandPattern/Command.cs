
namespace BaseProject.CommandPattern;

public abstract class Command
{
    public virtual void Update() { }
    public abstract void Execute();
}
