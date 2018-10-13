using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnableSurface : MonoBehaviour
{
    public float Width;

    public float Depth;

    public float CellSize;

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

        m_gridWidth = (int)Math.Floor(Width / CellSize);
        m_gridHeight = (int)Math.Floor(Depth / CellSize);
        m_grid = new bool[m_gridWidth * m_gridHeight];
        for (int i = 0; i < m_grid.Length; i++)
        {
            m_grid[i] = false;
        }
        m_halfWidth = Width * 0.5f;
        m_halfDepth = Depth * 0.5f;

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
        int x, z;
        do
        {
            x = UnityEngine.Random.Range(0, m_gridWidth);
            z = UnityEngine.Random.Range(0, m_gridHeight);
        } while (m_grid[z * m_gridWidth + x]);
        m_grid[z * m_gridWidth + x] = true;
        position = new Vector3(transform.position.x + x * CellSize - m_halfWidth, transform.position.y, transform.position.z + z * CellSize - m_halfDepth);
        return true;
    }
}
