using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Nebula.Core.UI
{
    public static class ControlUtils
    {
        public static void ApplyBlur(UIElement element, double blurRadius, TimeSpan duration, TimeSpan beginTime)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 0.0
            };
            DoubleAnimation doubleAnimation1 =
                new DoubleAnimation(0.0, blurRadius, duration) {BeginTime = beginTime};
            DoubleAnimation doubleAnimation2 = doubleAnimation1;
            element.Effect = blurEffect;
            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, doubleAnimation2);
        }

        public static void RemoveBlur(UIElement element, TimeSpan duration, TimeSpan beginTime)
        {
            if (!(element.Effect is BlurEffect effect) || effect.Radius == 0.0)
                return;
            DoubleAnimation doubleAnimation1 = new DoubleAnimation(effect.Radius, 0.0, duration);
            doubleAnimation1.BeginTime = beginTime;
            DoubleAnimation doubleAnimation2 = doubleAnimation1;
            effect.BeginAnimation(BlurEffect.RadiusProperty, doubleAnimation2);
        }
    }
}