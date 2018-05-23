namespace GestureModality
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using GestureModality;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using Microsoft.Kinect.Wpf.Controls;
    using mmisharp;
    using System.Windows.Media;
    using System.Threading;

    public partial class MainWindow
    {

        private readonly string gestureDatabasePath = @"mov1.gbd";

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        private VisualGestureBuilderDatabase gestureDatabase;
        private string expression;
        private LifeCycleEvents lce;
        private MmiCommunication mmic;
        private MultiSourceFrameReader multiFrameReader;
        private Body[] bodies;
        private bool imgInicio = false;
        private bool imgmet = false;
        private bool imgmusic = false;
        private bool imghelp = false;
        private bool imgBack = false;
        private bool imgForward = false;
        private bool imgSports = false;
        private const int fpsDelay = 60;
        private int fpsCounter;
        private bool gestureWasDetected;
        private String userSelected;
        private String channelSelected;
        private int count;



        public MainWindow()
        {
            this.InitializeComponent();
            this.expression = "";
            



            lce = new LifeCycleEvents("GESTURES", "FUSION", "m", "gesture", "command");// LifeCycleEvents(string source, string target, string id, string medium, string mode)
            mmic = new MmiCommunication("localhost", 8000, "User1", "GESTURES");
            mmic.Send(lce.NewContextRequest());

            KinectRegion.SetKinectRegion(this, kinectRegion);
            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;

            // Use the default sensor
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
            this.kinectRegion.KinectSensor.Open();

            bodies = new Body[this.kinectRegion.KinectSensor.BodyFrameSource.BodyCount];

            gestureDatabase = new VisualGestureBuilderDatabase(this.gestureDatabasePath);
            vgbFrameSource = new VisualGestureBuilderFrameSource(this.kinectRegion.KinectSensor, 0);

            foreach (var gesture in gestureDatabase.AvailableGestures)
            {
                this.vgbFrameSource.AddGesture(gesture);
            }

            vgbFrameSource.TrackingId = 1;
            vgbFrameReader = vgbFrameSource.OpenReader();
            vgbFrameReader.IsPaused = true;
            
            vgbFrameReader.FrameArrived += gestureFrameReader_FrameArrived;

            multiFrameReader = this.kinectRegion.KinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
            multiFrameReader.MultiSourceFrameArrived += multiFrameReader_MultiSourceFrameArrived;
            fpsCounter = 0;
            count = 0;
            gestureWasDetected = false;
            
    }

        private void Exit(object sender, RoutedEventArgs e)
        {
            
            Environment.Exit(-1);
        }
        public void showMensage(String ext)
        {
 
            TextRegion.Text = ext;
            Console.WriteLine(ext);
        }

        private void Images(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            
            switch (button.Name)
            {
                case "inicio": sendMessage("inicio"); showMensage("     Irá para o inicio"); break;
                case "met": sendMessage("met"); showMensage("Irá ver a meteorologia"); break;
                case "music": sendMessage("music"); showMensage("Irá para ouvir música") ; break;
                case "Back": sendMessage("back"); showMensage("     Irá voltar a trás"); break;
                case "Forward": sendMessage("forward"); showMensage("Irá voltar a ver à página anterior"); break;
                case "sports": sendMessage("sports"); showMensage("Irá para uma página desportiva "); break;
                
            }
            
            
        }

        private void ResultRequest(object sender, RoutedEventArgs e)
        {
            sendMessage(expression);
        }

        

        private void Help(object sender, RoutedEventArgs e)
        {
            showMensage("                 Ajuda");
            sendMessage("Help");
        }

    
        private void gestureFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {

           

            if (this.gestureWasDetected)
            {
                this.fpsCounter++;
                if (fpsCounter == fpsDelay)
                {
                    this.fpsCounter = 0;
                    this.gestureWasDetected = false;
                }
                return;
            }

            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                
                float progress = 0;
                if (frame != null)
                {
                    
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
                    IReadOnlyDictionary<Gesture, ContinuousGestureResult> continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {
                            
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);
                            
                            if (result != null && result.Detected)
                            {
                               
                                
                                switch (gesture.Name)
                                {
                                    case "inicio": if (result.Confidence > 0.80) SendDetectedGesture(gesture, result.Confidence);  break;
                                    case "met": if (result.Confidence > 0.90) SendDetectedGesture(gesture, result.Confidence); break;
                                    case "music": if (result.Confidence > 0.90) SendDetectedGesture(gesture, result.Confidence); break;
                                    case "Help": if (result.Confidence > 0.80) SendDetectedGesture(gesture, result.Confidence); break;
                                    case "Close": if (result.Confidence > 0.90) SendDetectedGesture(gesture, result.Confidence); break;
                                   
                                    
                                }
                               
                            }
                        }
                    }
                    if (continuousResults != null)
                    {
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {

                            ContinuousGestureResult result = null;
                            continuousResults.TryGetValue(gesture, out result);

                            if (result != null )
                            {
                                progress = result.Progress;
                                if (progress >= 1.0)
                                {
                                    count++;
                                    if (count != 15)
                                    {
                                        return;
                                    }
                                    count = 0;
                                        if (gesture.Name.Equals("back") && progress>=1.1)
                                        {
                                            SendDetectedGesture(gesture, progress);
                                        }
                                        else if (gesture.Name.Equals("forward"))
                                        {
                                            SendDetectedGesture(gesture, progress);
                                        }
                                        else if (gesture.Name.Equals("sports"))
                                        {
                                            SendDetectedGesture(gesture, progress);
                                        }



                                }
                            }
                        }

                    }
                }
            }
        }

        private void multiFrameReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiFrame = e.FrameReference.AcquireFrame();

            if (!vgbFrameSource.IsTrackingIdValid)
            {
                using (BodyFrame bodyFrame = multiFrame.BodyFrameReference.AcquireFrame())
                {
                    if (bodyFrame != null)
                    {
                        bodyFrame.GetAndRefreshBodyData(bodies);
                        foreach (var body in bodies)
                        {
                            if (body != null && body.IsTracked)
                            {
                                vgbFrameSource.TrackingId = body.TrackingId;
                                vgbFrameReader.IsPaused = false;
                            }
                        }
                    }
                }
            }
        }

        private void SendDetectedGesture(Gesture gesture, double confidence)
        {
            this.gestureWasDetected = true;
            if (confidence >= 0.80)
            {
               
                if (imgInicio == true||imghelp==true || imgmet ==true || imgmusic == true || imgForward == true || imgBack == true || imgSports == true)
                {
                    
                    inicio.Background = Brushes.White;
                    met.Background = Brushes.White;
                    music.Background = Brushes.White;
                    helpButton.Background = Brushes.White;
                    Back.Background = Brushes.White;
                    Forward.Background = Brushes.White;
                    sports.Background = Brushes.White;
                }
                showMensage(gesture.Name);
                switch (gesture.Name)
                {
                    case "inicio": inicio.Background = Brushes.SkyBlue;
                                   showMensage("Irá para o inicio");
                                   imgInicio = true;
                                   System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "met": met.Background = Brushes.SkyBlue;
                                    showMensage("Irá ver a metereologia");
                                    imgmet = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "music": music.Background = Brushes.SkyBlue;
                                    showMensage("Irá para ouvir musica");
                                    imgmusic = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "back": Back.Background = Brushes.SkyBlue;
                                    showMensage("Irá voltar a trás");
                                    imgBack = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "forward":Forward.Background = Brushes.SkyBlue;
                                    showMensage("Irá voltar a ver a página anterior ");
                                    imgForward = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "sports":sports.Background = Brushes.SkyBlue;
                                    showMensage("Irá para uma pagina desportiva ");
                                    imgSports = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "Help":helpButton.Background = Brushes.SkyBlue;
                                    imghelp = true;
                                    System.Threading.Tasks.Task.Delay(5000);
                                    break;
                    case "Close":Environment.Exit(-1);
                                break;
                }
                

               
                string json = "{ \"recognized\": [";
                json += "\"" + confidence + "\", ";
                json += "\"" + gesture.Name + "\", ";
                json = json.Substring(0, json.Length - 2);
                json += "] }";
                Console.WriteLine(gesture.Name+"!!!!!"+ confidence);
             
                var exNot = lce.ExtensionNotification("", "", (float)confidence, json);
                mmic.Send(exNot);
            }

        }
        public void sendMessage(String message)
        {
            this.gestureWasDetected = true;
            if (imgInicio == true || imghelp == true || imgmet == true || imgmusic == true || imgForward == true || imgBack == true || imgSports == true)
            {
               
                inicio.Background = Brushes.White;
                met.Background = Brushes.White;
                music.Background = Brushes.White;
                helpButton.Background = Brushes.White;
                Back.Background = Brushes.White;
                Forward.Background = Brushes.White;
                sports.Background = Brushes.White;
            }
           
            switch (message)
            {
                case "inicio":
                    inicio.Background = Brushes.SkyBlue;
                    imgInicio = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "met":
                    met.Background = Brushes.SkyBlue;
                    imgmet = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "music":
                    music.Background = Brushes.SkyBlue;
                    imgmusic = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "back":
                    Back.Background = Brushes.SkyBlue;
                    imgBack = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "forward":
                    Forward.Background = Brushes.SkyBlue;
                    imgForward = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "Help":helpButton.Background = Brushes.SkyBlue;
                    imghelp = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
                case "sports":
                    sports.Background = Brushes.SkyBlue;
                    imgSports = true;
                    System.Threading.Tasks.Task.Delay(5000);
                    break;
            }

            string json = "{ \"recognized\": [";
            json += "\"" + "1" + "\", ";
            json += "\"" + message + "\", ";
            json = json.Substring(0, json.Length - 2);
            json += "] }";

            Console.WriteLine(message);
            var exNot = lce.ExtensionNotification("", "", 1, json);
            mmic.Send(exNot);
            
            

            //Saida.Background = Brushes.White;
        }


    }
}
