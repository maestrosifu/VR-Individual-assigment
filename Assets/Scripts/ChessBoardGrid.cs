using UnityEngine;

/// <summary>
/// Describes the 8x8 grid of the giant chess board so pieces can find the square they are dropped on.
/// The board's pivot must be in the centre of the board.
/// </summary>
public class ChessBoardGrid : MonoBehaviour
{
    [SerializeField] private float squareSize = 0.4f;
    [SerializeField] private int squaresPerSide = 8;
    [SerializeField] private float surfaceHeight = 0.014f;
    [Tooltip("Pieces released higher than this above the board are not snapped.")]
    [SerializeField] private float maxSnapHeight = 1.5f;

    public bool TryGetSquareCenter(Vector3 worldPosition, out Vector3 squareCenter)
    {
        squareCenter = Vector3.zero;

        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        float halfBoard = squareSize * squaresPerSide * 0.5f;

        bool isAboveBoard = Mathf.Abs(localPosition.x) <= halfBoard && Mathf.Abs(localPosition.z) <= halfBoard;
        if (!isAboveBoard || localPosition.y > maxSnapHeight)
        {
            return false;
        }

        int file = Mathf.Clamp(Mathf.FloorToInt((localPosition.x + halfBoard) / squareSize), 0, squaresPerSide - 1);
        int rank = Mathf.Clamp(Mathf.FloorToInt((localPosition.z + halfBoard) / squareSize), 0, squaresPerSide - 1);

        var localCenter = new Vector3(
            -halfBoard + (file + 0.5f) * squareSize,
            surfaceHeight,
            -halfBoard + (rank + 0.5f) * squareSize);

        squareCenter = transform.TransformPoint(localCenter);
        return true;
    }
}
