using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TownPersonController : MonoBehaviour {
    public NavMeshAgent personAgent;

    public TownPersonAI tpa;

    public bool isControlling = false;

    private string personName = "Anna";

    private string personGender = "Female";

    private int personAge = 20;

    private float personHealth = 100;
    private float personSpeed = 5;
    private float personHappiness = 50, maxHappiness = 100;

    public int currentChildren = 0, maxChildren = 4;

    private int happinessIncreaseIncrement = 15, happinessDecreaseIncrement = 4;

    private int currentHappinessDelay, maxHappinessDelay = 120;

    public int matingRange = 5, talkRange = 15, useRange = 25;

    public int currentBoredDelay, maxBoredDelay = 600;

    private int moveDistance = 30;

    private Transform personHome = null;

    private Transform personParent = null;

    private Transform currentMate = null;

    private TownPersonController mateTCP;

    private bool decreasingHappiness =  false, increasingHappiness = false;

    public List<Transform> children;

    private void Awake()
    {
        personAgent = GetComponent<NavMeshAgent>();

        tpa = GetComponent<TownPersonAI>();

        currentHappinessDelay = maxHappinessDelay;

        currentBoredDelay = maxBoredDelay;
    }

    private void Update()
    {
        if (!GameController.instance.isPaused)
        {
            if (!increasingHappiness || !decreasingHappiness)
            {
                StartCoroutine(happinessManagement());
            }

            if (!isControlling)
            {
                currentBoredDelay--;
                if (currentBoredDelay <= maxBoredDelay)
                {
                    decreasePersonHappiness();
                    resetBoredDelay();
                }
            }

            if (!isControlling)
            {
                tpa.personAI();
            }else if (isControlling)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("Ground"))
                        {
                            float distance = Vector3.Distance(transform.position, hit.point);

                            if (distance < moveDistance)
                            {
                                movePersonPosition(hit.point);
                                increasePersonHappiness();
                                resetBoredDelay();
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator happinessManagement()
    {
        yield return new WaitForEndOfFrame();
        #region Increase Happiness
        if (TownController.instance.townCurrentSize <= TownController.instance.townMaxSize)
        {
            UIController.instance.updateHappinessInfoText(increasePersonHappiness());
        }
        if (TownController.instance.currentTownHappiness >= TownController.instance.minTownHappiness)
        {
            UIController.instance.updateHappinessInfoText(increasePersonHappiness());
        }
        #endregion

        yield return new WaitForEndOfFrame();
        #region Decrease Happiness
        if (TownController.instance.townCurrentSize > TownController.instance.townMaxSize)
        {
            UIController.instance.updateHappinessInfoText(decreasePersonHappiness());
        }

        if (TownController.instance.currentTownHappiness < TownController.instance.minTownHappiness)
        {
            UIController.instance.updateHappinessInfoText(decreasePersonHappiness());
        }

        if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled / 2)
        {
            UIController.instance.updateHappinessInfoText(decreasePersonHappiness());
        }

        if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled)
        {
            UIController.instance.updateHappinessInfoText(decreasePersonHappiness(true));
        }

        #endregion
        yield return new WaitForEndOfFrame();
    }

    public void createChild(Transform parent)
    {
        GameObject createdChild = Instantiate(TownController.instance.townPersonPrefab, new Vector3(parent.position.x, parent.position.y, parent.position.z - 50), Quaternion.identity, parent);

        setChildSettings(createdChild);

        TownController.instance.addTownsPerson(createdChild.transform);

        tpa.setCurrentTask("none");

        return;
    }

    public void setChildSettings(GameObject child)
    {
        TownPersonController tpc = child.GetComponent<TownPersonController>();

        int personsGender = Random.Range(0, 1);

        if (personsGender < 3)
        {
            if (PersonSpawner.instance.currentMales < PersonSpawner.instance.maxMales)
            {
                tpc.setPersonGender(PersonSpawner.instance.possibleGenders[0]);
                PersonSpawner.instance.currentMales++;

                if (tpc.getPersonGender() == "Male")
                {
                    int maleName = Random.Range(0, 9);

                    tpc.setPersonName(PersonSpawner.instance.possibleMaleNames[maleName]);
                }

                UIController.instance.updateSliders();
            }
            else
            {
                if (PersonSpawner.instance.currentFemales < PersonSpawner.instance.maxFemales)
                {
                    tpc.setPersonGender(PersonSpawner.instance.possibleGenders[1]);
                    PersonSpawner.instance.currentFemales++;

                    if (tpc.getPersonGender() == "Female")
                    {
                        int femaleName = Random.Range(0, 9);

                        tpc.setPersonName(PersonSpawner.instance.possibleFemaleNames[femaleName]);
                    }

                    UIController.instance.updateSliders();
                }
            }
        }
        else if (personsGender > 3)
        {
            if (PersonSpawner.instance.currentFemales < PersonSpawner.instance.maxFemales)
            {
                tpc.setPersonGender(PersonSpawner.instance.possibleGenders[1]);
                PersonSpawner.instance.currentFemales++;

                if (tpc.getPersonGender() == "Female")
                {
                    int femaleName = Random.Range(0, 9);

                    tpc.setPersonName(PersonSpawner.instance.possibleFemaleNames[femaleName]);
                }

                UIController.instance.updateSliders();
            }
            else
            {
                if (PersonSpawner.instance.currentMales < PersonSpawner.instance.maxMales)
                {
                    tpc.setPersonGender(PersonSpawner.instance.possibleGenders[0]);
                    PersonSpawner.instance.currentMales++;

                    if (tpc.getPersonGender() == "Male")
                    {
                        int maleName = Random.Range(0, 9);

                        tpc.setPersonName(PersonSpawner.instance.possibleMaleNames[maleName]);
                    }

                    UIController.instance.updateSliders();
                }
            }
        }

        tpc.setPersonHappiness(Random.Range(PersonSpawner.instance.minStatHappiness, PersonSpawner.instance.maxStartHappiness));

        tpc.setPersonAge(Random.Range(1, 2));

        tpc.setPersonSpeed(Random.Range(PersonSpawner.instance.minPersonSpeed, PersonSpawner.instance.maxPersonSpeed));
    }

    public float increasePersonHappiness()
    {
        if (personHappiness < maxHappiness)
        {
            increasingHappiness = true;

            currentHappinessDelay--;

            if (currentHappinessDelay <= 0)
            {
                personHappiness += happinessIncreaseIncrement;

                currentHappinessDelay = maxHappinessDelay;

                increasingHappiness = false;
                return personHappiness;
            }
        }

        increasingHappiness = false;
        return personHappiness;
    }

    public float decreasePersonHappiness(bool doubleHit = false)
    {
        if (!doubleHit)
        {
            if (personHappiness > 0)
            {
                decreasingHappiness = true;
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessDecreaseIncrement;

                    currentHappinessDelay = maxHappinessDelay;

                    decreasingHappiness = false;
                    return personHappiness;
                }

                decreasingHappiness = false;
                return personHappiness;
            }
        }
        else if (doubleHit)
        {
            if (personHappiness > 0)
            {
                decreasingHappiness = true;
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessDecreaseIncrement * 2;

                    currentHappinessDelay = maxHappinessDelay;

                    decreasingHappiness = false;
                    return personHappiness;
                }
                decreasingHappiness = false;
                return personHappiness;
            }
        }

        return personHappiness;
    }

    public void movePersonPosition(Vector3 _destination)
    {
        if (!personAgent.pathPending)
        {
            personAgent.SetDestination(_destination);
        }
    }

    public void takePersonHealth(float _amount)
    {
        if (personHealth > 0)
        {
            float finalHealth = personHealth - _amount;

            if (finalHealth >= 0)
            {
                personHealth = finalHealth;
            }
            else
            {
                killPerson();
            }
        }
        else
        {
            killPerson();
        }
    }

    public void killPerson()
    {
        Destroy(gameObject);
    }

    public void setMate(Transform mate)
    {
        currentMate = mate;
        mateTCP = mate.GetComponent<TownPersonController>();
    }

    public void resetBoredDelay()
    {
        currentBoredDelay = maxBoredDelay;
    }

    #region Getters
    public string getPersonName()
    {
        return personName;
    }

    public float getPersonHealth()
    {
        return personHealth;
    }

    public string getPersonGender()
    {
        return personGender;
    }

    public float getPersonSpeed()
    {
        return personSpeed;
    }

    public float getPersonHappiness()
    {
        return personHappiness;
    }

    public int getCurrentChildren()
    {
        return currentChildren;
    }

    public int getMaxChildren()
    {
        return maxChildren;
    }

    public int getPersonAge()
    {
        return personAge;
    }

    public Transform getPersonsPartner()
    {
        return currentMate;
    }

    public Transform getPersonsParent()
    {
        return personParent;
    }

    public Transform getPersonHome()
    {
        return personHome;
    }
    #endregion

    #region Setters
    public void setPersonName(string _name)
    {
        personName = _name;
    }

    public void setPersonGender(string _gender)
    {
        personGender = _gender;
    }

    public void setPersonAge(int _age)
    {
        personAge = _age;
    }

    public void setPersonHappiness(int _happiness)
    {
        personHappiness = _happiness;
    }

    public void setPersonHealth(float _health)
    {
        personHealth = _health;
    }

    public void setPersonSpeed(float _speed)
    {
        personSpeed = _speed;

        personAgent.speed = personSpeed;
    }

    public void setMaxChildren(int _max)
    {
        maxChildren = _max;
    }

    public void setPersonsPartner(Transform _partner)
    {
        currentMate = _partner;
    }
    #endregion
}
