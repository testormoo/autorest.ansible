
# How to use Autorest.Ansible?

Recommended way to use this extension is to use Docker image that is automatically produced by the CI.

CI is here: https://travis-ci.org/zikalino/autorest.ansible

  docker run -v <your-output-directory>:/ansible-hatchery -v <your-temporary-directory>:/ansible-hatchery-tmp dockiot/autorest-ansible-alt <api-to-generate>

where:

**your-output-directory** - is the directory where modules will be generated

**your-temporary-directory** - this directory is used by the generator as a temporary directory, and some additional diagnostic output will be written here

**api-to-generate** - what should be generated? Use **all** to generate all configured modules, or **sql**, **mysql**, **keyvault**, etc. Check **scripts** subdirectory for available scripts.

# How to add additional APIs?

You have to add additional **generate-<module-name>.sh** script in **scripts** subfolder, and include it in **generate-all.sh**.

# How to tweak generator output?

Following tweaks can be applied to the generator output:
- change module name
- rename options
- flatten options
- hide unnecessary options
- change default option values
- update sample option values
- update test dependencies
- update test field values

More information will be added here...



# AutoRest extension configuration

``` yaml
use-extension:
  "@microsoft.azure/autorest.modeler": "2.1.22"

pipeline:
  python/modeler:
    input: swagger-document/identity
    output-artifact: code-model-v1
    scope: python
  python/commonmarker:
    input: modeler
    output-artifact: code-model-v1
  python/cm/transform:
    input: commonmarker
    output-artifact: code-model-v1
  python/cm/emitter:
    input: transform
    scope: scope-cm/emitter
  python/generate:
    plugin: python
    input: cm/transform
    output-artifact: source-file-python
  python/transform:
    input: generate
    output-artifact: source-file-python
    scope: scope-transform-string
  python/emitter:
    input: transform
    scope: scope-python/emitter

scope-python/emitter:
  input-artifact: source-file-python
  output-uri-expr: $key

output-artifact:
- source-file-python
```
