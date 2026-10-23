using UnityEngine;
using UnityEngine.UI;

public class TalkingToPeople : MonoBehaviour
{
    [Tooltip("The person the player can talk to.")]
    public GameObject Person;
    [Tooltip("Assign the player if this script is not attached to the player.")]
    public Transform Player;
    public GameObject Dialog1;
    public GameObject NextButton;
    public GameObject Option1;
    public GameObject Option2;
    public GameObject DialogOption1;
    public GameObject DialogOption2;
    public GameObject ExitDialogue;

    [Min(0f)]
    public float InteractionDistance = 3f;

    private Button nextButton;
    private Button option1Button;
    private Button option2Button;
    private Button exitButton;
    private bool dialogueStarted = false;

    private void Start()
    {
        nextButton = NextButton.GetComponent<Button>();
        option1Button = Option1.GetComponent<Button>();
        option2Button = Option2.GetComponent<Button>();
        exitButton = ExitDialogue.GetComponent<Button>();
        nextButton.onClick.AddListener(ShowOptions);
        option1Button.onClick.AddListener(ChooseOption1);
        option2Button.onClick.AddListener(ChooseOption2);
        exitButton.onClick.AddListener(ExitConvo);

        Dialog1.SetActive(false);
        NextButton.SetActive(false);
        Option1.SetActive(false);
        Option2.SetActive(false);
        DialogOption1.SetActive(false);
        DialogOption2.SetActive(false);
        ExitDialogue.SetActive(false);
    }

    private void Update()
    {
        if (dialogueStarted || !Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        float distance = Vector3.Distance(Player.position, Person.transform.position);

        if (distance <= InteractionDistance)
        {
            print("HI");
            dialogueStarted = true;
            Dialog1.SetActive(true);
            NextButton.SetActive(true);
        }
        else
        {
            Debug.Log($"Too far to talk. Distance: {distance:F1}, required: {InteractionDistance:F1}.", this);
        }
    }

    private void ExitConvo()
    {
        Dialog1.SetActive(false);
        NextButton.SetActive(false);
        Option1.SetActive(false);
        Option2.SetActive(false);
        DialogOption1.SetActive(false);
        DialogOption2.SetActive(false);
        ExitDialogue.SetActive(false);
    }
    private void ShowOptions()
    {
        Dialog1.SetActive(false);
        NextButton.SetActive(false);
        Option1.SetActive(true);
        Option2.SetActive(true);
    }

    private void ChooseOption1()
    {
        ShowChosenDialogue(DialogOption1);
        // add point here
        // and other effects of choosing option 1
    }

    private void ChooseOption2()
    {
        ShowChosenDialogue(DialogOption2);
        // add effects of choosing option 2
    }

    private void ShowChosenDialogue(GameObject chosenDialogue)
    {
        Option1.SetActive(false);
        Option2.SetActive(false);
        chosenDialogue.SetActive(true);
        ExitDialogue.SetActive(true);
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ShowOptions);
        }

        if (option1Button != null)
        {
            option1Button.onClick.RemoveListener(ChooseOption1);
        }

        if (option2Button != null)
        {
            option2Button.onClick.RemoveListener(ChooseOption2);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitConvo);
        }
    }
}
