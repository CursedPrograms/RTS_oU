using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class MusicPlaylist : MonoBehaviour
{
    private AudioSource _audioSource;
    private int _currentSongIndex = 0;
    private bool _isSongPlaying = false;

    public bool playMusic = true;
    public AudioClip[] songs;

    private void Start()
    {
        InitializeAudioSource();
        Application.runInBackground = true;
    }

    private void InitializeAudioSource()
    {
        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (playMusic && !_isSongPlaying)
        {
            PlayNextSong();
        }

        if (!_audioSource.isPlaying)
        {
            _isSongPlaying = false;
        }
    }

    private void PlayNextSong()
    {
        if (songs.Length == 0) return;

        _audioSource.clip = songs[_currentSongIndex];
        _audioSource.Play();
        _currentSongIndex = (_currentSongIndex + 1) % songs.Length;
        _isSongPlaying = true;
    }
}
