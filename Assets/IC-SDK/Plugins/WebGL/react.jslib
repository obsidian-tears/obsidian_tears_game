var WebGLFunctions = {
  GetNFT: function () {
    try {
      window.dispatchReactUnityEvent("GetNFT");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  LoginIc: function () {
    try {
      window.dispatchReactUnityEvent("LoginIc");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
  SetUserName: function (json) {
    try {
      window.dispatchReactUnityEvent("SetUserName", UTF8ToString(json) );
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  }
};
  
mergeInto(LibraryManager.library, WebGLFunctions);