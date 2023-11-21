@SET CurrDir=%CD%
@CD..
@SET LUAC=%CD%/LuaC/bin/Debug/LuaC.exe
@CD %CurrDir%
@%LUAC% template.lua -o test.mpc
pause