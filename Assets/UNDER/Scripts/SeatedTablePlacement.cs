using System;
using UnityEngine;

namespace Under.Foundation
{
    /// <summary>
    /// Keeps the table, breach, and table-resting test objects aligned while exposing
    /// one seated-distance adjustment. The XR Origin and tracked camera are never moved.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class SeatedTablePlacement : MonoBehaviour
    {
        [SerializeField, Range(-0.35f, 0.15f)]
        [Tooltip("Meters relative to the original validated layout. Negative values move the table toward the seated player.")]
        float m_ForwardOffset = -0.18f;

        [SerializeField, HideInInspector]
        Transform[] m_PlacementRoots = Array.Empty<Transform>();

        [SerializeField, HideInInspector]
        Vector3[] m_BaselineWorldPositions = Array.Empty<Vector3>();

        public float forwardOffset => m_ForwardOffset;

        public void Configure(float forwardOffset, params Transform[] placementRoots)
        {
            m_ForwardOffset = forwardOffset;
            m_PlacementRoots = placementRoots ?? Array.Empty<Transform>();
            m_BaselineWorldPositions = new Vector3[m_PlacementRoots.Length];

            for (var i = 0; i < m_PlacementRoots.Length; i++)
            {
                if (m_PlacementRoots[i] != null)
                    m_BaselineWorldPositions[i] = m_PlacementRoots[i].position;
            }

            ApplyPlacement();
        }

        [ContextMenu("Apply Seated Table Placement")]
        public void ApplyPlacement()
        {
            if (m_PlacementRoots == null ||
                m_BaselineWorldPositions == null ||
                m_PlacementRoots.Length != m_BaselineWorldPositions.Length)
            {
                return;
            }

            var offset = Vector3.forward * m_ForwardOffset;
            for (var i = 0; i < m_PlacementRoots.Length; i++)
            {
                if (m_PlacementRoots[i] != null)
                    m_PlacementRoots[i].position = m_BaselineWorldPositions[i] + offset;
            }
        }

        void Awake()
        {
            ApplyPlacement();
        }

        void OnValidate()
        {
            ApplyPlacement();
        }
    }
}
