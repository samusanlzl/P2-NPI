using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class TargetPoint
    {
        SkeletonPoint p;
        double pointRadio;
        public TargetPoint(double a, double b, double c, double r)
        {
            p = new SkeletonPoint();
            p.X = (float)a;
            p.Y = (float)b;
            p.Z = (float)c;
            pointRadio = r;
        }
        public TargetPoint(SkeletonPoint point, double r)
        {
            p = new SkeletonPoint();
            p.X = point.X;
            p.Y = point.Y;
            p.Z = point.Z;
            pointRadio = r;
        }

        public bool isPointInArea(SkeletonPoint point){
            double d = Math.Sqrt(Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2) + Math.Pow(p.Z - point.Z, 2));
            if (d <= pointRadio)
                return true;
            else
                return false;
        }
        public void updateTarget(double a, double b, double c)
        {
            p.X = (float)a;
            p.Y = (float)b;
            p.Z = (float)c;
        }
        public void updateTarget(SkeletonPoint point)
        {
            p.X = point.X;
            p.Y = point.Y;
            p.Z = point.Z;
        }
        public void updateRadio(double r)
        {
            pointRadio = r;
        }
        public double getRadio()
        {
            return pointRadio;
        }
        public SkeletonPoint getPoint()
        {
            return p;
        }
    }
}
