﻿// -----------------------------------------------------------------------
// <copyright file="ItemsPresenter.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls.Presenters
{
    using Perspex.Controls.Generators;
    using Perspex.Input;
    using Perspex.Styling;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    public class ItemsPresenter : Control, IItemsPresenter, ITemplatedControl
    {
        public static readonly PerspexProperty<IEnumerable> ItemsProperty =
            ItemsControl.ItemsProperty.AddOwner<ItemsPresenter>();

        public static readonly PerspexProperty<ItemsPanelTemplate> ItemsPanelProperty =
            ItemsControl.ItemsPanelProperty.AddOwner<ItemsPresenter>();

        private bool createdPanel;

        static ItemsPresenter()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
                typeof(ItemsPresenter),
                KeyboardNavigationMode.Once);
        }

        public ItemsPresenter()
        {
            this.GetObservableWithHistory(ItemsProperty).Subscribe(this.ItemsChanged);
        }

        public ItemContainerGenerator ItemContainerGenerator
        {
            get;
            private set;
        }

        public IEnumerable Items
        {
            get { return this.GetValue(ItemsProperty); }
            set { this.SetValue(ItemsProperty, value); }
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get { return this.GetValue(ItemsPanelProperty); }
            set { this.SetValue(ItemsPanelProperty, value); }
        }

        public Panel Panel
        {
            get;
            private set;
        }

        public override sealed void ApplyTemplate()
        {
            if (!this.createdPanel)
            {
                this.CreatePanel();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.Panel.Measure(availableSize);
            return this.Panel.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Panel.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private void CreatePanel()
        {
            this.ClearVisualChildren();
            this.Panel = this.ItemsPanel.Build();
            this.Panel.TemplatedParent = this;
            KeyboardNavigation.SetTabNavigation(this.Panel, KeyboardNavigation.GetTabNavigation(this));
            ((IItemsPanel)this.Panel).ChildLogicalParent = this.TemplatedParent as ILogical;
            this.AddVisualChild(this.Panel);
            this.createdPanel = true;
            this.ItemsChanged(Tuple.Create(default(IEnumerable), this.Items));
        }

        private IItemContainerGenerator GetGenerator()
        {
            if (this.ItemContainerGenerator == null)
            {
                ItemsControl i = this.TemplatedParent as ItemsControl;
                this.ItemContainerGenerator = i?.ItemContainerGenerator ?? new ItemContainerGenerator(this);
            }

            return this.ItemContainerGenerator;
        }

        private void ItemsChanged(Tuple<IEnumerable, IEnumerable> value)
        {
            if (this.createdPanel)
            {
                var generator = this.GetGenerator();

                if (value.Item1 != null)
                {
                    generator.RemoveAll();
                    this.Panel.Children.Clear();

                    INotifyCollectionChanged incc = value.Item1 as INotifyCollectionChanged;

                    if (incc != null)
                    {
                        incc.CollectionChanged -= this.ItemsCollectionChanged;
                    }
                }

                if (this.Panel != null)
                {
                    if (value.Item2 != null)
                    {
                        this.Panel.Children.AddRange(generator.Generate(this.Items));

                        INotifyCollectionChanged incc = value.Item2 as INotifyCollectionChanged;

                        if (incc != null)
                        {
                            incc.CollectionChanged += this.ItemsCollectionChanged;
                        }
                    }
                }
            }
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.createdPanel)
            {
                var generator = this.GetGenerator();

                // TODO: Handle Move and Replace etc.
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.Panel.Children.AddRange(generator.Generate(e.NewItems));
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        this.Panel.Children.RemoveAll(generator.Remove(e.OldItems));
                        break;
                }

                this.InvalidateMeasure();
            }
        }
    }
}
