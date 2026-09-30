using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.OpenXR;
using UnityEngine.XR.Hands.Samples.VisualizerSample;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

namespace Under.Foundation
{
    /// <summary>
    /// Temporary native-build diagnostics for the controller-free hand startup path.
    /// This component only observes and logs state; it never starts, stops, enables,
    /// or disables XR systems.
    /// </summary>
    [DefaultExecutionOrder(-32000)]
    public sealed class XRStartupDiagnostics : MonoBehaviour
    {
        const string LogPrefix = "[UNDER XR STARTUP]";

        static readonly List<XRHandSubsystem> s_HandSubsystems = new List<XRHandSubsystem>();

        bool m_OpenXRReadyLogged;
        bool m_HandSubsystemFoundLogged;
        bool m_HandSubsystemRunningLogged;
        bool m_LeftTrackedLogged;
        bool m_RightTrackedLogged;
        bool m_VisualsReadyLogged;
        bool m_InteractorsReadyLogged;
        bool m_InputReadyLogged;
        bool m_ModalityManagerLogged;
        bool m_TenSecondWarningLogged;
        bool m_ThirtySecondWarningLogged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            var diagnosticsObject = new GameObject("[UNDER XR Startup Diagnostics]")
            {
                hideFlags = HideFlags.DontSave,
            };

            DontDestroyOnLoad(diagnosticsObject);
            diagnosticsObject.AddComponent<XRStartupDiagnostics>();
        }

        void Awake()
        {
            Log($"diagnostics started; platform={Application.platform}; unity={Application.unityVersion}; " +
                $"automaticHandSubsystemInitialization={HandTracking.automaticallyInitializeSubsystem}");

            var settings = OpenXRSettings.Instance;
            if (settings == null)
            {
                Log("OpenXR settings are not available yet.");
                return;
            }

            var enabledFeatures = settings.GetFeatures()
                .Where(feature => feature.enabled)
                .Select(feature => feature.GetType().Name)
                .OrderBy(name => name);
            Log($"enabled OpenXR features: {string.Join(", ", enabledFeatures)}");
        }

        void Update()
        {
            CheckOpenXR();
            CheckHands();
            CheckSceneSystems();
            CheckTimeouts();
        }

        void CheckOpenXR()
        {
            if (m_OpenXRReadyLogged)
                return;

            var manager = XRGeneralSettings.Instance != null ? XRGeneralSettings.Instance.Manager : null;
            if (manager == null || !manager.isInitializationComplete || !(manager.activeLoader is OpenXRLoader))
                return;

            m_OpenXRReadyLogged = true;
            Log($"1/5 OpenXR initialized; loader={manager.activeLoader.GetType().Name}");
        }

        void CheckHands()
        {
            s_HandSubsystems.Clear();
            SubsystemManager.GetSubsystems(s_HandSubsystems);
            var handSubsystem = s_HandSubsystems.FirstOrDefault();
            if (handSubsystem == null)
                return;

            if (!m_HandSubsystemFoundLogged)
            {
                m_HandSubsystemFoundLogged = true;
                Log($"2/5 XR Hands subsystem created; descriptor={handSubsystem.subsystemDescriptor.id}");
            }

            if (handSubsystem.running && !m_HandSubsystemRunningLogged)
            {
                m_HandSubsystemRunningLogged = true;
                Log("2/5 XR Hands subsystem started and running");
            }

            if (handSubsystem.leftHand.isTracked && !m_LeftTrackedLogged)
            {
                m_LeftTrackedLogged = true;
                Log("3/5 first valid LEFT hand tracking data received");
            }

            if (handSubsystem.rightHand.isTracked && !m_RightTrackedLogged)
            {
                m_RightTrackedLogged = true;
                Log("3/5 first valid RIGHT hand tracking data received");
            }
        }

        void CheckSceneSystems()
        {
            if (!m_VisualsReadyLogged)
            {
                var visualizers = FindObjectsByType<HandVisualizer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (visualizers.Any(visualizer => visualizer.isActiveAndEnabled && visualizer.drawMeshes))
                {
                    m_VisualsReadyLogged = true;
                    Log("4/5 hand visuals enabled with mesh drawing active");
                }
            }

            if (!m_InteractorsReadyLogged)
            {
                var interactors = FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(interactor => interactor.isActiveAndEnabled)
                    .ToArray();
                if (interactors.Length >= 2 && interactors.All(interactor => interactor.enableNearCasting && !interactor.enableFarCasting))
                {
                    m_InteractorsReadyLogged = true;
                    Log($"5/5 hand direct interactors enabled; count={interactors.Length}; near=true; far=false");
                }
            }

            if (!m_InputReadyLogged)
            {
                var inputManagers = FindObjectsByType<InputActionManager>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(manager => manager.isActiveAndEnabled)
                    .ToArray();
                var actionAssets = inputManagers.SelectMany(manager => manager.actionAssets).Where(asset => asset != null).Distinct().ToArray();
                if (inputManagers.Length > 0 && actionAssets.Length > 0 && actionAssets.All(asset => asset.enabled))
                {
                    m_InputReadyLogged = true;
                    Log($"input actions enabled immediately; managers={inputManagers.Length}; assets={string.Join(", ", actionAssets.Select(asset => asset.name))}");
                }
            }

            if (!m_ModalityManagerLogged)
            {
                var modalityManagers = FindObjectsByType<XRInputModalityManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (modalityManagers.Length > 0)
                {
                    m_ModalityManagerLogged = true;
                    Log($"XRI modality switching component present; enabled={modalityManagers[0].enabled} (expected false for hands-only prototype)");
                }
            }
        }

        void CheckTimeouts()
        {
            var elapsed = Time.realtimeSinceStartupAsDouble;
            if (!m_TenSecondWarningLogged && elapsed >= 10d && (!m_LeftTrackedLogged || !m_RightTrackedLogged))
            {
                m_TenSecondWarningLogged = true;
                Log($"WARNING: no valid tracking data for both hands by 10 seconds; left={m_LeftTrackedLogged}; right={m_RightTrackedLogged}");
            }

            if (!m_ThirtySecondWarningLogged && elapsed >= 30d && (!m_LeftTrackedLogged || !m_RightTrackedLogged))
            {
                m_ThirtySecondWarningLogged = true;
                Log($"WARNING: no valid tracking data for both hands by 30 seconds; left={m_LeftTrackedLogged}; right={m_RightTrackedLogged}");
            }
        }

        static void Log(string message)
        {
            Debug.Log($"{LogPrefix} t={Time.realtimeSinceStartupAsDouble:F3}s {message}");
        }
    }
}
