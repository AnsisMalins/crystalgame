using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;

namespace Utilities
{
    public static class View
    {
        public static FrameworkElement Create(ViewModel viewModel)
        {
            var attr = Attribute.GetCustomAttribute(
                viewModel.GetType(), typeof(DefaultViewAttribute)) as DefaultViewAttribute;
            if (attr == null) return null;
            var view = Activator.CreateInstance(attr.ViewType) as FrameworkElement;
            if (view == null) return null;

            view.ClearValue(FrameworkElement.HeightProperty);
            view.ClearValue(FrameworkElement.WidthProperty);

            Type viewModelType = viewModel.GetType();

            foreach (PropertyInfo property in viewModelType.GetProperties())
            {
                var control = view.FindName(property.Name) as FrameworkElement;
                if (control == null) continue;

                DependencyProperty viewProperty;
                if (control is ContentControl) viewProperty = ContentControl.ContentProperty;
                else if (control is ItemsControl) viewProperty = ItemsControl.ItemsSourceProperty;
                else if (control is RangeBase) viewProperty = RangeBase.ValueProperty;
                else continue;

                if (control.GetBindingExpression(viewProperty) != null) continue;

                var binding = new Binding(property.Name);
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                bool canRead = property.GetGetMethod() != null;
                bool canWrite = property.GetSetMethod() != null;
                if (canRead && canWrite) binding.Mode = BindingMode.TwoWay;
                else if (canRead) binding.Mode = BindingMode.OneWay;
                else if (canWrite) binding.Mode = BindingMode.OneWayToSource;
                else continue;

                if (viewProperty == UIElement.VisibilityProperty)
                    binding.Converter = ValueConverters.BooleanToVisibility;

                BindingOperations.SetBinding(control, viewProperty, binding);
            }

            foreach (MethodInfo method in viewModelType.GetMethods())
            {
                var control = view.FindName(method.Name) as DependencyObject;
                if (control == null) continue;

                DependencyProperty viewProperty;
                if (control is ButtonBase) viewProperty = ButtonBase.CommandProperty;
                else if (control is Hyperlink) viewProperty = Hyperlink.CommandProperty;
                else if (control is MenuBase) viewProperty = MenuItem.CommandProperty;
                else if (control is MenuItem) viewProperty = MenuItem.CommandProperty;
                else continue;

                PropertyInfo canExecute = viewModelType.GetProperty("Can" + method.Name);

                var command = new ReflectiveCommand(viewModel, method, canExecute);

                if (control is MenuBase)
                {
                    var menu = control as MenuBase;
                    menu.ItemContainerStyle = new Style(typeof(MenuItem), menu.ItemContainerStyle);
                    menu.ItemContainerStyle.Setters.Add(new Setter(viewProperty, command));
                    menu.ItemContainerStyle.Setters.Add(
                        new Setter(MenuItem.CommandParameterProperty, new Binding()));
                }
                else control.SetValue(viewProperty, command);

                if (canExecute != null)
                    BindingOperations.SetBinding(control, UIElement.IsEnabledProperty,
                        new Binding("CanExecute") { Source = command });
            }

            view.DataContext = viewModel;

            return view;
        }

        public static Type GetMockData(FrameworkElement obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            return obj.GetValue(MockDataProperty) as Type;
        }

        public static IList GetSelectedItems(ListBox obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            return obj.GetValue(SelectedItemsProperty) as IList;
        }

        public static ViewModel GetViewModel(ContentControl obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            return obj.GetValue(ViewModelProperty) as ViewModel;
        }

        public static void SetMockData(FrameworkElement obj, Type value)
        {
            Guard.ArgumentNotNull(obj, "obj");
            obj.SetValue(MockDataProperty, value);
        }

        public static void SetSelectedItems(ListBox obj, IList value)
        {
            Guard.ArgumentNotNull(obj, "obj");
            obj.SetValue(SelectedItemsProperty, value);
        }

        public static void SetViewModel(ContentControl obj, ViewModel value)
        {
            Guard.ArgumentNotNull(obj, "obj");
            obj.SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty MockDataProperty
            = DependencyProperty.RegisterAttached(
            "MockData", typeof(Type), typeof(View), new PropertyMetadata(MockDataChanged));

        public static readonly DependencyProperty SelectedItemsProperty
            = DependencyProperty.RegisterAttached(
            "SelectedItems", typeof(IList), typeof(View),
            new PropertyMetadata(SelectedItemsChanged));

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.RegisterAttached(
            "ViewModel", typeof(ViewModel), typeof(View), new PropertyMetadata(ViewModelChanged));

        private static void MockDataChanged(
            object sender, DependencyPropertyChangedEventArgs e)
        {
            Guard.ArgumentSubclassOf(sender, "sender", typeof(FrameworkElement));
            if (!Util.IsInDesignMode) return;
            (sender as FrameworkElement).DataContext = e.NewValue != null
                ? Activator.CreateInstance(e.NewValue as Type) : null;
        }

        private static void SelectedItemsChanged(
            object sender, DependencyPropertyChangedEventArgs e)
        {
            Guard.ArgumentSubclassOf(sender, "sender", typeof(ListBox));
            var listBox = sender as ListBox;
            var oldValue = e.OldValue as IList;
            var newValue = e.NewValue as IList;
            if (oldValue == null && newValue != null)
                listBox.SelectionChanged += SelectedItems_SelectionChanged;
            else if (oldValue != null && newValue == null)
                listBox.SelectionChanged -= SelectedItems_SelectionChanged;
            if (oldValue != null) oldValue.Clear();
            if (newValue != null)
            {
                newValue.Clear();
                foreach (object i in listBox.SelectedItems) newValue.Add(i);
            }
        }

        private static void SelectedItems_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            IList selectedItems = GetSelectedItems(sender as ListBox);
            foreach (object i in e.RemovedItems) selectedItems.Remove(i);
            foreach (object i in e.AddedItems) selectedItems.Add(i);
        }

        private static void ViewModelChanged(
            object sender, DependencyPropertyChangedEventArgs e)
        {
            Guard.ArgumentSubclassOf(sender, "sender", typeof(ContentControl));
            (sender as ContentControl).Content = Create(e.NewValue as ViewModel); 
        }
    }
}