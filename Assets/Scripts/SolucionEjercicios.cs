using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class SolucionEjercicios : MonoBehaviour
{
    [SerializeField] private int exerciseNumber;
    [SerializeField] private float Angle;

    private Vec3 vectorA = new Vec3(10, 0, 0);

    private Vec3 vectorB = new Vec3(10, 0, 0);
    private Vec3 vectorC = new Vec3(10, 10, 0);
    private Vec3 vectorD = new Vec3(20, 10, 0);

    private Vec3 vectorE = new Vec3(10, 0, 0);
    private Vec3 vectorF = new Vec3(10, 10, 0);
    private Vec3 vectorG = new Vec3(20, 10, 0);
    private Vec3 vectorH = new Vec3(20, 20, 0);

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
        MyQuaternion rotation = MyQuaternion.AngleAxis(Angle, Vec3.up);

        vectorA = rotation * vectorA;
    }

    private void Exercise2()
    {
        MyQuaternion rotation = MyQuaternion.AngleAxis(Angle, Vec3.up);

        vectorB = rotation * vectorB;
        vectorC = rotation * vectorC;
        vectorD = rotation * vectorD;
    }

    private void Exercise3()
    {
        MyQuaternion rotation = MyQuaternion.AngleAxis(Angle, vectorF);

        vectorE = rotation * vectorE;

        rotation = MyQuaternion.Inverse(MyQuaternion.AngleAxis(Angle, vectorF));

        vectorG = rotation * vectorG;
    }

    private void OnDrawGizmos()
    {
        if(exerciseNumber == 1)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)vectorA);
        }
        else if(exerciseNumber == 2)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)vectorB);
            Gizmos.DrawLine(vectorB, transform.position + (Vector3)vectorC);
            Gizmos.DrawLine(vectorC, transform.position + (Vector3)vectorD);
        }
        else if(exerciseNumber == 3)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)vectorE);
            Gizmos.DrawLine(vectorE, transform.position + (Vector3)vectorF);
            Gizmos.DrawLine(vectorF, transform.position + (Vector3)vectorG);
            Gizmos.DrawLine(vectorG, transform.position + (Vector3)vectorH);
        }
    }
}
