using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIHandler : MonoBehaviour
{
    [Header("InGame")]
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _highScoreText;
    [SerializeField] Transform _livesParent;
    [SerializeField] GameObject _liveSprite;

    [Header("Game Screens")]
    [SerializeField] GameObject _IngameScreen;
    [SerializeField] GameObject _LevelsScreen;

    [Header("Levels Screen")]
    [SerializeField] Button _playButton;
    [SerializeField] TMP_Text _levelText;

    [Header("Animation")]
    [SerializeField] float _animationTime;

    private GameManager _gameManager;
    private SoundManager _soundManager;

    private int _currentLevel = 0;

    private void Start()
    {
        HideScreen(_IngameScreen);
        ShowScreen(_LevelsScreen);

        _playButton.gameObject.SetActive(false);
        _levelText.transform.parent.gameObject.SetActive(false);
        _playButton.onClick.AddListener(OnPlayButtonClicked);

        _gameManager = FindObjectOfType<GameManager>();
        _soundManager = FindObjectOfType<SoundManager>();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        if (!_gameManager)
            _gameManager = FindObjectOfType<GameManager>();

        if (!_soundManager)
            _soundManager = FindObjectOfType<SoundManager>();

        OnLoadNextLevel();
        AddLives(_gameManager.PlayerMaxLives);
        _soundManager.OnStartingGame();

        yield return new WaitForSeconds(_animationTime);

        _gameManager.OnLivesChanged += OnLivesChanged;
        _gameManager.OnScoreChanged += OnScoreChanged;
        _gameManager.OnHighScoreChanged += OnHighScoreChanged;
        _gameManager.OnLoadingNextLevel.AddListener(OnLoadNextLevel);

        _gameManager.StartGame();
        
        _playButton.gameObject.SetActive(true);
        _levelText.transform.parent.gameObject.SetActive(true);
        _soundManager.PlayBGMusic();
    }

    private void OnPlayButtonClicked()
    {
        HideScreen(_LevelsScreen);
        ShowScreen(_IngameScreen);

        _soundManager.OnClick();
        _gameManager.LoadLevel();

    }

    private void OnScoreChanged(int newScore)
    {
        _scoreText.text = newScore.ToString();
    }

    private void OnHighScoreChanged(int newScore)
    {
        _highScoreText.text = newScore.ToString();
    }

    private void OnLivesChanged(int newNumberOflives)
    {
        if(newNumberOflives > _livesParent.childCount)
        {
            int diff = newNumberOflives - _livesParent.childCount;
            AddLives(diff);
        }

        if (newNumberOflives < _livesParent.childCount)
        {
            int diff = _livesParent.childCount - newNumberOflives;
            RemoveLives(diff);
        }
    }

    private void OnLoadNextLevel()
    {
        HideScreen(_IngameScreen);
        ShowScreen(_LevelsScreen);

        _currentLevel++;
        _levelText.text = _currentLevel.ToString();
    }

    private void AddLives(int numberOfLives)
    {
        for(int i=0;i<numberOfLives;i++)
        {
            GameObject live = Instantiate(_liveSprite);
            live.transform.SetParent(_livesParent);
        }
    }

    private void RemoveLives(int numberOfLives)
    {
        for (int i = 0; i < numberOfLives; i++)
        {
            Destroy(_livesParent.transform.GetChild(i).gameObject, 0.1f);
        }
    }

    private void ShowScreen(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideScreen(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
