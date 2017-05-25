@ECHO OFF

ECHO Removing Directories

RD /s /q %~dp0\app.publish
RD /s /q %~dp0\Properties

ECHO Removing Files

del SmallBasicLibrary.xml
del LitDev.xml
del DBM.application
del DBM.exe.config
del DBM.exe.manifest
del DBM.exe.lastcodeanalysissucceeded
del DBM.exe.CodeAnalysisLog.xml

PAUSE
