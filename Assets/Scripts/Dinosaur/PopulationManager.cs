using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] float minPairDstThreshold = 50f;

    [SerializeField] List<Transform> spawnedDinosaurs = new List<Transform>();
    [SerializeField] Queue<Transform> availableDinosaurs = new Queue<Transform>();

    private void Start()
    {
        StartCoroutine(TryFindPair());
    }

    IEnumerator TryFindPair()
    {
        while (true)
        {
            if (availableDinosaurs.Count > 1)
                CreatePair(availableDinosaurs.Dequeue(), availableDinosaurs.Dequeue());

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreatePair(Transform dinosaurFirst, Transform dinosaurSecond)
    {
        DinosaurPair connection = gameObject.AddComponent<DinosaurPair>();
        connection.DinosaurFirst = dinosaurFirst;
        connection.DinosaurSecond = dinosaurSecond;
    }

    public void AddDinosaur(Transform dinosaur)
    {
        spawnedDinosaurs.Add(dinosaur);
    }

    public void AddDinosaurToAvailable(Transform dinosaur)
    {
        availableDinosaurs.Enqueue(dinosaur);
    }

}
