using System;
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
        var piecesGameObject = GetPiecesGameObject();
        if (piecesGameObject != null)
        {
            DestroyImmediate(piecesGameObject);
        }

        piecesGameObject = new GameObject("Pieces");
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

        Player.Activate(new Vector3(-2.0f, currentHeight + 5.0f, -4.5f), lastPieceBounds.size.x);
    }

    private GameObject GetPiecesGameObject()
    {
        return transform.Find("Pieces")?.gameObject;
    }

    private Bounds SpawnMapPieces(MapPiece[] pieces, float yPosition, GameObject piecesGameObject)
    {
        var randomPiece = pieces[UnityEngine.Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, piecesGameObject.transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds;
    }
}
