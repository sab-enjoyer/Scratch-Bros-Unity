using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public List<AudioClip> songs;

    public AudioClip trainingSong;

    public Dictionary<string, AudioClip> songNames;

    private AudioSource audioSource;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = gameObject.GetComponent<AudioSource>();
            songNames = new Dictionary<string, AudioClip>()
            {
                {"Title", songs[0]},
                {"Vault", songs[0] },
                {"Options", songs[1] },
                {"Stage Select", songs[1] },
                {"Character Select", songs[2] }
            };
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += PlaySongOnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= PlaySongOnSceneLoad;
    }

    private void PlaySongOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        audioSource.loop = true;
        if (scene.name == "Win Screen")
        {
            audioSource.loop = false;
            audioSource.Stop();
            audioSource.clip = GameManager.instance.characters[GameManager.instance.playerStore[GameManager.instance.gameWinner].charIndex].theme;
            audioSource.Play();
        }
        if (scene.name == "Game")
        {
            audioSource.Stop();
            if (GameManager.instance.gameType == GameManager.GameTypes.Training)
                audioSource.clip = trainingSong;
            else
                audioSource.clip = GameManager.instance.maps[GameManager.instance.mapID].song;

            audioSource.Play();
        }
        else if (songNames.ContainsKey(scene.name) && GameManager.instance.gameType != GameManager.GameTypes.Online)
        {
            if (songNames[scene.name] != audioSource.clip)
            {
                audioSource.Stop();
                audioSource.clip = songNames[scene.name];
                audioSource.Play();
            }

        }
    }
}
