using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] InputActionProperty touch;
    [SerializeField] InputActionProperty position;

    [SerializeField] LayerMask interactableLayers;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        inputActionAsset.actionMaps[0].Enable();
    }

    private void Update()
    {
        if (!touch.action.IsPressed()) return;

        RaycastHit hit = new RaycastHit();

        
        if(Physics.Raycast(cam.ScreenPointToRay(position.action.ReadValue<Vector2>()), out hit, float.MaxValue, interactableLayers))
        {
            if (hit.collider == null) return;
            Interactable interactable = hit.collider.GetComponentInChildren<Interactable>();
            if (interactable == null) return;

            interactable.Interact();
        }
    }
}
