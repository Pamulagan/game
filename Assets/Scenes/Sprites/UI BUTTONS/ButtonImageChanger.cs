using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonHoldImageChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite defaultSprite; // Default button image
    public Sprite pushedSprite;  // Image when button is pressed

    private Image buttonImage;   // Image component of the button
    private bool isPushed = false; // Track the current state of the button

    void Start()
    {
        // Get the Image component from the Button
        buttonImage = GetComponent<Image>();

        // Set the default image initially
        if (buttonImage != null && defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }
    }

    // Called when the button is pressed
    public void OnPointerDown(PointerEventData eventData)
    {
        // Toggle the button state
        isPushed = !isPushed;

        // Change the button sprite based on its state
        if (buttonImage != null)
        {
            buttonImage.sprite = isPushed ? pushedSprite : defaultSprite;
        }
    }

    // Called when the button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        // If the button is pushed, load the GameScene
        if (isPushed)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
