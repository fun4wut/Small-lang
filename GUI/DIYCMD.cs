using System.Windows.Input;

namespace GUI
{
    public static class DIYCMD
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Exit",
            "Exit",
            typeof(DIYCMD),
            new InputGestureCollection
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );
        
        public static readonly RoutedUICommand Compile = new RoutedUICommand
        (
            "Compile",
            "Compile",
            typeof(DIYCMD),
            new InputGestureCollection
            {
                new KeyGesture(Key.B, ModifierKeys.Control)
            }
        );
        
        public static readonly RoutedUICommand RunAll = new RoutedUICommand
        (
            "Run All",
            "Run All",
            typeof(DIYCMD),
            new InputGestureCollection
            {
                new KeyGesture(Key.F5, ModifierKeys.Control)
            }
        );
        
        public static readonly RoutedUICommand RunByStep = new RoutedUICommand
        (
            "Run By Step",
            "Run By Step",
            typeof(DIYCMD),
            new InputGestureCollection
            {
                new KeyGesture(Key.F5)
            }
        );
    }
}