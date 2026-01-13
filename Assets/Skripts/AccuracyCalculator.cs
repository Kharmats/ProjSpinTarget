using UnityEngine;

public class AccuracyCalculator : MonoBehaviour
{
    public BallController ballController;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ballController.HandlePlayerInput();
        }
    }
}