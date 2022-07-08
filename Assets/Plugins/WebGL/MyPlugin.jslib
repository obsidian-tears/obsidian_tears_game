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
  OpenChest: function(chestId) {
      window.dispatchReactUnityEvent(
          "OpenChest",
          Pointer_stringify(chestId)
          Pointer_stringify(objectName)
      );
  },
  DefeatMonster: function(chestId) {
      window.dispatchReactUnityEvent(
          "DefeatMonster",
          Pointer_stringify(chestId)
          Pointer_stringify(objectName)
      );
  },
  EquipItems: function(itemIds, objectName) {
      window.dispatchReactUnityEvent(
          "EquipItems",
          itemIds,
          Pointer_stringify(objectName)
      );
  },
  BuyItem: function(itemId, objectName) {
      window.dispatchReactUnityEvent(
          "BuyItem",
          itemId,
          Pointer_stringify(objectName)
      );
  },
});
