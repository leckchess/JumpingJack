using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _jumpSpeed;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _fallingTime;

    [SerializeField] LayerMask _mask;

    PlayerAnimation _playerAnimationHandler;

    bool _canMove = false, _protected = false, _isJumping = false, _isFalling = false;
    private Vector2 _InitialPosition;

    private float _minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry;

    private RaycastHit2D _hit;
    private Vector2 _playerCenter;

    private GameManager _gameManager;
    private SoundManager _soundManager;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _soundManager = FindObjectOfType<SoundManager>();

        _gameManager.OnGameStarted += OnGameStarted;
        _gameManager.OnStartLevel.AddListener(OnLevelStarted);
        _gameManager.OnResetLevel.AddListener(OnResetLevel);
    }

    private void OnGameStarted(float minX, float maxX, float minY, float maxY)
    {
        _playerAnimationHandler = GetComponent<PlayerAnimation>();
        _InitialPosition = transform.position;

        _minXBoundry = minX;
        _maxXBoundry = maxX;

        _minYBoundry = minY;
        _maxYBoundry = maxY;
    }

    private void OnLevelStarted()
    {
        _canMove = true;
    }

    void Update()
    {
        if (!_canMove)
            return;

        _protected = false;
        _playerAnimationHandler.UpdateAnimationState(AnimationState.idle);

        _playerCenter = transform.position;
        _playerCenter.y += 0.5f;

        if (_hit = Physics2D.Raycast(_playerCenter, transform.up, 1, _mask, 0))
        {
            if (_hit.collider.CompareTag("Hole"))
            {
                _protected = true;
            }
        }
        if (_hit = Physics2D.Raycast(_playerCenter, -transform.up, 1, _mask, 0))
        {
            if (_hit.collider.CompareTag("Hole") && !_isJumping)
            {
                StartCoroutine(Fall(transform.position.y - _jumpHeight));
                StartCoroutine(OnFalling());
            }
        }

        #region Movement
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.position += Vector3.left * _speed * Time.deltaTime;
            _playerAnimationHandler.UpdateAnimationState(AnimationState.left);

            Vector2 pos = transform.position;
            if (transform.position.x < _minXBoundry) // change dynamic
                pos.x = _maxXBoundry;

            transform.position = pos;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.position += Vector3.right * _speed * Time.deltaTime;
            _playerAnimationHandler.UpdateAnimationState(AnimationState.right);

            Vector2 pos = transform.position;
            if (transform.position.x > _maxXBoundry) // change dynamic
                pos.x = _minXBoundry;

            transform.position = pos;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (_protected)
            {
                StartCoroutine(Jump(transform.localPosition.y + _jumpHeight));
                _playerAnimationHandler.UpdateAnimationState(AnimationState.jump);
            }
            else
            {
                if (_hit = Physics2D.Raycast(_playerCenter, transform.up, 1, _mask, 0))
                    if (_hit.collider.CompareTag("Floor"))
                        StartCoroutine(OnFalling());
            }
        }
        #endregion Movement

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
            StartCoroutine(OnFalling());
        if (collision.CompareTag("Ground") && _isFalling)
            _gameManager.PlayerHitTheGround();
        if (collision.CompareTag("TOP") && _isFalling)
            _gameManager.LoadNextLevel();
    }
    IEnumerator Jump(float newy)
    {
        if (_isJumping)
            yield break;

        _isJumping = true;

        _soundManager.OnJumping();
        while (transform.position.y < newy)
        {
            transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }

        _gameManager.PlayerJumpedToSecondFloor();

        _isJumping = false;
    }

    IEnumerator Fall(float newy)
    {
        if (_isJumping || _isFalling)
            yield break;

        _soundManager.OnFalling();

        while (transform.position.y > newy)
        {
            transform.position -= Vector3.up * _jumpSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator OnFalling()
    {
        _canMove = false;
        _isFalling = true;
        _playerAnimationHandler.UpdateAnimationState(AnimationState.fall);
        yield return new WaitForSeconds(_fallingTime);
        _playerAnimationHandler.UpdateAnimationState(AnimationState.idle);
        _isFalling = false;
        _canMove = true;
    }

    void OnResetLevel()
    {
        _canMove = false;
        transform.position = _InitialPosition;
    }
}
