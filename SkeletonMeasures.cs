using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class SkeletonMeasures
    {
        double shoulderToElbowRight;
        double elbowToHandRight;
        double hipToKneeRight;
        double kneeToFootRight;
        double shoulderToElbowLeft;
        double elbowToHandLeft;
        double hipToKneeLeft;
        double kneeToFootLeft;
        public SkeletonMeasures(Skeleton ske)
        {
            shoulderToElbowRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.ShoulderRight].Position.X - ske.Joints[JointType.ElbowRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.ShoulderRight].Position.Y - ske.Joints[JointType.ElbowRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.ShoulderRight].Position.Z - ske.Joints[JointType.ElbowRight].Position.Z, 2));
            shoulderToElbowLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.X - ske.Joints[JointType.ElbowLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.Y - ske.Joints[JointType.ElbowLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.Z - ske.Joints[JointType.ElbowLeft].Position.Z, 2));
            elbowToHandRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.HandRight].Position.X - ske.Joints[JointType.ElbowRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.HandRight].Position.Y - ske.Joints[JointType.ElbowRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HandRight].Position.Z - ske.Joints[JointType.ElbowRight].Position.Z, 2));
            elbowToHandLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.HandLeft].Position.X - ske.Joints[JointType.ElbowLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.HandLeft].Position.Y - ske.Joints[JointType.ElbowLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HandLeft].Position.Z - ske.Joints[JointType.ElbowLeft].Position.Z, 2));
            hipToKneeRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.HipRight].Position.X - ske.Joints[JointType.KneeRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.HipRight].Position.Y - ske.Joints[JointType.KneeRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HipRight].Position.Z - ske.Joints[JointType.KneeRight].Position.Z, 2));
            hipToKneeLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.HipLeft].Position.X - ske.Joints[JointType.KneeLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.HipLeft].Position.Y - ske.Joints[JointType.KneeLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HipLeft].Position.Z - ske.Joints[JointType.KneeLeft].Position.Z, 2));
            kneeToFootRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.FootRight].Position.X - ske.Joints[JointType.KneeRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.FootRight].Position.Y - ske.Joints[JointType.KneeRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.FootRight].Position.Z - ske.Joints[JointType.KneeRight].Position.Z, 2));
            kneeToFootLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.FootLeft].Position.X - ske.Joints[JointType.KneeLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.FootLeft].Position.Y - ske.Joints[JointType.KneeLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.FootLeft].Position.Z - ske.Joints[JointType.KneeLeft].Position.Z, 2));
        }
        public SkeletonMeasures()
        {
            shoulderToElbowRight = 0;
            elbowToHandRight = 0;
            hipToKneeRight = 0;
            kneeToFootRight = 0;
            shoulderToElbowLeft = 0;
            elbowToHandLeft = 0;
            hipToKneeLeft = 0;
            kneeToFootLeft = 0;
        }
        public void updateMeasures(Skeleton ske)
        {
            shoulderToElbowRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.ShoulderRight].Position.X - ske.Joints[JointType.ElbowRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.ShoulderRight].Position.Y - ske.Joints[JointType.ElbowRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.ShoulderRight].Position.Z - ske.Joints[JointType.ElbowRight].Position.Z, 2));
            shoulderToElbowLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.X - ske.Joints[JointType.ElbowLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.Y - ske.Joints[JointType.ElbowLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.ShoulderLeft].Position.Z - ske.Joints[JointType.ElbowLeft].Position.Z, 2));
            elbowToHandRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.HandRight].Position.X - ske.Joints[JointType.ElbowRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.HandRight].Position.Y - ske.Joints[JointType.ElbowRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HandRight].Position.Z - ske.Joints[JointType.ElbowRight].Position.Z, 2));
            elbowToHandLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.HandLeft].Position.X - ske.Joints[JointType.ElbowLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.HandLeft].Position.Y - ske.Joints[JointType.ElbowLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HandLeft].Position.Z - ske.Joints[JointType.ElbowLeft].Position.Z, 2));
            hipToKneeRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.HipRight].Position.X - ske.Joints[JointType.KneeRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.HipRight].Position.Y - ske.Joints[JointType.KneeRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HipRight].Position.Z - ske.Joints[JointType.KneeRight].Position.Z, 2));
            hipToKneeLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.HipLeft].Position.X - ske.Joints[JointType.KneeLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.HipLeft].Position.Y - ske.Joints[JointType.KneeLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.HipLeft].Position.Z - ske.Joints[JointType.KneeLeft].Position.Z, 2));
            kneeToFootRight = Math.Sqrt(Math.Pow(ske.Joints[JointType.FootRight].Position.X - ske.Joints[JointType.KneeRight].Position.X, 2) + Math.Pow(ske.Joints[JointType.FootRight].Position.Y - ske.Joints[JointType.KneeRight].Position.Y, 2) + Math.Pow(ske.Joints[JointType.FootRight].Position.Z - ske.Joints[JointType.KneeRight].Position.Z, 2));
            kneeToFootLeft = Math.Sqrt(Math.Pow(ske.Joints[JointType.FootLeft].Position.X - ske.Joints[JointType.KneeLeft].Position.X, 2) + Math.Pow(ske.Joints[JointType.FootLeft].Position.Y - ske.Joints[JointType.KneeLeft].Position.Y, 2) + Math.Pow(ske.Joints[JointType.FootLeft].Position.Z - ske.Joints[JointType.KneeLeft].Position.Z, 2));

        }
        public double getShoulderToElbowRight(){
            return shoulderToElbowRight;
        }
        public double getElbowToHandRight(){
            return elbowToHandRight;
        }
        public double getShoulderToHandRight()
        {
            return shoulderToElbowRight+elbowToHandRight;
        }
        public double getHipToKneeRight(){
            return hipToKneeRight;
        }
        public double getKneeToFootRight(){
            return kneeToFootRight;
        }
        public double getShoulderToElbowLeft(){
            return shoulderToElbowRight;
        }
        public double getElbowToHandLeft(){
            return elbowToHandLeft;
        }
        public double getShoulderToHandLeft()
        {
            return shoulderToElbowLeft + elbowToHandLeft;
        }
        public double getHipToKneeLeft(){
            return hipToKneeLeft;
        }
        public double getKneeToFootLeft(){
            return kneeToFootLeft;
        }
    }
}
