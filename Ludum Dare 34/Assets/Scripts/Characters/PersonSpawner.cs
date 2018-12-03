using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour {

    public int maxMales = 50, maxFemales = 50, currentMales = 0, currentFemales = 0;

    public int minSpawn, maxSpawn;

    public static PersonSpawner instance;

    public string[] possibleGenders = new string[2];

    public string[] possibleMaleNames = new string[10];

    public string[] possibleFemaleNames = new string[10];

    public int minStartAge, maxStartAge;

    public int minStatHappiness, maxStartHappiness;

    public int minPersonSpeed, maxPersonSpeed;

    public Material maleMaterial, femaleMaterial;

    private Vector3 spawnPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        #region Possible Genders
        possibleGenders[0] = "Male";
        possibleGenders[1] = "Female";
        #endregion

        #region Possible Male Names
        possibleMaleNames[0] = "Liam";
        possibleMaleNames[1] = "Noah";
        possibleMaleNames[2] = "William";
        possibleMaleNames[3] = "James";
        possibleMaleNames[4] = "Logan";
        possibleMaleNames[5] = "Ben";
        possibleMaleNames[6] = "Mason";
        possibleMaleNames[7] = "Rick";
        possibleMaleNames[8] = "Morty";
        possibleMaleNames[9] = "Josh";
        #endregion

        #region Possible Female Names
        possibleFemaleNames[0] = "Emma";
        possibleFemaleNames[1] = "Olivia";
        possibleFemaleNames[2] = "Ava";
        possibleFemaleNames[3] = "Isabella";
        possibleFemaleNames[4] = "Sophia";
        possibleFemaleNames[5] = "Mia";
        possibleFemaleNames[6] = "Charlotte";
        possibleFemaleNames[7] = "Amelia";
        possibleFemaleNames[8] = "Ella";
        possibleFemaleNames[9] = "Victoria";
        #endregion
    }

    public void spawnPerson()
    {
        if (Random.value < 0.3)
        {
            spawnPos = new Vector3(transform.position.x + Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z - Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value > 0.3 && Random.value < 0.5)
        {
            spawnPos = new Vector3(transform.position.x - Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z + Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value > 0.5 && Random.value < 0.7)
        {
            spawnPos = new Vector3(transform.position.x + Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z + Random.Range(minSpawn, maxSpawn));
        }
        else if (Random.value < 0.7)
        {
            spawnPos = new Vector3(transform.position.x - Random.Range(minSpawn, maxSpawn), transform.position.y, transform.position.z - Random.Range(minSpawn, maxSpawn));
        }

        Ray ray = Camera.main.ScreenPointToRay(spawnPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.CompareTag("Towns Person") || !hit.collider.CompareTag("House"))
            {
                GameObject spawnedPerson = Instantiate(TownController.instance.townPersonPrefab, spawnPos, Quaternion.identity, TownController.instance.spawnedPersonParent.transform);

                TownPersonController tpc = spawnedPerson.GetComponent<TownPersonController>();

                setPersonStats(tpc);

                spawnedPerson.name = tpc.getPersonName();

                if (tpc.getPersonGender() == "Male")
                {
                    spawnedPerson.GetComponentInChildren<MeshRenderer>().material = maleMaterial;
                }else if (tpc.getPersonGender() == "Female")
                {
                    spawnedPerson.GetComponentInChildren<MeshRenderer>().material = femaleMaterial;
                }

                TownController.instance.townPersonsTransforms.Add(spawnedPerson.transform);
                TownController.instance.townCurrentSize = TownController.instance.townPersonsTransforms.ToArray().Length;

                UIController.instance.updatePopulationText();
            }
        }
    }

    public void setPersonStats(TownPersonController _personController)
    {
        int personsGender = Random.Range(0, 1);

        if (personsGender < 3)
        {
            if (currentMales < maxMales)
            {
                _personController.setPersonGender(possibleGenders[0]);
                currentMales++;

                if (_personController.getPersonGender() == "Male")
                {
                    int maleName = Random.Range(0, 9);

                    _personController.setPersonName(possibleMaleNames[maleName]);
                }

                UIController.instance.updateSliders();
            }
            else
            {
                if (currentFemales < maxFemales)
                {
                    _personController.setPersonGender(possibleGenders[1]);
                    currentFemales++;

                    if (_personController.getPersonGender() == "Female")
                    {
                        int femaleName = Random.Range(0, 9);

                        _personController.setPersonName(possibleFemaleNames[femaleName]);
                    }

                    UIController.instance.updateSliders();
                }
            }
        }else if (personsGender > 3)
        {
            if (currentFemales < maxFemales)
            {
                _personController.setPersonGender(possibleGenders[1]);
                currentFemales++;

                if (_personController.getPersonGender() == "Female")
                {
                    int femaleName = Random.Range(0, 9);

                    _personController.setPersonName(possibleFemaleNames[femaleName]);
                }

                UIController.instance.updateSliders();
            }
            else
            {
                if (currentMales < maxMales)
                {
                    _personController.setPersonGender(possibleGenders[0]);
                    currentMales++;

                    if (_personController.getPersonGender() == "Male")
                    {
                        int maleName = Random.Range(0, 9);

                        _personController.setPersonName(possibleMaleNames[maleName]);
                    }

                    UIController.instance.updateSliders();
                }
            }
        }

        _personController.setPersonHappiness(Random.Range(minStatHappiness, maxStartHappiness));

        _personController.setPersonAge(Random.Range(minStartAge, maxStartAge));

        _personController.setPersonSpeed(Random.Range(minPersonSpeed, maxPersonSpeed));
    }
}
