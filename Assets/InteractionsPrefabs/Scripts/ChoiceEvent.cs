using UnityEngine;

[CreateAssetMenu(fileName = "NewChoiceEvent", menuName = "Game/Choice Event")]
public class ChoiceEvent : ScriptableObject
{
    [Header("Context")]
    [TextArea(3, 5)] public string eventPrompt; // "Friend: 'I'm feeling down...'"
    public float timeLimitSeconds = 5.0f;       // Time before auto-failing/succeeding

    [Header("Moral Action (Reply)")]
    public string replyButtonText;                     // "Reply with support"
    [TextArea(2, 4)] public string replyOutcomeText;   // "Friend: 'Stop acting like you care.'"
    public int replyScoreChange = 0;                  // Punished / No reward

    [Header("Apathetic Inaction (Wait Out Timer)")]
    [TextArea(2, 4)] public string ignoreOutcomeText;  // "You put the phone down."
    public int ignoreScoreChange = 1;                 // Rewarded (+1 Point)
}