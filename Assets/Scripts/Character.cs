using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _rotateSpeed = 5f;

    private Animator _animator;
    private CharacterController _controller;
    private PlayerAction _playerAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _playerAction = new PlayerAction();
    }

    private void OnEnable()
    {
        _playerAction.Player.Move.Enable();
    }

    private void OnDisable()
    {
        _playerAction.Player.Move.Disable();
    }

    void Update()
    {
        var move = _playerAction.Player.Move.ReadValue<Vector2>();
        _controller.Move(_speed * Time.deltaTime * new Vector3(move.x, 0, move.y));
        _animator.SetBool("move", move.magnitude > 0);

        // rotate the character to the direction of movement
        if (move.magnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(move.x, 0, move.y)),
                _rotateSpeed * Time.deltaTime
            );
        }
    }
}
