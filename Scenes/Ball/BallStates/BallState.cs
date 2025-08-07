using Godot;
using System;

public partial class BallState : Node
{
    [Signal] public delegate void OnStateTransitionRequestEventHandler(Ball.State newState);

    
    protected readonly float GRAVITY = 10.0f;

    protected Ball _ball;
    protected Player _carrier;
    protected Area2D _playerDetectionArea;
    protected AnimationPlayer _animationPlayer;
    protected Sprite2D _ballSprite;

    public void Setup(Ball contextBall, Area2D contextPlayerDetectionArea, Player contextCarrier, AnimationPlayer contextAnimationPlayer, Sprite2D contextBallSprite)
    {
        _ball = contextBall;
        _playerDetectionArea = contextPlayerDetectionArea;
        _carrier = contextCarrier;
        _animationPlayer = contextAnimationPlayer;
        _ballSprite = contextBallSprite;
    }

    public void SetBallAnimationFromVelocity()
    {
        if (_ball._velocity == Vector2.Zero)
        {
            _animationPlayer.Play("idle");
        }
        else if (_ball._velocity.X > 0)
        {
            _animationPlayer.Play("roll");
            _animationPlayer.Advance(0);
        }
        else
        {
            _animationPlayer.PlayBackwards("roll");
            _animationPlayer.Advance(0);
        }
    }

    public void ProcessGravity(float delta, float bounciness = 0.0f)
    {
        if (_ball._height > 0 || _ball._heightVelocity > 0)
        {
            _ball._heightVelocity -= GRAVITY * delta;
            _ball._height += _ball._heightVelocity;
            if (_ball._height < 0)
            {
                _ball._height = 0;
                if (bounciness > 0 && _ball._heightVelocity < 0)
                {
                    _ball._heightVelocity = -_ball._heightVelocity * bounciness;
                    _ball._velocity *= bounciness;
                }
            }
        }
    }

    public void MoveAndBounce(float delta)
    {
        var collision = _ball.MoveAndCollide(_ball._velocity * delta);
        if (collision != null)
        {
            _ball._velocity = _ball._velocity.Bounce(collision.GetNormal()) * _ball.BOUNCINESS;
            _ball.SwitchState(Ball.State.FREEFORM);
        }
    }
}
