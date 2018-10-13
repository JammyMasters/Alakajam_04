using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SpawnableSurface : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;

    private float Top => m_meshRenderer.bounds.max.y;

    private float Width => m_meshRenderer.bounds.size.x * Scale.x;

    private float Height => m_meshRenderer.bounds.size.z * Scale.y;

    public float CellSize;

    public Vector2 Scale = Vector2.one;

    public Vector2 Pivot;

    private int m_gridWidth;

    private int m_gridHeight;

    private bool[] m_grid;

    private float m_halfWidth;

    private float m_halfDepth;

    private bool m_initialised;

    public bool InitialiseInEditMode;

    private void Initialise(bool forceInitialisation = false)
    {
        if (m_initialised && !forceInitialisation)
        {
            return;
        }

        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_gridWidth = (int)Math.Floor(Width / CellSize);
        m_gridHeight = (int)Math.Floor(Height / CellSize);
        m_grid = new bool[m_gridWidth * m_gridHeight];
        for (int i = 0; i < m_grid.Length; i++)
        {
            m_grid[i] = false;
        }
        m_halfWidth = Width * 0.5f;
        m_halfDepth = Height * 0.5f;

        m_initialised = true;
    }

    public void Update()
    {
        if (Application.isEditor && InitialiseInEditMode)
        {
            Initialise(true);
            InitialiseInEditMode = false;
        }
    }

    public bool GetUnoccupiedPosition(out Vector3 position)
    {
        Initialise();

        if (!m_grid.Any())
        {
            position = Vector3.zero;
            return false;
        }
        int x, y;
        do
        {
            x = UnityEngine.Random.Range(0, m_gridWidth);
            y = UnityEngine.Random.Range(0, m_gridHeight);
        } while (m_grid[y * m_gridWidth + x]);
        m_grid[y * m_gridWidth + x] = true;
        position = new Vector3(Pivot.x + x * CellSize - m_halfWidth, Top, Pivot.y + y * CellSize - m_halfDepth);
        return true;
    }
}
