using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns new coloured magnets that the player can stick on the whiteboard.
/// </summary>
public class MagnetSpawner : MonoBehaviour
{
    [SerializeField] private WhiteboardMagnet magnetPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform whiteboardSurface;
    [SerializeField] private Material[] magnetColors;
    [SerializeField] private int maxSpawnedMagnets = 15;

    private readonly Queue<WhiteboardMagnet> spawnedMagnets = new Queue<WhiteboardMagnet>();

    public void SpawnMagnet()
    {
        if (spawnedMagnets.Count >= maxSpawnedMagnets)
        {
            Destroy(spawnedMagnets.Dequeue().gameObject);
        }

        WhiteboardMagnet magnet = Instantiate(magnetPrefab, spawnPoint.position, spawnPoint.rotation);
        magnet.SetWhiteboard(whiteboardSurface);
        // Magnets on the board are kinematic; a freshly spawned one should drop into the tray.
        magnet.GetComponent<Rigidbody>().isKinematic = false;

        if (magnetColors.Length > 0)
        {
            Material randomColor = magnetColors[Random.Range(0, magnetColors.Length)];
            magnet.GetComponentInChildren<Renderer>().sharedMaterial = randomColor;
        }

        spawnedMagnets.Enqueue(magnet);
    }
}
