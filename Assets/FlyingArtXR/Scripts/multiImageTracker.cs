using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class multiImageTracker : MonoBehaviour
{
    private ARTrackedImageManager TrackedImageManager;
    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects;

    private TrackingState tracking;
    // Start is called before the first frame update
    private void Awake()
    {
        TrackedImageManager = GetComponent<ARTrackedImageManager>();
        spawnedObjects = new Dictionary<string, GameObject>();

        foreach (GameObject obj in placeablePrefabs)
        {
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            newObject.SetActive(false);

            spawnedObjects.Add(newObject.name, newObject);
        }
    }
    private void OnEnable()
    {
        TrackedImageManager.trackedImagesChanged += ontrackedImageChanged;
    }
    private void OnDisable()
    {
        TrackedImageManager.trackedImagesChanged -= ontrackedImageChanged;

    }
    void ontrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateSpawnObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateSpawnObject(trackedImage);

        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            print("trackedimage name = "+trackedImage.name);
            print("trackedimage reference name = " + trackedImage.referenceImage.name);
            spawnedObjects[trackedImage.referenceImage.name].SetActive(false);
        }
    }


    void UpdateSpawnObject(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;
        spawnedObjects[referenceImageName].transform.position = trackedImage.transform.position;
        spawnedObjects[referenceImageName].transform.rotation = trackedImage.transform.rotation;
        spawnedObjects[referenceImageName].SetActive(true);
        foreach (GameObject go in spawnedObjects.Values)
        {
            Debug.Log($"Go in arObjects.Values: {go.name}");
            if (go.name != referenceImageName)
            {
                go.SetActive(false);
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public void resetARSession()
    {
        ARSession session = FindObjectOfType<ARSession>();
        session.Reset();
    }


}
