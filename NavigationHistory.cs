using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class NavigationHistory : EditorWindow {
	
	Object[] history;
	int historySize = 50;
	Object lastGameObject;
	int firstId;
	int lastId;
	int selectedId;
	bool arrayFull;
	GUIStyle boldButton;
	GUIStyle normalButton;


	[MenuItem("Window/History Window")]
	static void menuCall(){
		NavigationHistory nh= EditorWindow.GetWindow <NavigationHistory>(false, "History");
	}

	public void OnEnable(){
		if (history!=null)
			return;
		EditorApplication.hierarchyWindowItemOnGUI+=onHierarchyChangeListener;
		history = new Object[historySize];
		firstId = historySize - 1;
		lastId  = 0;
		selectedId = firstId;
		arrayFull = false;
		initStyles();
		checkSelection();
	}

	void initStyles(){
		normalButton           = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button);
		normalButton.stretchWidth = true;	
		normalButton.alignment = TextAnchor.MiddleLeft;
		boldButton             = new GUIStyle(normalButton);
		boldButton.fontStyle   = FontStyle.Bold;
	}

	Vector2 scrollPosition;
	void OnGUI(){
		EditorGUILayout.BeginHorizontal()	;

		GUI.enabled = (selectedId != lastId);
		if (GUILayout.Button("←"))
			selectObject(getPreviousId(selectedId));
		GUI.enabled = true;

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		int i = firstId;
		bool exit = false;
		while(!exit){
			if (history[i]==null)
				break;
			if (GUILayout.Button(history[i].name, i==selectedId ? boldButton : normalButton)){
				selectObject(i);
			}
			i = getPreviousId(i);
			exit = (i==firstId);
		}
		EditorGUILayout.EndScrollView();

		GUI.enabled = (selectedId != firstId);
		if (GUILayout.Button("→"))
			selectObject(getNextId(selectedId));
		GUI.enabled = true;

		EditorGUILayout.EndHorizontal();
	}

	void selectObject(int i){
		Selection.activeObject = history[i];
		selectedId = i;
	}

	void onHierarchyChangeListener (int instanceID, Rect selectionRect)
	{
		checkSelection ();
	}

	void checkSelection ()
	{
		if (Selection.activeObject != null && Selection.activeObject != lastGameObject && Selection.activeObject != history[selectedId]) {
			lastGameObject = Selection.activeObject;
			firstId = getNextId (firstId);
			if (arrayFull == false && history[firstId]!=null)
				arrayFull = true;
			history [firstId] = lastGameObject;
			selectedId = firstId;
			if (arrayFull)
				lastId = getNextId(firstId);
		}
		Repaint ();
	}

	int getNextId(int id){
		return ++id == historySize ? 0 : id;
	}

	int getPreviousId(int id){
		return --id == -1 ? historySize - 1 : id ;
	}

}
