using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _playerMaxLives;

    [HideInInspector] public Action<float,float,float,float> OnGameStarted;
    [HideInInspector] public UnityEvent OnStartLevel;
    [HideInInspector] public UnityEvent OnResetLevel;
    [HideInInspector] public UnityEvent OnLoadingNextLevel;

    [HideInInspector] public Action<int> OnScoreChanged;
    [HideInInspector] public Action<int> OnHighScoreChanged;
    [HideInInspector] public Action<int> OnLivesChanged;
    


    private float _minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry;
    private PlayerData _playerData;

    private SoundManager _soundManager;

    public int PlayerMaxLives { get { return _playerMaxLives; } }
    
    private void Start()
    {
        _minXBoundry = transform.Find("LeftBoundy").localPosition.x;
        _maxXBoundry = transform.Find("RightBoundy").localPosition.x;

        _minYBoundry = transform.Find("RightBoundy").localPosition.y;
        _maxYBoundry = transform.Find("LeftBoundy").localPosition.y;

        _soundManager = FindObjectOfType<SoundManager>();
    }

    public void StartGame()
    {
        OnGameStarted?.Invoke(_minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry);
        _playerData.Lives = _playerMaxLives;
        OnHighScoreChanged?.Invoke(_playerData.HighScore);
    }

    public void LoadLevel()
    {
        _soundManager.PlayBGMusic();
        OnStartLevel?.Invoke();
    }

    public void LoadNextLevel()
    {
        _soundManager.OnLevelEnd(true);
        ResetLevel();
        OnLoadingNextLevel?.Invoke();
    }

    public void PlayerHitTheGround()
    {
        _playerData.Lives--;
        OnLivesChanged?.Invoke(_playerData.Lives);

        if(_playerData.Lives == 0)
        {
            _soundManager.OnLevelEnd(false);
            ResetLevel();
            _playerData.Score = 0;
            _playerData.Lives = _playerMaxLives;
        }
    }

    public void PlayerJumpedToSecondFloor()
    {
        if (_playerData.Lives == 0)
            return;

        _playerData.Score += 10;
        OnScoreChanged?.Invoke(_playerData.Score);

        if(_playerData.Score == _playerData.HighScore)
            OnHighScoreChanged?.Invoke(_playerData.HighScore);
    }

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
    }
}

struct PlayerData
{
    int _score;
    int _highScore;

    int _lives;

    public int Score
    {
        get { return _score; }
        set 
        {
            _score = value;
            if (_score > _highScore)
            {
                _highScore = _score;
                PlayerPrefs.SetInt("HighScore", _highScore);
            }
        }
    }

    public int HighScore { get { return _highScore = PlayerPrefs.GetInt("HighScore"); } }

    public int Lives 
    {
        get { return _lives; }
        set { _lives = value; }
    }
}
