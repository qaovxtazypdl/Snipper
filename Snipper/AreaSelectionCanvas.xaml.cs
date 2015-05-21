using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Snipper
{
    public partial class AreaSelectionCanvas : Window
    {
        private bool mouseDown;
        private Point startPos;
        public double startX, startY, finalX, finalY;

        public AreaSelectionCanvas()
            : base()
        {
            InitializeComponent();
            this.mouseDown = false;
            SnippingManager.Instance.hkeyWindowCap.Disabled = true;
            SnippingManager.Instance.hkeyAreaCap.Disabled = true;
            startX = startY = finalX = finalY = -1;
            ShowDialog();
            this.Activate();
            this.Focus();  
        }

        private void DrawRect(Point pos1, Point pos2)
        {
            Canvas ellipseCanvas = new Canvas();
            Rectangle rect = new Rectangle();
            rect.Stroke = Brushes.Black;
            rect.Fill = Brushes.AliceBlue;
            rect.Width = pos2.X - pos1.X;
            rect.Height = pos2.Y - pos1.Y;
            rect.StrokeThickness = 3;
            ellipseCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, pos1.X);
            Canvas.SetTop(rect, pos1.Y);
            drawingGrid.Children.Add(ellipseCanvas);
        }


        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point curPos = e.GetPosition(this);

                drawingGrid.Children.Clear();
                DrawRect(startPos, curPos);
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            startPos = e.GetPosition(this);
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            mouseDown = false;

            Point finalPos = e.GetPosition(this);
            finalX = finalPos.X;
            finalY = finalPos.Y;

            startX = Math.Min(startX, finalX);
            startY = Math.Min(startY, finalY);
            finalX = Math.Max(startX, finalX);
            finalY = Math.Max(startY, finalY);

            ExitCanvas();
        }


        private void ExitCanvas()
        {
            if(DialogResult == null)
            {
                SnippingManager.Instance.hkeyWindowCap.Disabled = false;
                SnippingManager.Instance.hkeyAreaCap.Disabled = false;
                DialogResult = startX >= 0 && startY >= 0 && finalX >= 0 && finalY >= 0 && finalX > startX && finalY > startY;
            }
        }

        private void KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                ExitCanvas();
            }
        }
    }
}
