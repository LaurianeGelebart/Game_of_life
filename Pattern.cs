using UnityEngine;

[CreateAssetMenu(menuName = "Game of life/Pattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells; 

    /// <summary>
    /// Retourne le centre du motif en calculant la moyenne des coordonnées min et max des cellules
    /// </summary>
    /// <returns>Point central du motif</returns>
    public Vector2Int GetCenter()
    {
        if(cells == null || cells.Length == 0){
            return Vector2Int.zero;
        }
        Vector2Int min = Vector2Int.zero; 
        Vector2Int max = Vector2Int.zero;

        foreach (Vector2Int cell in cells)
        {
            min.x = Mathf.Min(cell.x, min.x);
            min.y = Mathf.Min(cell.y, min.y);
            max.x = Mathf.Max(cell.x, min.x);
            max.y = Mathf.Max(cell.y, min.y);
        }
        return (min + max)/2;
    }
}
