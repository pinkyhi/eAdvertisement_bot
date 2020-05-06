	Algorithm

1) Install Telegram.Bot NuGet package
2) Register on ngrok and get your Key
3) Start ngrok.exe and add there your key with -authtoken command
4) Open port 44360 on your computer (create rules for brandmaur for ports in two copies for TCP and UDP. This procedure should be done to in and out connections. Next open port throught router)
5) Initalize tunneling in ngrok with command: ngrok http https://localhost:44360 -host-header="localhost:44360" . 
6) Copy ngrok https link to BotSettings class in WebHookUrl part.
7) Don't forget to add initialization of bot in the end of Startup.cs
8) Add ngrok extension for Visual Studio
9) Add GitHub extension
10) TO GITIGNORE FILES THAT YOU WILL CHANGE, WHICH WILL USE EVERY USER FOR EXAMPLE BOTSETTINGS.CS OR CLASS WHERE YOU GET CONNECTION TO DB.
11) Download MySQL Notifier and MySQL Workbench
12) Import dbDump that is in Dumps folder (button to do it is in "Administration" page in MySQL Workbench)
13) Check if you have all required NuGet packages: MySQL.Data, Pomelo.EntityFrameworkCore.MySql, Telegram.Bot