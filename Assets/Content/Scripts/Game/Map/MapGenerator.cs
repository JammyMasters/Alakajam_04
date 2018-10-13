using System.Collections;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    public int MapHeight = 100;

    public MapPiece[] MapPieces;

    public PlayerController Player;

    public bool GenerateInEditMode = false;

    private GameObject m_piecesGameObject;

    public void Start()
    {
        if (Application.isPlaying)
        {
            Generate();
        }
    }

    public void Update()
    {
        if (GenerateInEditMode)
        {
            Generate();
            GenerateInEditMode = false;
        }
    }

    private void Generate()
    {
        if (m_piecesGameObject != null)
        {
            DestroyImmediate(m_piecesGameObject);
        }

        m_piecesGameObject = new GameObject("Pieces");
        m_piecesGameObject.transform.parent = transform;

        var currentHeight = 0.0f;
        var nonRoofTops = MapPieces.Where(x => x.Type != MapPiece.PieceType.RoofTop).ToArray();
        var roofPieces = MapPieces.Where(x => x.Type == MapPiece.PieceType.RoofTop).ToArray();

        if (nonRoofTops.Length > 0)
        {
            while (currentHeight < MapHeight)
            {
                currentHeight += SpawnMapPieces(nonRoofTops, currentHeight);
            }
        }

        if (roofPieces.Length > 0)
        {
            currentHeight += SpawnMapPieces(roofPieces, currentHeight);
        }

        Player.Activate(new Vector3(-2.0f, currentHeight + 5.0f, -4.5f));
    }

    private float SpawnMapPieces(MapPiece[] pieces, float yPosition)
    {
        var randomPiece = pieces[Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, m_piecesGameObject.transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds.size.y;
    }
}
