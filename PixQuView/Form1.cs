using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace PixQuView
{
    public partial class Form1 : Form
    {

        private Queue<string> displayQueue = new Queue<string>();
        private List<Frame> frames = new List<Frame>();

        public Form1()
        {
            InitializeComponent();
        }

        private void startDrawing()
        {
            string text = System.IO.File.ReadAllText(@"TextFile1.json");
            dynamic entry = JsonConvert.DeserializeObject(text);

            string title = entry.title;

            for (int i = 0; i < entry.frames.Count; i++)
            {
                dynamic frame = entry.frames[i];
                int duration = frame.duration;
                string data = frame.data;
                frames.Add(new Frame(duration, data));
            }
            timer1.Start();
        }

        private void startDrawing(string inputdata)
        {
            dynamic messageBody = JsonConvert.DeserializeObject(inputdata);
            string author = messageBody.author;
            label1.Text = "Author: " + author;
            if (messageBody.action == "SCREEN")
            {
                dynamic entry = JsonConvert.DeserializeObject("" + messageBody.data);

                string title = entry.title;
                label2.Text = "Number of frames: " + entry.frames.Count;
                label4.Text = title;

                for (int i = 0; i < entry.frames.Count; i++)
                {
                    dynamic frame = entry.frames[i];
                    int duration = frame.duration;
                    string data = frame.data;
                    frames.Add(new Frame(duration, data));
                }
                timer1.Start();
            }
        }

        private int frameNum = 0;
        private Frame currentFrame;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (frameNum < frames.Count && frames.Count > 0)
            {
                currentFrame = frames[frameNum];
                timer1.Interval = currentFrame.getDuration();
                this.Refresh();
                frameNum++;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            drawPane.Paint += DrawPanel_Paint;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            int spacing = 22;
            int circleSize = 18;
            int nHorizontalRows = 60;
            int nVerticalRows = 40;

            drawPane.Width = nHorizontalRows * spacing;
            drawPane.Height = nVerticalRows * spacing;

            e.Graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, this.Width, this.Height));
            for (int x = 0; x < nHorizontalRows; x++)
            {
                for (int y = 0; y < nVerticalRows; y++)
                {
                    e.Graphics.DrawEllipse(Pens.Gray, new Rectangle(x * spacing, y * spacing, circleSize, circleSize));
                    if(currentFrame != null && currentFrame.getMatrix()[y, x] != null && currentFrame.getMatrix()[y,x].Equals("1"))
                    {
                        e.Graphics.FillEllipse(Brushes.Red, new Rectangle(x * spacing, y * spacing, circleSize, circleSize));
                    }
                }
            }
        }


        Random rnd = new Random();
        private Brush getRandomBrush()
        {
            int color = rnd.Next(0, 3);
            switch (color)
            {
                case 0:
                    return Brushes.Red;
                case 1:
                    return Brushes.Blue;
                case 2:
                    return Brushes.Orange;
                default:
                    return Brushes.Black;
            }

        }

        

        private void Form1_Resize(object sender, EventArgs e)
        {
            updatePanelLocation();
        }

        private void updatePanelLocation()
        {
            drawPane.Left = (panel1.Width / 2 - drawPane.Width / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task task = new Task( () => connectToWS() );
            task.Start();
        }

        private void connectToWS()
        {
            var ws = new WebSocket(txtSocketUrl.Text);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;
            ws.Connect();
            
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {

        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            MessageBox.Show("Connection closed");
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            this.Invoke((MethodInvoker)delegate
            {
                startDrawing(e.Data); 
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            startDrawing();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
