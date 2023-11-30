-- CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
local Player = PACKAGE.InitMission.CreateCharacter(-2314.2590, 9.9802, 197.4827, 40, 29,  1.0,  0.0,  nil, 0, 0, true, 65537)
local Character = PACKAGE.InitMission.CreateCharacter(-2313.2590, 9.9802, 197.4827, 40, 29,  1.0,  0.0,  nil, 0, 0, false, 1)

local actorSetting = PACKAGE.InitMission.CreateActorSetting({Player,Character})
PACKAGE.InitMission.actorSetup.Add(actorSetting)

local LogicStart = PACKAGE.InitMission.LogicStart(0,"Driver: Parallel Lines Lua Mission")
LogicStart.wireCollection.Add(PACKAGE.InitMission.ActorCreation(Player))

-- Main mission
local mainMission = PACKAGE.AddMission(77,"mission77.mpc")

local mainMission_LogicStart = PACKAGE.InitMission.LogicStart()
mainMission_LogicStart.wireCollection.Add(mainMission.FadeAway(1.5))