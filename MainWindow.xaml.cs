//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Media.Imaging;
    using System.Timers;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows.Documents;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Globalization;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;
        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 800.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 600.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Red, 6);
        private readonly Pen correctPosition = new Pen(Brushes.Green, 6);
        private readonly Pen wrongPosition = new Pen(Brushes.Red, 6);
        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        private const string MediumGreyBrushKey = "MediumGreyBrush";

        //private KneelingPosition pos;
        private bool comenzado = false;
        private bool terminado = false;
        private bool visible = false;
        SkeletonMeasures skMeasures = new SkeletonMeasures();
        Exercise exercise = new Exercise(2,3);
        Movement[] movs = new Movement[2];
        TargetPoint[] tp1 = new TargetPoint[3];
        TargetPoint[] tp2 = new TargetPoint[3];
        int Step = 0;
        int score;
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            for (int i = 0; i < 3; i++)
            {
                tp1[i] = new TargetPoint(0,0,0, 5);
                tp2[i] = new TargetPoint(0, 0, 0, 5);
            }
            movs[0] = new Movement(tp1);
            movs[1] = new Movement(tp2);
            InitializeComponent();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            this.Image2i.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image1i.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;


                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
            RecognizerInfo ri = GetKinectRecognizer();
            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);
                var directions = new Choices();
                directions.Add(new SemanticResultValue("play", "PLAY"));
                directions.Add(new SemanticResultValue("pley", "PLAY"));
                directions.Add(new SemanticResultValue("photo", "PHOTO"));
                directions.Add(new SemanticResultValue("capture", "PHOTO"));
                directions.Add(new SemanticResultValue("finish", "FINISH"));
                directions.Add(new SemanticResultValue("finis", "FINISH"));
                directions.Add(new SemanticResultValue("exit", "EXIT"));
                directions.Add(new SemanticResultValue("exi", "EXIT"));
                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);
                var g = new Grammar(gb);
                speechEngine.LoadGrammar(g);



                speechEngine.SpeechRecognized += SpeechRecognized;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }

        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();
                this.sensor.Stop();
                this.sensor = null;
            }
            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.RecognizeAsyncStop();
            }

        }


        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;


            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "PLAY":
                        if (!visible)
                        {
                            this.text1t.Visibility = System.Windows.Visibility.Visible;
                            this.text1t.Text = "Tu cuerpo no se visualiza correctamente,\n prueba de nuevo ha colocarte y decir PLAY";
                            this.Image2i.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            comenzado = true;
                            this.text1t.Visibility = System.Windows.Visibility.Hidden;
                            this.Image1i.Visibility = System.Windows.Visibility.Visible;
                            this.Image2i.Visibility = System.Windows.Visibility.Visible;
                            this.statusBarText.Text = "YA HA COMENZADO EL EJERCICIO, para terminar di 'FINISH'";
                        }
                        break;
                    case "FINISH":
                        if (comenzado)
                        {
                            this.text1t.Visibility = System.Windows.Visibility.Visible;
                            this.text1t.Text = "Has logrado " + score + " puntos";
                            terminado = true;
                        }
                        break;

                    case "PHOTO":

                            // create a png bitmap encoder which knows how to save a .png file
                            BitmapEncoder encoder = new PngBitmapEncoder();

                            // create frame from the writable bitmap and add to encoder
                            encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));

                            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

                            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                            string path = Path.Combine(myPhotos, "KinectSnapshot-" + time + ".png");

                            // write the new file to disk
                            try
                            {
                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    encoder.Save(fs);
                                }

                            }
                            catch (IOException)
                            {
                                this.statusBarText.Text = "Error en la captura de pantalla";
                            }
                            this.statusBarText.Text = "La foto se ha almacenado en "+path;
                        break;
                    case "EXIT":
                        Application.Current.Shutdown();
                        break;
                    default:
                        this.statusBarText.Visibility = System.Windows.Visibility.Visible;
                        this.statusBarText.Text = "No se ha entendido lo que has dicho";
                        break;
                }
            }
            
        }


        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            Random random = new Random();

            int i = 3;
            if (terminado)
            {
                i = random.Next(0, 4);
                text1t.FontSize = 48;
            }
            if(i==0)
                text1t.Foreground = new SolidColorBrush(Colors.Yellow);
            if(i==1)
                text1t.Foreground = new SolidColorBrush(Colors.Blue);
            if(i==2)
                text1t.Foreground = new SolidColorBrush(Colors.Red);
            if(i==3)
                text1t.Foreground = new SolidColorBrush(Colors.Green);
            if(i==4)
                text1t.Foreground = new SolidColorBrush(Colors.Black);
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        // Copy the pixel data from the image to a temporary array
                        colorFrame.CopyPixelDataTo(this.colorPixels);

                        // Write the pixel data into our bitmap
                        this.colorBitmap.WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                            this.colorPixels,
                            this.colorBitmap.PixelWidth * sizeof(int),
                            0);
                    }
                }
        }

        public void drawTargets(DrawingContext dc, Brush b, int i)
        {
            if (exercise.isInitialized())
            {
                Point p;
                for (int j = 0; j < exercise.getNumMovs(); j++)
                {
                    FormattedText n = new FormattedText((i + 1).ToString(), CultureInfo.GetCultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Arial Black"), 16, Brushes.Black);
                    p = SkeletonPointToScreen(exercise.getMov(j).getTarget(i).getPoint());
                    dc.DrawEllipse(b, null, p,15, 15);
                    dc.DrawText(n, p);
                }
            }
        }


        public void updateAll(Skeleton ske)
        {
            double x,y,z;
            double r = 0.21;
            skMeasures.updateMeasures(ske);
            x = ske.Joints[JointType.ShoulderRight].Position.X + skMeasures.getShoulderToHandRight();
            y = ske.Joints[JointType.ShoulderRight].Position.Y;
            z = ske.Joints[JointType.ShoulderRight].Position.Z;
            tp1[0].updateTarget(x,y,z);
            tp1[0].updateRadio(r);
            x = ske.Joints[JointType.ShoulderRight].Position.X + skMeasures.getShoulderToHandRight()*Math.Cos(33*Math.PI/150);
            y = ske.Joints[JointType.ShoulderRight].Position.Y - skMeasures.getShoulderToHandRight()*Math.Sin(33*Math.PI/150);
            z = ske.Joints[JointType.ShoulderRight].Position.Z;
            tp1[1].updateTarget(x,y,z);
            tp1[1].updateRadio(r);
            x = ske.Joints[JointType.ShoulderRight].Position.X + skMeasures.getShoulderToHandRight()*Math.Cos(66*Math.PI/150);
            y = ske.Joints[JointType.ShoulderRight].Position.Y - skMeasures.getShoulderToHandRight()*Math.Sin(66*Math.PI/150);
            z = ske.Joints[JointType.ShoulderRight].Position.Z;
            tp1[2].updateTarget(x,y,z);
            tp1[2].updateRadio(r);

            x = ske.Joints[JointType.ShoulderLeft].Position.X - skMeasures.getShoulderToHandLeft();
            y = ske.Joints[JointType.ShoulderLeft].Position.Y;
            z = ske.Joints[JointType.ShoulderLeft].Position.Z;
            tp2[0].updateTarget(x,y,z);
            tp2[0].updateRadio(r);
            x = ske.Joints[JointType.ShoulderLeft].Position.X - skMeasures.getShoulderToHandLeft()*Math.Cos(33*Math.PI/150);
            y = ske.Joints[JointType.ShoulderLeft].Position.Y - skMeasures.getShoulderToHandLeft()*Math.Sin(33*Math.PI/150);
            z = ske.Joints[JointType.ShoulderLeft].Position.Z;
            tp2[1].updateTarget(x,y,z);
            tp2[1].updateRadio(r);
            x = ske.Joints[JointType.ShoulderLeft].Position.X - skMeasures.getShoulderToHandLeft()*Math.Cos(66*Math.PI/150);
            y = ske.Joints[JointType.ShoulderLeft].Position.Y - skMeasures.getShoulderToHandLeft()*Math.Sin(66*Math.PI/150);
            z = ske.Joints[JointType.ShoulderLeft].Position.Z;
            tp2[2].updateTarget(x,y,z);
            tp2[2].updateRadio(r);
            movs[0].updateMov(tp1);
            movs[1].updateMov(tp2);
            exercise.updateMovements(movs);
        }
        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            
            Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0,0.0, RenderWidth, RenderHeight));
                    if (skeletons.Length != 0)
                    {
                        foreach (Skeleton skel in skeletons)
                        {
                            
                            if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                visible = true;
                                if (comenzado && !terminado)
                                {
                                    updateAll(skel);
                                    //this.DrawBonesAndJoints(skel, dc);
                                    SkeletonPoint[] listOfPoints = {skel.Joints[JointType.WristRight].Position, skel.Joints[JointType.WristLeft].Position};
                                    for (int i = 0; i < 3; i++)
                                        if (exercise.isInMovementN(listOfPoints, i))
                                        {
                                            drawTargets(dc, Brushes.Green, i);
                                            Step = (i+1)%3;
                                            score++;
                                        }
                                        else
                                            if (exercise.isInErrorMovement(listOfPoints, i))
                                            {
                                                drawTargets(dc, Brushes.Red, i);
                                                Step = (i + 1) % 3;
                                                score--;
                                            }else
                                                drawTargets(dc, Brushes.Yellow, i);

                                }
                            }
                            else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                            {
                                visible = false;
                                dc.DrawEllipse(
                                this.centerPointBrush,
                                null,
                                this.SkeletonPointToScreen(skel.Position),
                                BodyCenterThickness,
                                BodyCenterThickness);
                            }
                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(-120.0, -200.0, RenderWidth, RenderHeight));
                }
        }



        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }




        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }
            Pen drawPen;
            //pos = new KneelingPosition(skeleton);
            
            //bool cPos = pos.correctPosition(skeleton, 30);
            // We assume all drawn bones are inferred unless BOTH joints are tracked
            drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                //if (cPos)
                    //drawPen = correctPosition;
                //else
                drawPen = wrongPosition;
            }
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

    }
}