﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private static Canvas pressFToTalkUI;
    public GameObject npcObj;
    public GameObject playerObj;
    public NPCQuestManager npcQuestManager;
    public DialogueManager dialogueManager;
    public AudioSource dialogueMusic;

    private bool nearNPC;
    private float triggerDist = 20.0f;
    private DogPatrolMovement dogMovementScript;

    private void Start()
    {
        dogMovementScript = GetComponentInParent<DogPatrolMovement>();
        GameObject tempCanvas = GameObject.Find("PressButtonToTalkUI");
        if(tempCanvas != null)
        {
            pressFToTalkUI = tempCanvas.GetComponent<Canvas>();
            pressFToTalkUI.enabled = false;
        }
        this.nearNPC = false;
    }

    private void Update()
    {
        if (npcObj.name.Equals("Eugene"))
        {
            triggerDist = 70.0f;
        }
        if ((npcObj.transform.position - playerObj.transform.position).magnitude < triggerDist && !nearNPC)
        {
            pressFToTalkUI.enabled = true;
            nearNPC = true;
        }
        if((npcObj.transform.position - playerObj.transform.position).magnitude > triggerDist && nearNPC)
        {
            pressFToTalkUI.enabled = false;
            nearNPC = false;
            if (!npcObj.name.Equals("Eugene"))
            {
                EndDialogue();
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && nearNPC)
        {
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        dialogueMusic.Play();
        dialogueManager.setNPCQuestManager(npcQuestManager);
        pressFToTalkUI.enabled = false;
        List<Quest> quests = npcQuestManager.getQuests();
        Quest tempQuest = null;
        foreach(Quest quest in quests)
        {
            if (!quest.getCompleted())
            {
                tempQuest = quest;
                npcQuestManager.setTempCurrentQuest(tempQuest);
                break;
            }
        }
        Debug.Log("QUEST: " + tempQuest.questName + "\tDONE: " + tempQuest.getConditionMetForCompletion());
        if((tempQuest != null && !tempQuest.getAccepted()) 
            || (tempQuest.getConditionMetForCompletion() && !tempQuest.getCompleted()))
        {
            FindObjectOfType<DialogueManager>().StartDialogue(tempQuest);
        }
    }

    private void EndDialogue()
    {
        dialogueMusic.Stop();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}
