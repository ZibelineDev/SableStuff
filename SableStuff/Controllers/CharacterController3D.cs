using Godot;

/// <summary>
/// Dependencies: <b>ExportCheck</b>.<br></br>
/// Please download <b>ExportCheck</b> from SableStuff/Utils.
/// </summary>

public partial class CharacterController3D : Node
{
    [Export] private CharacterBody3D _character;
    [Export] private Node3D _head;
    [Export] private bool _canCaptureMouse = true;

    [ExportGroup("Parameters")]
    [Export] private float _gravity = -30;
    [Export] private float _jumpVelocity = 12;
    [Export] private float _movementSpeed = 6;
    [Export] private float _rotationSpeed = 0.005f;

    [ExportGroup("Inputs")]
    [Export] private string _moveForward = "MoveForward";
    [Export] private string _moveBackward = "MoveBackward";
    [Export] private string _moveLeft = "MoveLeft";
    [Export] private string _moveRight = "MoveRight";
    [Export] private string _jump = "Jump";

    private float _lookRotationX;
    private float _lookRotationY;


    public override void _Ready()
    {
        ExportCheck.IsNull(this, [_character, _head]);

        _lookRotationX = _head.Rotation.X;
        _lookRotationY = _character.Rotation.Y;
    }


    public override void _PhysicsProcess(double delta)
    {
        ApplyDirection();
        ApplyGravity(delta);
        ApplyJump();
        _character.MoveAndSlide();
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion mouseMotion)
        {
            ApplyRotations(mouseMotion.Relative);
        }

        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && _canCaptureMouse)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }


    private void ApplyDirection()
    {
        var characterVelocity = _character.Velocity;

        var inputDirection = Input.GetVector(_moveLeft, _moveRight, _moveForward, _moveBackward);
        var moveDirection = _character.Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y);
        moveDirection = moveDirection.Normalized();

        if (moveDirection != Vector3.Zero)
        {
            characterVelocity.X = moveDirection.X * _movementSpeed;
            characterVelocity.Z = moveDirection.Z * _movementSpeed;
        }
        else
        {
            characterVelocity.X = 0;
            characterVelocity.Z = 0;
        }

        _character.Velocity = characterVelocity;
    }


    private void ApplyGravity(double delta)
    {
        var characterVelocity = _character.Velocity;

        if (!_character.IsOnFloor())
        {
            characterVelocity.Y += _gravity * (float)delta;
        }
        else
        {
            characterVelocity.Y = 0;
        }

        _character.Velocity = characterVelocity;
    }


    private void ApplyJump()
    {
        var characterVelocity = _character.Velocity;

        if (Input.IsActionJustPressed("Jump") && _character.IsOnFloor())
        {
            characterVelocity.Y = _jumpVelocity;
        }

        _character.Velocity = characterVelocity;
    }


    private void ApplyRotations(Vector2 mouseMotionRelative)
    {
        _lookRotationX -= mouseMotionRelative.Y * _rotationSpeed;
        _lookRotationY -= mouseMotionRelative.X * _rotationSpeed;

        _lookRotationX = Mathf.Clamp(_lookRotationX, Mathf.DegToRad(-85), Mathf.DegToRad(85));

        _character.Transform = new(Basis.Identity, _character.Transform.Origin);
        _head.Transform = new(Basis.Identity, _head.Transform.Origin);

        _character.RotateY(_lookRotationY);
        _head.RotateX(_lookRotationX);
    }

    private void CheckInputs()
    {
        if (!InputMap.HasAction(_moveLeft))
        {
            GD.PushError("Move left is not mapped.");
        }
        if (!InputMap.HasAction(_moveRight))
        {
            GD.PushError("Move right is not mapped.");
        }
        if (!InputMap.HasAction(_moveForward))
        {
            GD.PushError("Move forward is not mapped.");
        }
        if (!InputMap.HasAction(_moveBackward))
        {
            GD.PushError("Move backward is not mapped.");
        }
        if (!InputMap.HasAction(_jump))
        {
            GD.PushError("Jump is not mapped.");
        }
    }
}
