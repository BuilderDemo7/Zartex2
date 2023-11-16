-------------------------------------------------[[
-- template.lua coded to newbies by BuilderDemo7 ||
-- Game              -              Driv3r       ||
-------------------------------------------------]]

-- Functions Syntax:
-- MISSION.function_name(arguments)

-- Create Character Syntax: 
-- MISSION.CreateCharacter(float x, float y, float z, float angle, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, [ string note="", int flags = 131073, byte r = 0,byte g = 155,int b = 200)

-- Let's start up by the initial data

-- Location : Downtown Bar (Miami)
local X,Y,Z,angle = -1894.6520, 1.0625, -453.1732, 0 -- Player position as a global variable just in case.

MISSION.missionSummary.Level = "Miami_Day" -- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
-- Set start position so the game loads nicely with no delay
MISSION.missionSummary.X = X; MISSION.missionSummary.Y = Y
-- Set our sky to normal one...
MISSION.missionSummary.MoodId = 33 -- sky_trapped.d3s (Miami at Day - Dry)

-- Creates the player
local Player = MISSION.CreateCharacter(X,Y,Z,angle, 1, "Player", -1, -1, 1.0, 0.0, 0, 1.0)

-- Time for some logic scripting here
-- Let's make the basic stuff like when the player is dead, arrested or being chased!

-- Initialize the logic start, it's triggered when the game starts the mission.
local LogicStart = MISSION.LogicStart("Mission Name - Start") 

-- Now let's add the nodes to tell the game what to do when is when.
-- We'll be using:
-- .wireCollection.Add(Node logicnode, [ int wireType = 1)
-- For example:
-- if a node succeds (supossedly returned true) we use these:
-- LogicStart.wireCollection.Add(doSomethingNode,1) (on success enable...)
-- LogicStart.wireCollection.Add(doSomethingNode,2) (on success disable...)
-- if a node fails (supossedly returned false) we use these:
-- LogicStart.wireCollection.Add(doSomethingNode,3) (on failure enable...)
-- LogicStart.wireCollection.Add(doSomethingNode,4) (on failure disable...)

-- Let's begin!

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
playerIsDead.wireCollection.Add(MISSION.FailMission(0)) -- fail the mission 
playerIsDead.wireCollection.Add(MISSION.CameraSelect(Player,13)) -- set up the thrill cam
-- when arrested...
playerIsArrested.wireCollection.Add(MISSION.FailMission(0)) -- fail the mission 
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