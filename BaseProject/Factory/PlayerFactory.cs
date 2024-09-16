using BaseProject.CompositPattern;
using BaseProject.CompositPattern.Characters;
using BaseProject.GameManagement;
using Microsoft.Xna.Framework;

namespace BaseProject.Factory;

public static class PlayerFactory 
{
    public static GameObject Create()
    {
        GameObject playerGo = new GameObject();
        playerGo.Type = GameObjectTypes.Player;
        playerGo.AddComponent<SpriteRenderer>();
        playerGo.AddComponent<Animator>();
        playerGo.AddComponent<Collider>();
        playerGo.AddComponent<Player>();

        return playerGo;
    }
}
