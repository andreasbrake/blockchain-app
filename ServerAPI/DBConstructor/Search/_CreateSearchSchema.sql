ECHO OFF

SET server=ROC\SQLEXPRESS

SET username=admin
SET password=@dmiN!@#

SET db=BlockchainAppDb

ECHO ON

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\SearchSchema.sql


sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\SearchObject.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Selection.sql