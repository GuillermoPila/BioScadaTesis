using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace WpfComponent.Component.Visuals
{
    public class CircularLight : Common.Component
    {
        public CircularLight()
        {
            
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Pen pen = new Pen(new SolidColorBrush(Colors.Black), 1);
            if (LightOn)
            {
                GradientStopCollection stopC2 = new GradientStopCollection();
                stopC2.Add(new GradientStop(Colors.White, 0));
                stopC2.Add(new GradientStop(Colors.Red, 1));
                RadialGradientBrush r2 = new RadialGradientBrush(stopC2);
                r2.RadiusX = 1;
                r2.RadiusY = 1;
                r2.GradientOrigin = new Point(0.7, 0.3);

                drawingContext.DrawEllipse(r2, pen, new Point(Width / 2, Height / 2), Width / 2, Height / 2);
            }
            else
            {
                GradientStopCollection stopC1 = new GradientStopCollection();
                stopC1.Add(new GradientStop(Colors.White, 0));
                stopC1.Add(new GradientStop(Colors.LightGreen, 1));
                RadialGradientBrush r1 = new RadialGradientBrush(stopC1);
                r1.RadiusX = 1;
                r1.RadiusY = 1;
                r1.GradientOrigin = new Point(0.7, 0.3);

                drawingContext.DrawEllipse(r1, pen, new Point(Width / 2, Height / 2), Width / 2, Height / 2);
            }
                
        }

        public static readonly DependencyProperty ColorProperty;
        public static readonly DependencyProperty LightOnProperty;
      



        static CircularLight()
        {
            ColorProperty = DependencyProperty.Register("Color", typeof (Color), typeof (FrameworkElement),
                                                        new FrameworkPropertyMetadata(Colors.Green,
                                                                                      FrameworkPropertyMetadataOptions.
                                                                                          AffectsRender));
            LightOnProperty = DependencyProperty.Register("LightOn", typeof(bool), typeof(FrameworkElement),
                                                        new FrameworkPropertyMetadata(false,
                                                                                      FrameworkPropertyMetadataOptions.
                                                                                          AffectsRender));
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        
        
        public bool LightOn
        {
            get { return (bool)GetValue(LightOnProperty); }
            set
            {
                SetValue(LightOnProperty, value);
            }
        }
    }
}
