using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolucionEjercicios : MonoBehaviour
{
    [SerializeField] private int exerciseNumber;
    [SerializeField] private float Angle;

    private Vector3 vectorA = new Vector3(10, 0, 0);

    private Vector3 vectorB = new Vector3(10, 0, 0);
    private Vector3 vectorC = new Vector3(10, 10, 0);
    private Vector3 vectorD = new Vector3(20, 10, 0);

    private Vector3 vectorE = new Vector3(10, 0, 0);
    private Vector3 vectorF = new Vector3(10, 10, 0);
    private Vector3 vectorG = new Vector3(20, 10, 0);
    private Vector3 vectorH = new Vector3(20, 20, 0);

    private void FixedUpdate()
    {
        switch (exerciseNumber)
        {
            case 1:
                Exercise1();
                break;

            case 2:
                Exercise2();
                break;

            case 3:
                Exercise3();
                break;

            default:
                break;
        }
    }

    private void Exercise1()
    {
        Quaternion rotation = Quaternion.AngleAxis(Angle, Vector3.up);

        vectorA = rotation * vectorA;
    }

    private void Exercise2()
    {
        Quaternion rotation = Quaternion.AngleAxis(Angle, Vector3.up);

        vectorB = rotation * vectorB;
        vectorC = rotation * vectorC;
        vectorD = rotation * vectorD;
    }

    private void Exercise3()
    {
        Quaternion rotation = Quaternion.AngleAxis(Angle, vectorF);

        vectorE = rotation * vectorE;

        rotation = Quaternion.Inverse(Quaternion.AngleAxis(Angle, vectorF));

        vectorG = rotation * vectorG;
    }

    private void OnDrawGizmos()
    {
        if(exerciseNumber == 1)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + vectorA);
        }
        else if(exerciseNumber == 2)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + vectorB);
            Gizmos.DrawLine(vectorB, transform.position + vectorC);
            Gizmos.DrawLine(vectorC, transform.position + vectorD);
        }
        else if(exerciseNumber == 3)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + vectorE);
            Gizmos.DrawLine(vectorE, transform.position + vectorF);
            Gizmos.DrawLine(vectorF, transform.position + vectorG);
            Gizmos.DrawLine(vectorG, transform.position + vectorH);

        }
    }
}
