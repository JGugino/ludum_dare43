using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TownPersonController : MonoBehaviour {
    public NavMeshAgent personAgent;

    private string personName = "Anna";

    private string personGender = "Male";

    private int personAge = 20;

    private float personHealth = 100;
    private float personSpeed = 5;
    private float personHappiness = 50, maxHappiness = 100;

    private int currentChildren = 0, maxChildren = 4;

    private int happinessIncrement = 2;

    private int currentHappinessDelay, maxHappinessDelay = 120;

    private Transform personHome = null;

    private void Awake()
    {
        personAgent = GetComponent<NavMeshAgent>();

        currentHappinessDelay = maxHappinessDelay;
    }

    public void increasePersonHappiness()
    {
        if (TownController.instance.townCurrentSize <= TownController.instance.townMaxSize || TownController.instance.currentTownHappiness >= TownController.instance.minTownHappiness)
        {
            if (personHappiness < maxHappiness)
            {
                currentHappinessDelay--;

                if (currentHappinessDelay<=0)
                {
                    personHappiness += happinessIncrement;

                    currentHappinessDelay = maxHappinessDelay;

                    return;
                }
            }
        }
    }

    public void decreasePersonHappiness()
    {
        if ((TownController.instance.townCurrentSize > TownController.instance.townMaxSize) 
            || (TownController.instance.currentTownHappiness < TownController.instance.minTownHappiness) || TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled / 2)
        {
            if (personHappiness > 0)
            {
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessIncrement;

                    currentHappinessDelay = maxHappinessDelay;

                    return;
                }
            }
        }

        if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled)
        {
            if (personHappiness > 0)
            {
                currentHappinessDelay--;

                if (currentHappinessDelay <= 0)
                {
                    personHappiness -= happinessIncrement * 2;

                    currentHappinessDelay = maxHappinessDelay;

                    return;
                }
            }
        }
    }

    public void movePersonPosition(Vector3 _destination)
    {
        if (personAgent.pathStatus == NavMeshPathStatus.PathComplete)
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
        Destroy(gameObject, 1);
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
    #endregion
}
