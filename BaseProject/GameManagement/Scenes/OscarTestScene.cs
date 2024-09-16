using BaseProject.CommandPattern;
using BaseProject.CommandPattern.Commands;
using BaseProject.CompositPattern;
using BaseProject.CompositPattern.Characters;
using BaseProject.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BaseProject.ObserverPattern;
using BaseProject.ComponentPattern.Particles;
using BaseProject.ComponentPattern.Particles.BirthModifiers;
using BaseProject.ComponentPattern.Particles.Modifiers;
using System.Collections.Generic;


namespace BaseProject.GameManagement.Scenes
{
    public class OscarTestScene : Scene, IObserver 
    {
        private GameObject _playerGo;

        private Vector2 _playerPos;
        private Player _player;

        public override void Initialize()
        {
            MakePlayer();
            TestEmitter();
            SetCommands();
        }
        ParticleEmitter _emitter;
        private void TestEmitter()
        {
            GameObject go = EmitterFactory.CreateParticleEmitter("Test", Vector2.Zero, new Interval(100, 400), new Interval(-MathHelper.Pi, MathHelper.Pi), 200, new Interval(2000, 3000), 1000);
            _emitter = go.GetComponent<ParticleEmitter>();
            _emitter.AddBirthModifier(new TextureBirthModifier(TextureNames.Pixel));
            //emitter.AddModifier(new ColorRangeModifier(true));
            //emitter.AddBirthModifier(new ColorBirthModifier(Color.Black));
            _emitter.AddModifier(new ColorRangeModifier(new Color[] {Color.Aqua, Color.Black, Color.Transparent}));
            _emitter.AddModifier(new ScaleModifier(10, 1));
            _emitter.LayerName = LayerDepthTypes.Default;
            _emitter.StartEmitter();
            GameWorld.Instance.Instantiate(go);
        }

        private void MakePlayer()
        {
            _playerGo = PlayerFactory.Create();
            GameWorld.Instance.Instantiate(_playerGo);
        }

        private void SetCommands()
        {
            _player = _playerGo.GetComponent<Player>();
            _player.Attach(this);
            InputHandler.Instance.AddKeyUpdateCommand(Keys.D, new MoveCommand(_player, new Vector2(1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.A, new MoveCommand(_player, new Vector2(-1, 0)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.W, new MoveCommand(_player, new Vector2(0, -1)));
            InputHandler.Instance.AddKeyUpdateCommand(Keys.S, new MoveCommand(_player, new Vector2(0, 1)));
        }

        public override void Update()
        {
            base.Update();
        }

        Vector2 offSet = new Vector2(0, 30);
        public override void DrawOnScreen(SpriteBatch spriteBatch)
        {
            Vector2 pos = GameWorld.Instance.UiCam.TopLeft;
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"PlayerPos {_playerPos}", pos, Color.Black);
            pos += offSet;

            int amount = 0;
            foreach (List<GameObject> obj in SceneData.Instance.GameObjectLists.Values)
            {
                amount += obj.Count;
            }
            spriteBatch.DrawString(GlobalTextures.DefaultFont, $"GameObjects in scene {amount}", pos, Color.Black);

            base.DrawOnScreen(spriteBatch);
        }

        public void UpdateObserver()
        {
            _playerPos = _player.GameObject.Transform.Position;
        }
    }
}
