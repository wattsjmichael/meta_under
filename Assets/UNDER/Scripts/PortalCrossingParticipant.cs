using System;
using UnityEngine;

namespace Under.Foundation
{
    /// <summary>
    /// Switches a participant between portal-isolated and normal-room presentation
    /// when its physical path crosses a finite PortalBoundary.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PortalCrossingParticipant : MonoBehaviour
    {
        [Serializable]
        public sealed class RendererBinding
        {
            public Renderer renderer;
            public Material[] underMaterials;
            public Material[] roomMaterials;

            public void Apply(PortalBoundary.Side side, UnityEngine.Object logContext)
            {
                if (renderer == null)
                    return;

                var materials = side == PortalBoundary.Side.Under ? underMaterials : roomMaterials;
                if (materials == null || materials.Length == 0)
                    return;

                var slotCount = renderer.sharedMaterials.Length;
                if (materials.Length == slotCount)
                {
                    renderer.sharedMaterials = materials;
                    return;
                }

                if (materials.Length == 1)
                {
                    var expanded = new Material[Mathf.Max(1, slotCount)];
                    for (var i = 0; i < expanded.Length; i++)
                        expanded[i] = materials[0];
                    renderer.sharedMaterials = expanded;
                    return;
                }

                Debug.LogWarning(
                    $"Portal renderer binding on {renderer.name} has {materials.Length} materials for {slotCount} slots.",
                    logContext);
            }
        }

        [SerializeField]
        PortalBoundary m_Boundary;

        [SerializeField]
        PortalBoundary.Side m_StartingSide = PortalBoundary.Side.Under;

        [SerializeField]
        [Tooltip("Optional transform used as the crossing reference. Defaults to this object's transform.")]
        Transform m_CrossingReference;

        [SerializeField, Min(0f)]
        [Tooltip("Prevents repeated side changes while the reference point jitters on the portal plane.")]
        float m_CrossingHysteresis = 0.01f;

        [SerializeField]
        [Tooltip("Allows a small amount of renderer/collider width at the aperture edge.")]
        float m_ApertureEdgePadding = -0.01f;

        [SerializeField]
        RendererBinding[] m_RendererBindings = Array.Empty<RendererBinding>();

        [SerializeField]
        [Tooltip("Behaviours such as Recall that must only become available after entering the room.")]
        Behaviour[] m_EnableInRoom = Array.Empty<Behaviour>();

        PortalBoundary.Side m_CurrentSide;
        Vector3 m_LastPositionOnCurrentSide;
        bool m_Initialized;

        public PortalBoundary.Side currentSide => m_CurrentSide;

        void Awake()
        {
            Initialize();
        }

        void LateUpdate()
        {
            if (!m_Initialized || m_Boundary == null)
                return;

            var currentPosition = GetReferencePosition();
            var signedDistance = m_Boundary.GetSignedDistance(currentPosition);

            if (m_CurrentSide == PortalBoundary.Side.Under)
            {
                if (signedDistance <= 0f)
                {
                    m_LastPositionOnCurrentSide = currentPosition;
                    return;
                }

                if (signedDistance >= m_CrossingHysteresis &&
                    m_Boundary.TryGetCrossing(
                        m_LastPositionOnCurrentSide,
                        currentPosition,
                        out var destination,
                        m_ApertureEdgePadding) &&
                    destination == PortalBoundary.Side.Room)
                {
                    SetSide(PortalBoundary.Side.Room);
                }

                return;
            }

            if (signedDistance >= 0f)
            {
                m_LastPositionOnCurrentSide = currentPosition;
                return;
            }

            if (signedDistance <= -m_CrossingHysteresis &&
                m_Boundary.TryGetCrossing(
                    m_LastPositionOnCurrentSide,
                    currentPosition,
                    out var underDestination,
                    m_ApertureEdgePadding) &&
                underDestination == PortalBoundary.Side.Under)
            {
                SetSide(PortalBoundary.Side.Under);
            }
        }

        public void ConfigureSingleRenderer(
            PortalBoundary boundary,
            Renderer targetRenderer,
            Material underMaterial,
            Material roomMaterial,
            Behaviour[] enableInRoom)
        {
            m_Boundary = boundary;
            m_RendererBindings = new[]
            {
                new RendererBinding
                {
                    renderer = targetRenderer,
                    underMaterials = new[] { underMaterial },
                    roomMaterials = new[] { roomMaterial },
                },
            };
            m_EnableInRoom = enableInRoom ?? Array.Empty<Behaviour>();
            m_StartingSide = PortalBoundary.Side.Under;
            m_Initialized = false;
            Initialize();
        }

        public void SetSide(PortalBoundary.Side side)
        {
            m_CurrentSide = side;
            m_LastPositionOnCurrentSide = GetReferencePosition();

            foreach (var binding in m_RendererBindings)
                binding?.Apply(side, this);

            var enableRoomBehaviours = side == PortalBoundary.Side.Room;
            foreach (var behaviour in m_EnableInRoom)
            {
                if (behaviour == null || behaviour == this)
                    continue;

                if (behaviour is RecallableObject recallable)
                    recallable.SetRecallAvailable(enableRoomBehaviours);
                else
                    behaviour.enabled = enableRoomBehaviours;
            }

            Debug.Log($"Portal crossing: {name} entered {side}.", this);
        }

        void Initialize()
        {
            if (m_Initialized)
                return;

            if (m_CrossingReference == null)
                m_CrossingReference = transform;

            m_Initialized = true;
            SetSide(m_StartingSide);
        }

        Vector3 GetReferencePosition()
        {
            return m_CrossingReference != null ? m_CrossingReference.position : transform.position;
        }
    }
}
