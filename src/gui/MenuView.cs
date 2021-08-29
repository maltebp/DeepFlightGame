using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

z
namespace DeepFlight.gui {

    /// <summary>
    /// Simple controller to handle a series of Views as a traversable
    /// menu.
    /// The menu can be traversed using up and down keys, and when an option
    /// (View) is chosen, the View is Focused.
    /// </summary>
    public class MenuView : View {

        // Number of options 
        public int OptionCount {
            get => options.Count;
        }
        
        // The index of the current selected option
        private int focusedOptionIndex = 0;
        public int FocusedOptionIndex {
            get => focusedOptionIndex;
            set {
                if (OptionCount == 0)
                    focusedOptionIndex = -1;
                else {
                    focusedOptionIndex = MathExtension.Mod(value, OptionCount);
                    foreach (var child in Children) {
                        child.Focused = false;
                    }
                    options.ElementAt(focusedOptionIndex).Focused = true;
                }
            }
        }

        public LinkedList<View> options = new LinkedList<View>();

        public View FocusedOption { get => options.ElementAt(FocusedOptionIndex); }

        public Keys SelectKey { get; set; }

        public MenuOrientation Orientation { get; set; }

        public delegate void OptionSelectCallback();

        public Dictionary<View, OptionSelectCallback> optionCallbacks = new Dictionary<View, OptionSelectCallback>(); 



        public MenuView(MenuOrientation orientation = MenuOrientation.VERTICAL, Keys selectKey = Keys.Enter) {
            Orientation = orientation;
            SelectKey = selectKey;
        }


        /// <summary>
        /// Adds a View as an option to the Menu along with a callback, which
        /// will fire if the option is chosen. The View is also added as a Child
        /// of the menu.
        /// To remove the View, use the 'OnChildRemoved(..)' method
        /// </summary>
        public void AddMenuOption(View optionView, OptionSelectCallback optionCallback) {
            AddChild(optionView);
            options.AddLast(optionView);
            optionCallbacks.Add(optionView, optionCallback);
            if (Focused && OptionCount == 1)
                FocusedOptionIndex = 0;
        }

        protected override void OnChildRemoved(View removedChild) {
            if( options.Contains(removedChild)) {
                options.Remove(removedChild);
                optionCallbacks.Remove(removedChild);
                // If index was set to last, element it's now
                // out of bounds, so we update it
                FocusedOptionIndex = focusedOptionIndex;
            }
        }

        protected override void OnFocus() {
            FocusedOptionIndex = 0;
        }

        protected override bool OnKeyInput(KeyEventArgs e) {
            if( e.Action == KeyAction.PRESSED) {
                if(  e.Key == SelectKey ) {
                    optionCallbacks[FocusedOption]();
                    return true;
                }

                // Tab is a general key to move to next option
                if (e.Key == Keys.Tab) {
                    FocusedOptionIndex++;
                    return true;
                }

                if (Orientation == MenuOrientation.VERTICAL &&  e.Key == Keys.Up) {
                    FocusedOptionIndex--;
                    return true;
                }
                
                if(Orientation == MenuOrientation.VERTICAL &&  e.Key == Keys.Down) {
                    FocusedOptionIndex++;
                    return true;
                }

                if (Orientation == MenuOrientation.HORIZONTAL && e.Key == Keys.Left) {
                    FocusedOptionIndex--;
                    return true;
                }

                if (Orientation == MenuOrientation.HORIZONTAL && e.Key == Keys.Right) {
                    FocusedOptionIndex++;
                    return true;
                }


            }
            return false;
        }

        
        public enum MenuOrientation {
            HORIZONTAL,
            VERTICAL
        }
    }
}
