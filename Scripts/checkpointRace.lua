-- Main
local mainInfo = {
	startPosition = {-836.8671, 0.5190547, 1486.318, 0}, -- Downtown
	vehicleId = 0x6, -- Mustang
	vehicleColorId = 0,
	-- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
	city = "Miami_Day",
	MoodId = 33,

	MinutesLeft = 2,
	SecondsLeft = 36
}

local gCar = MISSION.CreateVehicle(mainInfo.startPosition[1],mainInfo.startPosition[2],mainInfo.startPosition[3], mainInfo.startPosition[4], mainInfo.vehicleId, mainInfo.vehicleColorId, "gCar")
local gPlayer = MISSION.CreateCharacterInVehicle(gCar,1, "Player", -1, -1, 1.0, 0.0,  0,  0.0,   0.35, -0.05, 0.1,  1)
MISSION.missionSummary.Level = mainInfo.city
MISSION.missionSummary.MoodId = mainInfo.MoodId

-- Check points
local checkpoints = {
    -- X,Y,Z, radius
	{-843.7353, 0.5190158, 1212.941, 10},
	{-1067.454, 0.5210367, 997.1308, 25},
	{-1172.464, 10.51439, 648.4756, 8},
	{-972.0201, 0.5765933, -72.92726, 7},
	{-534.1889, 0.5194557, -330.1078, 5},
	{-207.662, 0.521814, -809.6689, 5},
	{178.607, 2.519112, -458.6404, 5}
}

local LogicStart = MISSION.LogicStart("Checkpoint Race Template - Start")
local IGCS = MISSION.ToggleIngameCutscene(true,true)

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

local hitCPsMsgTimer = MISSION.Timer(2) -- "Hit the checkpoints"
ctd0_t.wireCollection.Add(hitCPsMsgTimer) -- disable in-game cutscene
hitCPsMsgTimer.wireCollection.Add(MISSION.DisplayMessage(6,5))

-- Finally add the countdown chain to logic
LogicStart.wireCollection.Add(ctd3_t)

-- Initalize the clock
LogicStart.wireCollection.Add(MISSION.ShowClock( (mainInfo.MinutesLeft*60)+mainInfo.SecondsLeft ))

-- Checkpoint creation
local oldAreaWatch = MISSION.ActorHasReachedPoint(gPlayer,checkpoints[1][1],checkpoints[1][2],checkpoints[1][3],checkpoints[1][4])
local oldObjective = MISSION.CreateObjectiveIcon(checkpoints[1][1],checkpoints[1][2],checkpoints[1][3],checkpoints[1][4])
oldAreaWatch.wireCollection.Add(MISSION.DestroyActor(oldObjective))
oldAreaWatch.wireCollection.Add(MISSION.PlayAudio(0,3)) -- "checkpoint" sound
ctd0_t.wireCollection.Add(MISSION.ActorCreation(oldObjective)) -- add the checkpoint to creation
ctd0_t.wireCollection.Add(oldAreaWatch) -- add the area watch to creation
-- bomb car testing
--local bombCar = MISSION.BombCar(gCar)
--bombCar.wireCollection.Add(MISSION.FailMission(0),3) -- on failure enable
--ctd0_t.wireCollection.Add(bombCar)

for i=2,#checkpoints do
   local areaWatch = MISSION.ActorHasReachedPoint(gPlayer,checkpoints[i][1],checkpoints[i][2],checkpoints[i][3],checkpoints[i][4])
   local objective = MISSION.CreateObjectiveIcon(checkpoints[i][1],checkpoints[i][2],checkpoints[i][3],checkpoints[i][4])
   areaWatch.wireCollection.Add(MISSION.DestroyActor(oldObjective))
   areaWatch.wireCollection.Add(MISSION.PlayAudio(0,3)) -- "checkpoint" sound
   areaWatch.wireCollection.Add(MISSION.DestroyActor(objective)) -- remove this checkpoint
   oldAreaWatch.wireCollection.Add(areaWatch) -- add this area watch to the old one to be enabled
   oldAreaWatch.wireCollection.Add(MISSION.ActorCreation(objective)) -- add this checkpoint to the old one to be enabled

   -- if it's last then complete the driving game
   if i==#checkpoints then 
      areaWatch.wireCollection.Add(MISSION.MissionComplete(false))  
	  areaWatch.wireCollection.Add(MISSION.CameraSelect(gPlayer,13))
	  areaWatch.wireCollection.Add(MISSION.DisplayMessage(4,10))
   end

   oldAreaWatch = areaWatch
   oldObjective = objective
end

-- Failed sequences
local OutOfTime = MISSION.HasClockRunnedOut()
OutOfTime.wireCollection.Add(MISSION.FailMission(12,5.0))
OutOfTime.wireCollection.Add(MISSION.CameraSelect(gPlayer,13))
ctd0_t.wireCollection.Add(OutOfTime)
-- Classic, "You wrecked your vehicle"
local vehicleWrecked = MISSION.VehicleWrecked(gCar)
vehicleWrecked.wireCollection.Add(MISSION.FailMission(5,5.0))
vehicleWrecked.wireCollection.Add(MISSION.CameraSelect(gPlayer,13))
ctd0_t.wireCollection.Add(vehicleWrecked)

-- starts the clock
ctd0_t.wireCollection.Add(MISSION.StartClock())