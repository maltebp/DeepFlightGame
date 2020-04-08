using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework.Input;
using System.Linq;


namespace DeepFlight.gui {

    public class MenuView : View {

        // Number of options 
        public int Size {
            get => Children.Count;
        }
        
        // The index of the current selected option
        private int currentIndex = 0;
        public int CurrentIndex {
            get => currentIndex;
            set {
                currentIndex = MathExtension.Mod(value, Size);
                foreach(var child in Children) {
                    child.Focused = false;
                }
                Children.ElementAt(currentIndex).Focused = true;
            }
        }

        public MenuView() : base(null){ }
        
        // Update focus when adding first child
        protected override void OnChildAdded() {
            if( Size == 1) {
                Children.ElementAt(0).Focused = true;
            }
        }

        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if( e.Key == Keys.Up) {
                    CurrentIndex--;
                    return true;
                }
                if( e.Key == Keys.Down || e.Key == Keys.Tab) {
                    CurrentIndex++;
                    return true;
                }
            }
            return false;
        }
    }
}
