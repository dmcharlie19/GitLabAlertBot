Application for send gitlab activity to telegram

1) Create bot in telegram
2) Get a bot token
3) Get a gitlab token
4) Create database in PostgreSQL
5) Fill appsettings.json
   
   "GitLabAlert": {
    "ServerUrl": "http://gitlabURL.ru",
    "Token": "",
    "GroupId": 0,
    "RequestRepeatInSec": 30
  },

  "Postgres": {
    "DbConnectionString": "Server=localhost;Username=postgres;Database=gitlab;Port=5432;Password=pswd;SSLMode=Prefer;"
  },

  "TgBot": {
    "Token": "",
    "BotName": "",
    "Supervisors": [ "channel_id" ]
  }
