using UnityEngine;

/// <summary>
/// Puts every chess piece below this object back on its starting square.
/// </summary>
public class ChessBoardResetter : MonoBehaviour
{
    [SerializeField] private float resetDuration = 0.8f;

    private ResettablePose[] pieces;

    private void Awake()
    {
        pieces = GetComponentsInChildren<ResettablePose>();
    }

    public void ResetAllPieces()
    {
        foreach (ResettablePose piece in pieces)
        {
            piece.ResetToStart(resetDuration);
        }
    }
}
