using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCardView : MonoBehaviour
{
    [SerializeField] private GameObject cardView;
    [SerializeField] private GameObject cardRosterIndicator;
    [SerializeField] private InputActionReference toggleCardViewAction;
    private bool isCardViewActive = false;
    private Vector3 originalCardViewPosition;
    private Vector3 originalIndicatorPosition;

    private void OnEnable()
    {
        toggleCardViewAction.action.performed += ToggleCardViewActive;
    }

    private void OnDisable()
    {
        toggleCardViewAction.action.performed -= ToggleCardViewActive;
    }

    private void Start()
    {
        originalCardViewPosition = cardView.transform.position;
        originalIndicatorPosition = cardRosterIndicator.transform.position;
    }

    public void ToggleCardViewActive(InputAction.CallbackContext context)
    {
        if (!isCardViewActive)
        {
            isCardViewActive = true;
            StartCoroutine(RaiseCardUIUpThenSettleAtPosition());
        }
        else
        {
            isCardViewActive = false;
            StartCoroutine(LowerCardUIDownThenSettleAtPosition());
        }
    }

    private IEnumerator RaiseCardUIUpThenSettleAtPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation
        Vector3 startCardViewPosition = cardView.transform.position;
        Vector3 targetCardViewPosition = startCardViewPosition + new Vector3(0, 200, 0); // Move up by 200 units

        Vector3 startIndicatorPosition = cardRosterIndicator.transform.position;
        Vector3 targetIndicatorPosition = startIndicatorPosition + new Vector3(0, 200, 0); // Move up by 200 units

        while (elapsedTime < duration)
        {
            cardView.transform.position = Vector3.Lerp(startCardViewPosition, targetCardViewPosition, elapsedTime / duration);
            cardRosterIndicator.transform.position = Vector3.Lerp(startIndicatorPosition, targetIndicatorPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardView.transform.position = targetCardViewPosition; // Ensure it ends at the exact target position
        cardRosterIndicator.transform.position = targetIndicatorPosition; // Ensure it ends at the exact target position
    }

    private IEnumerator LowerCardUIDownThenSettleAtPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation
        Vector3 startCardViewPosition = cardView.transform.position;
        Vector3 targetCardViewPosition = originalCardViewPosition;

        Vector3 startIndicatorPosition = cardRosterIndicator.transform.position;
        Vector3 targetIndicatorPosition = originalIndicatorPosition; // Assuming the indicator should return to its original position

        while (elapsedTime < duration)
        {
            cardView.transform.position = Vector3.Lerp(startCardViewPosition, targetCardViewPosition, elapsedTime / duration);
            cardRosterIndicator.transform.position = Vector3.Lerp(startIndicatorPosition, targetIndicatorPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardView.transform.position = targetCardViewPosition; // Ensure it ends at the exact target position
        cardRosterIndicator.transform.position = targetIndicatorPosition; // Ensure it ends at the exact target position
    }
}
