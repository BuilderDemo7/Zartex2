-------------------------------------------------------------------------
--|//////// Driver: Parallel Lines Lua Mission Package Vehicle  ////////| 
--|//////// Written By BuilderDemo7                             ////////|
--|\\\\\\\\ BRIEF:                                              \\\\\\\\|
--|This template is a freeroam on-car (Cerrano) in Manhattan    \\\\\\\\|
------------------------------------------------------------------------- 

-- CreateVehicle(float x, float y, float z, float angle, int vehicleId, int colorId, float initialSpeed = 0, float initialFelony = 0, float bulletSoftness = 0.3333f, float explosionSoftness = 0.5f, float impactSoftness = 0.6667f, float impactFragility = 0.6667f, Actor attachedVehicle = null, string note = "", int flags = 302186497, float damage = 0, float softness = 1, float weight = 1f, float fragility = 1, int r = 0, int g = 200, int b = 122)
-- CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)

local PlayerX,PlayerY,PlayerZ,PlayerAngle = 95.21597, 0.6525448, 115.5888, -8.5+180 -- the position of the player in variables
-- This is the player's vehicle
local PlayerVehicle = INITMISSION.CreateVehicle(PlayerX,PlayerY,PlayerZ,PlayerAngle, 47,  1)
INITMISSION.CountActor(PlayerVehicle) -- remember to add this to  the player or important actors
-- This is the player character
local Player = INITMISSION.CreateCharacter(PlayerX,PlayerY,PlayerZ,PlayerAngle, 29,  1.0,  0.0,  PlayerVehicle, 0, 0, true, 65537)
INITMISSION.CountActor(Player) -- remember to add this to  the player or important actors

local LogicStart_init = INITMISSION.LogicStart(0,"Paramedic (Init) - Start")
LogicStart_init.Wires.Add(INITMISSION.TimeControl(1,17,30)) -- 17:30

-- CreateMissionActor(float x, float y, float z, int subMissionID, int eventType, int potNumber = 0, int iconType = 12, string tempMissionName = "", string tempMissionDescription = "", string fileName = "", string FMVfileName = "", int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
-- This is mostly used for defining your type of mission, icon, pot number, event type, etc.
--local M1X,M1Y,M1Z = 95.9673, 0.09607407, -7.424409 -- the mission position
--local Mission1_markeractor = INITMISSION.CreateMissionActor(M1X,M1Y,M1Z, 1, 3, 0, 12, "Paramedics Incidental test", "template mission", "Scripts/DPL_ambulance.lua", "", 0)
--INITMISSION.CountActor(Mission1_markeractor)

local Ambulance = INITMISSION.CreateVehicle(84.05503082, 0.9875760078, 152.2644196, -90, 25,  1, 20, 0, 0.333, 0.5, 0.6667, 0.6667, nil, "Ambulance", 26214401)
local Paramedic = INITMISSION.CreateCharacter(84.05503082, 0.9875760078, 152.2644196, -90, 92,  1.0,  0.0,  Ambulance, 1, 0, false, 1)
INITMISSION.CountActor(Paramedic)
INITMISSION.CountActor(Ambulance)

LogicStart_init.Wires.Add(INITMISSION.ShowMarkerOnActor(Ambulance, 1, 2)) -- Show a marker on it
LogicStart_init.Wires.Add(INITMISSION.Wanderer(Paramedic, 23, 0.8)) -- Make him drive around

-- Initialize stuff

-- Some initialising stuff to not to crash or freeze the game.
local fadeIn = INITMISSION.SetFadeIn(0.0001) -- Fade in in 0.0001 seconds

local mission1_started = INITMISSION.MissionWatch(1,1)
LogicStart_init.Wires.Add(mission1_started) -- add that check of the mission

LogicStart_init.Wires.Add(INITMISSION.SetFadeOut(0.5))

LogicStart_init.Wires.Add(fadeIn) -- fade in animation
--fadeIn.Wires.Add(INITMISSION.StartMission(1,1)) -- and now the mission to start on that fade in

-- Traffic:
local trafficDensity = 0.8
local pedDensity = 1
local parkedCarsDensity = 0.1
-- format:
-- Vehicle ID, density/chance to show up
local trafficAssetDensities = {
    {36, 0.09}, -- Cerva              (common)
    {38, 0.06}, -- Bonsai             (common)
    {47, 0.06}, -- Cerrano            (common)
    {40, 0.07}, -- Brooklyn           (common)
    {76, 0.1},  -- Namorra            (mediocre)
    {53, 0.08}, -- Andec              (common)
    {62, 0.04}, -- Melizzano          (a bit common)
    {71, 0.015},-- San Marino         (rare)
    {73, 0.016} -- San Marino Spyder  (rare)
}; 
-- Character Skin Type, density/chance to show up
local pedAssetDensities = {
    {92, 0.1},
    {145, 0.1},
    {97, 0.1},
    {144, 0.1},
    {98, 0.05},
    {179, 0.05},
    {180, 0.05},
}; 

-- Now set the traffic in-game for real
LogicStart_init.Wires.Add(INITMISSION.DensityControl(trafficDensity, parkedCarsDensity, trafficAssetDensities))
LogicStart_init.Wires.Add(INITMISSION.PedestrianDensityControl(pedDensity, pedAssetDensities))

INITMISSION.JobComplete() -- important to start the next mission?
