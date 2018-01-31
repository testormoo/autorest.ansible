rem test
rmdir /S /Q c:\dev\ansible-hatchery\role\modules\azure_rm_sql*
rmdir /S /Q c:\dev\ansible-hatchery\all\modules\azure_rm_sql*
rmdir /S /Q c:\dev\ansible-hatchery\role\tests\azure_rm_sql*
rmdir /S /Q c:\dev\ansible-hatchery\all\tests\azure_rm_sql*
rmdir /S /Q c:\dev\ansible-hatchery\template\azure_rm_sql*

cd c:\dev\azure-rest-api-specs\specification\sql\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery --use=c:\dev\autorest.ansible\ --python --tag=package-2017-03-preview

xcopy /S c:\dev\ansible-hatchery\python\prs\* c:\dev\ansible-hatchery\prs\
xcopy /S c:\dev\ansible-hatchery\python\role\* c:\dev\ansible-hatchery\role\
xcopy /S c:\dev\ansible-hatchery\python\all\* c:\dev\ansible-hatchery\all\
xcopy /S c:\dev\ansible-hatchery\python\template\* c:\dev\ansible-hatchery\template\
rmdir /S /Q c:\dev\ansible-hatchery\python
