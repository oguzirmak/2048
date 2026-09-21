using UnityEngine;

namespace Game2048.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class GameAudioController : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip mergeClip;
        [SerializeField] private AudioClip spawnClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip win2048Clip;
        [SerializeField] private AudioClip gameOverClip;

        [Header("Volumes")]
        [SerializeField, Range(0f, 1f)] private float moveVolume = 0.45f;
        [SerializeField, Range(0f, 1f)] private float mergeVolume = 0.65f;
        [SerializeField, Range(0f, 1f)] private float spawnVolume = 0.45f;
        [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 0.55f;
        [SerializeField, Range(0f, 1f)] private float win2048Volume = 0.80f;
        [SerializeField, Range(0f, 1f)] private float gameOverVolume = 0.75f;

        private AudioSource audioSource;
        private bool warnedAboutAudioSource;
        private bool warnedAboutMoveClip;
        private bool warnedAboutMergeClip;
        private bool warnedAboutSpawnClip;
        private bool warnedAboutButtonClickClip;
        private bool warnedAboutWin2048Clip;
        private bool warnedAboutGameOverClip;

        private void Awake()
        {
            ConfigureAudioSource();
        }

        private void Reset()
        {
            ConfigureAudioSource();
        }

        private void OnValidate()
        {
            ConfigureAudioSource();
        }

        public void PlayMove()
        {
            PlayClip(moveClip, moveVolume, ref warnedAboutMoveClip, "Move");
        }

        public void PlayMerge()
        {
            PlayClip(mergeClip, mergeVolume, ref warnedAboutMergeClip, "Merge");
        }

        public void PlaySpawn()
        {
            PlayClip(spawnClip, spawnVolume, ref warnedAboutSpawnClip, "Spawn");
        }

        public void PlayButtonClick()
        {
            PlayClip(buttonClickClip, buttonClickVolume, ref warnedAboutButtonClickClip, "Button Click");
        }

        public void PlayWin()
        {
            PlayClip(win2048Clip, win2048Volume, ref warnedAboutWin2048Clip, "Win 2048");
        }

        public void PlayGameOver()
        {
            PlayClip(gameOverClip, gameOverVolume, ref warnedAboutGameOverClip, "Game Over");
        }

        private void ConfigureAudioSource()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null) return;

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
        }

        private void PlayClip(AudioClip clip, float volume, ref bool hasWarned, string clipName)
        {
            if (audioSource == null) ConfigureAudioSource();
            if (audioSource == null)
            {
                if (!warnedAboutAudioSource)
                {
                    warnedAboutAudioSource = true;
                    Debug.LogWarning($"GameAudioController on '{name}' requires an AudioSource.", this);
                }
                return;
            }

            if (clip == null)
            {
                if (!hasWarned)
                {
                    hasWarned = true;
                    Debug.LogWarning(
                        $"GameAudioController on '{name}' has no {clipName} AudioClip assigned.",
                        this);
                }
                return;
            }

            audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
