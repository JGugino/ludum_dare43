using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TownPersonAI : MonoBehaviour {

    private TownPersonController tpc;

    private string currentTask = "none";

    private Vector3 movePos;

    private bool destPicked = false;

    private int currentMatingDelay, maxMatingDelay = 65;

    private int personWanderWait = 5;

    private int minWanderDistance = 10, maxWanderDistance = 25;

    private void Awake()
    {
        tpc = GetComponent<TownPersonController>();

        currentMatingDelay = maxMatingDelay;
    }

    private void Update()
    {
        if (!tpc.assign)
        {
            float distance = Vector3.Distance(tpc.personAgent.transform.position, tpc.personAgent.destination);

            if (distance < 4)
            {
                StartCoroutine(PersonWander());

                if (destPicked)
                {
                    destPicked = false;
                }
            }
        }
    }

    public void TBD()
    {
        if (tpc.getPersonsPartner() != null)
        {
            float distance = Vector3.Distance(transform.position, tpc.getPersonsPartner().position);
            if (distance <= tpc.matingRange)
            {
                currentTask = "making child";

                tpc.increasePersonHappiness();

                currentMatingDelay--;

                tpc.movePersonPosition(tpc.getPersonsPartner().position);

                if (currentMatingDelay <= 0)
                {
                    if (tpc.getPersonGender() == "Female")
                    {
                        if (tpc.currentChildren < tpc.maxChildren)
                        {
                            tpc.createChild(transform);

                            tpc.currentChildren++;

                            tpc.getPersonsPartner().GetComponent<TownPersonController>().setPersonsPartner(null);
                            tpc.setPersonsPartner(null);
                        }
                    }
                    else if (tpc.getPersonGender() != "Female")
                    {
                        if (tpc.getPersonsPartner().GetComponent<TownPersonController>().getPersonGender() == "Female")
                        {
                            if (tpc.getPersonsPartner().GetComponent<TownPersonController>().getCurrentChildren() < tpc.getPersonsPartner().GetComponent<TownPersonController>().getMaxChildren())
                            {
                                tpc.createChild(tpc.getPersonsPartner().transform);

                                tpc.getPersonsPartner().GetComponent<TownPersonController>().currentChildren++;

                                tpc.getPersonsPartner().GetComponent<TownPersonController>().setPersonsPartner(null);
                                tpc.setPersonsPartner(null);
                            }
                        }
                    }
                }
            }

            if (tpc.getPersonsPartner() == null)
            {
                if (tpc.getPersonAge() < 50)
                {
                    if (tpc.getPersonHappiness() > 85)
                    {
                        currentTask = "finding mate";

                        findMate();
                    }
                }
            }
        }
    }


    public IEnumerator PersonWander()
    {
        if (tpc.personAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            float creatureRange = Random.Range(minWanderDistance, maxWanderDistance);

            yield return new WaitForSeconds(personWanderWait);

            currentTask = "Wandering around";

            if (!destPicked)
            {
                if (Random.value <= 0.2)
                {
                    if (!destPicked)
                    {
                        movePos = new Vector3(tpc.personAgent.transform.position.x + creatureRange, tpc.personAgent.transform.position.y, tpc.personAgent.transform.position.z + creatureRange);

                        if (!destPicked)
                        {
                            destPicked = true;
                        }
                    }
                }

                if (Random.value >= 0.3 && Random.value <= 0.5)
                {
                    if (!destPicked)
                    {
                        movePos = new Vector3(tpc.personAgent.transform.position.x - creatureRange, tpc.personAgent.transform.position.y, tpc.personAgent.transform.position.z - creatureRange);

                        if (!destPicked)
                        {
                            destPicked = true;
                        }
                    }
                }

                if (Random.value >= 0.6 && Random.value <= 0.8)
                {
                    if (!destPicked)
                    {
                        movePos = new Vector3(tpc.personAgent.transform.position.x - creatureRange, tpc.personAgent.transform.position.y, tpc.personAgent.transform.position.z + creatureRange);

                        if (!destPicked)
                        {
                            destPicked = true;
                        }
                    }
                }

                if (Random.value >= 0.9)
                {
                    if (!destPicked)
                    {
                        movePos = new Vector3(tpc.personAgent.transform.position.x + creatureRange, tpc.personAgent.transform.position.y, tpc.personAgent.transform.position.z - creatureRange);

                        if (!destPicked)
                        {
                            destPicked = true;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(2);

        if (movePos != Vector3.zero && !tpc.personAgent.pathPending && tpc.personAgent.pathStatus != NavMeshPathStatus.PathInvalid && tpc.personAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            tpc.personAgent.SetDestination(movePos);

            yield return new WaitForSeconds(personWanderWait);

            destPicked = false;

            currentTask = "none";
        }
    }

    public IEnumerator PersonWait()
    {
        yield return new WaitForEndOfFrame();
    }

    public void findMate()
    {
        foreach (Transform t in TownController.instance.townPersonsTransforms)
        {
            float distance = Vector3.Distance(transform.position, t.position);

            TownPersonController _tpc = t.GetComponent<TownPersonController>();

            if (distance <= tpc.talkRange)
            {
                if (_tpc.getPersonHappiness() > 85)
                {
                    if (_tpc.getPersonAge() < 50)
                    {
                        if (t != transform)
                        {
                            if (_tpc.getPersonGender() != tpc.getPersonGender())
                            {
                                if (_tpc.getPersonsPartner() == null)
                                {
                                    if (tpc.personAgent.pathStatus == NavMeshPathStatus.PathComplete)
                                    {
                                        tpc.setMate(t);

                                        _tpc.setMate(transform);

                                        tpc.movePersonPosition(t.position);

                                        currentTask = "none";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    public string getCurrentTask()
    {
        return currentTask;
    }

    public void setCurrentTask(string _task)
    {
        currentTask = _task;
    }
}
