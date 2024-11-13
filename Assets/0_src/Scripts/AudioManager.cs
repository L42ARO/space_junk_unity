using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioManager : MonoBehaviour
{
    public AudioSource startAudio;
    private bool hasPlayedStartAudio=false;
    private List<AudioSource> audioSources;
    private List<int> unplayedIndices;
    private int currentIndex = -1;
    public string currentAudioName;

    public bool startPlaying = false;
    private bool startedQueue = false;
    public void StartPlaying(){
        startPlaying = true;
    }
    public void DontStartPlaying(){
        startPlaying = false;
    }

    void Start()
    {
        // Initialize the list of AudioSources from child objects
        audioSources = new List<AudioSource>();
        foreach (Transform child in transform)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null)
            {
                audioSources.Add(source);
            }
        }

        // Handle case where no audio sources are found
        if (audioSources.Count == 0)
        {
            Debug.LogWarning("No audio sources found as children of the AudioManager.");
            return;
        }

        #if UNITY_EDITOR
        // Load the list of unplayed indices or initialize if not present
        unplayedIndices = ReadCurrentIndices();
        if (unplayedIndices == null || unplayedIndices.Count == 0)
        {
            ResetUnplayedIndices(); // Create new indices if loading failed or empty
            SaveCurrentIndices(unplayedIndices); // Ensure it's saved for future use
        }
        #else
        // Prepare the list of unplayed indices
        ResetUnplayedIndices();
        #endif

    }

    void Update(){
        if(startPlaying && !startedQueue){
            startedQueue = true;
            if(!hasPlayedStartAudio && startAudio != null){
                float maxStartTime = Mathf.Max(0, startAudio.clip.length - 10f);
                float randomStartTime = Random.Range(0, maxStartTime);
                startAudio.time = randomStartTime;
                startAudio.Play();
                hasPlayedStartAudio = true;
                StartCoroutine(StopAudioAfterDuration(startAudio, 6));
            }
            else{
                PlayRandomAudio();
            }
        }
    }

    IEnumerator StopAudioAfterDuration(AudioSource audio, float duration){
        yield return new WaitForSeconds(duration);
        if(audio.isPlaying) audio.Stop();

        PlayRandomAudio();
    }

    void PlayRandomAudio()
    {
        if (unplayedIndices.Count == 0)
        {
            ResetUnplayedIndices(); // All clips have been played, reset the list
        }

        // Select a random index from the unplayed list
        int randomIndex = Random.Range(0, unplayedIndices.Count);
        currentIndex = unplayedIndices[randomIndex];
        unplayedIndices.RemoveAt(randomIndex);

        // Play the selected audio
        AudioSource selectedAudio = audioSources[currentIndex];
        selectedAudio.Play();
        currentAudioName = selectedAudio.gameObject.name;

        #if UNITY_EDITOR
        SaveCurrentIndices(unplayedIndices);
        #endif

        // Schedule to play the next audio when this one finishes
        StartCoroutine(WaitForAudioToEnd(selectedAudio));
    }

    IEnumerator WaitForAudioToEnd(AudioSource audio)
    {
        yield return new WaitWhile(() => audio.isPlaying);
        PlayRandomAudio(); // Play another audio clip
    }

    void ResetUnplayedIndices()
    {
        unplayedIndices = new List<int>();
        for (int i = 0; i < audioSources.Count; i++)
        {
            unplayedIndices.Add(i);
        }
    }

    #if UNITY_EDITOR

    [System.Serializable]
    private class IndicesWrapper
    {
        public List<int> indices;
    }

    // Function to store the current unplayed indices in a json file
    public void SaveCurrentIndices(List<int> indices)
    {
        string name = "played_indices.json";
        string path = Application.persistentDataPath + "/" + name;

        // Wrap indices in a class to serialize properly
        IndicesWrapper wrapper = new IndicesWrapper { indices = indices };
        string json = JsonUtility.ToJson(wrapper);
        Debug.Log("Saving: " + json);

        try
        {
            System.IO.File.WriteAllText(path, json);
            Debug.Log("Saved played indices to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save played indices: " + e.Message);
        }
    }

    // Function to load the current unplayed indices from a json file
    List<int> ReadCurrentIndices()
    {
        string name = "played_indices.json";
        string path = Application.persistentDataPath + "/" + name;
        List<int> indices = new List<int>();

        if (System.IO.File.Exists(path))
        {
            try
            {
                // Read the json file
                string json = System.IO.File.ReadAllText(path);
                IndicesWrapper wrapper = JsonUtility.FromJson<IndicesWrapper>(json);
                if (wrapper != null && wrapper.indices != null)
                {
                    indices = wrapper.indices;
                }

                // Print what was loaded
                string loadedIndices = string.Join(", ", indices);
                Debug.Log("Loaded: " + loadedIndices);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load played indices: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Indices file not found, initializing new indices.");
        }

        return indices;
    }
    #endif
}
