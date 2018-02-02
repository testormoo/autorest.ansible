
del c:\dev\ansible-hatchery\library\azure_rm_keyvault.py
del c:\dev\ansible-hatchery\library\azure_rm_keyvault_facts.py
rmdir /S /Q c:\dev\ansible-hatchery\tests\integration\targets\azure_rm_keyvault
rmdir /S /Q c:\dev\ansible-hatchery\tests\integration\targets\azure_rm_keyvault_facts
rmdir /S /Q c:\dev\tmp

cd c:\dev\azure-rest-api-specs\specification\keyvault\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2016-10

xcopy /S c:\dev\tmp\python\all\modules\* c:\dev\ansible-hatchery\library\
xcopy /S c:\dev\tmp\python\all\tests\* c:\dev\ansible-hatchery\tests\integration\targets\
xcopy /S c:\dev\tmp\all\modules\* c:\dev\ansible-hatchery\library\
xcopy /S c:\dev\tmp\all\tests\* c:\dev\ansible-hatchery\tests\integration\targets\
