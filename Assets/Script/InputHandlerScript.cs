using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandlerScript : MonoBehaviour
{
    private Camera _mainCamera;

    // The array of the ORDERED target names 
    private string[] arrCircle= new string[] {"Circle5","Circle1","Circle3","Circle8","Circle7","Circle4","Circle2","Circle6","Circle"};
    private int index = 0;

    private void Awake() {
        _mainCamera = Camera.main;
    }

// Change the color of the first target
    private void Start() {
        ChangeColor(index);   //Debug.Log(arrCircle.Length);
    }

// Change the color of the next target
    private void Update() {
        ChangeColor(index);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(!context.started) return;
    
        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if(!rayHit.collider) return;

       
        if(rayHit.collider.gameObject.name == arrCircle[index]){
            Debug.Log(arrCircle[index]);
            index++;
        }
       
    }
    
    private void ChangeColor(int index){
        //string tempCircle;
        if(index < arrCircle.Length){
            GameObject.Find(arrCircle[index]).GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}
