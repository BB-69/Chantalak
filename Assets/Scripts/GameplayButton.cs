using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayButton : MonoBehaviour
{

    public GameManager.ButtonPressed buttonType;

    public void OnButtonPress()
    {
        Debug.Log($"ButtonPressed.{buttonType}");
        GameManager.Instance.HandleButtonPress(buttonType);
    }

}
