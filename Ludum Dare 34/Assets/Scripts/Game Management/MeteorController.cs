using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        string otherTag = collision.collider.tag;

        if (otherTag == "Ground")
        {
            MetorSpawner.instance.currentMetors--;

            Destroy(gameObject);
        }

        if (otherTag == "House")
        {
            Debug.Log("Hit a house");

            MetorSpawner.instance.currentMetors--;

            Destroy(gameObject);
        }

        if (otherTag == "Towns Person")
        {
            killPerson(collision.transform);

            MetorSpawner.instance.currentMetors--;

            Destroy(gameObject);
        }
    }

    public void killPerson(Transform _person)
    {
        if (GameController.instance.happyPeople.Contains(_person))
        {
            GameController.instance.happyPeople.Remove(_person);
        }
        else if (GameController.instance.unHappyPeople.Contains(_person))
        {
            GameController.instance.unHappyPeople.Remove(_person);
        }

        TownController.instance.townPersonsTransforms.Remove(_person);

        TownPersonController tpc = _person.GetComponent<TownPersonController>();

        string gender = tpc.getPersonGender();

        if (gender == "Male")
        {
            PersonSpawner.instance.currentMales--;
            UIController.instance.updateSliders();
        }
        else if (gender == "Female")
        {
            PersonSpawner.instance.currentFemales--;
            UIController.instance.updateSliders();
        }

        TownController.instance.totalPersonsKilled++;
        UIController.instance.updateKilledText();

        tpc.killPerson();

        UIController.instance.updatePopulationText();
    }
}
