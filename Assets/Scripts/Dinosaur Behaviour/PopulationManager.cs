using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    List<GameObject> spawnedDinosaurs = new List<GameObject>();
    public List<GameObject> idleDinosaurs = new List<GameObject>();

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

                CreateMatch(idleDinosaurs[firstDinosaurIndex], idleDinosaurs[secondDinosaurIndex]);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreateMatch(GameObject dinosaurFirst, GameObject dinosaurSecond)
    {
        Unit unitInstanceFist = dinosaurFirst.GetComponent<Unit>();
        Unit unitInstanceSecond = dinosaurSecond.GetComponent<Unit>();

        unitInstanceFist.target = dinosaurSecond.transform;
        unitInstanceSecond.target = dinosaurFirst.transform;

        idleDinosaurs.Remove(dinosaurFirst);
        idleDinosaurs.Remove(dinosaurSecond);
    }

    public void AddDinosaur(GameObject dinosaur)
    {
        spawnedDinosaurs.Add(dinosaur);
        idleDinosaurs.Add(dinosaur);
    }

    public void AddToIdle(GameObject dinosaur)
    {
        idleDinosaurs.Add(dinosaur);
    }

}
