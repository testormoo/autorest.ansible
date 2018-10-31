cd /azure-rest-api-specs/specification/mariadb/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2018-06-01-preview
cp -r /ansible-hatchery-tmp/azure-mgmt-rdbms/* /ansible-hatchery-tmpx/
