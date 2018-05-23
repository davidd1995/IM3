using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using multimodal;

namespace AppGui
{
    public partial class Form2 : Form
    {
        private MmiCommunication mmiC;
        private Tts t = new Tts();
        public Form2()
        {
            InitializeComponent();
            t.Speak("Olá, bem vindo ao Web browser!");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.GoHome();
            

            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();
        }
        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);
            Double confidence = Convert.ToDouble((string)json.recognized[0].ToString());
            Console.WriteLine((string)json.recognized[0].ToString());
            Console.WriteLine((string)json.recognized[1].ToString());
           String comando = (string)json.recognized[1].ToString();
            /*String goal = (string)json.recognized[2].ToString();
            String final = (string)json.recognized[3].ToString();
            String type = (string)json.recognized[4].ToString();*/





           /* if (t.getSpeech() == true)
            {
                return;
            }*/
            if (confidence >= 0.7)
            {
                
                switch (comando)
                { 
                    case "inicio":
                        t.Speak("Irá voltár para o inicio");
                        webBrowser1.GoHome();
                        break;
                    case "met":
                        t.Speak("Irá ver o estado do tempo");
                        webBrowser1.Navigate("https://www.ipma.pt/pt/otempo/obs.tempo.presente/");
                        break;
                    case "music":
                        t.Speak("As melhores músicas só para si.");
                        webBrowser1.Navigate("https://www.youtube.com/watch?v=f4Mc-NYPHaQ&index=2&list=RDEMbHaAxpOZhcVmmF6I3y0siA");
                        break;
                    case "back":
                        t.Speak("Irá voltár a trás");
                        webBrowser1.GoBack();
                        break;
                    case "forward":
                        t.Speak("Irá voltár a ver a página anterior");
                        webBrowser1.GoForward();
                        break;
                    case "sports": t.Speak("Irá ver informações sobre desporto.");
                        webBrowser1.Navigate("https://news.google.com/news/search/section/q/desporto/futebol?hl=pt-PT&gl=PT&ned=pt-pt_pt");
                        break;
                    case "Help":
                        t.Speak("Pode realizar várias operações, como por exemplo, voltár ao início, ler noticias de desporto,ver o estado do tempo, ouvir musica");
                        webBrowser1.GoHome();
                        break;
                }
            }
            /*else
            {
                t.Speak(" Desculpe, hoje estou um pouco lenta! Poderia repetir de novo por favor?");

            }*/
        }


private void toolStripButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            webBrowser1.Stop();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(toolStripTextBox1.Text);
        }
       

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = webBrowser1.Url.ToString();
        }

        // Updates the URL in TextBoxAddress upon navigation.
        private void webBrowser1_Navigated(object sender,
            EventArgs e)
        {
            toolStripTextBox1.Text = webBrowser1.Url.ToString();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}