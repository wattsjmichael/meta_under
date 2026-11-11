#if UNITY_EDITOR
using System.Collections.Generic;
using Under.Foundation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Under.VerticalSlice.Editor
{
    /// <summary>
    /// Rebuilds the first screenshot-ready lock from deterministic Unity primitives.
    /// Kept in-project so the prototype art and hierarchy remain reproducible.
    /// </summary>
    public static class ArtifactLockBuilder
    {
        const string MenuPath = "UNDER/Build Vertical Slice Artifact Lock";
        const string MaterialFolder = "Assets/UNDER/Materials/ArtifactLock";

        [MenuItem(MenuPath)]
        public static void Build()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "UNDER_Prototype")
                throw new System.InvalidOperationException("Open UNDER_Prototype before building the artifact lock.");

            var oldPresentation = GameObject.Find("Artifact Presentation");
            if (oldPresentation != null)
                Object.DestroyImmediate(oldPresentation);

            EnsureFolder("Assets/UNDER/Materials", "ArtifactLock");
            var darkMetal = Material("Artifact_Dark_Aged_Metal", new Color(0.055f, 0.065f, 0.072f), 0.86f, 0.28f);
            var edgeMetal = Material("Artifact_Worn_Bronze", new Color(0.24f, 0.105f, 0.035f), 0.82f, 0.34f);
            var ceramic = Material("Artifact_Smoked_Ceramic", new Color(0.19f, 0.17f, 0.15f), 0.08f, 0.22f);
            var dormant = Material("Artifact_Marking_Dormant", new Color(0.025f, 0.03f, 0.032f), 0.35f, 0.18f);
            var amber = EmissiveMaterial("Artifact_Marking_Amber", new Color(0.42f, 0.12f, 0.018f), new Color(2.8f, 0.42f, 0.035f));
            var cyan = EmissiveMaterial("Artifact_Core_Cyan", new Color(0.01f, 0.17f, 0.19f), new Color(0.04f, 2.8f, 3.8f));

            var presentation = NewObject("Artifact Presentation", null);
            var plinth = Primitive(
                "Artifact Display Plinth",
                PrimitiveType.Cube,
                presentation.transform,
                new Vector3(0f, 0.79f, 0.38f),
                Quaternion.identity,
                new Vector3(0.58f, 0.08f, 0.28f),
                ceramic,
                true);
            plinth.isStatic = true;

            CreatePlinthInlay(presentation.transform, darkMetal, edgeMetal, cyan);

            var root = NewObject("Artifact Lock", presentation.transform);
            root.transform.position = new Vector3(0f, 0.985f, 0.38f);

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.mass = 2.4f;
            rigidbody.linearDamping = 0.65f;
            rigidbody.angularDamping = 0.9f;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var bodyCollider = root.AddComponent<BoxCollider>();
            bodyCollider.size = new Vector3(0.15f, 0.19f, 0.11f);

            var bodyGrab = root.AddComponent<XRGrabInteractable>();
            bodyGrab.colliders.Clear();
            bodyGrab.colliders.Add(bodyCollider);
            bodyGrab.useDynamicAttach = true;
            bodyGrab.throwOnDetach = true;
            bodyGrab.throwVelocityScale = 1.15f;
            bodyGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;

            root.AddComponent<RecallableObject>();
            var gauntlet = root.AddComponent<GauntletPullable>();
            var targetIndicator = CreateTargetIndicator(root.transform, amber);
            gauntlet.Configure(targetIndicator);

            var controller = root.AddComponent<ArtifactLockController>();
            var visuals = NewObject("LOCKED BODY", root.transform).transform;
            var innerCore = CreateInnerCore(visuals, ceramic, darkMetal, cyan);
            var stageMarks = CreateStageMarks(visuals, dormant);
            CreateCentralBody(visuals, darkMetal, ceramic, edgeMetal, dormant);

            var shells = NewObject("STAGE 3 - TWO HAND SEAL", root.transform).transform;
            var leftShell = NewObject("Left Seal Half", shells).transform;
            var rightShell = NewObject("Right Seal Half", shells).transform;
            CreateShellHalf(leftShell, -1f, darkMetal, ceramic, edgeMetal, dormant);
            CreateShellHalf(rightShell, 1f, darkMetal, ceramic, edgeMetal, dormant);
            var leftHandle = CreateSealHandle(leftShell, -1f, edgeMetal, amber);
            var rightHandle = CreateSealHandle(rightShell, 1f, edgeMetal, amber);

            var rotary = CreateRotaryStage(root.transform, root.transform, darkMetal, edgeMetal, amber, dormant);
            var slider = CreateSliderStage(root.transform, root.transform, darkMetal, edgeMetal, cyan);

            var twoHandSeal = root.AddComponent<ArtifactTwoHandSeal>();
            twoHandSeal.Configure(controller, root.transform, leftHandle, rightHandle, leftShell, rightShell, 0.13f);
            rotary.Configure(controller, root.transform, rotary.transform, 90f);
            slider.Configure(controller, root.transform, slider.transform, 0.11f);
            controller.Configure(rotary, slider, twoHandSeal, innerCore, stageMarks, dormant, amber, cyan);

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[UNDER ARTIFACT] Built vertical-slice lock in UNDER_Prototype. Sequence: collar, spine, two-hand seal.", root);
        }

        static void CreatePlinthInlay(Transform parent, Material darkMetal, Material edgeMetal, Material cyan)
        {
            Primitive("Plinth Metal Bed", PrimitiveType.Cube, parent, new Vector3(0f, 0.836f, 0.38f), Quaternion.identity,
                new Vector3(0.40f, 0.012f, 0.19f), darkMetal, false);
            for (var i = -2; i <= 2; i++)
            {
                var material = i == 0 ? cyan : edgeMetal;
                Primitive($"Plinth Sigil {i + 3}", PrimitiveType.Cube, parent,
                    new Vector3(i * 0.065f, 0.844f, 0.38f), Quaternion.Euler(0f, i * 18f, 0f),
                    new Vector3(0.012f, 0.004f, 0.11f - Mathf.Abs(i) * 0.012f), material, false);
            }
        }

        static Transform CreateInnerCore(Transform parent, Material ceramic, Material darkMetal, Material cyan)
        {
            var coreRoot = NewObject("Exposed Inner Core", parent).transform;
            Primitive("Ceramic Heart", PrimitiveType.Sphere, coreRoot, Vector3.zero, Quaternion.identity,
                new Vector3(0.12f, 0.15f, 0.085f), ceramic, false);
            Primitive("Core Shadow", PrimitiveType.Sphere, coreRoot, new Vector3(0f, 0f, -0.052f), Quaternion.identity,
                new Vector3(0.082f, 0.105f, 0.036f), darkMetal, false);
            Primitive("Core Vertical Fissure", PrimitiveType.Cube, coreRoot, new Vector3(0f, 0f, -0.074f), Quaternion.Euler(0f, 0f, 7f),
                new Vector3(0.012f, 0.17f, 0.008f), cyan, false);
            Primitive("Core Cross Fissure", PrimitiveType.Cube, coreRoot, new Vector3(0f, 0.008f, -0.076f), Quaternion.Euler(0f, 0f, -13f),
                new Vector3(0.105f, 0.009f, 0.008f), cyan, false);
            return coreRoot;
        }

        static Renderer[] CreateStageMarks(Transform parent, Material dormant)
        {
            var marksRoot = NewObject("Three Stage Witness Marks", parent).transform;
            var result = new Renderer[3];
            for (var i = 0; i < result.Length; i++)
            {
                var mark = Primitive($"Witness Mark {i + 1}", PrimitiveType.Cube, marksRoot,
                    new Vector3((i - 1) * 0.05f, -0.145f, -0.105f), Quaternion.Euler(0f, 0f, i == 1 ? 45f : 0f),
                    new Vector3(0.022f, 0.022f, 0.012f), dormant, false);
                result[i] = mark.GetComponent<Renderer>();
            }
            return result;
        }

        static void CreateCentralBody(Transform parent, Material darkMetal, Material ceramic, Material edgeMetal, Material dormant)
        {
            Primitive("Rear Ceramic Disc", PrimitiveType.Cylinder, parent, new Vector3(0f, 0f, 0.035f), Quaternion.Euler(90f, 0f, 0f),
                new Vector3(0.12f, 0.035f, 0.12f), ceramic, false);
            Primitive("Central Axle", PrimitiveType.Cylinder, parent, new Vector3(0f, 0f, -0.04f), Quaternion.Euler(90f, 0f, 0f),
                new Vector3(0.035f, 0.12f, 0.035f), edgeMetal, false);

            for (var i = 0; i < 8; i++)
            {
                var angle = i * 45f;
                var radians = angle * Mathf.Deg2Rad;
                var position = new Vector3(Mathf.Cos(radians) * 0.105f, Mathf.Sin(radians) * 0.105f, -0.066f);
                Primitive($"Engraved Radial Mark {i + 1}", PrimitiveType.Cube, parent, position,
                    Quaternion.Euler(0f, 0f, angle), new Vector3(0.034f, 0.008f, 0.008f),
                    i % 2 == 0 ? dormant : darkMetal, false);
            }
        }

        static void CreateShellHalf(
            Transform shell,
            float side,
            Material darkMetal,
            Material ceramic,
            Material edgeMetal,
            Material dormant)
        {
            Primitive("Outer Cheek Plate", PrimitiveType.Cube, shell, new Vector3(side * 0.082f, 0f, 0.015f), Quaternion.identity,
                new Vector3(0.075f, 0.25f, 0.11f), darkMetal, false);
            Primitive("Ceramic Inset", PrimitiveType.Sphere, shell, new Vector3(side * 0.092f, 0.008f, -0.052f), Quaternion.identity,
                new Vector3(0.072f, 0.13f, 0.045f), ceramic, false);
            Primitive("Upper Jaw", PrimitiveType.Cube, shell, new Vector3(side * 0.055f, 0.108f, -0.01f), Quaternion.Euler(0f, 0f, side * -22f),
                new Vector3(0.12f, 0.042f, 0.12f), edgeMetal, false);
            Primitive("Lower Jaw", PrimitiveType.Cube, shell, new Vector3(side * 0.055f, -0.108f, -0.01f), Quaternion.Euler(0f, 0f, side * 22f),
                new Vector3(0.12f, 0.042f, 0.12f), edgeMetal, false);
            Primitive("Seal Tooth Upper", PrimitiveType.Cube, shell, new Vector3(side * 0.018f, 0.052f, -0.082f), Quaternion.identity,
                new Vector3(0.032f, 0.052f, 0.025f), dormant, false);
            Primitive("Seal Tooth Lower", PrimitiveType.Cube, shell, new Vector3(side * 0.018f, -0.052f, -0.082f), Quaternion.identity,
                new Vector3(0.032f, 0.052f, 0.025f), dormant, false);
        }

        static XRSimpleInteractable CreateSealHandle(Transform shell, float side, Material edgeMetal, Material amber)
        {
            var handle = Primitive(side < 0f ? "LEFT - Pull Handle" : "RIGHT - Pull Handle", PrimitiveType.Cube, shell,
                new Vector3(side * 0.178f, 0f, -0.01f), Quaternion.identity,
                new Vector3(0.075f, 0.13f, 0.09f), edgeMetal, true);
            var collider = handle.GetComponent<BoxCollider>();
            collider.isTrigger = true;

            Primitive("Grip Mark", PrimitiveType.Cube, handle.transform, new Vector3(0f, 0f, -0.052f), Quaternion.identity,
                new Vector3(0.04f, 0.072f, 0.012f), amber, false);

            var interactable = handle.AddComponent<XRSimpleInteractable>();
            interactable.colliders.Clear();
            interactable.colliders.Add(collider);
            return interactable;
        }

        static ArtifactRotaryMechanism CreateRotaryStage(
            Transform parent,
            Transform frame,
            Material darkMetal,
            Material edgeMetal,
            Material amber,
            Material dormant)
        {
            var pivot = NewObject("STAGE 1 - ALIGN OUTER COLLAR", parent).transform;
            pivot.localPosition = new Vector3(0f, 0f, -0.13f);
            const int segmentCount = 16;
            const float radius = 0.158f;
            var gripColliders = new List<Collider>();

            for (var i = 0; i < segmentCount; i++)
            {
                var angle = i * (360f / segmentCount);
                var radians = angle * Mathf.Deg2Rad;
                var position = new Vector3(Mathf.Cos(radians) * radius, Mathf.Sin(radians) * radius, 0f);
                var material = i % 4 == 0 ? edgeMetal : darkMetal;
                Primitive($"Collar Segment {i + 1:00}", PrimitiveType.Cube, pivot, position,
                    Quaternion.Euler(0f, 0f, angle + 90f), new Vector3(0.054f, 0.021f, 0.035f), material, false);
            }

            var gripAngles = new[] { 0f, 180f, 270f };
            for (var i = 0; i < gripAngles.Length; i++)
            {
                var angle = gripAngles[i];
                var radians = angle * Mathf.Deg2Rad;
                var grip = Primitive($"Large Collar Grip {i + 1}", PrimitiveType.Cube, pivot,
                    new Vector3(Mathf.Cos(radians) * radius, Mathf.Sin(radians) * radius, -0.014f),
                    Quaternion.Euler(0f, 0f, angle), new Vector3(0.075f, 0.052f, 0.065f),
                    i == 2 ? amber : edgeMetal, true);
                var collider = grip.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                gripColliders.Add(collider);
            }

            Primitive("Collar Alignment Needle", PrimitiveType.Cube, pivot, new Vector3(0f, 0.19f, 0f), Quaternion.identity,
                new Vector3(0.014f, 0.06f, 0.045f), amber, false);
            Primitive("Fixed Alignment Notch", PrimitiveType.Cube, frame, new Vector3(0.19f, 0f, -0.13f), Quaternion.Euler(0f, 0f, 90f),
                new Vector3(0.014f, 0.06f, 0.045f), dormant, false);

            var mechanism = pivot.gameObject.AddComponent<ArtifactRotaryMechanism>();
            mechanism.colliders.Clear();
            foreach (var collider in gripColliders)
                mechanism.colliders.Add(collider);
            return mechanism;
        }

        static ArtifactSliderMechanism CreateSliderStage(
            Transform parent,
            Transform frame,
            Material darkMetal,
            Material edgeMetal,
            Material cyan)
        {
            var rail = NewObject("STAGE 2 - DRAW SPINE RAIL", parent).transform;
            rail.localPosition = new Vector3(0f, 0.025f, -0.185f);
            Primitive("Revealed Energy Rail", PrimitiveType.Cube, rail, new Vector3(0f, 0.045f, 0f), Quaternion.identity,
                new Vector3(0.028f, 0.19f, 0.025f), cyan, false);
            Primitive("Rail Housing", PrimitiveType.Cube, rail, new Vector3(0f, 0.03f, 0.012f), Quaternion.identity,
                new Vector3(0.055f, 0.21f, 0.032f), darkMetal, false);

            var moving = NewObject("Sliding Spine and Grip", parent).transform;
            moving.localPosition = new Vector3(0f, -0.045f, -0.215f);
            Primitive("Spine", PrimitiveType.Cube, moving, new Vector3(0f, 0.03f, 0f), Quaternion.identity,
                new Vector3(0.052f, 0.17f, 0.045f), edgeMetal, false);
            var grip = Primitive("Large Pull Grip", PrimitiveType.Cube, moving, new Vector3(0f, 0.125f, 0f), Quaternion.identity,
                new Vector3(0.14f, 0.055f, 0.075f), edgeMetal, true);
            var collider = grip.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            Primitive("Spine Grip Mark", PrimitiveType.Cube, grip.transform, new Vector3(0f, 0f, -0.044f), Quaternion.identity,
                new Vector3(0.075f, 0.018f, 0.01f), cyan, false);

            var mechanism = moving.gameObject.AddComponent<ArtifactSliderMechanism>();
            mechanism.colliders.Clear();
            mechanism.colliders.Add(collider);
            return mechanism;
        }

        static GameObject CreateTargetIndicator(Transform parent, Material material)
        {
            var indicator = Primitive("Gauntlet Target Indicator", PrimitiveType.Sphere, parent, Vector3.zero, Quaternion.identity,
                new Vector3(0.38f, 0.38f, 0.38f), material, false);
            indicator.SetActive(false);
            return indicator;
        }

        static GameObject Primitive(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            Material material,
            bool keepCollider)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = localRotation;
            gameObject.transform.localScale = localScale;
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;

            if (!keepCollider)
            {
                var collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                    Object.DestroyImmediate(collider);
            }

            Undo.RegisterCreatedObjectUndo(gameObject, "Build UNDER Artifact Lock");
            return gameObject;
        }

        static GameObject NewObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name);
            if (parent != null)
                gameObject.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(gameObject, "Build UNDER Artifact Lock");
            return gameObject;
        }

        static Material Material(string name, Color baseColor, float metallic, float smoothness)
        {
            return CreateOrUpdateMaterial(name, baseColor, metallic, smoothness, Color.black, false);
        }

        static Material EmissiveMaterial(string name, Color baseColor, Color emission)
        {
            return CreateOrUpdateMaterial(name, baseColor, 0.45f, 0.38f, emission, true);
        }

        static Material CreateOrUpdateMaterial(
            string name,
            Color baseColor,
            float metallic,
            float smoothness,
            Color emission,
            bool emissive)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                    throw new System.InvalidOperationException("URP Lit shader is unavailable.");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", baseColor);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (emissive)
            {
                material.EnableKeyword("_EMISSION");
                if (material.HasProperty("_EmissionColor")) material.SetColor("_EmissionColor", emission);
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
#endif
