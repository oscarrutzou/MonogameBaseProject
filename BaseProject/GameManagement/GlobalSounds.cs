using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace BaseProject.GameManagement;

public enum SoundNames
{
    // UI -----------------------------------
  
}

// Oscar
public static class GlobalSounds
{
    #region Properties

    //Sound effects
    public static Dictionary<SoundNames, SoundEffect> Sounds { get; private set; }

    private static Dictionary<SoundNames, List<SoundEffectInstance>> _soundInstancesPool;
    private static int _maxInstanceOfOneSound = 5; // There can only be 2 of the same sounds playing, otherwise it wont play.
    //private static int maxInstanceOfGunSound = 10;

    public static bool PlayMenuMusic { get; set; } = true;

    private static SoundEffect _menuMusic;
    //private static SoundEffect _gameMusic;

    private static SoundEffectInstance _instanceMenuMusic;
    //private static SoundEffectInstance _instanceGameMusic;

    private static Random _rnd = new();
    public static float MusicVolume = 0.25f;
    public static float SfxVolume = 0.5f;
    private static bool _musicCountDown;
    private static bool _sfxCountDown;

    #endregion Properties

    public static void LoadContent()
    {
        _musicCountDown = MusicVolume != 0; // For the method to change up and down on volume
        _sfxCountDown = SfxVolume != 0;

        ContentManager content = GameWorld.Instance.Content;

        _soundInstancesPool = new Dictionary<SoundNames, List<SoundEffectInstance>>();

        // Loads music
        //_menuMusic = content.Load<SoundEffect>("Sound\\Music\\MenuTrack");
        //_gameMusic = content.Load<SoundEffect>("Sound\\Music\\GameTrack");

        // Loads SFX's
        Sounds = new Dictionary<SoundNames, SoundEffect>
        {
            //{SoundNames.ButtonHover, content.Load<SoundEffect>("Sound\\Gui\\HoverButton") },
        };

        //Create sound instances for the sound pool
        foreach (var sound in Sounds)
        {
            _soundInstancesPool[sound.Key] = new List<SoundEffectInstance>();
            int max = _maxInstanceOfOneSound;
            //if (sound.Key == SoundNames.Shot || sound.Key == SoundNames.Shotgun) max = maxInstanceOfGunSound;

            for (int i = 0; i < max; i++)
            {
                _soundInstancesPool[sound.Key].Add(sound.Value.CreateInstance());
            }
        }
    }
    private static float _musicVolumeMakeSmallerBy = 0.3f;
    public static void MusicUpdate()
    {
        if (_menuMusic == null) return;

        if (_instanceMenuMusic == null) // _instanceGameMusic == null || 
        {
            _instanceMenuMusic = _menuMusic.CreateInstance();
            //_instanceGameMusic = _gameMusic.CreateInstance();
        }

        // Make sure the volume is lower that the SFX's, since the SFX are more impactfull.
        _instanceMenuMusic.Volume = MusicVolume * _musicVolumeMakeSmallerBy;
        //_instanceGameMusic.Volume = MusicVolume * _musicVolumeMakeSmallerBy;

        //Check if the music should be playing
        if (!PlayMenuMusic)
            _instanceMenuMusic.Stop(); // Stops it once and does nothing if its already stopped
            //_instanceGameMusic.Stop(); // Stops it once and does nothing if its already stopped
        

        if (_instanceMenuMusic.State == SoundState.Stopped && PlayMenuMusic)
            _instanceMenuMusic.Play(); // Play only plays it once and does nothing if it already plays

        //if (_instanceGameMusic.State == SoundState.Stopped && !PlayMenuMusic)
        //    _instanceGameMusic.Play();// Play only plays it once and does nothing if it already plays
    }

    /// <summary>
    /// Changes the volume of music up or down with 25% each time.
    /// </summary>
    public static void ChangeMusicVolume()
    {
        // If the bool musicCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        MusicVolume = _musicCountDown ? Math.Max(0, MusicVolume - 0.25f) : Math.Min(1, MusicVolume + 0.25f);
        if (MusicVolume == 0 || MusicVolume == 1) _musicCountDown = !_musicCountDown; // Reverse the change direction
    }

    /// <summary>
    /// Changes the volume of sfx up or down with 25% each time.
    /// </summary>
    public static void ChangeSfxVolume()
    {
        // If the bool sfxCountDown is true, we go down in volume towards 0, if its false we can go up until we hit 1.
        SfxVolume = _sfxCountDown ? Math.Max(0, SfxVolume - 0.25f) : Math.Min(1, SfxVolume + 0.25f);
        if (SfxVolume == 0 || SfxVolume == 1) _sfxCountDown = !_sfxCountDown;// Reverse the change direction
    }

    public static bool IsAnySoundPlaying(SoundNames[] soundArray)
    {
        //Check if any sound is playing
        foreach (SoundNames name in soundArray)
        {
            foreach (SoundEffectInstance inst in _soundInstancesPool[name])
            {
                if (inst.State == SoundState.Playing)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public static bool IsAnySoundPlaying(SoundNames soundName)
    {
        //Check if any sound is playing
        SoundEffectInstance instance = GetAvailableInstance(soundName);

        if (instance == null) return false;
        if (instance.State == SoundState.Playing) return true;

        return false;
    }

    public static void ChangeSoundVolumeDistance(Vector2 soundPosition, int minDistance, int maxDistance, float maxSoundVolume, SoundEffectInstance soundEffect, bool checkEffectRunning = true)
    {
        if (checkEffectRunning)
        {
            if (soundEffect == null || soundEffect.State != SoundState.Playing) return;
        }

        Vector2 centerPos = Vector2.Zero; // The position it takes to do the distance check to. 

        float distance = Vector2.Distance(centerPos, soundPosition);

        // Calculate the volume based on distance
        float normalizedDistance = MathHelper.Clamp((distance - (float)minDistance) / ((float)maxDistance - (float)minDistance), 0f, 1f);
        float lerpedVolume = MathHelper.Lerp(maxSoundVolume, 0f, normalizedDistance);

        soundEffect.Volume = SfxVolume * lerpedVolume;
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="soundName">The sound to play</param>
    /// <param name="soundVolume">Can change how loud the sound is</param>
    /// <param name="enablePitch">If it should add a random pitch to the sounds</param>
    public static SoundEffectInstance PlaySound(SoundNames soundName, float soundVolume, bool enablePitch, float minPitch , float maxPitch)
    {
        // Play a sound with an optional random pitch
        float pitch = enablePitch ? GenerateRandomPitch(minPitch, maxPitch) : 0f; // Base pitch is 0f

        // Play a sound
        SoundEffectInstance instance = GetAvailableInstance(soundName);
        if (instance == null)
        {
            // All instances are playing, so stop and reuse the oldest one.
            instance = _soundInstancesPool[soundName][0];
            instance.Stop();
        }

        instance.Pitch = pitch;
        instance.Volume = SfxVolume * soundVolume;
        instance.Play();

        return instance;
    }

    public static SoundEffectInstance PlaySound(SoundNames soundName, int maxAmountPlaying, float soundVolume = 1f, bool enablePitch = false, float minPitch = -0.4f, float maxPitch = 0.2f)
    {
        int index = CountPlayingInstances(soundName);

        if (index >= maxAmountPlaying) return null;

        return PlaySound(soundName, soundVolume, enablePitch, minPitch, maxPitch);
    }

    /// <summary>
    /// Can play a random sound in a array
    /// </summary>
    /// <param name="soundArray">The array of different sound effets that can be played</param>
    /// <param name="maxAmountPlaying">How many of the sounds that can play at once</param>
    /// <param name="soundVolume">Can change how loud the sound is</param>
    /// <param name="enablePitch">If it should add a random pitch to the sounds</param>
    public static SoundEffectInstance PlayRandomizedSound(SoundNames[] soundArray, int maxAmountPlaying, float soundVolume = 1f, bool enablePitch = false, float minPitch = -0.2f, float maxPitch = 0.2f)
    {
        _rnd = new(); // To try and see if it can remove some of the non random
        // Play a random sound from the array
        int soundIndex = _rnd.Next(0, soundArray.Length);

        SoundNames soundName = soundArray[soundIndex];

        int index = CountPlayingInstances(soundName);
        if (index >= maxAmountPlaying) return null;

        return PlaySound(soundName, soundVolume, enablePitch, minPitch, maxPitch);
    }




    // Helper method
    private static SoundEffectInstance GetAvailableInstance(SoundNames soundName)
    {
        foreach (var inst in _soundInstancesPool[soundName])
        {
            if (inst.State != SoundState.Playing)
            {
                return inst;
            }
        }
        return null;
    }

    // Helper method
    private static int CountPlayingInstances(SoundNames soundName)
    {
        int count = 0;
        foreach (SoundEffectInstance inst in _soundInstancesPool[soundName])
        {
            if (inst.State == SoundState.Playing)
            {
                count++;
            }
        }
        return count;
    }

    // Helper method
    private static float GenerateRandomPitch(float minPitch, float maxPitch)
    {
        // Generate a random pitch within the specified range
        float pitch = (float)_rnd.NextDouble() * (maxPitch - minPitch) + minPitch;
        return pitch;
    }

}