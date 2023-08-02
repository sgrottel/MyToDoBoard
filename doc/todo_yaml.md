# MyToDoBoard - Yaml
MyToDoBoard™ stores all information in a single `yaml` file for easy sync.

```yaml
$format: sgrottel.mytodoboard.yaml.1
$schema: https://go.grottel.net/mytodoboard/schema.yaml

columns:

- name: ToDo
  cards:
  - title: Complex task
    desc: |
      Use the `desc` member to add a string with detailed, human-readable description
    links:
    - https://www.google.de
  - title: Second task
    desc: |
      The order of `cards` within a `column` is to be understood as ordering on the board top to bottom, e.g. a priority.
  - title: Simple task card without description

- name: Doing
  cards:
  - title: Writing an yaml example for MyToDoBoard™
    links:
    - https://github.com/sgrottel/MyToDoBoard

- name: Done
  cards:

```
_This is an example MyToDoBoard™ `yaml` file._

For ease of identification and differentiation to other generic `yaml` files, it is recommended to use the multi-dot file extension: `.mytodo.yaml`

This `yaml` file format is also ment to be stable for data interchange between different tools, apps, in the context of the MyToDoBoard™ project, and beyond.

This minimal example should stay valid for future versions of this tool, maybe limiting some functionality.

## Format Versioning
```yaml
$format: sgrottel.mytodoboard.yaml.1
$schema: https://go.grottel.net/mytodoboard/schema.yaml
```
Both lines are optional.
Loading the file should not fail if one of the lines is missing.

The value of `$format` is a hint to the loading code, which file format to expect.

The value of `$schema` is a live url to a schema file.
This is meant for editors supporting schema files, e.g. for validation or editing support, like auto complete.

## Columns
The vertical columns are mainly lists of task cards.
They also have a meaning, expressed by their `name`, and can have further attributes.
The ordering of the column objects in the top-level `columns` array relates to a "process order," i.e., ToDo -> Doing -> Done.

## Cards
The task card objects are collected in the `cards` array of each column.
Each card represents a logical task, which can be described by it's `title` and `desc`ription, both generic human-readable strings without conflict resolution.

Each card can have a list of `links`, usually live web urls, pointing to additional information.
