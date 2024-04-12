-- CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)

local Player = INITMISSION.CreateCharacter(-2314.2590, 9.9802, 197.4827, 40, 29,  1.0,  0.0,  nil, 0, 0, true, 65537)
INITMISSION.CountActor(Player) -- remember to add this to  the player or important actors

-- For test, useful for not having to use "ActorCreation" for each actor (interesting to save space in the missions logic nodes)
--local actorSetting = INITMISSION.CreateActorSetting({Player})
--INITMISSION.actorSetup.Add(actorSetting)

local LogicStart_init = INITMISSION.LogicStart(0,"Driver: Parallel Lines Lua Test Mission (Init) - Start")
--LogicStart_init.Wires.Add(INITMISSION.CreationGroup(0))

-- CreateMissionActor(float x, float y, float z, int subMissionID, int eventType, int potNumber = 0, int iconType = 12, string tempMissionName = "", string tempMissionDescription = "", string fileName = "", string FMVfileName = "", int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
local mainMission_markeractor = INITMISSION.CreateMissionActor(-2314.2590, 9.9802, 197.4827, 90, 3, 0, 12, "mission 90", "main mission", "Scripts/DPLtest.lua", "", 8)
INITMISSION.CountActor(mainMission_markeractor)

-- Main mission
local mainMission = PACKAGE.CreateMission("mission 90",5,0,0,0)
PACKAGE.AddMission(mainMission, 90)

local mainMission_LogicStart = mainMission.LogicStart(0,"Main Mission - Start")

local wanderCar = mainMission.CreateVehicle(-2312.2590, 9.9802, 195.4827, 0, 47, 1)
local wanderGuy = mainMission.CreateCharacter(-2312.2590, 9.9802, 195.4827, 0, 98,  1.0,  0.0,  wanderCar, 0, 1, false, 1)
mainMission.CountActor(wanderGuy)
mainMission.CountActor(wanderCar)

mainMission_LogicStart.Wires.Add(mainMission.Wanderer(wanderGuy, 20, 0.8))
--mainMission_LogicStart.Wires.Add(mainMission.ShowMarkerOnActor(wanderGuy, 2))
mainMission_LogicStart.wireCollection.Add(mainMission.SetFadeOut(1.5))

-- Start the mission
local trafficDensity = 0.8
local pedDensity = 1
local parkedCarsDensity = 0.1
local trafficAssetDensities = {
    {21, 1}
}; 
local pedAssetDensities = {
    {0, 1},
    {98, 1}
}; 
mainMission_LogicStart.Wires.Add(INITMISSION.DensityControl(trafficDensity, parkedCarsDensity, trafficAssetDensities))
mainMission_LogicStart.Wires.Add(INITMISSION.PedestrianDensityControl(pedDensity, pedAssetDensities))

LogicStart_init.Wires.Add(INITMISSION.GarageControl(8, 0, -1, 16777216, 2))
local fadeIn = INITMISSION.SetFadeIn(0.0001)
local fadeOut = INITMISSION.SetFadeOut(10)
local startMissionTimer = INITMISSION.SetTimer(0.5)

LogicStart_init.Wires.Add(startMissionTimer)
startMissionTimer.Wires.Add(INITMISSION.StartMission(90,1,2))