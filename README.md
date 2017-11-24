
# Prerequisites

To use this extenstion you have to install autorest. Just follow instructions here:

https://github.com/Azure/autorest

You also need Azure REST API specification, clone it from here:

https://github.com/Azure/azure-rest-api-specs

and you need to clone this repo as well.


# How to use?

To generate ansibel modules, go to selected directory in REST API spec, for example:

     cd ...\azure-rest-api-specs\specification\sql\resource-manager\

and execute following command:

     autorest --output-folder=[your output directory]\ --use=[your source directory]\autorest.ansible\ --python --tag=package-2017-03-preview

Note that you have to specify location **autorest.ansible** repo, and the plugin should be already built here, either using **npm** or **Visual Studio**.

Also note that **--tag** value comes from **readme.txt** file you can find in your curent directory.

# How it works?

Ansible generator works as follows:

(1) Parses entire REST API definition, just like other autorest plugins

(2) Generates Ansible Module Map which will be later used to generate ansible modules.

(3) Merges newly generated map with old version (if exists in **templates** directory)

(4) Generates Ansible modules and tests in a directory structure matching Ansible directory layout.

(5) ALso generates **xxx.template.json** file and **xxx.merge.txt** file

**xxx.template.json** is an updated (merged) version of new template generated in (2) merged with older version.
You can copy **xxx.template.json** to **templates** directory in **autorest.ansible**.
You can modify almost any values in this file to override values that were automatically generated from REST API.

**xxx.merge.txt** file is a short merge report. It will tell you which fields in the template were overriden.



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