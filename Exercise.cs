using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class Exercise
    {
        Movement[] movs;
        int nSteps;
        int nMovs;
        bool initialized;
        int actualStep;

        public Exercise(){
            nMovs = 0;
            initialized = false;
            actualStep = 0;
        }
        public Exercise(int n, int s)
        {
            nMovs = n;
            initialized = false;
            actualStep = 0;
            nSteps = s;
            movs = new Movement[n];
        }
        public Exercise(Movement[] list)
        {
            nMovs = list.Length;
            actualStep = 0;
            movs = new Movement[list.Length];
            nSteps = list[0].getNumTargets();
            for (int i = 0; i < list.Length; i++)
            {
                if(nSteps != list[i].getNumTargets())
                    System.Console.WriteLine("ERROR en el constructor de Exercise, numero de objetivos del movimiento {0} incorrecto, debe ser {1}", i, nSteps);
                else
                    movs[i] = list[i];
            }
            initialized = true;
        }

        public void updateMovements(Movement[] list){
            for (int i = 0; i < list.Length; i++)
            {
                if(nSteps != list[i].getNumTargets())
                    System.Console.WriteLine("ERROR en el constructor de Exercise, numero de objetivos del movimiento {0} incorrecto, debe ser {1}", i, nSteps);
                else
                    movs[i] = list[i];
            }
            initialized = true;
        }

        public bool isInMovementN(SkeletonPoint[] listOfPoints, int n)
        {
            if (listOfPoints.Length != nMovs)
            {
                System.Console.WriteLine("ERROR en la funcion isSyncronized, numero de puntos(joints) de la lista {0} y debe ser {1}", listOfPoints.Length, nMovs);
                return false;
            }
            for (int i = 0; i < nMovs; i++)
                if (!movs[i].isInTargetN(listOfPoints[i], n))
                    return false;
            actualStep++;
            return true;
        }


        public bool isInErrorMovement(SkeletonPoint[] listOfPoints, int n)
        {
            if (listOfPoints.Length != nMovs)
            {
                System.Console.WriteLine("ERROR en la funcion isSyncronized, numero de puntos(joints) de la lista {0} y debe ser {1}", listOfPoints.Length, nMovs);
                return false;
            }

            if (movs[0].isInTargetN(listOfPoints[0], n) && !movs[1].isInTargetN(listOfPoints[1], n))
            {
                actualStep = n;
                return true;
            }
            if (!movs[0].isInTargetN(listOfPoints[0], n) && movs[1].isInTargetN(listOfPoints[1], n))
            {
                actualStep = n;
                return true;
            }
            
            return false;
        }

        public bool isInitialized()
        {
            return initialized;
        }
        public int getNumSteps()
        {
            return nSteps;
        }
        public int getNumMovs()
        {
            return nMovs;
        }
        public int getActualStep()
        {
            return actualStep;
        }
        public Movement getMov(int i)
        {
            return movs[i];
        }

    }
}
