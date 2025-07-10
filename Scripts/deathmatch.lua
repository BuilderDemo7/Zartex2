-------------------------------------------------[[
-- template.lua coded to newbies by BuilderDemo7 ||
-- Game              -              Driv3r       ||
-------------------------------------------------]]

-- Functions Syntax:
-- MISSION.function_name(arguments)

-- Create Character Syntax: 
-- MISSION.CreateCharacter(float x, float y, float z, float angle, int role, string personality, int personalityId, int personalityIndex, float health, float felony, int weapon, float vulnerability, [ string note="", int flags = 131073, byte r = 0,byte g = 155,int b = 200)

-- Let's start up by the initial data

-- Location : P.D. Firing Range (Miami)
local X,Y,Z,W = 1633.591, 1.062499, -779.1888, -162.483 -- Player position as a global variable just in case.

MISSION.missionSummary.Level = "Miami_Day" -- set up the city, you have other options like "Istanbul" and "Nice" and the day time options "Day" and "Night"
-- Set start position so the game loads nicely with no delay
MISSION.missionSummary.X = X; MISSION.missionSummary.Y = Y
-- Set our sky to normal one...
MISSION.missionSummary.MoodId = 9 -- sky_trapped.d3s (Miami at Day - Dry)

-- Creates the player
local Player = MISSION.CreateCharacter(X,Y,Z,W, SKIN_TANNER, 1, "Player", -1, -1, 1.0, 0.0, 0, 1.5)

-- Time for some logic scripting here
-- Let's make the basic stuff like when the player is dead, arrested or being chased!

-- Initialize the logic start, it's triggered when the game starts the mission.
local title = "Deathmatch - Start"
local LogicStart = MISSION.LogicStart(title) 
LogicStart.wireCollection.Add(MISSION.DisplayMessage(19,5))
LogicStart.wireCollection.Add(MISSION.CopControl(0)) -- disable cops
LogicStart.wireCollection.Add(MISSION.FadeOut(0.5))
LogicStart.wireCollection.Add(MISSION.DebugText(title))

local collectables = {
    -- X,Y,Z,Rot,Type
	{1629.793, 1.05, -798.138, math.rad(90), COLLECTABLE_MG},
	{1620.459, 1.05, -793.2084, 0, COLLECTABLE_MEDKIT},
	{1619.626, 1.05, -793.1901, 0, COLLECTABLE_MEDKIT},
}

local Baddies = {
	-- X,Y,Z, Angle, Weapon, [ Personality, Role ]
	{1625.937, 2, -798.3182, 0, 8}, -- in a hall (troller)
	{1632.439, 2, -799.1473, 171.4081, 2}, -- hidden in the box 1
	{1631.881, 2, -786.3168, -180, 2}, -- hidden in the box 2
	{1619.186, 2, -781.7601, -11.54785, 8, nil, 6}, -- hiddein in another hall
	{1616.608, 2, -781.9532, 0, 8},
	-- army I think..
	{1598.358, 2, -791.9482, 0, 4},
	{1598.306, 2, -793.5316, 0, 4},
	{1598.256, 2, -795.0521, 0, 4}
}

local BaddieInstances = {}

for i=1,#Baddies do
   local baddie = Baddies[i]
   BaddieInstances[i] = MISSION.CreateCharacter(baddie[1],baddie[2],baddie[3],baddie[4], SKIN_TANNER, baddie[7] == nil and 3 or baddie[7], baddie[6] == nil and "Baddie" or baddie[6], 2, 2, 1.0, 0.0, baddie[5], 1.0)
end

for i=1,#collectables do
   local col = collectables[i]
   MISSION.CreateCollectable(col[1],col[2],col[3],col[5],10,col[4])
end

local cam1 = MISSION.CreateCamera(1629.809, 2.062498, -799.5001, 0,0,0,  1.5,  1, 0, Player)
local cam1_sel = MISSION.CameraSelect(cam1,1)

local trapPt = MISSION.ActorHasReachedPoint(Player,1629.809, 2.062498, -795.4251, 3.5)
local trapPath = MISSION.CreateAIPath(BaddieInstances[1], { {1625.056, 2.062498, -798.3605}, { 1628.507, 2.062498, -798.38 }, { 1629.685, 2.062498, -797.6786 }, { 1629.779, 2.103407, -796.6094 } }, 7)
LogicStart.wireCollection.Add(trapPt)
--trapPt.wireCollection.Add(MISSION.DisplayMessage(20,3))
trapPt.wireCollection.Add(MISSION.FollowPath(trapPath))
trapPt.wireCollection.Add(cam1_sel,2) -- disable camera

local carePt = MISSION.ActorHasReachedPoint(Player,1616.157, 2.062499, -783.4706, 2)
LogicStart.wireCollection.Add(carePt)
carePt.wireCollection.Add(MISSION.DisplayMessage(21,5))

local musicPT = MISSION.ActorHasReachedPoint(Player, 1629.571, 2.062499, -781.6116, 1.5)
LogicStart.wireCollection.Add(musicPT)
musicPT.wireCollection.Add(MISSION.MusicControl(2))
--musicPT.wireCollection.Add(cam1_sel)

local path = MISSION.CreateAIPath(BaddieInstances[4], { {1619.186, 2, -781.7601}, {1621.782, 2, -782.2904}, { 1622.911, 2, -787.5697 }, { 1622.986, 2, -793.1244 } }, 2 )
local pathTimer = MISSION.ActorHasReachedPoint(Player,1622.349, 2.062498, -796.2192, 4)
LogicStart.wireCollection.Add(pathTimer)
pathTimer.wireCollection.Add(MISSION.FollowPath(path))

local missionCmpPt = MISSION.ActorHasReachedPoint(Player,1635.561, 2.062499, -775.5707,2)
missionCmpPt.wireCollection.Add(pathTimer)
missionCmpPt.wireCollection.Add(MISSION.MissionComplete(true))
missionCmpPt.wireCollection.Add(pathTimer)

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

--[[
-- Alright! now, something: the chase and normal music is not set by the game itself, you have to code them.
local playerIsBeingChased = MISSION.CharacterIsBeingChased(Player)
-- once again add them to LogicStart
LogicStart.wireCollection.Add(playerIsBeingChased)
playerIsBeingChased.wireCollection.Add(MISSION.MusicControl(2),1) -- Oh no, the cops! play the song!
playerIsBeingChased.wireCollection.Add(MISSION.MusicControl(1),3) -- Not being chased? set music to normal.
]]

-- well. the music is not going normal now so let's put it on the normal one by default.
--LogicStart.wireCollection.Add(MISSION.MusicControl(2),1)