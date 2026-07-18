using Godot;

public partial class EnemyHealthbar : Line2D {
    private Vector2 fullLifePosition;

    public override void _Ready() {
        fullLifePosition = GetPointPosition(1);
    }

    public void SetHealth(int newHealth) {
        if(newHealth >= 100) {
            GetParent<CanvasItem>().Hide();
            return;
        }
       float newBarPosX = Mathf.Remap(newHealth, 0, 100, 0, fullLifePosition.X);
        SetPointPosition(1, new Vector2(newBarPosX, fullLifePosition.Y));
    }
}
