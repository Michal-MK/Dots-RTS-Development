﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamBox : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	public TeamSetup myParrent;
	public Vector3 initialPos;
	public int team;
	RectTransform rect;
	public RectTransform panel;
	Vector2 oldMousePos = Vector2.zero;
	bool cease;
	public float r;
	public static GameObject dragged;
    float timeOfLastTap;
    float doubleTapMaxTime = 0.5f;

    public void AllThingsSet() {
		//myParrent = transform.parent.GetComponent<TeamSetup>();
		rect = gameObject.GetComponent<RectTransform>();
		//panel = transform.parent.gameObject.GetComponent<RectTransform>();
		initialPos = transform.position;
	}

	public void OnDrag(PointerEventData eventData) {

		Vector2 mousePos = eventData.position;

		Vector2 mouseDelta = (mousePos - oldMousePos);
		Vector2 mouseOffset = ((Vector2)rect.position - mousePos);

        if (Vector2.Distance(panel.position, mousePos + mouseOffset + mouseDelta) < (r - rect.sizeDelta.x)) {
            rect.position = mousePos + mouseOffset + mouseDelta;
        }
		oldMousePos = mousePos;

	}

	public void OnPointerDown(PointerEventData eventData) {
		oldMousePos = eventData.position;
		dragged = gameObject;
       
        if (Time.time - timeOfLastTap < doubleTapMaxTime) {
            PrepareDiffInputBox();
            
        }
        timeOfLastTap = Time.time;
    }
    void PrepareDiffInputBox () {
        LevelEditorCore.aiDifficultySingleInput.transform.position = (Vector2)transform.position + new Vector2(0,rect.sizeDelta.y / 2);
        LevelEditorCore.aiDifficultySingleInput.gameObject.SetActive(true);
        SingleDiffIF singleDiffIf = LevelEditorCore.aiDifficultySingleInput.gameObject.GetComponent<SingleDiffIF>();
        singleDiffIf.OnMove(team);
        singleDiffIf.core = Camera.main.GetComponent<LevelEditorCore>();
        
    }


	public void OnPointerUp(PointerEventData eventData) {
		TeamBox t = eventData.pointerPress.GetComponent<TeamBox>();
		dragged = null;
		myParrent.TeamBoxPosChange(eventData.pointerPress.transform.position, t);
	}
}