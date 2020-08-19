using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Nebula.Core.Extensions
{
    /// <summary>
    /// Provide extensions for <see cref="UIElement"/>
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Apply blur effect to the specified <see cref="UIElement"/>
        /// </summary>
        /// <param name="element">Element to blur</param>
        /// <param name="blurRadius">Blur radius</param>
        /// <param name="duration">Blur transition duration</param>
        /// <param name="beginTime">Blur transition start time</param>
        public static void ApplyBlur(this UIElement element, double blurRadius, TimeSpan duration, TimeSpan beginTime)
        {
            BlurEffect blurEffect = new BlurEffect {Radius = 0.0};
            DoubleAnimation doubleAnimation1 = new DoubleAnimation(0.0, blurRadius, duration) {BeginTime = beginTime};
            DoubleAnimation doubleAnimation2 = doubleAnimation1;
            element.Effect = blurEffect;
            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, doubleAnimation2);
        }

        /// <summary>
        /// Remove blur effect from the specified <see cref="UIElement"/>
        /// </summary>
        /// <param name="element">Element to clear</param>
        /// <param name="duration">Blur transition duration</param>
        /// <param name="beginTime">Blur transition start time</param>
        public static void RemoveBlur(this UIElement element, TimeSpan duration, TimeSpan beginTime)
        {
            if (!(element.Effect is BlurEffect effect) || effect.Radius == 0.0)
                return;
            DoubleAnimation doubleAnimation1 = new DoubleAnimation(effect.Radius, 0.0, duration) {BeginTime = beginTime};
            DoubleAnimation doubleAnimation2 = doubleAnimation1;
            effect.BeginAnimation(BlurEffect.RadiusProperty, doubleAnimation2);
        }
    }
}