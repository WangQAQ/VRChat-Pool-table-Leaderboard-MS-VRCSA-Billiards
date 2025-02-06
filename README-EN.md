|[中文](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards)| |[EN](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-EN.md)| |[Русский](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-RU.md)| |[日本語](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-JP.md)| |[Español](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-ES.md)|

> Use AI Translation
> 
# VRC Billiards - Leaderboard - Snooker
## Project Inheritance Table
* #### MS-VRCSA-Billiards
  * #### VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards [Landmark Update](https://github.com/WangQAQ/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards)
  	  * ### VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards (Current)
### The purpose of this table is to provide an entertaining experience, allowing small plugins or fun features.
#### [Copyright Table](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/Copyright.md)

> You can play on this table here: [Map](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info)
---
## 1. New Features
* ### Upload Panel:
	* #### No more need to bind tasks in the Start function, current bindings are completed at EditTime, with no additional performance cost.
	* #### Simplified upload method, you no longer need to worry about bindings, just click in sequence on the panel to complete the bindings.
	* #### Improved leaderboard key management system. We noticed that when using automated leaderboards, keys often get lost due to certain actions. Now, we have created a new user system to bind your keys.
* ### User System:
	* #### You can register your account in [Account](https://www.wangqaq.com/PoolBar/Account).
	* #### You can now choose a colorful name on the WEB once registered.
	* #### Although it is possible to upload keys anonymously, to avoid losing keys and not being able to use the leaderboard in the future, we recommend registering an account (experience will accumulate in your account).
	* #### You can now choose your landmark on the WEB.
* ### New Optimizations:
	* #### I have fixed the GC problem of the leaderboard. Now, there will no longer be thousands of GC calls.
	* #### I have optimized the shader for the landmark, preventing lag.
	* #### I have updated the colorful name system. Now, colorful names are cached once fully created on the WEB, so there will be no repeated loops in Udon.
  	* #### The new panel for binding scripts no longer takes up game loading time in the Start function.
* ### New Benefits:
	* #### Colorful names are now unlimited. You can register your account, bind your player, and choose your colorful name.
	* #### Level System (Level 6 - 2 segment colorful names, Level 12 - 3 segment colorful names, Level 24 - 4 segment colorful names).
* ### New WEB:
	* #### Added personal panel for setting your landmark and colorful name.
	* #### Added personal info panel to view your upload count and history.
	* #### Added leaderboard record function, now you can track your win rate.
	* #### A simple ranking system based on win rate, allowing players to focus on more than just scores.
	* #### More languages for the WEB are under development....
---

## 2. How to Use It
### Open the Prefab folder (red is required, blue is optional)
![1](https://github.com/user-attachments/assets/24566164-7c7a-4d29-b29f-d012d887821e)
* #### Place snooker&pyramid&cn8&3c&10b (main pool table).
* #### Place TableHook (replica) 2 (required plugin).
* #### Place UI-Leaderboard (Leaderboard UI).
* #### Place TagPlug (Landmark main) (optional plugin).

## Then find VRC-VRCSA above.
![1](https://github.com/user-attachments/assets/09701d17-b73e-4cee-b834-ca5cb6385cdd)
* ### First, click Set Up Pool Table Layers.
* ### Then open the Build Tool:
	* #### Log in to your account (optional).
	* #### Click the buttons in sequence from top to bottom.
 	* #### This is an early version, so there may be some bugs.

## WEB System Usage (Early Testing)
* ### On your [Personal Homepage](https://www.wangqaq.com/PoolBar/Account), you can:
	* #### Set your colorful name.
	* #### Set your landmark.
	* #### If you want to use a colorful name, you need to first bind your user by going to the [map](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info) and copying the user code from the User Code System to the WEB, then click bind.
 	*  ![1](https://github.com/user-attachments/assets/b2f3a365-6ebe-452e-9d75-8b798ee98ac2)
* ### On the [Information Board](https://www.wangqaq.com/PoolBar/Information), you can:
	* #### View your season stats, user level, task list, events, and more.

## Update Plan
	* #### WebGL Billiards historical round viewing.
 	* #### Improved leaderboard.
  	* #### Abstract billiards mini-game.
  	* #### More languages for the WEB.

## Special Thanks

### Special thanks to everyone (no particular order): COCO , 蛋包饭 , 安洁罗塞塔,猪排,里爱吃...
### Special thanks to COCO for creating the logo, front-end, and UI (UI and front-end will be uploaded later due to time constraints).

## High Definition Images
![1](https://github.com/user-attachments/assets/22d982b4-a50e-420f-8db5-05553483445d)
![1](https://github.com/user-attachments/assets/3ab92dda-c7dc-4ab1-94dd-bce85f6809e2)
![1](https://github.com/user-attachments/assets/90a37503-a4c4-4b7f-936c-17f00c094bec)

![qrcode_1737098291587](https://github.com/user-attachments/assets/ebbfe76c-75b4-4352-b105-5e02ae20ff09)
