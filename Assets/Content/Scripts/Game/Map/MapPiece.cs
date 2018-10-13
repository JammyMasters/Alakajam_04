using UnityEngine;

[CreateAssetMenu(fileName = "Map Piece", menuName = "SuiSALTO/Map Piece", order = 1)]
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
