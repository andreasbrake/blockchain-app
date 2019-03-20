ECHO OFF

SET server=ROC\SQLEXPRESS

SET username=admin
SET password=@dmiN!@#

SET db=BlockchainAppDb

SET module=Patent

ECHO ON

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\%module%\Module.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\%module%\Object.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\%module%\ObjectField.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\%module%\RelationshipObject.sql