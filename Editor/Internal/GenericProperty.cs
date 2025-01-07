using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace UI.Li.Editor.Internal
{
    [PublicAPI] public class GenericProperty : ScriptableObject
    {
        [SerializeReference] public object property;
        
        public SerializedObject SerializedObject => serializedObject ??= new(this);
        
        private SerializedObject serializedObject;
        
        [NotNull] private static ConcurrentQueue<GenericProperty> pooledProperties = new();

        public static GenericProperty Get() =>
            pooledProperties.TryDequeue(out var property) ? property : CreateInstance<GenericProperty>();
        
        public static void Return(GenericProperty property)
        {
            if (property == null) return;
            pooledProperties.Enqueue(property);
        }
    }
}