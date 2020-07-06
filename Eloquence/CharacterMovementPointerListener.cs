using Movement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CharacterMovementPointerListener : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    // Removes nullreference warning values in the console. They are not null, because they are injected.
#pragma warning disable 649
    // Last second implementation for the demo. Audiosources are optional and manually assigned right now.
    [SerializeField]
    private AudioSource _moveAudioSource;
    [SerializeField]
    private AudioSource _denyAudioSource;

    // Automatically assigned dependencies
    [SerializeField][HideInInspector]
    private PlayerInput _playerInput;
    [SerializeField][HideInInspector]
    private CharacterMovement _characterMovement;
    [SerializeField][HideInInspector]
    private Image _image;
#pragma warning restore 649

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Closes the player's dialogue menu.
        _playerInput.CloseInput();

        // Closes the NPC's dialogue menu using a static reference to the last interacted NPC
        PlayerInput.CloseNpcDialogue();

        // Move towards position call. 
        if (_characterMovement.TryMoveTowardsUiPosition(eventData.position))
            if (_moveAudioSource != null) _moveAudioSource.Play();
            else Debug.LogWarning("No MoveAudioSource found on " + gameObject.name);
        else
            if (_denyAudioSource != null) _denyAudioSource.Play();
            else Debug.LogWarning("No DenyAudioSource found on " + gameObject.name);       
    }

    private void OnEnable()
    {
        ValidateDependencies();
    }

    private void ValidateDependencies()
    {
        if (_playerInput == null)
        {
            Debug.LogError(typeof(PlayerInput).ToString() + " not set on component " + GetType() + " on GameObject " + gameObject.name + ", disabling component.");
            Debug.LogWarning("Go to the component settings and activate 'Find dependencies'");
            this.enabled = false;
        }
        if (_characterMovement == null)
        {
            Debug.LogError(typeof(CharacterMovement).ToString() + " not set on component " + GetType() + " on GameObject " + gameObject.name + ", disabling component.");
            Debug.LogWarning("Go to the component settings and activate 'Find dependencies'");
            this.enabled = false;
        }
    }

    // Uses Unity's editor functionality to immediately find the dependencies required.
    [ContextMenu("Find dependencies")]
    private void Reset()
    {
        RectTransform rectTransform = (RectTransform)transform;
        if (rectTransform == null)
        {
            Debug.LogError("Only allowed on RectTransform. Deleting this component.");
            DestroyImmediate(this);
        }

        // Center the position
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);    
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        // Find and fill the dependencies
        _playerInput = FindObjectOfType<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError(typeof(PlayerInput).ToString() + " does not exist in this scene. Create one first. Deleting this component.");
            DestroyImmediate(this);
        }
        _characterMovement = FindObjectOfType<CharacterMovement>();
        if (_characterMovement == null)
        {
            Debug.LogError(typeof(CharacterMovement).ToString() + " does not exist in this scene. Create one first. Deleting this component.");
            DestroyImmediate(this);
        }
        _image = GetComponent<Image>();
        _image.color = new Color(0, 0, 0, 0);
    }
}
