#!/bin/bash

cat << EOF > docfx.json
{
  "metadata": [
    {
      "src": [
        {
          "files": ["${DOC_XML_PATH}"],
          "cwd": ".",
          "type": "xml"
        }
      ],
      "dest": "obj/api"
    }
  ],
  "build": {
    "content": [
      {
        "files": ["obj/api/**/*.yml"]
      }
    ],
    "dest": "${DOC_OUTPUT}",
    "globalMetadata": {
      "_enableSearch": "true"
    }
  }
}
EOF
