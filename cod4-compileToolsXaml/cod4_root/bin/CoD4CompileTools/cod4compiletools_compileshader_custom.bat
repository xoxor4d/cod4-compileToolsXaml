@ECHO OFF

set shader_bin=%~1
set shader_name=%2
set exeName=shader_tool.exe

echo CONSOLESTATUS:COMPILE SHADER
echo.

cd %shader_bin%
echo %exeName% %shader_name%
%exeName% %shader_name%

