cd /azure-rest-api-specs/specification/machinelearningservices/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchey-tmp/ --use=/autorest.ansible --python --tag=package-2018-03-preview
cp -r /ansible-hatchery-tmp/* /ansible-hatchery-tmpx/
