using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;
    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    /// <summary>
    /// Tableau contenant les voisins d'une cellule
    /// </summary>
    private Vector3Int[] directions = 
    {
        new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0)
    };


    /// <summary>
    /// Initialise les listes des cellules vivantes et de celles à vérifier
    /// </summary>
    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
    }

    /// <summary>
    /// Place le motif de départ sur le plateau au début du jeu
    /// </summary>
    private void Start()
    {
        SetPattern(pattern);
    }


    /// <summary>
    /// Positionne un motif sélectionné dans unity sur le plateau et le centre
    /// </summary>
    /// <param name="pattern">Le motif</param>
    private void SetPattern(Pattern pattern)
    {
        Clear();
        Vector2Int center = pattern.GetCenter();
        foreach (Vector2Int cell in pattern.cells)
        {
            Vector3Int cellPos = (Vector3Int)(cell - center);
            currentState.SetTile(cellPos, aliveTile);
            aliveCells.Add(cellPos);  
        }
    }


    /// <summary>
    /// Vide le plateau et réinitialise les variables
    /// </summary>
    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        aliveCells.Clear();
        cellsToCheck.Clear();
    }


    /// <summary>
    /// Lance la simulation 
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }


    /// <summary>
    /// Exécute la simulation en continu avec un délai entre chaque mise à jour
    /// </summary>
    /// <returns>Retourne un IEnumerator pour gérer le délai de mises à jour</returns>
    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;
        while (enabled)
        {
            UpdateState();
            yield return interval;
        }
    }


    /// <summary>
    /// Met à jour l'état du plateau avec les règles du jeu de la vie
    /// (2 ou 3 voisins pour survivre et 3 pour naître)
    /// </summary>
    private void UpdateState()
    {
        cellsToCheck.Clear();
        foreach (Vector3Int cell in aliveCells)
        {
            AddNeighborsToCheck(cell);
        }

        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell); 
            } 
            else if (alive && (neighbors < 2 || neighbors > 3))
            {
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell); 
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

        SwapTilemaps();
    }


    /// <summary>
    /// Compte le nombre de voisins vivants d'une cellule
    /// </summary>
    /// <param name="cell">La cellule</param>
    /// <returns>Le nombre de voisins vivants</returns>
    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;
        foreach (var direction in directions)
        {
            if (IsAlive(cell + direction))
            {
                count++;
            }
        }
        return count;
    }


    /// <summary>
    /// Ajoute une cellule et ses voisins dans la liste des cellules à vérifier
    /// </summary>
    /// <param name="cell">La cellule centrale</param>
    private void AddNeighborsToCheck(Vector3Int cell)
    {
        cellsToCheck.Add(cell);
        foreach (var direction in directions)
        {
            cellsToCheck.Add(cell + direction);
        }
    }


    /// <summary>
    /// Vérifie si une cellule est vivante en vérifiant si elle contient une tile vivante
    /// </summary>
    /// <param name="cell">La cellule à vérifier</param>
    /// <returns>Vrai si la cellule est vivante sinon faux</returns>
    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }


    /// <summary>
    /// Permute currentState et nextState pour que l'état actuel devienne l'état suivant
    /// </summary>
    private void SwapTilemaps()
    {
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }
}
