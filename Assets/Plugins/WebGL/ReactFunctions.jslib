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
  SetUserAlias: function(wallet){
    try{
      window.dispatchReactUnityEvent("SetUserAlias",UTF8ToString(wallet));
    }catch(e){
      console.warn("Falied to dispach setUserAlias"+e);
    }
  }

};

mergeInto(LibraryManager.library, WebGLFunctions);