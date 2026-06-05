@echo off
chcp 65001 > nul

netsh http delete urlacl url=http://+:50080/ 

netsh http add urlacl url=http://+:50080/ user=Everyone

pause > nul
