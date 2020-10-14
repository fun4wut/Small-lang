using System.Windows.Input;

namespace GUI
{
    public static class Diycmd
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Exit",
            "Exit",
            typeof(Diycmd),
            new InputGestureCollection
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );
        
        public static readonly RoutedUICommand Compile = new RoutedUICommand
        (
            "Compile",
            "Compile",
            typeof(Diycmd),
            new InputGestureCollection
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            }
        );
        
        public static readonly RoutedUICommand RunAll = new RoutedUICommand
        (
            "Run All",
            "Run All",
            typeof(Diycmd),
            new InputGestureCollection
            {
                new KeyGesture(Key.F5, ModifierKeys.Control)
            }
        );
        
        public static readonly RoutedUICommand RunByStep = new RoutedUICommand
        (
            "Run By Step",
            "Run By Step",
            typeof(Diycmd),
            new InputGestureCollection
            {
                new KeyGesture(Key.F5)
            }
        );
        
        public static readonly RoutedUICommand Reset = new RoutedUICommand
        (
            "Reset",
            "Reset",
            typeof(Diycmd),
            new InputGestureCollection
            {
                new KeyGesture(Key.L, ModifierKeys.Control)
            }
        );
    }
}