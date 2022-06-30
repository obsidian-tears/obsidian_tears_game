/*
------------------------------
  Dialogue System for Unity  
        Menu Framework
        
  Copyright © Pixel Crushers
------------------------------


See Documentation.pdf for instructions.


RELEASE HISTORY:

2022-02-28:
- Return to main menu now uses scene transition (e.g., fade).
- Fixed typo in TitleMenu script.

2021-12-08
- Updated for compatibility with disabled domain reloading.
- Configured Options menu volume sliders to better [-20,+20] dB range.

2019-02-07
- MusicManager now automatically switches from title to gameplay music; also keeps
  title music playing when switching to/from Credits scene; added FadeOut() method.
- Fixed: ArgumentOutOfRangeException when resolution options differ from saved choice.

2019-01-14
- Added Never Sleep checkbox to TitleMenu.

2018-07-30
- Added ScrollToMe component to Options > Resolution dropdown for joystick/keyboard.

2018-07-20
- Minimum Unity version is now: 5.6.0.
- Changed: LoadGamePanel Details field changed. You may need to reassign your UI element.

2018-07-09
- Updated for Dialogue System for Unity 2.x.

2017-12-04
- Added: Delete Saved Games button in Save Helper.
- Improved: Warns if scene is missing an EventSystem.
- Fixed: Quick save & return to menu now updates title menu Continue/Start buttons.

2017-06-10
- Improved: Added option to keep showing mouse cursor if switching to keyboard mode.

2017-05-24
- Fixed: Singleton bug. Added Act As Singleton checkbox to Title Menu component.

2017-04-30
- Menu Framework no longer needs to be child of Dialogue Manager.
- Music Manager now plays title music automatically in title scene.
- Added MusicManager.Stop() method.

2017-01-09
- Added: Graphics settings to options menu: fullscreen, resolution, quality levels.
- Added: Credits scene.
- Added: More customization options for saved game summary info.
- Fixed: Bug fixes with persistent data components and loading scenes.

2016-09-29
- Fixed: LoadingSceneTo() sequencer command wasn't saving persistent data first.
- Fixed: When using loading scenes, wasn't telling persistent data components that level was
  about to be unloaded, causing IncrementOnDestroy etc. to fire when they shouldn't.
- Added: Option to show loading scene for a minimum duration.

2016-09-16
- CONTAINS BREAKING CHANGES! Import a clean copy of this version; don't overwrite old versions.
- No longer uses Unity's Game Jam Menu Template.
- Improved cursor, focus, and open/close animation handling.
- Added quicksave and quickload.

2016-06-19
- Improved: Refinements to joystick/mouse input switching.
- Improved: Refinements to fading when restarting and loading games.

2016-06-03
- Added: Optional animation when opening/closing Pause menu.
- Added: Optional CheckInputDevice to toggle autofocus (joystick) mode based on
  whether player uses joystick or mouse.
- Changed: Reverted Quests button by popular demand; once again closes Pause menu.

2016-05-28
- Changed: Quests button now opens quest menu on top of Pause menu without closing Pause.
- Added: Restart confirmation panel.
- Added: Option to use a loading scene.
- Added: Component to save games to disk files instead of PlayerPrefs.

2016-05-14:
- Added: Subtitles toggle in Options menu.
- Changed: In Pause menu, replaced volume sliders with Options button.
- Improved: Better autofocusing.
- Fixed: If UI is in non-start scene and MenuPanel is inactive, sets startOptions.inMainMenu=false.

2016-04-29: 
- Added: HideCursor method.
- Added: Save Helper > On Return To Menu event.
- Fixed: Return To Menu sets StartOptions.inMainMenu true.

2016-03-05:
- Fixed: Misc. small bugs.

2015-10-24:
- Initial release.

*/