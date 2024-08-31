using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _backgroundMusic;

    [SerializeField]
    private AudioClip _buttonClickClip;

    [SerializeField]
    private AudioClip _winningClip;

    [SerializeField]
    private AudioClip _losingClip;

    [SerializeField]
    private AudioClip _jumpingClip;

    [SerializeField]
    private AudioClip _fallingClip;

    [SerializeField]
    private AudioClip _stepsClip;

    private AudioSource _bgAudioSource;
    private AudioSource _effectsAudioSource;


    private void Start()
    {
        CreateAudioSources();
    }

    private void CreateAudioSources()
    {
        _bgAudioSource = gameObject.AddComponent<AudioSource>();
        _effectsAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBGMusic()
    {
        if (_bgAudioSource.isPlaying)
            return;

        _effectsAudioSource.Stop();

        _bgAudioSource.clip = _backgroundMusic;
        _bgAudioSource.volume = 0.7f;
        _bgAudioSource.loop = true;
        _bgAudioSource.Play();
    }

    public void OnLevelEnd(bool Iswin)
    {
        _effectsAudioSource.volume = 0.7f;
        _bgAudioSource.Stop();

        if (Iswin)
        {
            _effectsAudioSource.PlayOneShot(_winningClip);
        }
        else
        {
            _effectsAudioSource.PlayOneShot(_losingClip);
        }
    }

    public void OnClick()
    {
        _effectsAudioSource.volume = 1f;
        _effectsAudioSource.PlayOneShot(_buttonClickClip);
    }

    public void OnStartingGame()
    {
        if (!_effectsAudioSource)
            CreateAudioSources();

        _effectsAudioSource.volume = 1f;
        _effectsAudioSource.PlayOneShot(_stepsClip);
    }

    public void OnFalling()
    {
        _effectsAudioSource.volume = 1f;
        _effectsAudioSource.PlayOneShot(_fallingClip);
    } 
    
    public void OnJumping()
    {
        _effectsAudioSource.volume = 1f;
        _effectsAudioSource.PlayOneShot(_jumpingClip);
    }

    public void StopEffectsSounds()
    {
        _effectsAudioSource.Stop();
    }
}
