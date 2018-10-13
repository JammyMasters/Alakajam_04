using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour, IGameStateObserver
{
    private const string c_piecesGameObjectName = "Pieces";

    public int MapHeight = 100;

    public MapPiece[] MapPieces;

    public PlayerController Player;

    public bool GenerateInEditMode = false;

    public void Update()
    {
        if (Application.isEditor && GenerateInEditMode)
        {
            Generate();
            GenerateInEditMode = false;
        }
    }

    private void Generate()
    {
        DestroyPiecesGameObject();

        var piecesGameObject = new GameObject(c_piecesGameObjectName);
        piecesGameObject.transform.parent = transform;

        var currentHeight = 0.0f;
        var nonRoofTops = MapPieces.Where(x => x.Type != MapPiece.PieceType.RoofTop).ToArray();
        var roofPieces = MapPieces.Where(x => x.Type == MapPiece.PieceType.RoofTop).ToArray();

        if (!nonRoofTops.Any())
        {
            Debug.LogError("No 'non-roof' map pieces have been set, failed to generate building.");
            return;
        }

        if (!roofPieces.Any())
        {
            Debug.LogError("No 'roof' map pieces have been set, failed to generate building.");
            return;
        }

        while (currentHeight < MapHeight)
        {
            currentHeight += SpawnMapPieces(nonRoofTops, currentHeight, piecesGameObject).size.y;
        }

        var lastPieceBounds = SpawnMapPieces(roofPieces, currentHeight, piecesGameObject);
        currentHeight += lastPieceBounds.size.y;

        Player.Spawn(new Vector3(-2.0f, currentHeight + 5.0f, -4.5f), lastPieceBounds.size.x);
    }

    private void DestroyPiecesGameObject()
    {
        var piecesGameObject = transform.Find("Pieces")?.gameObject;
        if (piecesGameObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(piecesGameObject);
            }
            else
            {
                DestroyImmediate(piecesGameObject);
            }
        }
    }

    private Bounds SpawnMapPieces(MapPiece[] pieces, float yPosition, GameObject piecesGameObject)
    {
        var randomPiece = pieces[Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, piecesGameObject.transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds;
    }

    public void OnLeaveState(GameState state)
    {
        DestroyPiecesGameObject();
    }

    public void OnEnterState(GameState state)
    {
        if (state != GameState.Falling)
        {
            return;
        }
        Generate();
    }
}
