// File: MyPlugin.jslib

mergeInto(LibraryManager.library, {
  GameOver: function () {
    window.dispatchReactUnityEvent(
      "GameOver"
    );
  },
//  GiveGold: function(goldAmount) {
//      window.dispatchReactUnityEvent(
//          "GiveGold",
//          goldAmount
//      );
//  },
});
