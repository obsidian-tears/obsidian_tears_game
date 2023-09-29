# Obsidian Tears Game

Responsibility: to hold and deploy Unity code. It exposes a Unity WebGL build (see folder WebServer) that is used by the "game" frontend repo.

### System Requirements
- Unity Hub + Editor
- [Dfinity DFX](https://internetcomputer.org/docs/current/developer-docs/setup/install/)

After install, you will need to:
- create an identity,
- create a wallet,
- fill it with a few ICPs,
- convert these ICPs to Cycles
To help on that, this Bootcamp's video should be helpfull:  https://www.youtube.com/watch?v=r5s7nD_XO0M

### How to build and deploy
After you install, on Unity Hub build to WebGL, store it in a folder called Desktop (this is important! as the files need to be Desktop.wasm, etc.) and then replace those with the WebServer/src/obsidian_unity_frontend/assets/unity folder ones.

After that, make sure you are into WebServer folder (```cd WebServer```) and run ```dfx deploy --network staging```.
