using Ibrahim.MemoryGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ibrahim.MemoryGame
{
    public partial class Form1 : Form
    {
        Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        void AddControlToFlowLayout(FlowLayoutPanel panel, UserControl ctl)
        {
            if (flowContainer.InvokeRequired)
            {
                flowContainer.Invoke(new Action<FlowLayoutPanel, UserControl>(AddControlToFlowLayout), flowContainer, ctl);
            }
            panel.Controls.Add(ctl);
        }

        Task CreateGameArea()
        {
            return Task.Run(() =>
            {
                /*
                    Önce bir dictionary içerisine tüm elemanları yükleyelim. 
                */
                for (int i = 1; i <= 20; i++)
                {
                    int imageNumber = (i == 20 || i == 10 ? 10 : i % 10);
                    gameObjects.Add(i, new GameObject
                    {
                        NormalImage = Resources._0,
                        OpenImage = (Bitmap)
                            Resources.ResourceManager.GetObject("_" + imageNumber),
                        Id = i,
                        ImageNumber=imageNumber
                    });                    
                }
            }).ContinueWith((t)=> {
                Parallel.For(1, 21, (i) =>
                {
                    AddControlToFlowLayout(flowContainer, gameObjects[i]);
                });
            }).ContinueWith((t)=> {
                for (int i = 1; i <=20 ; i++)
                {
                    gameObjects[i].Click += GameObjectClick;
                }
            });
        }

        GameObject GetPartner(GameObject obj)
        {
            GameObject returnObj = null;
            for (int i = 1; i <= 20; i++)
            {
                if(gameObjects[i].ObjectState==State.Open 
                    && 
                    gameObjects[i].Id!=obj.Id)
                {
                    returnObj = gameObjects[i];
                }
            }
            return returnObj;
        }

        private void GameObjectClick(object sender, EventArgs e)
        {
            GameObject obj = sender as GameObject;

            if (obj.ObjectState != State.Normal)
                return;

            GameObject partner = GetPartner(obj);
            obj.ObjectState = State.Open;
            if (partner!=null)
            {
                if (partner.ImageNumber == obj.ImageNumber)
                {
                    partner.ObjectState = State.Matched;
                    obj.ObjectState = State.Matched;
                }
                else
                {
                    partner.ObjectState = State.Normal;
                }
                partner.Animate();
            }
            obj.Animate();
            
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CreateGameArea();
        }
    }
}

