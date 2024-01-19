--[[
// Game: Driv3r
// Code by: BuilderDemo7
// Script: President's Run
//
// Inspired by President's Run from Driver 1
--]]

-- Main
local LogicStart = MISSION.LogicStart("President's Run - Start")
local IGCS = MISSION.ToggleIngameCutscene(true,true)

local mainInfo = {
	startPosition = {864.1714, 10.0, -627.9531, -95},
	vehicleId = VehicleType.BMW_Alpina, 
	vehicleColorId = 1,
	vehicleSoftness = 1.1, -- this will make the car a bit weaker
	vehicleWeight = 1.3, -- this will make the car a bit heavier I think
	vehicleFragility = 0.8, -- make parts less breakable
	-- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
	city = "Nice_Day",
	MoodId = 17, -- For example, 33 is Miami at Day Dry.
	
	FirstPersonCamera = false, -- enable this for an test first person camera
	FirstPersonCameraOffset = {0.4, 0.5, 0},
	CountdownCameraPosition = {883.0967, 13.72495, -677.8094},
	Countdown2CameraPosition = {871.0052, 9.91834, -644.2992},
	
	CountdownCameraBlendTime = 0,
	Countdown2CameraBlendTime = 4,
	
	CountdownCameraZoom = 3,
	Countdown2CameraZoom = 1.5,
	
	PlayerSkin = 0,
	
	wreckedMessageId = 5,
	objectiveMessageId = 6,
	objectivePosition = {-2042.135, 6.914793, 961.794},
	objectiveRange = 5
}

-- roadblocks present in the game
local roadBlockCar = VehicleType.CopCar_Nice
local roadBlockCarWeight = 100.0
local roadBlockCarSoftness = 0.3
local roadBlocks = {
    -- near spawn point
	--{2535.714, 47.51862, -364.6821, 90.0},
	--{2535.709, 47.58025, -368.8509, 84.0},

    {-1594.595, 6.70713, 704.7729, -115.0641},
	{-1592.507, 6.557433, 709.2252, -115.2365},
	{-1590.737, 6.557465, 712.9821, -115.2364},
	{-1589.003, 6.557533, 716.673, -115.2362},
	{-1586.881, 6.632001, 721.0812, -115.9522},
	{-1586.156, 6.877462, 726.3002, -61.45793},
	{-2129.229, 8.701674, 864.8749, -87.26768},
	{-2129.336, 8.846882, 869.2698, -86.95452},
	{-2128.785, 8.845162, 860.5769, -87.42158},
	{-2200.302, 10.70258, 865.3787, 3.944548}

    -- near a jump
    --{1992.631, 31.58685, 231.0061, -164.8},
    --{1987.562, 31.4365, 229.6413, -164.8},
    --{1982.611, 31.43652, 228.3073, -164.8},
    --{1977.902, 31.43661, 227.0376, -164.8},
    --{1974.511, 31.58654, 226.161, -164.8}
};
for rID = 1, #roadBlocks do
   local x,y,z,a = roadBlocks[rID][1],roadBlocks[rID][2],roadBlocks[rID][3], roadBlocks[rID][4] -- /2+45
   local car = MISSION.CreateVehicle(x,y,z,a, roadBlockCar, 0, "ROADBLOCK"..rID, 655361, 0, roadBlockCarSoftness, roadBlockCarWeight)
end

local gCar = MISSION.CreateVehicle(mainInfo.startPosition[1],mainInfo.startPosition[2],mainInfo.startPosition[3], mainInfo.startPosition[4], mainInfo.vehicleId, mainInfo.vehicleColorId, "gCar", 302186497, 0, mainInfo.vehicleSoftness, mainInfo.vehicleWeight, mainInfo.vehicleFragility)
-- NOTE: the passenger should be created before the driver or else if the player is the driver he can't move.
local gPresident = MISSION.CreateCharacterInVehicle(gCar, math.floor(0x3F92B120), 1, "Civilian", -1, -1, 1.0, 0.0,  0,  0.0,   -0.35, -0.1, -0.9,  10)
local gPlayer = MISSION.CreateCharacterInVehicle(gCar, mainInfo.PlayerSkin, 1, "Player", -1, -1, 1.0, 0.0,  0,  0.0,   0.4, -0.1, 0,  1)

MISSION.missionSummary.Level = mainInfo.city
MISSION.missionSummary.MoodId = mainInfo.MoodId

local copsArmor = 1
local copsAggresion = 1
local survivalCopsControl = MISSION.CopControl(3, 1, copsAggresion, copsArmor, 0, 0, 0, 917504, "Set cops to insane/angry",  255,0,0)
LogicStart.wireCollection.Add(survivalCopsControl)
local firstPersonCamera = MISSION.CreateCamera(mainInfo.FirstPersonCameraOffset[1], mainInfo.FirstPersonCameraOffset[2], mainInfo.FirstPersonCameraOffset[3],   0, 0, 0,   1, 1, 0, nil, gPlayer)
local countdownCamera = MISSION.CreateCamera(mainInfo.CountdownCameraPosition[1], mainInfo.CountdownCameraPosition[2], mainInfo.CountdownCameraPosition[3],   0, 0, 0,   mainInfo.CountdownCameraZoom, 1, 0, gPlayer, nil)
local countdown2Camera = MISSION.CreateCamera(mainInfo.Countdown2CameraPosition[1], mainInfo.Countdown2CameraPosition[2], mainInfo.Countdown2CameraPosition[3],   0, 0, 0,   mainInfo.Countdown2CameraZoom, 1, 0, gPlayer, nil)
local countdownCameraSelect = MISSION.CameraSelect(countdownCamera,1,0.01,mainInfo.CountdownCameraBlendTime)
local countdown2CameraSelect = MISSION.CameraSelect(countdown2Camera,1,0,mainInfo.Countdown2CameraBlendTime)
local countdown3CameraSelect = MISSION.CameraSelect(gCar,2,   1,1)
LogicStart.wireCollection.Add(MISSION.SetCharacterFelonyTo(gPlayer,1))
if (mainInfo.FirstPersonCamera) then
   LogicStart.wireCollection.Add(MISSION.CameraSelect(firstPersonCamera,1))
else 
   LogicStart.wireCollection.Add(countdownCameraSelect)
   countdownCameraSelect.wireCollection.Add(countdown2CameraSelect)
end
LogicStart.wireCollection.Add(IGCS)
LogicStart.wireCollection.Add(MISSION.SetCharacterStuckInVehicle(gPlayer))

-- Countdown
local ctd3_t = MISSION.Timer(1) -- 3
ctd3_t.wireCollection.Add(MISSION.DisplayMessage(0,1))
ctd3_t.wireCollection.Add(MISSION.PlayAudio(10,0))
local ctd2_t = MISSION.Timer(1) -- 2
ctd3_t.wireCollection.Add(ctd2_t)

ctd2_t.wireCollection.Add(MISSION.DisplayMessage(1,1))
ctd2_t.wireCollection.Add(MISSION.PlayAudio(10,0))
local ctd1_t = MISSION.Timer(1) -- 1
ctd2_t.wireCollection.Add(ctd1_t)

ctd1_t.wireCollection.Add(MISSION.DisplayMessage(2,1))
ctd1_t.wireCollection.Add(MISSION.PlayAudio(10,0))
-- "GO!"
local ctd0_t = MISSION.Timer(1) -- GO
ctd1_t.wireCollection.Add(ctd0_t)

ctd0_t.wireCollection.Add(MISSION.DisplayMessage(3,1))
ctd0_t.wireCollection.Add(MISSION.PlayAudio(10,1))
ctd0_t.wireCollection.Add(MISSION.MusicControl(2)--[[MISSION.PlayAudio(59,0)]])
ctd0_t.wireCollection.Add(IGCS,2) -- disable in-game cutscene

-- Init Message
local initMsgTimer = MISSION.Timer(2)
ctd0_t.wireCollection.Add(initMsgTimer) -- disable in-game cutscene
initMsgTimer.wireCollection.Add(MISSION.DisplayMessage(mainInfo.objectiveMessageId,5))
local objectiveIcon = MISSION.CreateObjectiveIcon(mainInfo.objectivePosition[1],mainInfo.objectivePosition[2],mainInfo.objectivePosition[3],mainInfo.objectiveRange)
initMsgTimer.wireCollection.Add(MISSION.ActorCreation(objectiveIcon))
-- add detection if you have reached the objective icon
local objectiveAreaWatch = MISSION.ActorHasReachedPoint(gPlayer,mainInfo.objectivePosition[1],mainInfo.objectivePosition[2],mainInfo.objectivePosition[3],mainInfo.objectiveRange)
objectiveAreaWatch.wireCollection.Add(MISSION.MissionComplete(false))
initMsgTimer.wireCollection.Add(objectiveAreaWatch)

-- fix camera going back
if (mainInfo.FirstPersonCamera) then
   ctd0_t.wireCollection.Add(MISSION.CameraSelect(firstPersonCamera,1))
else
   -- disable countdown camera
   --ctd0_t.wireCollection.Add(countdownCameraSelect,2)
   --[[
   ctd0_t.wireCollection.Add(MISSION.CameraSelect(countdown2Camera,1,0,mainInfo.Countdown2CameraBlendTime))
   -- add final camera
   ctd0_t.wireCollection.Add(countdown3CameraSelect)   
   local dst = MISSION.Timer(1)
   dst.wireCollection.Add(countdown3CameraSelect,2)
   
   countdown3CameraSelect.wireCollection.Add(countdown3CameraSelect,2)
   ctd0_t.wireCollection.Add(dst)
   --]]
end

local cameraDuration = 1
local stuntAreaWatch = MISSION.ActorHasReachedPoint(gPlayer,1961.588, 37.2466, 248.7551,2)
local stuntCamera = MISSION.CreateCamera(1961.542, 50.0, 265.5558,   0, 0, 0,   1, 0.5, 0.3, gPlayer, nil)
local stuntCameraS = MISSION.CameraSelect(stuntCamera,1, cameraDuration,   1,1,1,0,   2, "Stunt Camera")
local stuntCameraT = MISSION.Timer(cameraDuration)
stuntCameraS.wireCollection.Add(stuntCameraT)
stuntAreaWatch.wireCollection.Add(stuntCameraS)
stuntCameraT.wireCollection.Add(stuntCameraS,2)
LogicStart.wireCollection.Add(stuntAreaWatch)

LogicStart.wireCollection.Add(stuntAreaWatch)

-- to disable the camera
local _igcs = MISSION.ToggleIngameCutscene(false,false)
stuntCameraT.wireCollection.Add(_igcs,2)
local _fd = MISSION.FrameDelay(1)
_fd.wireCollection.Add(_igcs,2)
_igcs.wireCollection.Add(_fd)

-- Finally add the countdown chain to logic
LogicStart.wireCollection.Add(ctd3_t)

-- Failed sequences
-- Classic, "You wrecked your vehicle"
local vehicleWrecked = MISSION.VehicleWrecked(gCar)
vehicleWrecked.wireCollection.Add(MISSION.FailMission(mainInfo.wreckedMessageId,10.0))
vehicleWrecked.wireCollection.Add(MISSION.CameraSelect(gPlayer,13))
ctd0_t.wireCollection.Add(vehicleWrecked)

-- Shows and starts the clock when countdown is done
ctd0_t.wireCollection.Add( MISSION.CountdownIntro(0,1) )