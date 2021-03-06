﻿using DeepFlight.rendering;
using DeepFlight.utility.KeyboardController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DeepFlight {

    /// <summary>
    /// General base class for UI elements. The view is what is drawn and updated to the
    /// screen in each game loop.
    /// It may contain child views, which may also be drawn
    /// </summary>
    public abstract class View : Movable {

        public Camera Camera { get; set; }
        public View Parent { get; protected set; }
        public LinkedList<View> Children { get; } = new LinkedList<View>();


        /// <summary>
        /// The background Texture will be drawn before this View or any of this
        /// View's children are drawn.
        /// </summary>
        private TextureView background;
        public Color BackgroundColor {
            get {
                if (background == null) return Color.Transparent;
                return background.Color;
            }
            set {
                if (background == null)
                    background = new TextureView(
                        new Camera(layer: 1f), BackgroundTexture);
                background.Color = value;
            }
        }



        /// <summary>
        /// The background Texture will be drawn before this View or any of this
        /// View's children are drawn.
        /// </summary>
        public Texture2D BackgroundTexture {
            get {
                if (background == null) return Textures.SQUARE;
                return background.Texture;
            }
            set {
                if (background == null)
                    background = new TextureView(
                        new Camera(layer: 1f), value);
                background.Color = BackgroundColor;
                background.Texture = value;
            }
        }


        public bool Initialized { get; private set; }


        /* Determines the layering of children when drawn.
         * Lowest DepthOffset gets drawn in the front */
        private float depthOffset = 0.0f;
        public float DepthOffset {
            get => depthOffset;
            set {
                depthOffset = value;
                var childDepth = depthOffset;
                foreach( var child in Children) {
                    childDepth -= 0.01f;
                    child.DepthOffset = childDepth;
                }
            }
        }

        private bool focused = false;
        public bool Focused {
            get => focused;
            set {
                if( value == true && focused == false ) {
                    if( Parent != null )
                        Parent.Focused = true;
                    focused = value;
                    OnFocus();
                }
                if( value == false && focused == true ){
                    focused = value;
                    foreach (var child in Children){
                        child.Focused = false;
                    }
                    OnUnfocus();
                }
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



        public View(Camera camera = null) {
            Camera = camera;
        }



        public void AddChildren(params View[] children) {
            foreach (var child in children) AddChild(child);
        }



        /// <summary>
        /// Adds a child to the View. Children are drawn together with the View
        /// and is hidden if the parent is.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(View child) {
            if (Children.Contains(child) )
                throw new ArgumentException("View is already child of this parent.");

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            Children.AddLast(child);
            child.Parent = this;

            // Bad solution, but it updates offset of all children (including new one)
            DepthOffset = depthOffset;

            if (child.Focused) Focused = true;
            if (Hidden) child.Hidden = true;
            if(Initialized) child.Initialize();

            OnChildAdded(child);
        }
        protected virtual void OnChildAdded(View addedChild) { }



        public void RemoveChild(View child) {
            if (!Children.Contains(child))
                throw new ArgumentException($"'{child.GetType().Name}' is not child view of parent '{this.GetType().Name}'!");

            Children.Remove(child);
            child.Parent = null;
            OnChildRemoved(child);
        }
        protected virtual void OnChildRemoved(View removedChild) { }



        public void Initialize() {
            Application.DrawEvent += Draw;
            Application.UpdateEvent += Update;
            OnInitialize();
            foreach (var child in Children)
                child.Initialize();
            Initialized = true;
        }
        protected virtual void OnInitialize() { }



        /// <summary>
        /// Terminates this View, removing it as an action to the Draw and Update
        /// events. 
        /// </summary>
        public void Terminate() {
            Application.DrawEvent -= Draw;
            Application.UpdateEvent -= Update;
            OnTerminate();
            foreach (var child in Children)
                child.Terminate();
        }
        protected virtual void OnTerminate() { }



        public void Draw(Renderer renderer) {
            if (!Hidden) {

                // Render the background
                if (Camera != null && background != null) {
                    background.X = GetCenterX();
                    background.Y = GetCenterY()-1;
                    background.Width = Width;
                    background.Height = Height;
                    renderer.DrawTexture(Camera, background);
                }
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
        


        /// <summary>
        /// Signals to to the View that a key has been input, and it now has a chance to handle
        /// it. However, first, it'll notify its child views.
        /// </summary>
        /// <param name="e"></param>
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
