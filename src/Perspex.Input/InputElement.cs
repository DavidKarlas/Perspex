﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Linq;
using Perspex.Interactivity;
using Perspex.Rendering;
using Perspex.VisualTree;

namespace Perspex.Input
{
    /// <summary>
    /// Implements input-related functionality for a control.
    /// </summary>
    public class InputElement : Interactive, IInputElement
    {
        /// <summary>
        /// Defines the <see cref="Focusable"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> FocusableProperty =
            PerspexProperty.Register<InputElement, bool>(nameof(Focusable));

        /// <summary>
        /// Defines the <see cref="IsEnabled"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> IsEnabledProperty =
            PerspexProperty.Register<InputElement, bool>(nameof(IsEnabled), true);

        /// <summary>
        /// Defines the <see cref="IsEnabledCore"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> IsEnabledCoreProperty =
            PerspexProperty.Register<InputElement, bool>("IsEnabledCore", true);

        /// <summary>
        /// Gets or sets associated mouse cursor.
        /// </summary>
        public static readonly PerspexProperty<Cursor> CursorProperty =
            PerspexProperty.Register<InputElement, Cursor>("Cursor", null, true);

        /// <summary>
        /// Defines the <see cref="IsFocused"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> IsFocusedProperty =
            PerspexProperty.Register<InputElement, bool>("IsFocused");

        /// <summary>
        /// Defines the <see cref="IsHitTestVisible"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> IsHitTestVisibleProperty =
            PerspexProperty.Register<InputElement, bool>("IsHitTestVisible", true);

        /// <summary>
        /// Defines the <see cref="IsPointerOver"/> property.
        /// </summary>
        public static readonly PerspexProperty<bool> IsPointerOverProperty =
            PerspexProperty.Register<InputElement, bool>("IsPointerOver");

        /// <summary>
        /// Defines the <see cref="GotFocus"/> event.
        /// </summary>
        public static readonly RoutedEvent<GotFocusEventArgs> GotFocusEvent =
            RoutedEvent.Register<InputElement, GotFocusEventArgs>("GotFocus", RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="LostFocus"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> LostFocusEvent =
            RoutedEvent.Register<InputElement, RoutedEventArgs>("LostFocus", RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="KeyDown"/> event.
        /// </summary>
        public static readonly RoutedEvent<KeyEventArgs> KeyDownEvent =
            RoutedEvent.Register<InputElement, KeyEventArgs>(
                "KeyDown",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="KeyUp"/> event.
        /// </summary>
        public static readonly RoutedEvent<KeyEventArgs> KeyUpEvent =
            RoutedEvent.Register<InputElement, KeyEventArgs>(
                "KeyUp",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="TextInput"/> event.
        /// </summary>
        public static readonly RoutedEvent<TextInputEventArgs> TextInputEvent =
            RoutedEvent.Register<InputElement, TextInputEventArgs>(
                "TextInput",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="PointerEnter"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerEventArgs> PointerEnterEvent =
            RoutedEvent.Register<InputElement, PointerEventArgs>("PointerEnter", RoutingStrategies.Direct);

        /// <summary>
        /// Defines the <see cref="PointerLeave"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerEventArgs> PointerLeaveEvent =
            RoutedEvent.Register<InputElement, PointerEventArgs>("PointerLeave", RoutingStrategies.Direct);

        /// <summary>
        /// Defines the <see cref="PointerMoved"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerEventArgs> PointerMovedEvent =
            RoutedEvent.Register<InputElement, PointerEventArgs>(
                "PointerMove",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="PointerPressed"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerPressEventArgs> PointerPressedEvent =
            RoutedEvent.Register<InputElement, PointerPressEventArgs>(
                "PointerPressed",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="PointerReleased"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerEventArgs> PointerReleasedEvent =
            RoutedEvent.Register<InputElement, PointerEventArgs>(
                "PointerReleased",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="PointerWheelChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<PointerWheelEventArgs> PointerWheelChangedEvent =
            RoutedEvent.Register<InputElement, PointerWheelEventArgs>(
                "PointerWheelChanged",
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        /// <summary>
        /// Initializes static members of the <see cref="InputElement"/> class.
        /// </summary>
        static InputElement()
        {
            IsEnabledProperty.Changed.Subscribe(IsEnabledChanged);

            GotFocusEvent.AddClassHandler<InputElement>(x => x.OnGotFocus);
            LostFocusEvent.AddClassHandler<InputElement>(x => x.OnLostFocus);
            KeyDownEvent.AddClassHandler<InputElement>(x => x.OnKeyDown);
            KeyUpEvent.AddClassHandler<InputElement>(x => x.OnKeyUp);
            TextInputEvent.AddClassHandler<InputElement>(x => x.OnTextInput);
            PointerEnterEvent.AddClassHandler<InputElement>(x => x.OnPointerEnter);
            PointerLeaveEvent.AddClassHandler<InputElement>(x => x.OnPointerLeave);
            PointerMovedEvent.AddClassHandler<InputElement>(x => x.OnPointerMoved);
            PointerPressedEvent.AddClassHandler<InputElement>(x => x.OnPointerPressed);
            PointerReleasedEvent.AddClassHandler<InputElement>(x => x.OnPointerReleased);
            PointerWheelChangedEvent.AddClassHandler<InputElement>(x => x.OnPointerWheelChanged);
        }

        /// <summary>
        /// Occurs when the control receives focus.
        /// </summary>
        public event EventHandler<RoutedEventArgs> GotFocus
        {
            add { AddHandler(GotFocusEvent, value); }
            remove { RemoveHandler(GotFocusEvent, value); }
        }

        /// <summary>
        /// Occurs when the control loses focus.
        /// </summary>
        public event EventHandler<RoutedEventArgs> LostFocus
        {
            add { AddHandler(LostFocusEvent, value); }
            remove { RemoveHandler(LostFocusEvent, value); }
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown
        {
            add { AddHandler(KeyDownEvent, value); }
            remove { RemoveHandler(KeyDownEvent, value); }
        }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp
        {
            add { AddHandler(KeyUpEvent, value); }
            remove { RemoveHandler(KeyUpEvent, value); }
        }

        /// <summary>
        /// Occurs when a user typed some text while the control has focus.
        /// </summary>
        public event EventHandler<TextInputEventArgs> TextInput
        {
            add { AddHandler(TextInputEvent, value); }
            remove { RemoveHandler(TextInputEvent, value); }
        }

        /// <summary>
        /// Occurs when the pointer enters the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerEnter
        {
            add { AddHandler(PointerEnterEvent, value); }
            remove { RemoveHandler(PointerEnterEvent, value); }
        }

        /// <summary>
        /// Occurs when the pointer leaves the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerLeave
        {
            add { AddHandler(PointerLeaveEvent, value); }
            remove { RemoveHandler(PointerLeaveEvent, value); }
        }

        /// <summary>
        /// Occurs when the pointer moves over the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerMoved
        {
            add { AddHandler(PointerMovedEvent, value); }
            remove { RemoveHandler(PointerMovedEvent, value); }
        }

        /// <summary>
        /// Occurs when the pointer is pressed over the control.
        /// </summary>
        public event EventHandler<PointerPressEventArgs> PointerPressed
        {
            add { AddHandler(PointerPressedEvent, value); }
            remove { RemoveHandler(PointerPressedEvent, value); }
        }

        /// <summary>
        /// Occurs when the pointer is released over the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerReleased
        {
            add { AddHandler(PointerReleasedEvent, value); }
            remove { RemoveHandler(PointerReleasedEvent, value); }
        }

        /// <summary>
        /// Occurs when the mouse wheen is scrolled over the control.
        /// </summary>
        public event EventHandler<PointerWheelEventArgs> PointerWheelChanged
        {
            add { AddHandler(PointerWheelChangedEvent, value); }
            remove { RemoveHandler(PointerWheelChangedEvent, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control can receive focus.
        /// </summary>
        public bool Focusable
        {
            get { return GetValue(FocusableProperty); }
            set { SetValue(FocusableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is enabled for user interaction.
        /// </summary>
        public bool IsEnabled
        {
            get { return GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets associated mouse cursor.
        /// </summary>
        public Cursor Cursor
        {
            get { return GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is focused.
        /// </summary>
        public bool IsFocused
        {
            get { return GetValue(IsFocusedProperty); }
            private set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is considered for hit testing.
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pointer is currently over the control.
        /// </summary>
        public bool IsPointerOver
        {
            get { return GetValue(IsPointerOverProperty); }
            internal set { SetValue(IsPointerOverProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the control is effectively enabled for user interaction.
        /// </summary>
        /// <remarks>
        /// The <see cref="IsEnabled"/> property is used to toggle the enabled state for individual
        /// controls. The <see cref="IsEnabledCore"/> property takes into account the
        /// <see cref="IsEnabled"/> value of this control and its parent controls.
        /// </remarks>
        bool IInputElement.IsEnabledCore => IsEnabledCore;

        /// <summary>
        /// Gets a value indicating whether the control is effectively enabled for user interaction.
        /// </summary>
        /// <remarks>
        /// The <see cref="IsEnabled"/> property is used to toggle the enabled state for individual
        /// controls. The <see cref="IsEnabledCore"/> property takes into account the
        /// <see cref="IsEnabled"/> value of this control and its parent controls.
        /// </remarks>
        protected bool IsEnabledCore
        {
            get { return GetValue(IsEnabledCoreProperty); }
            set { SetValue(IsEnabledCoreProperty, value); }
        }

        /// <summary>
        /// Returns the input element that can be found within the current control at the specified
        /// position.
        /// </summary>
        /// <param name="p">The position, in control coordinates.</param>
        /// <returns>The <see cref="IInputElement"/> at the specified position.</returns>
        public IInputElement InputHitTest(Point p)
        {
            return this.GetInputElementsAt(p).FirstOrDefault();
        }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        public void Focus()
        {
            FocusManager.Instance.Focus(this);
        }

        /// <inheritdoc/>
        protected override void OnDetachedFromVisualTree(IRenderRoot oldRoot)
        {
            base.OnDetachedFromVisualTree(oldRoot);

            if (IsFocused)
            {
                FocusManager.Instance.Focus(null);
            }
        }

        /// <inheritdoc/>
        protected override void OnAttachedToVisualTree(IRenderRoot root)
        {
            base.OnAttachedToVisualTree(root);
            UpdateIsEnabledCore();
        }

        /// <summary>
        /// Called before the <see cref="GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnGotFocus(GotFocusEventArgs e)
        {
            IsFocused = e.Source == this;
        }

        /// <summary>
        /// Called before the <see cref="LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnLostFocus(RoutedEventArgs e)
        {
            IsFocused = false;
        }

        /// <summary>
        /// Called before the <see cref="KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="KeyUp"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnTextInput(TextInputEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="PointerEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerEnter(PointerEventArgs e)
        {
            IsPointerOver = true;
        }

        /// <summary>
        /// Called before the <see cref="PointerLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerLeave(PointerEventArgs e)
        {
            IsPointerOver = false;
        }

        /// <summary>
        /// Called before the <see cref="PointerMoved"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerMoved(PointerEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="PointerPressed"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerPressed(PointerPressEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="PointerReleased"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerReleased(PointerEventArgs e)
        {
        }

        /// <summary>
        /// Called before the <see cref="PointerWheelChanged"/> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
        }

        private static void IsEnabledChanged(PerspexPropertyChangedEventArgs e)
        {
            ((InputElement)e.Sender).UpdateIsEnabledCore();
        }

        /// <summary>
        /// Updates the <see cref="IsEnabledCore"/> property value.
        /// </summary>
        private void UpdateIsEnabledCore()
        {
            UpdateIsEnabledCore(this.GetVisualParent<InputElement>());
        }

        /// <summary>
        /// Updates the <see cref="IsEnabledCore"/> property based on the parent's
        /// <see cref="IsEnabledCore"/>.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        private void UpdateIsEnabledCore(InputElement parent)
        {
            if (parent != null)
            {
                IsEnabledCore = IsEnabled && parent.IsEnabledCore;
            }
            else
            {
                IsEnabledCore = IsEnabled;
            }

            foreach (var child in this.GetVisualChildren().OfType<InputElement>())
            {
                child.UpdateIsEnabledCore(this);
            }
        }
    }
}
