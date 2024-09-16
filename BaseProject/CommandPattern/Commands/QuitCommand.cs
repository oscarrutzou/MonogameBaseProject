using BaseProject.GameManagement;

namespace BaseProject.CommandPattern.Commands;

internal class QuitCommand : Command
{
    public override void Execute()
    {
        GameWorld.Instance.Exit();
    }
}
