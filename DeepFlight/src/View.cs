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
        private LinkedList<View> children = new LinkedList<View>();

        public View(Camera camera) {
            Camera = camera;
        }

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
                    foreach(var child in children){
                        child.Focused = false;
                    }
                    OnUnfocus();
                }
                focused = value;
            }
        }

        protected virtual void OnFocus() { }
        protected virtual void OnUnfocus() { }
        

        
        public void AddChild(View child) {
            if (children.Contains(child) )
                throw new ArgumentException("View is already child of this parent.");

            if (child.Parent != null)
                throw new ArgumentException("View is already a child of a parent");

            children.AddLast(child);
            child.Parent = this;
        }

        public void RemoveChild(View child) {
            if (!children.Contains(child))
                throw new ArgumentException("View is not child of parent!");

            children.Remove(child);
            child.Parent = null;
        }

        public void Initialize(ApplicationController application) {
            application.DrawEvent += Draw;
            application.UpdateEvent += Update;
            OnInitialize();
            foreach (var child in children)
                child.Initialize(application);
        }
        protected virtual void OnInitialize() { }


        public void Terminate(ApplicationController application) {
            application.DrawEvent -= Draw;
            application.UpdateEvent -= Update;
            OnTerminate();
            foreach (var child in children)
                child.Terminate(application);
        }
        protected virtual void OnTerminate() { }


        public void Draw(Renderer renderer) {
            OnDraw(renderer);
            foreach (var child in children)
                child.Draw(renderer);
        }
        protected virtual void OnDraw(Renderer renderer) { }


        public void Update(double deltaTime) {
            OnUpdate(deltaTime);
            foreach (var child in children)
                child.Update(deltaTime);
        }
        protected virtual void OnUpdate(double deltaTime) { }
        

        public void KeyInput(KeyEventArgs e) {
            if (!Focused || e.Handled) return;

            // Focused children should handle the event first
            foreach(var child in children) {
                child.KeyInput(e);
            }

            // Check if child handled the event
            if (!Focused || e.Handled) return;
            e.Handled = OnKeyInput(e);
        }
        protected virtual bool OnKeyInput(KeyEventArgs e) { return false; }


        public void CharInput(CharEventArgs e) {
            if (!Focused || e.Handled) return;

            // Focused children should handle the event first
            foreach(var child in children) {
                child.CharInput(e);
            }

            if (!Focused || e.Handled) return;
            e.Handled = OnCharInput(e);
        }
        protected virtual bool OnCharInput(CharEventArgs e) { return false;  }
        

        
    }
}
