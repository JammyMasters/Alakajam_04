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
            StartCoroutine(Generate());
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

    public IEnumerator Generate()
    {
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

        yield break;
    }

    private float SpawnMapPieces(MapPiece[] pieces, float yPosition)
    {
        var randomPiece = pieces[UnityEngine.Random.Range(0, pieces.Length)];
        var newPosition = new Vector3(0.0f, yPosition, 0.0f);
        var pieceGeometry = Instantiate(randomPiece.Prefab, newPosition, Quaternion.identity, transform);
        return pieceGeometry.GetComponent<MeshRenderer>().bounds.size.y;
    }
}
