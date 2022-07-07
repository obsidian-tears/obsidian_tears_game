// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Add this interface to objects that are included in proxy serialization. Quests aren't
    /// directly serializable because they contain references to subassets. To serialize and
    /// deserialize quests in JSON format, their data must be copied to/from a serializable
    /// proxy object. The proxy object and its subobjects receive the messages in this 
    /// interface.
    /// </summary>
    public interface IProxySerializationCallbackReceiver
    {

        /// <summary>
        /// Invoked before the object is serialized. Implementations may want to
        /// copy non-serializable data into a temporary, serializable variable. For example,
        /// a ScriptableObject with subassets may want to serialize those subassets into
        /// a string representation in JSON format.
        /// </summary>
        void OnBeforeProxySerialization();

        /// <summary>
        /// Invoked after the object has been deserialized. Implementations may
        /// want to reconstruct non-serializable data from temporary, serialized variables.
        /// For example, a ScriptableObject with subassets may want to recreate those
        /// subassets by reconstituting them from a previously-saved JSON representation.
        /// </summary>
        void OnAfterProxyDeserialization();

    }
}
