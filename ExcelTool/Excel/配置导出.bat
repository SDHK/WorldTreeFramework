@echo off
chcp 65001 >nul
rem 关闭命令回显。行首的 @ 让这一行本身也不打印。

rem 本文件是 UTF-8，没有 BOM。每一行中文后面都空一行，否则 cmd 会把下一行拆开执行。

rem chcp 65001 把当前窗口改成 UTF-8，好让后面 echo 出的中文能显示。它不改变文件本身的编码。

rem %~dp0 是本 bat 所在目录，末尾通常带反斜杠。

set "SCRIPT_ROOT=%~dp0"
rem :~0,-1 从下标 0 取到倒数第 1 个字符之前，用来去掉末尾的反斜杠。

set "SCRIPT_ROOT=%SCRIPT_ROOT:~0,-1%"

rem for 这里只转一轮，把相对路径收成绝对路径。

rem %%I 是这一轮的那一项，不是 set 出来的变量。写在 bat 里必须是两个百分号。

rem %%~fI 取完整路径。

for %%I in ("%SCRIPT_ROOT%\..\Luban\Luban.dll") do set "LUBAN_DLL=%%~fI"
for %%I in ("%SCRIPT_ROOT%\..\Luban\Templates") do set "DIR_TEMPLATES=%%~fI"
for %%I in ("%SCRIPT_ROOT%\..\Output") do set "DIR_OUTPUT=%%~fI"

rem Game 的入口。

for %%I in ("%SCRIPT_ROOT%\Excels\_Game.conf") do set "FILE_CONFIG_GAME=%%~fI"

rem 只要客户端 c 和服务器 s。没有名为 cs 的目录，没有 StartConfig，也没有编辑器目录。

rem 代码目录用 CSharp，不用 Script。

for %%I in ("%DIR_OUTPUT%\Bytes\c\GameConfig") do set "DIR_OUTPUT_BYTES_CLIENT=%%~fI"
for %%I in ("%DIR_OUTPUT%\Bytes\s\GameConfig") do set "DIR_OUTPUT_BYTES_SERVER=%%~fI"
for %%I in ("%DIR_OUTPUT%\CSharp\c") do set "DIR_OUTPUT_CSHARP_CLIENT=%%~fI"
for %%I in ("%DIR_OUTPUT%\CSharp\s") do set "DIR_OUTPUT_CSHARP_SERVER=%%~fI"

rem if not exist 看路径在不在。左括号和 if 写在同一行。块里面用 rem，不用两个冒号。

rem 失败用 exit /b 1，只结束本脚本，不关掉 cmd 窗口，也不 pause。

rem if errorlevel 1 表示上一条命令的退出码大于等于 1。它要紧跟在被检查的命令后面。

if not exist "%LUBAN_DLL%" (
    echo 找不到 Luban.dll：%LUBAN_DLL%

    exit /b 1
)

if not exist "%DIR_TEMPLATES%" (
    echo 找不到模板目录：%DIR_TEMPLATES%

    exit /b 1
)

if not exist "%FILE_CONFIG_GAME%" (
    echo 找不到游戏表入口：%FILE_CONFIG_GAME%

    exit /b 1
)

rem 行尾的 ^ 把下一行接成同一条命令，^ 后面不能有空格。

rem 检查时 outputSaver=null，只验证，不写文件。

echo ========================检查 client========================
dotnet "%LUBAN_DLL%" ^
    -t client --conf "%FILE_CONFIG_GAME%" --customTemplateDir "%DIR_TEMPLATES%" ^
    -d bin -c cs-bin ^
    --validationFailAsError true -x forceLoadDatas=1 -x outputSaver=null
if errorlevel 1 (
    echo 客户端检查失败

    exit /b 1
)

echo ========================检查 server========================
dotnet "%LUBAN_DLL%" ^
    -t server --conf "%FILE_CONFIG_GAME%" --customTemplateDir "%DIR_TEMPLATES%" ^
    -d bin -c cs-bin ^
    --validationFailAsError true -x forceLoadDatas=1 -x outputSaver=null
if errorlevel 1 (
    echo 服务器检查失败

    exit /b 1
)

rem 两次检查都通过后，才清空输出目录。rd /s /q 连子目录一起删，并且不询问。

if exist "%DIR_OUTPUT%" rd /s /q "%DIR_OUTPUT%"

echo ========================导出 client========================
dotnet "%LUBAN_DLL%" ^
    -t client --conf "%FILE_CONFIG_GAME%" --customTemplateDir "%DIR_TEMPLATES%" ^
    -d bin -x bin.outputDataDir=%DIR_OUTPUT_BYTES_CLIENT% -x outputSaver.bin.cleanUpOutputDir=0 ^
    -c cs-bin -x cs-bin.outputCodeDir=%DIR_OUTPUT_CSHARP_CLIENT% -x outputSaver.cs-bin.cleanUpOutputDir=0
if errorlevel 1 (
    echo 客户端导出失败

    exit /b 1
)

echo ========================导出 server========================
dotnet "%LUBAN_DLL%" ^
    -t server --conf "%FILE_CONFIG_GAME%" --customTemplateDir "%DIR_TEMPLATES%" ^
    -d bin -x bin.outputDataDir=%DIR_OUTPUT_BYTES_SERVER% -x outputSaver.bin.cleanUpOutputDir=0 ^
    -c cs-bin -x cs-bin.outputCodeDir=%DIR_OUTPUT_CSHARP_SERVER% -x outputSaver.cs-bin.cleanUpOutputDir=0
if errorlevel 1 (
    echo 服务器导出失败

    exit /b 1
)

echo ========================导出完成========================
exit /b 0
