rmdir /S /Q c:\dev\ansible-hatchery\library
rmdir /S /Q c:\dev\ansible-hatchery\tests
rmdir /S /Q c:\dev\ansible-hatchery\template
rmdir /S /Q c:\dev\tmp

cd c:\dev\azure-rest-api-specs\specification\sql\resource-manager
call autorest --output-folder=c:\dev\tmp --use=c:\dev\autorest.ansible\ --python --tag=package-2017-03-preview

cd c:\dev\azure-rest-api-specs\specification\mysql\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-04-preview

cd c:\dev\azure-rest-api-specs\specification\postgresql\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-04-preview

cd c:\dev\azure-rest-api-specs\specification\authorization\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2015-07

cd c:\dev\azure-rest-api-specs\specification\web\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2016-09

cd c:\dev\azure-rest-api-specs\specification\network\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10

cd c:\dev\azure-rest-api-specs\specification\containerinstance\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10-preview

cd c:\dev\azure-rest-api-specs\specification\containerregistry\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10

cd c:\dev\azure-rest-api-specs\specification\keyvault\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2016-10

cd c:\dev\azure-rest-api-specs\specification\batch\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-09

cd c:\dev\azure-rest-api-specs\specification\batchai\resource-manager
call autorest --output-folder=c:\dev\tmp\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-09-preview

xcopy /S c:\dev\tmp\python\all\modules\* c:\dev\ansible-hatchery\library\
xcopy /S c:\dev\tmp\python\all\tests\* c:\dev\ansible-hatchery\tests\integration\targets\
xcopy /S c:\dev\tmp\all\modules\* c:\dev\ansible-hatchery\library\
xcopy /S c:\dev\tmp\all\tests\* c:\dev\ansible-hatchery\tests\integration\targets\
