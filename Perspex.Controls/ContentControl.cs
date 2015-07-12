﻿// -----------------------------------------------------------------------
// <copyright file="ContentControl.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;
    using Perspex.Collections;
    using Perspex.Controls.Presenters;
    using Perspex.Controls.Primitives;
    using Perspex.Controls.Templates;
    using Perspex.Layout;

    public class ContentControl : TemplatedControl, IContentControl, ILogical
    {
        public static readonly PerspexProperty<object> ContentProperty =
            PerspexProperty.Register<ContentControl, object>("Content");

        public static readonly PerspexProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            PerspexProperty.Register<ContentControl, HorizontalAlignment>("HorizontalContentAlignment");

        public static readonly PerspexProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            PerspexProperty.Register<ContentControl, VerticalAlignment>("VerticalContentAlignment");

        private PerspexReadOnlyListView<ILogical> logicalChildren = new PerspexReadOnlyListView<ILogical>();

        public ContentControl()
        {
        }

        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        public ContentPresenter Presenter
        {
            get;
            private set;
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return this.GetValue(HorizontalContentAlignmentProperty); }
            set { this.SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return this.GetValue(VerticalContentAlignmentProperty); }
            set { this.SetValue(VerticalContentAlignmentProperty, value); }
        }

        IPerspexReadOnlyList<ILogical> ILogical.LogicalChildren
        {
            get { return this.logicalChildren; }
        }

        protected override void OnTemplateApplied()
        {
            // We allow ContentControls without ContentPresenters in the template. This can be
            // useful for e.g. a simple ToggleButton that displays an image. There's no need to
            // have a ContentPresenter in the visual tree for that.
            this.Presenter = this.FindTemplateChild<ContentPresenter>("contentPresenter");
            this.logicalChildren.Source = ((ILogical)this.Presenter)?.LogicalChildren;
        }
    }
}
