﻿using System;
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
        public double minX, minY, maxX, maxY;
        private int borderWidth = 3;
        Canvas drawCanvas;

        public AreaSelectionCanvas()
            : base()
        {
            InitializeComponent();
            this.mouseDown = false;

            drawCanvas = new Canvas();
            drawCanvas.MouseDown += MouseDownEventHandler;
            drawCanvas.MouseUp += MouseUpEventHandler;
            drawCanvas.MouseMove += MouseMoveEventHandler;
            drawingGrid.Children.Add(drawCanvas);

            SnippingManager.Instance.hkeyWindowCap.Disabled = true;
            SnippingManager.Instance.hkeyAreaCap.Disabled = true;
            minX = minY = maxX = maxY = -1;
            ShowDialog();
            this.Activate();
            this.Focus();  
        }

        private void DrawRect(Point pos1, Point pos2)
        {
            Point upperLeft = new Point(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
            Point bottomRight = new Point(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y));

            Rectangle rect = new Rectangle();
            rect.MouseDown += MouseDownEventHandler;
            rect.MouseUp += MouseUpEventHandler;
            rect.MouseMove += MouseMoveEventHandler;
            rect.Stroke = Brushes.Black;

            Color fillColor = new Color();
            fillColor.A = 0x19;
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

            minX = Math.Min(startPos.X, finalPos.X);
            minY = Math.Min(startPos.Y, finalPos.Y);
            maxX = Math.Max(startPos.X, finalPos.X);
            maxY = Math.Max(startPos.Y, finalPos.Y);

            ExitCanvas();
        }


        private void ExitCanvas()
        {
            if(DialogResult == null)
            {
                SnippingManager.Instance.hkeyWindowCap.Disabled = false;
                SnippingManager.Instance.hkeyAreaCap.Disabled = false;
                DialogResult = minX >= 0 && minY >= 0 && maxX >= 0 && maxY >= 0 && maxX > minX && maxY > minY;
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
