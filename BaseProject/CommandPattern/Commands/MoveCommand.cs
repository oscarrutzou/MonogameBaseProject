using Microsoft.Xna.Framework;
using BaseProject.CompositPattern.Characters;

namespace BaseProject.CommandPattern.Commands;

internal class MoveCommand : Command
{
    private Player player;
    private Vector2 velocity;

    public MoveCommand(Player player, Vector2 velocity)
    {
        this.player = player;
        this.velocity = velocity;
    }
    public override void Execute()
    {
        player.AddInput(velocity);
    }
}
