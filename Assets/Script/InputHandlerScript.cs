// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using TMPro;

// public class InputHandlerScript : MonoBehaviour
// {
//     private Camera _mainCamera;
//     // The array of the ORDERED target names 
//     private string[] arrCircle = new string[] { "Circle5", "Circle1", "Circle3", "Circle8", "Circle7", "Circle4", "Circle2", "Circle6", "Circle" };

//     private string[] arrScenes = new string[] { "scene11", "scene12", "scene13", "scene21", "scene22", "scene23", "scene31", "scene32", "scene33" };
//     private int index = 0;
//     private static int sceneIndex = 0;

//     public SceneManagerScript sceneManagerScript;
//     //public TMP_Text complete;
//     private void Awake()
//     {
//         _mainCamera = Camera.main;
//     }

//     // Change the color of the first target
//     private void Start()
//     {
//         ChangeColor(index);   //Debug.Log(arrCircle.Length);
//         //sceneManagerScript.LoadScene("scene" + arrScenes[sceneIndex]);
//     }

//     // Change the color of the next target
//     private void Update()
//     {
//         ChangeColor(index);

//     }

//     public void OnClick(InputAction.CallbackContext context)
//     {
//         if (!context.started) return;

//         var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
//         if (!rayHit.collider) return;

//         // Hit the target
//         if (rayHit.collider.gameObject.name == arrCircle[index] && index < arrCircle.Length)
//         {
//             // Change the color of the target to green if the target is clicked
//             GameObject.Find(arrCircle[index]).GetComponent<SpriteRenderer>().color = Color.green;

//             index++;
//         }

//         // if (sceneIndex == arrScenes.Length - 1 && rayHit.collider.gameObject.name == arrCircle[arrCircle.Length - 1])
//         // {
//         //     sceneManagerScript.LoadScene("complete");
//         // }



//         // Load the next scene if all targets are clicked
//         if (index >= arrCircle.Length && sceneIndex < arrScenes.Length-1)
//         {

//             index = 0;
//             sceneIndex++;
//             sceneManagerScript.LoadScene(arrScenes[sceneIndex]);
//             Debug.Log("sceneIndex: " + sceneIndex);
//             Debug.Log(arrScenes[sceneIndex]);
//         }




//     }

//     private void ChangeColor(int index)
//     {
//         //string tempCircle;
//         if (index < arrCircle.Length)
//         {
//             GameObject.Find(arrCircle[index]).GetComponent<SpriteRenderer>().color = Color.blue;
//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class InputHandlerScript : MonoBehaviour
{
    private Camera _mainCamera;
    // The array of the ORDERED target names 
    private string[] arrCircle = new string[] { "Circle5", "Circle1", "Circle3", "Circle8", "Circle7", "Circle4", "Circle2", "Circle6", "Circle" };

    private string[] arrScenes = new string[] { "scene11", "scene12", "scene13", "scene21", "scene22", "scene23", "scene31", "scene32", "scene33" };

    private float[] amplitudes = new float[] { 0.05f, 0.05f, 0.05f, 0.1f, 0.1f, 0.1f, 0.15f, 0.15f, 0.15f }; 
    private float[] widths = new float[] { 0.005f, 0.01f, 0.015f, 0.005f, 0.01f, 0.015f, 0.005f, 0.01f, 0.015f }; 

    private int index = 0;
    private static int sceneIndex = 0;
    private float startTime;
    private string filename = "";

    public SceneManagerScript sceneManagerScript;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    // Change the color of the first target
    private void Start()
    {
        ChangeColor(index);
        startTime = Time.time;
        filename = Application.dataPath + "/FittsLawData.csv";

        // CSV file header
        if (!File.Exists(filename))
        {
            File.WriteAllText(filename, "Technique,Width,Amplitude,Time,Correct,Scene\n");
        }
    }

    // Change the color of the next target
    private void Update()
    {
        ChangeColor(index);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var mousePosition = Mouse.current.position.ReadValue();
        Vector2 mousePosition2D = _mainCamera.ScreenToWorldPoint(mousePosition);
        RaycastHit2D rayHit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
        float selectionTime = Time.time - startTime;

        // Get current width and amplitude based on the scene index
        float currentWidth = widths[sceneIndex];
        float currentAmplitude = amplitudes[sceneIndex];

        if (!rayHit.collider)
        {
            // Misclick
            LogData("Mouse", currentWidth, currentAmplitude, selectionTime, 0, arrScenes[sceneIndex]);
            Debug.Log($"Technique: Mouse, Width: {currentWidth}, Amplitude: {currentAmplitude}, Time: {selectionTime}, Correct: 0, Scene: {arrScenes[sceneIndex]}");
            return;
        }

        bool correctSelection = rayHit.collider.gameObject.name == arrCircle[index];

        // Log and console output
        LogData("Mouse", currentWidth, currentAmplitude, selectionTime, correctSelection ? 1 : 0, arrScenes[sceneIndex]);
        Debug.Log($"Technique: Mouse, Width: {currentWidth}, Amplitude: {currentAmplitude}, Time: {selectionTime}, Correct: {(correctSelection ? 1 : 0)}, Scene: {arrScenes[sceneIndex]}");

        if (correctSelection && index < arrCircle.Length)
        {
            GameObject.Find(arrCircle[index]).GetComponent<SpriteRenderer>().color = Color.green;
            index++;
            startTime = Time.time; // Reseting the time
        }

        if (index >= arrCircle.Length && sceneIndex < arrScenes.Length - 1)
        {
            index = 0;
            sceneIndex++;
            sceneManagerScript.LoadScene(arrScenes[sceneIndex]);
        }
    }

    private void ChangeColor(int index)
    {
        if (index < arrCircle.Length)
        {
            GameObject.Find(arrCircle[index]).GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void LogData(string technique, float width, float amplitude, float time, int correct, string scene)
    {
        string logLine = $"{technique}, {width}, {amplitude}, {time}, {correct}, {scene}\n";
        File.AppendAllText(filename, logLine);
    }
}
