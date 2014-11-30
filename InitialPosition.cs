using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class InitialPosition
    {
        private double height;
        private double kneelingHeight;
        public InitialPosition(Skeleton ske){
            Joint kneeRight = ske.Joints[JointType.KneeRight];
            Joint kneeLeft = ske.Joints[JointType.KneeLeft];
            Joint head = ske.Joints[JointType.Head];
            double kry = kneeRight.Position.Y;//altura en eje Y de la rodilla derecha
            double kly = kneeLeft.Position.Y;//altura en eje Y de la rodilla izquierda
            double hy = head.Position.Y;
            //altura en el eje Y de la cabeza
            height = hy;
            //A la altura de la persona le resto la altura de las rodillas en el eje Y y obtengo la altura de rodillas
            double kneeY = ((kry + kly) / 2);
            kneelingHeight = height - kneeY;
        }

        public bool correctPosition(Skeleton ske, double angle)
        {
            Joint kneeRight = ske.Joints[JointType.KneeRight];
            Joint kneeLeft = ske.Joints[JointType.KneeLeft];
            Joint ankleRight = ske.Joints[JointType.AnkleRight];
            Joint ankleLeft = ske.Joints[JointType.AnkleLeft];
            Joint hipCenter = ske.Joints[JointType.HipCenter];
            Joint hipRight = ske.Joints[JointType.HipRight];
            Joint hipLeft = ske.Joints[JointType.HipLeft];
            Joint footLeft = ske.Joints[JointType.FootLeft];
            Joint footRight = ske.Joints[JointType.FootRight];
            Joint head = ske.Joints[JointType.Head];
            //Obtengo la posicion de la cabeza en el eje Y que ahora debe ser la altura de rodillas si de verdad esta de rodillas
            double kneelingHeightCalculated = head.Position.Y;
            double hcy = hipCenter.Position.Y;
            double hcx = hipCenter.Position.X;
            double kry = kneeRight.Position.Y;
            double krz = kneeRight.Position.Z;
            double krx = kneeRight.Position.X;
            double kly = kneeLeft.Position.Y;
            double klz = kneeLeft.Position.Z;
            double klx = kneeLeft.Position.X;
            double arz = ankleRight.Position.Z;
            double alz = ankleLeft.Position.Z;

            //distancia del centro de la cadera a la rodilla derecha en el plano XY
            double hipCenterToKneeRight = System.Math.Sqrt(((hcx - krx) * (hcx - krx)) + ((hcy - kry) * (hcy - kry)));
            //distancia del centro de la cadera a la rodilla izquierda en el plano XY
            double hipCenterToKneeLeft = System.Math.Sqrt(((hcx - klx) * (hcx - klx)) + ((hcy - kly) * (hcy - kly)));
            //distancia de la rodilla derecha a la rodilla izquierda en el plano XY
            double kneeRightToKneeLeft = System.Math.Sqrt(((krx - klx) * (krx - klx)) + ((kry - kly) * (kry - kly)));
            //Calculo el angulo que se forma de las rodillas con el centro la cintura segun la posicion del cuerpo en el plano XY
            double angleCalculated = System.Math.Acos(((hipCenterToKneeRight * hipCenterToKneeRight) + (hipCenterToKneeLeft * hipCenterToKneeLeft) - (kneeRightToKneeLeft * kneeRightToKneeLeft)) / (2 * hipCenterToKneeRight * hipCenterToKneeLeft));
            angleCalculated = angleCalculated * 180 / System.Math.PI;

            //Calculo un porcentaje de error de la altura de rodillas
            double error = System.Math.Abs(5 * kneelingHeight / 100);
            
            //Compruebo que los tobillos esten a mas profundidad en el eje Z que las rodillas o que esten inferidos y que los pies esten inferidos y que el angulo formado este entre el angulo pedido (angulopedido-5, angulopedido+5) y que la altura ahora este por debajo en el eje Y el trozo correspondiente a la distancia entre el tobillo y las rodillas
            if (((krz < arz) || (ankleRight.TrackingState == JointTrackingState.Inferred)) && ((klz < alz) || (ankleLeft.TrackingState == JointTrackingState.Inferred)) && (footLeft.TrackingState == JointTrackingState.Inferred) && (footRight.TrackingState == JointTrackingState.Inferred) && (angle + 5 > angleCalculated) && (angle - 5 < angleCalculated) && (kneelingHeightCalculated < (kneelingHeight + error)))
                return true;

            return false;
        }
    }
}
