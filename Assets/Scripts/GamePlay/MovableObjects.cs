using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableObjects : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector2 _originalPosition;
    private Rigidbody2D _rigidbody;
    private float _levelHeight;
    private Vector2 _dir;

    private float _minXBoundry, _maxXBoundry, _minYBoundry, _maxYBoundry;
    
    public void Initialize(float minX, float maxX, float minY, float maxY, float levelHeight, Vector2 dir)
    {
        _originalPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();

        _minXBoundry = minX;
        _maxXBoundry = maxX;

        _minYBoundry = minY;
        _maxYBoundry = maxY;

        _levelHeight = levelHeight;
        _dir = dir;
    }

    private void Update()
    {
        if (_rigidbody.velocity.x == 0)
            return;

        if (_rigidbody.velocity.x > 0 && transform.position.x > _maxXBoundry)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y + _levelHeight;
            pos.x = _minXBoundry;
            bool overBoundry = gameObject.CompareTag("NPC") ? pos.y >= _maxYBoundry - _levelHeight - 0.1f : pos.y >= _maxYBoundry - 0.1f;
            if (overBoundry)
                pos.y = _minYBoundry + _levelHeight;

            transform.position = pos;
        }
        else if (_rigidbody.velocity.x < 0 && transform.position.x < _minXBoundry)
        {
            Vector2 pos = transform.position;
            pos.y = transform.position.y - _levelHeight;
            pos.x = _maxXBoundry;
            if (pos.y <= _minYBoundry + 0.1f)
                pos.y = gameObject.CompareTag("NPC")? _maxYBoundry - (2 * _levelHeight) : _maxYBoundry - _levelHeight;

            transform.position = pos;
        }
    }

    private void OnEnable()
    {
        Move();
    }

    public void Move()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = _dir * _speed;
    }

    public void OnResetLevel()
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.isKinematic = true;
        transform.position = _originalPosition;
        gameObject.SetActive(false);
    }
}
