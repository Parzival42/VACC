using UnityEngine;

public interface PaintDataReceiver
{
    float Radius { get; set; }
    float BrushStrength { get; set; }
    float BrushFalloff { get; set; }
    void SetUVHitPosition(Vector2 uvHit);
}
