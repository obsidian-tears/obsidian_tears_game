// File: MyPlugin.jslib

mergeInto(LibraryManager.library, {
  SaveGame: function(gameData, objectName) {
    window.dispatchReactUnityEvent(
      "SaveGame",
      Pointer_stringify(gameData),
      Pointer_stringify(objectName)
    );
  },
  LoadGame: function(objectName) {
      window.dispatchReactUnityEvent(
        "LoadGame",
        Pointer_stringify(objectName)
      );
  },
  OpenChest: function(chestId, objectName) {
      window.dispatchReactUnityEvent(
        "OpenChest",
        Pointer_stringify(chestId),
        Pointer_stringify(objectName)
      );
  },
  DefeatMonster: function(chestId, objectName) {
      window.dispatchReactUnityEvent(
        "DefeatMonster",
        Pointer_stringify(chestId),
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
