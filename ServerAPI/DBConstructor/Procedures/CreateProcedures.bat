ECHO OFF

SET server=ROC\SQLEXPRESS

SET username=admin
SET password=@dmiN!@#

SET db=BlockchainAppDb

ECHO ON

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Search\BuildQuery.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Search\PerformSearch.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Search\ResolveSavedSearch.sql