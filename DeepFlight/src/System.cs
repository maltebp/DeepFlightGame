


//using System;
//using System.Collections.Generic;


//abstract class EntitySystem {

//    private LinkedList<Entity> entities = new LinkedList<Entity>();

//    protected void AddEntity(Entity entity) { 
//        entities.AddLast(entity);
//    }

//    public void RemoveEntity(Entity entity) {
//        entities.Remove(entity);
//    }

//    public void Update() {
//        foreach(Entity entity in entities) {
//            if (entity.IsDestroyed()) {
//                entities.Remove(entity);
//            } else {
//                OnUpdate( entity );
//            }
//        }
//    }

//    public abstract void OnUpdate(Entity entity);
    
//}
