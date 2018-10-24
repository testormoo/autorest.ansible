cd /azure-rest-api-specs/specification/sql/resource-manager
autorest --output-folder=/ansible-hatchery-tmp --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-composite-v3
cp -r /ansible-hatchery-tmp/azure-mgmt-sql/* /ansible-hatchery-tmpx/
