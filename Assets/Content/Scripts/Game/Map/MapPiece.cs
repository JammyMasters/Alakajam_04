using UnityEngine;

[CreateAssetMenu(fileName = "Map Piece", menuName = "Suicide/Map Piece", order = 1)]
public class MapPiece : ScriptableObject
{
    public enum PieceType
    {
        Generic,
        DodgeBased,
        RoofTop
    }

    public PieceType Type;
    public GameObject Prefab;
}
