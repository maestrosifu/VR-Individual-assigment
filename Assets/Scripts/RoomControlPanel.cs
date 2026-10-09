using TMPro;
using UnityEngine;

/// <summary>
/// World-space control panel. Each UI button calls one of these methods,
/// which triggers a visible change in the room and updates the button label.
/// </summary>
public class RoomControlPanel : MonoBehaviour
{
    [SerializeField] private RoomLightingController roomLighting;
    [SerializeField] private WindowBlindsController windowBlinds;
    [SerializeField] private ChessBoardResetter chessBoard;
    [SerializeField] private MagnetSpawner magnetSpawner;

    [Header("Button labels")]
    [SerializeField] private TMP_Text lightsButtonLabel;
    [SerializeField] private TMP_Text blindsButtonLabel;

    [Header("Feedback")]
    [SerializeField] private AudioSource clickSound;

    private void Start()
    {
        RefreshLabels();
    }

    public void ToggleCeilingLights()
    {
        roomLighting.ToggleCeilingLights();
        OnButtonPressed();
    }

    public void ToggleWindowBlinds()
    {
        windowBlinds.ToggleBlinds();
        OnButtonPressed();
    }

    public void ResetChessBoard()
    {
        chessBoard.ResetAllPieces();
        OnButtonPressed();
    }

    public void SpawnMagnet()
    {
        magnetSpawner.SpawnMagnet();
        OnButtonPressed();
    }

    private void OnButtonPressed()
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }

        RefreshLabels();
    }

    private void RefreshLabels()
    {
        if (lightsButtonLabel != null)
        {
            lightsButtonLabel.text = roomLighting.AreLightsOn ? "Turn lights OFF" : "Turn lights ON";
        }

        if (blindsButtonLabel != null)
        {
            blindsButtonLabel.text = windowBlinds.AreClosed ? "Open blinds" : "Close blinds";
        }
    }
}
