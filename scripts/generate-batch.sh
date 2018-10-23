cd /azure-rest-api-specs/specification/batch/resource-manager
echo BATCH GENERATING
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-09
echo BATCH GENERATED
cp -r  /ansible-hatchery-tmp/azure-mgmt-batch/* /ansible-hatchery-tmp