using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab; // El prefab de tu carta
    public int rows = 4;
    public int columns = 6;
    public float cardSpacing = 1.2f; // Espacio entre cartas
    public Vector3 startPosition = new Vector3(-3f, 0.15f, -1.5f); // Posición inicial

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calcular posición de cada carta
                Vector3 spawnPosition = startPosition + new Vector3(
                    col * cardSpacing,
                    0,
                    row * cardSpacing
                );

                // Crear la carta
                GameObject card = Instantiate(cardPrefab, spawnPosition, Quaternion.identity);
                card.name = "Card_" + row + "_" + col;
            }
        }
    }
}