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
MISSION.missionSummary.X = X; MISSION.missionSummary.Y = Z
-- Set our sky to normal one...
MISSION.missionSummary.MoodId = 33 -- sky_trapped.d3s (Miami at Day - Dry)

local drivingSpeed = 15
local burnoutTime = 1
local desiredTransport = 0
local civilianVulnerability = 2
local CIVCAR1 = MISSION.CreateVehicle(-1913.595, 0.5190591, -455.0526, -3.45654-45+90, VehicleType.Mustang_Grey, 1, "CIVCAR1")
local CIV1 = MISSION.CreateCharacterInVehicle(CIVCAR1, 5, "Civilian", 1, 1, 1.0, 0, 0, 0, civilianVulnerability, 0,0,0, 2, "CIV1", 131073,   28, 241, 28, 17)
local CIV1Path = {
    {-1912.26, 0.5194987, -455.585},
    {-1878.728, 0.5184318, -457.1945},
    {-1846.242, 0.52123, -455.8999},
    {-1810.301, 0.5188981, -448.0114},
    {-1800.7, 0.5145493, -443.0225},
    {-1795.549, 0.5189492, -435.8185},
    {-1795.775, 0.5201246, -425.7539},
    {-1802.882, 0.5210822, -411.6844},
    {-1828.937, 0.5198317, -383.2437},
    {-1849.703, 0.5186965, -360.2006},
    {-1866.888, 0.5178781, -328.1004},
	{-1870.674, 0.5188338, -311.264},
	{-1871.504, 0.5197124, -261.0361},
	{-1872.207, 0.5190955, -185.0325},
	{-1871.959, 0.5187896, -57.72547},
	{-1873.339, 0.5180643, -51.13936},
	{-1888.276, 0.5187631, -43.72892},
	{-2071.333, 0.5186058, -43.58247}
};
local CIV1PathActor = MISSION.CreateAIPath(CIV1,CIV1Path,drivingSpeed, burnoutTime, desiredTransport)
local CIV2CAR = MISSION.CreateVehicle(-1832.179, 0.5194556, -464.6792, -171.6823-45, VehicleType.Mustang_Grey, 2, "CIVCAR2")
local CIV2 = MISSION.CreateCharacterInVehicle(CIV2CAR, 5, "Civilian", 1, 1, 1.0, 0, 0, 0, civilianVulnerability, 0,0,0, 2, "CIV2", 131073,   28, 241, 28, 17)
local CIV2Path = {
    {-1832.682, 0.517638, -464.7524},
    {-1843.607, 0.5212051, -466.203},
    {-1859.831, 0.5199969, -467.379},
    {-1879.005, 0.5191385, -467.461},
    {-1896.746, 0.5180237, -466.788},
    {-1914.602, 0.5196089, -465.531},
    {-2038.877, 0.5199167, -450.948},
    {-2067.819, 0.5189644, -449.104},
    {-2100.562, 0.5197734, -451.988},
    {-2108.286, 0.6641797, -461.774},
    {-2108.486, 0.657473, -475.1469}
};
local CIV2PathActor = MISSION.CreateAIPath(CIV2,CIV2Path,drivingSpeed, burnoutTime, desiredTransport)
local CIVCAR3 = MISSION.CreateVehicle(-1827.396, 0.5189827, -375.1512, -43.93795, VehicleType.Challenger, 1, "CIVCAR3")
local CIV3 = MISSION.CreateCharacterInVehicle(CIVCAR3, 5, "Civilian", 1, 1, 1.0, 0, 0, 0, civilianVulnerability, 0,0,0, 2, "CIV3", 131073,   28, 241, 28, 17)
local CIV3Path = {
    {-1825.543, 0.5193697, -376.935},
    {-1812.459, 0.5189157, -389.557},
    {-1797.734, 0.5203368, -406.275},
	{-1788.96, 0.5194197, -421.4757},
	{-1776.576, 0.5191603, -444.899},
	{-1763.099, 1.191698, -475.6479},
	{-1757.413, 4.871417, -498.9307},
	{-1753.642, 10.50421, -541.4252}
};
local CIV3PathActor = MISSION.CreateAIPath(CIV3,CIV3Path,drivingSpeed, burnoutTime, desiredTransport)
local CIVCAR4 = MISSION.CreateVehicle(-1939.896, 1.68, -456.3064, -3.45654-45+90, VehicleType.RigTruck, 0, "CIVCAR4")
--local CIV4CARRIAGE = MISSION.CreateVehicle(-1939.896, 0.520709, -456.3064, -3.45654-45+90, VehicleType.RigTrailer, 1, "CIV4CARRIAGE")
local CIV4 = MISSION.CreateCharacterInVehicle(CIVCAR4, 5, "Civilian", 1, 1, 1.0, 0, 0, civilianVulnerability, 0,0,0, 2, "CIV4", 131073, 28, 241, 28)
local CIV4Path = {
    {-1936.09, 0.5191002, -456.7126},
    {-1919.698, 0.5180494, -458.517},
    {-1890.196, 0.51917, -460.68451},
    {-1862.384, 0.5203974, -460.853},
    {-1833.085, 0.5204166, -458.156},
    {-1808.449, 0.5188897, -451.377},
    {-1797.166, 0.5183992, -444.893},
    {-1792.839, 0.5184948, -422.478},
    {-1832.002, 0.5209721, -374.526},
    {-1850.041, 0.5196335, -353.570},
    {-1861.592, 0.5191852, -333.870},
    {-1866.091, 0.5185254, -316.888},
    {-1867.706, 0.518914, -226.7884},
    {-1870.718, 0.520692, -205.0789},
    {-1883.34, 0.6519698, -201.9816},
    {-1896.429, 0.6526512, -197.745},
	-- this clone might stop the truck, I think
    {-1896.429, 0.6526512, -197.744},
};
local CIV4PathActor = MISSION.CreateAIPath(CIV4,CIV4Path,drivingSpeed, burnoutTime, desiredTransport)

-- Creates the player
local Player = MISSION.CreateCharacter(X,Y,Z,angle, 1, "Player", -1, -1, 1.0, 0.0, 0, 1.0)
--local Player = MISSION.CreateCharacterInVehicle(CIVCAR1, 1, "Player", -1, -1, 1.0, 0.0, 0, 1.0, 0.1, -4, 0, 10)

-- Time for some logic scripting here
-- Let's make the basic stuff like when the player is dead, arrested or being chased!

-- Initialize the logic start, it's triggered when the game starts the mission.
local LogicStart = MISSION.LogicStart("Scripted Traffic - Start") 
-- Fully deactive traffic (including cops else it doesn't work)
LogicStart.wireCollection.Add(MISSION.CivilianTrafficControl(0, 0, 0, 0, -3, 65536, "No traffic"))
LogicStart.wireCollection.Add(MISSION.CopControl(0))
-- Make scripted traffic
LogicStart.wireCollection.Add(MISSION.FollowPath(CIV1PathActor,"CIV1 follow path"))
local CIV2FOLLOW = MISSION.FollowPath(CIV2PathActor,"CIV2 follow path")
CIV2FOLLOW.wireCollection.Add(CIV2FOLLOW,2)
LogicStart.wireCollection.Add(CIV2FOLLOW)
LogicStart.wireCollection.Add(MISSION.FollowPath(CIV3PathActor,"CIV3 follow path"))
LogicStart.wireCollection.Add(MISSION.FollowPath(CIV4PathActor,"CIV4 follow path"))

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