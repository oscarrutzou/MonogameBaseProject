using BaseProject.CompositPattern;

namespace BaseProject.ObserverPattern;

public interface ILayerDepthObserver
{
    void OnLayerDepthChanged(SpriteRenderer spriteRenderer);
}
