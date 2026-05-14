@echo off
chcp 65001

@REM 获取脚本所在目录的绝对路径
set "SCRIPT_ROOT=%~dp0"
@REM -1去掉末尾的反斜杠
set "SCRIPT_ROOT=%SCRIPT_ROOT:~0,-1%"


@REM 启动配置Luban导表文件全称
set "NAME_CONFIG_START=_Start.conf"

@REM 导表dll
for %%I in ("%SCRIPT_ROOT%\..\Luban\Luban.dll") do set "LUBAN_DLL=%%~fI"

@REM 导表模板路径
for %%I in ("%SCRIPT_ROOT%\Templates") do set "DIR_TEMPLATES=%%~fI"
@REM 输出根目录
for %%I in ("%SCRIPT_ROOT%\..\Output") do set "DIR_OUTPUT=%%~fI"

@REM ===[GameConfig]=========================================

@REM _Game.conf配置文件路径
for %%I in ("%SCRIPT_ROOT%\Excels\_Game.conf") do set "FILE_CONFIG_GAME=%%~fI"

@REM BYTES导出路径
for %%I in ("%DIR_OUTPUT%\Bytes\c\GameConfig") do set "DIR_OUTPUT_BYTES_CLIENT=%%~fI"
for %%I in ("%DIR_OUTPUT%\Bytes\s\GameConfig") do set "DIR_OUTPUT_BYTES_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\Bytes\cs\GameConfig") do set "DIR_OUTPUT_BYTES_CS=%%~fI"

@REM JSON导出路径
for %%I in ("%DIR_OUTPUT%\Json\c\GameConfig") do set "DIR_OUTPUT_JSON_CLIENT=%%~fI"
for %%I in ("%DIR_OUTPUT%\Json\s\GameConfig") do set "DIR_OUTPUT_JSON_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\Json\cs\GameConfig") do set "DIR_OUTPUT_JSON_CS=%%~fI"

@REM C#导出路径
for %%I in ("%DIR_OUTPUT%\Script\c") do set "DIR_OUTPUT_CSHARP_CLIENT=%%~fI"
for %%I in ("%DIR_OUTPUT%\Script\s") do set "DIR_OUTPUT_CSHARP_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\Script\cs") do set "DIR_OUTPUT_CSHARP_CS=%%~fI"

@REM ===[StartConfig]=========================================

@REM StartConfig文件夹路径
for %%I in ("%SCRIPT_ROOT%\Excels\StartConfig") do set "DIR_CONFIG_START=%%~fI"

@REM StartConfig Bytes导出路径
for %%I in ("%DIR_OUTPUT%\Bytes\s\StartConfig") do set "DIR_OUTPUT_BYTES_START_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\Bytes\cs\StartConfig") do set "DIR_OUTPUT_BYTES_START_CS=%%~fI"

@REM StartConfig Json导出路径
for %%I in ("%DIR_OUTPUT%\Json\s\StartConfig") do set "DIR_OUTPUT_JSON_START_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\Json\cs\StartConfig") do set "DIR_OUTPUT_JSON_START_CS=%%~fI"

@REM StartConfig C#导出路径
for %%I in ("%DIR_OUTPUT%\CSharp\s") do set "DIR_OUTPUT_CSharp_START_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\CSharp\cs") do set "DIR_OUTPUT_CSharp_START_CS=%%~fI"


@REM ===[配置检查]=========================================

echo .
echo ========================检查合规配置========================
echo .
echo ========================[配置GameConfig检查]========================

@REM -t 生成目标，取schema全局参数target中的一个
@REM -d 生成的数据目标。可以有0-n个。如 -d bin -d json
@REM -validationFailAsError 如果有任何校验器未通过，则生成失败。此参数一般在正式发布时使用
@REM %ERRORLEVEL% 是上一条命令的错误码 NEQ 0 不等于0表示错误

dotnet %LUBAN_DLL% ^
    -t all --conf %FILE_CONFIG_GAME% ^
    -d bin -d json -c cs-bin -c cs-editor-json ^
    --validationFailAsError true -x forceLoadDatas=1 -x outputSaver=null
    
if %ERRORLEVEL% NEQ 0 (
    echo ========================[配置检查 - 失败] 详情请看Log!========================
    pause
    exit
)

echo ========================[配置StartConfig检查]========================





pause