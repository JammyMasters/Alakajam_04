using System.Collections;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public int MapHeight = 100;

    /// <summary>
    /// 
    /// </summary>
    public MapPiece[] MapPieces;

    /// <summary>
    /// 
    /// </summary>
    public GameObject Player;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        StartCoroutine(SpawnMap());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnMap()
    {
        var currentHeight = 0.0f;
        var nonRoofTops = MapPieces.Where(x => x.Type != MapPiece.PieceType.RoofTop).ToArray();
        var roofPieces = MapPieces.Where(x => x.Type == MapPiece.PieceType.RoofTop).ToArray();

        while (currentHeight < MapHeight)
        {
            currentHeight += SpawnMapPiece(nonRoofTops, currentHeight);
        }

        currentHeight += SpawnMapPiece(roofPieces, currentHeight);
        Player.transform.position = new Vector3(-1.0f, currentHeight + 5.0f, -4.5f);

        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pieces"></param>
    /// <param name="yPosition"></param>
    /// <returns></returns>
    private float SpawnMapPiece(MapPiece[] pieces, float yPosition)
    {
        var randomPiece = pieces[Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds.size.y;
    }
}
