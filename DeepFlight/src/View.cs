using DeepFlight.utility.KeyboardController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DeepFlight {
    public abstract class View : Movable {

        public Camera Camera { get; set; }
        public View Parent { get; protected set; }
        public LinkedList<View> Children { get; } = new LinkedList<View>();

        public bool Initialized {get; private set;}


        private bool focused = false;
        public bool Focused {
            get => focused;
            set {
                if( value == true && focused == false ) {
                    if( Parent != null )
                        Parent.Focused = true;
                    OnFocus();
                }
                if( value == false && focused == true ){
                    foreach(var child in Children){
                        child.Focused = false;
                    }
                    OnUnfocus();
                }
                focused = value;
            }
        }
        protected virtual void OnFocus() { }
        protected virtual void OnUnfocus() { }


        /// <summary>
        /// Hiding the view prevents it (and its children), from
        /// being drawn, updated and handle input events.
        /// </summary>
        private bool hidden = false;
        public bool Hidden {
            get => hidden;
            set {
                if (value == false && hidden == true) {
                    hidden = false;
                    if (Parent != null && Parent.Hidden)
                        Parent.Hidden = false;
                    foreach (var child in Children)
                        child.Hidden = false;
                }
                if (value == true && hidden == false) {
                    hidden = true;
                    foreach (var child in Children) {
                        child.Hidden = true;
                    }
                }
            }
        }


        public View(Camera camera) {
            Camera = camera;
        }


        public void AddChildren(params View[] children) {
            foreach (var child in children) AddChild(child);
        }

        public void AddChild(View child) {
            if (Children.Contains(child) )
                throw new ArgumentException("View is already child of this parent.");

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            Children.AddLast(child);
            child.Parent = this;
            if (child.Focused) Focused = true;
            if (Hidden) child.Hidden = true;
            if(Initialized) child.Initialize();

            OnChildAdded();
        }
        protected virtual void OnChildAdded() { }

        public void RemoveChild(View child) {
            if (!Children.Contains(child))
                throw new ArgumentException("View is not child of parent!");

            Children.Remove(child);
            child.Parent = null;
        }

        public void Initialize() {
            ApplicationController.DrawEvent += Draw;
            ApplicationController.UpdateEvent += Update;
            OnInitialize();
            foreach (var child in Children)
                child.Initialize();
            Initialized = true;
        }
        protected virtual void OnInitialize() { }


        public void Terminate() {
            ApplicationController.DrawEvent -= Draw;
            ApplicationController.UpdateEvent -= Update;
            OnTerminate();
            foreach (var child in Children)
                child.Terminate();
        }
        protected virtual void OnTerminate() { }


        public void Draw(Renderer renderer) {
            if (!Hidden) {
                OnDraw(renderer);
            }
        }
        protected virtual void OnDraw(Renderer renderer) { }


        public void Update(double deltaTime) {
            if (!Hidden) {
                OnUpdate(deltaTime);
            }
        }
        protected virtual void OnUpdate(double deltaTime) { }
        

        public void KeyInput(KeyEventArgs e) {
            if (!Focused || Hidden || e.Handled) return;

            // Focused children should handle the event first
            foreach(var child in Children) {
                child.KeyInput(e);
            }

            // Check if child handled the event
            if (!Focused || Hidden || e.Handled) return;
            e.Handled = OnKeyInput(e);
        }
        protected virtual bool OnKeyInput(KeyEventArgs e) { return false; }

        
        public void CharInput(CharEventArgs e) {
            if (!Focused || Hidden || e.Handled) return;


            // Focused children should handle the event first
            foreach (var child in Children) {
                child.CharInput(e);
            }

            if (!Focused || Hidden || e.Handled) return;
            e.Handled = OnCharInput(e);
        }
        protected virtual bool OnCharInput(CharEventArgs e) { return false;  }
        

        
    }
}
