namespace WpfBase.Common
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    #region Interfaces

    public interface IEventArgsConverter
    {
        #region Methods

        object Convert(object value, object parameter);

        #endregion
    }

    #endregion

    public class EventToCommand : TriggerAction<DependencyObject>
    {
        #region Constants

        public const string EventArgsConverterParameterPropertyName = "EventArgsConverterParameter";

        #endregion

        #region Fields

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) =>
                {
                    var sender = s as EventToCommand;

                    if (sender?.AssociatedObject == null)
                    {
                        return;
                    }

                    sender.EnableDisableElement();
                }));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) => OnCommandChanged(s as EventToCommand, e)));

        public static readonly DependencyProperty EventArgsConverterParameterProperty = DependencyProperty.Register(
            EventArgsConverterParameterPropertyName,
            typeof(object),
            typeof(EventToCommand),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register(
            "MustToggleIsEnabled",
            typeof(bool),
            typeof(EventToCommand),
            new PropertyMetadata(
                false,
                (s, e) =>
                {
                    var sender = s as EventToCommand;

                    if (sender?.AssociatedObject == null)
                    {
                        return;
                    }

                    sender.EnableDisableElement();
                }));

        private object _commandParameterValue;

        private bool? _mustToggleValue;

        #endregion

        #region Properties

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }

            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        public object CommandParameterValue
        {
            get
            {
                return this._commandParameterValue ?? this.CommandParameter;
            }

            set
            {
                this._commandParameterValue = value;
                this.EnableDisableElement();
            }
        }

        public IEventArgsConverter EventArgsConverter { get; set; }

        public object EventArgsConverterParameter
        {
            get
            {
                return this.GetValue(EventArgsConverterParameterProperty);
            }
            set
            {
                this.SetValue(EventArgsConverterParameterProperty, value);
            }
        }

        public bool MustToggleIsEnabled
        {
            get
            {
                return (bool)this.GetValue(MustToggleIsEnabledProperty);
            }

            set
            {
                this.SetValue(MustToggleIsEnabledProperty, value);
            }
        }

        public bool MustToggleIsEnabledValue
        {
            get
            {
                return this._mustToggleValue ?? this.MustToggleIsEnabled;
            }

            set
            {
                this._mustToggleValue = value;
                this.EnableDisableElement();
            }
        }

        public bool PassEventArgsToCommand { get; set; }

        #endregion

        #region Methods

        private static void OnCommandChanged(EventToCommand element, DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }

            element.EnableDisableElement();
        }

        public void Invoke()
        {
            this.Invoke(null);
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedElementIsDisabled())
            {
                return;
            }

            var command = this.GetCommand();
            var commandParameter = this.CommandParameterValue;

            if (commandParameter == null && this.PassEventArgsToCommand)
            {
                commandParameter = this.EventArgsConverter == null
                    ? parameter
                    : this.EventArgsConverter.Convert(parameter, this.EventArgsConverterParameter);
            }

            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
        }

        private bool AssociatedElementIsDisabled()
        {
            var element = this.GetAssociatedObject();
            return this.AssociatedObject == null || (element != null && !element.IsEnabled);
        }

        private void EnableDisableElement()
        {
            var element = this.GetAssociatedObject();

            if (element == null)
            {
                return;
            }

            var command = this.GetCommand();

            if (this.MustToggleIsEnabledValue && command != null)
            {
                element.IsEnabled = command.CanExecute(this.CommandParameterValue);
            }
        }

        private FrameworkElement GetAssociatedObject()
        {
            return this.AssociatedObject as FrameworkElement;
        }

        private ICommand GetCommand()
        {
            return this.Command;
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }

        #endregion
    }
}
