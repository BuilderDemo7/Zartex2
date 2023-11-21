-- CreateCharacter(float x, float y, float z, float angle, int skin, float health, float felony, Actor vehicle=null, int vehicleSeat=0, int weapon = 0, bool player = false, int flags = 1, string note = "", byte r = 0, byte g = 155, int b = 200)
local Player = MISSION.CreateCharacter(-2314.2590, 9.9802, 197.4827, 40, 29,  1.0,  0.0,  nil, 0, 0, true, 65537)
local Character = MISSION.CreateCharacter(-2313.2590, 9.9802, 197.4827, 40, 29,  1.0,  0.0,  nil, 0, 0, false, 1)

local LogicStart = MISSION.LogicStart("Driver: Parallel Lines Lua Mission")
LogicStart.wireCollection.Add(MISSION.FadeOut(1.5))