@SET CurrDir=%CD%
@CD..
@SET LUAC=%CD%/LuaC/bin/Debug/LuaC.exe
@CD %CurrDir%

@%LUAC% template.lua -o template.mpc
@%LUAC% areatest.lua -o areatest.mpc
@%LUAC% checkpointRace.lua -o checkpointRace.mpc
@%LUAC% DPLtest.lua -o DPLtest.mpc -dpl

pause