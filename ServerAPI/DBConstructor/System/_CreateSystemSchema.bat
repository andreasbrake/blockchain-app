ECHO OFF

SET server=ROC\SQLEXPRESS

SET username=admin
SET password=@dmiN!@#

SET db=BlockchainAppDb

ECHO ON

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\System.sql

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Module.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Object.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\ObjectField.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\RelationshipObject.sql

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Widget.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\Page.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\PageWidget.sql
sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\ObjectFieldWidget.sql

sqlcmd -U %username% -P %password% -S %server% -d %db% -i .\BaseObject.sql