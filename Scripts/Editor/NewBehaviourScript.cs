using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using System.Collections;

// Creates an instance of a primitive depending on the option selected by the user.
public class DeveloperCheats : EditorWindow {
    public string[] options = new string[] { "BeforeHUD", "Basic"};
    public int index = 0;
    [MenuItem("Window/Developer Cheats")]
    static void Init() {
        EditorWindow window = GetWindow(typeof(DeveloperCheats));
        window.Show();
    }

    void OnGUI() {
        index = EditorGUILayout.Popup(index, options);
        if (GUILayout.Button("Alterar GameState"))
            SetGameState();
    }

    void SetGameState() {
        GameLogic.Instance.SetGameState(options[index]);
    }
}