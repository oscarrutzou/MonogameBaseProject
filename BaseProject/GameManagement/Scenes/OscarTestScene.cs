using BaseProject.CommandPattern;
using BaseProject.CommandPattern.Commands;
using BaseProject.CompositPattern;
using BaseProject.CompositPattern.Characters;
using BaseProject.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BaseProject.ObserverPattern;
namespace BaseProject.GameManagement.Scenes
{
    public class OscarTestScene : Scene, IObserver 
    {
        private PlayerFactory playerFactory;
        private GameObject playerGo;

        private Vector2 velocity;

        public override void Initialize()
        {

            MakePlayer();

            SetCommands();
        }

        
        private void MakePlayer()
        {
            //playerFactory = new PlayerFactory();
            //player = playerFactory.Create().GetComponent<Player>(); //Creates the player and takes the player component

            playerFactory = new PlayerFactory();
            playerGo = playerFactory.Create();

            GameWorld.Instance.Instantiate(playerGo);
        }
        Player player;
        private void SetCommands()
        {
            player = playerGo.GetComponent<Player>();
            player.Attach(this);
            InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(player, new Vector2(1, 0)));
            InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(player, new Vector2(-1, 0)));
            InputHandler.Instance.AddUpdateCommand(Keys.W, new MoveCommand(player, new Vector2(0, -1)));
            InputHandler.Instance.AddUpdateCommand(Keys.S, new MoveCommand(player, new Vector2(0, 1)));
        }

        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public override void DrawInWorld(SpriteBatch spriteBatch)
        {


            base.DrawInWorld(spriteBatch);
        }

        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(GlobalTextures.DefaultFont, velocity.ToString(), Vector2.Zero, Color.White);

            base.DrawOnScreen(spriteBatch);
        }

        public void UpdateObserver()
        {
            velocity = player.targetVelocity;
        }
    }
}
