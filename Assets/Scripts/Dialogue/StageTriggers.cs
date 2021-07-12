using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Cinemachine;
using UnityEngine.SceneManagement;
public class StageTriggers : MonoBehaviour
{
    public Flowchart flow;
    bool start = true;
    public float timer=0;
    public CinemachineDollyCart cart;

    //end of level variables
    private Vector3 endDirection;
    public GameObject playerShip;
    private bool levelFinished;
    private float endSpeed;

    private void Start()
    {
        levelFinished = false;
    }

    void Update()
    {
        if (timer <= 0) cart.enabled = true;
        else timer = timer - Time.deltaTime;

        if(levelFinished)
        {
            EndLevel();
        }
    }

    // TODO: assert that the collision is the correct object
    // start boolean can be taken out if this is easy to do
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name=="Start Trigger")
        {
            //flow.ExecuteBlock("start");
            start = false;
        }
        if(other.gameObject.name == "End Trigger")
        {
            //flow.ExecuteBlock("finish");
            //store final cart speed and execute end of level
            endSpeed = playerShip.GetComponent<ShipMovement>().forwardSpeed;
            endDirection = new Vector3(0, 0, endSpeed * Time.deltaTime);
            levelFinished = true;
            EndLevel();
        }
        if(other.gameObject.CompareTag("Dialogue_Trigger"))
        {
            GameObject passivePilot = GameManager.singleton.passiveCopilot;
            GameObject activePilot = GameManager.singleton.activeCopilot;
            DialogueTriggers.Emotion selectedEmotion = other.gameObject.GetComponent<DialogueTriggers>().pilotEmotion;
            string specialName = other.gameObject.GetComponent<DialogueTriggers>().hiddenPilot.ToString();
            //check if it is hidden dialogue or not
            if(specialName.CompareTo("Any") != 0)
            {
                //does the player have the hidden pilot selected
                if(passivePilot.name.CompareTo(specialName) == 0)
                {
                    //activate HIDDEN dialogue
                    RemarkManager.singleton.ExecuteDialogue(other.gameObject.name, passivePilot, selectedEmotion);
                }
                else if(activePilot.name.CompareTo(specialName) == 0)
                {
                    //activate HIDDEN dialogue
                    RemarkManager.singleton.ExecuteDialogue(other.gameObject.name, activePilot, selectedEmotion);
                }
            }
            else //it is not hidden dialogue
            {
                //check chosen pilot on dialogue trigger
                if(other.gameObject.GetComponent<DialogueTriggers>().chosenPilot == DialogueTriggers.DialoguePilot.passive)
                {
                    //activate STORY dialogue
                    RemarkManager.singleton.ExecuteDialogue(other.gameObject.name, passivePilot, selectedEmotion);
                }
                else
                {
                    //activate STORY dialogue
                    RemarkManager.singleton.ExecuteDialogue(other.gameObject.name, activePilot, selectedEmotion);
                }
            }
        }
    }

    //prob will need to use a coroutine later
    private void EndLevel()
    {
        //fly ship away from camera at cart speed (can adjust later)
        playerShip.transform.Translate(endDirection);

        //update currenStage and save profile
        ProfileManager.instance.CurrentProfile.currentStage += 1;
        ProfileManager.instance.SaveCurrentProfile();

        //load active pilot VN scene
        SceneManager.LoadScene(GameManager.singleton.activeCopilot.name);
    }
}
