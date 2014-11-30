using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class Movement
    {
        TargetPoint[] targets;
        int numTargets;
        public Movement(TargetPoint[] list)
        {
            numTargets = list.Length;
            targets = new TargetPoint[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                targets[i]=list[i];
            }
        }
        public bool isInTargetN(SkeletonPoint p, int n)
        {
            if (n<0 || n>=numTargets){
                System.Console.WriteLine("ERROR en la funcion isInTargetN, valor introducido {0} y debe estar entre 1 y {1}",n,numTargets);
                return false;
            }
            return targets[n].isPointInArea(p);
        }

        public void updateMov(SkeletonPoint []list)
        {
            if(list.Length != targets.Length)
                System.Console.WriteLine("ERROR en la funcion updateMov, tamaño de la lista de puntos {0} y debe ser {1}", list.Length, numTargets);
            else
                for (int i = 0; i < list.Length; i++)
                {
                    targets[i].updateTarget(list[i]);
                }
        }
        public void updateMov(TargetPoint[] list)
        {
            numTargets = list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                targets[i] = list[i];
            }
        }
        public int getNumTargets()
        {
            return numTargets;
        }
        public TargetPoint getTarget(int n)
        {
            if (n < 0 || n >= numTargets)
            {
                System.Console.WriteLine("ERROR en la funcion isInTargetN, valor introducido {0} y debe estar entre 1 y {1}", n, numTargets);
                return null;
            }
            else
                return targets[n];
        }

    }
}
