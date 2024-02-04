using UnityEngine;
using UnityEngine.UI;
public class GamepadManager : MonoBehaviour
{
    public static GamepadManager Instance;

    [Header("LeftStick")]
    [SerializeField] Slider leftStick;
    [SerializeField] Image leftStickHandle;

    [Header("A")]
    [SerializeField] Image buttonSouth;

    [Header("LeftTrigger")]
    [SerializeField] Slider leftTrigger;

    [Header("RightTrigger")]
    [SerializeField] Slider rightTrigger;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        buttonSouth.gameObject.SetActive(false);
    }
    public void SetStickValue(float value)
    {
        leftStick.value = value;
        if (value == 0)
        {
            leftStickHandle.color = Color.white;
            return;
        }
        leftStickHandle.color = Color.black;
    }
    public void SetButtonValue(bool value)
    {
        buttonSouth.gameObject.SetActive(value);
    }
    public void SetTriggerValue(float value)
    {
        if(value > 0)
        {
            leftTrigger.value = 0;
            rightTrigger.value = value;
        }
        else if(value < 0)
        {
            leftTrigger.value = -value;
            rightTrigger.value = 0;
        }
        else
        {
            leftTrigger.value = 0;
            rightTrigger.value = 0;
        }
    }
}