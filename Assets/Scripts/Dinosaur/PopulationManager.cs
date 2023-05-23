using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] float minMatchDstThreshold = 50f;

    [SerializeField] List<Transform> spawnedDinosaurs = new List<Transform>();
    [SerializeField] List<Transform> idleDinosaurs = new List<Transform>(); // to remove public later

    float matchDstThresholdSqr; 

    private void Awake()
    {
        matchDstThresholdSqr = minMatchDstThreshold * minMatchDstThreshold;
    }

    private void Start()
    {
        StartCoroutine("TryFindMatch");
    }

    IEnumerator TryFindMatch()
    {
        while (true)
        {
            int idleDinosaurCount = idleDinosaurs.Count;

            if (idleDinosaurCount > 1)
            {
                int firstDinosaurIndex;
                int secondDinosaurIndex;

                firstDinosaurIndex = secondDinosaurIndex = Random.Range(0, idleDinosaurCount);

                while (firstDinosaurIndex == secondDinosaurIndex)
                {
                    secondDinosaurIndex = Random.Range(0, idleDinosaurCount);
                }

                Transform dinosaurFirst = idleDinosaurs[firstDinosaurIndex].transform;
                Transform dinosaurSecond = idleDinosaurs[secondDinosaurIndex].transform;

                if ((dinosaurFirst.position - dinosaurSecond.position).sqrMagnitude > matchDstThresholdSqr)
                    CreateMatch(dinosaurFirst, dinosaurSecond);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreateMatch(Transform dinosaurFirst, Transform dinosaurSecond)
    {
        Unit unitInstanceFist = dinosaurFirst.GetComponent<Unit>();
        Unit unitInstanceSecond = dinosaurSecond.GetComponent<Unit>();

        unitInstanceFist.target = dinosaurSecond.transform;
        unitInstanceSecond.target = dinosaurFirst.transform;

        idleDinosaurs.Remove(dinosaurFirst);
        idleDinosaurs.Remove(dinosaurSecond);
    }

    public void AddDinosaur(Transform dinosaur)
    {
        spawnedDinosaurs.Add(dinosaur);
        idleDinosaurs.Add(dinosaur);
    }

    public void AddToIdle(Transform dinosaur)
    {
        idleDinosaurs.Add(dinosaur);
    }

}
