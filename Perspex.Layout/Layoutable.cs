﻿// -----------------------------------------------------------------------
// <copyright file="Layoutable.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Layout
{
    using System;
    using System.Linq;
    using Perspex.VisualTree;
    using Splat;

    public enum HorizontalAlignment
    {
        Stretch,
        Left,
        Center,
        Right,
    }

    public enum VerticalAlignment
    {
        Stretch,
        Top,
        Center,
        Bottom,
    }

    public class Layoutable : Visual, ILayoutable, IEnableLogger
    {
        public static readonly PerspexProperty<double> WidthProperty =
            PerspexProperty.Register<Layoutable, double>("Width", double.NaN);

        public static readonly PerspexProperty<double> HeightProperty =
            PerspexProperty.Register<Layoutable, double>("Height", double.NaN);

        public static readonly PerspexProperty<double> MinWidthProperty =
            PerspexProperty.Register<Layoutable, double>("MinWidth");

        public static readonly PerspexProperty<double> MaxWidthProperty =
            PerspexProperty.Register<Layoutable, double>("MaxWidth", double.PositiveInfinity);

        public static readonly PerspexProperty<double> MinHeightProperty =
            PerspexProperty.Register<Layoutable, double>("MinHeight");

        public static readonly PerspexProperty<double> MaxHeightProperty =
            PerspexProperty.Register<Layoutable, double>("MaxHeight", double.PositiveInfinity);

        public static readonly PerspexProperty<Thickness> MarginProperty =
            PerspexProperty.Register<Layoutable, Thickness>("Margin");

        public static readonly PerspexProperty<HorizontalAlignment> HorizontalAlignmentProperty =
            PerspexProperty.Register<Layoutable, HorizontalAlignment>("HorizontalAlignment");

        public static readonly PerspexProperty<VerticalAlignment> VerticalAlignmentProperty =
            PerspexProperty.Register<Layoutable, VerticalAlignment>("VerticalAlignment");

        private Size? previousMeasure;

        private Rect? previousArrange;

        static Layoutable()
        {
            Layoutable.AffectsMeasure(Visual.IsVisibleProperty);
            Layoutable.AffectsMeasure(Layoutable.WidthProperty);
            Layoutable.AffectsMeasure(Layoutable.HeightProperty);
            Layoutable.AffectsMeasure(Layoutable.MinWidthProperty);
            Layoutable.AffectsMeasure(Layoutable.MaxWidthProperty);
            Layoutable.AffectsMeasure(Layoutable.MinHeightProperty);
            Layoutable.AffectsMeasure(Layoutable.MaxHeightProperty);
            Layoutable.AffectsMeasure(Layoutable.MarginProperty);
            Layoutable.AffectsMeasure(Layoutable.HorizontalAlignmentProperty);
            Layoutable.AffectsMeasure(Layoutable.VerticalAlignmentProperty);
        }

        public double Width
        {
            get { return this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        public double Height
        {
            get { return this.GetValue(HeightProperty); }
            set { this.SetValue(HeightProperty, value); }
        }

        public double MinWidth
        {
            get { return this.GetValue(MinWidthProperty); }
            set { this.SetValue(MinWidthProperty, value); }
        }

        public double MaxWidth
        {
            get { return this.GetValue(MaxWidthProperty); }
            set { this.SetValue(MaxWidthProperty, value); }
        }

        public double MinHeight
        {
            get { return this.GetValue(MinHeightProperty); }
            set { this.SetValue(MinHeightProperty, value); }
        }

        public double MaxHeight
        {
            get { return this.GetValue(MaxHeightProperty); }
            set { this.SetValue(MaxHeightProperty, value); }
        }

        public Thickness Margin
        {
            get { return this.GetValue(MarginProperty); }
            set { this.SetValue(MarginProperty, value); }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return this.GetValue(HorizontalAlignmentProperty); }
            set { this.SetValue(HorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return this.GetValue(VerticalAlignmentProperty); }
            set { this.SetValue(VerticalAlignmentProperty, value); }
        }

        public Size? DesiredSize
        {
            get;
            set;
        }

        public bool IsMeasureValid
        {
            get;
            private set;
        }

        public bool IsArrangeValid
        {
            get;
            private set;
        }

        Size? ILayoutable.PreviousMeasure
        {
            get { return this.previousMeasure; }
        }

        Rect? ILayoutable.PreviousArrange
        {
            get { return this.previousArrange; }
        }

        public virtual void ApplyTemplate()
        {
        }

        public void Measure(Size availableSize, bool force = false)
        {
            if (double.IsNaN(availableSize.Width) || double.IsNaN(availableSize.Height))
            {
                throw new InvalidOperationException("Cannot call Measure using a size with NaN values.");
            }

            if (force || !this.IsMeasureValid || this.previousMeasure != availableSize)
            {
                this.IsMeasureValid = true;

                var desiredSize = this.MeasureCore(availableSize).Constrain(availableSize);

                if (desiredSize.Width < 0 || desiredSize.Height < 0 ||
                    double.IsInfinity(desiredSize.Width) || double.IsInfinity(desiredSize.Height) ||
                    double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height))
                {
                    throw new InvalidOperationException("Invalid size returned for Measure.");
                }

                this.DesiredSize = desiredSize;
                this.previousMeasure = availableSize;

                this.Log().Debug(
                    "Measure of {0} (#{1:x8}) requested {2} ",
                    this.GetType().Name,
                    this.GetHashCode(),
                    this.DesiredSize);
            }
        }

        public void Arrange(Rect rect, bool force = false)
        {
            if (rect.Width < 0 || rect.Height < 0 ||
                double.IsInfinity(rect.Width) || double.IsInfinity(rect.Height) ||
                double.IsNaN(rect.Width) || double.IsNaN(rect.Height))
            {
                throw new InvalidOperationException("Invalid Arrange rectangle.");
            }

            // If the measure was invalidated during an arrange pass, wait for the measure pass to 
            // be re-run.
            if (!this.IsMeasureValid)
            {
                return;
            }

            if (force || !this.IsArrangeValid || this.previousArrange != rect)
            {
                this.Log().Debug(
                    "Arrange of {0} (#{1:x8}) gave {2} ",
                    this.GetType().Name,
                    this.GetHashCode(),
                    rect);

                this.IsArrangeValid = true;
                this.ArrangeCore(rect);
                this.previousArrange = rect;
            }
        }

        public void InvalidateMeasure()
        {
            var parent = this.GetVisualParent<ILayoutable>();

            if (this.IsMeasureValid)
            {
                this.Log().Debug(
                    "Invalidated measure of {0} (#{1:x8})",
                    this.GetType().Name,
                    this.GetHashCode());
            }

            this.IsMeasureValid = false;
            this.IsArrangeValid = false;
            this.previousMeasure = null;
            this.previousArrange = null;
            this.DesiredSize = null;

            if (parent != null && IsResizable(parent))
            {
                parent.InvalidateMeasure();
            }
            else
            {
                var root = this.GetLayoutRoot();

                if (root != null && root.Item1.LayoutManager != null)
                {
                    root.Item1.LayoutManager.InvalidateMeasure(this, root.Item2);
                }
            }
        }

        public void InvalidateArrange()
        {
            var root = this.GetLayoutRoot();

            if (this.IsArrangeValid)
            {
                this.Log().Debug(
                    "Invalidated arrange of {0} (#{1:x8})",
                    this.GetType().Name,
                    this.GetHashCode());
            }

            this.IsArrangeValid = false;
            this.previousArrange = null;

            if (root != null && root.Item1.LayoutManager != null)
            {
                root.Item1.LayoutManager.InvalidateArrange(this, root.Item2);
            }
        }

        protected static void AffectsArrange(PerspexProperty property)
        {
            property.Changed.Subscribe(AffectsArrangeInvalidate);
        }

        protected static void AffectsMeasure(PerspexProperty property)
        {
            property.Changed.Subscribe(AffectsMeasureInvalidate);
        }

        protected virtual void ArrangeCore(Rect finalRect)
        {
            if (this.IsVisible)
            {
                double originX = finalRect.X + this.Margin.Left;
                double originY = finalRect.Y + this.Margin.Top;
                var sizeMinusMargins = new Size(
                    Math.Max(0, finalRect.Width - this.Margin.Left - this.Margin.Right),
                    Math.Max(0, finalRect.Height - this.Margin.Top - this.Margin.Bottom));
                var size = sizeMinusMargins;

                if (this.HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    size = size.WithWidth(Math.Min(size.Width, this.DesiredSize.Value.Width));
                }

                if (this.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    size = size.WithHeight(Math.Min(size.Height, this.DesiredSize.Value.Height));
                }

                size = LayoutHelper.ApplyLayoutConstraints(this, size);
                size = this.ArrangeOverride(size).Constrain(size);

                switch (this.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        originX += (sizeMinusMargins.Width - size.Width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        originX += sizeMinusMargins.Width - size.Width;
                        break;
                }

                switch (this.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        originY += (sizeMinusMargins.Height - size.Height) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        originY += sizeMinusMargins.Height - size.Height;
                        break;
                }

                this.Bounds = new Rect(originX, originY, size.Width, size.Height);
            }
        }

        protected virtual Size ArrangeOverride(Size finalSize)
        {
            foreach (ILayoutable child in this.GetVisualChildren().OfType<ILayoutable>())
            {
                child.Arrange(new Rect(finalSize));
            }

            return finalSize;
        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            if (this.IsVisible)
            {
                this.ApplyTemplate();

                var constrained = LayoutHelper
                    .ApplyLayoutConstraints(this, availableSize)
                    .Deflate(this.Margin);

                var measured = this.MeasureOverride(constrained);
                var width = measured.Width;
                var height = measured.Height;

                if (!double.IsNaN(this.Width))
                {
                    width = this.Width;
                }

                width = Math.Min(width, this.MaxWidth);
                width = Math.Max(width, this.MinWidth);

                if (!double.IsNaN(this.Height))
                {
                    height = this.Height;
                }

                height = Math.Min(height, this.MaxHeight);
                height = Math.Max(height, this.MinHeight);

                return new Size(width, height).Inflate(this.Margin);
            }
            else
            {
                return new Size();
            }
        }

        protected virtual Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;

            foreach (ILayoutable child in this.GetVisualChildren().OfType<ILayoutable>())
            {
                child.Measure(availableSize);
                width = Math.Max(width, child.DesiredSize.Value.Width);
                height = Math.Max(height, child.DesiredSize.Value.Height);
            }

            return new Size(width, height);
        }

        private static void AffectsArrangeInvalidate(PerspexPropertyChangedEventArgs e)
        {
            ILayoutable control = e.Sender as ILayoutable;

            if (control != null)
            {
                control.InvalidateArrange();
            }
        }

        private static void AffectsMeasureInvalidate(PerspexPropertyChangedEventArgs e)
        {
            ILayoutable control = e.Sender as ILayoutable;

            if (control != null)
            {
                control.InvalidateMeasure();
            }
        }

        private static bool IsResizable(ILayoutable control)
        {
            return double.IsNaN(control.Width) || double.IsNaN(control.Height);
        }

        private Tuple<ILayoutRoot, int> GetLayoutRoot()
        {
            var control = (IVisual)this;
            var distance = 0;

            while (control != null && !(control is ILayoutRoot))
            {
                control = control.GetVisualParent();
                ++distance;
            }

            return control != null ? Tuple.Create((ILayoutRoot)control, distance) : null;
        }
    }
}
