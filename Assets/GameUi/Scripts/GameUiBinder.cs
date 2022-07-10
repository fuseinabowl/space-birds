using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUiBinder : MonoBehaviour
{
    [SerializeField]
    private TurnRunner turnRunner = null;

    private void Start()
    {
        var document = GetComponent<UIDocument>();
        var continueButton = document.rootVisualElement.Q<VisualElement>("screen").Q<Button>("continue");
        continueButton.clicked += () => TriggerNextTurn(continueButton);

        // bind to the turn runner
    }

    private void TriggerNextTurn(Button continueButton)
    {
        continueButton.SetEnabled(false);
        turnRunner.ResumeGame(() => continueButton.SetEnabled(true));
    }
}
