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

        if (nonRoofTops.Length > 0)
        {
            while (currentHeight < MapHeight)
            {
                currentHeight += SpawnMapPieces(nonRoofTops, currentHeight, piecesGameObject);
            }
        }

        if (roofPieces.Length > 0)
        {
            currentHeight += SpawnMapPieces(roofPieces, currentHeight, piecesGameObject);
        }

        Player.Activate(new Vector3(-2.0f, currentHeight + 5.0f, -4.5f));
    }

    private GameObject GetPiecesGameObject()
    {
        return transform.Find("Pieces")?.gameObject;
    }

    private float SpawnMapPieces(MapPiece[] pieces, float yPosition, GameObject piecesGameObject)
    {
        var randomPiece = pieces[UnityEngine.Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, piecesGameObject.transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds.size.y;
    }
}
