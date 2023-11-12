using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class Binder{
    public class Bind<T>{

        public Transform t;
        
        public T c;

        public Bind<T> mount(Transform root){
            t = root;

            // var mount = c.GetType().GetMethod("method");
            // if(mount != null){
            //     mount.Invoke(f.GetValue(c), new object[]{root.Find(f.Name)});
            // }

            // if(c.GetType().GetMethod("method"))

            // Debug.Log(root);
            // Debug.Log(c.GetType());
            // c.GetType().GetProperties

            foreach(var f in c.GetType().GetProperties()){
                // Debug.Log(f.Name);
                var mount = f.PropertyType.GetMethod("mount");
                if(mount != null){
                    // Debug.Log($"find {f.Name}");
                    mount.Invoke(f.GetValue(c), new object[]{root.Find(f.Name)});
                }
                else{
                    var comp = f.PropertyType.GetField("comp");
                    if(comp != null){
                        var holder = f.GetValue(c);
                        holder.GetType().GetMethod("mountC").Invoke(holder, new object[]{root});
                    }
                    else{
                        if(f.PropertyType == typeof(EmptyBind)){
                            var empty = f.GetValue(c);
                            empty.GetType().GetField("t").SetValue(empty, root.Find(f.Name));
                        }
                    }
                }
            }
            return this;
        }
    }

    public static Bind<T> b<T>(T template){
        var b = new Bind<T>(){
            c = template,
        };

        return b;
    }

    public class EmptyBind{
        public Transform t;
    }

    public static EmptyBind e{
        get{
            return new EmptyBind();
        }
    }
    

    public class ComponentHolder<T> where T: Component{
        public T comp;

        public ComponentHolder<T> mountC(Transform root){
            comp = root.GetComponent<T>();
            return this;
        }
    }

    public static ComponentHolder<T> c<T>() where T: Component{
        return new ComponentHolder<T>();
    }
}

public class baseBind{
    public Transform transform;

    protected void recursiveMount(Transform self){
        transform = self;

        foreach(var field in GetType().GetFields(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)){
            Debug.Log(field.Name);
            if(field.FieldType.IsSubclassOf(typeof(baseBind))){
                var constructor = field.FieldType.GetConstructor(new Type[0]);
                var instance = constructor.Invoke(new object[0]) as baseBind;
                instance.recursiveMount(self.Find(field.Name));
                field.SetValue(this, instance);
            }
            else if(field.FieldType.IsSubclassOf(typeof(Component))){
                field.SetValue(this, self.GetComponent(field.FieldType));
            }
        }
    }
}

public class bindRoot: baseBind{
    public void mount(Transform root){
        recursiveMount(root);
    }
}

public class ClassBind<T>{
    public void mount(Transform root){
        find(root, typeof(T));
    }

    private void find(Transform parent, Type type){
        var child = parent.Find(type.Name);
        Debug.Log($"{type.Name} = {child}");
        foreach(var nestedType in type.GetNestedTypes()){
            if(nestedType.IsSubclassOf(typeof(Component))){
                var engineType = nestedType;
                while(engineType.Namespace != nameof(UnityEngine)){
                    engineType = engineType.BaseType;
                }
                var component = child.GetComponent(engineType);
                Debug.Log($"{nestedType.Name} = {component}");
            }
            else{
                find(child, nestedType);
            }
        }
    }
}

public class Tbind<T>{
    public T children;
    public Transform transform;
}

// interface B{
//     interface B0{
//         class meshRenderer: MeshRenderer{}

//     }
// }

// enum eA{
// }

// struct K{
//     public struct M{
        
//     }
    
//     int a;
// }

public class MySpecialAttribute : Attribute
{
}

namespace TestBind{
    namespace Cone{
        class meshRenderer: MeshRenderer{}
    }
    namespace Cube{
        class meshRenderer: MeshRenderer{}
    }
};

// #define 

public class binder : MonoBehaviour
{
    public class root{
        public class Cone{
            public class mr: MeshRenderer {}
        }
        public class Cube{
            public class mr: MeshRenderer {}
        }
    }
    
    // (string Alpha, string Beta) namedLetters;
    

    // (

    //     (
    //         MeshRenderer meshRenderer, 
    //         int a
    //     )Cone, 
    //     (
    //         MeshRenderer meshRenderer, 
    //         int a
    //     )Cube
    // ) bind2;

    public class bind: bindRoot{
        class CCone: baseBind{
            public MeshRenderer meshRenderer;

        };
        CCone Cone;

        public class CCube: baseBind{
            public MeshRenderer meshRenderer;

        };
        public CCube Cube;

    }
    public bind bindExample = new bind();

    public ClassBind<root> classBind = new ClassBind<root>();

    class THCreator{
        public class TH<T>{
            public Transform t;
            public T trait;
            
        }

        public static THCreator.TH<T> create<T>(T traitTemplate){
            return new THCreator.TH<T>();
        }

    }

    
    
    void Start()
    {
        // bindExample.mount(transform);

        classBind.mount(transform);

        // var a = typeof(TestBind.Cone.meshRenderer);

        // var k = nameof(TestBind.Cone);
        
        // // typeof(BB.CC)

        // // var k = B.B0;
        // var c = nameof(TestBind.Cone);

        // var b = nameof(a);

        // var jj = typeof(K.M);

        // (new K()).a


        

        var status = new{
            difficulty = THCreator.create(new{
                easy = null as Transform,
                normal = null as Transform,
                hard = null as Transform,
            }),
            stage = new{
                Text = new {
                    text = null as UnityEngine.UI.Text,
                }
            },
            timer = new{
                Text = new {
                    text = null as UnityEngine.UI.Text,
                }
            }
        };

        // status.difficulty.trait.
    }

    // Transform getTransform(string @namespace){
    //     // var j = typeof(T);

    // }

    // T getComponent<T>() where T: Component{

    // }

    
    void Update()
    {
        var item = TypeExtensions.CastByPrototype(new { ch = 'a', length = 0, additionam = 1}, new { ch = 'a', length = 0});
        //you can use item.ch and item.length here
        // Trace.WriteLine(item.ch);
        
    }

    public static class TypeExtensions
    {
        public static T CastByPrototype<T>(object obj, T prototype)
        {
            return (T)obj;
        }
    }
}
