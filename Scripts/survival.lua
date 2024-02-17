-- Main
local LogicStart = MISSION.LogicStart("Survival Template - Start")
local IGCS = MISSION.ToggleIngameCutscene(true,true)

local mainInfo = {
	startPosition = {-837.4846, 0.5, 1450.464, 0}, -- Downtown
	vehicleId = 0x6, -- Bruiser
	vehicleColorId = 0,
	-- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
	city = "Miami_Day",
	MoodId = 10, -- For example, 33 is Miami at Day Dry.
	
	WinTime = 3600 -- in seconds; 1 hour
}

local gCar = MISSION.CreateVehicle(mainInfo.startPosition[1],mainInfo.startPosition[2],mainInfo.startPosition[3], mainInfo.startPosition[4], mainInfo.vehicleId, mainInfo.vehicleColorId, "gCar", 302186497, 0, 1)
local gPlayer = MISSION.CreateCharacterInVehicle(gCar,SKIN_TANNER,1, "Player", -1, -1, 1.0, 0.0,  0,  0.0,   0.35, -0.05, 0.1,  1)
MISSION.missionSummary.Level = mainInfo.city
MISSION.missionSummary.MoodId = mainInfo.MoodId

local survivalCopsControl = MISSION.CopControl(3, 1, -2, -2, 0, 0, 0, 917504, "Set cops to insane/angry",  255,0,0)
LogicStart.wireCollection.Add(survivalCopsControl)
LogicStart.wireCollection.Add(MISSION.SetCharacterFelonyTo(gPlayer,1))
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

-- Finally add the countdown chain to logic
LogicStart.wireCollection.Add(ctd3_t)

-- Failed sequences
local MissionComp = MISSION.OverlayClockWatch(mainInfo.WinTime)
MissionComp.wireCollection.Add(MISSION.MissionComplete(false))
ctd0_t.wireCollection.Add(MissionComp)
-- Classic, "You wrecked your vehicle"
local vehicleWrecked = MISSION.VehicleWrecked(gCar)
vehicleWrecked.wireCollection.Add(MISSION.FailMission(5,5.0))
vehicleWrecked.wireCollection.Add(MISSION.CameraSelect(gPlayer,13))
ctd0_t.wireCollection.Add(vehicleWrecked)

-- Shows and starts the clock when countdown is done
ctd0_t.wireCollection.Add( MISSION.CountdownIntro(0,1) )