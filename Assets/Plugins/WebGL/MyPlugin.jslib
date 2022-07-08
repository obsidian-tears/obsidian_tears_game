// File: MyPlugin.jslib

mergeInto(LibraryManager.library, {
  SaveGame: function(gameData) {
    window.dispatchReactUnityEvent(
      "SaveGame",
      Pointer_stringify(gameData)
    );
  },
  LoadGame: function() {
      window.dispatchReactUnityEvent(
          "LoadGame"
      );
  },
});
