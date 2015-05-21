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
        private int borderWidth = 3;
        Canvas drawCanvas;

        public AreaSelectionCanvas()
            : base()
        {
            InitializeComponent();
            this.mouseDown = false;

            drawCanvas = new Canvas();
            drawCanvas.MouseUp += MouseUpEventHandler;
            drawCanvas.MouseMove += MouseMoveEventHandler;
            drawingGrid.Children.Add(drawCanvas);

            SnippingManager.Instance.hkeyWindowCap.Disabled = true;
            SnippingManager.Instance.hkeyAreaCap.Disabled = true;
            startX = startY = finalX = finalY = -1;
            ShowDialog();
            this.Activate();
            this.Focus();  
        }

        private void DrawRect(Point pos1, Point pos2)
        {
            Point upperLeft = new Point(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
            Point bottomRight = new Point(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y));

            Rectangle rect = new Rectangle();
            rect.MouseUp += MouseUpEventHandler;
            rect.MouseMove += MouseMoveEventHandler;
            rect.Stroke = Brushes.Black;

            Color fillColor = new Color();
            fillColor.A = 0x1f;
            fillColor.R = fillColor.G = fillColor.B = 0xff;
            rect.Fill = new SolidColorBrush(fillColor);

            rect.Width = bottomRight.X - upperLeft.X + 2 * borderWidth;
            rect.Height = bottomRight.Y - upperLeft.Y + 2 * borderWidth;
            rect.StrokeThickness = borderWidth;
            drawCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, upperLeft.X - borderWidth);
            Canvas.SetTop(rect, upperLeft.Y - borderWidth);
        }


        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point curPos = e.GetPosition(this);

                drawCanvas.Children.Clear();
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
