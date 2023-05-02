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
        chestId,
        Pointer_stringify(objectName)
      );
  },
  DefeatMonster: function(monsterId, objectName) {
      window.dispatchReactUnityEvent(
        "DefeatMonster",
        monsterId,
        Pointer_stringify(objectName)
      );
  },
  NewGame: function(objectName) {
      window.dispatchReactUnityEvent(
        "NewGame",
        Pointer_stringify(objectName)
      );
  },
  EquipItems: function(itemIds, objectName) {
      window.dispatchReactUnityEvent(
        "EquipItems",
        Pointer_stringify(itemIds),
        Pointer_stringify(objectName)
      );
  },
  BuyItem: function(shopId, itemId, qty, objectName) {
      window.dispatchReactUnityEvent(
        "BuyItem",
        shopId,
        Pointer_stringify(itemId),
        qty,
        Pointer_stringify(objectName)
      );
  },
});
