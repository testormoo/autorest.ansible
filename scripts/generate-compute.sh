cd /azure-rest-api-specs/specification/compute/resource-manager
cat readme.md
autorest --output-folder=/ansible-hatchery-tmp --use=/autorest.ansible --python --tag=package-2017-12
