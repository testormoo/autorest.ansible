rem test
rmdir /S /Q c:\dev\ansible-hatchery\prs
rmdir /S /Q c:\dev\ansible-hatchery\role
rmdir /S /Q c:\dev\ansible-hatchery\all
rmdir /S /Q c:\dev\ansible-hatchery\template


cd c:\dev\azure-rest-api-specs\specification\sql\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery --use=c:\dev\autorest.ansible\ --python --tag=package-2017-03-preview

cd c:\dev\azure-rest-api-specs\specification\mysql\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-04-preview

cd c:\dev\azure-rest-api-specs\specification\postgresql\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-04-preview

cd c:\dev\azure-rest-api-specs\specification\authorization\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2015-07

cd c:\dev\azure-rest-api-specs\specification\web\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2016-09

cd c:\dev\azure-rest-api-specs\specification\network\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10

cd c:\dev\azure-rest-api-specs\specification\containerinstance\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10-preview

cd c:\dev\azure-rest-api-specs\specification\containerregistry\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-10

cd c:\dev\azure-rest-api-specs\specification\keyvault\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2016-10

cd c:\dev\azure-rest-api-specs\specification\batch\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-09

cd c:\dev\azure-rest-api-specs\specification\batchai\resource-manager
call autorest --output-folder=c:\dev\ansible-hatchery\ --use=c:\dev\autorest.ansible\ --python --tag=package-2017-09-preview

xcopy /S c:\dev\ansible-hatchery\python\prs\* c:\dev\ansible-hatchery\prs\
xcopy /S c:\dev\ansible-hatchery\python\role\* c:\dev\ansible-hatchery\role\
xcopy /S c:\dev\ansible-hatchery\python\all\* c:\dev\ansible-hatchery\all\
xcopy /S c:\dev\ansible-hatchery\python\template\* c:\dev\ansible-hatchery\template\
rmdir /S /Q c:\dev\ansible-hatchery\python
