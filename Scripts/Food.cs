using Godot;

namespace Scripts;
[Tool]
[GlobalClass]
public partial class Food : Node2D
{
    [Signal] public delegate void SelectedEventHandler(Creature creature);

    private float _counter = 0;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("select"))
            if (GlobalPosition.IsEqualApprox(GetGlobalMousePosition()))
                EmitSignal(SignalName.Selected, this);
    }

    public override void _PhysicsProcess(double delta)
    {
        // animate the food to go up and down
        GlobalPosition += new Vector2(0, Mathf.Sin(_counter) * 60) * (float)delta;
        _counter = (_counter + 0.1f) % (Mathf.Pi * 2);
    }
}
