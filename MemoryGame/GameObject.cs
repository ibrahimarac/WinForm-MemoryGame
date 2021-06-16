using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Ibrahim.MemoryGame
{
    public enum State
    {
        Normal,
        Open,
        Matched
    }
    public partial class GameObject : UserControl
    {
        Bitmap openImage;
        Bitmap normalImage;
        int id;
        State objectState,previousState;

        public GameObject()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            ObjectState = State.Normal;
            PreviousState = State.Normal;
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public int ImageNumber { get; set; }

        public Bitmap OpenImage
        {
            get
            {
                return openImage;
            }
            set
            {
                openImage = value;
            }
        }

        public Bitmap NormalImage
        {
            get
            {
                return normalImage;
            }
            set
            {
                normalImage = value;
                this.BackgroundImage = value;
            }
        }
              
        public State ObjectState
        {
            get
            {
                return objectState;
            }
            set
            {
                PreviousState = objectState;
                objectState = value;
            }
        }

        public State PreviousState
        {
            get
            {
                return previousState;
            }
            set
            {
                previousState = value;
            }
        }
       
        void ChangeImage(Image imgSource,Image imgDestination)
        {
            Task.Run(() =>
            {
                for (float i = 1; i >=0; i-=0.1F)
                {
                    this.BackgroundImage=SetImageOpacity(imgSource, i);
                    Task.Delay(50).Wait();
                }
            }).ContinueWith(t=> {
                if (imgDestination != null)
                {
                    for (float i = 0; i <= 1; i += 0.1F)
                    {
                        this.BackgroundImage = SetImageOpacity(imgDestination, i);
                        Task.Delay(50).Wait();
                    }
                }                
            });
        }
        
        Image SetImageOpacity(Image image, float opacity)
        {
            //create a Bitmap the size of the image provided  
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            //create a graphics object from the image  
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();
                //set the opacity  
                matrix.Matrix33 = opacity;
                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();
                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        public void Animate()
        {
            if (ObjectState == State.Normal && PreviousState == State.Open)
            {
                ChangeImage(OpenImage, NormalImage);
            }
            else if (ObjectState == State.Open && PreviousState == State.Normal)
            {
                ChangeImage(NormalImage, OpenImage);
            }
            else if (ObjectState == State.Matched && PreviousState == State.Open)
            {
                ChangeImage(OpenImage, null);
            }
        }

    }
}

