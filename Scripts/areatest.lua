local X,Y,Z,W = -1894.6520, 1.0625, -453.1732, 0 
local PX,PY,PZ,PW = -1890.66, 1.0625, -458.3958, 53.42831 -- Player position as a global variable just in case.

local X2,Y2,Z2 = X+20,Y-0.3,Z

MISSION.missionSummary.Level = "Miami_Day" -- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
-- Set start position so the game loads nicely with no delay
MISSION.missionSummary.X = X; MISSION.missionSummary.Y = Y
-- Set our sky to normal one...
MISSION.missionSummary.MoodId = 33 -- sky_trapped.d3s (Miami at Day - Dry)

-- Creates the player
local Player = MISSION.CreateCharacter(PX,PY,PZ,PW, 1, "Player", -1, -1, 1.0, 0.0, 0, 1.0)
-- Creates a row, Idk just for testing
for i=1,5 do
   local role = i+1
   MISSION.CreateCharacter(X+i,Y,Z,i*90, role, "Char", 1, 1, 1.0, 0.0, 1, 1.0)
end
for i=1,10 do
   local col = i-1
   if col>4 then col = 0 end
   MISSION.CreateVehicle(X+(i*6),Y-0.5,Z-5,i*20,0x5+i,col)
end
for i=0,20 do
   MISSION.CreateCollectable(X+i,Y,Z-10,i,9999,0)
end


-- Time for some logic scripting here
-- Let's make the basic stuff like when the player is dead, arrested or being chased!

-- Initialize the logic start, it's triggered when the game starts the mission.
local LogicStart = MISSION.LogicStart("Area Test - Start") 

local objTimer = MISSION.Timer(0.01)
LogicStart.wireCollection.Add(objTimer)

local range = 1.5

local point = MISSION.ActorHasReachedPoint(Player,X2,Y2,Z2,range)
point.wireCollection.Add(MISSION.MissionComplete())
local obj = MISSION.CreateObjectiveIcon(X2,Y2,Z2,1,0,0,false)
-- felony set to max test
local playAudio = MISSION.PlayAudio(0,3)
local point2 = MISSION.ActorHasReachedPoint(Player,X2+5,Y2,Z2,range)
point2.wireCollection.Add(playAudio,2)
point2.wireCollection.Add(playAudio)
point2.wireCollection.Add(MISSION.SetCharacterFelonyTo(Player,1))
point2.wireCollection.Add(MISSION.SetCharacterFelonyTo(Player,1),2) -- on success disable
point2.wireCollection.Add(point2,3)
local obj2 = MISSION.CreateObjectiveIcon(X2+5,Y2,Z2,1,0,0,false)
-- felony set to zero test
local point3 = MISSION.ActorHasReachedPoint(Player,X2+10,Y2,Z2,range)
point3.wireCollection.Add(playAudio,2)
point3.wireCollection.Add(playAudio)
point3.wireCollection.Add(MISSION.SetCharacterFelonyTo(Player,0))
point3.wireCollection.Add(MISSION.SetCharacterFelonyTo(Player,0),2) -- on success disable
point3.wireCollection.Add(point3,3)
local obj3 = MISSION.CreateObjectiveIcon(X2+10,Y2,Z2,1,0,0,false)

objTimer.wireCollection.Add(MISSION.ActorCreation(obj))
objTimer.wireCollection.Add(point)
objTimer.wireCollection.Add(MISSION.ActorCreation(obj2))
objTimer.wireCollection.Add(point2)
--point2.wireCollection.Add(point2) -- this makes a chain loop
objTimer.wireCollection.Add(MISSION.ActorCreation(obj3))
objTimer.wireCollection.Add(point3)
--point3.wireCollection.Add(point3) -- this makes a chain loop

-- CAMERA TEST
local camera1 = MISSION.CreateCamera(X+2,Y,Z+2,0,0,0)
local camera2 = MISSION.CreateCameraLookingAtPosition(X+2,Y,Z+2, 0,Y,10) -- rotate 90 degrees and move a bit further
local camera3 = MISSION.CreateCameraLookingAtPosition(X,Y+10,Z,  X,Y,Z) -- now move up and look to the player
-- cameras selection
local cam1 = MISSION.CameraSelect(camera1,1,5,2)
local cam2 = MISSION.CameraSelect(camera2,1,5,2)
local cam3 = MISSION.CameraSelect(camera3,1,5,2)

--LogicStart.wireCollection.Add(cam1)
cam1.wireCollection.Add(cam2)
cam2.wireCollection.Add(cam3)
local backToNormalTimer = MISSION.Timer(5)
backToNormalTimer.wireCollection.Add(cam3,2)

cam3.wireCollection.Add(backToNormalTimer)

-- Let's create the node of when the player is dead...
local playerIsDead = MISSION.CharacterHasDied(Player)
-- arrested...
local playerIsArrested = MISSION.CharacterWasArrested(Player)
-- ...and add their jobs to the LogicStart
LogicStart.wireCollection.Add(playerIsDead)
LogicStart.wireCollection.Add(playerIsArrested)

-- OK, now let's code when the player is dead,
-- Let's make something like a thrill cam like in the normal game and..
-- ..fail the mission of course.

-- REMINDER: Remember to add the ID of the localised string to your failed mission node!
-- MISSION.FailMission(localisedStringId)

-- when dead...
playerIsDead.wireCollection.Add(MISSION.FailMission(157)) -- fail the mission 
playerIsDead.wireCollection.Add(MISSION.CameraSelect(Player,13)) -- set up the thrill cam
-- when arrested...
playerIsArrested.wireCollection.Add(MISSION.FailMission(158)) -- fail the mission 
playerIsArrested.wireCollection.Add(MISSION.CameraSelect(Player,13)) -- set up the thrill cam

-- Alright! now, something: the chase and normal music is not set by the game itself, you have to code them.
local playerIsBeingChased = MISSION.CharacterIsBeingChased(Player)
-- once again add them to LogicStart
LogicStart.wireCollection.Add(playerIsBeingChased)
playerIsBeingChased.wireCollection.Add(MISSION.MusicControl(2),1) -- Oh no, the cops! play the song!
playerIsBeingChased.wireCollection.Add(MISSION.MusicControl(1),3) -- Not being chased? set music to normal.

-- well. the music is not going normal now so let's put it on the normal one by default.
LogicStart.wireCollection.Add(MISSION.MusicControl(1),1)

-- Anyways, this is the end. in order to make this script a mission you have to do the following steps:
-- 1. Open Zartex 2.0
-- 2. Load any mission (any mission you want to replace or something..)
-- 3. Go to "Import Lua Script" and select this script or your script from this one.
-- 4. Save it and try it out. make sure you fixed all your errors before making it a mission!
-- Enjoy!!