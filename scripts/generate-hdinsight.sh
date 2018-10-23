cd /azure-rest-api-specs/specification/hdinsight/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2015-03-preview
cp -r  /ansible-hatchery-tmp/azure-mgmt-hdinsight/* /ansible-hatchery-tmp && rm -R /ansible-hatchery-tmp/azure-mgmt-hdinsight/*