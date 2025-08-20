namespace SatProductions
{
    using UnityEngine;
    using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BreakableObject : MonoBehaviour
    {
        [Header("Explosion Settings")]
        [Tooltip("Controls both the force and radius of the explosion.")]
        public float explosionPower = 5f;

        [Header("Audio Settings")]
        public AudioClip breakSound;
        public AudioClip spawnSound;
        [Range(0f, 1f)] public float volume = 0.8f;

        [Header("Debug & Optimization")]
        public bool showGizmos = true;
        public bool destroyPiecesAfterDelay = true;
        public float destroyDelay = 5f;
        public float fadeOutDuration = 2f;
        public bool BreakOnHit = false;

        private GameObject originalObject;
        private GameObject brokenPiecesParent;
        private Rigidbody[] fragments;
        private Renderer[] fragmentRenderers;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private bool isBroken = false;
        private Coroutine fadeCoroutine;
        private AudioSource tempAudioSource;

        private Transform[] pieceTransforms;
        private Vector3[] piecePositions;
        private Quaternion[] pieceRotations;
        private Vector3[] pieceScales;

        void Awake()
        {
            if (transform.childCount < 2)
            {
                Debug.LogError("BreakableObject: Missing child objects!");
                return;
            }

            originalObject = transform.GetChild(0).gameObject;
            brokenPiecesParent = transform.GetChild(1).gameObject;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalScale = transform.localScale;

            fragmentRenderers = brokenPiecesParent.GetComponentsInChildren<Renderer>();

            originalObject.SetActive(true);
            brokenPiecesParent.SetActive(false);

            SavePieceTransforms();
        }

        private void SavePieceTransforms()
        {
            pieceTransforms = brokenPiecesParent.GetComponentsInChildren<Transform>();
            piecePositions = new Vector3[pieceTransforms.Length - 1];
            pieceRotations = new Quaternion[pieceTransforms.Length - 1];
            pieceScales = new Vector3[pieceTransforms.Length - 1];

            for (int i = 1; i < pieceTransforms.Length; i++)
            {
                piecePositions[i - 1] = pieceTransforms[i].localPosition;
                pieceRotations[i - 1] = pieceTransforms[i].localRotation;
                pieceScales[i - 1] = pieceTransforms[i].localScale;
            }
        }

        public void Break(float forceMultiplier = 1f)
        {
            if (isBroken || !originalObject || !brokenPiecesParent) return;
            isBroken = true;

            // Play break sound if available
            if (breakSound != null)
            {
                PlayTempAudio(breakSound);
            }

            originalObject.SetActive(false);
            brokenPiecesParent.SetActive(true);

            GetComponent<Collider>().isTrigger = true;

            fragments = new Rigidbody[pieceTransforms.Length - 1];

            for (int i = 1; i < pieceTransforms.Length; i++)
            {
                GameObject piece = pieceTransforms[i].gameObject;

                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb == null) rb = piece.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.isKinematic = false;


                MeshCollider collider = piece.GetComponent<MeshCollider>();
                if (collider == null)
                {
                    MeshFilter meshFilter = piece.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        collider = piece.AddComponent<MeshCollider>();
                        collider.convex = true;
                    }
                }

                fragments[i - 1] = rb;

                float randomForce = Random.Range(explosionPower * 0.5f, explosionPower * 1.5f);
                rb.AddExplosionForce(randomForce * forceMultiplier, gameObject.transform.position, explosionPower, 0.5f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * randomForce * 0.5f, ForceMode.Impulse);
            }

            if (destroyPiecesAfterDelay)
            {
                fadeCoroutine = StartCoroutine(FadeOutAndDisablePieces());
            }
        }

        private void PlayTempAudio(AudioClip clip)
        {
            if (clip == null) return;

            // Create temporary audio source
            tempAudioSource = gameObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.volume = volume;
            tempAudioSource.spatialBlend = 1f; // 3D sound
            tempAudioSource.Play();

            // Destroy after clip finishes
            Destroy(tempAudioSource, clip.length);
        }

        private IEnumerator FadeOutAndDisablePieces()
        {
            yield return new WaitForSeconds(destroyDelay);

            foreach (Renderer renderer in fragmentRenderers)
            {
                if (renderer != null)
                {
                    StartCoroutine(FadeAndDisablePiece(renderer));
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        private IEnumerator FadeAndDisablePiece(Renderer renderer)
        {
            Material material = renderer.material;
            Color initialColor = material.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeOutDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
            renderer.gameObject.SetActive(false);
        }

        public void FixObject()
        {
            if (!isBroken || !originalObject || !brokenPiecesParent) return;
            isBroken = false;

            // Play spawn sound if available
            if (spawnSound != null)
            {
                PlayTempAudio(spawnSound);
            }

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalScale = transform.localScale;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            StopAllCoroutines();

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;

            brokenPiecesParent.SetActive(false);

            GetComponent<Collider>().isTrigger = false;

            for (int i = 1; i < pieceTransforms.Length; i++)
            {
                Transform piece = pieceTransforms[i];

                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                MeshCollider collider = piece.GetComponent<MeshCollider>();
                if (collider != null) Destroy(collider);

                piece.localPosition = piecePositions[i - 1];
                piece.localRotation = pieceRotations[i - 1];
                piece.localScale = pieceScales[i - 1];
                piece.gameObject.SetActive(true);
            }

            foreach (Renderer renderer in fragmentRenderers)
            {
                if (renderer != null)
                {
                    Material material = renderer.material;
                    Color color = material.color;
                    material.color = new Color(color.r, color.g, color.b, 1f);
                }
            }

            originalObject.SetActive(true);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            // Get the current scene camera
            if (SceneView.currentDrawingSceneView != null)
            {
                Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
                if (sceneCamera == null) return;

                // Calculate the distance between the object and the scene camera
                float distanceToCamera = Vector3.Distance(sceneCamera.transform.position, transform.position);

                // Define the maximum distance at which the label is visible
                float maxLabelDistance = 50f; // Adjust this value as needed

                // Draw explosion radius with gradient
                Handles.color = new Color(1f, 0.5f, 0f, 0.2f);
                Handles.DrawSolidDisc(transform.position, Vector3.up, explosionPower);

                Handles.color = new Color(1f, 0.5f, 0f, 0.5f);
                Handles.DrawWireDisc(transform.position, Vector3.up, explosionPower);

                // Draw the label only if the camera is within the max distance
                if (distanceToCamera <= maxLabelDistance)
                {
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = new Color(1f, 0.5f, 0f);
                    style.alignment = TextAnchor.MiddleCenter;
                    Handles.Label(transform.position + Vector3.up * explosionPower, $"Explosion Power: {explosionPower}", style);
                }
            }
        }
#endif
    }
}