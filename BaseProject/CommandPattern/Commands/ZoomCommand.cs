using BaseProject.GameManagement;

namespace BaseProject.CommandPattern.Commands;

internal class ZoomCommand : Command
{
    private float zoomAmount;
    internal ZoomCommand(float zoomAmount) { 
        this.zoomAmount = zoomAmount;
    }

    public override void Execute()
    {
        GameWorld.Instance.WorldCam.ChangeZoom(zoomAmount); // Only change zoom on worldCam
    }
}
