using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TownPersonController : MonoBehaviour {
    public NavMeshAgent personAgent;

    public TownPersonAI tpa;

    private bool dumbMode = false;

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

    public GameObject happyIcon, madIcon, matingIcon;

    public bool happyActive =false, madActive = false, matingActive = false;

    public bool assign = true;

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
            //StartCoroutine(IconDisplay());

            if (TownController.instance.murder || !GameController.instance.townHappy || 
                TownController.instance.currentTownHappiness < TownController.instance.maxTownHappiness || MetorSpawner.instance.startMetorShower)
            {
                happinessDecreaseManagement();
            }

            if (!TownController.instance.murder || GameController.instance.townHappy || !MetorSpawner.instance.startMetorShower)
            {
                if (personHappiness < maxHappiness)
                {
                    happinessIncreaseManagement();
                }
                else if(personHappiness > maxHappiness)
                {
                    personHappiness = maxHappiness;
                }
            }

            if (isControlling)
            {
                if (assign)
                {
                    tpa.setCurrentTask("Obeying Orders");
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
                                    TownController.instance.increaseTownHappiness();

                                    UIController.instance.toggleActionBar(true);

                                    assign = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator IconDisplay()
    {
        yield return new WaitForSeconds(10);

        if (personHappiness > maxHappiness / 2)
        {
            if (!happyActive)
            {
                happyActive = true;
                madActive = false;
            }
        }

        if (personHappiness < maxHappiness / 2)
        {
            if (!madActive)
            {
                madActive = true;
                happyActive = false;
            }
        }

        yield return new WaitForSeconds(1f);

        if (happyActive)
        {
            if (!happyIcon.activeSelf)
            {
                happyIcon.SetActive(true);
                madIcon.SetActive(false);
                matingIcon.SetActive(false);
            }
        }

        if (madActive)
        {
            if (!madIcon.activeSelf)
            {
                happyIcon.SetActive(false);
                madIcon.SetActive(true);
                matingIcon.SetActive(false);
            }
        }

        yield return new WaitForSeconds(10);

        if (happyIcon.activeSelf == true)
        {
            happyIcon.SetActive(false);
            happyActive = false;
        }
        if (madIcon.activeSelf)
        {
            madIcon.SetActive(false);
            madActive = false;
        }
        if (matingIcon.activeSelf)
        {
            matingIcon.SetActive(false);
            matingActive = false;
        }

        yield return null;
    }

    public void happinessIncreaseManagement()
    {
        #region Increase Happiness
        if (TownController.instance.townCurrentSize <= TownController.instance.townMaxSize)
        {
            UIController.instance.updateHappinessInfoSlider(increasePersonHappiness());
            return;
        }
        if (TownController.instance.currentTownHappiness >= TownController.instance.minTownHappiness)
        {
            UIController.instance.updateHappinessInfoSlider(increasePersonHappiness());
            return;
        }
        #endregion
    }

    public void happinessDecreaseManagement()
    {
        #region Decrease Happiness
        if (TownController.instance.townCurrentSize > TownController.instance.townMaxSize)
        {
            UIController.instance.updateHappinessInfoSlider(decreasePersonHappiness());
            return;
        }

        if (TownController.instance.currentTownHappiness < TownController.instance.minTownHappiness)
        {
            UIController.instance.updateHappinessInfoSlider(decreasePersonHappiness());
            return;
        }

        if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled / 2)
        {
            UIController.instance.updateHappinessInfoSlider(decreasePersonHappiness());
            return;
        }

        if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled)
        {
            UIController.instance.updateHappinessInfoSlider(decreasePersonHappiness(true));
            return;
        }

        if (TownController.instance.murder)
        {
            if (TownController.instance.totalPersonsKilled >= 2)
            {
                UIController.instance.updateHappinessInfoSlider(decreasePersonHappiness());
            }
            return;
        }

        #endregion
    }


    public float increasePersonHappiness()
    {
        if (personHappiness < maxHappiness)
        { 
            currentHappinessDelay--;

            if (currentHappinessDelay <= 0)
            {
                personHappiness += happinessIncreaseIncrement;

                currentHappinessDelay = maxHappinessDelay;
                return personHappiness;
            }
        }
        return personHappiness;
    }

    public float decreasePersonHappiness(bool doubleHit = false)
    {
        if (!doubleHit)
        {
            if (personHappiness > 0)
            {
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessDecreaseIncrement;

                    currentHappinessDelay = maxHappinessDelay;

                    return personHappiness;
                }
                return personHappiness;
            }
        }
        else if (doubleHit)
        {
            if (personHappiness > 0)
            {
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessDecreaseIncrement * 2;

                    currentHappinessDelay = maxHappinessDelay;

                    return personHappiness;
                }
                return personHappiness;
            }
        }

        return personHappiness;
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
        //mateTCP = mate.GetComponent<TownPersonController>();
    }

    public void resetBoredDelay()
    {
        currentBoredDelay = maxBoredDelay;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Firepit"))
        {
            if (GodController.instance.sacrifices.Contains(transform))
            {
                GodController.instance.sacrifices.Remove(transform);

                TownController.instance.killPerson(transform);
            }
        }
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

    public float getMaxHappiness()
    {
        return maxHappiness;
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
