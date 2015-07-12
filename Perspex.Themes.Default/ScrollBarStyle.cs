﻿// -----------------------------------------------------------------------
// <copyright file="ScrollBarStyle.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Themes.Default
{
    using System.Linq;
    using Perspex.Controls;
    using Perspex.Controls.Presenters;
    using Perspex.Controls.Primitives;
    using Perspex.Media;
    using Perspex.Styling;

    public class ScrollBarStyle : Styles
    {
        public ScrollBarStyle()
        {
            this.AddRange(new[]
            {
                new Style(x => x.OfType<ScrollBar>())
                {
                    Setters = new[]
                    {
                        new Setter(ScrollBar.TemplateProperty, ControlTemplate.Create<ScrollBar>(this.Template)),
                    },
                },
                new Style(x => x.OfType<ScrollBar>().Class(":horizontal"))
                {
                    Setters = new[]
                    {
                        new Setter(ScrollBar.TemplateProperty, ControlTemplate.Create<ScrollBar>(this.Template)),
                        new Setter(ScrollBar.HeightProperty, 10.0),
                    },
                },
                new Style(x => x.OfType<ScrollBar>().Class(":horizontal").Template().Name("thumb"))
                {
                    Setters = new[]
                    {
                        new Setter(Thumb.MinWidthProperty, 10.0),
                    },
                },
                new Style(x => x.OfType<ScrollBar>().Class(":vertical"))
                {
                    Setters = new[]
                    {
                        new Setter(ScrollBar.TemplateProperty, ControlTemplate.Create<ScrollBar>(this.Template)),
                        new Setter(ScrollBar.WidthProperty, 10.0),
                    },
                },
                new Style(x => x.OfType<ScrollBar>().Class(":vertical").Template().Name("thumb"))
                {
                    Setters = new[]
                    {
                        new Setter(Thumb.MinHeightProperty, 10.0),
                    },
                },
            });
        }

        private Control Template(ScrollBar control)
        {
            return new Border
            {
                Background = Brushes.Silver,
                Content = new Track
                {
                    Name = "track",
                    [~Track.MinimumProperty] = control[~ScrollBar.MinimumProperty],
                    [~Track.MaximumProperty] = control[~ScrollBar.MaximumProperty],
                    [~Track.ViewportSizeProperty] = control[~ScrollBar.ViewportSizeProperty],
                    [~Track.OrientationProperty] = control[~ScrollBar.OrientationProperty],
                    Thumb = new Thumb
                    {
                        Name = "thumb",
                        Template = ControlTemplate.Create<Thumb>(this.ThumbTemplate),
                    },
                },
            };
        }

        private Control ThumbTemplate(Thumb control)
        {
            return new Border
            {
                Background = Brushes.Gray,
            };
        }
    }
}
