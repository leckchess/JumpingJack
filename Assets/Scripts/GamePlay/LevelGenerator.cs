using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Rows")]
    [SerializeField] private GameObject _row;
    [SerializeField] private float _gapBetweenRows;
    [SerializeField] private int _numberOfRows;

    [Header("Holes")]
    [SerializeField] private GameObject _hole;
    [SerializeField] private int _InitialNumberOfHoles;
    [SerializeField] private float _timeBetweenGeneratingHoles;

    [Header("NPC")]
    [SerializeField] private GameObject[] _NPCs;
    [SerializeField] private int _InitialNumberOfNPCs;
    [SerializeField] private float _timeBetweenGeneratingNPCs;

    [Header("level defficulty")]
    [SerializeField] int _holesGenerationAcceleration;
    [SerializeField] float _holesGenerationTimeAcceleration;
    [SerializeField] int _NPCGenerationAcceleration;
    [SerializeField] float _NPCsGenerationTimeAcceleration;

    private List<GameObject> _rows;
    private List<GameObject> _poolOfHoles;
    private List<GameObject> _usedHoles;
    private List<GameObject> _PoolOfNPCs;
    private List<GameObject> _usedNPCs;

    private float _minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnGameStarted += OnGameStarted;
        _gameManager.OnStartLevel.AddListener(OnStartLevel);
        _gameManager.OnResetLevel.AddListener(OnResetLevel);
        _gameManager.OnLoadingNextLevel.AddListener(LoadNextLevel);

        _rows = new List<GameObject>();
        _poolOfHoles = new List<GameObject>();
        _usedHoles = new List<GameObject>();

        _PoolOfNPCs = new List<GameObject>();
        _usedNPCs = new List<GameObject>();

        GenerateRows();
    }

    private void OnGameStarted(float minX, float maxX, float minY, float maxY)
    {
        _minXBoundry = minX;
        _maxXBoundry = maxX;

        _minYBoundry = minY;
        _maxYBoundry = maxY;

        GenerateHoles();
        GenerateNPCs();
    }

    void OnStartLevel()
    {
        InitHoles();
        InitNPCs();

        if (_timeBetweenGeneratingHoles > 0)
            InvokeRepeating("EnableRandomHole", _timeBetweenGeneratingHoles, _timeBetweenGeneratingHoles);

        if (_timeBetweenGeneratingNPCs > 0)
            InvokeRepeating("EnableRandomNPC", _timeBetweenGeneratingNPCs, _timeBetweenGeneratingNPCs);
    }

    void LoadNextLevel()
    {
        _InitialNumberOfHoles += _holesGenerationAcceleration;
        _InitialNumberOfNPCs += _NPCGenerationAcceleration;

        _timeBetweenGeneratingHoles += _holesGenerationTimeAcceleration;
        _NPCsGenerationTimeAcceleration += _NPCsGenerationTimeAcceleration;
    }

    private void GenerateRows()
    {
        for (int i = 0; i < _numberOfRows; i++)
        {
            GameObject row = Instantiate(_row);
            row.name = "Row_" + i;

            Vector3 pos = row.transform.position;
            pos.y = i * _gapBetweenRows;
            row.transform.position = pos;

            row.transform.SetParent(transform);
            _rows.Add(row);
        }
    }

    private void GenerateHoles()
    {
        for (int i = 0; i < 20; i++)
        {
            AddHole(i);
        }
    }

    private void AddHole(int index)
    {
        GameObject hole = Instantiate(_hole);
        hole.name = "Hole_" + index;
        InitMovable(hole);
        _poolOfHoles.Add(hole);
    }

    private void InitHoles()
    {
        for (int i = 0; i < _InitialNumberOfHoles; i++)
        {
            EnableRandomHole();
        }
    }

    private void EnableRandomHole()
    {
        if (_poolOfHoles.Count == 0)
        {
            AddHole(_usedHoles.Count);
        }

        int randIndex = Random.Range(0, _poolOfHoles.Count);

        _poolOfHoles[randIndex].SetActive(true);

        _usedHoles.Add(_poolOfHoles[randIndex]);
        _poolOfHoles.RemoveAt(randIndex);
    }

    private void GenerateNPCs()
    {
        for (int i = 0; i < 20; i++)
        {
            AddNPC(i);
        }
    }
    private void AddNPC(int index)
    {
        int prefabRandomIndex = Random.Range(0, _NPCs.Length);

        GameObject npc = Instantiate(_NPCs[prefabRandomIndex]);
        npc.name = "NPC_" + index;
        InitMovable(npc);
        _PoolOfNPCs.Add(npc);
    }

    private void InitNPCs()
    {
        for (int i = 0; i < _InitialNumberOfNPCs; i++)
        {
            EnableRandomNPC();
        }
    }

    private void EnableRandomNPC()
    {
        if (_PoolOfNPCs.Count == 0)
        {
            AddHole(_usedNPCs.Count);
        }

        int randIndex = Random.Range(0, _PoolOfNPCs.Count);

        _PoolOfNPCs[randIndex].SetActive(true);

        _usedNPCs.Add(_PoolOfNPCs[randIndex]);
        _PoolOfNPCs.RemoveAt(randIndex);
    }

    private void InitMovable(GameObject movable)
    {
        int randomIndex = Random.Range(0, movable.CompareTag("NPC") ? _rows.Count - 1 : _rows.Count);
        int randomDir = Random.Range(0, 2);

        Vector3 pos = movable.transform.position;
        pos.y = _rows[randomIndex].transform.position.y;
        pos.x = randomDir == 0 ? _minXBoundry : _maxXBoundry;
        movable.transform.position = pos;

        Vector2 dir = pos.x <= _minXBoundry ? Vector2.right : Vector2.left;
        movable.GetComponent<MovableObjects>().Initialize(_minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry, _gapBetweenRows, dir);

        movable.transform.SetParent(transform);
    }

    private void OnResetLevel()
    {
        for (int i = 0; i < _usedHoles.Count; i++)
        {
            _usedHoles[i].GetComponent<MovableObjects>().OnResetLevel();
            _poolOfHoles.Add(_usedHoles[i]);
        }

        _usedHoles.Clear();

        for (int i = 0; i < _usedNPCs.Count; i++)
        {
            _usedNPCs[i].GetComponent<MovableObjects>().OnResetLevel();
            _PoolOfNPCs.Add(_usedNPCs[i]);
        }

        _usedNPCs.Clear();

        CancelInvoke("EnableRandomHole");
        CancelInvoke("EnableRandomNPC");
    }
}
