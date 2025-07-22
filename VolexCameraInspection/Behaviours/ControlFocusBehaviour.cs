using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.Behaviours
{
    public class FocusBehaviour { }
    public static class ControlFocusBehaviour
    {
        public static readonly StyledProperty<bool> IsFocusedProperty = AvaloniaProperty.RegisterAttached<FocusBehaviour,Control, bool>("IsFocused", inherits: false, defaultValue: false,
                defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static bool GetIsFocused(Control element) => element.GetValue(IsFocusedProperty);
        public static void SetIsFocused(Control element,bool value)=>element.SetValue(IsFocusedProperty, value);
        
        static ControlFocusBehaviour()
        {
            IsFocusedProperty.Changed.Subscribe((args) =>
            {
                if (args.Sender is Control control && args.NewValue.Value)
                    control.Focus();
            });
        }
    }
}
